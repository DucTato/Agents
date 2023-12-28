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
    private Vector3 intendedTarget;
    [SerializeField]
    private HashSet<GameObject> stackCubeSet;
    private GameObject previousCube;
    // Start is called before the first frame update
    void Start()
    {
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
            
            Debug.Log(prioritizedCubeSet.Count +" Priority");
            Debug.Log(stackCubeSet.Count +" Stack");
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
                if (robotControl.IsHandEmpty())
                {
                    // if the hand is empty and the current state is Priority, grab the cube
                    robotControl.PickAtTarget(intendedTarget);
                }
                else
                {
                    // the hand is holding a cube, so it need to complete its current task
                    previousCube = robotControl.GetCurrentCube();
                    intendedTarget = sysControl.DropTargetLocator(gameObject, SystemController.LocatorType.EndMattress);
                    robotControl.DropAtTarget(intendedTarget);
                }
                break;
            case PerceptionStates.StackTower:
                break;
            default: 
                if (NearestCubeLocator(prioritizedCubeSet) == null)
                {
                    if (NearestCubeLocator(stackCubeSet) == null)
                    {
                        // No cube to stack and no priority cube
                        decision = PerceptionStates.Idle;
                    }
                    else
                    {
                        //No possible priority cube, switch state to Stacking 
                        decision = PerceptionStates.StackTower;
                        intendedTarget = NearestCubeLocator(stackCubeSet).transform.position;
                    } 
                }
                else
                {
                    Debug.Log("Red Cube in range");
                    // Possible priority cube within range, do priority task
                    decision = PerceptionStates.DoPriority;
                    intendedTarget = NearestCubeLocator(prioritizedCubeSet).transform.position;
                }
                break;
        }
    }
    private GameObject NearestCubeLocator(HashSet<GameObject> cubeSet)
    {
        // Maximum search distance is 5 unit(s) from the Claw Tip
        float distance = 10f;
        GameObject result = null;
        foreach (GameObject cube in cubeSet)
        {
            //if (Vector3.Distance(cube.transform.position, sysControl.DropTargetLocator(gameObject, SystemController.LocatorType.EndMattress)) < Vector3.Distance(transform.position, sysControl.DropTargetLocator(gameObject, SystemController.LocatorType.EndMattress)))
            //{
            //    continue;
            //}
            if (cube == previousCube)
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
    
}
public enum PerceptionStates
{
    Idle,
    DoPriority,
    PassPriority,
    StackTower,
    StackPyramid
}
