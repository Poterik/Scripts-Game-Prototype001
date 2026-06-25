using System.Collections;
using UnityEngine;

public class HealCircle : MonoBehaviour
{
    private PlayerFighter fighter;
    private Coroutine healRoutine;
    private bool isCollide;

    private int healingCount = 5;
    private int recovery = 0;

    private void Start()
    {
        fighter = GameManager.Instance.player;
        recovery = GameManager.Instance.recovery;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player") || fighter.isHealing) return;

        fighter.isHealing = true;
        isCollide = true;
        fighter.healAura.SetActive(true);
        healRoutine = StartCoroutine(HealPerSecond());
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        fighter.isHealing = false;
        isCollide = false;
        //fighter.healAura.SetActive(false);
        StartCoroutine(fighter.ShowHealAura());
        if (healRoutine != null) StopCoroutine(healRoutine);
    }

    private void OnDestroy()
    {
        if (!isCollide || fighter == null) return;

        fighter.isHealing = false;
        fighter.healAura.SetActive(false);
    }

    public void SetLifeTime(float lifetime)
    {
        Destroy(gameObject, lifetime);
    }

    private IEnumerator HealPerSecond()
    {
        while (fighter.isHealing)
        {
            fighter.UpdateHealth(healingCount + recovery);

            yield return new WaitForSeconds(1f);
        }
    }
}
