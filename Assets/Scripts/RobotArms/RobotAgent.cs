using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RobotAgent : MonoBehaviour, IPointerClickHandler
{
    private SystemController sysControl;
    private RobotArmController robotControl;
    [SerializeField]
    private float pollingRate;
    private float pollingCount, currentMagnitude = 1;
    [SerializeField]
    private PerceptionStates decision;
    [SerializeField]
    private HashSet<GameObject> prioritizedCubeSet;
    [SerializeField]
    private Vector3 intendedTarget, dropTarget;
    [SerializeField]
    private HashSet<GameObject> stackCubeSet;
    private GameObject previousCube;
    [SerializeField]
    private GameObject canvasElements;
    [SerializeField]
    private Button rotateButton;
    [SerializeField]
    private Text magnitudeText;
    [SerializeField]
    private int towerStep, towerAttempt;
    private Outline highlight;
    private bool reverseRotation;
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
        highlight = GetComponent<Outline>();
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
                        decision = PerceptionStates.Idle;
                        robotControl.ResetArm();
                        return;
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
                    return;
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
                                //Debug.Log("Tower Completed");
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
                //Debug.Log("Done thinking");
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
    // This part is for the in-world-space UI elements
    public bool GetSelectState()
    {
        return highlight.enabled;
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        // This method here is to intercept the click event 
        //Debug.Log("Clicked on a robot arm");
        if (highlight.enabled)
        {
            if (eventData.button == PointerEventData.InputButton.Right)
            {
                // Right click toggle reverse rotation
                // switch to reverse state of the 'Rotate' button
                if (reverseRotation)
                {
                    reverseRotation = false;
                    rotateButton.transform.localScale = new Vector3(0.52f, 0.52f, 0.52f);
                }
                else
                {
                    reverseRotation = true;
                    rotateButton.transform.localScale = new Vector3(-0.52f, 0.52f, 0.52f);
                }
                return;
            }
            canvasElements.SetActive(false);
            highlight.enabled = false;
        }
        else
        {
            canvasElements.SetActive(true);
            magnitudeText.text = currentMagnitude.ToString();
            highlight.enabled = true;
        }
    }
    public void ZplusButton()
    {
        Vector3 direction = transform.forward * currentMagnitude;
        transform.position += direction;
    }
    public void ZminusButton()
    {
        Vector3 direction = transform.forward * currentMagnitude;
        transform.position -= direction;
    }
    public void XplusButton()
    {
        Vector3 direction = transform.right * currentMagnitude;
        transform.position += direction;
    }
    public void XminusButton()
    {
        Vector3 direction = transform.right * currentMagnitude;
        transform.position -= direction;
    }
    public void MagnitudeButton()
    {
        switch(currentMagnitude)
        {
            case 1:
                currentMagnitude = 2;
                break;
            case 2:
                currentMagnitude = 5;
                break;
            default:
                currentMagnitude = 1;
                break;
        }
        magnitudeText.text = currentMagnitude.ToString();
    }
    public void RotateButton()
    {
        if(reverseRotation)
        {
            transform.Rotate(0f, -10 * currentMagnitude, 0f);
        }
        else
        {
            transform.Rotate(0f, 10 * currentMagnitude, 0f);
        }
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
