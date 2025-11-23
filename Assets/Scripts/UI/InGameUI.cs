using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InGameUI : MonoBehaviour
{
    [SerializeField] Button startGame;
    public GameObject mainPanel, loadingPanel;
    [SerializeField] TextMeshProUGUI nick1, nick2, nick3;
    public TextMeshProUGUI codeTextField;

    [SerializeField] MainMenuUI mainMenuUI;

    private void Start()
    {
        mainPanel.SetActive(false);
        startGame.onClick.AddListener(GameStarter);
    }

    private void OnEnable()
    {
        loadingPanel.SetActive(false);
        nick1.SetText("");
        nick2.SetText("");
        nick3.SetText("");
        if (GameMeneger.instance != null)
        {
            if (!GameMeneger.instance.host)
            {
                startGame.gameObject.SetActive(false);
            }
            else
            {
                startGame.interactable = false;
            }
        }
    }

    public void ChangeButtonInteractable()
    {
        if (!GameMeneger.instance.host)
            return;
        if (GameMeneger.instance.activeGame)
        {
            startGame.gameObject.SetActive(false);
        }
        else
        {
            startGame.gameObject.SetActive(true);
            if (GameMeneger.instance.PlayerCount > 1)
            {
                startGame.interactable = true;
            }
            else
            {
                startGame.interactable = false;
            }
        }
    }

    private void GameStarter()
    {
        startGame.interactable = false;
        loadingPanel.SetActive(true);
        Laczenie.instance.SendMessageToServer("GAME_START%");
    }

    public void SetNicks()
    {
        List<string> tmp = new List<string>();
        int playersCount = GameMeneger.instance.players.Count;
        for (int i = 0; i < playersCount; i++)
        {
            if(GameMeneger.instance.players[i] == mainMenuUI.nick)
                { break; }
            tmp.Add(GameMeneger.instance.players[i]);
        }
        int ilosc = tmp.Count;
        switch(ilosc)
        {
            case 0:
                break;
            case 1:
                nick3.SetText(tmp[0]);
                break;
            case 2:
                nick3.SetText(tmp[0]);
                nick2.SetText(tmp[1]);
                break;
            case 3:
                nick3.SetText(tmp[0]);
                nick2.SetText(tmp[1]);
                nick2.SetText(tmp[2]);
                break;
        }
        int ilosc2 = playersCount - ilosc - 1;
        switch(ilosc2)
        {
            case 0:
                return;
            case 1:
                nick1.SetText(GameMeneger.instance.players[ilosc + 1]);
                break;
            case 2:
                nick1.SetText(GameMeneger.instance.players[ilosc + 1]);
                nick2.SetText(GameMeneger.instance.players[ilosc + 2]);
                break;
            case 3:
                nick1.SetText(GameMeneger.instance.players[ilosc + 1]);
                nick2.SetText(GameMeneger.instance.players[ilosc + 2]);
                nick3.SetText(GameMeneger.instance.players[ilosc + 2]);
                break;
        }
    }
}
