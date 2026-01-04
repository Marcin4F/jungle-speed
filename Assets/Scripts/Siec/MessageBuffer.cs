using Assets.Scripts.UI;
using System;
using System.Collections.Concurrent;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Siec
{
    public class MessageBuffer : MonoBehaviour
    {
        private readonly ConcurrentQueue<string> _incomingQueue = new();
        private readonly StringBuilder _processingBuffer = new();

        public event Action<string> OnCompleteMessage;

        private void Start()
        {
            Laczenie.instance.OnMessageReceived += OnMessage;
        }


        private void OnMessage(string receivedMessage)
        {
            try
            {
                _incomingQueue.Enqueue(receivedMessage);
            }
            catch
            { ErrorCatcher.instance.ErrorHandler(); }

        }

        private void Update()
        {
            while (_incomingQueue.TryDequeue(out string chunk))
            {
                _processingBuffer.Append(chunk);
            }
            if (_processingBuffer.Length == 0) return;
            string currentContent = _processingBuffer.ToString();

            int delimiterIndex = currentContent.IndexOf('%');

            while (delimiterIndex != -1)
            {
                // pelna wiadomosc (do znaku '%')
                string completeMessage = currentContent[..delimiterIndex];

                try
                {
                    Debug.Log($"[MessageBuffer] Przetwarzam: {completeMessage}");
                    OnCompleteMessage?.Invoke(completeMessage);
                }
                catch (Exception e)
                {
                    Debug.LogError($"B³¹d przy przetwarzaniu wiadomoœci '{completeMessage}': {e.Message}");
                    ErrorCatcher.instance.ErrorHandler();
                }

                // usuwanie przetworzonej wiadomosci (+1 bo tez znak '%')
                _processingBuffer.Remove(0, delimiterIndex + 1);

                // odswierzenie zmiennych do kolejnego komunikatu
                currentContent = _processingBuffer.ToString();
                delimiterIndex = currentContent.IndexOf('%');
            }
        }

        private void OnDestroy()
        {
            Laczenie.instance.OnMessageReceived -= OnMessage;
        }
    }
}