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

    private void ArmControl_OnRotationCompleted(object sender, System.EventArgs e)
    {
        Debug.Log("Done Rotating");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            handleControl.StartGrab();
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            armControl.StartRotation(intendedTarget);
        }
        if (Input.GetKeyDown(KeyCode.G))
        {
            armControl.ResetRotation();
        }
        
    }
    public void PickAtTarget(Vector3 target)
    {
        armControl.StartRotation(target);
        
    }
    private void OnDisable()
    {
        armControl.OnRotationCompleted -= ArmControl_OnRotationCompleted;
    }
}
