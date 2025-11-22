using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MessageBuffer : MonoBehaviour
{
    public Queue<string> messages = new Queue<string>();
    private string incomplete = "";
    private bool hasNewMessage = false;

    public event Action<string> OnCompleteMessage;

    private void Start()
    {
        Laczenie.instance.OnMessageReceived += OnMessage;
    }

    private void OnMessage(string recivedMessage)
    {
        messages.Enqueue(recivedMessage);
        hasNewMessage = true;
    }

    private void Update()
    {
        if (!hasNewMessage)
        {
            return;
        }

        while (messages.Count > 0)
        {
            string messageToProcess = messages.Dequeue();
            Debug.Log($"[MessageBuffer - Processing] Przetwarzam wiadomoœæ: {messageToProcess}");

            List<string> completedMessages = new List<string>();

            string[] parts = messageToProcess.Split('%');
            completedMessages = parts.Take(parts.Length - 1).ToList();

            if (completedMessages.Count > 0)
            {
                if (incomplete.Length != 0)
                {
                    completedMessages[0] = incomplete + completedMessages[0];
                    incomplete = "";
                }
                
                if (!messageToProcess.EndsWith("%"))
                {
                    incomplete = parts[parts.Length - 1];
                }

                foreach (string completeMsg in completedMessages)
                {
                    OnCompleteMessage.Invoke(completeMsg);
                    Debug.Log($"[MessageBuffer - Complete] Gotowa wiadomoœæ: {completeMsg}");
                }
            }
            else
            {
                incomplete += messageToProcess;
            }
        }

        hasNewMessage = false;
    }

    private void OnDestroy()
    {
        Laczenie.instance.OnMessageReceived -= OnMessage;
    }
}
