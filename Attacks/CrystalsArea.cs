using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class CrystalsArea : MonoBehaviour
{
    private MyPlayerControl playerControl;
    private PlayerFighter fighter;
    private bool hasAttacked;
    private int damage;

    private AudioSource audioSource;
    private AudioSource audioSource2;
    public AudioMixerGroup sfxGroup;
    public AudioClip iceStartSound;
    public AudioClip iceTouchSound;

    private void Awake()
    {
        audioSource2 = gameObject.AddComponent<AudioSource>();
        audioSource2.loop = false;
        audioSource2.playOnAwake = false;
        if (sfxGroup != null) audioSource2.outputAudioMixerGroup = sfxGroup;
    }

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        playerControl = GameManager.Instance.player.GetComponent<MyPlayerControl>();
        fighter = GameManager.Instance.player.GetComponent<PlayerFighter>();
        damage = Mathf.RoundToInt(2.5f * GameManager.Instance.gameDifferent);
        StartCoroutine(PlaySoundPerSeconds());
    }

    private void OnParticleCollision(GameObject other)
    {
        if (!other.CompareTag("Player") || hasAttacked) return;

        //hasAttacked = true;
        audioSource2.PlayOneShot(iceTouchSound);
        StartCoroutine(ToggleAttack());
        playerControl.ApplySlowDebuf();
        fighter.UpdateHealth(-damage);
    }

    private IEnumerator ToggleAttack()
    {
        hasAttacked = true;
        yield return new WaitForSeconds(1f);
        hasAttacked = false;
    }

    private IEnumerator PlaySoundPerSeconds()
    {
        while (true)
        {
            audioSource.PlayOneShot(iceStartSound);
            yield return new WaitForSeconds(2f);
        }
    }
}
