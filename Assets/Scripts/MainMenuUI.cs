using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] GameObject mainPanel, invalidIpAddressPanel, invalidPortPanel, EmptyFieldPanel;
    public GameObject inputUIPanel;
    [SerializeField] Button joinGameButton, startLobbyButton, quitButton, startButton, backButton;
    [SerializeField] TMP_InputField nickInput, codeInput, ipAddressInput, portInput;
    [SerializeField] TextMeshProUGUI startButtonText;
    public string nick, code, ipAddress, port;
    private readonly string allowedIPString = "0123456789.", allowedNameString = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_+=!*()",
        allowedCodeString = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789", allowedPortString = "0123456789";
    private char type;

    [SerializeField] Laczenie laczenie;

    // wywaliæ wpisywanie kodu przy tworzeniu pokoju, kod generowany przez serwer i odsy³any w ACCEPT_START, dodaæ wyœwietlanie kodu w UI gry

    private void Start()
    {
        inputUIPanel.SetActive(false);

        joinGameButton.onClick.AddListener(OpenJoinGamePanel);
        startLobbyButton.onClick.AddListener(OpenStartLobbyPanel);
        quitButton.onClick.AddListener(CloseGame);
    }

    void OpenStartLobbyPanel()
    {
        inputUIPanel.SetActive(true);
        startButtonText.text = "Start";
        type = 's';
        InputUIPanel();
        startButton.onClick.AddListener(StartLobby);
    }

    void OpenJoinGamePanel()
    {
        inputUIPanel.SetActive(true);
        startButtonText.text = "Join";
        type = 'j';
        InputUIPanel();
        startButton.onClick.AddListener(StartLobby);
    }

    void InputUIPanel()
    {
        mainPanel.SetActive(false);
        invalidIpAddressPanel.SetActive(false);
        invalidPortPanel.SetActive(false);
        EmptyFieldPanel.SetActive(false);

        nickInput.characterValidation = TMP_InputField.CharacterValidation.CustomValidator;
        nickInput.onValidateInput += ValidateNameChar;

        codeInput.characterValidation = TMP_InputField.CharacterValidation.CustomValidator;
        codeInput.onValidateInput += ValidateCodeChar;

        ipAddressInput.characterValidation = TMP_InputField.CharacterValidation.CustomValidator;
        ipAddressInput.onValidateInput += ValidateIpAddressChar;

        portInput.characterValidation = TMP_InputField.CharacterValidation.CustomValidator;
        portInput.onValidateInput += ValidatePortChar;
    }

    void StartLobby()
    {
        if (nick.Length != 0 && code.Length != 0 && ipAddress.Length != 0 && port.Length != 0)
        {
            laczenie.ConnectToServer(type);
        }
        else
        {
            EmptyFieldPanel.SetActive(true);
        }
    }

    public void BackToMenu()
    {
        inputUIPanel.SetActive(false);
        mainPanel.SetActive(true);

        nick = null;
        code = null;
        ipAddress = null;
        port = null;

        nickInput.text = null;
        codeInput.text = null;
        ipAddressInput.text = null;
        portInput.text = null;
    }

    void CloseGame()
    {
        Application.Quit();
    }



    // ----------------------------------- Setting input fields -----------------------------------

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
        if (InputValidators.IsValidIP(input))
        {
            ipAddress = input;
            Debug.Log("Ustawiono IP: " + ipAddress);
            invalidIpAddressPanel.SetActive(false);
        }
        else
        {
            invalidIpAddressPanel.SetActive(true);
            ipAddressInput.text = null;
        }
    }

    public void SetPort(string input)
    {
        if (InputValidators.IsValidPort(input))
        {
            port = input;
            Debug.Log("Ustawiono port: " + port);
            invalidPortPanel.SetActive(false);
        }
        else
        {
            invalidPortPanel.SetActive(true);
            portInput.text = null;
        }
    }



    // ----------------------------------- Character input validators -----------------------------------

    private char ValidateNameChar(string text, int charIndex, char addedChar)
    {
        if (allowedNameString.Contains(addedChar))
        {
            return addedChar;
        }
        else
        {
            return '\0';
        }
    }

    private char ValidateCodeChar(string text, int charIndex, char addedChar)
    {
        if (allowedCodeString.Contains(addedChar))
        {
            return addedChar;
        }
        else
        {
            return '\0';
        }
    }

    private char ValidateIpAddressChar(string text, int charIndex, char addedChar)
    {
        if (allowedIPString.Contains(addedChar))
        {
            return addedChar;
        }
        else
        {
            return '\0';
        }
    }

    private char ValidatePortChar(string text, int charIndex, char addedChar)
    {
        if (allowedPortString.Contains(addedChar))
        {
            return addedChar;
        }
        else
        {
            return '\0';
        }
    }
}
