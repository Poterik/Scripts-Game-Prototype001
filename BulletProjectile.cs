using System.Collections;
using UnityEngine;

public class BulletProjectile : MonoBehaviour
{
    private Transform target;

    [SerializeField]
    public GameObject damageTextPrefab;
    
    [Header("Settings Manager")]
    private float speed;
    private int damage;
    private int ricochet;
    private float searchRange;
    private int vampirism = 0;
    private float critChance;
    private float critDamage;

    private Rigidbody rb;

    private void Start()
    {
        StartCoroutine(ResetCollide());

        damage = GameManager.Instance.bulletDamage;
        ricochet = GameManager.Instance.bulletRicochet;
        speed = GameManager.Instance.bulletSpeed;
        vampirism = GameManager.Instance.vampirism;
        searchRange = GameManager.Instance.bulletSearchRange;
        critChance = GameManager.Instance.critChance;
        critDamage = GameManager.Instance.critDamage;

        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            //Debug.LogError("BulletProjectile: Missing Rigidbody!");
            Destroy(gameObject);
            return;
        }

        rb.useGravity = false;
        rb.linearDamping = 0f;

        //Destroy(gameObject, lifetime);
    }

    private void FixedUpdate()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }
        Vector3 direction = (target.position - transform.position).normalized;
        transform.rotation = Quaternion.LookRotation(direction, Vector3.up);

        rb.linearVelocity = transform.forward * speed;
        speed += Time.deltaTime;
    }

    public void SetTarget(Transform tran)
    {
        target = tran;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Enemy")) return;

        EnemysAI eai = other.GetComponent<EnemysAI>();
        if (eai == null) return;

        float damageMultipler = Random.value < critChance ? 2 + critDamage : 1;
        int totalDamage = Mathf.RoundToInt(damage * damageMultipler);

        eai.UpdateHealth(-totalDamage);
        SpawnDamageText(-totalDamage, Mathf.RoundToInt(damageMultipler));
        UpgradeStatistics.Instance.RecordEndStatistic("Damage", totalDamage);

        TryApplyVampirism(totalDamage);

        ricochet--;
        damage = Mathf.RoundToInt(damage * 0.9f);

        if (ricochet <= 0 || damage < 5)
        {
            Destroy(gameObject);
            return;
        }

        SearchNewTarget();

    }

    private void TryApplyVampirism(int dealDamage)
    {
        if (!GameManager.Instance.player.CheckHealth()) return;

        int healAmount = Mathf.RoundToInt(dealDamage * vampirism / 1000);
        if (healAmount < 0) return;

        GameManager.Instance.player.UpdateHealth(healAmount);
        UpgradeStatistics.Instance.RecordEndStatistic("Heals", healAmount);
    }

    //old

    /*private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            EnemysAI eai = other.GetComponent<EnemysAI>();
            int critMultiple = 1;
            if (Random.value < critChance) critMultiple = 2;
            eai.UpdateHealth(-damage * critMultiple);
            SpawnDamageText(-damage * critMultiple, critMultiple);
            UpgradeStatistics.Instance.RecordEndStatistic("Damage", damage);

            if (GameManager.Instance.player.CheckHealth())
            {
                int vamp = Mathf.RoundToInt(damage * vampirism / 1000);
                GameManager.Instance.player.UpdateHealth(vamp);
                UpgradeStatistics.Instance.RecordEndStatistic("Heals", vamp);
            }

            ricochet--;
            float newDamage = damage * 0.9f;
            damage = Mathf.RoundToInt(newDamage);
            if (ricochet <= 0 || damage < 5)
            {
                //Debug.Log("Bullet destroy, ricochet: " + ricochet + " || damage: " + damage + " || speed: " + speed);
                Destroy(gameObject);
                return;
            }

            SearchNewTarget();
            //Debug.Log("Bullet Projectile: bullet hitted!");
        }
    }*/

    private void SpawnDamageText(int damage, int color)
    {
        GameObject damageText = Instantiate(damageTextPrefab, transform.position + Vector3.up, Quaternion.identity);
        DamagePopup dp = damageText.GetComponent<DamagePopup>();
        if (dp != null) dp.SetDamageText(damage, color);
    }

    private void SearchNewTarget()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, searchRange);
        Transform newTarget = null;

        foreach (Collider col in hitColliders)
        {
            if (col.CompareTag("Enemy") && col.gameObject != target?.gameObject)
            {
                newTarget = col.transform;
                break;
            }
        }
        if (newTarget != null)
        {
            SetTarget(newTarget);
            StartCoroutine(ResetCollide());
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private IEnumerator ResetCollide()
    {
        Collider collide = GetComponent<Collider>();

        collide.enabled = false;
        yield return new WaitForSeconds(0.3f);
        collide.enabled = true;
    }

    /*private void OnDestroy()
    {
        Debug.Log("Bullet speed: " + speed);
    }*/
}
