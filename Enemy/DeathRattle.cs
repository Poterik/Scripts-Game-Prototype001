using UnityEngine;

public class DeathRattle : MonoBehaviour
{
    public GameObject[] prefabs;
    public AudioClip[] audioClips;
    private Transform player;
    private AudioSource audioSource;
    private bool isQuitting;

    private void Start()
    {
        player = GameManager.Instance.player.GetComponent<Transform>();
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.outputAudioMixerGroup = GameManager.Instance.sfxGroup;
    }

    private void OnApplicationQuit()
    {
        isQuitting = true;
    }

    private void OnDestroy()
    {
        if (!Application.isPlaying || isQuitting) return;

        if (prefabs.Length <= 0) return;

        audioSource.PlayOneShot(audioClips[Random.Range(0, audioClips.Length)]);
        Instantiate(prefabs[Random.Range(0, prefabs.Length)], player.transform.position, Quaternion.identity);
    }
}
