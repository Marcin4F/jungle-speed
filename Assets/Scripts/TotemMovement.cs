using System.Collections;
using UnityEngine;

public class TotemMovement : MonoBehaviour
{
    private Vector3 startPosition, targetPosition;

    private float duration = 0.2f, elapsedTime = 0;

    public void MoveTotem(int id)
    {
        StartCoroutine(MovingTotem(id));
    }

    private IEnumerator MovingTotem(int id)
    {
        startPosition = transform.position;
        switch(id)      // przesuniecie do odpowiedniego gracza
        {
            case 0:
                targetPosition = startPosition + new Vector3(2, 0, -5.25f);
                break;
            case 1:
                targetPosition = startPosition + new Vector3(-9, 0, -2);
                break;
            case 2:
                targetPosition = startPosition + new Vector3(-2, 0, 5.25f);
                break;
            case 3:
                targetPosition = startPosition + new Vector3(9, 0, 2);
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
    }
}
