using UnityEngine;

public class KnockbackTestScript : MonoBehaviour
{
    #region Inspector Properties
    // -------------------------
    [SerializeField]
    private string knockbackLayerName = "Knockback";
    // -------------------------
    #endregion
    
    #region Private State
    // -------------------------
    private Camera _camera;
    private bool _justClicked = true;
    private Vector3 _mousePosition;
    // -------------------------
    #endregion
    
    private void Start()
    {
        _camera = Camera.main;
        _mousePosition = Input.mousePosition;
    }

    private void Update()
    {
        CollectInput();
        TryHitMonster();
    }

    private void CollectInput()
    {
        _justClicked = Input.GetKeyDown(KeyCode.Mouse0);
        _mousePosition = Input.mousePosition;
    }

    private void TryHitMonster()
    {
        if (!_justClicked)
            return;

        _justClicked = false;
        
        var ray = _camera.ScreenPointToRay(_mousePosition);
        Debug.DrawRay(ray.origin, ray.direction * Mathf.Infinity, Color.red, 1);
        var layerMask = LayerMask.GetMask(knockbackLayerName);
        var hitSomething = Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask);
        if (!hitSomething)
            return;

        var monsterHit = hit.collider.gameObject.GetComponentInParent<MonsterKnockbackController>();
        monsterHit?.Hit(hit.collider, hit.normal);
    }
}
