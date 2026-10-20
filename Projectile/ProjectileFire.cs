using System.Collections.Generic;
using UnityEngine;

public class ProjectileFire : ProjectileBase
{
    private float fireRange;

    protected override void Start()
    {
        base.Start();
        fireRange = gameManager.bulletSearchRange / 2f;
    }

    protected override void FixedUpdate()
    {
        CheckToDestroy();
        MoveToTarget();
    }

    protected override void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Enemy")) return;

        AllHit();

        ricochet -= 10;
        damage = Mathf.RoundToInt(damage * 0.975f);

        FindNewTarget();
    }
}
