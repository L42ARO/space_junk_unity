using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public struct LastShot{
    public float time;
    public Vector3 position;
    public Vector3 direction;
}
public struct StateTimer
{
    public float lastStart;
    public float lastEnd;
    public bool isRunning;

    public void StartTimer(float time)
    {
        lastStart = time;
        isRunning = true;
    }
    public void EndTimer(float time)
    {
        lastEnd = time;
        isRunning = false;
    }
    public float GetTime()
    {
        return lastEnd - lastStart;
    }
}

[System.Serializable]
public class HandProps
{
    public GameObject gameObject;
    public GameObject innerGlow;
    public GameObject dirRef;
    [HideInInspector] public bool isOpen;
    [HideInInspector] public bool isClosed;
    [HideInInspector] public StateTimer closedTimer;
    [HideInInspector] public StateTimer openedTimer;
    public List<GameObject> debugOutputs;
    public bool isDebugging;

    public void ActivateOutput(int index)
    {
        if(!isDebugging){
            return;
        }
        foreach (GameObject output in debugOutputs)
        {
            output.SetActive(false);
        }
        debugOutputs[index].SetActive(true);
    }
}



public class HandShooterManager : MonoBehaviour
{
    public float max_delta_time = 1.0f;
    public HandProps leftHand;
    public HandProps rightHand;
    public GameObject ray_template;
    public GameObject hand_test;
    [System.Serializable]
    public struct CrossStreamProps
    {
        public float distanceThreshold;
        public float angleThreshold;
        public float shootAngleThreshold;
        public GameObject energyBall;
        public GameObject energyBeam;
        public StateTimer crossStreamTimer;
        public StateTimer beamTimer;
    }
    public CrossStreamProps crossStreamProps;

    private LastShot lastShot;
    void Start(){

        // ShootRay(hand_test);
    }

    void Update(){
        HandProps[] hands = {leftHand, rightHand};
        bool isCrossing = CheckForStreamCross(leftHand.gameObject, rightHand.gameObject, crossStreamProps.distanceThreshold, crossStreamProps.angleThreshold);
        if(isCrossing && !leftHand.isClosed && !rightHand.isClosed){
            if(!crossStreamProps.crossStreamTimer.isRunning){
                crossStreamProps.crossStreamTimer.StartTimer(Time.time);
                //activate the energy ball
                crossStreamProps.energyBall.SetActive(true);
                crossStreamProps.beamTimer.EndTimer(Time.time);
                //deactivate all other timers
                for (int i = 0; i < 2; i++){
                    HandProps hand = hands[i];
                    if(hand.closedTimer.isRunning){
                        hand.closedTimer.EndTimer(Time.time);
                    }
                    if(hand.openedTimer.isRunning){
                        hand.openedTimer.EndTimer(Time.time);
                    }
                }
            }
            //move the energy ball to the middle of the hands
            crossStreamProps.energyBall.transform.position = (leftHand.gameObject.transform.position + rightHand.gameObject.transform.position) / 2;
            return;


        }else{
            if(crossStreamProps.crossStreamTimer.isRunning){
                crossStreamProps.crossStreamTimer.EndTimer(Time.time);
                crossStreamProps.energyBall.SetActive(false);
            }
            if (!crossStreamProps.beamTimer.isRunning){
                if(Time.time - crossStreamProps.crossStreamTimer.lastEnd < 1){
                    //deactivate the energy ball
                    //if the hands are not closed, then the player is trying to shoot
                    if(!leftHand.isClosed && !rightHand.isClosed){
                        //get the direction of the hands
                        bool isSameDir = CheckForSameHandDir(leftHand.gameObject, rightHand.gameObject, crossStreamProps.shootAngleThreshold);
                        if(isSameDir){
                            //if the hands are pointing in the same direction
                            //shoot energy beam in that direction
                            //multiple times for now
                          ShootEnergyBeam(leftHand.dirRef, rightHand.dirRef, leftHand.gameObject.transform.position, rightHand.gameObject.transform.position);
                            crossStreamProps.beamTimer.StartTimer(Time.time);
                            return;
                        }
                    }
                }
            }
            else
            {
                crossStreamProps.beamTimer.EndTimer(Time.time);
            }
        }
        
        for (int i = 0; i < 2; i++){
            //first analyze left hand
            HandProps hand = hands[i];
            if(hand.isClosed && !hand.isOpen){
                if(!hand.closedTimer.isRunning){
                    hand.ActivateOutput(1);
                    hand.innerGlow.SetActive(true);
                    hand.closedTimer.StartTimer(Time.time);
                }
                if(hand.openedTimer.isRunning){
                    hand.openedTimer.EndTimer(Time.time);
                }
            }
            else if (hand.isOpen && !hand.isClosed){
                
                if(hand.closedTimer.isRunning){
                    hand.innerGlow.SetActive(false);
                    hand.closedTimer.EndTimer(Time.time);
                }
                if(!hand.openedTimer.isRunning){
                    //Check how long ago the hand was closed
                    float timeSinceClosed = Time.time - hand.closedTimer.lastEnd;
                    if(timeSinceClosed < max_delta_time){
                        hand.ActivateOutput(2);
                        hand.openedTimer.StartTimer(Time.time);
                        ShootRay(hand.dirRef);
                    }
                    else{
                        hand.ActivateOutput(3);
                    }
                }
            }
            else {
                hand.ActivateOutput(0);
                hand.innerGlow.SetActive(false);
                if(hand.closedTimer.isRunning){
                    hand.closedTimer.EndTimer(Time.time);
                }
                if(hand.openedTimer.isRunning){
                    hand.openedTimer.EndTimer(Time.time);
                }
            }
        }
        
    }

    public bool CheckForSameHandDir(GameObject LHand, GameObject RHand, float angleThreshold)
    {
        Vector3 V_LDir = LHand.transform.up;
        Vector3 V_RDir = RHand.transform.up;
        float angle_LDir2RDir = Vector3.Angle(V_LDir, V_RDir);
        if (angle_LDir2RDir < angleThreshold)
        {
            return true;
        }
        return false;
    }
    public void ShootEnergyBeam(GameObject L_Dir, GameObject R_Dir, Vector3 rPos, Vector3 lPos)
    {
        Vector3 V_LDir = L_Dir.transform.forward;
        Vector3 V_RDir = R_Dir.transform.forward;
        Vector3 V_BeamDir = (V_LDir + V_RDir) / 2;
        Quaternion rotation = Quaternion.LookRotation(V_BeamDir * -1);
        Vector3 position = (lPos + rPos) / 2;
        GameObject beam = Instantiate(crossStreamProps.energyBeam, position, rotation);
        beam.SetActive(true);
        Rigidbody rb = beam.GetComponent<Rigidbody>();
        rb.velocity = V_BeamDir * 10f;
    }

    public bool CheckForStreamCross(GameObject LHand, GameObject RHand, float distanceThreshold, float angleThreshold)
    {
        //if the hands are close enough to each other
        //And their y axis is facing each other
        Vector3 V_L2R = RHand.transform.position - LHand.transform.position;
        if (V_L2R.magnitude > distanceThreshold)
        {
            return false;
        }
        Vector3 V_R2L = LHand.transform.position - RHand.transform.position;

        Vector3 V_LDir = LHand.transform.up;
        Vector3 V_RDir = RHand.transform.up;

        float angle_LDir2L2R = Vector3.Angle(V_LDir, V_L2R);
        float angle_RDir2R2L = Vector3.Angle(V_RDir, V_R2L);

        //both angles should be less than 45 degrees
        if (angle_LDir2L2R < angleThreshold && angle_RDir2R2L < angleThreshold)
        {
            return true;
        }
        return false;
    }

    public void ShootRay(GameObject hand)
    {
        Vector3 rayOrigin = hand.transform.position;
        Vector3 direction = hand.transform.forward;
        float currentTime = Time.time;

        // Calculate the dot product to find the angle between the new and last shot direction
        float dotProduct = Vector3.Dot(direction.normalized, lastShot.direction.normalized);
        float angleThreshold = Mathf.Cos(Mathf.Deg2Rad * 30f); // Cosine of 60 degrees

        // Check if the new position is close, less than 3 seconds have passed, and the direction is similar
        if (Vector3.Distance(rayOrigin, lastShot.position) < 0.1f && dotProduct > angleThreshold &&
            (currentTime - lastShot.time) < 1.0f)
        {
            // Use the last shot's direction for consistency
            direction = lastShot.direction;
        }

        // Instantiate the ray
        Quaternion rotation = Quaternion.LookRotation(direction * -1);
        GameObject ray = Instantiate(ray_template, rayOrigin, rotation);
        ray.SetActive(true);

        // Get the Rigidbody of the ray and set the velocity
        Rigidbody rb = ray.GetComponent<Rigidbody>();
        rb.velocity = direction * 50f; // Adjust the speed as needed

        // Update the last shot information
        lastShot.time = currentTime;
        lastShot.position = rayOrigin;
        lastShot.direction = direction;
    }

   

    public void OnLeftHandOpen()
    {
        leftHand.isOpen = true;
    }
    public void OnLeftHandClose()
    {
        leftHand.isClosed = true;
    }
    public void OnLefthandNotOpen()
    {
        leftHand.isOpen = false;
    }
    public void OnLeftHandNotClose()
    {
        leftHand.isClosed = false;
    }
    //now the same for the right hand
    public void OnRightHandOpen()
    {
        rightHand.isOpen = true;
    }
    public void OnRightHandClose()
    {
        rightHand.isClosed = true;
    }
    public void OnRightHandNotOpen()
    {
        rightHand.isOpen = false;
    }
    public void OnRightHandNotClose()
    {
        rightHand.isClosed = false;
    }


}
