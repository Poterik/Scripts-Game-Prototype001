using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager Instance { get; private set; }

    [Header("UI Elements")]
    public GameObject upgradeWindow;
    public Transform upgradeContainer;
    public GameObject upgradeButtonPrefab;

    [Header("All Upgrades")]
    public List<UpgradeData> allUpgrades = new List<UpgradeData>();
    public List<UpgradeData> legendaryUpgrades = new List<UpgradeData>();
    public List<UpgradeData> cursedUpgrades = new List<UpgradeData>();
    private List<UpgradeData> currentUpgrades = new List<UpgradeData>();
    private List<Button> upgradeButtons = new List<Button>();

    [Header("Settings")]
    public int upgradesToShow = 3;

    private MyPlayerControl player;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        //ShowRandomUpgrades();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<MyPlayerControl>();
    }

    private void Update()
    {
        if (Keyboard.current.fKey.wasPressedThisFrame)
        {
            //ShowRandomUpgrades(allUpgrades);
            ShowRandomUpgrades(legendaryUpgrades);
            //ShowRandomUpgrades(cursedUpgrades);
            //ApplyAllUpgrades();
        }
    }

    public void ShowRandomUpgrades(List<UpgradeData> upgrades)
    {
        List<UpgradeData> shuffled = new List<UpgradeData>(upgrades);
        for (int i = 0; i < shuffled.Count; i++)
        {
            int randomindex = Random.Range(i, shuffled.Count);
            UpgradeData temp = shuffled[i];
            shuffled[i] = shuffled[randomindex];
            shuffled[randomindex] = temp;
        }
        currentUpgrades = shuffled.Take(upgradesToShow).ToList();

        ShowWindow();
    }

    private void ApplyAllUpgrades()
    {
        foreach (UpgradeData upgraded in allUpgrades)
        {
            ApplyUpgrade(upgraded);
        }
        foreach (UpgradeData upgraded in legendaryUpgrades)
        {
            ApplyUpgrade(upgraded);
        }
        foreach (UpgradeData upgraded in cursedUpgrades)
        {
            ApplyUpgrade(upgraded);
        }
    }

    public void ApplyUpgrade(UpgradeData upgrade)
    {
        upgrade.applyAction?.Invoke(upgrade);

        UpgradeStatistics.Instance?.RecordUpgradeSelection(upgrade.name);
        UpgradeStatistics.Instance.RecordEndStatistic("Upgrades", 1);

        Debug.Log($"✓ Applied: {upgrade.name} (+{upgrade.bonus})");
    }

    private void ShowWindow()
    {
        upgradeWindow.SetActive(true);
        ShowUpgrades();

        Time.timeScale = 0;
        player.isMenu = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void CloseWindow()
    {
        upgradeWindow.SetActive(false);

        Time.timeScale = 1;
        player.isMenu = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void ShowUpgrades()
    {
        foreach (Transform child in upgradeContainer)
            Destroy(child.gameObject);
        upgradeButtons.Clear();

        for (int i = 0; i < currentUpgrades.Count;  i++)
        {
            GameObject btnObj = Instantiate(upgradeButtonPrefab, upgradeContainer);
            Button btn = btnObj.GetComponent<Button>();
            upgradeButtons.Add(btn);

            TextMeshProUGUI[] texts = btnObj.GetComponentsInChildren<TextMeshProUGUI>();
            if (texts.Length >= 2)
            {
                texts[0].text = currentUpgrades[i].name;
                texts[1].text = currentUpgrades[i].description;
            }
            Image img = btnObj.transform.Find("Icon").GetComponent<Image>();
            if (img != null) img.sprite = currentUpgrades[i].icon;

            /*TextMeshProUGUI titleText = btnObj.transform.Find("Text Name")?.GetComponent<TextMeshProUGUI>();
            if (titleText != null) titleText.text = currentUpgrades[i].name;
            TextMeshProUGUI descText = btnObj.transform.Find("Text Description")?.GetComponent<TextMeshProUGUI>();
            if (descText != null) descText.text = currentUpgrades[i].description;
            string justText = currentUpgrades[i].name + " " + currentUpgrades[i].description;*/

            int index = i;
            btn.onClick.AddListener(() => SelectUpgrade(index));
            btn.onClick.AddListener(() => CloseWindow());
        }
    }

    private void SelectUpgrade(int index)
    {
        if (index < 0 || index >= currentUpgrades.Count) return;

        ApplyUpgrade(currentUpgrades[index]);
    }
}
