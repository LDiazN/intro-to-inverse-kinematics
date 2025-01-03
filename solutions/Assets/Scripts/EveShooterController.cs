using Unity.Mathematics;
using UnityEngine;

[RequireComponent(typeof(EveAimController))]
public class EveShooterController : MonoBehaviour
{
    #region EditorProperties
    // --------------------------
    [SerializeField] private string knockbackLayerName = "Knockback";
    [SerializeField] private Blaster blaster;

    [Header("Effects")] [SerializeField] private GameObject impactEffect;
    
    // --------------------------
    #endregion
    
    #region Components
    // --------------------------
    private EveAimController _aimController;
    // --------------------------
    #endregion
    
    #region Private State
    // --------------------------
    private bool _shootRequested;
    // --------------------------
    #endregion

    private void Awake()
    {
        _aimController = GetComponent<EveAimController>();
        if (blaster == null)
            blaster = FindObjectOfType<Blaster>();
    }

    private void Update()
    {
        CollectInput();
        Shoot();
    }

    private void CollectInput()
    {
        _shootRequested = Input.GetKeyDown(KeyCode.Mouse0);
    }

    private void Shoot()
    {
        if (!_shootRequested || !_aimController.Aiming)        
            return;
        
        blaster.Shoot();
        
        // Compute ray direction
        var startPos = blaster.transform.position;
        var direction = blaster.transform.forward;
        var layerMask = LayerMask.GetMask(knockbackLayerName);
        var hitSomething = Physics.Raycast(startPos, direction, out RaycastHit hit, Mathf.Infinity, layerMask);
        
        if (!hitSomething)
            return;
        
        // Spawn particles
        if (impactEffect != null)
        {
            Instantiate(impactEffect, hit.point, quaternion.identity);
        }
        

        // Check if actually a monster
        var monsterKnockbackController = hit.collider.gameObject.GetComponentInParent<MonsterKnockbackController>();
        if (monsterKnockbackController == null)
            return;
        
        // Hit monster
        monsterKnockbackController.Hit(hit.collider, hit.normal);
    }

    private void OnDrawGizmos()
    {
        var startPos = blaster.transform.position;
        var direction = blaster.transform.forward;
        Gizmos.color = Color.red;
        Gizmos.DrawRay(startPos, direction * 10f);
    }
}
