using TMPro;
using UnityEngine;

public class MessageDecoder : MonoBehaviour
{
    [SerializeField] MessageBuffer messageBuffer;
    [SerializeField] MainMenuUI mainMenuUI;
    [SerializeField] TextMeshProUGUI displayText;

    private void Start()
    {
        if (messageBuffer != null)
        {
            messageBuffer.OnCompleteMessage += decodeMessage;
        }
        else
        {
            Debug.LogError("Brak referencji do MessageBuffer w MessageDecoder!");
        }
    }

    private void decodeMessage(string message)
    {
        string[] parts = message.Split(' ');
        string command = parts[0];

        switch(command)
        {
            case "ACCEPT_START":
                mainMenuUI.inputUIPanel.SetActive(false);
                break;
            case "ACCEPT_JOIN":
                mainMenuUI.inputUIPanel.SetActive(false);
                break;
            case "START_ERR":
                mainMenuUI.connectingServerPanel.SetActive(false);
                mainMenuUI.connectionErrorPanel.SetActive(true);
                displayText.text = "Server declained lobby creation.";
                break;
            case "JOIN_ERR":
                mainMenuUI.connectingServerPanel.SetActive(false);
                mainMenuUI.connectionErrorPanel.SetActive(true);
                switch (parts[1])
                {
                    case "0":
                        displayText.text = "No lobby with given code.\r\nCheck if the code is correct.";
                        break;
                    case "1":
                        displayText.text = "Lobby full.";
                        break;
                    case "2":
                        displayText.text = "Player with that nick already exists.\r\nPleas change your nick.";
                        break;
                }
                break;
            case "CARD_ID":
                break;
            case "TOTEM_WON":
                break;
            case "TOTEM_LOST":
                break;
            case "PLAYER_NEW":
                break;
            case "PLAYER_DISC":
                break;
            default:
                Debug.LogWarning($"Nieznana komenda: {command}");
                break;
        }
    }
}
