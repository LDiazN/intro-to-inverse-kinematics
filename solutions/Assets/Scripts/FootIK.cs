using UnityEngine;

[RequireComponent(typeof(Animator))]
public class FootIK : MonoBehaviour
{
    #region Inspector properties
    // ------------------------------
    [SerializeField] private float padding = 0.1f;

    [SerializeField] private float maxDistanceToFloor = 1;

    [SerializeField] private bool footIKActive = true;
    // ------------------------------
    #endregion

    #region Components
    // ------------------------------
    private Animator _animator;
    // ------------------------------
    #endregion

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void OnAnimatorIK(int layerIndex)
    {
        if (!footIKActive)
            return;

        var lFootPosition = _animator.GetIKPosition(AvatarIKGoal.LeftFoot);
        var rFootPosition = _animator.GetIKPosition(AvatarIKGoal.RightFoot);

        _animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 1);
        _animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, 1);
        _animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot, 0);
        _animator.SetIKRotationWeight(AvatarIKGoal.RightFoot, 0);

        _animator.SetIKPosition(AvatarIKGoal.LeftFoot, ComputeNewFootPosition(lFootPosition));
        _animator.SetIKPosition(AvatarIKGoal.RightFoot, ComputeNewFootPosition(rFootPosition));
    }

    private Vector3 ComputeNewFootPosition(Vector3 footIKPosition)
    {
        var impacted = Physics.Raycast(footIKPosition, Vector3.down, out var hit, maxDistanceToFloor);

        // If foot is hanging, leave it there
        if (!impacted)
            return footIKPosition;

        return footIKPosition + Vector3.down * (hit.distance - padding);
    }
}