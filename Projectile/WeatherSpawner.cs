using System.Collections.Generic;
using UnityEngine;

public class WeatherSpawner : ProjectileSpawner
{
    private ProjectileSpawner projectileSpawner;

    protected override void Start()
    {
        base.Start();
        projectileSpawner = GameManager.Instance.player.GetComponent<ProjectileSpawner>();
        attackRange = projectileSpawner.attackRange + 10f;
        searchDelay = projectileSpawner.searchDelay - 0.4f;
        maxTarget = projectileSpawner.maxTarget;

        Destroy(gameObject, 1.5f);
    }

    protected override void Update()
    {
        NewFindEnemys();
    }

    private void NewFindEnemys()
    {
        if (Time.time < lastSearchTime) return;

        Collider[] colliders = Physics.OverlapSphere(transform.position, attackRange);

        List<Collider> enemys = new List<Collider>();
        foreach (Collider col in colliders)
        {
            if (col.CompareTag("Enemy")) enemys.Add(col);
        }

        if (enemys.Count > 0) CreateProjectile(enemys[Random.Range(0, enemys.Count)].gameObject.transform);

        lastSearchTime = Time.time + searchDelay;
    }
}
