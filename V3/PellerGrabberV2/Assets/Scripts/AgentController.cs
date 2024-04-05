using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class AgentMove : Agent
{
    [SerializeField] private Transform target;
    [SerializeField] private Transform target2;
    [SerializeField] private Transform target3;
    private bool target1Catch = false;
    private bool target2Catch = false;
    private bool target3Catch = false;
    private int targetHitOrder = 0;
    private bool hitInFullOrder = true;
    [SerializeField] private float moveSpeed = 1f;
    public bool firstRun = true;
    public bool hitWalls = false;
    private Rigidbody rb;
    private Vector3[] positions = new Vector3[3];
    //public Text text;
    public ScoreManager scoreManager;
    //public ScoreManager scoreManager = new ScoreManager;



    public override void Initialize()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Shuffle(int[] array)
    {
        System.Random random = new System.Random();
        for (int i = array.Length - 1; i > 0; i--)
        {
            int j = random.Next(i + 1);
            int temp = array[i];
            array[i] = array[j];
            array[j] = temp;
        }
    }
    
    public override void OnEpisodeBegin()
    {
        targetHitOrder = 0;
        target.gameObject.SetActive(true);
        target2.gameObject.SetActive(true);
        target3.gameObject.SetActive(true);
        target1Catch = false;
        target2Catch = false;
        target3Catch = false;
        hitInFullOrder = true;

        if(firstRun || hitWalls)
        {
            transform.localPosition = new Vector3(Random.Range(-4f,4f), 0f,Random.Range(-4f,4f));
            firstRun = hitWalls = false;
        }

        
        if(transform.localPosition[0] < 0 && transform.localPosition[2] > 0)
        {
            positions[0] = new Vector3(Random.Range(-2f,-4f), 0f,Random.Range(-2f,-4f));
            positions[1] = new Vector3(Random.Range(2f,4f), 0f,Random.Range(-2f,-4f));
            positions[2] = new Vector3(Random.Range(2f,4f), 0f,Random.Range(2f,4f));
        }
        else if(transform.localPosition[0] > 0 && transform.localPosition[2] > 0)
        {
            positions[0] = new Vector3(Random.Range(-2f,-4f), 0f,Random.Range(2f,4f));
            positions[1] = new Vector3(Random.Range(-2f,-4f), 0f,Random.Range(-2f,-4f));
            positions[2] = new Vector3(Random.Range(2f,4f), 0f,Random.Range(-2f,-4f));  
        }
        else if(transform.localPosition[0] < 0 && transform.localPosition[2] < 0)
        {
            positions[0] = new Vector3(Random.Range(-2f,-4f), 0f,Random.Range(2f,4f));
            positions[1] = new Vector3(Random.Range(2f,4f), 0f,Random.Range(2f,4f));
            positions[2] = new Vector3(Random.Range(2f,4f), 0f,Random.Range(-2f,-4f));  
        }
        else if(transform.localPosition[0] > 0 && transform.localPosition[2] < 0)
        {
            positions[0] = new Vector3(Random.Range(-2f,-4f), 0f,Random.Range(-2f,-4f));
            positions[1] = new Vector3(Random.Range(-2f,-4f), 0f,Random.Range(2f,4f));
            positions[2] = new Vector3(Random.Range(2f,4f), 0f,Random.Range(2f,4f));  
        }

        int[] numbers = { 0, 1, 2 };
        Shuffle(numbers);

        target.localPosition = positions[numbers[0]];
        target2.localPosition = positions[numbers[1]];
        target3.localPosition = positions[numbers[2]];
    }
            
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);
    }
    public override void OnActionReceived(ActionBuffers action)
    {
        float moveRotate = action.ContinuousActions[0];
        float moveForward = action.ContinuousActions[1];

        rb.MovePosition(transform.position + transform.forward * moveForward * moveSpeed * Time.deltaTime);
        transform.Rotate(0f, moveRotate * moveSpeed, 0f, Space.Self);
    }

    public override void Heuristic(in ActionBuffers actionOut)
    {
        ActionSegment<float> continuousActions = actionOut.ContinuousActions;
        continuousActions[0] = Input.GetAxisRaw("Horizontal");
        continuousActions[1] = Input.GetAxisRaw("Vertical");
    }

    private void OnTriggerEnter(Collider other)
    {
        float reward = 0f;
        if(other.gameObject.tag != "Walls")
        {
            if(other.gameObject.tag == "Target")
            {
                target1Catch = true;
                target.gameObject.SetActive(false);
                if(targetHitOrder == 0)
                {
                    reward =30f;
                }
                else if(targetHitOrder == 1)
                {
                    reward =1f;
                    hitInFullOrder = false;
                }
                else
                {
                    reward =1f;
                    hitInFullOrder = false;
                }
            }
            if(other.gameObject.tag == "Target2")
            {
                target2Catch = true;
                target2.gameObject.SetActive(false);
                if(targetHitOrder == 1)
                {
                    reward =20f;
                }
                else if(targetHitOrder == 0)
                {
                    reward =1f;
                    hitInFullOrder = false;
                }
                else
                {
                    reward =1f;
                    hitInFullOrder = false;
                }
            }
            
            if(other.gameObject.tag == "Target3")
            {
                target3Catch = true;
                target3.gameObject.SetActive(false);
                if(targetHitOrder == 2)
                {
                    reward =10f;
                }
                else if(targetHitOrder == 1)
                {
                    reward =1f;
                    hitInFullOrder = false;
                }
                else
                {
                    reward =1f;
                    hitInFullOrder = false;
                }
            }
            targetHitOrder++;
        }

        if(other.gameObject.tag == "Walls")
        {
            reward = -100f;
            hitWalls = true;
            EndEpisode();
        }

        if(target1Catch && target2Catch && target3Catch)
        {
            if(hitInFullOrder)
            {
                reward = 100f;
                AudioSource audioSource = GetComponent<AudioSource>(); // Assurez-vous que votre cible a un composant AudioSource
                if(audioSource != null /*&& !audioSource.isPlaying*/)
                {
                    audioSource.Play(); // Joue le son
                }
            }
            UIManager.Instance.ShowFloatingText(((int)reward).ToString(), other.transform.position);
            scoreManager.AddScore(reward);
            AddReward(reward);
            EndEpisode();
        }
        else
        {
            UIManager.Instance.ShowFloatingText(((int)reward).ToString(), other.transform.position);
            scoreManager.AddScore(reward);
            AddReward(reward);
        }
    }
}
