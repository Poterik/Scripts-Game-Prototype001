using System.Collections.Generic;
using UnityEngine;

public class AbilityManager : MonoBehaviour
{
    public List<AbilityData> abilities = new List<AbilityData>();

    private void Update()
    {
        if (abilities.Count == 0) return;

        for (int i = 0; i < abilities.Count; i++)
        {
            var ability = abilities[i];

            if (!ability.isActive || Time.time < ability.lastUsedCooldown) continue;

            Instantiate(ability.prefab, GetPositionForSpawn(), Quaternion.identity);
            ability.lastUsedCooldown = Time.time + ability.cooldown;
        }
    }

    private Vector3 GetPositionForSpawn()
    {
        Vector3 spawnPos = transform.position;

        if (Physics.Raycast(spawnPos, Vector3.down, out RaycastHit hit, 200f)) return hit.point;
        return spawnPos - Vector3.down * 0.5f;
    }

    public AbilityData GetAbility(string abilityName)
    {
        return abilities.Find(a => a.abilityName == abilityName);
    }

    public void ApplyUpgrade(string name, float bonus)
    {
        var a = GetAbility(name);
        if (a == null) Debug.LogWarning("Abilities not found!");

        if (a.isActive) a.cooldown = Mathf.Max(1, a.cooldown - bonus);
        else a.isActive = true;
    }
}

[System.Serializable]
public class AbilityData
{
    public string abilityName;
    public GameObject prefab;
    public float cooldown;
    public float lastUsedCooldown = 0f;
    public bool isActive;
}
