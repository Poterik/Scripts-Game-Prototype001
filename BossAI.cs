using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossAI : EnemysAI
{
    [Header("Attack References")]
    public GameObject magicCircle;
    public GameObject delayedExplosion;
    public GameObject laserAOE;
    public GameObject crystalsArea;
    public GameObject crystalsFall;

    [Header("References")]
    public Slider bossHealthSlider;
    private GameManager gameManager;

    [Header("Cooldown Settings")]
    private float lastUsedCooldown;
    private float circleCooldown = 30f;
    private float explosionCooldown = 15f;
    private float perExplosionCooldown = 0.5f;
    private float laserCooldown = 10f;
    private float crystalsCooldown = 30f;
    private float waitStateCooldown = 2.5f;
    private float crystalsAreaCooldown = 2.5f;

    //[Header("Attack Settings")]

    private enum BossState { Wait, Idle, Circle, Explosion, Laser, Crystals, CrystalsArea }
    private BossState currentState = BossState.Wait;
    private Dictionary<BossState, float> allUsedCooldownPool = new();
    private BossState[] meleeAttackPool = { BossState.Explosion, BossState.Laser, BossState.Crystals };


    protected override void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        gameManager = GameManager.Instance;
        player = FindAnyObjectByType<PlayerFighter>().GetComponent<Transform>();
        bossHealthSlider = gameManager.bossHealthSlider;

        bossHealthSlider.gameObject.SetActive(true);
        baseHealth *= gameManager.gameDifferent * 2;
        bossHealthSlider.maxValue = baseHealth;
        health = baseHealth;
        bossHealthSlider.value = health;
        damage = baseDamage;
        expForDead *= 10;

        lastUsedCooldown = Time.time + waitStateCooldown;
    }

    private void Update()
    {
        switch (currentState)
        {
            case BossState.Wait:
                Wait();
                break;
            case BossState.Idle:
                Idle();
                break;
            case BossState.Circle:
                Circle();
                break;
            case BossState.Explosion:
                Explosion();
                break;
            case BossState.Laser:
                Laser();
                break;
            case BossState.Crystals:
                Crystals();
                break;
        }
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    protected override void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Destroy(collision.gameObject);
            UpdateHealth(100);
        }

        if (!collision.gameObject.CompareTag("Player") || isDead || !TryUseAttack(BossState.CrystalsArea)) return;
        //if (!TryUseAttack(BossState.Crystals)) return;

        Instantiate(crystalsArea, transform.position, Quaternion.identity);
        Debug.Log("Used Crystals Area");

        allUsedCooldownPool[BossState.CrystalsArea] = Time.time + crystalsAreaCooldown;
    }

    private void Wait()
    {
        if (Time.time < lastUsedCooldown) return;
        currentState = BossState.Idle;
    }


    private void Idle()
    {
        float dist = Vector3.Distance(transform.position, player.transform.position);

        if (dist > 50f && gameManager.gameDifferent >= 30)
        {
            if (TryUseAttack(BossState.Circle)) currentState = BossState.Circle;
            return;
        }

        speed = dist > 50f ? 0 : 7;
        currentState = GetMeleeAttack();
    }

    private BossState GetMeleeAttack()
    {
        foreach (var atk in meleeAttackPool) if (TryUseAttack(atk)) return atk;

        lastUsedCooldown = Time.time + waitStateCooldown;
        return BossState.Wait;
    }

    private void Crystals()
    {
        if (gameManager.gameDifferent < 30) { ChangeCooldown(60f, BossState.Crystals); return; }

        Vector3 spawnPos = Physics.Raycast(player.transform.position, Vector3.down, out RaycastHit hit, 200f)
                        ? hit.point
                        : player.transform.position;

        Instantiate(crystalsFall, spawnPos, Quaternion.identity);
        //ChangeCooldownAndState(crystalsCooldown, BossState.Wait);
        ChangeCooldown(crystalsCooldown, BossState.Crystals);
    }

    private void Circle()
    {
        Instantiate(magicCircle, transform.position, Quaternion.identity);

        /*lastUsedCooldown = Time.time + circleCooldown;
        currentState = BossState.Wait;*/

        //ChangeCooldownAndState(circleCooldown, BossState.Wait);
        ChangeCooldown(circleCooldown, BossState.Circle);
    }

    private void Explosion()
    {
        StartCoroutine(Random.Range(0, 10) < 7 ? SpawnExplosion() : SpawnExplosionForward());

        /*lastUsedCooldown = Time.time + explosionCooldown;
        currentState = BossState.Wait;*/
        //ChangeCooldownAndState(explosionCooldown, BossState.Wait);
        ChangeCooldown(explosionCooldown, BossState.Explosion);
    }

    private IEnumerator SpawnExplosion()
    {
        for (int i = 0, count = gameManager.gameDifferent / 2; i < count; i++)
        {
            Vector3 spawnPos = Physics.Raycast(player.transform.position, Vector3.down, out RaycastHit hit, 200f)
                        ? hit.point
                        : player.transform.position;

            Instantiate(delayedExplosion, spawnPos, Quaternion.identity);
            yield return new WaitForSeconds(perExplosionCooldown);
        }
    }
    private IEnumerator SpawnExplosionForward()
    {
        for (int i = 0, count = gameManager.gameDifferent / 2; i < count; i++)
        {
            Vector3 spawnPos = Physics.Raycast(player.transform.position, Vector3.down, out RaycastHit hit, 200f)
                        ? hit.point
                        : player.transform.position;

            // New спавн обьектов вперед по отношению игрока
            Vector3 forwardDir = player.transform.forward;
            forwardDir.y = 0;
            forwardDir.Normalize();

            Vector3 finalPos = spawnPos + forwardDir * 7.5f;

            Instantiate(delayedExplosion, /*spawnPos*/finalPos, Quaternion.identity);
            yield return new WaitForSeconds(perExplosionCooldown);
        }
    }

    private void Laser()
    {
        if (gameManager.gameDifferent < 20) { ChangeCooldown(60f, BossState.Laser); return; }

        Vector3 pos = Physics.Raycast(player.transform.position, Vector3.down, out RaycastHit hit, 200f)
                ? hit.point
                : player.transform.position;
        Instantiate(laserAOE, pos, Quaternion.identity);

        //ChangeCooldownAndState(laserCooldown, BossState.Wait);
        ChangeCooldown(laserCooldown, BossState.Laser);
    }

    private void ChangeCooldown(float cooldown, BossState state)
    {
        allUsedCooldownPool[state] = Time.time + cooldown;

        lastUsedCooldown = Time.time + waitStateCooldown;
        currentState = BossState.Wait;
    }

    private bool TryUseAttack(BossState state)
    {
        if (!allUsedCooldownPool.ContainsKey(state)) allUsedCooldownPool[state] = 0;
        return Time.time >= allUsedCooldownPool[state] ? true : false;
    }

    public override void UpdateHealth(int value)
    {
        if (isDead) return;

        //base.UpdateHealth(value);
        int damage = value / 2;
        health += damage;
        bossHealthSlider.value = health;

        TriggerAnimation(damage);

        if (health <= 0)
        {
            isDead = true;
            animator.SetBool("Walk", false);

            bossHealthSlider.gameObject.SetActive(false);

            gameManager.UpdateExp(expForDead);
            gameManager.UpdateMoney(bounty);
            UpgradeStatistics.Instance.RecordEndStatistic("Boss Kills", 1);
            TriggerAnimation(1800);
            Destroy(gameObject, 1f);
        }
    }

    private IEnumerator TestSpawns()
    {
        while (true)
        {
            Instantiate(magicCircle, transform.position, Quaternion.identity);
            yield return new WaitForSeconds(5f);
        }
    }

    private IEnumerator SpawnLaserAOE()
    {
        while (true)
        {
            yield return new WaitForSeconds(5f);
            Vector3 pos = Physics.Raycast(player.transform.position, Vector3.down, out RaycastHit hit, 200f) 
                ? hit.point 
                : player.transform.position;
            Instantiate(laserAOE, pos, Quaternion.identity);
        }
    }

    private IEnumerator SpawnDelayedExplosion()
    {
        while (true)
        {
            float dist = Vector3.Distance(transform.position, player.transform.position);

            if (dist < 50f)
            {
                for (int i = 0, count = gameManager.gameDifferent / 2; i < count; i++)
                {
                    Vector3 spawnPos = Physics.Raycast(player.transform.position, Vector3.down, out RaycastHit hit, 200f) 
                        ? hit.point 
                        : player.transform.position;

                    Instantiate(delayedExplosion, spawnPos, Quaternion.identity);
                    yield return new WaitForSeconds(0.5f);
                }
                yield return new WaitForSeconds(10f);
            }
            else yield return null;
        }
    }

    private IEnumerator SpawnMagicCircle()
    {
        while (true)
        {
            float dist = Vector3.Distance(transform.position, player.transform.position);

            if (dist > 50f)
            {
                Instantiate(magicCircle, transform.position, Quaternion.identity);
                yield return new WaitForSeconds(circleCooldown);
                Debug.Log("Player in distance");
            }
            else
            {
                yield return null;
            }
        }
    }

    private void TriggerAnimation(int damage)
    {
        animator.SetTrigger("Damage");
        animator.SetInteger("DamageType", GetHitAnimation(damage));
    }

    private int GetHitAnimation(int damage)
    {
        if (damage == 1800) return 2;
        if (damage <= -2500) return 1;
        return 0;
    }
}
