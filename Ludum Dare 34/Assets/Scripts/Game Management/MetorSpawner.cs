using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MetorSpawner : MonoBehaviour {

    public static MetorSpawner instance;

    public GameObject metorPrefab;

    public Transform metorParent;

    private Vector3 spawnPos;

    public int minSpawn, maxSpawn;

    public int currentMetors = 0, maxDefaultMetors = 50, maxCustomMeteors = 100;

    public bool startMetorShower = false;

    public bool useDefault = true;

	void Awake () {
        instance = this;
	}

    private void Update()
    {
        if (startMetorShower)
        {
            if (useDefault)
            {
                if (currentMetors < maxDefaultMetors)
                {
                    spawnMetor();
                }
            }
            else if (!useDefault)
            {
                if (currentMetors < maxCustomMeteors)
                {
                    spawnMetor();
                }
            }
        }
    }

    public void spawnMetor()
    {
        if (Random.value < 0.3)
        {
            spawnPos = new Vector3(transform.position.x + Random.Range(minSpawn, maxSpawn), transform.position.y, transform.position.z - Random.Range(minSpawn, maxSpawn));
        }
        else if (Random.value > 0.3 && Random.value < 0.5)
        {
            spawnPos = new Vector3(transform.position.x - Random.Range(minSpawn, maxSpawn), transform.position.y, transform.position.z + Random.Range(minSpawn, maxSpawn));
        }
        else if (Random.value > 0.5 && Random.value < 0.7)
        {
            spawnPos = new Vector3(transform.position.x + Random.Range(minSpawn, maxSpawn), transform.position.y, transform.position.z + Random.Range(minSpawn, maxSpawn));
        }
        else if (Random.value < 0.7)
        {
            spawnPos = new Vector3(transform.position.x - Random.Range(minSpawn, maxSpawn), transform.position.y, transform.position.z - Random.Range(minSpawn, maxSpawn));
        }

        GameObject spawnedMetor = Instantiate(metorPrefab, spawnPos, Quaternion.identity, metorParent);

        currentMetors++;

        return;
    }
}