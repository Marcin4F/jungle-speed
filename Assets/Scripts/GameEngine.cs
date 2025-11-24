using UnityEngine;

public class GameEngine : MonoBehaviour
{
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            FireScreenRay();
        }
    }

    void FireScreenRay()
    {
        Ray cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(cameraRay, out RaycastHit hitInfo))
        {
            if (hitInfo.collider.gameObject.tag == "Card")
            {
                Debug.Log("DFGHJK");
            }
        }
    }
}
