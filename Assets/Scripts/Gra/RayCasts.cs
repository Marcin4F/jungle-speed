using UnityEngine;

public class RayCasts : MonoBehaviour
{
    private float rayDistance = 11.0f;
    private string layerToIgnore = "CardsBlockers";
    public CardMovement card;

    public bool SendRay()
    {
        try
        {
            int layerMask = 1 << LayerMask.NameToLayer(layerToIgnore);  // pobranie bitu warstwy
            layerMask = ~layerMask;     // negacja bitowa -> ignorujemy tylko warstwe layerToIgnore

            RaycastHit hit;
            if (Physics.Raycast(transform.position, Vector3.down, out hit, rayDistance, layerMask))
            {
                hit.collider.gameObject.tag = "UsedCard";
                card = hit.collider.GetComponent<CardMovement>();
                return true;
            }
            return false;
        }
        catch
        {
            ErrorCatcher.instance.ErrorHandler();
            return false;
        }
    }
}
