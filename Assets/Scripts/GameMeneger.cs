using System.Collections.Generic;
using UnityEngine;

// TODO wyk³adanie karty dobrego gracza, sprawdzanie tury graczy (po CARD_ID), start game nie jest widoczny na poczatku, ustawianie pozycji w InGameUI?
// ZROBIONE DO TESTOW: odswierzanie kto jest hostem, panel z zaczynaniem gry, menuPauzy przyciski

public class GameMeneger : MonoBehaviour
{
    public static GameMeneger instance;

    [SerializeField] InGameUI inGameUI;

    public bool host = false;
    public bool activeGame = false;
    public bool yourTour = false;
    private int _playerCount = 1;
    public int activePlayers = 0;
    public List<string> players = new List<string>();
    public string[] playersTableOrder = new string[4];
    public List<int> playersHiddenCards = new List<int>();
    public List<int> playersShownCards = new List<int>();
    public List<Vector3> playersCardPositions = new List<Vector3>();

    public int PlayerCount 
    { 
        get { return _playerCount; }

        set
        {
            if (_playerCount != value)
            {
                _playerCount = value;

                if (host)
                {
                    inGameUI.ChangeButtonInteractable();
                }
            }
        }
    }

    private void Awake()
    {
        instance = this;
        playersCardPositions.Add(new Vector3(0, 0.005f, -5.25f));
        playersCardPositions.Add(new Vector3(-9, 0.005f, 0));
        playersCardPositions.Add(new Vector3(0, 0.005f, 5.25f));
        playersCardPositions.Add(new Vector3(9, 0.005f, 0));
        for (int i = 0; i < 4;  i++)
        {
            playersShownCards.Add(0);
            playersHiddenCards.Add(0);
            playersTableOrder[i] = "%";
        }
    }
}
