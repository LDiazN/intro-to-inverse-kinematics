using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class ImmersiveWalking : MonoBehaviour
{
    #region Inspector properties
    // ----------------------------
    [SerializeField] private float maxIKHandWeight = 0.25f;
    // ----------------------------
    #endregion 
    
    #region Components 
    // ----------------------------
    private Animator _animator;
    // ----------------------------
    #endregion
    
    #region Private State 
    // ----------------------------
    private GameObject _target;
    public GameObject Target {get => _target;
        set => SetTarget(value);
    }

    private AvatarIKGoal _hand;

    private float _currentIKAnimationWeight;
    private Tweener _currentTweener;
    // ----------------------------
    #endregion

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void OnAnimatorIK(int layerIndex)
    {
        if (_target == null)
            return;
            
        // Look at the target
        _animator.SetLookAtWeight(_currentIKAnimationWeight * 1.0f, 0.25f, .5f, 1.0f, 0.8f);
        _animator.SetLookAtPosition(_target.transform.position);

        
        // Put hand in target
        _animator.SetIKPositionWeight(_hand, _currentIKAnimationWeight * maxIKHandWeight);
        _animator.SetIKPosition(_hand, _target.transform.position);
        _animator.SetIKRotationWeight(_hand, _currentIKAnimationWeight * maxIKHandWeight);
        _animator.SetIKRotation(_hand, _target.transform.rotation);
    }

    private AvatarIKGoal GetClosestHand()
    {
        var toTarget = _target.transform.position - transform.position;
        var forwardCrossTarget = Vector3.Cross(transform.forward, toTarget);
        Debug.Log("Cross is " + forwardCrossTarget);
        return forwardCrossTarget.y > 0 ? AvatarIKGoal.RightHand : AvatarIKGoal.LeftHand;
    }

    private void SetTarget(GameObject target)
    {
        if (target != null)
        {
            _target = target;
            // If new target acquired, set up the next ik movement
            _hand = GetClosestHand();
            
            if (_currentTweener != null)
                _currentTweener.Kill();
                
            _currentIKAnimationWeight = 0.0f;
            _currentTweener = DOTween.To(
                () => _currentIKAnimationWeight, 
                x => _currentIKAnimationWeight = x, 
                1.0f, 1.0f
                )
                .SetEase(Ease.OutCirc)
                .OnComplete(() => _currentTweener = null)
                .Play();
        }
        else
        {
            if (_currentTweener != null)
                _currentTweener.Kill();
            
            _currentTweener = DOTween.To(
                () => _currentIKAnimationWeight,
                (x) => _currentIKAnimationWeight = x,
                0.0f, 0.5f
                )
                .SetEase(Ease.Linear)
                .OnComplete(() => {_currentTweener = null; _currentTweener = null;})
                .Play();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + transform.forward);
        if (_target == null) return;
        var toTarget = _target.transform.position - transform.position;
        var forwardCrossTarget = Vector3.Cross(transform.forward, toTarget);
            
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, _target.transform.position);
            
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + forwardCrossTarget.normalized);
    }
}
