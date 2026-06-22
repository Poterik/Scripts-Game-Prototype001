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

    [Header("Fight")]
    public float attackCD = 2.5f;
    public int baseHealth = 100;
    protected int health;
    public int baseDamage = 10;
    protected int damage;
    public int damageDivider;
    protected bool isAttacking = false;
    protected bool isColliding = false;
    protected bool isDead;
    protected int expForDead = 50;
    public int bounty = 5;
    public float speed = 3;
    private float saveSpeed;
    public float stoppingDistance = 1.3f;  //1.3

    protected virtual void Start()
    {
        animator = GetComponent<Animator>();
        //player = GameManager.Instance.player.gameObject.transform;
        player = FindAnyObjectByType<PlayerFighter>().GetComponent<Transform>();
        collide = GetComponent<SphereCollider>();
        //bounty = bounty + (GameManager.Instance.lootBoxCost / 3);
        bounty = bounty + (GameManager.Instance.gameDifferent - 1) * 2;

        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        //health = baseHealth * GameManager.Instance.gameDifferent;
        //health = 10 + (int)(baseHealth * Mathf.Pow(GameManager.Instance.gameDifferent, 1.5f));
        health = baseHealth * (GameManager.Instance.gameDifferent * GameManager.Instance.gameDifferent);
        //damage = baseDamage + GameManager.Instance.gameDifferent * 2;
        //damage = baseDamage + (GameManager.Instance.gameDifferent * GameManager.Instance.gameDifferent) / 2;
        damage = GameManager.Instance.player.maxHealth / damageDivider;
        //speed = speed + GameManager.Instance.gameDifferent * 0.25f;
        saveSpeed = speed;
    }

    protected virtual void FixedUpdate()
    {
        if (isDead || GameManager.Instance.gameOver) return;

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
        //if (!canAttack) return;

        //isAttacking = false;
        PlayerFighter pf = player.GetComponent<PlayerFighter>();
        if (pf != null) pf.UpdateHealth(-damage);
        animator.SetTrigger("Attack");
        //StartCoroutine(AttackCooldown());
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
        Vector3 direction = player.position - transform.position;
        direction.y = 0;
        float distance = direction.magnitude;

        animator.SetBool("Walk", distance >= stoppingDistance);
        if (distance <= stoppingDistance) return;

        direction.Normalize();

        rb.linearVelocity = new Vector3(direction.x * speed, rb.linearVelocity.y, direction.z * speed);
        transform.rotation = Quaternion.LookRotation(direction, Vector3.up);
    }

    private void NewAcceleration()
    {
        float distance = Vector3.Distance(transform.position, player.position);

        if (distance >= 50f) speed += Time.deltaTime;
        //else speed = Mathf.Lerp(speed, saveSpeed, Time.deltaTime);
        else speed = saveSpeed;
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
