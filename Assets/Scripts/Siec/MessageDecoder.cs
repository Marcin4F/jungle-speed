using TMPro;
using UnityEngine;

public class MessageDecoder : MonoBehaviour
{
    [SerializeField] MessageBuffer messageBuffer;
    [SerializeField] MainMenuUI mainMenuUI;
    [SerializeField] TextMeshProUGUI displayText;
    [SerializeField] InGameUI inGameUI;

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
            case "ACCEPT_CR_ROOM":
                mainMenuUI.inputUIPanel.SetActive(false);
                inGameUI.codeTextField.SetText("Code: " + parts[1]);
                GameMeneger.instance.host = true;
                GameMeneger.instance.players.Add(mainMenuUI.nick);
                inGameUI.mainPanel.SetActive(true);
                break;
            case "ACCEPT_JOIN":
                mainMenuUI.inputUIPanel.SetActive(false);
                inGameUI.codeTextField.SetText("Code: " + mainMenuUI.code);
                GameMeneger.instance.host = false;
                inGameUI.mainPanel.SetActive(true);
                GameMeneger.instance.activePlayers = int.Parse(parts[2]);
                GameMeneger.instance.PlayerCount = int.Parse(parts[3]);
                for(int i = 0; i < GameMeneger.instance.PlayerCount - 1; i++)
                {
                    GameMeneger.instance.players.Add(parts[i + 4]);
                }
                GameMeneger.instance.players.Add(mainMenuUI.nick);
                inGameUI.SetNicks();
                break;
            case "CR_ROOM_ERR":
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
            case "ACCEPT_GAME_START":
                GameMeneger.instance.activeGame = true;
                inGameUI.ChangeButtonInteractable();
                inGameUI.loadingPanel.SetActive(false);
                break;
            case "DENY_GAME_START":
                break;
            case "CARD_ID":
                break;
            case "TOTEM_WON":
                break;
            case "TOTEM_LOST":
                break;
            case "PLAYER_NEW":
                GameMeneger.instance.PlayerCount++;
                GameMeneger.instance.players.Add(parts[1]);
                inGameUI.playerStatusText.SetText(parts[1] + " joined");
                inGameUI.playerStatusText.gameObject.SetActive(true);
                if(!GameMeneger.instance.activeGame)
                {
                    inGameUI.SetNicks();
                }
                break;
            case "PLAYER_DISC":
                GameMeneger.instance.PlayerCount--;
                GameMeneger.instance.players.Remove(parts[1]);
                inGameUI.playerStatusText.SetText(parts[1] + " left");
                inGameUI.playerStatusText.gameObject.SetActive(true);
                inGameUI.SetNicks();
                break;
            default:
                Debug.LogWarning($"Nieznana komenda: {command}");
                break;
        }
    }
}
