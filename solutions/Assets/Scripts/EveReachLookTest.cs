using UnityEngine;

public class EveReachLookTest : MonoBehaviour
{
    # region Inspector Properties
    // -----------------------------------------
    [SerializeField]
    private Transform target;
    [SerializeField]
    [Range(0, 1)]
    private float targetWeight = 1;
    [SerializeField]
    private bool IKActive = true;
    // -----------------------------------------
    # endregion
    
    # region Components 
    // -----------------------------------------
    private Animator _animator;
    // -----------------------------------------
    # endregion

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void OnAnimatorIK(int layerIndex)
    {
        if (target == null || !IKActive)
            return;
        
        _animator.SetLookAtWeight(1);
        _animator.SetLookAtPosition(target.position);
        _animator.SetIKPositionWeight(AvatarIKGoal.RightHand, targetWeight);
        _animator.SetIKPosition(AvatarIKGoal.RightHand, target.position);
        _animator.SetIKRotationWeight(AvatarIKGoal.RightHand, targetWeight);
        _animator.SetIKRotation(AvatarIKGoal.RightHand, target.rotation);
    }
}
