using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RobotAgent : MonoBehaviour
{
    private SystemController sysControl;
    private RobotArmController robotControl;
    [SerializeField]
    private float pollingRate;
    private float pollingCount;
    [SerializeField]
    private PerceptionStates decision;
    [SerializeField]
    private HashSet<GameObject> prioritizedCubeSet;
    [SerializeField]
    private Vector3 intendedTarget, dropTarget;
    [SerializeField]
    private HashSet<GameObject> stackCubeSet;
    [SerializeField]
    private GameObject previousCube;
    [SerializeField]
    private int towerStep, towerAttempt;
    // Start is called before the first frame update
    void Start()
    {
        pollingCount = Random.Range(0, 5f);
        sysControl = SystemController.instance;
        // Register to the System upon being activated
        sysControl.RegisterRobotArm(gameObject);
        robotControl = GetComponent<RobotArmController>();
        robotControl.OnDropCompletion += RobotControl_OnDropCompletion;
        robotControl.OnReset += RobotControl_OnReset;
    }
    private void OnDisable()
    {
        robotControl.OnDropCompletion -= RobotControl_OnDropCompletion;
        robotControl.OnReset -= RobotControl_OnReset;
    }
    private void RobotControl_OnDropCompletion(object sender, System.EventArgs e)
    {
        if (decision == PerceptionStates.DoStacking)
        {
            StartCoroutine(ResetArmAfterSeconds(0.5f));
            towerAttempt++;
        }
        decision = PerceptionStates.Idle;
    }
    private void RobotControl_OnReset(object sender, System.EventArgs e)
    {
        decision = PerceptionStates.Idle;
    }

    // Update is called once per frame
    void Update()
    {
        if(pollingCount >= 0)
        {
            pollingCount -= Time.deltaTime;
        }
        else
        {
            // Execute every x seconds (polling rate)
            See();
            Think();
            pollingCount = pollingRate;
            
            //Debug.Log(prioritizedCubeSet.Count +" Priority");
            //Debug.Log(stackCubeSet.Count +" Stack");
        }
        
    }
    private void See()
    {
        prioritizedCubeSet = GameObject.FindGameObjectsWithTag("Priority").ToHashSet();
        stackCubeSet = GameObject.FindGameObjectsWithTag("Grabbable").ToHashSet();
    }
    private void Think()
    {
        if(robotControl.IsArmBusy())
        {
            return;
        }
        //The default state is always the Idle state
        switch (decision)
        {
            case PerceptionStates.DoPriority:
                if (Vector3.Distance(transform.position, sysControl.FindClosestPositionWithType(gameObject, SystemController.LocatorType.EndMattress)) <5f)
                {
                    // Check if there's an End point within range and set the drop target
                    dropTarget = sysControl.FindClosestPositionWithType(gameObject, SystemController.LocatorType.EndMattress);
                    // Dropping at End points need an elevation
                    dropTarget.y += 1f;
                }
                else
                {
                    if (Vector3.Distance(transform.position, sysControl.FindClosestPositionWithType(gameObject, SystemController.LocatorType.Robot)) <10f)
                    {
                        //Check if there's another robot arm close by and set the drop target
                        Vector3 robotPos = sysControl.FindClosestPositionWithType(gameObject, SystemController.LocatorType.Robot);
                        dropTarget.x = transform.position.x + (robotPos.x - transform.position.x) / 2f; 
                        dropTarget.y = robotPos.y + 1f;
                        dropTarget.z = transform.position.z + (robotPos.z - transform.position.z) / 2f;
                    }
                    else
                    {
                        // Switch decision to the another state because there's no close by valid point
                        decision = PerceptionStates.DoStacking;
                        break;
                    }
                }
                if (robotControl.IsHandEmpty())
                {
                    // If the hand is currently empty and the state is DoPriority, pick up a red cube
                    robotControl.PickAtTarget(intendedTarget);
                }
                else
                {
                    // The hand is currently holding a cube and it needs to complete its task
                    previousCube = robotControl.GetCurrentCube();
                    if (robotControl.GetCurrentTag() == "Grabbable")
                    {
                        decision = PerceptionStates.DoStacking;
                        return;
                    }
                    
                    robotControl.DropAtTarget(dropTarget);
                }
                break;
            case PerceptionStates.DoStacking:
                // Do stacking algorithm here :l
               
                switch (towerStep)
                {
                case 1:
                    robotControl.PickAtTarget(intendedTarget);
                    if(!robotControl.IsHandEmpty())
                    {
                        towerStep = 2;
                        return;
                    }
                    break;
                case 2:
                    robotControl.ResetArm();
                    previousCube = robotControl.GetCurrentCube();
                    towerStep = 3;
                    return;
                case 3:
                    robotControl.DropAtTarget(dropTarget);
                    break;
                default:
                    dropTarget = sysControl.FindClosestObjectWithType(gameObject, SystemController.LocatorType.StackMattress).GetComponent<StackMattressScript>().GetButtonLocation();
                    dropTarget.y += .1f;
                    robotControl.PickAtTarget(dropTarget);
                    break;
                }
            break;
            default:
                if (NearestCubeLocator(prioritizedCubeSet) == null || robotControl.GetCurrentTag() == "Grabbable")
                {
                    if (Vector3.Distance(transform.position, sysControl.FindClosestPositionWithType(gameObject, SystemController.LocatorType.StackMattress)) >5f)
                    {
                        // No cube stacking area and no priority cube near by, switch state to Idle
                        decision = PerceptionStates.Idle;
                    }
                    else
                    {
                        //No possible priority cube, switch state to Stacking
                        if (robotControl.IsHandEmpty())
                        {
                            //if (previousCube != null)
                            //{
                            //    if (previousCube.CompareTag("StackedCube"))
                            //    {
                            //        previousStackedCube = previousCube;
                            //    }
                            //}
                            if (NearestCubeLocator(stackCubeSet, 11f) == null)
                            {
                                // Check if there's any stacking cube nearby, if not, then switch state to Idle
                                Debug.Log("No cube to stack");
                                decision = PerceptionStates.Idle;
                                break;
                            }
                            else
                            {
                                intendedTarget = NearestCubeLocator(stackCubeSet, 11f).transform.position;
                                //Debug.Log("Set nearest cube: " + NearestCubeLocator(stackCubeSet).name);
                                towerStep = 1;
                            }
                        }
                        // Check the current tower height
                        switch (sysControl.FindClosestObjectWithType(gameObject, SystemController.LocatorType.StackMattress).GetComponent<StackMattressScript>().GetCurrentTowerStack())
                        {
                            case 0:
                                // The first cube of the stack
                                dropTarget = sysControl.FindClosestObjectWithType(gameObject, SystemController.LocatorType.StackMattress).GetComponent<StackMattressScript>().GetStackLocation();
                                dropTarget.y += 1f;
                                break;
                            case 4:
                                Debug.Log("Tower Completed");
                                towerStep = 0;
                                break;
                            default:
                                // While the tower is not yet finished
                                // Not the first cube of the stack, the drop location will be the previous one's xyz with +height
                                dropTarget = sysControl.FindClosestObjectWithType(gameObject, SystemController.LocatorType.StackMattress).GetComponent<StackMattressScript>().GetStackTop();
                                dropTarget.y += 1f;
                                break;
                        }
                        if(towerAttempt > 5)
                        {
                            towerStep = 0;
                        }
                        decision = PerceptionStates.DoStacking;
                    } 
                }
                else
                {
                    //Debug.Log("Red Cube in range");
                    // Possible priority cube within range, do priority task
                    decision = PerceptionStates.DoPriority;
                    intendedTarget = NearestCubeLocator(prioritizedCubeSet).transform.position;
                }
                break;
        }
    }
    private GameObject NearestCubeLocator(HashSet<GameObject> cubeSet)
    {
        if (cubeSet.Count == 0) return null;
        // Maximum search distance is 5 unit(s) from the Claw Tip
        float distance = 10f;
        GameObject result = null;
        foreach (GameObject cube in cubeSet)
        {
            if (cube == previousCube || cube.GetComponent<Rigidbody>().velocity != Vector3.zero)
            {
                continue;
            }
            if(Vector3.Distance(robotControl.GetClawTipPosition(), cube.transform.position) <= distance)
            {
                result = cube;
                distance = Vector3.Distance(robotControl.GetClawTipPosition(), cube.transform.position);
            }
        }
        return result;
    }
    private GameObject NearestCubeLocator(HashSet<GameObject> cubeSet, float distance)
    {
        if (cubeSet.Count == 0) return null;
        // Maximum search distance is 5 unit(s) from the Claw Tip
        GameObject result = null;
        foreach (GameObject cube in cubeSet)
        {
            if (cube == previousCube || cube.GetComponent<Rigidbody>().velocity != Vector3.zero)
            {
                continue;
            }
            if (Vector3.Distance(robotControl.GetClawTipPosition(), cube.transform.position) <= distance)
            {
                result = cube;
                distance = Vector3.Distance(robotControl.GetClawTipPosition(), cube.transform.position);
            }
        }
        return result;
    }
    private IEnumerator ResetArmAfterSeconds(float timer)
    {
        yield return new WaitForSeconds(timer);
        robotControl.ResetArm();
    }
    public void ResetTowerStep()
    {
        towerStep = 1;
        towerAttempt = 0;
    }
}
public enum PerceptionStates
{
    Idle,
    DoPriority,
    PassPriority,
    DoStacking,
    StackPyramid
}
