using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Spawner : MonoBehaviour
{
    public LayerMask groundLayer;
    private GameManager gameManager;
    private Transform player;

    [Header("Spawn Settings")]
    private float spawnRange = 50f;

    public List<SpawnData> spawns = new List<SpawnData>();

    private void Start()
    {
        gameManager = GameManager.Instance;
        player = gameManager.player.transform;
        if (player == null || gameManager == null) Debug.LogWarning("Spawner not found elements!");
    }

    private void Update()
    {
        if (gameManager == null || player == null) return;

        MainSpawner();
    }

    private void MainSpawner()
    {
        for (int i = 0; i < spawns.Count; i++)
        {
            var s = spawns[i];
            if (s.requiredLevel > gameManager.gameDifferent || s.lastUsedCooldown > Time.time) continue;

            Instantiate(s.prefab, GetSpawnPosition(s.spawnRangeMultiple), Quaternion.identity);
            s.lastUsedCooldown = Time.time + s.cooldown;
        }
    }

    private Vector3 GetSpawnPosition(float multiple)
    {
        Vector3 spawnPosition = player.position
            + new Vector3(Random.Range(-spawnRange, spawnRange) * multiple,
                        50f,
                        Random.Range(-spawnRange, spawnRange) * multiple);
        if (Physics.Raycast(spawnPosition, Vector3.down, out RaycastHit hit, 200f, groundLayer)) return hit.point + Vector3.up * 0.5f;
        else return transform.position;
    }
}


[Serializable]
public class SpawnData
{
    public GameObject prefab;
    public float cooldown;
    [HideInInspector] public float lastUsedCooldown;
    public int requiredLevel;
    public float spawnRangeMultiple;
}
