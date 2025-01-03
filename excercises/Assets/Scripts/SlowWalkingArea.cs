using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class SlowWalkingArea : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        var eveController = other.GetComponentInParent<EveMovementController>();
        if (eveController == null)
            return;
        eveController.Walking = true;
    }

    private void OnTriggerExit(Collider other)
    {
        var eveController = other.GetComponentInParent<EveMovementController>();
        if (eveController == null)
            return;
        eveController.Walking = false;
    }
}
