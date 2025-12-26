using System;
using TMPro;
using UnityEditor.Build.Content;
using UnityEngine;
using UnityEngine.UIElements;

public class MessageDecoder : MonoBehaviour
{
    [SerializeField] MessageBuffer messageBuffer;
    [SerializeField] MainMenuUI mainMenuUI;
    [SerializeField] TextMeshProUGUI displayText;
    [SerializeField] InGameUI inGameUI;
    [SerializeField] GameEngine gameEngine;
    [SerializeField] RayCasts g1, g2, g3;       // obiekty z raycastem do odkrywania kart innych graczy

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

    private void checkIfTour(int index)     // sprawdzanie czy teraz twoja tura
    {
        int nextIndex = (index + 1) % 4;    // nastepny indeks
        while (GameMeneger.instance.playersTableOrder[nextIndex] == "%")    // pomijamy "pustych" graczy
            nextIndex = (nextIndex + 1) % 4;
        if (nextIndex == 0)     // jezeli nastpeny indkes to 0 -> twoja tura (kazdy gracz u "siebie" jest na indkesie 0)
            GameMeneger.instance.yourTour = true;
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
                GameMeneger.instance.playersTableOrder[0] = mainMenuUI.nick;
                inGameUI.mainPanel.SetActive(true);
                break;

            case "ACCEPT_JOIN":
                mainMenuUI.inputUIPanel.SetActive(false);
                inGameUI.codeTextField.SetText("Code: " + mainMenuUI.code);
                GameMeneger.instance.host = false;
                inGameUI.mainPanel.SetActive(true);
                GameMeneger.instance.activePlayers = int.Parse(parts[2]);
                GameMeneger.instance.PlayerCount = int.Parse(parts[3]);
                for (int i = 0; i < GameMeneger.instance.PlayerCount - 1; i++)
                {
                    GameMeneger.instance.players.Add(parts[i + 4]);
                }
                GameMeneger.instance.players.Add(mainMenuUI.nick);
                
                inGameUI.SetNicks();
                break;

            case "JOIN_ERR":
                mainMenuUI.connectingServerPanel.SetActive(false);
                mainMenuUI.connectionErrorPanel.SetActive(true);
                switch (parts[1])
                {
                    case "INVALID_CODE":
                        displayText.text = "No lobby with given code.\r\nCheck if the code is correct.";
                        break;
                    case "ROOM_FULL":
                        displayText.text = "Lobby full.";
                        break;
                    case "NICK_TAKEN":
                        displayText.text = "Player with that nick already exists.\r\nPleas change your nick.";
                        break;
                }
                break;

            case "ACCEPT_GAME_START":
                GameMeneger.instance.activeGame = true;
                if (GameMeneger.instance.host)
                    GameMeneger.instance.yourTour = true;
                for(int i = 0; i < 4; i++)
                {
                    if(GameMeneger.instance.playersTableOrder[i] != "%")
                    {
                        gameEngine.SpawnStack(i, int.Parse(parts[1]));
                        GameMeneger.instance.activePlayers++;
                    }
                }
                inGameUI.ChangeButtonInteractable();
                inGameUI.loadingPanel.SetActive(false);
                inGameUI.waitingStartPanel.SetActive(false);
                inGameUI.gameStartsPanel.SetActive(true);
                break;

            case "CARD_ID":
                int index = Array.IndexOf(GameMeneger.instance.playersTableOrder, parts[2]);        // indeks gracza ktory odkrywa karte

                if (parts[1] == "-1" || index == -1)        // cos nie tak
                {
                    Debug.LogError("Przyszlo -1 w cardID");
                    break;
                }

                else if (parts[1] == "#")       // pominiecie kolejki
                {
                    checkIfTour(index);
                    break;
                }
                
                CardMovement card;
                if (index == 0)                 // znalezienie odpowiedniej karty
                    card = gameEngine.myCard;
                else if (index == 1 && g1.SendRay())
                    card = g1.card;
                else if (index == 2 && g2.SendRay())
                    card = g2.card;
                else if (index == 3 && g3.SendRay())
                    card = g3.card;
                else
                {
                    Debug.LogError("Nie znalzeiono karty");
                    break;
                }


                if (card != null)
                {
                    GameMeneger.instance.playerDecks[index].hiddenCards.Remove(card);   // Usuñ z zakrytych
                    GameMeneger.instance.playerDecks[index].shownCards.Add(card);       // Dodaj do odkrytych
                    gameEngine.CardMover(parts[1], card);
                }
                else
                    Debug.LogError("Card is null - message decoder");
                checkIfTour(index);
                break;

            case "DECK_SIZE":
                for (int i = 0; i < GameMeneger.instance.activePlayers; i++)        // dla wszystkich aktywnych graczy
                {
                    gameEngine.ClearPlayerStack(i, true, false);        // usuniecie stosu kart zakrytych
                    string player = parts[(i - 1) / 2];                 // nick kolejnego gracza
                    index = Array.IndexOf(GameMeneger.instance.playersTableOrder, player);      // indeks tego gracza
                    if (index >= 0)
                        gameEngine.SpawnStack(index, int.Parse(parts[(i - 2) / 2]));            // spawn nowego stosu kart
                }
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
                string playerDisc = parts[1];
                GameMeneger.instance.players.Remove(playerDisc);
                inGameUI.playerStatusText.SetText(playerDisc + " left");
                inGameUI.playerStatusText.gameObject.SetActive(true);
                inGameUI.SetNicks();
                
                if (GameMeneger.instance.players[0] == mainMenuUI.nick)     // jezeli wyszedl host to sprawdzamy czy nie zostalismy nowym hostem
                {
                    GameMeneger.instance.host = true;
                    inGameUI.ChangeButtonInteractable();
                }

                if (GameMeneger.instance.activeGame)
                    GameMeneger.instance.activePlayers--;

                index = Array.IndexOf(GameMeneger.instance.playersTableOrder, playerDisc);      // usuniecie odkrytych kart gdy wyszedl gracz
                gameEngine.ClearPlayerStack(index, false, true);
                break;

            default:
                Debug.LogWarning($"Nieznana komenda: {command}");
                break;
        }
    }
}
