using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SystemController : MonoBehaviour
{
    // Make System Control a Singleton
    public static SystemController instance;

    // Make Lists of elements that need constant tracking
    [SerializeField]
    private List<StartMattressScript> startMattresses;
    [SerializeField]
    private List<EndMattressScript> endMattresses;
    [SerializeField]
    private List<StackMattressScript> stackMattresses;
    [SerializeField]
    private List<RobotAgent> robotAgents;
    private void Awake()
    {
        instance = this; 
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void RegisterRobotArm(RobotAgent agent)
    {
        robotAgents.Add(agent);
    }
    public void RegisterStartPoint(StartMattressScript startPoint)
    {
        startMattresses.Add(startPoint);
    }
    public void RegisterEndPoint(EndMattressScript endPoint)
    {
        endMattresses.Add(endPoint);
    }
    public void RegisterStackPoint(StackMattressScript stackPoint)
    {
        stackMattresses.Add(stackPoint);
    }
}
