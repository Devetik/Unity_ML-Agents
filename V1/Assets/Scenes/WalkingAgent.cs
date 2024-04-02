using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class WalkingAgent : Agent
{
    Rigidbody rBody;
    Vector3 startPosition;
    float timer;
    public float episodeDuration = 10f; // Durée de l'épisode en secondes
    public float someThreshold = 1.0f; // Seuil pour la distance à l'objectif
    float previousDistance = float.MaxValue; // Distance précédente au cylindre
    public Transform cylinderTransform; // Assignez cette variable dans l'inspecteur Unity

    public override void Initialize()
    {
        Debug.Log("Initialized");
        rBody = GetComponent<Rigidbody>();
        startPosition = transform.localPosition;
    }

    public override void OnEpisodeBegin()
    {
        Debug.Log("OnEpisodeBegin");
        // Réinitialise la position et la vélocité du cube
        transform.localPosition = startPosition;
        rBody.velocity = Vector3.zero;
        rBody.angularVelocity = Vector3.zero;
        timer = 0f;
        // Réinitialise la distance précédente à une valeur élevée
        previousDistance = float.MaxValue;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        Debug.Log("CollectObservations");
        // Observations : position actuelle, vitesse, temps restant
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(rBody.velocity);
        sensor.AddObservation(episodeDuration - timer);
        // Ajoutez ici d'autres observations si nécessaire
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        Debug.Log("Action Received");
        // Mouvements basés sur les actions de l'agent
        float moveX = actions.ContinuousActions[0];
        float moveZ = actions.ContinuousActions[1];

        rBody.AddForce(new Vector3(moveX, 0, moveZ) * 100);

        // Mise à jour du timer
        timer += Time.fixedDeltaTime;

        // Calcul de la récompense basée sur la proximité du cylindre
        float distanceToCylinder = Vector3.Distance(cylinderTransform.position, transform.position);

        if (distanceToCylinder < previousDistance)
        {
            AddReward(0.1f); // Récompensez l'approche du cylindre
        }

        previousDistance = distanceToCylinder;

        // Fin de l'épisode si l'agent atteint le cylindre
        if (distanceToCylinder < someThreshold)
        {
            AddReward(1.0f); // Récompense importante pour avoir atteint le cylindre
            EndEpisode();
        }

        // Fin de l'épisode après la durée spécifiée
        if (timer >= episodeDuration)
        {
            // Vous pouvez choisir de récompenser l'agent ici si nécessaire
            // SetReward(...);

            EndEpisode();
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        Debug.Log("Heuristic");
        // Tester l'agent manuellement avec les entrées du clavier
        var continuousActionsOut = actionsOut.ContinuousActions;
        continuousActionsOut[0] = Input.GetAxis("Horizontal");
        continuousActionsOut[1] = Input.GetAxis("Vertical");
    }
}
