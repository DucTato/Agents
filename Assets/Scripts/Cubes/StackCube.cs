using UnityEngine;

public class StackCube : MonoBehaviour
{
    private bool touchedGround, isKinematic;
    private float touchGround;
    private GameObject initialMattress;
    private void Awake()
    {
        GetComponent<Renderer>().material.color = Random.ColorHSV(0f, 1f, 0f, 0f, 0f, 1f);
    }
    // Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(false);
        ReturnToSpawn();
    }

    // Update is called once per frame
    void Update()
    {
        if (touchedGround && gameObject.CompareTag("Grabbable"))
        {
            touchGround += Time.deltaTime;
            if (touchGround > 5f)
            {
                ReturnToSpawn(); 
            }
        }
        else
        {
            // Reset the timer upon being picked up 
            touchGround = 0f;
        }
        if (transform.position.y < -10f)
        {
            // If the cube somehow falls out of the map, reset the cube position
            gameObject.SetActive(false);
        }
        if (isKinematic)
        {
            if (gameObject.GetComponent<Rigidbody>().velocity == Vector3.zero)
            {
                gameObject.tag = "StackedCube";
                gameObject.GetComponent<Rigidbody>().isKinematic = true;
                gameObject.layer = LayerMask.NameToLayer("StackedCubes");
                // Reset the state of the kinematic call
                isKinematic = false;
            }
        }
    }
    private void OnEnable()
    {
        gameObject.tag = "Grabbable";
        gameObject.layer = LayerMask.NameToLayer("Default");
    }
    private void OnDisable()
    {
        ReturnToSpawn();
    }
    private void ReturnToSpawn()
    {
        touchedGround = false;
        touchGround = 0;
        //Locate the initial mattress that spawned this cube
        if (initialMattress != null)
        {
            BoxCollider boundary = initialMattress.GetComponent<BoxCollider>();
            float randomX = Random.Range(boundary.bounds.center.x - 0.8f, boundary.bounds.center.x + 0.8f);
            float randomZ = Random.Range(boundary.bounds.center.z - 0.5f, boundary.bounds.center.z + 0.5f);
            float randomY = Random.Range(0.7f, 4f);
            //Debug.Log(gameObject.name + " " + randomX + " " + randomY + " " + randomZ);
            transform.SetPositionAndRotation(new Vector3(randomX, randomY, randomZ), initialMattress.transform.rotation);
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            if (gameObject.CompareTag("StackedCube"))
            {
                gameObject.SetActive(false);
            }
            //Debug.Log("Touch Ground");
            touchedGround = true;
        }
        if (collision.gameObject.CompareTag("StackedCube"))
        {
            // This cube has just hit another stacked cube and is now considered a stacked cube
            SetKinematic();
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
    public void SetKinematic()
    {
        isKinematic = true;
    }
    public void UnSetKinematic()
    {
        // This method is for unsetting the kinematic state :D
        isKinematic = false;
        gameObject.tag = "Grabbable";
        gameObject.GetComponent<Rigidbody>().isKinematic = false;
        gameObject.layer = LayerMask.NameToLayer("Default");
    }
    public GameObject SetSpawn(GameObject mattress)
    {
        initialMattress = mattress;
        return gameObject;
    }
}
