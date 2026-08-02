using UnityEngine;
using UnityEngine.UI;

public class StationTower : MonoBehaviour
{
    [Header("Settings")]
    private int minCapture = 0;
    private int maxCapture = 5;
    private float currentCapture;
    private bool isCaptured;

    [Header("References")]
    public Slider slider;
    private UpgradeManager upgradeManager;

    private void Start()
    {
        if (slider != null)
        {
            slider.minValue = minCapture;
            slider.maxValue = maxCapture;
            slider.value = currentCapture;
        }

        upgradeManager = UpgradeManager.Instance;
    }

    private void Update()
    {
        currentCapture = isCaptured ? currentCapture + Time.deltaTime : currentCapture - Time.deltaTime;
        currentCapture = Mathf.Clamp(currentCapture, minCapture, maxCapture);
        slider.value = currentCapture;

        if (currentCapture >= maxCapture)
        {
            RandomlyGift();
            Destroy(gameObject);
        }
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

    private void RandomlyGift()
    {
        int rand = Random.Range(0, 10);
        switch (rand)
        {
            /*case < 1:
                upgradeManager.ShowRandomUpgrades(upgradeManager.cursedUpgrades);
                break;*/
            case < 2:
                upgradeManager.ShowRandomUpgrades(upgradeManager.legendaryUpgrades);
                break;
            default:
                upgradeManager.ShowRandomUpgrades(upgradeManager.allUpgrades);
                break;
        }
    }
}
