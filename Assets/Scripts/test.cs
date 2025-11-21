using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class test : MonoBehaviour
{
    private string message = " jeden%dwa%aaaaa";
    private string incomplete = "test";
    private List<string> completedMessages = new List<string>();


    void Start()
    {
        string[] parts = message.Split('%');
        completedMessages = parts.Take(parts.Length - 1).ToList();

        if (completedMessages.Count > 0)
        {
            if (incomplete.Length != 0)
            {
                completedMessages[0] = incomplete + completedMessages[0];
                incomplete = "";
            }
            if (!message.EndsWith("%"))
            {
                incomplete = parts[parts.Length - 1];
            }
        }
        else
        {
            incomplete += message;
        }

        if (completedMessages.Count > 0)
        {
            for (int i = 0; i < completedMessages.Count; i++)
            {
                Debug.Log($"Wiadomoœæ {i + 1}: {completedMessages[i]}");
            }
        }
        else
        {
            Debug.Log("Brak pe³nych wiadomoœci.");
        }
        Debug.Log($"Incomplete: {incomplete}");
    }
}
