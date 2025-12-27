using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] GameObject invalidIpAddressPanel, invalidPortPanel, emptyFieldPanel, codePanel;
    public GameObject inputUIPanel, connectionErrorPanel, connectingServerPanel, mainPanel;
    [SerializeField] Button joinGameButton, startLobbyButton, quitButton, startButton, backButton;
    public TMP_InputField nickInput, codeInput, ipAddressInput, portInput;
    [SerializeField] TextMeshProUGUI startButtonText;
    public string nick, code, ipAddress, port;      // parametry polaczenia
    private readonly string allowedIPString = "0123456789.", allowedNameString = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_+=!*()",
        allowedCodeString = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789", allowedPortString = "0123456789";     // mozliwe znaki do uzycia przy wpisywaniu danych elementow
    private char type;          // typ polaczenia -> dolaczanie do pokoju czy jego tworzenie: j - join, s - start


    private void Start()        // setup
    {
        connectingServerPanel.SetActive(false);
        inputUIPanel.SetActive(false);

        joinGameButton.onClick.AddListener(OpenJoinGamePanel);
        startLobbyButton.onClick.AddListener(OpenStartLobbyPanel);
        quitButton.onClick.AddListener(CloseGame);
    }

    void OpenStartLobbyPanel()
    {
        inputUIPanel.SetActive(true);
        startButtonText.text = "Start";
        codePanel.SetActive(false);         // jezeli tworzymy pokoj to nie podajemy kodu pokoju
        type = 's';
        InputUIPanel();
        startButton.onClick.RemoveAllListeners();
        startButton.onClick.AddListener(StartLobby);
    }

    void OpenJoinGamePanel()
    {
        inputUIPanel.SetActive(true);
        startButtonText.text = "Join";
        type = 'j';
        InputUIPanel();
        startButton.onClick.RemoveAllListeners();
        startButton.onClick.AddListener(StartLobby);
    }

    void InputUIPanel()         // obsluga panelu z polami do wpisywania danych polaczenia
    {
        mainPanel.SetActive(false);
        invalidIpAddressPanel.SetActive(false);
        invalidPortPanel.SetActive(false);
        emptyFieldPanel.SetActive(false);
        connectionErrorPanel.SetActive(false);
        connectingServerPanel.SetActive(false);

        // dodanie odpowiednich walidatorow
        nickInput.characterValidation = TMP_InputField.CharacterValidation.CustomValidator;
        nickInput.onValidateInput += ValidateNameChar;

        codeInput.characterValidation = TMP_InputField.CharacterValidation.CustomValidator;
        codeInput.onValidateInput += ValidateCodeChar;

        ipAddressInput.characterValidation = TMP_InputField.CharacterValidation.CustomValidator;
        ipAddressInput.onValidateInput += ValidateIpAddressChar;

        portInput.characterValidation = TMP_InputField.CharacterValidation.CustomValidator;
        portInput.onValidateInput += ValidatePortChar;
    }

    void StartLobby()       // zaczecie gry
    {
        if (type == 's' && nick.Length != 0 && ipAddress.Length != 0 && port.Length != 0)       // jezeli tworzymy pokoj (kod moze byc pusty)
        {
            Laczenie.instance.ConnectToServer(type);
            connectingServerPanel.SetActive(true);
        }    
        else if (nick.Length != 0 && code.Length != 0 && ipAddress.Length != 0 && port.Length != 0)
        {
            Laczenie.instance.ConnectToServer(type);
            connectingServerPanel.SetActive(true);
        }
        else
        {
            emptyFieldPanel.SetActive(true);        // proba polaczenia z pustym polem
        }
    }

    public void BackToMenu()        // wyjscie do menu
    {
        codePanel.SetActive(true);
        inputUIPanel.SetActive(false);
        mainPanel.SetActive(true);

        nick = null;        // reset parametrow polaczenia
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
        code = input.ToUpper();
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
