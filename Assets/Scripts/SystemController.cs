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
    public Vector3 FindClosestPositionWithType(GameObject startPoint, LocatorType type)
    {
        switch (type)
        {
            case LocatorType.Robot:
                return FindClosestObject(startPoint.transform.position, robotAgents).transform.position;
            case LocatorType.EndMattress:
                return FindClosestObject(startPoint.transform.position, endMattresses).transform.position;
            case LocatorType.StackMattress:
                return FindClosestObject(startPoint.transform.position, stackMattresses).transform.position;
            default:
                return Vector3.zero;
        }
    }
    public GameObject FindClosestObjectWithType(GameObject startPoint, LocatorType type)
    {
        switch (type)
        {
            case LocatorType.Robot:
                return FindClosestObject(startPoint.transform.position, robotAgents);
            case LocatorType.EndMattress:
                return FindClosestObject(startPoint.transform.position, endMattresses);
            case LocatorType.StackMattress:
                return FindClosestObject(startPoint.transform.position, stackMattresses);
            default:
                return null;
        }
    }
    private GameObject FindClosestObject(Vector3 point, List<GameObject> list)
    {
        float distance = Mathf.Infinity;
        GameObject closest = null;
        foreach (GameObject obj in list)
        {
            if (obj.transform.position == point)
            {
                // Skip the check if encounter object itself, lest the result will be 0 :>
                continue;
            }
            if (Vector3.Distance(point, obj.transform.position) < distance)
            {
                closest = obj;
                distance = Vector3.Distance(point, obj.transform.position);
                //Debug.Log(distance.ToString());
            }
        }
        return closest;
    }
}
