using System.Collections.Generic;
using UnityEngine;

// TODO 

// DO TESTOW: zamiana argumentow w CARD_ID, zmiana miejsca ustawiania tagu CARD_USED,


[System.Serializable]
public class PlayerDeck     // klasa z listami przechowujacymi karty graczy (odkryte i zakryte)
{
    public List<CardMovement> hiddenCards = new List<CardMovement>();
    public List<CardMovement> shownCards = new List<CardMovement>();
}

public class GameMeneger : MonoBehaviour
{
    public static GameMeneger instance;

    [SerializeField] InGameUI inGameUI;

    public bool host = false;               // czy gracz jest hostem
    public bool activeGame = false;         // czy trwa gra
    public bool _yourTurn = false;           // czy twoja tura
    public bool isActivePlayer = false;
    private int _playerCount = 1;           // ilosc graczy (1 bo jestes ty)
    public int activePlayers = 0;           // ilosc aktywnych graczy, 0 bo na poczatku nie trwa gra
    public List<string> players = new List<string>();           // gracze (kolejnosc jaka widzi serwer -> kolejnosc dolaczania do pokoju)
    public string[] playersTableOrder = new string[4];          // gracze (kolejnosc stolikowa, zaczynajac od nas)
    public List<string> winners = new List<string>();           // lista zwyciezcow w kolejnosc
    public List<string> spectators = new List<string>();        // widzowie
    public List<int> playersHiddenCards = new List<int>();      // ilosc ukrytych kart kazdego gracza
    public List<int> playersShownCards = new List<int>();       // ilosc odkrytych kart kazdego gracza
    public List<Vector3> playersCardPositions = new List<Vector3>();        // pozycje kazdego stosu (zakryte)
    public PlayerDeck[] playerDecks = new PlayerDeck[4];                    // tablica z klasami playerDeck

    public int PlayerCount      // ilosc graczy
    { 
        get { return _playerCount; }

        set
        {
            if (_playerCount != value)
            {
                _playerCount = value;

                if (host)
                    inGameUI.ChangeButtonInteractable();        // zmiana dostepnosci przycisku start game przy zmianie ilosci graczy (jezeli jestes hostem)
            }
        }
    }

    public bool yourTurn        // czy twoja tura
    {
        get { return _yourTurn; }

        set
        {
            if (_yourTurn != value)
            {
                _yourTurn = value;
                if(_yourTurn)
                {
                    inGameUI.youWonText.GetComponent<AutoHide>().hideDelay = 1;
                    inGameUI.youWonText.SetText("Your turn");
                    inGameUI.youWonText.gameObject.SetActive(true);
                }
            }
        }
    }

    private void Awake()        // setup
    {
        instance = this;
        playersCardPositions.Add(new Vector3(0, 0.005f, -5.25f));       // pozycje stosow kart
        playersCardPositions.Add(new Vector3(-9, 0.005f, 0));
        playersCardPositions.Add(new Vector3(0, 0.005f, 5.25f));
        playersCardPositions.Add(new Vector3(9, 0.005f, 0));
        for (int i = 0; i < 4;  i++)
        {
            playersShownCards.Add(0);       // na poczatku nie ma kart
            playersHiddenCards.Add(0);
            playersTableOrder[i] = "%";     // wypelnienie playersTableOrder "pustymi" graczami -> symbol '%'
            playerDecks[i] = new PlayerDeck();      // dodanie pustych deckow kart dla kazdego gracza
        }
    }

    public void ResetParameters()
    {
        host = false;
        activeGame = false;
        isActivePlayer = false;
        yourTurn = false;
        _playerCount = 1;
        activePlayers = 0;
        players.Clear();
        playersHiddenCards.Clear();
        playersShownCards.Clear();
        winners.Clear();
        spectators.Clear();

        for (int i = 0; i < 4; i++)
        {
            playersTableOrder[i] = "%";
            playerDecks[i].hiddenCards.Clear();
            playerDecks[i].shownCards.Clear();
            playersShownCards.Add(0);
            playersHiddenCards.Add(0);
        }
    }
}
