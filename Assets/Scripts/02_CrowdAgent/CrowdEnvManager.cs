using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrowdEnvManager : MonoBehaviour
{
    public Camera camera;
    public GameObject instancePlane;
    public GameObject instanceAgent;
    public GameObject instanceFlag;
    public GameObject instanceObstacle;

    private Renderer m_FloorMat;

    private static Color FloorColor = new Color(5/255, 30/255, 36/255, 1);

    public List<GameObject> m_Agents = new List<GameObject>();
    public List<GameObject> m_Flags = new List<GameObject>();

    public int maxInstanceNumber;
    public int planeSquareScale;
    private float m_MinX;
    private float m_MaxX;
    private float m_MinZ;
    private float m_MaxZ;
    private int m_Counter = 0;

    private Coroutine m_Coroutine = null;

    private void Start() {
        if (camera == null) {
            camera = Camera.main;
        }

        var angle = 75 * Mathf.PI / 180;

        camera.transform.position = new Vector3(Mathf.Cos(angle) * planeSquareScale * .6f,
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
        if (m_Counter > 5) {
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

    public void SpawnAgentsAndFlags() {
        for (int i = 0; i < maxInstanceNumber; ++i) {
            var col = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));

            var agent = SpawenerAtThePlane(instanceAgent);
            var agentComponent = agent.GetComponent<CrowdAgent>();
            agentComponent.individualTag = i;
            agent.transform.GetChild(0).GetComponent<Renderer>().material.color = col;
            m_Agents.Add(agent);

            var flag = SpawenerAtThePlane(instanceFlag);
            flag.GetComponent<CrowdFlag>().individualTag = i;
            flag.transform.GetChild(1).GetComponent<Renderer>().material.color = col;
            m_Flags.Add(flag);

            agentComponent.agentFlag = flag.transform;
        }
    }

    public void Increment() {
        m_Counter++;
    }

    public void ShuffleObjects() {
        foreach (Transform child in transform) {
            child.position = RandomPosition();
        }
    }

    IEnumerator RewardAndChangeMaterial(Color col, float time) {
        m_FloorMat.material.color = col;
        
        yield return new WaitForSeconds(time); // Wait for 'time' sec
        m_FloorMat.material.color = FloorColor;
    }

    public void EndEpoch(Color col) {
        if (m_Coroutine == null) {
            StartCoroutine(RewardAndChangeMaterial(col, 2f));
        }

        m_Counter = 0;

        foreach (var agent in m_Agents) {
            agent.GetComponent<CrowdAgent>().EndEpisode();
        }

        foreach (var flag in m_Flags) {
            flag.transform.position = RandomPosition();
        }
    }

    // Just in case function
    public void BeginEpoch() {
        m_FloorMat.material.color = FloorColor;
    }
}
