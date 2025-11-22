using System.Collections;
using UnityEngine;
using TMPro;

public class Dotter : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI displayText;

    private float delay = 0.5f;
    private string starter;

    void OnEnable()
    {
        StopAllCoroutines();

        starter = displayText.text;
        StartCoroutine(ChangeText());
    }

    IEnumerator ChangeText()
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