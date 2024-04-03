using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class AgentMove : Agent
{
    [SerializeField] private Transform target;
    [SerializeField] private float moveSpeed = 1f;
    public bool firstRun = true;
    public bool hitWalls = false;
    
    public override void OnEpisodeBegin()
    {
        int rand = Random.Range(0,2);
        if(rand == 0)
        {
            target.localPosition = new Vector3(Random.Range(1,4), 0f,Random.Range(1,4));
            if(firstRun || hitWalls)
            {
                transform.localPosition = new Vector3(Random.Range(-1,-4), 0f,Random.Range(-1,-4));
                firstRun = hitWalls = false;
            }
            else
            {
                transform.localPosition = new Vector3(transform.localPosition[0],transform.localPosition[1],transform.localPosition[2]);
            }
        }
        if(rand == 1)
        {
            target.localPosition = new Vector3(Random.Range(-1,-4), 0f,Random.Range(-1,-4));
            if(firstRun || hitWalls)
            {
                transform.localPosition = new Vector3(Random.Range(1,4), 0f,Random.Range(1,4));
                firstRun = hitWalls = false;
            }
            else
            {
                transform.localPosition = new Vector3(transform.localPosition[0],transform.localPosition[1],transform.localPosition[2]);
            }
        }
    }

            
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(target.localPosition);
    }
    public override void OnActionReceived(ActionBuffers action)
    {
        float moveX = action.ContinuousActions[0];
        float moveY = action.ContinuousActions[1];

        Vector3 velocity = new Vector3(moveX, 0f, moveY);
        velocity = velocity.normalized * Time.deltaTime * moveSpeed;
        transform.localPosition += velocity;
        // transform.localPosition += new Vector3(moveX, 0f, moveY) * Time.deltaTime * moveSpeed;
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
            AddReward(-1f);
            hitWalls = true;
            EndEpisode();
        }
    }
}
