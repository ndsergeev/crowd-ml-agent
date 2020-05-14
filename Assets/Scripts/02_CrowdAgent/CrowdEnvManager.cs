using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrowdEnvManager : MonoBehaviour
{
    public Camera myCamera;
    public GameObject instancePlane;
    public GameObject instanceAgent;
    public GameObject instanceFlag;
    public GameObject instanceObstacle;

    private Renderer m_FloorMat;

    private static Color FloorColor = new Color(5/255, 30/255, 36/255, 1);

    public List<GameObject> m_Agents = new List<GameObject>();
    // public List<GameObject> m_Flags = new List<GameObject>();

    public int maxInstanceNumber;
    public int planeSquareScale;
    private float m_MinX;
    private float m_MaxX;
    private float m_MinZ;
    private float m_MaxZ;
    public int m_FailCounter = 0;
    public int m_FlagCounter = 0;

    private Coroutine m_Coroutine = null;

    private void Start() {
        if (myCamera == null) {
            myCamera = Camera.main;
        }

        var angle = 75 * Mathf.PI / 180;

        myCamera.transform.position = new Vector3(Mathf.Cos(angle) * planeSquareScale * .6f,
                                                Mathf.Sin(angle) * planeSquareScale * .6f, 0);

        instancePlane.transform.localScale = new Vector3(planeSquareScale, 1, planeSquareScale);

        m_MinX = -instancePlane.transform.localScale.x / 2;
        m_MaxX = instancePlane.transform.localScale.x / 2;

        m_MinZ = -instancePlane.transform.localScale.z / 2;
        m_MaxZ = instancePlane.transform.localScale.z / 2;

        m_FloorMat = instancePlane.GetComponent<Renderer>();
        m_FloorMat.material.color = FloorColor;

        SpawnAgentsAndFlags();
    }

    private void Update() {
        if (m_FlagCounter > maxInstanceNumber - 1) {
            EndEpoch(Color.green);
        } else if (maxInstanceNumber - m_FailCounter - m_FlagCounter < 1) {
            EndEpoch(Color.red);
        }
    }

    public Vector3 RandomPosition() {
        return new Vector3(instancePlane.transform.position.x + Random.Range(m_MinX, m_MaxX),
                           instancePlane.transform.position.y + .5f,
                           instancePlane.transform.position.z + Random.Range(m_MinZ, m_MaxZ));
    }

    GameObject SpawenerAtThePlane(GameObject instance) {
        GameObject go = GameObject.Instantiate(instance,
                                               RandomPosition(),
                                               Quaternion.identity,
                                               gameObject.transform);
        return go;
    }

    GameObject SpawenerAtThePlane(GameObject instance, Quaternion rotation) {
        GameObject go = GameObject.Instantiate(instance,
                                               RandomPosition(),
                                               rotation,
                                               gameObject.transform);
        return go;
    }

    public void SpawnAgentsAndFlags() {
        for (int i = 0; i < maxInstanceNumber; ++i) {
            // var col = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));

            var agent = SpawenerAtThePlane(instanceAgent);
            var agentComponent = agent.GetComponent<CrowdAgent>();
            agentComponent.individualTag = i;
            agent.transform.GetChild(0).GetComponent<Renderer>().material.color = Color.green;
            m_Agents.Add(agent);

            var flag = SpawenerAtThePlane(instanceFlag, Quaternion.Euler(0, 90, 0));
            flag.transform.GetChild(1).GetComponent<Renderer>().material.color = Color.red;
        }
    }

    public void FailIncrement() {
        m_FailCounter++;
    }

    public void FlagIncrement() {
        m_FlagCounter++;
    }

    IEnumerator RewardAndChangeMaterial(Color col, float time) {
        m_FloorMat.material.color = col;
        
        yield return new WaitForSeconds(time); // Wait for 'time' sec
        m_FloorMat.material.color = FloorColor;
    }

    public void EndEpoch(Color col) {
        if (m_Coroutine == null) {
            m_Coroutine = StartCoroutine(RewardAndChangeMaterial(col, 2f));
        }

        m_FlagCounter = 0;
        m_FailCounter = 0;

        ActivateAllFlags();

        foreach (var agent in m_Agents) {
            agent.GetComponent<CrowdAgent>().EndEpisode();
        }
    }

    public void SpawnFlag() {
        var flag = SpawenerAtThePlane(instanceFlag, Quaternion.Euler(0, 90, 0));
        flag.transform.GetChild(1).GetComponent<Renderer>().material.color = Color.red;
    }

    public void ActivateAllFlags() {
        foreach (Transform child in transform) {
            child.gameObject.SetActive(true);
            child.position = RandomPosition();
        }
    }

    public void ResetCounters() {
        m_FlagCounter = 0;
        m_FailCounter = 0;
    }
}
