using System;
using System.Collections.Concurrent;
using System.Text;
using UnityEngine;

public class MessageBuffer : MonoBehaviour
{
    private ConcurrentQueue<string> _incomingQueue = new ConcurrentQueue<string>();
    private StringBuilder _processingBuffer = new StringBuilder();

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
        } catch
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
            // Wyci¹gnij pe³n¹ wiadomoœæ (od pocz¹tku do znaku %)
            string completeMessage = currentContent.Substring(0, delimiterIndex);

            // Powiadom resztê gry (tu ³apiemy b³êdy, ¿eby jeden b³¹d nie zatrzyma³ pêtli)
            try
            {
                Debug.Log($"[MessageBuffer] Przetwarzam: {completeMessage}");
                OnCompleteMessage?.Invoke(completeMessage);
            }
            catch (Exception e)
            {
                // Logujemy b³¹d, ale pêtla while dzia³a dalej dla kolejnych wiadomoœci!
                Debug.LogError($"B³¹d przy przetwarzaniu wiadomoœci '{completeMessage}': {e.Message}");
                // Opcjonalnie tutaj ErrorCatcher, jeœli chcesz pokazaæ panel
            }

            // Usuñ przetworzon¹ czêœæ z bufora (+1 ¿eby usun¹æ te¿ znak %)
            _processingBuffer.Remove(0, delimiterIndex + 1);

            // Odœwie¿ zmienne do nastêpnego obiegu pêtli
            currentContent = _processingBuffer.ToString();
            delimiterIndex = currentContent.IndexOf('%');
        }
    }

    private void OnDestroy()
    {
        Laczenie.instance.OnMessageReceived -= OnMessage;
    }
}
