using System;
using UnityEngine;

public class RobotArmController : MonoBehaviour
{
    public event EventHandler<EventArgs> OnDropCompletion;
    public event EventHandler<EventArgs> OnReset;
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
        armControl.OnResetArm += ArmControl_OnResetArm;
        handleControl.OnSuccessfulGrab += HandleControl_OnSuccessfulGrab;
        handleControl.OnDropObject += HandleControl_OnDropObject;
    }
    private void OnDisable()
    {
        armControl.OnRotationCompleted -= ArmControl_OnRotationCompleted;
        armControl.OnResetArm -= ArmControl_OnResetArm;
        handleControl.OnSuccessfulGrab -= HandleControl_OnSuccessfulGrab;
        handleControl.OnDropObject -= HandleControl_OnDropObject;   
    }
    private void ArmControl_OnResetArm(object sender, EventArgs e)
    {
        OnReset?.Invoke(this, e);
    }
    private void HandleControl_OnDropObject(object sender, EventArgs e)
    {
        OnDropCompletion?.Invoke(this, e);
    }
    private void HandleControl_OnSuccessfulGrab(object sender, HandleController.EventArguments e)
    {
        if(e.GetState() == HandleController.HandleState.ActionFailed)
        {
            armControl.ResetRotation();
        }
    }

    private void ArmControl_OnRotationCompleted(object sender, IKCalculator.EventArguments e)
    {       
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
    public void ResetArm()
    {
        armControl.ResetRotation();
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
    public Vector3 GetClawTipPosition()
    {
        return armControl.GetTipPosition();
    }
    public bool IsArmBusy()
    {
        if (armControl.GetActionType() == IKCalculator.ActionType.NoneAt)
        {
            if (armControl.GetRotationStatus())
            {
                return true;
            }
            else
                return false;
        }
        else
        {
            if (armControl.GetRotationStatus() || handleControl.GetHandleState())
            {
                return true;
            }
            else
                return false;
        }

    }
    public bool IsHandEmpty()
    {
        return handleControl.IsEmpty();
    }
    public GameObject GetCurrentCube()
    {
        return handleControl.GetCube();
    }
    public string GetCurrentTag()
    {
        return handleControl.CurrentTag();
    }
}
