using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LootBox : MonoBehaviour
{
    public List<UpgradeData> upgrades = new List<UpgradeData>();
    public GameObject interact;
    private int cost;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        GameManager.Instance.currentLootBox = this.gameObject;
        cost = GameManager.Instance.lootBoxCost;

        CreateInteract();
    }

    private void CreateInteract()
    {
        Instantiate(interact, transform.position + Vector3.up, Quaternion.identity)
            .GetComponent<DamagePopup>()
            .SetSpeedup($"[E] {cost}$", 0f);
    }


    /*private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            UpgradeManager.Instance.ShowRandomUpgrades(upgrades);
            Destroy(gameObject);
        }
    }*/

    public void SetList(List<UpgradeData> list)
    {
        upgrades = list;
    }
}
