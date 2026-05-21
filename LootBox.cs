using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LootBox : MonoBehaviour
{
    public List<UpgradeData> upgrades = new List<UpgradeData>();
    public GameObject interact;
    private int cost;
    private int healthCost;
    public bool isCursed;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        GameManager.Instance.currentLootBox = this.gameObject;
        GameManager.Instance.ActivateTerritoryBool(isCursed);
        cost = GameManager.Instance.lootBoxCost;
        healthCost = GameManager.Instance.cursedLootBoxCost;

        CreateInteract();
    }

    private void CreateInteract()
    {
        string text = isCursed ? $"[E] {healthCost} HP" : $"[E] {cost}$";

        Instantiate(interact, transform.position + Vector3.up, Quaternion.identity)
            .GetComponent<DamagePopup>()
            .SetSpeedup(text, 0f);
    }

    public void SetList(List<UpgradeData> list)
    {
        upgrades = list;
    }
}
