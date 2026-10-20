using UnityEngine;

public class ProjectileConcussive : ProjectileBase
{
    private float multiple = 1f;
    private float duration = 0.1f;

    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);
        if (!other.CompareTag("Enemy")) return;

        EnemysAI eai = other.GetComponent<EnemysAI>();
        eai.ApplySlow(multiple, duration);
    }
}
