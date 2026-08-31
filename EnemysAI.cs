using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class EnemysAI : MonoBehaviour
{
    protected Transform player;
    private NavMeshAgent agent;
    protected Animator animator;
    protected Rigidbody rb;
    private SphereCollider collide;
    protected AudioSource audioSource;
    protected AudioClip[] attackSound;

    [Header("Fight")]
    public float attackCD = 2.5f;
    public int baseHealth = 100;
    public int health;
    public int baseDamage = 10;
    protected int damage;
    public int damageDivider;
    protected bool isAttacking = false;
    protected bool isColliding = false;
    protected bool isDead;
    protected int expForDead = 50;
    public int bounty = 5;
    public float speed = 3;
    public float stoppingDistance = 1.3f;  //1.3
    private float startDifficulty = 0.3f;
    private float endDifficulty = 0.9f;

    [Header("Ice")]
    private float slowMultiplier = 1f;
    private float slowTimer = 0f;

    [Header("Toxic")]
    public float vulnerability = 1f;
    private float toxicTimer = 0f;

    protected void Awake()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.outputAudioMixerGroup = GameManager.Instance.sfxGroup;
        audioSource.spatialBlend = 1f;
        attackSound = GameManager.Instance.slimeAttackSound;
    }

    protected virtual void Start()
    {
        animator = GetComponent<Animator>();
        //player = GameManager.Instance.player.gameObject.transform;
        player = FindAnyObjectByType<PlayerFighter>().GetComponent<Transform>();
        collide = GetComponent<SphereCollider>();
        //bounty = bounty + (GameManager.Instance.lootBoxCost / 3);
        bounty = Mathf.RoundToInt(bounty * Mathf.Lerp(1f, 25f, GameManager.Instance.gameDifferent / 100f));

        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        //health = Mathf.RoundToInt(baseHealth * Mathf.Pow(1f + GameManager.Instance.gameDifferent * 0.4f, 3f));
        float k = Mathf.Lerp(startDifficulty, endDifficulty, GameManager.Instance.gameDifferent / 100f);
        health = Mathf.RoundToInt(baseHealth * Mathf.Pow(1f + GameManager.Instance.gameDifferent * k, 3f));

        float difficultyMultiplier = 1f + (GameManager.Instance.gameDifferent * 0.15f);
        //damage = Mathf.RoundToInt(baseDamage * difficultyMultiplier);
        float maxDamage = GameManager.Instance.player.maxHealth * 0.9f;
        damage = Mathf.RoundToInt(Mathf.Min(baseDamage * difficultyMultiplier, maxDamage));
    }

    protected virtual void FixedUpdate()
    {
        if (isDead || GameManager.Instance.gameOver) return;

        HandleSlowIce();
        HandleToxicTime();
        NewHandleMovement();
        //HandleAcceleration();
        //NewAcceleration();
    }

    protected virtual void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isColliding = true;
            //NewHandleAttack();
            StartAttack();
        }
    }

    protected void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isColliding = false;
        }
    }

    public void ApplyToxic(float multiple = 0.2f, float duration = 10f)
    {
        vulnerability += multiple;
        toxicTimer += duration;
    }

    protected void HandleToxicTime()
    {
        if (toxicTimer > 0)
        {
            toxicTimer -= Time.deltaTime;
            if (toxicTimer <= 0)
            {
                vulnerability = 1f;
            }
        }
    }

    public void ApplySlow(float multiplier = 0.2f, float duration = 7.5f)
    {
        slowMultiplier -= multiplier;
        slowTimer += duration;
        Debug.Log("Enemys: slow has been appled");
    }

    protected void HandleSlowIce()
    {
        if (slowTimer > 0)
        {
            slowTimer -= Time.deltaTime;
            if (slowTimer <= 0)
            {
                slowMultiplier = 1f;
                Debug.Log("Enemys: slow has been canceled");
            }
        }
    }

    protected void StartAttack()
    {
        if (!isAttacking)
        {
            isAttacking = true;
            NewHandleAttack();
            StartCoroutine(AttackCooldown());
        }
    }

    protected void NewHandleAttack()
    {
        audioSource.PlayOneShot(attackSound[Random.Range(0, attackSound.Length)]);
        PlayerFighter pf = player.GetComponent<PlayerFighter>();
        if (pf != null) pf.UpdateHealth(-damage);
        animator.SetTrigger("Attack");
    }

    protected IEnumerator AttackCooldown()
    {
        yield return new WaitForSeconds(attackCD);
        isAttacking = false;

        if (isColliding)
        {
            StartAttack();
        }
    }

    protected void NewHandleMovement()
    {
        float currentSpeed = speed * slowMultiplier;

        Vector3 direction = player.position - transform.position;
        direction.y = 0;
        float distance = direction.magnitude;

        animator.SetBool("Walk", distance >= stoppingDistance);
        if (distance <= stoppingDistance) return;

        direction.Normalize();

        rb.linearVelocity = new Vector3(direction.x * currentSpeed, rb.linearVelocity.y, direction.z * currentSpeed);
        transform.rotation = Quaternion.LookRotation(direction, Vector3.up);
    }

    public virtual void UpdateHealth(int value)
    {
        if (isDead) return;

        health += value;
        //if (health <= 0) Destroy(gameObject);

        if (health <= 0)
        {
            isDead = true;
            GameManager.Instance.UpdateExp(expForDead);
            GameManager.Instance.OnEnemyDied();
            GameManager.Instance.UpdateMoney(bounty);
            UpgradeStatistics.Instance.RecordEndStatistic("Kills", 1);
            Destroy(gameObject);
        }
    }
}
