using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;

public class AgentLogic : Agent
{

    Rigidbody rBody;
    float startTime;

    // Start is called before the first frame update
    void Start()
    {
        rBody = GetComponent<Rigidbody>();
    }

    public Transform goal;
    public override void OnEpisodeBegin()
    {

        startTime = Time.time;
        //reset agent
        this.rBody.angularVelocity = Vector3.zero;
        this.rBody.velocity = Vector3.zero;
        this.transform.localPosition = new Vector3(-2f, 1f, -12.5f);

        //Move target position each time
        goal.transform.localPosition = new Vector3(Random.value * 18 - 9, .49f, 4.389f);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(goal.localPosition);
        sensor.AddObservation(this.transform.localPosition);
        sensor.AddObservation(rBody.velocity);

    }

    public override void OnActionReceived(float[] vectorActions)
    {
        float speed = 2f;
        Vector3 controller = Vector3.zero;

        var forwardAxis = vectorActions[0];
        var rightAxis = vectorActions[1];

        switch (forwardAxis)
        {
            case 1:
                controller = transform.forward * speed;
                break;
            case 2:
                controller = transform.forward * -speed;
                break;
        }

        switch (rightAxis)
        {
            case 1:
                controller = transform.right * speed;
                break;
            case 2:
                controller = transform.right * -speed;
                break;
        }

        //rBody.position = rBody.position + controller*speed*.5f;

        rBody.AddForce(controller * speed, ForceMode.VelocityChange);

        float distanceToTarget = Vector3.Distance(this.transform.localPosition, goal.localPosition);
        Debug.Log("Vector: " + controller*speed);

        //agentRb.AddForce(dirToGo * m_SoccerSettings.agentRunSpeed,ForceMode.VelocityChange);

        if(distanceToTarget < 1.0f)
        {
            SetReward(10.0f);
            EndEpisode();
        }
        else
        {
                AddReward(-.75f * distanceToTarget);
                Debug.Log("Time in Episode: " + (Time.time - startTime)); 
        }

        if(Time.time - startTime > 60)
                {
                    SetReward(-5f);
                    EndEpisode();
                }
    }
    
    void OnCollisionEnter(Collision c)
    {
        if (c.gameObject.CompareTag("wall"))
        {
            AddReward(-.5f);
        }
    }

    public override void Heuristic(float[] actionsOut)
    {
        actionsOut[0] = Input.GetAxis("Vertical");
        actionsOut[1] = Input.GetAxis("Horizontal");
    }
}
