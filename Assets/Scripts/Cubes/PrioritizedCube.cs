using UnityEngine;

public class PrioritizedCube : MonoBehaviour
{
    private GameObject initialMattress;
    private float touchGround;
    private bool touchedGround;
    private void Awake()
    {
        //Generate a color for the cube
        GetComponent<Renderer>().material.color = Color.red;
    }
    private void Start()
    {
        gameObject.SetActive(false);
        ReturnToSpawn();
    }
    private void Update()
    {
        if (touchedGround && gameObject.CompareTag("Priority"))
        {
            touchGround += Time.deltaTime;
            if (touchGround >= 10f)
            {
                // If a cube stay idle on the ground for 10 seconds then reset the cube's position
                ReturnToSpawn();
            }
        }
    }
    private void ReturnToSpawn()
    {
        touchedGround = false;
        touchGround = 0;
        //Locate the initial mattress that spawned this cube
        if (initialMattress != null)
        {
            BoxCollider boundary = initialMattress.GetComponent<BoxCollider>();
            float randomX = Random.Range(boundary.bounds.center.x - 2f, boundary.bounds.center.x + 2f);
            float randomZ = Random.Range(boundary.bounds.center.z - 1f, boundary.bounds.center.z + 1f);
            float randomY = Random.Range(0.7f, 4f);
            //Debug.Log(gameObject.name + " " + randomX + " " + randomY + " " + randomZ);
            transform.SetPositionAndRotation(new Vector3(randomX, randomY, randomZ), initialMattress.transform.rotation);
        }
    }
    private void OnDisable()
    {
        ReturnToSpawn();
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            //Debug.Log("Touch Ground");
            touchedGround = true;
        }
    }    
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            touchedGround = false; 
            touchGround = 0;
        }
    }
    public GameObject SetSpawn(GameObject mattress)
    {
        initialMattress = mattress;
        return gameObject;
    }
}
