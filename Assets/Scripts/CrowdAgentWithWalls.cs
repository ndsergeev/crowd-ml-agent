using System;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;
using MLAgents;
using MLAgents.Sensors;

public class CrowdAgentWithWalls : Agent
{
    public GameObject FlagGameObject;
    public GameObject Floor;
    private Renderer m_floorMat;
    private Rigidbody m_AgentRb;

    public float minX;
    public float maxX;
    public float minZ;
    public float maxZ;

    private void Start() {
        m_floorMat = Floor.GetComponent<Renderer>();
        m_floorMat.material.color = new Color(5/255, 30/255, 36/255, 1);

        minX = -45.0f;
        maxX = 45.0f;

        minZ = -45.0f;
        maxZ = 45.0f;

        m_AgentRb.angularVelocity = Vector3.zero;
        m_AgentRb.velocity = Vector3.zero;
        gameObject.transform.position = new Vector3( 0, 0.5f, 0);
    }

    public override void Initialize()
    {
        m_AgentRb = GetComponent<Rigidbody>();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.InverseTransformDirection(m_AgentRb.velocity));

        sensor.AddObservation(gameObject.transform.position);
        // sensor.AddObservation(m_AgentRb.velocity.x);
        // sensor.AddObservation(m_AgentRb.velocity.z);

        sensor.AddObservation(FlagGameObject.transform.position);
    }

    public void MoveAgent(float[] act)
    {
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
        }
        transform.Rotate(rotateDir, Time.deltaTime * 200f);
        m_AgentRb.AddForce(dirToGo * 2f, ForceMode.VelocityChange);
    }

    public override void OnActionReceived(float[] vectorAction)
    {
        // if (Mathf.Abs(transform.position.x) > 47 || Mathf.Abs(transform.position.z) > 47)
        // {
        //     AddReward(-0.1f);
        //     EndEpisode();
        // }
        // else
        // {
        // if (Vector3.Distance(FlagGameObject.transform.position, gameObject.transform.position) < 5) {
        //     AddReward(2f);
        //     EndEpisode();
        // }
        // if (transform.position.y<0) {
        //     AddReward(-1f);
        //     EndEpisode();
        // }

        if (Vector3.Distance(FlagGameObject.transform.position, gameObject.transform.position) < 2f) {
            AddReward(1.5f);
            EndEpisode();
        }
        AddReward(.01f / Vector3.Distance(FlagGameObject.transform.position, gameObject.transform.position) - 0.0004f); // I think I can make an equation to balance with
        MoveAgent(vectorAction);
        // }
    }

    // void OnCollisionEnter(Collision collision)
    // {
    //     if (collision.gameObject.CompareTag("Finish"))
    //     {
    //         m_floorMat.material.color = Color.green;
    //         SetReward(2f);
    //         EndEpisode();
    //     }
    // }

    public override void OnEpisodeBegin()
    {
        m_floorMat.material.color = new Color(5/255, 30/255, 36/255, 1);
        m_AgentRb.angularVelocity = Vector3.zero;
        m_AgentRb.velocity = Vector3.zero;
        gameObject.transform.position = new Vector3( 0, 0.5f, 0);

        FlagGameObject.transform.position = new Vector3(Random.Range(minX, maxX),
                                                        0,
                                                        Random.Range(minZ, maxZ));
    }
}
