using System.Collections;
using UnityEngine;

public class CrystalsArea : MonoBehaviour
{
    private PlayerFighter fighter;
    private bool hasAttacked;
    private int damage;

    private void Start()
    {
        fighter = GameManager.Instance.player;
        damage = fighter.maxHealth / 5;
    }

    private void OnParticleCollision(GameObject other)
    {
        if (!other.CompareTag("Player") || hasAttacked) return;

        //hasAttacked = true;
        StartCoroutine(ToggleAttack());
        StartCoroutine(fighter.SpeedDebuff());
        fighter.UpdateHealth(-damage);
    }

    private IEnumerator ToggleAttack()
    {
        hasAttacked = true;
        yield return new WaitForSeconds(1f);
        hasAttacked = false;
    }
}
