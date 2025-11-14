using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] GameObject mainPanel, joinGamePanel, startLobbyPanel;
    [SerializeField] Button joinGameButton, startLobbyButton, quitButton, startButton;
    [SerializeField] TMP_InputField nickInput, codeInput, ipAddressInput, portInput;
    private string nick, code, ipAddress, port;

    private void Start()
    {
        joinGamePanel.SetActive(false);
        startLobbyPanel.SetActive(false);

        joinGameButton.onClick.AddListener(OpenJoinGamePanel);
        startLobbyButton.onClick.AddListener(OpenStartLobbyPanel);
        quitButton.onClick.AddListener(CloseGame);
    }

    void OpenJoinGamePanel()
    {
        mainPanel.SetActive(false);
        joinGamePanel.SetActive(true);
    }

    void OpenStartLobbyPanel()
    {
        mainPanel.SetActive(false);
        startLobbyPanel.SetActive(true);

        startButton.onClick.AddListener(StartLobby);
    }

    void StartLobby()
    {
        //TODO: laczenie z serwerem
    }

    public void SetNick(string input)
    {
        nick = input;
        Debug.Log("Ustawiono nick: " + nick);
    }

    public void SetCode(string input)
    {
        code = input;
        Debug.Log("Ustawiono code: " + code);
    }

    public void SetIpAddress(string input)
    {
        ipAddress = input;
        Debug.Log("Ustawiono IP: " + ipAddress);
    }

    public void SetPort(string input)
    {
        port = input;
        Debug.Log("Ustawiono port: " + port);
    }

    void CloseGame()
    {
        Application.Quit();
    }
}
