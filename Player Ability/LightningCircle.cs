using UnityEngine;

public class LightningCircle : MonoBehaviour
{
    public AudioClip[] lightningClips;
    public GameObject orbit;
    private Transform playerPos;
    private AudioSource audioSource;

    [Header("Settings")]
    private float rotatedSpeed = 135f;
    private float drainPercent = 0.25f;
    private float lifetime = 10f;

    private void Start()
    {
        playerPos = FindAnyObjectByType<PlayerFighter>().GetComponent<Transform>();
        audioSource = GetComponent<AudioSource>();

        if (playerPos == null) Debug.LogWarning("Ligthning Circle: player not found");
        if (orbit == null) Debug.LogWarning("Ligthning Circle: orbit not found");

        transform.SetParent(playerPos, false);
        transform.localPosition = new Vector3(0f, 0f, 0f);

        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        if (orbit == null || GameManager.Instance.gameOver) return;
        orbit.transform.Rotate(Vector3.up * rotatedSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Enemy")) return;

        EnemysAI eai = other.GetComponent<EnemysAI>();
        int damage = Mathf.RoundToInt(eai.health * drainPercent);
        eai.UpdateHealth(-damage);
        audioSource.PlayOneShot(lightningClips[Random.Range(0, lightningClips.Length)]);
    }
}
