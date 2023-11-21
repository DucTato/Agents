using UnityEngine;

public class HandleController : MonoBehaviour
{
    [SerializeField]
    private FingerClaw[] fingerClaws;
    [SerializeField]
    private Transform fingerJoint1, fingerJoint2, fingerJoint3;
    public bool isGrabbing = false;
    private bool grabEngage, matchedContact;
    [SerializeField]
    private int contactPoint;
    [SerializeField]
    private float engageSpeed = 15f;
    [SerializeField]
    private GameObject clawHand;
    private GameObject clawGrabbedObject;
    // Quaternion states
    private Quaternion joint1Open = Quaternion.Euler(40, -180, 0);
    private Quaternion joint2Open = Quaternion.Euler(40, 60, 0);
    private Quaternion joint3Open = Quaternion.Euler(40, 300, 0);
    //
    private Quaternion joint1Close = Quaternion.Euler(0, -180, 0);
    private Quaternion joint2Close = Quaternion.Euler(0, 60, 0);
    private Quaternion joint3Close = Quaternion.Euler(0, 300, 0);
    //
    private Vector3 moveDir;
    // Start is called before the first frame update
    void Start()
    {
        moveDir = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartGrab();
        }
        if (grabEngage)
        {
            if (!isGrabbing)
            {
                fingerJoint1.localRotation = Quaternion.RotateTowards(fingerJoint1.localRotation, joint1Close, engageSpeed * 15f * Time.deltaTime);
                fingerJoint2.localRotation = Quaternion.RotateTowards(fingerJoint2.localRotation, joint2Close, engageSpeed * 15f * Time.deltaTime);
                fingerJoint3.localRotation = Quaternion.RotateTowards(fingerJoint3.localRotation, joint3Close, engageSpeed * 15f * Time.deltaTime);
                clawHand.transform.Rotate(0f, 0f, 0.25f);
                Extrude();
                /////////////////////
                for (int i = 0; i < fingerClaws.Length; i++)
                {
                    if (fingerClaws[i].contact)
                    {
                        if (clawGrabbedObject == null)
                        {
                            clawGrabbedObject = fingerClaws[i].grabbedObject;
                        }
                        else
                        {
                            if (clawGrabbedObject == fingerClaws[i].grabbedObject)
                            {
                                matchedContact = true;
                            }
                            else
                            {
                                matchedContact = false;
                            }
                        }
                        contactPoint++;
                    }
                }
                if (contactPoint >= 3 && matchedContact)
                {
                    isGrabbing = true;
                    GrabProcedure();
                    Debug.Log(gameObject.GetInstanceID() + " Grab successful");
                }
                contactPoint = 0;
            }
            else
            {
                Intrude();
            }
        }
        else
        {
            fingerJoint1.localRotation = Quaternion.RotateTowards(fingerJoint1.localRotation, joint1Open, engageSpeed * 60f * Time.deltaTime);
            fingerJoint2.localRotation = Quaternion.RotateTowards(fingerJoint2.localRotation, joint2Open, engageSpeed * 60f * Time.deltaTime);
            fingerJoint3.localRotation = Quaternion.RotateTowards(fingerJoint3.localRotation, joint3Open, engageSpeed * 60f * Time.deltaTime);
            Intrude();
        }

    }
    private void StartGrab()
    {
        if (grabEngage)
        {
            grabEngage = false;
            isGrabbing = false;
            contactPoint = 0;
            UnGrabProcedure(clawGrabbedObject);
            for (int i = 0; i < fingerClaws.Length; i++)
            {
                fingerClaws[i].UpdateContact();
            }
        }
        else
        {
            grabEngage = true;
        }
    }
    private void GrabProcedure()
    {
        clawGrabbedObject.transform.parent = clawHand.transform;
        clawGrabbedObject.GetComponent<Rigidbody>().isKinematic = true;
        clawGrabbedObject.GetComponent<BoxCollider>().enabled = false;
    }
    private void UnGrabProcedure(GameObject target)
    {
        if (target == null) return;
        clawGrabbedObject.GetComponent<BoxCollider>().enabled = true;
        clawGrabbedObject.GetComponent<Rigidbody>().isKinematic = false;
        clawGrabbedObject.transform.parent = null;
        clawGrabbedObject = null;
    }
    private void Extrude()
    {
        if(transform.localPosition.z < 0.6f)
        {
            moveDir = 1.1f * Time.deltaTime * Vector3.forward;
            transform.localPosition += moveDir;
        }
    }
    private void Intrude()
    {
        if(transform.localPosition.z > 0.2f)
        {
            moveDir = -0.7f * Time.deltaTime * Vector3.forward;
            transform.localPosition += moveDir;
        }
    }
}

