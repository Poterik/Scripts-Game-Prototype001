using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerFighter : MonoBehaviour
{
    private MyPlayerControl controller;

    [Header("Health")]
    private Slider healthBar;
    public int maxHealth = 100;
    public int currentHealth;
    public TextMeshProUGUI currentHealthText;
    public int regeneration = 5;

    [Header("Fight")]
    public GameObject bulletPrefab;
    public float attackRange = 10f;
    public float searchDelay = 2.5f;
    public int maxTarget = 2;

    [Header("References")]
    public GameObject healAura;


    private void Start()
    {
        controller = GetComponent<MyPlayerControl>();

        healthBar = GetComponentInChildren<Slider>();
        if (healthBar != null)
        {
            healthBar.maxValue = maxHealth;
            healthBar.value = maxHealth;
        }
        currentHealth = maxHealth;
        currentHealthText.text = currentHealth.ToString();

        healAura.SetActive(false);

        StartCoroutine(FindEnemys());
        StartCoroutine(RegenerateHealth());
    }

    public IEnumerator SpeedDebuff(int amount = 2, float duration = 15f)
    {
        if (controller == null) yield break;

        controller.moveSpeed = Mathf.Max(0, controller.moveSpeed - amount);
        Debug.Log("Apply speed debuff: " + controller.moveSpeed);

        yield return new WaitForSeconds(duration);

        controller.moveSpeed += amount;
        Debug.Log("Remove speed debuff: " + controller.moveSpeed);
    }

    private IEnumerator RegenerateHealth()
    {
        while (true)
        {
            if (CheckHealth()) UpdateHealth(regeneration + GameManager.Instance.recovery);
            yield return new WaitForSeconds(7.5f);
        }
    }

    private IEnumerator FindEnemys()
    {
        while (true)
        {
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, attackRange);

            List<Collider> enemies = new List<Collider>();
            foreach (Collider col in hitColliders)
            {
                if (col.CompareTag("Enemy"))
                    enemies.Add(col);
            }

            enemies.Sort((a, b) =>
            Vector3.Distance(transform.position, a.transform.position)
            .CompareTo(Vector3.Distance(transform.position, b.transform.position))
            );

            int target = maxTarget;
            foreach (Collider col in enemies)
            {
                if (col.CompareTag("Enemy"))
                {
                    if (target <= 0) break;
                    if (bulletPrefab == null) yield break;

                    CreateBullet(col.transform);
                    target--;
                }
            }
            
            yield return new WaitForSeconds(searchDelay);
        }
    }

    public void DecreaseSearchDelay()
    {
        searchDelay = Mathf.Max(0.1f, searchDelay - 0.1f);
        if (searchDelay <= 0.1f)
        {
            /*UpgradeData item = UpgradeManager.Instance.allUpgrades.Find(u => u.name == "Bulleter");
            UpgradeManager.Instance.allUpgrades.Remove(item);*/
            UpgradeManager.Instance.allUpgrades.RemoveAll(u => u.name == "Bulleter");
        }
    }

    private void CreateBullet(Transform transform)
    {
        //Vector3 distance = transform.position - this.transform.position;
        GameObject bullet = Instantiate(bulletPrefab, this.transform.position + Vector3.forward, Quaternion.identity);
        BulletProjectile bp = bullet.GetComponent<BulletProjectile>();
        bp.SetTarget(transform);
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
        if (value > 0) StartCoroutine(ShowHealAura());

        if (currentHealth <= 0) 
        { 
            Destroy(this.gameObject);
            GameManager.Instance.GameOver();
        }
    }

    private IEnumerator ShowHealAura(float duration = 1f)
    {
        healAura.SetActive(true);

        yield return new WaitForSeconds(duration);

        healAura.SetActive(false);
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
