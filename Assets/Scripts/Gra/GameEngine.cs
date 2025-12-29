using UnityEditor.Build.Content;
using UnityEditor.PackageManager;
using UnityEngine;

public class GameEngine : MonoBehaviour
{
    [SerializeField] GameObject card;
    private GameObject spawnedCard;

    [SerializeField] MainMenuUI mainMenuUI;
    public CardMovement myCard;     // karta trafiona raycastem po kliknieciu mysza

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && GameMeneger.instance.activeGame && GameMeneger.instance.yourTurn)    // odkrycie karty - tylko jezeli trwa gra i twoja tura
        {
            FireScreenRay();
        }

        else if (Input.GetKeyDown(KeyCode.Space) && GameMeneger.instance.activeGame)       // proba chwycenia totemu
        {
            Laczenie.instance.SendMessageToServer("TOTEM%");
        }
    }

    private void FireScreenRay()
    {
        Ray cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(cameraRay, out RaycastHit hitInfo))
        {
            if (hitInfo.collider.gameObject.tag == "Card")      // sprawdzamy czy to karta ktora mozna ruszyc
            {
                myCard = hitInfo.collider.GetComponent<CardMovement>();     // zapisujemy obiekt ktory trafil raycast
                Laczenie.instance.SendMessageToServer("CARD_REVEAL%");      // komunikat do serwera
                GameMeneger.instance.yourTurn = false;                      // zakonczenie tury -> brak spamu do serwera
            }
        }
    }

    public void CardMover(string id, CardMovement cardToMove)       // przygotowanie karty do ruchu
    {
        if (cardToMove != null)
        {
            AddTextureToCard(id, cardToMove);   // dodanie tekstury do karty
            cardToMove.MoveCard();              // ruch karty
        }
    }

    public void SpawnCard(int id, bool visable, string CardID)       // spawn kart (id -> numer gracza wedlug TableOrder, visable -> czy stos odkrytych (true -> tak), CardID -> numer tekstury (!tylko je¿eli visable == true)
    {
        spawnedCard = Instantiate(card);
        CardMovement cardMovement = spawnedCard.GetComponent<CardMovement>();       // uzyskanie skryptu cardMovement dla nowej karty (potrzebne przy usuwaniu kart)
        Vector3 cardPosition = GameMeneger.instance.playersCardPositions[id];       // przesuniecie karty na odpowiednia pozycje (Y wyliczany zgodnie z iloscia kart na stosie)

        if (!visable)
        {
            spawnedCard.transform.position = new Vector3(cardPosition.x, cardPosition.y + GameMeneger.instance.playersHiddenCards[id] * 0.01f, cardPosition.z);
            GameMeneger.instance.playersHiddenCards[id]++;          // zwiekszenie ilosc zakrytych kart
            GameMeneger.instance.playerDecks[id].hiddenCards.Add(cardMovement);         // dodanie obiektu karty do listy
        }

        else
        {
            AddTextureToCard(CardID, cardMovement);

            cardPosition.y += GameMeneger.instance.playersShownCards[id] * 0.01f;
            Vector3 targetRotation = new Vector3 (0, 0, 0);
            switch (id)
            {
                case 0:
                    cardPosition += new Vector3(0, 0, 2.55f);
                    targetRotation += new Vector3(180, 0, 0);
                    break;
                case 1:
                    cardPosition += new Vector3(2.55f, 0, 0);
                    targetRotation += new Vector3(180, 0, 0);
                    break;
                case 2:
                    cardPosition += new Vector3(0, 0, -2.55f);
                    targetRotation += new Vector3(0, 0, 180);
                    break;
                case 3:
                    cardPosition += new Vector3(-2.55f, 0, 0);
                    targetRotation += new Vector3(0, 0, 180);
                    break;
            }
            spawnedCard.transform.position = cardPosition;
            spawnedCard.transform.rotation = Quaternion.Euler(targetRotation);
            GameMeneger.instance.playersShownCards[id]++;
            GameMeneger.instance.playerDecks[id].shownCards.Add(cardMovement);
        }
    }

    public void SpawnStack(int id, int number, bool stack, string cardID)      // spawn stosu kart o zadanej ilosci
    {
        for (int i = 0; i < number; i++)
        {
            SpawnCard(id, stack, cardID);
        }
    }

    public void ClearPlayerStack (int playerId, bool clearHidden, bool clearShown)      // usuwanie stosu kart dla danego gracza
    {
        if (playerId == -1)
            { return; }
        PlayerDeck deck = GameMeneger.instance.playerDecks[playerId];       // wczytanie klasy ze stosami danego gracza

        if (clearHidden)
        {
            foreach (var card in deck.hiddenCards)
            {
                if (card != null)
                {
                    Destroy(card.gameObject);           // Usuñ obiekt ze sceny
                }
            }
            deck.hiddenCards.Clear();                   // Wyczyœæ listê
            GameMeneger.instance.playersHiddenCards[playerId] = 0;      // Zresetuj licznik
        }

        if (clearShown)
        {
            foreach (var card in deck.shownCards)
            {
                if (card != null) 
                    Destroy(card.gameObject);
            }
            deck.shownCards.Clear();
            GameMeneger.instance.playersShownCards[playerId] = 0;
        }
    }

    private void AddTextureToCard(string id, CardMovement cardToMove)
    {
        cardToMove.gameObject.tag = "UsedCard";
        Renderer[] childRenderers = cardToMove.GetComponentsInChildren<Renderer>();     // renderer dzieci karty
        foreach (Renderer r in childRenderers)
        {
            if (r.gameObject.CompareTag("DisplayCard"))     // uzyskanie strony, ktora ma wyswietlac symbol
            {
                Texture2D newTexture = Resources.Load<Texture2D>(id);   // wczytanie tesktury

                if (newTexture != null)
                {
                    r.material.SetTexture("_BaseMap", newTexture);      // ustawienie tekstury karty
                }
                else
                {
                    Debug.LogWarning("No texture for ID: " + id);
                }
                break;
            }
        }
    }

    public void RelocateDeck(int newId, PlayerDeck deck)
    {
        // 1. Przesuniêcie kart zakrytych (Hidden)
        Vector3 hiddenBasePos = GameMeneger.instance.playersCardPositions[newId];
        for (int i = 0; i < deck.hiddenCards.Count; i++)
        {
            if (deck.hiddenCards[i] != null)
            {
                // Ustawienie pozycji (karty uk³adane jedna na drugiej)
                deck.hiddenCards[i].transform.position = new Vector3(
                    hiddenBasePos.x,
                    hiddenBasePos.y + (i * 0.01f),
                    hiddenBasePos.z
                );
                // Karty zakryte zawsze le¿¹ "plecami" do góry (mo¿esz tu dostosowaæ rotacjê jeœli masz specyficzn¹ dla graczy)
                deck.hiddenCards[i].transform.rotation = Quaternion.Euler(0, 0, 180);
            }
        }

        // 2. Przesuniêcie kart odkrytych (Shown)
        Vector3 shownBasePos = GameMeneger.instance.playersCardPositions[newId];

        // Obliczamy przesuniêcie i rotacjê zale¿nie od miejsca przy stole (logika skopiowana z SpawnCard)
        Vector3 targetRotation = Vector3.zero;
        Vector3 offset = Vector3.zero;

        switch (newId)
        {
            case 0: // My
                offset = new Vector3(0, 0, 2.55f);
                targetRotation = new Vector3(180, 0, 0);
                break;
            case 1: // Lewy
                offset = new Vector3(2.55f, 0, 0);
                targetRotation = new Vector3(180, 0, 0);
                break;
            case 2: // Góra
                offset = new Vector3(0, 0, -2.55f);
                targetRotation = new Vector3(0, 0, 180);
                break;
            case 3: // Prawy
                offset = new Vector3(-2.55f, 0, 0);
                targetRotation = new Vector3(0, 0, 180);
                break;
        }

        shownBasePos += offset;

        for (int i = 0; i < deck.shownCards.Count; i++)
        {
            if (deck.shownCards[i] != null)
            {
                deck.shownCards[i].transform.position = new Vector3(
                    shownBasePos.x,
                    shownBasePos.y + (i * 0.01f),
                    shownBasePos.z
                );
                deck.shownCards[i].transform.rotation = Quaternion.Euler(targetRotation);
            }
        }
    }
}
