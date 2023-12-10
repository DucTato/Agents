using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotAction
{
    private Vector3 target;
    private bool finishAction;
    public void SetTarget(Vector3 position)
    {
        target = position;
    }
    public void SetStatus(bool value)
    {
        finishAction = value;
    }
    public bool CheckStatus()
    {
        return finishAction;
    }
    public void DoAtTarget()
    {
        
    }
}
public class ActionSequence
{
    public List<RobotAction> sequenceOfAction;
}
