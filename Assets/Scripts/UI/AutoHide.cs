using System.Collections;
using UnityEngine;

public class AutoHide : MonoBehaviour
{
    public float hideDelay = 5.0f;

    void OnEnable()
    {
        StopAllCoroutines();        // zatrzymanie jezeli juz jest jakas korutyna (zeby nie bylo bledow)

        StartCoroutine(HideAfterDelay());
    }

    IEnumerator HideAfterDelay()
    {
        yield return new WaitForSeconds(hideDelay);

        gameObject.SetActive(false);
    }
}