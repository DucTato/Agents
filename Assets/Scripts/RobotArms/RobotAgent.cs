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
    private HashSet<GameObject> stackCubeSet;
    // Start is called before the first frame update
    void Start()
    {
        sysControl = SystemController.instance;
        // Register to the System upon being activated
        sysControl.RegisterRobotArm(this);
        robotControl = GetComponent<RobotArmController>();
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
            
            pollingCount = pollingRate;
            
            Debug.Log(prioritizedCubeSet.Count +" Priority");
            Debug.Log(stackCubeSet.Count +" Stack");
        }
        switch(decision)
        {
            case PerceptionStates.DoPriority:
                break;
        }
    }
    private void See()
    {
        prioritizedCubeSet = GameObject.FindGameObjectsWithTag("Priority").ToHashSet();
        stackCubeSet = GameObject.FindGameObjectsWithTag("Grabbable").ToHashSet();
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
