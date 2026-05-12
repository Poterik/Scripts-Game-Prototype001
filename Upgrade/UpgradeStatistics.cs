using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class UpgradeStatistics : MonoBehaviour
{
    public static UpgradeStatistics Instance;

    [Header("UI Elements")]
    public GameObject statsPanel;
    public GameObject statPrefab;
    public Transform defaultContainer;
    public GameObject endStatsAddon;
    public GameObject endStatPrefab;
    public Transform endStatContainer;

    private UpgradeManager upgradeManager;
    private Dictionary<string, int> upgradeSelections = new Dictionary<string, int>();
    private Dictionary<string, int> endStatistic = new Dictionary<string, int>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        upgradeManager = FindAnyObjectByType<UpgradeManager>();
        statsPanel.SetActive(false);
        endStatsAddon.SetActive(false);
    }

    private void Update()
    {
        if (GameManager.Instance.gameOver) return;

        if (Keyboard.current.tabKey.wasPressedThisFrame || Keyboard.current.escapeKey.wasPressedThisFrame && statsPanel.activeSelf)
        {
            ToggleStatPanel();
        }
    }

    public void RecordEndStatistic(string name, int value)
    {
        if (!endStatistic.ContainsKey(name)) endStatistic[name] = 0;
        endStatistic[name] += value;
    }

    public void RecordUpgradeSelection(string upgradeName)
    {
        if (!upgradeSelections.ContainsKey(upgradeName)) upgradeSelections[upgradeName] = 0;

        upgradeSelections[upgradeName]++;
    }

    private void ToggleStatPanel()
    {
        if (upgradeManager.upgradeWindow.activeSelf || UIManager.Instance.escapeMenu.activeSelf) return;

        bool willOpen = !statsPanel.activeSelf;
        statsPanel.SetActive(willOpen);
        UIManager.Instance.UpdateGameState(willOpen);

        if (willOpen)
        {
            RefreshStatsDisplay();
        }
    }

    public void ShowEndStatistic()
    {
        UIManager.Instance.escapeMenuAddon.SetActive(true);
        endStatsAddon.SetActive(true);

        foreach (var end in endStatistic)
        {
            if (end.Value > 0)
            {
                GameObject stat = Instantiate(endStatPrefab, endStatContainer);
                TextMeshProUGUI text = stat.GetComponentInChildren<TextMeshProUGUI>();
                text.text = $"{end.Key}: {end.Value:N0}";
            }
        }
    }

    private void RefreshStatsDisplay()
    {
        foreach (Transform child in defaultContainer)
        {
            Destroy(child.gameObject);
        }

        foreach (var kvp in upgradeSelections)
        {
            if (kvp.Value > 0)
            {
                GameObject item = Instantiate(statPrefab, defaultContainer);
                StatItemUI itemUI = item.GetComponent<StatItemUI>();
                if (itemUI != null)
                {
                    itemUI.SetData($"{kvp.Key}: {kvp.Value}");
                }
            }
        }
    }
}
