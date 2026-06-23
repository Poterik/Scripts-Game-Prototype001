using UnityEngine;

public class HealCapsule : MonoBehaviour
{
    private int healPoint = 25;
    private float lifetime = 120f;

    private PlayerFighter fighter;

    private void Start()
    {
        fighter = GameManager.Instance.player;

        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player") || !fighter.CheckHealth()) return;

        int heal = healPoint + GameManager.Instance.recovery;
        fighter.UpdateHealth(heal);
        StartCoroutine(fighter.ShowHealAura());
        Destroy(this.gameObject);
    }
}
