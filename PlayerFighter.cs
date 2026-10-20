using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerFighter : MonoBehaviour
{
    [Header("Health")]
    private Slider healthBar;
    public int maxHealth = 100;
    public int currentHealth;
    public TextMeshProUGUI currentHealthText;
    public int regeneration = 5;
    public float regenCooldown = 7.5f;
    public bool isHealing;
    public float maxDelay = 0.1f;

    [Header("References")]
    public GameObject healAura;
    private ProjectileSpawner projectileSpawner;

    private void Start()
    {
        projectileSpawner = GetComponent<ProjectileSpawner>();
        healthBar = GetComponentInChildren<Slider>();
        if (healthBar != null)
        {
            healthBar.maxValue = maxHealth;
            healthBar.value = maxHealth;
        }
        currentHealth = maxHealth;
        currentHealthText.text = currentHealth.ToString();

        healAura.SetActive(false);

        //StartCoroutine(FindEnemys());
        StartCoroutine(RegenerateHealth());
    }

    private IEnumerator RegenerateHealth()
    {
        while (true)
        {
            if (CheckHealth()) { UpdateHealth(regeneration + GameManager.Instance.recovery); StartCoroutine(ShowHealAura()); }
            yield return new WaitForSeconds(regenCooldown);
        }
    }

    public void DecreaseSearchDelay()
    {
        projectileSpawner.searchDelay = Mathf.Max(maxDelay, projectileSpawner.searchDelay - 0.1f);
        if (projectileSpawner.searchDelay <= maxDelay)
        {
            /*UpgradeData item = UpgradeManager.Instance.allUpgrades.Find(u => u.name == "Bulleter");
            UpgradeManager.Instance.allUpgrades.Remove(item);*/
            UpgradeManager.Instance.allUpgrades.RemoveAll(u => u.name == "Bulleter");
        }
    }

    public bool CheckHealth()
    {
        if (currentHealth < maxHealth) return true;
        return false;
    }

    public void UpdateHealth(int value)
    {
        currentHealth += value;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        healthBar.value = currentHealth;
        currentHealthText.text = currentHealth.ToString();
        if (value < 0) GameManager.Instance.ShowHit();

        if (currentHealth <= 0) 
        { 
            Destroy(this.gameObject);
            GameManager.Instance.GameOver();
        }
    }

    public IEnumerator ShowHealAura(float duration = 1f)
    {
        healAura.SetActive(true);

        yield return new WaitForSeconds(duration);

        if (!isHealing) healAura.SetActive(false);
    }

    public void UpdateMaxHealth(int value)
    {
        maxHealth = Mathf.Max(1, maxHealth + value);
        healthBar.maxValue = maxHealth;
    }

    public void SetMaxHealth(int value)
    {
        maxHealth = Mathf.Max(1, value);
        healthBar.maxValue = maxHealth;
    }
}
