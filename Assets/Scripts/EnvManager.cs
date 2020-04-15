using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvManager : MonoBehaviour
{
    public GameObject instanceAgent;
    public GameObject instanceFlag;
    public GameObject instanceObstacle;

    public float minX;
    public float maxX;
    public float minZ;
    public float maxZ;

    private void Start() {
        minX = -49.0f;
        maxX = 49.0f;

        minZ = -49.0f;
        maxZ = 49.0f;
    }

    public void SpawnAgent() {
        GameObject.Instantiate(
            // RANDOM
            // instanceAgent, new Vector3(Random.Range(minX, maxX), gameObject.transform.position.y, Random.Range(minZ, maxZ)), 
            // Quaternion.identity);

            instanceAgent, new Vector3(20, gameObject.transform.position.y, 20), Quaternion.identity);
            // Invoke("SpawnAgent", spawnTime);
    }

    public void SpawnFlag() {
    }

    public void SpawnObstacle() {
        
    }
}
