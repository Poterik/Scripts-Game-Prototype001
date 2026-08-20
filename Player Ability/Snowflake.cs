using UnityEngine;

public class Snowflake : MonoBehaviour
{
    private AudioSource audioSource;
    public AudioClip[] iceSpawnClips;
    public AudioClip iceTouchClip;
    private int damage;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.PlayOneShot(iceSpawnClips[Random.Range(0, iceSpawnClips.Length)]);
        transform.Rotate(Vector3.right * 90f);
        transform.position += Vector3.up * 0.25f;
        damage = GameManager.Instance.bulletDamage / 2;

        Destroy(gameObject, 50f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<EnemysAI>(out EnemysAI ai))
        {
            ai.UpdateHealth(-damage);
            ai.ApplySlow();
            audioSource.PlayOneShot(iceTouchClip);
        }
    }
}
