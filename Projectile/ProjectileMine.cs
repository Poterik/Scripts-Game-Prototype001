using UnityEngine;

public class ProjectileMine : ProjectileBase
{
    private float rangeMultiple = 2f;
    private float lifetime = 10f;
    [SerializeField] private LayerMask groundLayer;

    protected override void Start()
    {
        base.Start();

        transform.position = GetPositionOnWorld();

        Destroy(gameObject, lifetime);
    }

    protected override void FixedUpdate()
    {

    }

    protected override void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Enemy")) return;

        AllHit(rangeMultiple + ricochet * 0.1f);
        ricochet = 0;

        Destroy(gameObject);
    }

    private Vector3 GetPositionOnWorld()
    {
        float range = speed / 2f;
        Vector3 position = transform.position + new Vector3(Random.Range(-range, range), transform.position.y, Random.Range(-range, range));

        if (Physics.Raycast(position, Vector3.down, out RaycastHit hit, 200f, groundLayer)) return hit.point;
        else return position;
    }
}
