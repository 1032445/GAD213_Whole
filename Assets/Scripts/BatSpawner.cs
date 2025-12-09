using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatSpawner : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public GameObject batPrefab;

    [Header("Spawn Points")]
    public Transform[] spawnPoints;
    public bool useSpawnPoints = true;

    [Header("Trigger Spawning")]
    public bool enableTriggerSpawn = true;
    public int triggerSpawnCount = 3;
    public bool destroyAfterTrigger = true;

    [Header("Periodic Spawning")]
    public bool enablePeriodicSpawn = true;
    public float spawnInterval = 8f;
    public int maxActiveBats = 5;
    private float timer;

    private int currentActiveBats = 0;

    void Start()
    {
        timer = spawnInterval;
    }

    void Update()
    {
        if (!enablePeriodicSpawn)
            return;

        timer -= Time.deltaTime;

        if (timer <= 0f && currentActiveBats < maxActiveBats)
        {
            SpawnBat();
            timer = spawnInterval;
        }
    }

    public void SpawnBat()
    {
        if (batPrefab == null || player == null)
            return;

        Transform spawnPos = null;

        if (useSpawnPoints && spawnPoints.Length > 0)
        {
            spawnPos = spawnPoints[Random.Range(0, spawnPoints.Length)];
        }
        else
        {
            Vector3 randomOffset = Random.insideUnitSphere * 10f;
            randomOffset.y = Mathf.Abs(randomOffset.y) + 1f;
            spawnPos = new GameObject("TempSpawn").transform;
            spawnPos.position = player.position + randomOffset;
        }

        GameObject bat = Instantiate(batPrefab, spawnPos.position, Quaternion.identity);

        // assign player to all enemy scripts
        BatEnemy batEnemy = bat.GetComponent<BatEnemy>();
        if (batEnemy != null)
            batEnemy.player = player;

        EnemyDamage dmg = bat.GetComponent<EnemyDamage>();
        if (dmg != null)
            dmg.player = player;

        // track deletion
        StartCoroutine(TrackExitOnDeath(bat));

        if (!useSpawnPoints)
            Destroy(spawnPos.gameObject);

        currentActiveBats++;
    }

    public void SpawnBats(int amount)
    {
        for (int i = 0; i < amount; i++)
            SpawnBat();
    }

    IEnumerator TrackExitOnDeath(GameObject bat)
    {
        while (bat != null)
            yield return null;

        currentActiveBats--;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!enableTriggerSpawn)
            return;

        if (other.CompareTag("Player"))
        {
            for (int i = 0; i < triggerSpawnCount; i++)
                SpawnBat();

            if (destroyAfterTrigger)
                Destroy(gameObject);
        }
    }
}