using System.Collections;
using UnityEngine;

public class CardMovement : MonoBehaviour
{
    private Vector3 startPosition;
    private Vector3 targetPosition;
    private Vector3 targetRotationAngles;

    private Quaternion startRotation;
    private Quaternion endRotation;

    private float duration = 0.2f;
    private float elapsedTime = 0;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(FirstCardMovement());
        }
    }

    IEnumerator FirstCardMovement()
    {
        startPosition = transform.position;
        startRotation = transform.rotation;
        targetPosition = startPosition + new Vector3(0, 2.0f, 1.275f);
        targetRotationAngles = new Vector3(270, 0, 0);
        endRotation = Quaternion.Euler(targetRotationAngles);
        
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;

            transform.position = Vector3.Lerp(startPosition, targetPosition, t);
            transform.rotation = Quaternion.Slerp(startRotation, endRotation, t);

            yield return null;
        }

        transform.position = targetPosition;
        transform.rotation = endRotation;
        elapsedTime = 0;
        StartCoroutine(SecondCardMovement());
    }

    IEnumerator SecondCardMovement()
    {
        startPosition = transform.position;
        startRotation = transform.rotation;
        targetPosition = startPosition + new Vector3(0, -2.0f, 1.275f);
        targetRotationAngles = new Vector3(180, 0, 0);
        endRotation = Quaternion.Euler(targetRotationAngles);

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;

            transform.position = Vector3.Lerp(startPosition, targetPosition, t);
            transform.rotation = Quaternion.Slerp(startRotation, endRotation, t);

            yield return null;
        }

        transform.position = targetPosition;
        transform.rotation = endRotation;
    }
}
