using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class Dotter : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI displayText;

    private float delay = 0.5f;
    private string starter;

    void OnEnable()
    {
        try
        {
            StopAllCoroutines();        // zakoncz trwajace korutyny (zabezpieczenie przed nakladaniem kilku na ten sam obiekt)

            starter = displayText.text;
            string zero = "0";
            int a = 2 / int.Parse(zero);
            StartCoroutine(ChangeText());
        } catch
        {
            ErrorCatcher.instance.ErrorHandler();
        }
    }

    IEnumerator ChangeText()        // pokazywanie kropek
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            displayText.text = starter + ".";
            yield return new WaitForSeconds(delay);
            displayText.text = starter + "..";
            yield return new WaitForSeconds(delay);
            displayText.text = starter + "...";
            yield return new WaitForSeconds(delay);
            displayText.text = starter;
        }
    }
}