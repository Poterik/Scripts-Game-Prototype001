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

    [Header("Fight")]
    public GameObject bulletPrefab;
    public float attackRange = 10f;
    public float searchDelay = 2.5f;
    public int maxTarget = 2;

    private void Start()
    {
        healthBar = GetComponentInChildren<Slider>();
        if (healthBar != null)
        {
            healthBar.maxValue = maxHealth;
            healthBar.value = maxHealth;
        }
        currentHealth = maxHealth;
        currentHealthText.text = currentHealth.ToString();

        StartCoroutine(FindEnemys());
        StartCoroutine(RegenerateHealth());
    }

    private IEnumerator RegenerateHealth()
    {
        while (true)
        {
            if (CheckHealth()) UpdateHealth(regeneration);
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

        if (currentHealth <= 0) 
        { 
            Destroy(this.gameObject);
            GameManager.Instance.GameOver();
        }
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
