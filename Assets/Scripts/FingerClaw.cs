using UnityEngine;

public class FingerClaw : MonoBehaviour
{
    public bool contact;
    public GameObject grabbedObject;
    
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
