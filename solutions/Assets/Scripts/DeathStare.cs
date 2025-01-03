using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(Animator), typeof(SphereCollider))]
public class DeathStare : MonoBehaviour
{
    #region Inspector Properties
    // ---------------------------
    [SerializeField] [Tooltip("This volume is used to detect when the player is within reach")]
    private SphereCollider playerDetectionVolume;
    // ---------------------------
    #endregion

    #region Components
    // ---------------------------
    private Animator _animator;
    // ---------------------------
    #endregion

    #region Private State
    // ---------------------------
    private GameObject _target;
    private Tweener _headTweener;
    private float _lookAtWeight;
    // ---------------------------
    #endregion

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        if (playerDetectionVolume == null)
            playerDetectionVolume = GetComponent<SphereCollider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        // Not eve, don't care
        if (other.GetComponent<EveMovementController>() == null)
            return;

        _target = other.gameObject;
        if (_headTweener != null)
            _headTweener.Kill();

        _headTweener = DOTween.To(
                () => _lookAtWeight,
                x => _lookAtWeight = x,
                1,
                0.25f
            )
            .SetEase(Ease.OutCubic)
            .OnComplete(() => _headTweener = null)
            .Play();
    }

    private void OnTriggerExit(Collider other)
    {
        // Not target, don't care
        if (other.gameObject != _target)
            return;


        if (_headTweener != null)
            _headTweener.Kill();

        _headTweener = DOTween.To(
                () => _lookAtWeight,
                x => _lookAtWeight = x,
                0,
                0.25f
            )
            .SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                _headTweener = null;
                _target = null;
            })
            .Play();
    }

    private void OnAnimatorIK(int layerIndex)
    {
        if (_target == null)
            return;

        _animator.SetLookAtWeight(_lookAtWeight, 0.2f, 1.0f, 1.0f, 0.8f);
        _animator.SetLookAtPosition(_target.transform.position);
    }
}