using System.Collections;
using UnityEngine;

public class TotemMovement : MonoBehaviour
{
    private Vector3 startPosition, targetPosition;

    private readonly float duration = 0.2f;
    private float elapsedTime = 0;

    public void MoveTotem(int id)
    {
        try
        { StartCoroutine(MovingTotem(id)); }
        catch
        { ErrorCatcher.instance.ErrorHandler(); }
    }

    private IEnumerator MovingTotem(int id)
    {
        startPosition = transform.position;
        switch (id)      // przesuniecie do odpowiedniego gracza
        {
            case 0:
                targetPosition = new Vector3(2, 1, -5.25f);
                break;
            case 1:
                targetPosition = new Vector3(-9, 1, -2);
                break;
            case 2:
                targetPosition = new Vector3(-2, 1, 5.25f);
                break;
            case 3:
                targetPosition = new Vector3(9, 1, 2);
                break;
        }

        while (elapsedTime < duration)      // przesuwanie totemu
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;

            transform.position = Vector3.Lerp(startPosition, targetPosition, t);

            yield return null;
        }
        transform.position = targetPosition;    // poprawienie pozycji

        StartCoroutine(MovingTotemBack());
    }

    private IEnumerator MovingTotemBack()
    {
        yield return new WaitForSeconds(4);

        startPosition = transform.position;
        targetPosition = new Vector3(0, 1, 0);

        while (elapsedTime < duration)      // przesuwanie totemu
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;

            transform.position = Vector3.Lerp(startPosition, targetPosition, t);

            yield return null;
        }
        transform.position = targetPosition;
    }
}
