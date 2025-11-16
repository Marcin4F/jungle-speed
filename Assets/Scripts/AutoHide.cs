using System.Collections;
using UnityEngine;

public class AutoHide : MonoBehaviour
{
    public float hideDelay = 5.0f;

    void OnEnable()
    {
        StopAllCoroutines();

        StartCoroutine(HideAfterDelay());
    }

    IEnumerator HideAfterDelay()
    {
        yield return new WaitForSeconds(hideDelay);

        gameObject.SetActive(false);
    }
}