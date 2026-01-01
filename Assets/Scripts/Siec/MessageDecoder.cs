using System;
using System.Collections;
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
    [SerializeField] Laczenie laczenie;

    private int index;

    private void Start()
    {
        try
        {
            if (messageBuffer != null)
            {
                messageBuffer.OnCompleteMessage += decodeMessage;
            }
            else
            {
                Debug.LogError("Brak referencji do MessageBuffer w MessageDecoder!");
            }
        } catch
        { ErrorCatcher.instance.ErrorHandler(); }
    }

    private void checkIfTour(int playerIndex)     // sprawdzanie czy teraz twoja tura
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
                GameMeneger.instance.yourTurn = true;
            }
        } catch
        { ErrorCatcher.instance.ErrorHandler(); }
    }

    IEnumerator CardCooldown()
    {
        yield return new WaitForSeconds(0.7f);
        checkIfTour(index);
    }

    private void decodeMessage(string message)
    {
        try
        {
            string[] parts = message.Split(' ');
            string command = parts[0];

            switch (command)
            {
                case "ACCEPT_CR_ROOM":
                    if (laczenie.isConnected)
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
                    break;

                case "ACCEPT_JOIN":
                    if (laczenie.isConnected)
                    {
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
                    break;

                case "JOIN_ERR":
                    if (laczenie.isConnected)
                    {
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
                                displayText.text = "Player with that nick already exists.\r\nPlease change your nick.";
                                break;
                            default:
                                displayText.text = "Unknown error.";
                                break;
                        }
                    }
                    break;

                case "ACCEPT_GAME_START":
                    if (laczenie.isConnected)
                    {
                        GameMeneger.instance.activeGame = true;
                        if (GameMeneger.instance.host)
                            GameMeneger.instance.yourTurn = true;
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
                    break;

                case "CARD_ID":
                    if (laczenie.isConnected)
                    {
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
                        StartCoroutine(CardCooldown());
                    }
                    break;

                case "DECK_SIZE":
                    if (laczenie.isConnected)
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
                    break;

                case "FACE_UP_CARDS":
                    if (laczenie.isConnected)
                    {
                        for (int i = 0; i < GameMeneger.instance.activePlayers; i++)
                        {
                            index = Array.IndexOf(GameMeneger.instance.playersTableOrder, parts[i * 3 + 1]);
                            gameEngine.SpawnStack(index, int.Parse(parts[i * 3 + 3]), true, parts[i * 3 + 2]);
                        }
                    }
                    break;

                case "TOTEM_WON":
                    if (laczenie.isConnected)
                    {
                        index = Array.IndexOf(GameMeneger.instance.playersTableOrder, parts[1]);
                        gameEngine.ClearPlayerStack(index, false, true);
                        totemMovement.MoveTotem(index);
                        if (parts[2] == mainMenuUI.nick)
                            GameMeneger.instance.yourTurn = true;
                        else
                            GameMeneger.instance.yourTurn = false;
                    }
                    break;

                case "TOTEM_INVALID":
                    if (laczenie.isConnected)
                    {
                        index = Array.IndexOf(GameMeneger.instance.playersTableOrder, parts[1]);
                        for (int i = 0; i < 4; i++)
                        {
                            gameEngine.ClearPlayerStack(i, false, true);
                        }
                        totemMovement.MoveTotem(index);
                        if (parts[2] == mainMenuUI.nick)
                            GameMeneger.instance.yourTurn = true;
                        else
                            GameMeneger.instance.yourTurn = false;
                    }
                    break;

                case "PLAYER_NEW":
                    if (laczenie.isConnected)
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
                    break;

                case "PLAYER_DISC":
                    if (laczenie.isConnected)
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

                        // ZMIANY DO TESTOW
                        System.Collections.Generic.Dictionary<string, PlayerDeck> savedDecks = new System.Collections.Generic.Dictionary<string, PlayerDeck>();
                        for (int i = 0; i < 4; i++)
                        {
                            string pName = GameMeneger.instance.playersTableOrder[i];
                            // Jeœli to nie puste miejsce, nie gracz wychodz¹cy i nie my (nasze miejsce jest sta³e - indeks 0)
                            if (pName != "%" && pName != playerDisc && i != 0)
                            {
                                savedDecks[pName] = GameMeneger.instance.playerDecks[i];

                                // Wa¿ne: Odpinamy deck od starego indeksu, ¿eby ClearPlayerStack go nie zniszczy³ przypadkiem
                                // lub ¿eby nie zosta³ nadpisany pustym deckiem.
                                GameMeneger.instance.playerDecks[i] = new PlayerDeck();
                                GameMeneger.instance.playersHiddenCards[i] = 0;
                                GameMeneger.instance.playersShownCards[i] = 0;
                            }
                        }

                        inGameUI.playerStatusText.SetText(playerDisc + " left");
                        inGameUI.playerStatusText.gameObject.SetActive(true);
                        inGameUI.SetNicks();

                        for (int i = 1; i < 4; i++) // Pêtla od 1, bo my (0) siê nie zmieniamy
                        {
                            string pName = GameMeneger.instance.playersTableOrder[i];
                            if (savedDecks.ContainsKey(pName))
                            {
                                // Przypisujemy zachowany deck do nowego indeksu
                                GameMeneger.instance.playerDecks[i] = savedDecks[pName];

                                // Aktualizujemy liczniki
                                GameMeneger.instance.playersHiddenCards[i] = savedDecks[pName].hiddenCards.Count;
                                GameMeneger.instance.playersShownCards[i] = savedDecks[pName].shownCards.Count;

                                // FIZYCZNE PRZESUNIÊCIE KART
                                gameEngine.RelocateDeck(i, savedDecks[pName]);
                            }
                        }

                        if (parts.Length == 3 && parts[2] == mainMenuUI.nick)
                            GameMeneger.instance.yourTurn = true;

                        if (GameMeneger.instance.players[0] == mainMenuUI.nick)     // jezeli wyszedl host to sprawdzamy czy nie zostalismy nowym hostem
                        {
                            GameMeneger.instance.host = true;
                            inGameUI.ChangeButtonInteractable();
                        }
                    }
                    break;

                case "GAME_FINISHED":
                    if (laczenie.isConnected)
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
                    break;

                case "GAME_OVER":
                    if (laczenie.isConnected)
                    {
                        inGameUI.youWonText.gameObject.SetActive(false);        // reset parametrow
                        GameMeneger.instance.yourTurn = false;
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
                    break;

                default:
                    Debug.LogWarning($"Nieznana komenda: {command}");
                    break;
            }
        } catch
        { ErrorCatcher.instance.ErrorHandler(); }
        
    }
}
