using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class RobotArmController : MonoBehaviour
{
    [SerializeField]
    private HandleController handleControl;
    [SerializeField]
    private IKCalculator armControl;
    public Vector3 intendedTarget;
    // Start is called before the first frame update
    void Start()
    {
       
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
}
