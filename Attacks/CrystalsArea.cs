using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class CrystalsArea : MonoBehaviour
{
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
        fighter = GameManager.Instance.player;
        damage = fighter.maxHealth / 5;
        StartCoroutine(PlaySoundPerSeconds());
    }

    private void OnParticleCollision(GameObject other)
    {
        if (!other.CompareTag("Player") || hasAttacked) return;

        //hasAttacked = true;
        audioSource2.PlayOneShot(iceTouchSound);
        StartCoroutine(ToggleAttack());
        StartCoroutine(fighter.SpeedDebuff());
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
