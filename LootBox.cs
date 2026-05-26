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

        cost = GameManager.Instance.lootBoxCost;
        healthCost = GameManager.Instance.cursedBoxCost;

        CreateInteract();
    }

    private void CreateInteract()
    {
        //int costdis = isCursed ? Mathf.RoundToInt(cost * GameManager.Instance.cursedMultiple) : cost;
        string text = isCursed ? $"[E] {healthCost} HP" : $"[E] {cost}$";
        //string newText = $"[E] {costdis}$";

        Instantiate(interact, transform.position + Vector3.up, Quaternion.identity)
            .GetComponent<DamagePopup>()
            .SetSpeedup(text, 0f);
    }

    public void SetList(List<UpgradeData> list)
    {
        upgrades = list;
    }
}
