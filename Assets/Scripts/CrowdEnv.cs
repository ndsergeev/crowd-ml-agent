using UnityEngine;
using MLAgents;
using MLAgentsExamples;

public class CrowdEnv : Area
{
    public override void ResetArea()
    {
        ResetWalls();
        PositionAgents();

    }

    private static Vector3 ChooseRandomPosition()
    {
        return Vector3.zero;
    }

    private void ResetWalls()
    {

    }

    private void PositionAgents()
    {

    }
}
