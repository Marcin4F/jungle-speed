using System.Collections;
using UnityEngine;

public class CardMovement : MonoBehaviour
{
    private Vector3 startPosition, targetPosition, targetRotationAngles;

    private Quaternion startRotation, endRotation;

    private float duration = 0.35f, elapsedTime = 0;

    public void MoveCard()
    {
        StartCoroutine(FirstCardMovement());
    }

    IEnumerator FirstCardMovement()
    {
        startPosition = transform.position;
        startRotation = transform.rotation;
        if(startPosition.z < -2)
        {
            targetPosition = startPosition + new Vector3(0, 2.0f, 1.275f);
            targetRotationAngles = new Vector3(270, 0, 0);
        }
        else if (startPosition.z > 2)
        {
            targetPosition = startPosition + new Vector3(0, 2.0f, -1.275f);
            targetRotationAngles = new Vector3(90, 0, 0);
        }
        else if(startPosition.x < 0)
        {
            targetPosition = startPosition + new Vector3(1.275f, 2.0f, 0);
            targetRotationAngles = new Vector3(0, 0, 90);
        }
        else
        {
            targetPosition = startPosition + new Vector3(-1.275f, 2.0f, 0);
            targetRotationAngles = new Vector3(0, 0, 270);
        }

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
        if (startPosition.z < -2)
        {
            targetPosition = startPosition + new Vector3(0, -2.0f, 1.275f);
            targetRotationAngles = new Vector3(180, 0, 0);
        }
        else if (startPosition.z > 2)
        {
            targetPosition = startPosition + new Vector3(0, -2.0f, -1.275f);
            targetRotationAngles = new Vector3(180, 0, 0);
        }
        else if (startPosition.x < 0)
        {
            targetPosition = startPosition + new Vector3(1.275f, -2.0f, 0);
            targetRotationAngles = new Vector3(0, 0, 180);
        }
        else
        {
            targetPosition = startPosition + new Vector3(-1.275f, -2.0f, 0);
            targetRotationAngles = new Vector3(0, 0, 180);
        }

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
