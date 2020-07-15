using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;

public class SingleAgent : Agent
{
    public GameObject FlagGameObject;
    public GameObject Floor;
    private Renderer m_floorMat;
    private Rigidbody m_AgentRb;

    private static Color FloorColor = new Color(5/255, 30/255, 36/255, 1);

    private float m_minX;
    private float m_maxX;
    private float m_minZ;
    private float m_maxZ;

    private float m_punisher;

    // private void Start() {

    // }

    public override void Initialize()
    {
        m_floorMat = Floor.GetComponent<Renderer>();
        m_floorMat.material.color = FloorColor;

        m_AgentRb = GetComponent<Rigidbody>();


        m_minX = -45.0f;
        m_maxX = 45.0f;

        m_minZ = -45.0f;
        m_maxZ = 45.0f;

        m_punisher = 0f;

        m_AgentRb.angularVelocity = Vector3.zero;
        m_AgentRb.velocity = Vector3.zero;
        gameObject.transform.position = new Vector3( 0, Floor.transform.position.y + 0.5f, 0);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // sensor.AddObservation(transform.InverseTransformDirection(m_AgentRb.velocity));

        // sensor.AddObservation(gameObject.transform.position.x);
        // sensor.AddObservation(gameObject.transform.position.z);
        // sensor.AddObservation(m_AgentRb.velocity.x);
        // sensor.AddObservation(m_AgentRb.velocity.z);

        // sensor.AddObservation(FlagGameObject.transform.position);
    }

    public void MoveAgent(float[] act)
    {
        Debug.Log("Length" + (act.Length).ToString());
        Debug.Log("Element" + (act[0]).ToString());

        var dirToGo = Vector3.zero;
        var rotateDir = Vector3.zero;

        var action = Mathf.FloorToInt(act[0]);

        switch (action)
        {
            case 1:
                dirToGo = transform.forward * 1f;
                break;
            case 2:
                dirToGo = transform.forward * -1f;
                break;
            case 3:
                rotateDir = transform.up * 1f;
                break;
            case 4:
                rotateDir = transform.up * -1f;
                break;
            case 5:
                dirToGo = transform.right * -0.75f;
                break;
            case 6:
                dirToGo = transform.right * 0.75f;
                break;
        }
        transform.Rotate(rotateDir, Time.fixedDeltaTime * 200f);
        m_AgentRb.AddForce(dirToGo * 2f, ForceMode.VelocityChange);
    }

    public override void OnActionReceived(float[] vectorAction)
    {
        MoveAgent(vectorAction);

        if (transform.position.y < Floor.transform.position.y)
        {
            AddReward(-5f);
            EndEpisode();
            m_punisher = 0f;

            m_floorMat.material.color = Color.red;
            StartCoroutine(RewardAndChangeMaterial(FloorColor, 1f));
        }

        m_punisher += 0.1f;

        AddReward(-m_punisher / MaxStep);
    }

    IEnumerator RewardAndChangeMaterial(Color col, float time)
    {
        yield return new WaitForSeconds(time); // Wait for 2 sec
        m_floorMat.material.color = col;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Finish"))
        {
            SetReward(20f);
            EndEpisode();
            m_punisher = 0f;

            m_floorMat.material.color = Color.green;
            StartCoroutine(RewardAndChangeMaterial(FloorColor, 1f));
        }
    }

    public override void OnEpisodeBegin()
    {
        m_floorMat.material.color = FloorColor;
        m_AgentRb.angularVelocity = Vector3.zero;
        m_AgentRb.velocity = Vector3.zero;
        gameObject.transform.position = new Vector3( 0, Floor.transform.position.y + 0.5f, 0);

        // FlagGameObject.transform.position = new Vector3(20, 0, 20);

        FlagGameObject.transform.position = new Vector3(Random.Range(m_minX, m_maxX),
                                                        Floor.transform.position.y,
                                                        Random.Range(m_minZ, m_maxZ));
    }
}
