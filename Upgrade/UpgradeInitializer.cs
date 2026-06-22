using UnityEngine;
using System;

[DefaultExecutionOrder(-100)]
public class UpgradeInitializer : MonoBehaviour
{
    private GameManager gameManager;

    private void Start()
    {
        gameManager = FindAnyObjectByType<GameManager>();
        UpgradeManager upgradeManager = FindAnyObjectByType<UpgradeManager>();
        if (upgradeManager == null)
        {
            Debug.LogError("UpgradeSystem not found!");
            return;
        }

        InitializeUpgrades(upgradeManager);
    }

    private void InitializeUpgrades(UpgradeManager manager)
    {
        //Default Upgrades
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
        Bind("Attrange", u => GameManager.Instance.player.attackRange += u.bonus, manager.allUpgrades);
        Bind("Targeter", u => GameManager.Instance.player.maxTarget += u.bonus, manager.allUpgrades);
        Bind("Ricorange", u => GameManager.Instance.bulletSearchRange += u.bonus, manager.allUpgrades);
        //Bind("Luckerok", u => GameManager.Instance.dropRate += u.bonus / 100f, manager.allUpgrades);
        //Bind("Criter", u => gameManager.critChance += u.bonus / 100f, manager.allUpgrades);
        //Bind("Regenerator", u => gameManager.player.regeneration += u.bonus, manager.allUpgrades);
        Bind("Recover", u => gameManager.recovery += u.bonus, manager.allUpgrades);
        //Bind("Multicrit", u => gameManager.critDamage += u.bonus / 100f, manager.allUpgrades);

        //Legendary Upgrades
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
        Bind("Extra Attrange", u => GameManager.Instance.player.attackRange += u.bonus, manager.legendaryUpgrades);
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

        //Cursed Upgrades
        Bind("Cursed Damage", u => gameManager.bulletDamage += u.bonus, manager.cursedUpgrades);
        Bind("Cursed Health", u => gameManager.player.UpdateMaxHealth(u.bonus), manager.cursedUpgrades);
        Bind("Cursed Ricochette", u => gameManager.bulletRicochet += u.bonus, manager.cursedUpgrades);
        //Bind("Cursed Targeter", u => gameManager.player.maxTarget += u.bonus, manager.cursedUpgrades);
        Bind("Cursed Attrange", u => gameManager.player.attackRange += u.bonus, manager.cursedUpgrades);
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

        Debug.Log("✓ Upgrades initialized");
    }

    private void Bind(string name, System.Action<UpgradeData> action, System.Collections.Generic.List<UpgradeData> pool)
    {
        UpgradeData upgrade = pool.Find(u => u.name == name);
        if (upgrade != null) upgrade.applyAction = action;
        else Debug.LogWarning($"Upgrade '{name}' not found in pool");
    }
}
