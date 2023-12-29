using UnityEngine;

public class EndMattressScript : MonoBehaviour
{
    private void Awake()
    {
        // Generate Pastel Red color with rgb(160, 233, 255)
        GetComponent<Renderer>().material.color = new Color(0.625f, 0.91f, 1);
    }
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Priority"))
        {
            //Debug.Log("Returned a Cube");
            collision.gameObject.SetActive(false);
        }
    }
    private void Start()
    {
        SystemController.instance.RegisterEndPoint(gameObject);
    }
}
