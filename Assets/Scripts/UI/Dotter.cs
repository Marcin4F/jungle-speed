using System.Collections;
using UnityEngine;
using TMPro;

public class Dotter : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI displayText;

    public float delay = 0.5f;

    void OnEnable()
    {
        StopAllCoroutines();

        StartCoroutine(ChangeText());
    }

    IEnumerator ChangeText()
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            displayText.text = "Connecting to the server.";
            yield return new WaitForSeconds(delay);
            displayText.text = "Connecting to the server..";
            yield return new WaitForSeconds(delay);
            displayText.text = "Connecting to the server...";
            yield return new WaitForSeconds(delay);
            displayText.text = "Connecting to the server";
        }
    }
}