using System.Collections;
using UnityEngine;

public class MagicCircle : MonoBehaviour
{
    [Header("References")]
    private GameManager gameManager;
    private PlayerFighter fighter;
    private AudioSource audioSource;
    public AudioClip circleSound;
    public AudioClip[] circleTouch;
    private BossAI boss;

    [Header("Settings")]
    private bool isCollide = false;
    private float growSpeed = 3f; //3f
    private float tickInterval = 5f;
    private float lastTickInterval = 0f;
    private int damage = 1;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        gameManager = GameManager.Instance;
        fighter = gameManager.player;
        damage *= gameManager.gameDifferent;
        boss = FindAnyObjectByType<BossAI>();
        if (boss == null) Debug.Log("Boss not found");

        StartCoroutine(SwapSound());

    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        isCollide = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        isCollide = false;
    }

    private IEnumerator SwapSound()
    {
        yield return new WaitForSeconds(1f);
        audioSource.loop = true;
        audioSource.generator = circleSound;
        audioSource.Play();
    }

    private void Update()
    {
        if (boss == null)
        {
            Destroy(gameObject);
            return;
        }

        if (transform.localScale.x > 0f) transform.localScale -= new Vector3(growSpeed, 0f, growSpeed) * Time.deltaTime;

        if (Time.time <= lastTickInterval || isCollide) return;

        lastTickInterval = Time.time + tickInterval;
        fighter.UpdateHealth(-damage);
        audioSource.PlayOneShot(circleTouch[Random.Range(0, circleTouch.Length)]);
    }
}
