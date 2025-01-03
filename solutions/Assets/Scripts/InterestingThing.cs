using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class InterestingThing : MonoBehaviour
{
    #region Inspector Properties
    [SerializeField] private GameObject target;
    #endregion

    private void Start()
    {
        if (target == null)
            target = gameObject;
    }
    
    private void OnTriggerEnter(Collider other)
    {
        var immersiveWalking = other.gameObject.GetComponent<ImmersiveWalking>();
        if (immersiveWalking is null)
            return;
        immersiveWalking.Target = target;
    }

    private void OnTriggerExit(Collider other)
    {
        var immersiveWalking = other.gameObject.GetComponent<ImmersiveWalking>();
        if (immersiveWalking == null)
            return;
        
        if (immersiveWalking.Target == target)
            immersiveWalking.Target = null;
    }
}
