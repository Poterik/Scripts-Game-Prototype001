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

    [Header("Settings")]
    private float scale;
    private bool hasAttacking;
    private float scaleMultiple = 1f;
    private float lifetime = 30f;
    private float damage = 10;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        gameManager = GameManager.Instance;
        fighter = gameManager.player;
        damage *= gameManager.gameDifferent;

        StartCoroutine(SwapSound());

        Destroy(this.gameObject, lifetime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player") || hasAttacking) return;

        hasAttacking = true;

        audioSource.PlayOneShot(circleTouch[Random.Range(0, circleTouch.Length)]);

        //fighter.UpdateHealth(Mathf.RoundToInt(-damage));
        fighter.UpdateHealth(-fighter.maxHealth / 5);
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
        scale += Time.deltaTime * scaleMultiple;
        transform.localScale = new Vector3(scale, 5f, scale);

        damage += Time.deltaTime;
        scaleMultiple += Time.deltaTime;
    }
}
