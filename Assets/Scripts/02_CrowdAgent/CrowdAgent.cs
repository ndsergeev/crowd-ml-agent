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
    private bool m_Done;
    private bool m_Fail;

    public override void Initialize() {
        m_Done = false;
        m_Fail = false;

        m_Parent = transform.parent.gameObject.GetComponent<CrowdEnvManager>();

        m_Floor = m_Parent.instancePlane;

        m_AgentRb = GetComponent<Rigidbody>();

        m_Punisher = 0f;

        m_AgentRb.angularVelocity = Vector3.zero;
        m_AgentRb.velocity = Vector3.zero;
    }

    public override void CollectObservations(VectorSensor sensor) {
        // sensor.AddObservation(transform.InverseTransformDirection(m_agentRb.velocity));
        sensor.AddObservation(System.Convert.ToInt32(m_Done));
    }

    public void MoveAgent(float[] act) {
        var dirToGo = Vector3.zero;
        var rotateDir = Vector3.zero;

        if (m_Done == false && m_Fail == false) {
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
        }

        transform.Rotate(rotateDir, Time.fixedDeltaTime * 200f);
        m_AgentRb.AddForce(dirToGo * 2f, ForceMode.VelocityChange);
    
    }

    public override void OnActionReceived(float[] vectorAction) {
        MoveAgent(vectorAction);

        if (transform.position.y < m_Floor.transform.position.y && m_Fail == false) {
            AddReward(-3f);
            m_Parent.FailIncrement();
            m_Fail = true;
        }
        
        if (m_Done == false && m_Fail == false) {
            m_Punisher += 0.05f;
            AddReward(-m_Punisher / maxStep);
        }
    }

    void OnCollisionEnter(Collision collision) {
        if (m_Done == false && m_Fail == false) {
            if (collision.gameObject.CompareTag("Flag")) {
                // SetReward(2 * m_Parent.m_FlagCounter / m_Parent.maxInstanceNumber);
                SetReward(1);
                collision.gameObject.SetActive(false);
                m_Parent.FlagIncrement();

                m_Done = true;

                m_AgentRb.angularVelocity = Vector3.zero;
                m_AgentRb.velocity = Vector3.zero;
            } else if (collision.gameObject.CompareTag("Agent")) {
                SetReward(-1f);
            }
        }
    }

    public override void OnEpisodeBegin() {
        m_AgentRb.angularVelocity = Vector3.zero;
        m_AgentRb.velocity = Vector3.zero;
        m_Punisher = 0f;

        transform.position = m_Parent.RandomPosition();

        m_Fail = false;
        m_Done = false;

        m_Parent.ResetCounters();
    }
}
