using UnityEngine;

public class GrenadeShard : ProjectileBase
{
    protected override void Start()
    {
        base.Start();
        StartCoroutine(ResetCollide(0.1f));
        Destroy(gameObject, 0.5f);
    }

    protected override void FixedUpdate()
    {
        rb.linearVelocity = transform.forward * speed;
    }

    protected override void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Enemy")) return;

        NewOnHit(other);
        Debug.Log("Shards deel damage");
    }

    private void NewOnHit(Collider other)
    {
        EnemysAI eai = other.GetComponent<EnemysAI>();
        if (eai == null) return;

        eai.UpdateHealth(-damage / 2);
    }
}
