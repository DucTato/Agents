using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class SystemController : MonoBehaviour
{
    // Make System Control a Singleton
    public static SystemController instance;

    // Make Lists of elements that need constant tracking
    [SerializeField]
    private List<GameObject> startMattresses;
    [SerializeField]
    private List<GameObject> endMattresses;
    [SerializeField]
    private List<GameObject> stackMattresses;
    [SerializeField]
    private List<GameObject> robotAgents;
    public enum LocatorType
    {
        Robot,
        EndMattress,
        StackMattress,
    }
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
    public void RegisterRobotArm(GameObject agent)
    {
        robotAgents.Add(agent);
    }
    public void RegisterStartPoint(GameObject startPoint)
    {
        startMattresses.Add(startPoint);
    }
    public void RegisterEndPoint(GameObject endPoint)
    {
        endMattresses.Add(endPoint);
    }
    public void RegisterStackPoint(GameObject stackPoint)
    {
        stackMattresses.Add(stackPoint);
    }
    public Vector3 DropTargetLocator(GameObject startPoint, LocatorType type)
    {
        switch (type)
        {
            case LocatorType.Robot:
                return FindClosestObject(startPoint.transform.position, robotAgents);
            case LocatorType.EndMattress:
                Vector3 result = FindClosestObject(startPoint.transform.position, endMattresses);
                result.y += 1f;
                return result;
            case LocatorType.StackMattress:
                return FindClosestObject(startPoint.transform.position, stackMattresses);
            default:
                return Vector3.zero;
        }
    }

    private Vector3 FindClosestObject(Vector3 point, List<GameObject> list)
    {
        float distance = Mathf.Infinity;
        GameObject closest = null;
        foreach (GameObject obj in list)
        {
            if(Vector3.Distance(point, obj.transform.position) < distance)
            {
                closest = obj;
                distance = Vector3.Distance(point, obj.transform.position);
            }
        }
        return closest.transform.position;
    }
}
