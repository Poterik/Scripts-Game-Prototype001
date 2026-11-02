using UnityEngine;

public class ProjectileCube : ProjectileBase
{
    private float forcePower;
    private Transform player;

    protected override void Start()
    {
        base.Start();
        player = gameManager.player.transform;
        forcePower = (damage / 1000f) * 3f;
    }

    protected override void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Enemy")) return;

        OnHit(other);
        PushEnemy(other);

        ricochet--;
        FindNewTarget();
    }

    private void PushEnemy(Collider other)
    {
        Rigidbody rb = other.GetComponent<Rigidbody>();
        Vector3 pusher = other.transform.position - player.position;
        pusher.y = 0f;

        if (pusher.sqrMagnitude < 0.001f) return;

        rb.AddForce(pusher.normalized * forcePower, ForceMode.Impulse);
    }
}
