using UnityEngine;

public class MagicCircle : MonoBehaviour
{
    [Header("References")]
    private GameManager gameManager;
    private PlayerFighter fighter;

    [Header("Settings")]
    private float scale;
    private bool hasAttacking;
    private float scaleMultiple = 1f;
    private float lifetime = 30f;
    private float damage = 10;

    private void Start()
    {
        gameManager = GameManager.Instance;
        fighter = gameManager.player;
        damage *= gameManager.gameDifferent;

        Destroy(this.gameObject, lifetime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player") || hasAttacking) return;

        hasAttacking = true;

        //fighter.UpdateHealth(Mathf.RoundToInt(-damage));
        fighter.UpdateHealth(-fighter.maxHealth / 5);
    }

    private void Update()
    {
        scale += Time.deltaTime * scaleMultiple;
        transform.localScale = new Vector3(scale, 5f, scale);

        damage += Time.deltaTime;
        scaleMultiple += Time.deltaTime;
    }
}
