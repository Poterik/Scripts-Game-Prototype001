using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class DropLaser : MonoBehaviour
{
    private bool isDealingDamage;
    private Coroutine damageRoutine;
    private PlayerFighter fighter;
    public AudioMixerGroup sfxMixer;
    private AudioSource source1;
    private AudioSource source2;
    public AudioClip laserSound;

    private int damage;

    private void Start()
    {
        fighter = GameManager.Instance.player;
        if (fighter == null) Debug.Log("Player not fouynd");

        damage = fighter.maxHealth / 25;

        SourceInitilize();

        Destroy(gameObject, 4f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player") || isDealingDamage) return;

        isDealingDamage = true;
        damageRoutine = StartCoroutine(DamagePerSecond());
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        isDealingDamage = false;
        if (damageRoutine != null) StopCoroutine(damageRoutine);
    }

    private void SourceInitilize()
    {
        source1 = GetComponent<AudioSource>();
        source2 = gameObject.AddComponent<AudioSource>();

        AudioSource[] sources = { source1, source2 };

        for (int i = 0; i < sources.Length; i++)
        {
            sources[i].loop = true;
            sources[i].spatialBlend = 1f;
            sources[i].generator = laserSound;
            if (sfxMixer != null) sources[i].outputAudioMixerGroup = sfxMixer;
            else Debug.LogWarning("SFX not found!");
        }

        source1.Play();
        source2.PlayDelayed(0.25f);
    }

    private IEnumerator DamagePerSecond()
    {
        while (isDealingDamage)
        {
            fighter.UpdateHealth(-damage);
            yield return new WaitForSeconds(1f);
        }
    }
}
