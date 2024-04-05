using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class AgentTest : Agent
{
    public Transform cuisseRight;
    public HingeJoint jointCuisse; // Ajoutez ceci et assignez-le dans l'inspecteur Unity
    private float maxVelocity = 2000f; // Ajustez selon le besoin
    private float maxForce = 100f; // Ajustez selon le besoin
    [SerializeField] private Transform target;

    public Transform fullBody;
    private Vector3 posOnStart;


    public override void Initialize()
    {
        var motor = jointCuisse.motor;
        motor.targetVelocity = 2000f;
    }
    public override void OnEpisodeBegin()
    {
        target.localPosition = new Vector3(Random.Range(-40f,40f), 23f,Random.Range(-40f,40f));
        posOnStart = fullBody.localPosition;
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        float targetVelocity = actionBuffers.ContinuousActions[0] * maxVelocity;
        var motor = jointCuisse.motor;
        motor.targetVelocity = targetVelocity;
        motor.force = maxForce;
        jointCuisse.motor = motor;
        jointCuisse.useMotor = true;


        float distance = Vector3.Distance(posOnStart, fullBody.position);
        AddReward(distance);
    }
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        
        var continuousActionsOut = actionsOut.ContinuousActions;
        if (Input.GetKey(KeyCode.UpArrow))
        {
            // Supposons que la première action continue contrôle la cuisse
            continuousActionsOut[0] = 1.0f; // Action pour lever la cuisse
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            continuousActionsOut[0] = -1.0f; // Action pour abaisser la cuisse
        }
        else
        {
            continuousActionsOut[0] = 0.0f; // Aucune action
        }
        Debug.Log($"Action: {continuousActionsOut[0]}");
    }
    public override void CollectObservations(VectorSensor sensor)
    {
        // Exemple: Ajouter l'angle actuel du joint comme observation
        sensor.AddObservation(jointCuisse.angle);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Target")
        {
            AddReward(10f);
            EndEpisode();
        }
    }

}
