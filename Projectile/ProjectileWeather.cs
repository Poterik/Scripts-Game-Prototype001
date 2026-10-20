using System.Collections.Generic;
using UnityEngine;

public class ProjectileWeather : ProjectileBase
{
    public GameObject weather;

    [Header("Settings")]
    private float rotateSpeed = 360f;
    private float upSpeed;
    private Vector3 startPos;
    private float upPos = 7.5f;
    private bool isDead = false;

    protected override void Start()
    {
        base.Start();
        startPos = transform.position;
        upSpeed = Mathf.Max(1f, gameManager.bulletSpeed / 10f);
    }

    protected override void FixedUpdate()
    {
        if (isDead) return;
        Spinning();
    }

    protected override void OnTriggerEnter(Collider other)
    {
    }

    private void Spinning()
    {
        transform.Rotate(Vector3.up * (rotateSpeed * Time.fixedDeltaTime));
        transform.position += Vector3.up * upSpeed * Time.fixedDeltaTime;
        if (transform.position.y >= startPos.y + upPos) SpawnWeather();
    }

    private void SpawnWeather()
    {
        Instantiate(weather, transform.position, Quaternion.identity);
        audioSource.PlayOneShot(bulletRicochetSound);
        isDead = true;
        Destroy(gameObject);
    }
}
