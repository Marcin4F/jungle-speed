using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class ErrorCatcher : MonoBehaviour
    {
        public bool errorOccured = false;

        [SerializeField] Button closeGameButton;
        [SerializeField] GameObject errorCatcherPanel;

        public static ErrorCatcher instance;

        private void Start()
        {
            instance = this;
            closeGameButton.onClick.AddListener(CloseGame);
            errorCatcherPanel.SetActive(false);
        }

        public void ErrorHandler()
        {
            errorCatcherPanel.SetActive(true);
        }

        private static void CloseGame()
        {
            Application.Quit();
        }
    }
}