using Assets.Scripts.UI;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Gra
{
    public class CardMovement : MonoBehaviour
    {
        private Vector3 startPosition, targetPosition, targetRotationAngles;

        private Quaternion startRotation, endRotation;

        private readonly float duration = 0.35f;
        private float elapsedTime = 0;

        public void MoveCard()
        {
            try { StartCoroutine(FirstCardMovement()); }
            catch { ErrorCatcher.instance.ErrorHandler(); }

        }

        IEnumerator FirstCardMovement()     // pierwszy ruch (do gory)
        {
            startPosition = transform.position;
            startRotation = transform.rotation;
            if (startPosition.z < -2)            // ruch w odpowiednia strone
            {
                targetPosition = startPosition + new Vector3(0, 2.0f, 1.275f);
                targetRotationAngles = new Vector3(270, 0, 0);
            }
            else if (startPosition.z > 2)
            {
                targetPosition = startPosition + new Vector3(0, 2.0f, -1.275f);
                targetRotationAngles = new Vector3(90, 0, 0);
            }
            else if (startPosition.x < 0)
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

                transform.SetPositionAndRotation(Vector3.Lerp(startPosition, targetPosition, t), Quaternion.Slerp(startRotation, endRotation, t));
                yield return null;
            }

            transform.SetPositionAndRotation(targetPosition, endRotation);
            elapsedTime = 0;
            StartCoroutine(SecondCardMovement());       // zaczecie drugiego ruchu
        }

        IEnumerator SecondCardMovement()
        {
            startPosition = transform.position;
            startRotation = transform.rotation;
            if (startPosition.z < -2)
            {
                targetPosition = new Vector3(startPosition.x, GameMeneger.instance.playersShownCards[0] * 0.01f, startPosition.z + 1.275f);     // wartosc y wyliczana na podstawie ilosci kart na stosie odkrytych
                targetRotationAngles = new Vector3(180, 0, 0);
                GameMeneger.instance.playersShownCards[0]++;        // zmiany w ilosci kart odkrytych/zakrytych
                GameMeneger.instance.playersHiddenCards[0]--;
            }
            else if (startPosition.z > 2)
            {
                targetPosition = new Vector3(startPosition.x, GameMeneger.instance.playersShownCards[2] * 0.01f, startPosition.z - 1.275f);
                targetRotationAngles = new Vector3(180, 0, 0);
                GameMeneger.instance.playersShownCards[2]++;
                GameMeneger.instance.playersHiddenCards[2]--;
            }
            else if (startPosition.x < 0)
            {
                targetPosition = new Vector3(startPosition.x + 1.275f, GameMeneger.instance.playersShownCards[1] * 0.01f, startPosition.z);
                targetRotationAngles = new Vector3(0, 0, 180);
                GameMeneger.instance.playersShownCards[1]++;
                GameMeneger.instance.playersHiddenCards[1]--;
            }
            else
            {
                targetPosition = new Vector3(startPosition.x - 1.275f, GameMeneger.instance.playersShownCards[3] * 0.01f, startPosition.z);
                targetRotationAngles = new Vector3(0, 0, 180);
                GameMeneger.instance.playersShownCards[3]++;
                GameMeneger.instance.playersHiddenCards[3]--;
            }

            endRotation = Quaternion.Euler(targetRotationAngles);

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                float t = elapsedTime / duration;

                transform.SetPositionAndRotation(Vector3.Lerp(startPosition, targetPosition, t), Quaternion.Slerp(startRotation, endRotation, t));
                yield return null;
            }

            transform.SetPositionAndRotation(targetPosition, endRotation);
        }
    }
}