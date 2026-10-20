using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileBase : MonoBehaviour
{
    [Header("Main Ref")]
    protected GameManager gameManager;
    protected Transform target;
    protected Rigidbody rb;
    protected AudioSource audioSource;

    [Header("Projectile Settings")]
    protected float speed;
    protected int damage;
    protected int ricochet;
    protected float searchRange;
    protected float critChance;
    protected float critDamage;

    [Header("Text")]
    public GameObject damageTextPrefab;

    [Header("Sound")]
    public AudioClip bulletRicochetSound;
    public AudioClip[] bulletTouchSound;

    protected virtual void Start()
    {
        gameManager = GameManager.Instance;
        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
        if (rb == null || gameManager == null || audioSource == null)
            Debug.LogWarning("ProjectileBase not found components");

        rb.useGravity = false;
        rb.linearDamping = 0f;

        speed = gameManager.bulletSpeed;
        damage = gameManager.bulletDamage;
        ricochet = gameManager.bulletRicochet;
        searchRange = gameManager.bulletSearchRange;
        critChance = gameManager.critChance;
        critDamage = gameManager.critDamage;
    }

    protected virtual void FixedUpdate()
    {
        CheckToDestroy();
        MoveToTarget();
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Enemy")) return;

        OnHit(other);

        ricochet--;
        damage = Mathf.RoundToInt(damage * 0.975f);

        FindNewTarget();
    }

    protected void OnHit(Collider other, bool sound = true)
    {
        if (sound) audioSource.PlayOneShot(bulletTouchSound[Random.Range(0, bulletTouchSound.Length)]);

        EnemysAI eai = other.GetComponent<EnemysAI>();
        if (eai == null) return;

        float critMultiple = GetCritMultiple();
        int totalDamage = Mathf.RoundToInt(damage * critMultiple * eai.vulnerability);

        eai.UpdateHealth(-totalDamage);
        SpawnDamageText(totalDamage, Mathf.RoundToInt(critMultiple), other.transform.position);
        UpgradeStatistics.Instance.RecordEndStatistic("Damage", totalDamage);
    }

    protected virtual void AllHit(float multipleRange = 1f)
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, searchRange * multipleRange);

        foreach (Collider collider in colliders)
        {
            if (!collider.CompareTag("Enemy")) continue;
            OnHit(collider, false);
        }

        audioSource.PlayOneShot(bulletTouchSound[Random.Range(0, bulletTouchSound.Length)]);
    }

    protected void SpawnDamageText(int damage, int color, Vector3 target)
    {
        GameObject damageText = Instantiate(damageTextPrefab, target + Vector3.up, Quaternion.identity);
        DamagePopup dp = damageText.GetComponent<DamagePopup>();
        if (dp != null) dp.SetDamageText(damage, color);
    }

    protected virtual void CheckToDestroy()
    {
        if (ricochet <= 0 || damage <= 0 || target == null)
        {
            //GetComponent<MeshRenderer>().enabled = false;
            if (TryGetComponent<MeshRenderer>(out var mr)) mr.enabled = false;
            MeshRenderer[] rend = GetComponentsInChildren<MeshRenderer>();
            if (rend != null)
                foreach (MeshRenderer rend2 in rend) rend2.enabled = false;
            
            Destroy(gameObject, 0.3f);
        }
    }

    protected virtual IEnumerator ResetCollide(float duration = 0.3f)
    {
        Collider collide = GetComponent<Collider>();

        collide.enabled = false;
        yield return new WaitForSeconds(duration);
        collide.enabled = true;
    }

    protected float GetCritMultiple()
    {
        float mult = 1f;

        if (Random.value < critChance)
        {
            mult = 2f + critDamage;

            float overflow = critChance - 1f;
            if (overflow > 0f && Random.value < overflow)
                mult = 4f + critDamage;
        }

        return mult;
    }

    protected virtual void MoveToTarget()
    {
        if (target == null) return;

        Vector3 toTarget = target.position - transform.position;
        transform.rotation = Quaternion.LookRotation(toTarget, Vector3.up);

        rb.linearVelocity = transform.forward * speed;
        speed += Time.fixedDeltaTime;
    }

    protected void FindNewTarget()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, searchRange);

        Collider closest = null;

        foreach (Collider col in colliders)
        {
            if (!col.CompareTag("Enemy")) continue;
            if (target != null && col.gameObject == target.gameObject) continue;

            closest = col;
        }

        target = closest != null ? closest.transform : null;
        StartCoroutine(ResetCollide());
    }

    public void SetTarget(Transform targ)
    {
        target = targ;
    }

    public void SetRicochet(int count)
    {
        ricochet = count;
    }
}
