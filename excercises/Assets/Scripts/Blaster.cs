using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class Blaster : MonoBehaviour
{
    #region Inspector Properties
    // --------------------------------
    [SerializeField] private GameObject sightStart;
    [SerializeField] private float sightLength = 3;

    [SerializeField] private ParticleSystem muzzleFlash;
    // --------------------------------
    #endregion

    #region Components
    // --------------------------------
    private LineRenderer _lineRenderer;
    // --------------------------------
    #endregion


    private void Awake()
    {
        _lineRenderer = GetComponent<LineRenderer>();
    }

    private void FixedUpdate()
    {
        _lineRenderer.SetPosition(0, sightStart.transform.position);
        _lineRenderer.SetPosition(1, sightStart.transform.position + sightLength * transform.forward);
    }

    public void Shoot() => muzzleFlash.Play();
}