using System;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;
using MLAgents;
using MLAgents.Sensors;

public class CrowdAgent : Agent
{
    public GameObject env;
    private Rigidbody m_AgentRb;
    private CrowdEnv m_Env;
    private GameObject m_destination;

    public override void Initialize()
    {
        m_AgentRb = GetComponent<Rigidbody>();
        m_Env = transform.parent.gameObject.GetComponent<CrowdEnv>();
    }

    public override void CollectObservations(VectorSensor sensor)
    {

        sensor.AddObservation(transform.InverseTransformDirection(m_AgentRb.velocity));

    }

    public void MoveAgent(float[] act)
    {
        var destination = Vector3.zero;
        var rotateToDestination = Vector3.zero;


        transform.Rotate(rotateToDestination, Time.deltaTime * 200f);
        m_AgentRb.AddForce(destination * 2f, ForceMode.VelocityChange);
    }

    public override void OnActionReceived(float[] vectorAction)
    {
        AddReward(-1f / maxStep);
        MoveAgent(vectorAction);
    }

    public override void OnEpisodeBegin()
    {

    }

    void OnCollisionEnter(Collision other)
    {
        
    }
}
