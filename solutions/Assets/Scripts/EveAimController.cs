using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(Animator), typeof(EveMovementController))]
public class EveAimController : MonoBehaviour
{
    #region Editor Properties

    // ----------------------------
    [SerializeField] private GameObject aimTarget;
    [SerializeField] private float aimSquareLength = 1.0f;
    [SerializeField] private float aimSensibility = 1.0f;
    [SerializeField] [Range(0, 1)] private float aimWeight = 1.0f;
    [SerializeField] private GameObject gunSocket;
    [SerializeField] private GameObject shoulderCamera;
    [SerializeField] private GameObject playerCamera;

    [Header("Animator Parameters")] [SerializeField]
    private string aimingParameterName = "Aiming";

    [SerializeField] private string aimRequestedParameterName = "Aim Requested";
    // ----------------------------

    #endregion

    #region Components

    // ----------------------------
    private Animator _animator;
    // ----------------------------

    #endregion

    #region Internal State

    // ----------------------------
    private Vector3 _aimTargetOriginalPosition;
    private Vector3 _prevMousePosition;
    private Vector3 _mouseDelta;

    private bool _aimRequested;
    public bool Aiming { get; private set; }

    private int _aimingParameter;
    private int _aimRequestedParameter;
    private Tweener _currentCameraPositionTweener;
    private Tweener _currentCameraRotationTweener;
    private Vector3 _originalCameraLocalPosition;

    private Quaternion _originalCameraLocalRotation;
    // ----------------------------

    #endregion

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _aimingParameter = Animator.StringToHash(aimingParameterName);
        _aimRequestedParameter = Animator.StringToHash(aimRequestedParameterName);
    }

    public void Start()
    {
        _aimTargetOriginalPosition = aimTarget.transform.localPosition;
        _prevMousePosition = Input.mousePosition;
        shoulderCamera.SetActive(false);
        _originalCameraLocalPosition = playerCamera.transform.localPosition;
        _originalCameraLocalRotation = playerCamera.transform.localRotation;
    }

    public void Update()
    {
        CollectInput();
        UpdateAiming();
        UpdateTargetPosition();
        UpdateGunActive();
        UpdateAnimationState();
    }


    private void CollectInput()
    {
        var newMousePosition = Input.mousePosition;
        _mouseDelta = newMousePosition - _prevMousePosition;
        _prevMousePosition = newMousePosition;
        _aimRequested = Input.GetKeyDown(KeyCode.Mouse1);
    }

    private void UpdateTargetPosition()
    {
        if (!Aiming)
        {
            ResetAimTarget();
            return;
        }

        var newAimTargetPosition = aimTarget.transform.localPosition + Time.deltaTime * aimSensibility * _mouseDelta;
        var minX = _aimTargetOriginalPosition.x - aimSquareLength / 2.0f;
        var minY = _aimTargetOriginalPosition.y - aimSquareLength / 2.0f;
        var maxX = _aimTargetOriginalPosition.x + aimSquareLength / 2.0f;
        var maxY = _aimTargetOriginalPosition.y + aimSquareLength / 2.0f;

        newAimTargetPosition.x = Mathf.Clamp(newAimTargetPosition.x, minX, maxX);
        newAimTargetPosition.y = Mathf.Clamp(newAimTargetPosition.y, minY, maxY);

        aimTarget.transform.localPosition = newAimTargetPosition;
    }

    private void OnAnimatorIK(int layerIndex)
    {
        if (!Aiming)
            return;
        _animator.SetIKPositionWeight(AvatarIKGoal.RightHand, aimWeight);
        _animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, aimWeight);
        _animator.SetLookAtWeight(1, 0.5f, 1.0f, 1.0f);
        _animator.SetLookAtPosition(aimTarget.transform.position);
        _animator.SetIKPosition(AvatarIKGoal.RightHand, aimTarget.transform.position);
        _animator.SetIKPosition(AvatarIKGoal.LeftHand, aimTarget.transform.position);
    }

    private void OnDrawGizmos()
    {
        if (aimTarget == null)
            return;

        Gizmos.color = Color.red;
        var center = transform.localToWorldMatrix * new Vector4(_aimTargetOriginalPosition.x,
            _aimTargetOriginalPosition.y, _aimTargetOriginalPosition.z, 1.0f);

        Gizmos.DrawWireCube(center, new Vector3(aimSquareLength, aimSquareLength, 0.1f));
    }

    private void UpdateGunActive()
    {
        if (gunSocket.activeSelf && !Aiming)
            gunSocket.SetActive(false);
        else if (!gunSocket.activeSelf && Aiming)
            gunSocket.SetActive(true);
    }

    private void ResetAimTarget()
    {
        aimTarget.transform.localPosition = _aimTargetOriginalPosition;
    }

    private void UpdateAnimationState()
    {
        if (_aimRequested)
            _animator.SetTrigger(_aimRequestedParameter);
        _animator.SetBool(_aimingParameter, Aiming);
    }

    void UpdateAiming()
    {
        if (!_aimRequested) return;

        Aiming = !Aiming;

        if (_currentCameraPositionTweener != null)
            _currentCameraPositionTweener.Kill();

        if (_currentCameraRotationTweener != null)
            _currentCameraRotationTweener.Kill();

        var targetLocalPosition = Aiming ? shoulderCamera.transform.localPosition : _originalCameraLocalPosition;
        var targetLocalRotation = Aiming ? shoulderCamera.transform.localRotation : _originalCameraLocalRotation;

        _currentCameraPositionTweener = playerCamera.transform
            .DOLocalMove(targetLocalPosition, 0.25f)
            .SetEase(Ease.OutQuart)
            .OnComplete(() => _currentCameraPositionTweener = null)
            .Play();

        _currentCameraRotationTweener = playerCamera.transform
            .DOLocalRotateQuaternion(targetLocalRotation, 0.25f)
            .SetEase(Ease.OutQuart)
            .OnComplete(() => _currentCameraRotationTweener = null)
            .Play();
    }
}