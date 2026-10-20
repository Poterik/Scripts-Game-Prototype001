using UnityEngine;

public class ProjectileBounce : ProjectileBase
{
    private float multiple = 1.025f;

    protected override void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Enemy")) return;

        OnHit(other);

        ricochet--;
        damage = Mathf.RoundToInt(damage * multiple);

        FindNewTarget();
    }
}
