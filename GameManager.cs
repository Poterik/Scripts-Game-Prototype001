using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("Money")]
    public TextMeshProUGUI moneyDisplay;
    private int money = 0;
    public GameObject moneyPrefab;
    public GameObject currentLootBox;
    public int lootBoxCost;
    public bool lootBoxTerr;

    [Header("References")]
    public PlayerFighter player;

    [Header("Level Settings")]
    public Slider levelSlider;
    public TextMeshProUGUI levelText;
    public int gameDifferent = 1;
    private int expForDiff = 100;
    private int currentExp;
    public bool gameOver = false;
    private int lifetime;

    [Header("Upgrade Settings")]
    public int bulletDamage = 50;
    public int bulletRicochet = 3;
    public float bulletSpeed = 8f;
    public int vampirism = 0;
    public float bulletSearchRange = 10f;
    //public float dropRate = 0f;
    public float critChance = 0.1f;
    public float critDamage = 0f;

    [Header("Spawner")]
    public GameObject enemyPrefab;
    public GameObject enemyLightPrefab;
    public GameObject enemyTankPrefab;
    public float spawnDelay = 5f;
    public float rangeSpawn = 150f;
    public GameObject healCapsule;
    public int maxEnemies = 1000;
    private int currentEnemyCount = 0;
    public GameObject cursedLootBoxPrefab;
    public GameObject defaultLootBoxPrefab;
    //Story
    public TextMeshProUGUI newStoryText;
    public GameObject storyPrefab;

    private List<float> enemysSpawnDelay = new List<float> {1, 3, 5};

    public static GameManager Instance;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else 
        { 
            Destroy(gameObject); 
            return; 
        }

        Application.runInBackground = true;
    }

    private void Start()
    {
        StartCoroutine(SpawnEnemys());
        StartCoroutine(SpawnHeals());
        StartCoroutine(SpawnLootBox());
        UpdateLevelSlider();
        //StarterPack();
        SpawnStory();
        Initializer();
    }

    private void Update()
    {
        if (Keyboard.current.eKey.wasPressedThisFrame && CheckActiveWindow() && TryOpenLootBox())
        {
            LootBox lb = currentLootBox.GetComponent<LootBox>();
            UpgradeManager.Instance.ShowRandomUpgrades(lb.upgrades);
            lootBoxCost += gameDifferent; //5
            UpgradeStatistics.Instance.RecordEndStatistic("Loot Box Opens", 1);
            Destroy(currentLootBox);
        }
    }

    private void Initializer()
    {
        moneyDisplay.text = $"{money:N0}$";

        StartCoroutine(LifeTimer());
    }

    public void ActivateTerritoryBool()
    {
        StartCoroutine(ChangeTerritoryBool());
    }

    private IEnumerator ChangeTerritoryBool()
    {
        lootBoxTerr = true;
        yield return new WaitForSeconds(1f);
        lootBoxTerr = false;
    }

    private bool CheckActiveWindow()
    {
        if (UpgradeManager.Instance.upgradeWindow.activeSelf || UIManager.Instance.escapeMenu.activeSelf) return false;
        return true;
    }

    private bool TryOpenLootBox()
    {
        if (money < lootBoxCost || !lootBoxTerr || currentLootBox == null) return false;
        UpdateMoney(-lootBoxCost);
        return true;
    }

    public void UpdateMoney(int bounty)
    {
        //int getMoney = bounty + (gameDifferent - 1) * 2;
        money += bounty;
        moneyDisplay.text = $"{money:N0}$";
        if (bounty > 0) UpgradeStatistics.Instance.RecordEndStatistic("Money", bounty);
        Instantiate(moneyPrefab, player.transform.position + Vector3.right * 1.25f, Quaternion.identity)
            .GetComponent<DamagePopup>()
            .SetName($"{bounty}$");
    }

    private IEnumerator LifeTimer()
    {
        while (!gameOver)
        {
            lifetime += 1;
            UpgradeStatistics.Instance.RecordEndStatistic("Lifetime Mins", 1);
            yield return new WaitForSeconds(60f);
        }
    }

    private void SpawnStory()
    {
        int count = storyPrefab.GetComponent<StoryBox>().GetCount();

        for (int i = 0; i < count; i++)
        {
            GameObject storyBox = Instantiate(storyPrefab, SetRandomPosition(rangeSpawn), Quaternion.identity);
            StoryBox sb = storyBox.GetComponent<StoryBox>();
            if (sb != null) sb.SetValue(i, newStoryText);
        }
    }

    private IEnumerator SpawnEnemys()
    {
        while (true && !gameOver)
        {
            if (currentEnemyCount >= maxEnemies)
            {
                yield return new WaitForSeconds(2f);
                continue;
            }

            GameObject prefab = GetEnemy();

            Instantiate(prefab, SetRandomPosition(rangeSpawn / 3f), Quaternion.identity);
            currentEnemyCount++;
            yield return new WaitForSeconds(spawnDelay);
        }
    }

    private GameObject GetEnemy()
    {
        if (gameDifferent < 3) return enemyPrefab;
        if (gameDifferent < 6) return Random.Range(0, 10) < 7 ? enemyPrefab : enemyLightPrefab;

        int rand = Random.Range(0, 10);
        return rand switch
        {
            < 5 => enemyPrefab,
            < 8 => enemyLightPrefab,
            _ => enemyTankPrefab,
        };
    }

    public void OnEnemyDied()
    {
        currentEnemyCount = Mathf.Max(0, currentEnemyCount - 1);
    }

    private IEnumerator SpawnHeals()
    {
        while (true)
        {
            Instantiate(healCapsule, SetRandomPosition(rangeSpawn), Quaternion.identity);
            yield return new WaitForSeconds(spawnDelay * 5);
        }
    }

    private IEnumerator SpawnLootBox()
    {
        while (true)
        {
            //float dropRate = Mathf.Sqrt(gameDifferent) / 10f;

            if (Random.value <= 0f + gameDifferent / 100f)
            {
                Instantiate(cursedLootBoxPrefab, SetRandomPosition(rangeSpawn / 3), Quaternion.identity)
                    .GetComponent<LootBox>()
                    .SetList(UpgradeManager.Instance.cursedUpgrades);
            }
            else
            {
                Instantiate(defaultLootBoxPrefab, SetRandomPosition(rangeSpawn / 3), Quaternion.identity)
                    .GetComponent<LootBox>()
                    .SetList(UpgradeManager.Instance.allUpgrades);
            }

            yield return new WaitForSeconds(spawnDelay * 5);
        }
    }

    private void StarterPack()
    {
        for (int i = 0; i < 10; i++)
        {
            Instantiate(defaultLootBoxPrefab, SetRandomPosition(10), Quaternion.identity)
                .GetComponent<LootBox>()
                .SetList(UpgradeManager.Instance.allUpgrades);
        }
        Instantiate(cursedLootBoxPrefab, SetRandomPosition(10), Quaternion.identity)
                .GetComponent<LootBox>()
                .SetList(UpgradeManager.Instance.cursedUpgrades);
    }

    private Vector3 SetRandomPosition(float range)
    {
        Vector3 randomPos = new Vector3(Random.Range(-range, range), 20, Random.Range(-range, range));
        //Vector3 spawnPos = player.position + randomPos;
        //Vector3 spawnPos = transform.position + randomPos;
        Vector3 spawnPos = player.transform.position + randomPos;
        if (Physics.Raycast(spawnPos, Vector3.down, out RaycastHit hit, 200f))
        { 
            return hit.point + Vector3.up * 0.5f; 
        }
        else
        {
            Debug.LogWarning("No ground found at " + spawnPos + ". Using fallback position.");
            return transform.position + Vector3.up * 2f;
        }
    }

    public void UpdateExp(int value)
    {
        currentExp += value;

        if (currentExp >= expForDiff)
        {
            LevelUp();
        }
        else
        {
            levelSlider.value = currentExp;
        }
    }

    private void LevelUp()
    {
        currentExp = 0;

        gameDifferent++;

        //expForDiff += expForDiff / 2;
        //expForDiff -= expForDiff % 5;
        expForDiff = 100 + gameDifferent * 200;

        float minSpawnDelay = 0.5f;
        spawnDelay = Mathf.Max(spawnDelay / gameDifferent, minSpawnDelay);

        UpgradeManager.Instance.ShowRandomUpgrades(UpgradeManager.Instance.legendaryUpgrades);
        UpdateLevelSlider();
    }

    private void UpdateLevelSlider()
    {
        levelSlider.maxValue = expForDiff;
        levelSlider.value = currentExp;
        levelText.text = $"LvL: {gameDifferent}";
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        UIManager.Instance.UpdateGameState(false);
    }

    public void GameOver()
    {
        /*Time.timeScale = 0f;
        Time.fixedDeltaTime = 0f;*/
        if (gameOver) return;

        gameOver = true;
        UIManager.Instance.escapeMenu.SetActive(true);
        UIManager.Instance.settingAddon.SetActive(false);
        UpgradeStatistics.Instance.ShowEndStatistic();
        UIManager.Instance.UpdateGameState(true);
    }
}
