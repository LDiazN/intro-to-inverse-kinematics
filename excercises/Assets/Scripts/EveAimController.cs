using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(Animator), typeof(EveMovementController))]
public class EveAimController : MonoBehaviour
{
    #region Editor Properties

    // ----------------------------
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
        shoulderCamera.SetActive(false);
        _originalCameraLocalPosition = playerCamera.transform.localPosition;
        _originalCameraLocalRotation = playerCamera.transform.localRotation;
    }

    public void Update()
    {
        CollectInput();
        UpdateAiming();
        UpdateGunActive();
        UpdateAnimationState();
    }


    private void CollectInput()
    {
        _aimRequested = Input.GetKeyDown(KeyCode.Mouse1);
    }

    private void UpdateGunActive()
    {
        if (gunSocket.activeSelf && !Aiming)
            gunSocket.SetActive(false);
        else if (!gunSocket.activeSelf && Aiming)
            gunSocket.SetActive(true);
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