using System;
using System.Linq;
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
    [SerializeField] TotemMovement totemMovement;

    private int index;

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

    private void checkIfTour(int playerIndex)     // sprawdzanie czy teraz twoja tura
    {
        int nextIndex = (playerIndex + 1) % 4;    // nastepny indeks
        while (GameMeneger.instance.playersTableOrder[nextIndex] == "%")    // pomijamy "pustych" graczy
            nextIndex = (nextIndex + 1) % 4;
        if (nextIndex == 0)     // jezeli nastpeny indkes to 0 -> twoja tura (kazdy gracz u "siebie" jest na indkesie 0)
        {
            if (GameMeneger.instance.playersHiddenCards[0] == 0)
            {
                for (int i = 0; i < 4;  i++)
                {
                    if (GameMeneger.instance.playersHiddenCards[i] != 0)
                    {
                        Laczenie.instance.SendMessageToServer("CARD_REVEAL%");
                        return;
                    }
                }
                return;
            }
            GameMeneger.instance.yourTurn = true;
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
                GameMeneger.instance.playersTableOrder[0] = mainMenuUI.nick;
                inGameUI.mainPanel.SetActive(true);
                inGameUI.ChangeButtonInteractable();
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

                if (parts[1] == "1")
                    inGameUI.waitingStartPanel.SetActive(false);
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
                    default:
                        displayText.text = "Unknown error.";
                        break;
                }
                break;

            case "ACCEPT_GAME_START":
                GameMeneger.instance.activeGame = true;
                if (GameMeneger.instance.host)
                    GameMeneger.instance.yourTurn = true;
                for(int i = 0; i < 4; i++)
                {
                    if(GameMeneger.instance.playersTableOrder[i] != "%")
                    {
                        gameEngine.SpawnStack(i, int.Parse(parts[1]), false, null);
                        GameMeneger.instance.activePlayers++;
                    }
                }
                inGameUI.ChangeButtonInteractable();
                inGameUI.loadingPanel.SetActive(false);
                inGameUI.waitingStartPanel.SetActive(false);
                inGameUI.gameStartsPanel.SetActive(true);
                break;

            case "CARD_ID":
                index = Array.IndexOf(GameMeneger.instance.playersTableOrder, parts[1]);        // indeks gracza ktory odkrywa karte

                if (index == -1)
                {
                    Debug.LogError("Nie ma gracza z cardID");
                    break;
                }

                if (parts[2] == "-1")        // cos nie tak
                {
                    Debug.LogWarning("Przyszlo -1 w cardID");
                    if (parts[3] != null && parts[3] == mainMenuUI.nick)
                        GameMeneger.instance.yourTurn = true;
                    break;
                }

                else if (parts[2] == "#")       // pominiecie kolejki
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
                    gameEngine.CardMover(parts[2], card);
                }
                else
                    Debug.LogError("Card is null - message decoder");
                checkIfTour(index);
                break;

            case "DECK_SIZE":
                for (int i = 0; i < GameMeneger.instance.activePlayers; i++)        // dla wszystkich aktywnych graczy
                {
                    string player = parts[i * 2 + 1];                   // nick kolejnego gracza
                    index = Array.IndexOf(GameMeneger.instance.playersTableOrder, player);      // indeks tego gracza
                    if (index >= 0)
                    {
                        int cardsNumber = int.Parse(parts[i * 2 + 2]);
                        if (GameMeneger.instance.playersHiddenCards[index] != cardsNumber)
                            gameEngine.ClearPlayerStack(index, true, true);
                        else
                            gameEngine.ClearPlayerStack(index, true, false);                // usuniecie stosu kart zakrytych
                        gameEngine.SpawnStack(index, cardsNumber, false, null);      // spawn nowego stosu kart
                    }   
                }
                break;

            case "FACE_UP_CARDS":
                for (int i = 0; i < GameMeneger.instance.activePlayers; i++)
                {
                    index = Array.IndexOf(GameMeneger.instance.playersTableOrder, parts[i * 3 + 1]);
                    gameEngine.SpawnStack(index, int.Parse(parts[i * 3 + 3]), true, parts[i * 3 + 2]);
                }
                break;

            case "TOTEM_WON":
                index = Array.IndexOf(GameMeneger.instance.playersTableOrder, parts[1]);
                gameEngine.ClearPlayerStack(index, false, true);
                totemMovement.MoveTotem(index);
                break;

            case "TOTEM_INVALID":
                index = Array.IndexOf(GameMeneger.instance.playersTableOrder, parts[1]);
                for (int i = 0; i < 4; i++)
                {
                    gameEngine.ClearPlayerStack(i, false, true);
                }
                totemMovement.MoveTotem(index);
                break;

            case "PLAYER_NEW":
                GameMeneger.instance.PlayerCount++;
                if (!GameMeneger.instance.activeGame)
                {
                    GameMeneger.instance.players.Add(parts[1]);             // jezeli nie trwa gra to dodajemy gracza do listy graczy
                    inGameUI.SetNicks();
                }
                else
                    GameMeneger.instance.spectators.Add(parts[1]);          // je¿eli trwa gra to dodajemy gracza do listy widzow
                inGameUI.playerStatusText.SetText(parts[1] + " joined");
                inGameUI.playerStatusText.gameObject.SetActive(true);
                break;

            case "PLAYER_DISC":
                GameMeneger.instance.PlayerCount--;
                string playerDisc = parts[1];

                index = Array.IndexOf(GameMeneger.instance.playersTableOrder, playerDisc);      // usuniecie odkrytych kart gdy wyszedl gracz
                gameEngine.ClearPlayerStack(index, true, true);

                if (GameMeneger.instance.players.Contains(playerDisc))      // usuniecie gracza z odowiedniej listy
                {
                    GameMeneger.instance.players.Remove(playerDisc);
                    GameMeneger.instance.activePlayers--;
                }
                else
                    GameMeneger.instance.spectators.Remove(playerDisc);

                inGameUI.playerStatusText.SetText(playerDisc + " left");
                inGameUI.playerStatusText.gameObject.SetActive(true);
                inGameUI.SetNicks();
                
                if (GameMeneger.instance.players[0] == mainMenuUI.nick)     // jezeli wyszedl host to sprawdzamy czy nie zostalismy nowym hostem
                {
                    GameMeneger.instance.host = true;
                    inGameUI.ChangeButtonInteractable();
                }
                break;

            case "GAME_FINISHED":
                GameMeneger.instance.winners.Add(parts[1]);
                index = Array.IndexOf(GameMeneger.instance.playersTableOrder, parts[1]);
                GameMeneger.instance.playersTableOrder[index] = "%";
                GameMeneger.instance.activePlayers--;
                int place = GameMeneger.instance.winners.Count;
                switch (place)
                {
                    case 1:
                        if (GameMeneger.instance.activePlayers >= 3)
                        {
                            inGameUI.youWonText.GetComponent<AutoHide>().hideDelay = 5;
                            inGameUI.youWonText.SetText("You won!");
                            inGameUI.youWonText.gameObject.SetActive(true);
                        }
                        break;
                    case 2:
                        if(GameMeneger.instance.activePlayers == 4)
                        {
                            inGameUI.youWonText.GetComponent<AutoHide>().hideDelay = 5;
                            inGameUI.youWonText.SetText("Second place!");
                            inGameUI.youWonText.gameObject.SetActive(true);
                        }
                        break;
                    default:
                        break;
                }
                break;

            case "GAME_OVER":
                inGameUI.youWonText.gameObject.SetActive(false);        // reset parametrow
                GameMeneger.instance.yourTurn = false;
                GameMeneger.instance.activeGame = false;
                for (int i = 0; i < 4; i++)
                {
                    gameEngine.ClearPlayerStack(i, true, true);
                }
                GameMeneger.instance.activePlayers = 0;

                inGameUI.gameOverPanel.SetActive(true);
                int winners = GameMeneger.instance.winners.Count;
                if (winners == 0)
                {
                    inGameUI.gameWinnersTextField.SetText("Game ended in a draw.");
                }
                else
                {
                    inGameUI.gameWinnersTextField.SetText("WINNERS:\n 1: " + GameMeneger.instance.winners[0]);
                    for (int i = 1; i < winners; i++)
                    {
                        string currentText = inGameUI.gameWinnersTextField.text;
                        inGameUI.gameWinnersTextField.SetText(currentText + "\n" + (i + 1) + ": " + GameMeneger.instance.winners[i]);
                    }
                    GameMeneger.instance.winners.Clear();
                }
                GameMeneger.instance.players.AddRange(GameMeneger.instance.spectators);     // dodanie widzow jako graczy
                inGameUI.SetNicks();
                inGameUI.ChangeButtonInteractable();
                break;

            default:
                Debug.LogWarning($"Nieznana komenda: {command}");
                break;
        }
    }
}
