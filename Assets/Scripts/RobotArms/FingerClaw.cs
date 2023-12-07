using UnityEngine;

public class FingerClaw : MonoBehaviour
{
    public bool contact;
    public GameObject grabbedObject;
    
    private void OnTriggerEnter (Collider other)
    {
        if (other.gameObject.CompareTag("Grabbable") || other.gameObject.CompareTag("Priority"))
        {
            contact = true;
            grabbedObject = other.gameObject;
            //Debug.Log(other.gameObject);
        }    
    }
    private void OnTriggerExit (Collider other)
    {
        if(other.gameObject.CompareTag("Grabbable") || other.gameObject.CompareTag("Priority"))
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
