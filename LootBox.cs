using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class LootBox : MonoBehaviour
{
    private List<UpgradeData> upgrades = new List<UpgradeData>();

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            UpgradeManager.Instance.ShowRandomUpgrades(upgrades);
            Destroy(gameObject);
        }
    }

    public void SetList(List<UpgradeData> list)
    {
        upgrades = list;
    }
}
