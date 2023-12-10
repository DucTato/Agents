using System;
using UnityEngine;

public class HandleController : MonoBehaviour
{
    public event EventHandler OnSuccessfulGrab;
    [SerializeField]
    private FingerClaw[] fingerClaws;
    [SerializeField]
    private Transform fingerJoint1, fingerJoint2, fingerJoint3;
    public bool isGrabbing = false;
    private bool grabEngage, matchedContact;
    private int contactPoint;
    [SerializeField]
    private float engageSpeed = 15f;
    [SerializeField]
    private GameObject clawHand;
    private GameObject clawGrabbedObject;
    private string grabbedTag;
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
        if (grabEngage)
        {
            if (!isGrabbing)
            {
                fingerJoint1.localRotation = Quaternion.RotateTowards(fingerJoint1.localRotation, joint1Close, engageSpeed * 15f * Time.deltaTime);
                fingerJoint2.localRotation = Quaternion.RotateTowards(fingerJoint2.localRotation, joint2Close, engageSpeed * 15f * Time.deltaTime);
                fingerJoint3.localRotation = Quaternion.RotateTowards(fingerJoint3.localRotation, joint3Close, engageSpeed * 15f * Time.deltaTime);
                //Rotate the claw hand clockwise
                clawHand.transform.Rotate(0f, 0f, 0.25f);
                Extrude();
                //Contact claw collision check. If all 3 claws are touching the same object then contactPoint will increase. 
                //If there're 3 contact points then the collision check will pass
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
                    OnSuccessfulGrab?.Invoke(this, EventArgs.Empty);
                }
                contactPoint = 0;
            }
            else
            {
                // Retreat the claw handle upon a successful grab
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
    public void StartGrab()
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
        grabbedTag = clawGrabbedObject.tag;
        clawGrabbedObject.tag = "Untagged";
        clawGrabbedObject.GetComponent<Rigidbody>().isKinematic = true;
        clawGrabbedObject.GetComponent<BoxCollider>().enabled = false;
    }
    private void UnGrabProcedure(GameObject target)
    {
        if (target == null) return;
        clawGrabbedObject.GetComponent<BoxCollider>().enabled = true;
        clawGrabbedObject.GetComponent<Rigidbody>().isKinematic = false;
        clawGrabbedObject.tag = grabbedTag;
        clawGrabbedObject.transform.parent = null;
        clawGrabbedObject = null;
    }
    private void Extrude()
    {
        if(transform.localPosition.z < 0.6f)
        {
            moveDir = 1.2f * Time.deltaTime * Vector3.forward;
            transform.localPosition += moveDir;
        }
    }
    private void Intrude()
    {
        if(transform.localPosition.z > -0.15f)
        {
            moveDir = -0.8f * Time.deltaTime * Vector3.forward;
            transform.localPosition += moveDir;
        }
    }
}

