using System.Collections.Generic;
using UnityEngine;

public class ProjectileGrenade : ProjectileBase
{
    [SerializeField] private GameObject m_Projectile;

    protected override void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Enemy")) return;

        OnHit(other);
        GrenadeExplosion();

        ricochet = 0;
    }

    private void GrenadeExplosion()
    {
        for (int i = 0; i < ricochet; i++)
        {
            Vector3 dir = Random.onUnitSphere;
            Quaternion spawnRot = Quaternion.FromToRotation(Vector3.forward, dir);
            Instantiate(m_Projectile, transform.position, spawnRot);
        }
    }
}
