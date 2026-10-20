using System.Collections.Generic;
using UnityEngine;

public class ProjectileBlackHole : ProjectileBase
{
    [Header("Refereces")]
    private Collider ownCollide;
    private Renderer ownRenderer;
    [SerializeField] private GameObject eventHorizonPrefab;

    [Header("Settings")]
    private float rotateSpeed = 120f;
    private float gravityPoolPower = 5f;
    private float minGravityPoolPower = 2.5f;
    private float lastTickInterval = 0f;
    private float tickInterval = 1f;
    private float gravyZone;
    private float baseDamage;
    private bool isActive;

    private HashSet<Collider> enemys = new HashSet<Collider>();

    protected override void Start()
    {
        base.Start();

        gravyZone = searchRange / 5f;
        baseDamage = damage;

        ownCollide = GetComponent<Collider>();
        if (ownCollide == null) Debug.LogWarning("Collider not found");
        ownRenderer = GetComponent<Renderer>();

        Destroy(gameObject, 5f);
    }

    private void Update()
    {
        transform.Rotate(Vector3.up * (rotateSpeed * Time.deltaTime));

        if (!isActive) return;

        transform.localScale += Vector3.one * (0.1f * Time.deltaTime);
    }

    protected override void FixedUpdate()
    {
        if (isActive)
            ActivateTime();
        else
            base.FixedUpdate();
    }

    protected override void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Enemy")) return;

        rb.linearVelocity = Vector3.zero;
        if (ownCollide != null) ownCollide.enabled = false;

        isActive = true;
    }

    private void ActivateTime()
    {
        FindEnemys();
        ApplyGravityAndDamage();

        CheckToDestroy();
    }

    private void FindEnemys()
    {
        enemys.RemoveWhere(e =>
        e == null ||
        Vector3.Distance(e.transform.position, transform.position) > gravyZone);

        foreach (var col in Physics.OverlapSphere(transform.position, gravyZone))
        {
            if (col.CompareTag("Enemy"))
            {
                enemys.Add(col);
            }
        }

        damage = Mathf.Max(1, Mathf.RoundToInt(baseDamage / enemys.Count));
    }

    private void ApplyGravityAndDamage()
    {
        float visualRadius = ownRenderer != null
            ? ownRenderer.bounds.extents.magnitude
            : 1f;

        bool shouldTick = Time.time >= lastTickInterval;

        foreach (var enemy in enemys)
        {
            Vector3 dir = transform.position - enemy.transform.position;
            dir.y = 0f;
            float dist = dir.magnitude;
            if (dist < 0.01f) continue;

            float power = dist > visualRadius ? minGravityPoolPower : gravityPoolPower;
            Vector3 move = dir.normalized * (power * Time.fixedDeltaTime);

            var rb = enemy.GetComponent<Rigidbody>();
            if (rb != null)
                rb.MovePosition(rb.position + move);
            else
                enemy.transform.position += move;

            if (shouldTick) OnHit(enemy, false);
        }

        if (shouldTick)
        {
            //damage = Mathf.Max(1, Mathf.RoundToInt(damage * damageDecread));
            lastTickInterval = Time.time + tickInterval;
            audioSource.PlayOneShot(bulletTouchSound[Random.Range(0, bulletTouchSound.Length)]);
        }
    }

    private void OnDrawGizmos()
    {
        float r = Application.isPlaying ? gravyZone : searchRange / 5f;
        if (r <= 0f) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, r);
        Gizmos.color = new Color(1f, 0.9f, 0f, 0.1f);
        Gizmos.DrawSphere(transform.position, r);
    }
}
