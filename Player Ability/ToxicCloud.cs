using System.Collections.Generic;
using UnityEngine;

public class ToxicCloud : MonoBehaviour
{
    public AudioClip[] toxicClips;
    private List<EnemysAI> enemys = new List<EnemysAI>();
    private AudioSource audioSource;

    [Header("Settings")]
    private float tickInterval = 1.25f;
    private float nextTickTime = 0f;
    private int damage;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();

        damage = GameManager.Instance.bulletDamage / 2;

        Destroy(gameObject, 10f);
    }

    private void Update()
    {
        if (Time.time < nextTickTime || enemys.Count == 0) return;

        bool hitAny = false;
        for (int i = enemys.Count - 1;  i >= 0; i--)
        {
            if (enemys[i] == null)
            {
                enemys.RemoveAt(i);
                continue;
            }

            hitAny = true;
            enemys[i].UpdateHealth(-damage);
            enemys[i].ApplyToxic();
        }

        if (hitAny) audioSource.PlayOneShot(toxicClips[Random.Range(0, toxicClips.Length)]);

        nextTickTime = Time.time + tickInterval;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<EnemysAI>(out EnemysAI ai) && !enemys.Contains(ai)) enemys.Add(ai);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<EnemysAI>(out EnemysAI ai) && !enemys.Contains(ai)) enemys.Remove(ai);
    }
}
