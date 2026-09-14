using UnityEngine;
using UnityEngine.UI;

public class StationTower : MonoBehaviour
{
    [Header("Settings")]
    protected int minCapture = 0;
    protected int maxCapture = 5;
    protected float currentCapture;
    protected bool isCaptured;
    protected float lifetime = 60f;
    protected float maxDistance = 25f;

    [Header("References")]
    public Slider slider;
    protected UpgradeManager upgradeManager;
    protected Transform player;

    protected virtual void Start()
    {
        if (slider != null)
        {
            slider.minValue = minCapture;
            slider.maxValue = maxCapture;
            slider.value = currentCapture;
        }

        upgradeManager = UpgradeManager.Instance;
        player = GameManager.Instance.player.transform;
    }

    protected virtual void Update()
    {
        currentCapture = isCaptured ? currentCapture + Time.deltaTime : currentCapture - Time.deltaTime;
        currentCapture = Mathf.Clamp(currentCapture, minCapture, maxCapture);
        slider.value = currentCapture;

        if (currentCapture >= maxCapture)
        {
            RandomlyGift();
            Destroy(gameObject);
        }

        HandleCheckDistance();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        isCaptured = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        isCaptured = false;
    }

    protected void HandleCheckDistance()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.transform.position);
        if (distance < maxDistance) return;
        lifetime -= Time.deltaTime;
        if (lifetime <= 0) Destroy(gameObject);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, maxDistance);
    }

    private void RandomlyGift()
    {
        int rand = Random.Range(0, 10);
        switch (rand)
        {
            /*case < 2:
                upgradeManager.ShowRandomUpgrades(upgradeManager.legendaryUpgrades);
                break;*/
            default:
                upgradeManager.ShowRandomUpgrades(upgradeManager.abilitiesUpgrades);
                break;
        }
    }
}
