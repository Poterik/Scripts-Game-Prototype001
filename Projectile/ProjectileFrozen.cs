using UnityEngine;

public class ProjectileFrozen : ProjectileBase
{
    [Header("Settings")]
    private float multiple; //= 0.01f;
    private float duratiorn = 0.2f;
    private float currentSPD;

    protected override void Start()
    {
        base.Start();
        currentSPD = GameManager.Instance.player.GetComponent<ProjectileSpawner>().searchDelay;
        multiple = GetSlowdown(currentSPD);
    }

    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);

        if (!other.CompareTag("Enemy")) return;

        EnemysAI eai = other.GetComponent<EnemysAI>();
        if (eai != null) eai.ApplySlow(multiple, duratiorn);
    }

    private float GetSlowdown(float attackSpeed, float maxSlowdown = 0.25f, float minAttackSpeed = 0.1f, float maxAttackSpeed = 2.5f)
    {
        float t = (attackSpeed - minAttackSpeed) / (maxAttackSpeed - minAttackSpeed);
        t = Mathf.Clamp01(t);
        return Mathf.Lerp(0.005f, maxSlowdown, t);
    }
}
