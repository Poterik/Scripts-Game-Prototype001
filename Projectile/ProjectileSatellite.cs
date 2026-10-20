using UnityEngine;

public class ProjectileSatellite : ProjectileBase
{
    [Header("Orbit Settings")]
    private float orbitRadius;
    private float orbitSpeed;
    private float angle;
    private float direction = 1f;
    private float lifetime = 10f;
    private bool isAttacking = false;

    private Transform orbitCenter;
    private Vector3 orbitAxis;
    private PlayerFighter pf;

    protected override void Start()
    {
        base.Start();

        pf = gameManager.player;
        orbitCenter = gameManager.player.transform;
        if (orbitCenter == null) Debug.LogWarning("Player not found");

        orbitAxis = Random.onUnitSphere;
        angle = Random.Range(0f, 360f);
        orbitRadius = Mathf.Max(2f, ricochet / 5f);
        orbitSpeed = speed * 10f;

        BoxCollider box = GetComponent<BoxCollider>();
        float sizeValue = searchRange / 5f;
        box.size = new Vector3(sizeValue, sizeValue, sizeValue);

        transform.position = GetOrbitPosition();
        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        angle += orbitSpeed * direction * Time.deltaTime;
        transform.position = GetOrbitPosition();
        transform.rotation = Quaternion.LookRotation(GetOrbitPosition() - transform.position);
    }

    protected override void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Enemy") || isAttacking) return;

        isAttacking = true;

        OnHit(other);
        pf.UpdateHealth(1);

        Destroy(gameObject, 0.3f);
    }

    private Vector3 GetOrbitPosition()
    {
        Quaternion rot = Quaternion.AngleAxis(angle, orbitAxis);
        Vector3 offset = rot * Vector3.forward * orbitRadius;
        return orbitCenter.position + offset;
    }
}
