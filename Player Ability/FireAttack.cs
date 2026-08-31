using System.Collections.Generic;
using UnityEngine;

public class FireAttack : MonoBehaviour
{
    private AudioSource audioSource;
    public AudioClip[] impactClips;
    private List<EnemysAI> enemys = new();

    [Header("Damage Settings")]
    private float tickInterval = 1f;
    private float nextTickTime;
    private int damage;
    private float fireMultiple = 1f;

    [Header("Circle Setting")]
    private float growSpeed = 1f;
    //private float rotateSpeed = 45f;

    private void Start()
    {
        damage = GameManager.Instance.bulletDamage;
        audioSource = GetComponent<AudioSource>();

        Destroy(gameObject, 15f);
    }

    private void Update()
    {
        transform.localScale += new Vector3(growSpeed, 0f, growSpeed) * Time.deltaTime;
        //transform.Rotate(Vector3.up * (rotateSpeed * Time.deltaTime));

        if (Time.time < nextTickTime || enemys.Count == 0) return;

        bool hitAny = false;
        for (int i = enemys.Count - 1; i >= 0; i--)
        {
            if (enemys[i] == null)
            {
                enemys.RemoveAt(i);
                continue;
            }

            enemys[i].UpdateHealth(-Mathf.RoundToInt((damage / 10f) * fireMultiple));
            hitAny = true;
        }

        if (hitAny)
        {
            audioSource.PlayOneShot(impactClips[Random.Range(0, impactClips.Length)]);
            fireMultiple += 0.5f;
        }
        nextTickTime = Time.time + tickInterval;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<EnemysAI>(out var ai) && !enemys.Contains(ai)) enemys.Add(ai);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<EnemysAI>(out var ai)) enemys.Remove(ai);
    }
}
