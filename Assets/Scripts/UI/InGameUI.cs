using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InGameUI : MonoBehaviour
{
    [SerializeField] Button startGame, continueButton, mainMenuButton, quitButton;
    
    [SerializeField] GameObject pausePanel;
    public GameObject mainPanel, loadingPanel, waitingStartPanel, gameStartsPanel;
    
    [SerializeField] TextMeshProUGUI nick1, nick2, nick3;
    public TextMeshProUGUI codeTextField, playerStatusText;

    private bool isPaused = false;

    [SerializeField] MainMenuUI mainMenuUI;
    [SerializeField] Laczenie laczenie;

    private void Start()
    {
        mainPanel.SetActive(false);
        startGame.onClick.AddListener(GameStarter);
    }

    private void OnEnable()
    {
        loadingPanel.SetActive(false);
        pausePanel.SetActive(false);
        gameStartsPanel.SetActive(false);
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
                waitingStartPanel.SetActive(false);
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
                continueButton.onClick.AddListener(ContinueGame);
                mainMenuButton.onClick.AddListener(QuitToMainMenu);
                quitButton.onClick.AddListener(QuitGame);
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
            waitingStartPanel.SetActive(false);
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
        waitingStartPanel.SetActive(false);
        loadingPanel.SetActive(true);
        Laczenie.instance.SendMessageToServer("GAME_START%");
    }

    public void ContinueGame()
    {
        isPaused = false;
        pausePanel.SetActive(false);
    }

    public void QuitToMainMenu()
    {
        mainPanel.SetActive(false);
        mainMenuUI.mainPanel.SetActive(true);
        laczenie.CloseConnection();
        mainMenuUI.nick = null;
        mainMenuUI.code = null;
        mainMenuUI.ipAddress = null;
        mainMenuUI.port = null;
    }   
    
    public void QuitGame()
    {
        Application.Quit();
    }

    public void SetNicks()
    {
        List<string> tmp = new List<string>();
        int playersCount = GameMeneger.instance.players.Count;

        nick1.SetText("");
        nick2.SetText("");
        nick3.SetText("");

        for(int i = 0; i < 4; i++)
        {
            GameMeneger.instance.playersTableOrder[i] = "%";
        }

        for (int i = 0; i < playersCount; i++)
        {
            if(GameMeneger.instance.players[i] == mainMenuUI.nick)
                { break; }
            tmp.Add(GameMeneger.instance.players[i]);
        }

        GameMeneger.instance.playersTableOrder[0] = mainMenuUI.nick;
        int ilosc = tmp.Count;
        switch(ilosc)
        {
            case 0:
                break;
            case 1:
                nick3.SetText(tmp[0]);
                GameMeneger.instance.playersTableOrder[3] = tmp[0];
                break;
            case 2:
                nick2.SetText(tmp[0]);
                nick3.SetText(tmp[1]);
                GameMeneger.instance.playersTableOrder[2] = tmp[2];
                GameMeneger.instance.playersTableOrder[3] = tmp[3];
                break;
            case 3:
                nick1.SetText(tmp[0]);
                nick2.SetText(tmp[1]);
                nick3.SetText(tmp[2]);
                GameMeneger.instance.playersTableOrder[1] = tmp[1];
                GameMeneger.instance.playersTableOrder[2] = tmp[2];
                GameMeneger.instance.playersTableOrder[3] = tmp[3];
                break;
        }
        int ilosc2 = playersCount - ilosc - 1;
        switch(ilosc2)
        {
            case 0:
                return;
            case 1:
                nick1.SetText(GameMeneger.instance.players[ilosc + 1]);
                GameMeneger.instance.playersTableOrder[1] = GameMeneger.instance.players[ilosc + 1];
                break;
            case 2:
                nick1.SetText(GameMeneger.instance.players[ilosc + 1]);
                nick2.SetText(GameMeneger.instance.players[ilosc + 2]);
                GameMeneger.instance.playersTableOrder[1] = GameMeneger.instance.players[ilosc + 1];
                GameMeneger.instance.playersTableOrder[2] = GameMeneger.instance.players[ilosc + 2];
                break;
            case 3:
                nick1.SetText(GameMeneger.instance.players[ilosc + 1]);
                nick2.SetText(GameMeneger.instance.players[ilosc + 2]);
                nick3.SetText(GameMeneger.instance.players[ilosc + 3]);
                GameMeneger.instance.playersTableOrder[1] = GameMeneger.instance.players[ilosc + 1];
                GameMeneger.instance.playersTableOrder[2] = GameMeneger.instance.players[ilosc + 2];
                GameMeneger.instance.playersTableOrder[3] = GameMeneger.instance.players[ilosc + 3];
                break;
        }
    }
}
