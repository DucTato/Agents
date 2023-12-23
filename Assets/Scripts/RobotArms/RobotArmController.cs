using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class RobotArmController : MonoBehaviour
{
    [SerializeField]
    private HandleController handleControl;
    [SerializeField]
    private IKCalculator armControl;
    public Vector3 intendedTarget;

    public bool isPicking;
    // Start is called before the first frame update
    void Start()
    {
        armControl.OnRotationCompleted += ArmControl_OnRotationCompleted;
    }

    private void ArmControl_OnRotationCompleted(object sender, IKCalculator.EventArguments e)
    {
        Debug.Log("Done Rotating");
        
        switch (e.GetEvent())
        {
            case IKCalculator.ActionType.PickAt:
                handleControl.StartGrab();
                break;
            case IKCalculator.ActionType.DropAt: 
                handleControl.EndGrab();
                break;
            case IKCalculator.ActionType.NoneAt: 
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            DropAtTarget(intendedTarget);
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            PickAtTarget(intendedTarget);
        }
        if (Input.GetKeyDown(KeyCode.G))
        {
            armControl.ResetRotation();
        }
        
    }
    public void PickAtTarget(Vector3 target)
    {
        armControl.StartRotation(target);
        armControl.SetActionUponRotation(IKCalculator.ActionType.PickAt);
    }
    public void DropAtTarget(Vector3 target)
    {
        armControl.StartRotation(target);
        armControl.SetActionUponRotation(IKCalculator.ActionType.DropAt);
    }
    private void OnDisable()
    {
        armControl.OnRotationCompleted -= ArmControl_OnRotationCompleted;
    }
}
