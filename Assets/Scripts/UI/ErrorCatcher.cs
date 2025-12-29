using UnityEngine;
using UnityEngine.UI;

public class ErrorCatcher : MonoBehaviour
{
    public bool errorOccured = false;

    [SerializeField] Button closeGameButton;
    [SerializeField] GameObject errorCatcherPanel;

    public static ErrorCatcher instance;

    private void Start()
    {
        instance = this;
        closeGameButton.onClick.AddListener(closeGame);
        errorCatcherPanel.SetActive(false);
    }

    public void ErrorHandler()
    {
        errorCatcherPanel.SetActive(true);
    }

    private void closeGame()
    {
        Application.Quit();
    }
}
