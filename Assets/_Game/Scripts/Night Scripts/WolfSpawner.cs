using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WolfSpawner : MonoBehaviour
{
    [Header("Wolf Settings")]
    [SerializeField] private GameObject wolfPrefab;
    [SerializeField] private Transform[] spawnPoints;

    [Header("Timer & Pacing")]
    [SerializeField] private float defenseDuration = 60f;
    [SerializeField] private float phase1Interval = 4.0f; // 0s - 30s: Slower spawns
    [SerializeField] private float phase2Interval = 1.8f; // 30s - 60s: Aggressive spawns

    [Header("Audio")]
    [SerializeField] private AudioClip wolfSpawnGrowl;
    [SerializeField] private AudioSource spawnerAudioSource;

    private List<GameObject> activeWolves = new List<GameObject>();
    private float timer;
    private bool isDefending = false;

    public void StartDefenseWave()
    {
        timer = defenseDuration;
        isDefending = true;
        StartCoroutine(SpawnLoop());
    }

    private void Update()
    {
        if (!isDefending) return;

        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            WinDefense();
        }
    }

    private IEnumerator SpawnLoop()
    {
        while (isDefending)
        {
            SpawnSingleWolf();

            // After 30 seconds left, spawn intervals become much faster
            float currentInterval;
            if (timer > (defenseDuration / 2f))
            {
                currentInterval = phase1Interval;
            }
            else
            {
                currentInterval = phase2Interval;
            }

            yield return new WaitForSeconds(currentInterval);
        }
    }

    private void SpawnSingleWolf()
    {
        if (spawnPoints.Length == 0) return;

        Transform randomPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        GameObject wolf = Instantiate(wolfPrefab, randomPoint.position, randomPoint.rotation);
        activeWolves.Add(wolf);

        if (spawnerAudioSource != null)
        {
            spawnerAudioSource.PlayOneShot(wolfSpawnGrowl);
        }
        else
        {
            AudioSource.PlayClipAtPoint(wolfSpawnGrowl, Camera.main.transform.position, 0.8f);
        }
    }

    public void ClearAllWolves()
    {
        foreach (var wolf in activeWolves)
        {
            Destroy(wolf);
        }
        activeWolves.Clear();
    }

    private void WinDefense()
    {
        isDefending = false;
        StopAllCoroutines();
        ClearAllWolves();
    }
}