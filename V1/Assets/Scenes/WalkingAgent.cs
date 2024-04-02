using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class WalkingAgent : Agent
{
    Rigidbody rBody;
    Vector3 startPosition;
    float timer;
    public float episodeDuration = 10f; // Dur�e de l'�pisode en secondes
    public float someThreshold = 1.0f; // Seuil pour la distance � l'objectif
    float previousDistance = float.MaxValue; // Distance pr�c�dente au cylindre
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
        // R�initialise la position et la v�locit� du cube
        transform.localPosition = startPosition;
        rBody.velocity = Vector3.zero;
        rBody.angularVelocity = Vector3.zero;
        timer = 0f;
        // R�initialise la distance pr�c�dente � une valeur �lev�e
        previousDistance = float.MaxValue;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        Debug.Log("CollectObservations");
        // Observations : position actuelle, vitesse, temps restant
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(rBody.velocity);
        sensor.AddObservation(episodeDuration - timer);
        // Ajoutez ici d'autres observations si n�cessaire
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        Debug.Log("Action Received");
        // Mouvements bas�s sur les actions de l'agent
        float moveX = actions.ContinuousActions[0];
        float moveZ = actions.ContinuousActions[1];

        rBody.AddForce(new Vector3(moveX, 0, moveZ) * 100);

        // Mise � jour du timer
        timer += Time.fixedDeltaTime;

        // Calcul de la r�compense bas�e sur la proximit� du cylindre
        float distanceToCylinder = Vector3.Distance(cylinderTransform.position, transform.position);

        if (distanceToCylinder < previousDistance)
        {
            AddReward(0.1f); // R�compensez l'approche du cylindre
        }

        previousDistance = distanceToCylinder;

        // Fin de l'�pisode si l'agent atteint le cylindre
        if (distanceToCylinder < someThreshold)
        {
            AddReward(1.0f); // R�compense importante pour avoir atteint le cylindre
            EndEpisode();
        }

        // Fin de l'�pisode apr�s la dur�e sp�cifi�e
        if (timer >= episodeDuration)
        {
            // Vous pouvez choisir de r�compenser l'agent ici si n�cessaire
            // SetReward(...);

            EndEpisode();
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        Debug.Log("Heuristic");
        // Tester l'agent manuellement avec les entr�es du clavier
        var continuousActionsOut = actionsOut.ContinuousActions;
        continuousActionsOut[0] = Input.GetAxis("Horizontal");
        continuousActionsOut[1] = Input.GetAxis("Vertical");
    }
}
