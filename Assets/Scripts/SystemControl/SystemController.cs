using System.Collections;
using System.Collections.Generic;
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
                if (FindClosestObject(startPoint.transform.position, RobotScreening(startPoint.transform.position)) == null) return Vector3.positiveInfinity;
                else return FindClosestObject(startPoint.transform.position, RobotScreening(startPoint.transform.position)).transform.position;
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
                return FindClosestObject(startPoint.transform.position, RobotScreening(startPoint.transform.position));
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
            }
        }
        return closest;
    }
    private List<GameObject> RobotScreening(Vector3 point)
    {
        // The purpose of this function is to only get robots that are closer to the end than the requested robot's position (point)
        var result = new List<GameObject>();
        // The distance between selected robot (point) and the closest end point from that point
        Vector3 closestEnd = FindClosestObject(point, endMattresses).transform.position;
        float distance = Vector3.Distance(point, closestEnd);
        foreach (GameObject seenRobot in robotAgents)
        {
            if (Vector3.Distance(seenRobot.transform.position, closestEnd) < distance)
            {
                result.Add(seenRobot);
            }
        }
        return result;
    }
}
