using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityStation : StationTower
{
    private Material mat;
    private Color baseEmission = Color.cyan;
    private float intesity = 4f;
    private bool eventHorizonActivated = false;
    private float duration = 20f;

    [Header("Gravity")]
    private float basePull = 0f;
    private float pullGrowth = 0.5f;
    private float maxPull = 20f;

    private List<Transform> enemys = new();
    private float elapsed;

    protected override void Start()
    {
        base.Start();

        mat = GetComponentInChildren<Renderer>().material;
    }

    protected override void Update()
    {
        HandleCaptured();
        HandleCheckDistance();
        HandleGravity();
    }

    private void HandleGravity()
    {
        if (!eventHorizonActivated) return;

        elapsed += Time.deltaTime;
        float pull = Mathf.Min(maxPull, basePull + pullGrowth * elapsed);

        FindNearbyEnemy();

        for (int i = enemys.Count - 1; i >= 0; i--)
        {
            var e = enemys[i];
            if (e == null) { enemys.RemoveAt(i); continue; }

            Vector3 toCenter = transform.position - e.position;
            toCenter.y = 0f;
            float dist = toCenter.magnitude;

            if (dist > maxDistance) { enemys.RemoveAt(i); continue; }
            if (dist < 1f) continue;

            e.position += toCenter.normalized * pull * Time.deltaTime;
        }
    }

    private void HandleCaptured()
    {
        if (eventHorizonActivated) return;

        currentCapture = isCaptured ? currentCapture + Time.deltaTime : currentCapture - Time.deltaTime;
        currentCapture = Mathf.Clamp(currentCapture, minCapture, maxCapture);
        slider.value = currentCapture;

        if (currentCapture >= maxCapture && !eventHorizonActivated)
        {
            eventHorizonActivated = true;
            StartCoroutine(ActivateEventHorizon(0f, intesity, 10f));
            Destroy(gameObject, duration);
        }
    }

    private void FindNearbyEnemy()
    {
        Collider[] hit = Physics.OverlapSphere(transform.position, base.maxDistance);

        foreach (Collider c in hit) if (c.CompareTag("Enemy") && !enemys.Contains(c.transform)) enemys.Add(c.transform);
    }

    private IEnumerator ActivateEventHorizon(float from, float to, float time)
    {
        float t = 0f;
        while (t < time)
        {
            t += Time.deltaTime;
            SetEmission(Mathf.Lerp(from, to, t / time));
            yield return null;
        }
        Debug.Log("Event Horizon Activated");
    }

    private void SetEmission(float mult)
    {
        mat.SetColor("_EmissionColor", baseEmission * mult);
    }
}
