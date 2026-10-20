using UnityEngine;
using System;

[DefaultExecutionOrder(-100)]
public class UpgradeInitializer : MonoBehaviour
{
    private GameManager gameManager;
    private AbilityManager abilityManager;
    private ProjectileSpawner projectileSpawner;

    private void Start()
    {
        gameManager = FindAnyObjectByType<GameManager>();
        UpgradeManager upgradeManager = FindAnyObjectByType<UpgradeManager>();
        if (upgradeManager == null)
        {
            Debug.LogError("UpgradeSystem not found!");
            return;
        }
        abilityManager = gameManager.player.GetComponent<AbilityManager>();
        if (abilityManager == null)
        {
            Debug.LogError("AbilityManager not found!");
            return;
        }
        projectileSpawner = gameManager.player.GetComponent<ProjectileSpawner>();

        InitializeUpgrades(upgradeManager);
    }

    private void InitializeUpgrades(UpgradeManager manager)
    {
        //
        //----Default Upgrades----
        //
        Bind("Ricochette", u => GameManager.Instance.bulletRicochet += u.bonus, manager.allUpgrades);
        Bind("Damage", u => GameManager.Instance.bulletDamage += u.bonus, manager.allUpgrades);
        //Bind("Health Point", u => GameManager.Instance.player?.UpdateMaxHealth(u.bonus), manager.allUpgrades);
        /*Bind("Speedster", u =>
        {
            var mpc = GameManager.Instance.player?.GetComponent<MyPlayerControl>();
            if (mpc != null)
            {
                mpc.moveSpeed += u.bonus;
            }
        }, manager.allUpgrades);*/
        Bind("Bulleter", u =>
        {
            for (int i = 0; i < u.bonus; i ++)
            {
                GameManager.Instance.player?.DecreaseSearchDelay();
            }
        },manager.allUpgrades);
        //Bind("Projectile", u => GameManager.Instance.bulletSpeed += u.bonus, manager.allUpgrades);
        //Bind("Vampirism", u => GameManager.Instance.vampirism += u.bonus, manager.allUpgrades);
        Bind("Attrange", u => projectileSpawner.attackRange += u.bonus, manager.allUpgrades);
        Bind("Targeter", u => projectileSpawner.maxTarget += u.bonus, manager.allUpgrades);
        Bind("Ricorange", u => GameManager.Instance.bulletSearchRange += u.bonus, manager.allUpgrades);
        //Bind("Luckerok", u => GameManager.Instance.dropRate += u.bonus / 100f, manager.allUpgrades);
        //Bind("Criter", u => gameManager.critChance += u.bonus / 100f, manager.allUpgrades);
        //Bind("Regenerator", u => gameManager.player.regeneration += u.bonus, manager.allUpgrades);
        Bind("Recover", u => gameManager.recovery += u.bonus, manager.allUpgrades);
        //Bind("Multicrit", u => gameManager.critDamage += u.bonus / 100f, manager.allUpgrades);
        Bind("Expalibur", u => gameManager.expMultiplier += u.bonus / 100f, manager.allUpgrades);
        Bind("GreedIsGood", u => gameManager.moneyMultiplier += u.bonus / 100f, manager.allUpgrades);

        //
        //----Legendary Upgrades----
        //
        Bind("Extra Damage", u => GameManager.Instance.bulletDamage += u.bonus, manager.legendaryUpgrades);
        Bind("Extra HP", u => GameManager.Instance.player?.UpdateMaxHealth(u.bonus), manager.legendaryUpgrades);
        Bind("Extra Ricochette", u => GameManager.Instance.bulletRicochet += u.bonus, manager.legendaryUpgrades);
        /*Bind("Extra Speedster", u =>
        {
            var mpc = GameManager.Instance.player?.GetComponent<MyPlayerControl>();
            if (mpc != null)
            {
                mpc.moveSpeed += u.bonus;
                mpc.sprintSpeed += u.bonus;
            }
        }, manager.legendaryUpgrades);*/
        Bind("Extra Projectile", u => GameManager.Instance.bulletSpeed += u.bonus, manager.legendaryUpgrades);
        //Bind("Extra Vampirism", u => GameManager.Instance.vampirism += u.bonus, manager.legendaryUpgrades);
        Bind("Extra Attrange", u => projectileSpawner.attackRange += u.bonus, manager.legendaryUpgrades);
        //Bind("Extra Targeter", u => GameManager.Instance.player.maxTarget += u.bonus, manager.legendaryUpgrades);
        Bind("Extra Ricorange", u => GameManager.Instance.bulletSearchRange += u.bonus, manager.legendaryUpgrades);
        //Bind("Extra Luckerok", u => GameManager.Instance.dropRate += u.bonus / 100f, manager.legendaryUpgrades);
        /*Bind("Extra Bulleter", u =>
        {
            for (int i = 0; i < u.bonus; i++)
            {
                GameManager.Instance.player?.DecreaseSearchDelay();
            }
        }, manager.legendaryUpgrades);*/
        Bind("Extra Criter", u => gameManager.critChance += u.bonus / 100f, manager.legendaryUpgrades);
        Bind("Extra Multicrit", u => gameManager.critDamage += u.bonus / 100f, manager.legendaryUpgrades);
        Bind("Extra Regenerator", u => gameManager.player.regeneration += u.bonus, manager.legendaryUpgrades);

        //
        //----Cursed Upgrades----
        //
        Bind("Cursed Damage", u => gameManager.bulletDamage += u.bonus, manager.cursedUpgrades);
        Bind("Cursed Health", u => gameManager.player.UpdateMaxHealth(u.bonus), manager.cursedUpgrades);
        Bind("Cursed Ricochette", u => gameManager.bulletRicochet += u.bonus, manager.cursedUpgrades);
        //Bind("Cursed Targeter", u => gameManager.player.maxTarget += u.bonus, manager.cursedUpgrades);
        Bind("Cursed Attrange", u => projectileSpawner.attackRange += u.bonus, manager.cursedUpgrades);
        Bind("Cursed Projectile", u => gameManager.bulletSpeed += u.bonus, manager.cursedUpgrades);
        Bind("Cursed Multicrit", u => gameManager.critDamage += u.bonus / 100f, manager.cursedUpgrades);
        //Bind("Cursed Vampirism", u => gameManager.vampirism += u.bonus, manager.cursedUpgrades);
        /*Bind("Cursed Bulleter", u =>
        {
            for (int i = 0; i < u.bonus; i++)
            {
                gameManager.player.DecreaseSearchDelay();
            }
        }, manager.cursedUpgrades);*/

        //
        //----Abilities Upgrade----
        //
        Bind("Fire Circle", u => abilityManager.ApplyUpgrade("Fire Circle", u.bonus), manager.abilitiesUpgrades);
        Bind("Light Circle", u => abilityManager.ApplyUpgrade("Light Circle", u.bonus), manager.abilitiesUpgrades);
        Bind("Aura Buffer", u => abilityManager.ApplyUpgrade("Aura Buffer", u.bonus), manager.abilitiesUpgrades);
        Bind("Ice Flakes", u => abilityManager.ApplyUpgrade("Ice Flakes", u.bonus), manager.abilitiesUpgrades);
        Bind("Toxic Cloud", u => abilityManager.ApplyUpgrade("Toxic Cloud", u.bonus), manager.abilitiesUpgrades);

        //
        //----Projectile Upgrade----
        //
        Bind("Bullet", u => projectileSpawner.projectilePrefab = projectileSpawner.projectileVariants[u.bonus], manager.projectileUpgrades);
        Bind("Pusher", u => projectileSpawner.projectilePrefab = projectileSpawner.projectileVariants[u.bonus], manager.projectileUpgrades);
        Bind("Hex", u => projectileSpawner.projectilePrefab = projectileSpawner.projectileVariants[u.bonus], manager.projectileUpgrades);
        Bind("Weather", u =>
        {
            projectileSpawner.projectilePrefab = projectileSpawner.projectileVariants[u.bonus];
            UpgradeManager.Instance.allUpgrades.RemoveAll(u => u.name == "Targeter");
            gameManager.player.maxDelay = 0.5f;
        }, manager.projectileUpgrades);
        Bind("Fire", u => projectileSpawner.projectilePrefab = projectileSpawner.projectileVariants[u.bonus], manager.projectileUpgrades);
        Bind("Frozen", u => projectileSpawner.projectilePrefab = projectileSpawner.projectileVariants[u.bonus], manager.projectileUpgrades);
        Bind("Grenade", u => projectileSpawner.projectilePrefab = projectileSpawner.projectileVariants[u.bonus], manager.projectileUpgrades);
        Bind("Concussive", u => projectileSpawner.projectilePrefab = projectileSpawner.projectileVariants[u.bonus], manager.projectileUpgrades);
        Bind("Bounce", u => projectileSpawner.projectilePrefab = projectileSpawner.projectileVariants[u.bonus], manager.projectileUpgrades);
        Bind("Mine", u => projectileSpawner.projectilePrefab = projectileSpawner.projectileVariants[u.bonus], manager.projectileUpgrades);
        Bind("Black Hole", u =>
        {
            projectileSpawner.projectilePrefab = projectileSpawner.projectileVariants[u.bonus];
            gameManager.player.maxDelay = 0.5f;
            UpgradeManager.Instance.allUpgrades.RemoveAll(u => u.name == "Ricochette");
            UpgradeManager.Instance.legendaryUpgrades.RemoveAll(u => u.name == "Extra Ricochette");
            UpgradeManager.Instance.cursedUpgrades.RemoveAll(u => u.name == "Cursed Ricochette");
        }, manager.projectileUpgrades);
        Bind("Satellite", u => projectileSpawner.projectilePrefab = projectileSpawner.projectileVariants[u.bonus], manager.projectileUpgrades);

        Debug.Log("✓ Upgrades initialized");
    }

    private void Bind(string name, System.Action<UpgradeData> action, System.Collections.Generic.List<UpgradeData> pool)
    {
        UpgradeData upgrade = pool.Find(u => u.name == name);
        if (upgrade != null) upgrade.applyAction = action;
        else Debug.LogWarning($"Upgrade '{name}' not found in pool");
    }
}
