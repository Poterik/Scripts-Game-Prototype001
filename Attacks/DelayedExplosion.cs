using System.Collections;
using UnityEngine;

public class DelayedExplosion : MonoBehaviour
{
    private Collider collide;
    private GameManager gameManager;
    private PlayerFighter fighter;

    private float timeToActivate = 1f;
    private int damage; //10
    private bool hasAttacked;

    private void Start()
    {
        gameManager = GameManager.Instance;
        collide = GetComponent<Collider>();
        fighter = gameManager.player;

        collide.enabled = false;
        //damage *= gameManager.gameDifferent;
        damage = fighter.maxHealth / 10;

        StartCoroutine(ActivateCollider());
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasAttacked)
        {
            hasAttacked = true;

            fighter.UpdateHealth(-damage);
        }
    }

    private IEnumerator ActivateCollider()
    {
        yield return new WaitForSeconds(timeToActivate);

        collide.enabled = true;

        yield return new WaitForSeconds(timeToActivate / 5);

        Destroy(this.gameObject);
    }
}
