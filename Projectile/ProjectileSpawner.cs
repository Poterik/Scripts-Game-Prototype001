using System.Collections.Generic;
using UnityEngine;

public class ProjectileSpawner : MonoBehaviour
{
    [Header("Audio")]
    protected AudioSource audioSource;
    [SerializeField] protected AudioClip[] bulletShotClips;

    [Header("Projectile")]
    public GameObject projectilePrefab;
    public float attackRange = 10f;
    public float searchDelay = 2.5f;
    public int maxTarget = 1;
    protected float lastSearchTime = 0f;
    public List<GameObject> projectileVariants = new List<GameObject>();

    protected virtual void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    protected virtual void Update()
    {
        FindAtNearbyEnemies();
    }

    private void FindAtNearbyEnemies()
    {
        if (Time.time < lastSearchTime) return;

        Collider[] hit = Physics.OverlapSphere(transform.position, attackRange);

        List<Collider> enemies = new List<Collider>();
        foreach (Collider col in hit)
        {
            if (col.CompareTag("Enemy"))
                enemies.Add(col);
        }

        Vector3 myPos = transform.position;
        enemies.Sort((a, b) =>
            (a.transform.position - myPos).sqrMagnitude
            .CompareTo((b.transform.position - myPos).sqrMagnitude));

        int attackCount = Mathf.Min(maxTarget, enemies.Count);
        for (int i = 0; i < attackCount; i++)
        {
            CreateProjectile(enemies[i].gameObject.transform);
        }

        lastSearchTime = Time.time + searchDelay;
    }

    protected virtual void CreateProjectile(Transform target)
    {
        if (projectilePrefab == null) return;

        GameObject projectile = Instantiate(projectilePrefab, this.transform.position + Vector3.forward, Quaternion.identity);

        ProjectileBase bb = projectile.GetComponent<ProjectileBase>();
        if (bb == null)
        {
            Debug.Log("ProjectileBase not found on Projectile");
            return;
        }

        audioSource.PlayOneShot(bulletShotClips[Random.Range(0, bulletShotClips.Length)]);
        bb.SetTarget(target);
    }
}
