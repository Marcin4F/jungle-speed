using System.Collections.Generic;
using UnityEngine;


public class GameMeneger : MonoBehaviour
{
    public static GameMeneger instance;

    [SerializeField] InGameUI inGameUI;

    public bool host = false;
    public bool activeGame = false;
    private int _playerCount = 1;
    public int activePlayers = 0;
    public List<string> players = new List<string>();

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
    }
}
