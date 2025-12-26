using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InGameUI : MonoBehaviour
{
    [SerializeField] Button startGame, continueButton, mainMenuButton, quitButton;
    
    [SerializeField] GameObject pausePanel;
    public GameObject mainPanel, loadingPanel, waitingStartPanel, gameStartsPanel;
    
    [SerializeField] TextMeshProUGUI nick1, nick2, nick3;       // wyswietlane nicki graczy
    public TextMeshProUGUI codeTextField, playerStatusText;     // code -> kod pokoju

    private bool isPaused = false;

    [SerializeField] MainMenuUI mainMenuUI;
    [SerializeField] Laczenie laczenie;

    private void Start()
    {
        mainPanel.SetActive(false);
        startGame.onClick.AddListener(GameStarter);
        continueButton.onClick.AddListener(ContinueGame);
        mainMenuButton.onClick.AddListener(QuitToMainMenu);
        quitButton.onClick.AddListener(QuitGame);
    }

    private void OnEnable()         // poczatkowy setup
    {
        loadingPanel.SetActive(false);      // ukrycie wszystkich paneli i reset nickow
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
        if (Input.GetKeyDown(KeyCode.Escape))       // menu w trakcie gry
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

    public void ChangeButtonInteractable()          // zmiana dostepnosci przycisku do rozpoczynania gry
    {
        if (!GameMeneger.instance.host)             // jezeli nie jestes hostem to zakoncz
            return;
        if (GameMeneger.instance.activeGame)        // jezeli trwa gra to ukryj przycisk
        {
            startGame.gameObject.SetActive(false);
        }
        else
        {
            startGame.gameObject.SetActive(true);       // jezeli nie trwa gra to pokaz przycisk (ukryj wiadomosc o czekaniu na hosta bo ty jestes hostem)
            waitingStartPanel.SetActive(false);
            if (GameMeneger.instance.PlayerCount > 1)   // jezeli jest wiecej niz 1 gracz to daj mozliwosc zaczecia gry
            {
                startGame.interactable = true;
            }
            else
            {
                startGame.interactable = false;
            }
        }
    }

    private void GameStarter()      // rozpoczecie gry przez hosta
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

    public void QuitToMainMenu()        // wyjscie do menu
    {
        mainPanel.SetActive(false);
        mainMenuUI.mainPanel.SetActive(true);
        laczenie.CloseConnection();     // rozlaczenie z serwerem
        mainMenuUI.nick = null;         // wyczyszczenie parametrow polaczenia
        mainMenuUI.code = null;
        mainMenuUI.ipAddress = null;
        mainMenuUI.port = null;
    }   
    
    public void QuitGame()      // wyjscie z gry
    {
        Application.Quit();
    }

    public void SetNicks()      // ustawienie nickow graczy w odpowiedniej kolejnosci
    {
        List<string> tmp = new List<string>();      // lista pomocnicza
        int playersCount = GameMeneger.instance.players.Count;

        nick1.SetText("");      // wyczyszczenie nickow
        nick2.SetText("");
        nick3.SetText("");

        for(int i = 0; i < 4; i++)      // wyczyszczenie tablicy z danym graczem na pozycji 0
        {
            GameMeneger.instance.playersTableOrder[i] = "%";
        }

        for (int i = 0; i < playersCount; i++)      // poszukiwanie "swojego" nicku na liscie z nickami graczy
        {
            if(GameMeneger.instance.players[i] == mainMenuUI.nick)
                { break; }
            tmp.Add(GameMeneger.instance.players[i]);       // zapisywanie wszystkich innych graczy -> byli oni przed dolaczeniem tego gracza -> nicki dodawane na prawo w odwrotnej kolejnosci
        }

        GameMeneger.instance.playersTableOrder[0] = mainMenuUI.nick;        // dodanie nicku gracza na pierwsza pozycje tablicy TableOrder
        int ilosc = tmp.Count;
        switch(ilosc)           // inne rozstawienie w zaleznosci ilu graczy jest przed naszym
        {
            case 0:
                break;
            case 1:
                nick3.SetText(tmp[0]);                                  // ustawienie nickow i 
                GameMeneger.instance.playersTableOrder[3] = tmp[0];     // wpisanie graczy na odpowiednie pole w tablicy TableOrder
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
        int ilosc2 = playersCount - ilosc - 1;      // ilu graczy jeszcze zostalo do wpisania -> gracze ktorzy dolaczyli po nas -> dodajemy normalnie od lewej
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
