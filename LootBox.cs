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
    public bool isCursed;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        cost = GameManager.Instance.lootBoxCost;

        CreateInteract();
    }

    private void CreateInteract()
    {
        int costdis = isCursed ? Mathf.RoundToInt(cost * GameManager.Instance.cursedMultiple) : cost;
        //string text = isCursed ? $"[E] {cost * 1.5}$" : $"[E] {cost}$";
        string newText = $"[E] {costdis}$";

        Instantiate(interact, transform.position + Vector3.up, Quaternion.identity)
            .GetComponent<DamagePopup>()
            .SetSpeedup(newText, 0f);
    }

    public void SetList(List<UpgradeData> list)
    {
        upgrades = list;
    }
}
