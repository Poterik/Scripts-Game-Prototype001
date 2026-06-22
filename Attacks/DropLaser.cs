using System.Collections;
using UnityEngine;

public class DropLaser : MonoBehaviour
{
    private bool isDealingDamage;
    private Coroutine damageRoutine;
    private PlayerFighter fighter;

    private int damage;

    private void Start()
    {
        fighter = GameManager.Instance.player;
        if (fighter == null) Debug.Log("Player not fouynd");

        damage = fighter.maxHealth / 25;

        Destroy(gameObject, 5f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player") || isDealingDamage) return;

        isDealingDamage = true;
        damageRoutine = StartCoroutine(DamagePerSecond());
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        isDealingDamage = false;
        if (damageRoutine != null) StopCoroutine(damageRoutine);
    }

    private IEnumerator DamagePerSecond()
    {
        while (isDealingDamage)
        {
            fighter.UpdateHealth(-damage);
            yield return new WaitForSeconds(1f);
        }
    }
}
