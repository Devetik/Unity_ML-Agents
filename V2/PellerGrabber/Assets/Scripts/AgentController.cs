using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class AgentMove : Agent
{
    public override void OnEpisodeBegin()
    {
        int rand = Random.Range(0,2);
        if(rand == 0)
        {
            target.localPosition = new Vector3(Random.Range(1,4), 0f,Random.Range(1,4));
            transform.localPosition = new Vector3(Random.Range(-1,-4), 0f,Random.Range(-1,-4));
        }
        if(rand == 1)
        {
            target.localPosition = new Vector3(Random.Range(-1,-4), 0f,Random.Range(-1,-4));
            transform.localPosition = new Vector3(Random.Range(1,4), 0f,Random.Range(1,4));
        }
    }
    [SerializeField] private Transform target;
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(target.localPosition);
    }
    public override void OnActionReceived(ActionBuffers action)
    {
        float moveSpeed = 2f;
        float moveX = action.ContinuousActions[0];
        float moveY = action.ContinuousActions[1];

        transform.localPosition += new Vector3(moveX, 0f, moveY) * Time.deltaTime * moveSpeed;
    }

    public override void Heuristic(in ActionBuffers actionOut)
    {
        ActionSegment<float> continuousActions = actionOut.ContinuousActions;
        continuousActions[0] = Input.GetAxisRaw("Horizontal");
        continuousActions[1] = Input.GetAxisRaw("Vertical");
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Target")
        {
            AddReward(10f);
            EndEpisode();
        }
        if(other.gameObject.tag == "Walls")
        {
            AddReward(-5f);
            EndEpisode();
        }
    }
}
