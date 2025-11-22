using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InGameUI : MonoBehaviour
{
    [SerializeField] Button startGame;
    public GameObject mainPanel, loadingPanel;
    [SerializeField] TextMeshProUGUI nick1, nick2, nick3;
    public TextMeshProUGUI codeTextField;

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
        switch(GameMeneger.instance.PlayerCount)
        {
            case 1:
                return;
            case 2:
                nick1.SetText(GameMeneger.instance.players[0]);
                break;
            case 3:
                nick1.SetText(GameMeneger.instance.players[0]);
                nick2.SetText(GameMeneger.instance.players[1]);
                break;
            case 4:
                nick1.SetText(GameMeneger.instance.players[0]);
                nick2.SetText(GameMeneger.instance.players[1]);
                nick3.SetText(GameMeneger.instance.players[2]);
                break;
        }
    }
}
