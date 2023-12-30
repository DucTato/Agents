using System;
using UnityEngine;


public class IKCalculator : MonoBehaviour
{
    public event EventHandler<EventArguments> OnRotationCompleted;
    public event EventHandler<EventArgs> OnResetArm;
    public Transform pivot, upper, lower, effector, tip;
    public Vector3 normal = Vector3.up;
    [SerializeField]
    private float rotationSpeed;
    private float upperLength, lowerLength, effectorLength;
    private Vector3 effectorTarget, tipTarget;
    private Quaternion desiredPivot, desiredUpper, desiredLower, desiredEffector;
    
    private bool isRotating;
    
    // Quaternion states
    private Quaternion pivotReset = Quaternion.Euler(-90, 0, 0);
    private Quaternion zeroQuaternion = Quaternion.Euler(0, 0, 0);

    private ActionType actionUponRotation;
    public enum ActionType
    {
        PickAt,
        DropAt,
        NoneAt
    }
    public class EventArguments : EventArgs
    {
        
        private ActionType eventType;
        
        public EventArguments(ActionType type)
        {
            eventType = type;
        }
        public ActionType GetEvent()
        {
            return eventType;
        }
    }
    void Reset()
    {
        pivot = transform;
        
        try
        {
            upper = pivot.GetChild(0);
            lower = upper.GetChild(0);
            effector = lower.GetChild(0);
            tip = effector.GetChild(0);
        }
        catch (UnityException)
        {
            Debug.Log("Could not find required transforms, please assign manually.");
        }
    }

    void Awake()
    {
        upperLength = (lower.position - upper.position).magnitude;
        lowerLength = (effector.position - lower.position).magnitude;
        effectorLength = (tip.position - effector.position).magnitude;
    }


    void Solve()
    {
        Vector3 pivotDir = effectorTarget - pivot.position;
        desiredPivot = Quaternion.LookRotation(pivotDir);

        Vector3 upperToTarget = (effectorTarget - upper.position);
        float a = upperLength;
        float b = lowerLength;
        float c = upperToTarget.magnitude;

        float B = Mathf.Acos((c * c + a * a - b * b) / (2 * c * a)) * 57.29578f;
        float C = Mathf.Acos((a * a + b * b - c * c) / (2 * a * b)) * 57.29578f;
        // 1 radian = 57.29578 degree
        if (!float.IsNaN(C))
        {
            Quaternion upperRotation = Quaternion.AngleAxis((-B), Vector3.right);
            desiredUpper = upperRotation;
            Quaternion lowerRotation = Quaternion.AngleAxis(180 - C, Vector3.right);
            desiredLower = lowerRotation;
        }
        
    }

    void Update()
    {
        if (isRotating)
        {
            pivot.rotation = Quaternion.RotateTowards(pivot.rotation, desiredPivot, rotationSpeed * Time.deltaTime);
            upper.localRotation = Quaternion.RotateTowards(upper.localRotation, desiredUpper, rotationSpeed * Time.deltaTime);
            lower.localRotation = Quaternion.RotateTowards(lower.localRotation, desiredLower, rotationSpeed * Time.deltaTime);
            Quaternion effectorRotation = Quaternion.LookRotation(tipTarget - effector.position);
            desiredEffector = effectorRotation;
            effector.rotation = Quaternion.RotateTowards(effector.rotation, desiredEffector, rotationSpeed * Time.deltaTime);
            if (effector.rotation == desiredEffector)
            {
                //Finish rotating the entire arm, publish an event for the arm to catch
                isRotating = false;
                OnRotationCompleted?.Invoke(this, new EventArguments(actionUponRotation));
                actionUponRotation = ActionType.NoneAt;
            }
        }
    }
    public void StartRotation(Vector3 desiredTarget)
    {
        tipTarget = desiredTarget;
        effectorTarget = tipTarget + normal * effectorLength;
        Solve();
        isRotating = true;
    }
    public void ResetRotation()
    {
        actionUponRotation = ActionType.NoneAt;
        desiredPivot = pivotReset;
        desiredLower = zeroQuaternion;
        desiredUpper = zeroQuaternion;
        tipTarget = new Vector3(transform.parent.position.x, transform.parent.position.y + 12, transform.parent.position.z);
        effectorTarget = tipTarget + normal * effectorLength;
        isRotating = true;
        OnResetArm?.Invoke(this, EventArgs.Empty);
        //Debug.Log("Reset");
    }
    public void SetActionUponRotation(ActionType action)
    {
        actionUponRotation = action;
    }
    public bool GetRotationStatus()
    {
        return isRotating;
    }
    public ActionType GetActionType()
    {
        return actionUponRotation;
    }
    public Vector3 GetTipPosition()
    {
        return tip.position;
    }
}
