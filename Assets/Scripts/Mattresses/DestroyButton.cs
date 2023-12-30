using UnityEngine;

public class DestroyButton : MonoBehaviour
{
    private float downTime;
    private bool pushedDown;
    // Start is called before the first frame update
    void Start()
    {
        // Generate Color with rgb (255, 91, 34)
        GetComponent<Renderer>().material.color = new Color(1f, 0.356f, 0.133f);
        downTime = 2f;
    }

    // Update is called once per frame
    void Update()
    {
        if (pushedDown)
        {
            if (downTime > 0)
            {
                downTime -= Time.deltaTime;
            }
            else
            {
                downTime = 2f;
                GetComponent<BoxCollider>().enabled = true;
                // Return to normal color
                GetComponent<Renderer>().material.color = new Color(1f, 0.356f, 0.133f);
                pushedDown = false;
                PushedDownState(pushedDown);
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Recolorable"))
        {
            // Activate an explosion
            Debug.Log("Boom");
            GetComponent<BoxCollider>().enabled = false;
            pushedDown = true;
            PushedDownState(pushedDown);
            // Switch to a lighter shade of Orange
            GetComponent<Renderer>().material.color = new Color(1f, 0.76f, 0.49f);
        }
    }
    private void PushedDownState(bool down)
    {
        if(down)
        {
            transform.localPosition = new Vector3(0f, 0.2f, 0f);
        }
        else
        {
            transform.localPosition = new Vector3(0f, 0.9f, 0f);
        }
    }
}
