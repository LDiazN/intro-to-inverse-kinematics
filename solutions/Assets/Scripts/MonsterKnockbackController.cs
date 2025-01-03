using System;
using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class MonsterKnockbackController : MonoBehaviour
{
    #region Inspector properties
    // --------------------------
    [Header("Colliders")] [SerializeField] private CapsuleCollider leftHandCollider;
    [SerializeField] private CapsuleCollider rightHandCollider;
    [SerializeField] private CapsuleCollider headCollider;

    [Header("Animation")] [SerializeField] [Range(0, 1)]
    private float maxOffset = 1.0f;

    [SerializeField] [Min(0)] private float headMaxOffset = 1.0f;

    [SerializeField] private float animationTime = 0.25f;

    [Header("Head")] [SerializeField] [Tooltip("Used to animate the head when it gets shot")]
    private GameObject knockBackLookAtTarget;
    // --------------------------
    #endregion

    #region Components
    // --------------------------
    private Animator _animator;
    // --------------------------
    #endregion

    #region Private State
    // --------------------------
    private readonly AnimationData[] _animationData = new AnimationData[3];
    private bool IsOnAnimation(BodyPart p) => _animationData[(int)p].IsOnAnimation;
    private float CurrentOffset(BodyPart p) => _animationData[(int)p].CurrentOffset;
    private void SetOnAnimation(BodyPart p, bool on) => _animationData[(int)p].IsOnAnimation = on;
    private void SetCurrentOffset(BodyPart p, float offset) => _animationData[(int)p].CurrentOffset = offset;

    private ref AnimationData GetAnimationData(BodyPart p) => ref _animationData[(int)p];

    private enum BodyPart
    {
        LeftHand = 0,
        RightHand = 1,
        Head = 2,
        None = 3,
    }

    private struct AnimationData
    {
        public Vector3 HitNormal;
        public bool IsOnAnimation;
        public float CurrentOffset;
    }

    /// <summary>
    /// This position (in local space) is used to animate the look at target when
    /// the head is hit
    /// </summary>
    private Vector3 _lookAtTargetOriginalPosition;

    // --------------------------
    #endregion


    private void Awake()
    {
        for (var i = 0; i < _animationData.Length; i++)
        {
            _animationData[i] = new AnimationData
            {
                HitNormal = Vector3.zero,
                IsOnAnimation = false,
                CurrentOffset = 0.0f,
            };
        }

        _animator = GetComponent<Animator>();
        _lookAtTargetOriginalPosition = knockBackLookAtTarget.transform.localPosition;
    }

    public void Hit(Collider bodyPartCollider, Vector3 hitNormal)
    {
        var part = ColliderToBodyPart(bodyPartCollider);
        Debug.Assert(part != BodyPart.None, "Invalid collider passed to hit");

        switch (part)
        {
            case BodyPart.RightHand:
            case BodyPart.LeftHand:
                StartMovingHand(hitNormal, part);
                break;
            case BodyPart.Head:
                StartMovingHead(hitNormal);
                break;
            case BodyPart.None:
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private BodyPart ColliderToBodyPart(Collider bodyPartCollider)
    {
        if (bodyPartCollider == headCollider)
            return BodyPart.Head;
        if (bodyPartCollider == leftHandCollider)
            return BodyPart.LeftHand;
        if (bodyPartCollider == rightHandCollider)
            return BodyPart.RightHand;

        return BodyPart.None;
    }

    private static AvatarIKGoal BodyPartToGoal(BodyPart part)
    {
        switch (part)
        {
            case BodyPart.LeftHand:
                return AvatarIKGoal.LeftHand;
            case BodyPart.RightHand:
                return AvatarIKGoal.RightHand;
            case BodyPart.Head:
            case BodyPart.None:
            default:
                Debug.LogError("Invalid body part for Avatar Ik Goal");
                break;
        }

        // Error 
        return AvatarIKGoal.LeftFoot;
    }

    private void OnAnimatorIK(int layerIndex)
    {
        for (int i = 0; i < (int)BodyPart.None; i++)
        {
            var bodyPart = (BodyPart)i;
            if (!IsOnAnimation(bodyPart))
                continue;

            if (bodyPart is BodyPart.RightHand or BodyPart.LeftHand)
            {
                MoveHand(bodyPart);
            }
            else if (bodyPart is BodyPart.Head)
            {
                MoveHead();
            }
        }
    }

    private void MoveHand(BodyPart part)
    {
        var goal = BodyPartToGoal(part);
        var animationData = GetAnimationData(part);

        var currentPosition = _animator.GetIKPosition(goal);
        var newPosition = currentPosition - animationData.HitNormal * animationData.CurrentOffset;

        _animator.SetIKPositionWeight(goal, 1.0f);
        _animator.SetIKPosition(goal,
            newPosition
        );
    }

    private void MoveHead()
    {
        _animator.SetLookAtWeight(1);
        _animator.SetLookAtPosition(knockBackLookAtTarget.transform.position);
    }

    private void StartMovingHand(Vector3 hitNormal, BodyPart hand)
    {
        if (IsOnAnimation(hand))
            return;

        SetOnAnimation(hand, true);
        ref var animationData = ref GetAnimationData(hand);
        animationData.CurrentOffset = 0;
        animationData.HitNormal = hitNormal;

        var sequence = DOTween.Sequence();
        sequence.Append(DOTween.To(() => CurrentOffset(hand), x => SetCurrentOffset(hand, x), maxOffset, animationTime)
            .SetEase(Ease.OutBack)
        );

        sequence.Append(DOTween.To(() => CurrentOffset(hand), x => SetCurrentOffset(hand, x), 0, animationTime)
            .SetEase(Ease.OutExpo)
            .OnComplete(() => SetOnAnimation(hand, false))
        );
        sequence.Play();
    }

    private void StartMovingHead(Vector3 hitNormal)
    {
        if (IsOnAnimation(BodyPart.Head))
            return;

        SetOnAnimation(BodyPart.Head, true);
        ref var animationData = ref GetAnimationData(BodyPart.Head);
        animationData.HitNormal = hitNormal;

        var newPosition= _lookAtTargetOriginalPosition - hitNormal * headMaxOffset;

        var sequence = DOTween.Sequence();
        sequence.Append(DOTween.To(() => knockBackLookAtTarget.transform.localPosition,
                x => knockBackLookAtTarget.transform.localPosition = x, newPosition, animationTime)
            .SetEase(Ease.OutBack)
        );
        sequence.Append(DOTween.To(() => knockBackLookAtTarget.transform.localPosition,
                x => knockBackLookAtTarget.transform.localPosition = x, _lookAtTargetOriginalPosition, animationTime)
            .SetEase(Ease.OutExpo)
            .OnComplete(() => SetOnAnimation(BodyPart.Head, false))
        );
        sequence.Play();
    }
}