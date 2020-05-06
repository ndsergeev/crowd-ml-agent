using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;
using MLAgents;
using MLAgents.Sensors;

public class CrowdAgent : Agent
{
    public int individualTag = 0;
    public Transform agentFlag = null;
    private GameObject m_Floor;
    private Rigidbody m_AgentRb;
    private CrowdEnvManager m_Parent;
    private float m_Punisher;

    public override void Initialize() {
        m_Parent = transform.parent.gameObject.GetComponent<CrowdEnvManager>();

        m_Floor = m_Parent.instancePlane;

        m_AgentRb = GetComponent<Rigidbody>();

        m_Punisher = 0f;

        m_AgentRb.angularVelocity = Vector3.zero;
        m_AgentRb.velocity = Vector3.zero;
    }

    public override void CollectObservations(VectorSensor sensor) {
        // sensor.AddObservation(transform.InverseTransformDirection(m_agentRb.velocity));

        // sensor.AddObservation(gameObject.transform.position.x);
        // sensor.AddObservation(gameObject.transform.position.z);
        // sensor.AddObservation(m_agentRb.velocity.x);
        // sensor.AddObservation(m_agentRb.velocity.z);

        // sensor.AddObservation(m_floor.transform.position.x);
        // sensor.AddObservation(m_floor.transform.position.y);

        sensor.AddObservation(agentFlag.position.x);
        sensor.AddObservation(agentFlag.position.z);
    }

    public void MoveAgent(float[] act) {
        var dirToGo = Vector3.zero;
        var rotateDir = Vector3.zero;

        var action = Mathf.FloorToInt(act[0]);

        switch (action) {
            case 1:
                dirToGo = transform.forward * -1f;
                break;
            case 2:
                dirToGo = transform.forward * 1f;
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

    public override void OnActionReceived(float[] vectorAction) {
        MoveAgent(vectorAction);

        if (transform.position.y < m_Floor.transform.position.y) {
            AddReward(-5f);
            m_Parent.Increment();
            transform.position = m_Parent.RandomPosition();
        } else {
            m_Punisher += 0.05f;

            AddReward(-m_Punisher / maxStep);
        }
    }

    void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.CompareTag("Flag")) {
            if (collision.gameObject.GetComponent<CrowdFlag>().individualTag == individualTag) {
                SetReward(5f);

                m_Parent.EndEpoch(Color.green);

            } else {
                SetReward(-1f);
            }
        } else if (collision.gameObject.CompareTag("Agent")) {
            SetReward(-1f);
        }
    }

    public override void OnEpisodeBegin() {
        m_AgentRb.angularVelocity = Vector3.zero;
        m_AgentRb.velocity = Vector3.zero;
        m_Punisher = 0f;

        m_Parent.BeginEpoch();
    }
}
