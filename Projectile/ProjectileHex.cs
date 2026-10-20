using UnityEngine;

public class ProjectileHex : ProjectileBase
{
    [Header("Vulnerability Settings")]
    private float multiple = 0.01f;
    private float duration = 2.5f;

    protected override void OnTriggerEnter(Collider other)
    {
        AddVulnerability(other);
        base.OnTriggerEnter(other);
    }

    private void AddVulnerability(Collider other)
    {
        EnemysAI eai = other.GetComponent<EnemysAI>();
        if (eai == null) return;
        eai.ApplyToxic(multiple, duration);
    }
}
