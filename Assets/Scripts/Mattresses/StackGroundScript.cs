using UnityEngine;

public class StackGroundScript : MonoBehaviour
{
    private GameObject currentBase;
    private ParticleSystem boomFX;
    // Start is called before the first frame update
    void Start()
    {
        currentBase = null;
        boomFX = transform.root.GetComponent<ParticleSystem>();
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Grabbable"))
        {
            // There's a collision with a stack cube
            if (currentBase == null)
            {
                // If there's no base for the current tower, make this the base
                collision.gameObject.GetComponent<StackCube>().SetKinematic();
                currentBase = collision.gameObject;
            }
            else
            {
                collision.gameObject.GetComponent<StackCube>().UnSetKinematic();
                // Add a little bounce for rejected cubes :D
                collision.gameObject.GetComponent<Rigidbody>().AddForce(transform.up * 2f, ForceMode.Impulse);
            }
        }
    }
    public int GetCurrentHeight()
    {
        return Physics.OverlapSphere(transform.position, 6f, 1 << 8).Length;
    }
    public Vector3 GetCurrentTop()
    {
        if (currentBase == null)
        {
            // There's nothing on the stack area
            return Vector3.zero;
        }
        else
        {
            Collider highest = null;
            foreach (Collider collider in Physics.OverlapSphere(transform.position, 6f, 1 << 8))
            {
                if (highest == null)
                {
                    highest = collider;
                }
                else if(collider.transform.position.y > highest.transform.position.y)
                {
                    highest = collider;
                }
            }
            return highest.transform.position;
        }
    } 
    public void Explode()
    {
        if (currentBase==null)
        { return; }
        boomFX.Play();
        BoxCollider StackGroundCollider = GetComponent<BoxCollider>();
        foreach (Collider collider in Physics.OverlapSphere(transform.position, 6f, 1<<8|1<<3))
        {
            collider.GetComponent<Rigidbody>().isKinematic = false;
            collider.GetComponent<Rigidbody>().AddExplosionForce(15f, StackGroundCollider.bounds.max, 10f, 10f, ForceMode.Impulse);
            currentBase = null;
        }
    }
}
