using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InGameUI : MonoBehaviour
{
    [SerializeField] Button startGame, continueButton, mainMenuButton, quitButton;
    
    [SerializeField] GameObject pausePanel;
    public GameObject mainPanel, loadingPanel;
    
    [SerializeField] TextMeshProUGUI nick1, nick2, nick3;
    public TextMeshProUGUI codeTextField, playerStatusText;

    private bool isPaused = false;

    private Vector3[] stacksPositions = new Vector3[4];

    [SerializeField] MainMenuUI mainMenuUI;
    [SerializeField] Laczenie laczenie;

    private void Start()
    {
        mainPanel.SetActive(false);
        startGame.onClick.AddListener(GameStarter);

        stacksPositions[0] = new Vector3(0, 0.005f, -5.25f);
        stacksPositions[1] = new Vector3(-9, 0.005f, 0);
        stacksPositions[2] = new Vector3(0, 0.005f, 5.25f);
        stacksPositions[4] = new Vector3(9, 0.005f, 0);

    }

    private void OnEnable()
    {
        loadingPanel.SetActive(false);
        pausePanel.SetActive(false);
        playerStatusText.gameObject.SetActive(false);
        nick1.SetText("");
        nick2.SetText("");
        nick3.SetText("");
        if (GameMeneger.instance != null)
        {
            if (!GameMeneger.instance.host)
            {
                startGame.gameObject.SetActive(false);
            }
            else
            {
                startGame.interactable = false;
            }
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if(isPaused)
            {
                isPaused = false;
                pausePanel.SetActive(false);
            }
            else
            {
                isPaused = true;
                pausePanel.SetActive(true);
            }
        }
    }

    public void ChangeButtonInteractable()
    {
        if (!GameMeneger.instance.host)
            return;
        if (GameMeneger.instance.activeGame)
        {
            startGame.gameObject.SetActive(false);
        }
        else
        {
            startGame.gameObject.SetActive(true);
            if (GameMeneger.instance.PlayerCount > 1)
            {
                startGame.interactable = true;
            }
            else
            {
                startGame.interactable = false;
            }
        }
    }

    private void GameStarter()
    {
        startGame.interactable = false;
        loadingPanel.SetActive(true);
        Laczenie.instance.SendMessageToServer("GAME_START%");
    }

    private void ContinueGame()
    {
        isPaused = false;
        pausePanel.SetActive(false);
    }

    private void QuitToMainMenu()
    {
        mainPanel.SetActive(false);
        mainMenuUI.mainPanel.SetActive(true);
        laczenie.CloseConnection();
        mainMenuUI.nick = null;
        mainMenuUI.code = null;
        mainMenuUI.ipAddress = null;
        mainMenuUI.port = null;
    }   
    
    private void QuitGame()
    {
        Application.Quit();
    }

    public void SetNicks()
    {
        List<string> tmp = new List<string>();
        int playersCount = GameMeneger.instance.players.Count;
        for (int i = 0; i < playersCount; i++)
        {
            if(GameMeneger.instance.players[i] == mainMenuUI.nick)
                { break; }
            tmp.Add(GameMeneger.instance.players[i]);
        }
        int ilosc = tmp.Count;
        switch(ilosc)
        {
            case 0:
                GameMeneger.instance.playersCardPositions[0] = stacksPositions[0];
                break;
            case 1:
                nick3.SetText(tmp[0]);
                GameMeneger.instance.playersCardPositions[0] = stacksPositions[3];
                GameMeneger.instance.playersCardPositions[1] = stacksPositions[0];
                break;
            case 2:
                nick2.SetText(tmp[0]);
                nick3.SetText(tmp[1]);
                GameMeneger.instance.playersCardPositions[0] = stacksPositions[2];
                GameMeneger.instance.playersCardPositions[1] = stacksPositions[3];
                GameMeneger.instance.playersCardPositions[2] = stacksPositions[0];
                break;
            case 3:
                nick1.SetText(tmp[0]);
                nick2.SetText(tmp[1]);
                nick3.SetText(tmp[2]);
                GameMeneger.instance.playersCardPositions[0] = stacksPositions[1];
                GameMeneger.instance.playersCardPositions[1] = stacksPositions[2];
                GameMeneger.instance.playersCardPositions[2] = stacksPositions[3];
                GameMeneger.instance.playersCardPositions[3] = stacksPositions[0];
                break;
        }
        int ilosc2 = playersCount - ilosc - 1;
        switch(ilosc2)
        {
            case 0:
                return;
            case 1:
                nick1.SetText(GameMeneger.instance.players[ilosc + 1]);
                GameMeneger.instance.playersCardPositions[ilosc + 1] = stacksPositions[1];
                break;
            case 2:
                nick1.SetText(GameMeneger.instance.players[ilosc + 1]);
                nick2.SetText(GameMeneger.instance.players[ilosc + 2]);
                GameMeneger.instance.playersCardPositions[ilosc + 1] = stacksPositions[1];
                GameMeneger.instance.playersCardPositions[ilosc + 2] = stacksPositions[2];
                break;
            case 3:
                nick1.SetText(GameMeneger.instance.players[ilosc + 1]);
                nick2.SetText(GameMeneger.instance.players[ilosc + 2]);
                nick3.SetText(GameMeneger.instance.players[ilosc + 3]);
                GameMeneger.instance.playersCardPositions[ilosc + 1] = stacksPositions[1];
                GameMeneger.instance.playersCardPositions[ilosc + 2] = stacksPositions[2];
                GameMeneger.instance.playersCardPositions[ilosc + 3] = stacksPositions[3];
                break;
        }
    }
}
