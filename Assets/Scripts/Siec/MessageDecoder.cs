using Assets.Scripts.Gra;
using Assets.Scripts.UI;
using System;
using System.Collections;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.Siec
{
    public class MessageDecoder : MonoBehaviour
    {
        [SerializeField] MessageBuffer messageBuffer;
        [SerializeField] MainMenuUI mainMenuUI;
        [SerializeField] TextMeshProUGUI displayText;
        [SerializeField] InGameUI inGameUI;
        [SerializeField] GameEngine gameEngine;
        [SerializeField] RayCasts g1, g2, g3;       // obiekty z raycastem do odkrywania kart innych graczy
        [SerializeField] TotemMovement totemMovement;
        [SerializeField] Laczenie laczenie;

        private int index;

        private void Start()
        {
            try
            {
                if (messageBuffer != null)
                {
                    messageBuffer.OnCompleteMessage += DecodeMessage;
                }
                else
                {
                    Debug.LogError("Brak referencji do MessageBuffer w MessageDecoder!");
                }
            }
            catch
            { ErrorCatcher.instance.ErrorHandler(); }
        }

        private static void CheckIfTour(int playerIndex)     // sprawdzanie czy teraz twoja tura
        {
            try
            {
                int nextIndex = (playerIndex + 1) % 4;    // nastepny indeks
                while (GameMeneger.instance.playersTableOrder[nextIndex] == "%")    // pomijamy "pustych" graczy
                    nextIndex = (nextIndex + 1) % 4;
                if (nextIndex == 0)     // jezeli nastpeny indkes to 0 -> twoja tura (kazdy gracz u "siebie" jest na indkesie 0)
                {
                    if (GameMeneger.instance.playersHiddenCards[0] == 0)
                    {
                        for (int i = 0; i < 4; i++)
                        {
                            if (GameMeneger.instance.playersHiddenCards[i] != 0)
                            {
                                Laczenie.instance.SendMessageToServer("CARD_REVEAL%");
                                return;
                            }
                        }
                        return;
                    }
                    GameMeneger.instance.YourTurn = true;
                }
            }
            catch
            { ErrorCatcher.instance.ErrorHandler(); }
        }

        IEnumerator CardCooldown()
        {
            yield return new WaitForSeconds(0.7f);
            CheckIfTour(index);
        }

        private void DecodeMessage(string message)
        {
            try
            {
                string[] parts = message.Split(' ');
                string command = parts[0];

                if (laczenie.isConnected)
                {
                    switch (command)
                    {
                        case "ACCEPT_CR_ROOM":
                            HandleAccperCrRoom(parts);
                            break;

                        case "ACCEPT_JOIN":
                            HandeAcceptJoin(parts);
                            break;

                        case "JOIN_ERR":
                            HandleJoinErr(parts);
                            break;

                        case "ACCEPT_GAME_START":
                            HandleAcceptGameStart(parts);
                            break;

                        case "CARD_ID":
                            HandleCardID(parts);
                            break;

                        case "DECK_SIZE":
                            HandleDeckSize(parts);
                            break;

                        case "FACE_UP_CARDS":
                            HandleFaceUpCards(parts);
                            break;

                        case "TOTEM_WON":
                            HandleTotemWon(parts);
                            break;

                        case "TOTEM_INVALID":
                            HandleTotemInvalid(parts);
                            break;

                        case "PLAYER_NEW":
                            HandlePlayerNew(parts);
                            break;

                        case "PLAYER_DISC":
                            HandlePlayerDisc(parts);
                            break;

                        case "GAME_FINISHED":
                            HandleGameFinished(parts);
                            break;

                        case "GAME_OVER":
                            HandleGameOver();
                            break;

                        default:
                            Debug.LogWarning($"Nieznana komenda: {command}");
                            break;
                    }
                }
            }
            catch
            { ErrorCatcher.instance.ErrorHandler(); }
        }

        private void HandleAccperCrRoom(string[] parts)
        {
            mainMenuUI.inputUIPanel.SetActive(false);
            inGameUI.codeTextField.SetText("Code: " + parts[1]);
            GameMeneger.instance.host = true;
            GameMeneger.instance.isActivePlayer = true;
            GameMeneger.instance.players.Add(mainMenuUI.nick);
            GameMeneger.instance.playersTableOrder[0] = mainMenuUI.nick;
            inGameUI.mainPanel.SetActive(true);
            inGameUI.ChangeButtonInteractable();
        }

        private void HandeAcceptJoin(string[] parts)
        {
            mainMenuUI.inputUIPanel.SetActive(false);
            mainMenuUI.mainPanel.SetActive(false);
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

            if (parts[1] == "1")
            {
                GameMeneger.instance.isActivePlayer = false;
            }

            else
            {
                GameMeneger.instance.isActivePlayer = true;
                inGameUI.waitingStartPanel.SetActive(false);
            }
            inGameUI.SetNicks();
        }

        private void HandleJoinErr(string[] parts)
        {
            mainMenuUI.connectingServerPanel.SetActive(false);
            mainMenuUI.connectionErrorPanel.SetActive(true);
            displayText.text = parts[1] switch
            {
                "INVALID_CODE" => "No lobby with given code or room was closed recently.\r\nCheck if the code is correct.",
                "ROOM_FULL" => "Lobby full.",
                "NICK_TAKEN" => "Player with that nick already exists.\r\nPlease change your nick.",
                _ => "Unknown error.",
            };
        }

        private void HandleAcceptGameStart(string[] parts)
        {
            GameMeneger.instance.activeGame = true;
            if (GameMeneger.instance.host)
                GameMeneger.instance.YourTurn = true;
            for (int i = 0; i < 4; i++)
            {
                if (GameMeneger.instance.playersTableOrder[i] != "%")
                {
                    gameEngine.SpawnStack(i, int.Parse(parts[1]), false, null);
                    GameMeneger.instance.activePlayers++;
                }
            }
            inGameUI.ChangeButtonInteractable();
            inGameUI.loadingPanel.SetActive(false);
            inGameUI.waitingStartPanel.SetActive(false);
            inGameUI.gameOverPanel.SetActive(false);
            inGameUI.gameStartsPanel.SetActive(true);
        }

        private void HandleCardID(string[] parts)
        {
            index = Array.IndexOf(GameMeneger.instance.playersTableOrder, parts[1]);        // indeks gracza ktory odkrywa karte

            if (index == -1)
            {
                Debug.LogError("Nie ma gracza z cardID");
                return;
            }

            if (parts[2] == "-1")        // cos nie tak
            {
                Debug.LogWarning("Przyszlo -1 w cardID");
                if (parts[3] != null && parts[3] == mainMenuUI.nick)
                    GameMeneger.instance.YourTurn = true;
                return;
            }

            else if (parts[2] == "#")       // pominiecie kolejki
            {
                CheckIfTour(index);
                return;
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
                return;
            }


            if (card != null)
            {
                GameMeneger.instance.playerDecks[index].hiddenCards.Remove(card);   // Usuñ z zakrytych
                GameMeneger.instance.playerDecks[index].shownCards.Add(card);       // Dodaj do odkrytych
                gameEngine.CardMover(parts[2], card);
            }
            StartCoroutine(CardCooldown());
        }

        private void HandleDeckSize(string[] parts)
        {
            for (int i = 0; i < GameMeneger.instance.activePlayers; i++)        // dla wszystkich aktywnych graczy
            {
                string player = parts[i * 2 + 1];                   // nick kolejnego gracza
                index = Array.IndexOf(GameMeneger.instance.playersTableOrder, player);      // indeks tego gracza
                if (index >= 0)
                {
                    int cardsNumber = int.Parse(parts[i * 2 + 2]);
                    if (parts[^1] == "DUEL" && GameMeneger.instance.playersHiddenCards[index] != cardsNumber)
                        gameEngine.ClearPlayerStack(index, true, true);
                    else
                        gameEngine.ClearPlayerStack(index, true, false);                // usuniecie stosu kart zakrytych
                    gameEngine.SpawnStack(index, cardsNumber, false, null);      // spawn nowego stosu kart
                }
            }
        }

        private void HandleFaceUpCards(string[] parts)
        {
            for (int i = 0; i < GameMeneger.instance.activePlayers; i++)
            {
                index = Array.IndexOf(GameMeneger.instance.playersTableOrder, parts[i * 3 + 1]);
                gameEngine.SpawnStack(index, int.Parse(parts[i * 3 + 3]), true, parts[i * 3 + 2]);
            }
        }

        private void HandleTotemWon(string[] parts)
        {
            index = Array.IndexOf(GameMeneger.instance.playersTableOrder, parts[1]);
            gameEngine.ClearPlayerStack(index, false, true);
            totemMovement.MoveTotem(index);
            if (parts[2] == mainMenuUI.nick)
                GameMeneger.instance.YourTurn = true;
            else
                GameMeneger.instance.YourTurn = false;
        }

        private void HandleTotemInvalid(string[] parts)
        {
            index = Array.IndexOf(GameMeneger.instance.playersTableOrder, parts[1]);
            for (int i = 0; i < 4; i++)
            {
                gameEngine.ClearPlayerStack(i, false, true);
            }
            totemMovement.MoveTotem(index);
            if (parts[2] == mainMenuUI.nick)
                GameMeneger.instance.YourTurn = true;
            else
                GameMeneger.instance.YourTurn = false;
        }

        private void HandlePlayerNew(string[] parts)
        {
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
        }

        private void HandlePlayerDisc(string[] parts)
        {
            GameMeneger.instance.PlayerCount--;
            string playerDisc = parts[1];

            int discIndex = Array.IndexOf(GameMeneger.instance.playersTableOrder, playerDisc);      // usuniecie odkrytych kart gdy wyszedl gracz
            Debug.Log("INDEKS PRZY WYJSCIU " + discIndex);
            gameEngine.ClearPlayerStack(discIndex, true, true);

            if (discIndex != -1)
            {
                GameMeneger.instance.playersHiddenCards[discIndex] = 0;
                GameMeneger.instance.playersShownCards[discIndex] = 0;
                GameMeneger.instance.playerDecks[discIndex] = new PlayerDeck(); // Reset decku
            }

            if (GameMeneger.instance.players.Contains(playerDisc))      // usuniecie gracza z odowiedniej listy
            {
                GameMeneger.instance.players.Remove(playerDisc);
                if (GameMeneger.instance.activeGame)
                    GameMeneger.instance.activePlayers--;
            }
            else
                GameMeneger.instance.spectators.Remove(playerDisc);

            System.Collections.Generic.Dictionary<string, PlayerDeck> savedDecks = new();
            for (int i = 0; i < 4; i++)
            {
                string pName = GameMeneger.instance.playersTableOrder[i];
                // jesli to nie puste miejsce, nie gracz wychodz¹cy i nie gracz
                if (pName != "%" && pName != playerDisc && i != 0)
                {
                    savedDecks[pName] = GameMeneger.instance.playerDecks[i];

                    // odpinamy deck od starego indeksu zeby ClearPlayerStack go nie zniszczyl
                    GameMeneger.instance.playerDecks[i] = new PlayerDeck();
                    GameMeneger.instance.playersHiddenCards[i] = 0;
                    GameMeneger.instance.playersShownCards[i] = 0;
                }
            }

            inGameUI.playerStatusText.SetText(playerDisc + " left");
            inGameUI.playerStatusText.gameObject.SetActive(true);
            inGameUI.SetNicks();

            for (int i = 1; i < 4; i++) // od 1 bo gracz to 0
            {
                string pName = GameMeneger.instance.playersTableOrder[i];
                if (savedDecks.ContainsKey(pName))
                {
                    // przypisanie decku do nowego indeksu
                    GameMeneger.instance.playerDecks[i] = savedDecks[pName];
                    GameMeneger.instance.playersHiddenCards[i] = savedDecks[pName].hiddenCards.Count;
                    GameMeneger.instance.playersShownCards[i] = savedDecks[pName].shownCards.Count;

                    // FIZYCZNE PRZESUNIECIE KART
                    gameEngine.RelocateDeck(i, savedDecks[pName]);
                }
            }

            if (parts.Length == 3 && parts[2] == mainMenuUI.nick)
                GameMeneger.instance.YourTurn = true;

            if (GameMeneger.instance.players[0] == mainMenuUI.nick)     // jezeli wyszedl host to sprawdzamy czy nie zostalismy nowym hostem
            {
                GameMeneger.instance.host = true;
                inGameUI.ChangeButtonInteractable();
            }
        }

        private void HandleGameFinished(string[] parts)
        {
            string winner = parts[1];
            GameMeneger.instance.winners.Add(winner);
            index = Array.IndexOf(GameMeneger.instance.playersTableOrder, winner);
            GameMeneger.instance.playersTableOrder[index] = "%";
            if (winner == mainMenuUI.nick)
            {
                GameMeneger.instance.isActivePlayer = false;
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
                        if (GameMeneger.instance.activePlayers == 4)
                        {
                            inGameUI.youWonText.GetComponent<AutoHide>().hideDelay = 5;
                            inGameUI.youWonText.SetText("Second place!");
                            inGameUI.youWonText.gameObject.SetActive(true);
                        }
                        break;
                    default:
                        break;
                }
            }
            GameMeneger.instance.activePlayers--;
        }

        private void HandleGameOver()
        {
            inGameUI.youWonText.gameObject.SetActive(false);        // reset parametrow
            GameMeneger.instance.YourTurn = false;
            GameMeneger.instance.activeGame = false;
            GameMeneger.instance.isActivePlayer = true;
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
            GameMeneger.instance.spectators.Clear();
            inGameUI.SetNicks();
            inGameUI.ChangeButtonInteractable();
        }
    }
}