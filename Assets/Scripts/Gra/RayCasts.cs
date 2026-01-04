using Assets.Scripts.UI;
using UnityEngine;

namespace Assets.Scripts.Gra
{
    public class RayCasts : MonoBehaviour
    {
        private readonly float rayDistance = 11.0f;
        private readonly string layerToIgnore = "CardsBlockers";
        public CardMovement card;

        public bool SendRay()
        {
            try
            {
                int layerMask = 1 << LayerMask.NameToLayer(layerToIgnore);  // pobranie bitu warstwy
                layerMask = ~layerMask;     // negacja bitowa -> ignorujemy tylko warstwe layerToIgnore

                if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, rayDistance, layerMask))
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
}