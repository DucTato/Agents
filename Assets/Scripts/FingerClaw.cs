using UnityEngine;

public class FingerClaw : MonoBehaviour
{
    public bool contact;
    public GameObject grabbedObject;
    
    // Start is called before the first frame update
    //void Start()
    //{
        
    //}

    // Update is called once per frame
    //void Update()
    //{
        
    //}
    private void OnTriggerEnter (Collider other)
    {
        if (other.gameObject.CompareTag("Grabbable"))
        {
            contact = true;
            grabbedObject = other.gameObject;
            //Debug.Log(other.gameObject);
        }    
    }
    private void OnTriggerExit (Collider other)
    {
        if(other.gameObject.CompareTag("Grabbable"))
        {
            UpdateContact();
        }    
    }
    public void UpdateContact()
    {
        contact = false;
        grabbedObject = null;
    }
}
