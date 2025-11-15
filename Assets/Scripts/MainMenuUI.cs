using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] GameObject mainPanel, inputUIPanel, invalidIpAddressPanel, invalidPortPanel;
    [SerializeField] Button joinGameButton, startLobbyButton, quitButton, startButton;
    [SerializeField] TMP_InputField nickInput, codeInput, ipAddressInput, portInput;
    private string nick, code, ipAddress, port;
    private readonly string allowedIPString = "0123456789.", allowedNameString = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_+=!*()",
        allowedCodeString = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789", allowedPortString = "0123456789";
    public static MainMenuUI instance;

    private void Start()
    {
        instance = this;

        inputUIPanel.SetActive(false);

        joinGameButton.onClick.AddListener(OpenJoinGamePanel);
        startLobbyButton.onClick.AddListener(OpenStartLobbyPanel);
        quitButton.onClick.AddListener(CloseGame);
    }

    // TODO    ZMIANA TEKSTU PRZYCISKU

    void OpenStartLobbyPanel()
    {
        mainPanel.SetActive(false);
        inputUIPanel.SetActive(true);
        InputUIPanel();
    }

    void OpenJoinGamePanel()
    {
        mainPanel.SetActive(false);
        inputUIPanel.SetActive(true);
        InputUIPanel();
    }

    void InputUIPanel()
    {
        invalidIpAddressPanel.SetActive(false);
        invalidPortPanel.SetActive(false);

        nickInput.characterValidation = TMP_InputField.CharacterValidation.CustomValidator;
        nickInput.onValidateInput += ValidateNameChar;

        codeInput.characterValidation = TMP_InputField.CharacterValidation.CustomValidator;
        codeInput.onValidateInput += ValidateCodeChar;

        ipAddressInput.characterValidation = TMP_InputField.CharacterValidation.CustomValidator;
        ipAddressInput.onValidateInput += ValidateIpAddressChar;

        portInput.characterValidation = TMP_InputField.CharacterValidation.CustomValidator;
        portInput.onValidateInput += ValidatePortChar;

        startButton.onClick.AddListener(StartLobby);
    }    

    void StartLobby()
    {
        //TODO: laczenie z serwerem
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
