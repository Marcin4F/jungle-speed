using UnityEngine;

public class GameEngine : MonoBehaviour
{
    [SerializeField] GameObject card;
    private GameObject spawnedCard;

    [SerializeField] MainMenuUI mainMenuUI;
    public CardMovement myCard;     // karta trafiona raycastem po kliknieciu mysza

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && GameMeneger.instance.activeGame && GameMeneger.instance.yourTour)
        {
            FireScreenRay();
        }

        if (Input.GetMouseButtonDown(1))
        {
            SpawnStack(0, 50);
        }
    }

    void FireScreenRay()
    {
        Ray cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(cameraRay, out RaycastHit hitInfo))
        {
            if (hitInfo.collider.gameObject.tag == "Card")
            {
                hitInfo.collider.gameObject.tag = "UsedCard";
                myCard = hitInfo.collider.GetComponent<CardMovement>();
                Laczenie.instance.SendMessageToServer("CARD_REVEAL%");
                GameMeneger.instance.yourTour = false;
            }
        }
    }

    public void CardMover(string id, CardMovement cardToMove)
    {
        if (cardToMove != null)
        {
            Renderer[] childRenderers = cardToMove.GetComponentsInChildren<Renderer>();
            foreach (Renderer r in childRenderers)
            {
                if (r.gameObject.CompareTag("DisplayCard"))
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
            cardToMove.MoveCard();
        }
    }

    public void SpawnCard(int id)
    {
        spawnedCard = Instantiate(card);
        CardMovement cardMovement = spawnedCard.GetComponent<CardMovement>();

        Vector3 cardPosition = GameMeneger.instance.playersCardPositions[id];
        spawnedCard.transform.position = new Vector3 (cardPosition.x, cardPosition.y + GameMeneger.instance.playersHiddenCards[id] * 0.01f,
            cardPosition.z);
        GameMeneger.instance.playersHiddenCards[id]++;
        GameMeneger.instance.playerDecks[id].hiddenCards.Add(cardMovement);
    }

    public void SpawnStack(int id, int number)
    {
        for (int i = 0; i < number; i++)
        {
            SpawnCard(id);
        }
    }

    public void ClearPlayerStack (int playerId, bool clearHidden, bool clearShown)
    {
        PlayerDeck deck = GameMeneger.instance.playerDecks[playerId];

        if (clearHidden)
        {
            foreach (var card in deck.hiddenCards)
            {
                if (card != null) 
                    Destroy(card.gameObject); // Usuñ obiekt ze sceny
            }
            deck.hiddenCards.Clear(); // Wyczyœæ listê
            GameMeneger.instance.playersHiddenCards[playerId] = 0; // Zresetuj licznik
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
}
