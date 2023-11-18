using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spectator : MonoBehaviour
{
    public static Spectator instance;
    private Camera mainCam;
    //initial speed
    public int speed = 10;
    private void Awake()
    {
        instance = this;
        Cursor.lockState = CursorLockMode.Locked;
        mainCam = Camera.main;
    }

    // Use this for initialization
    //void Start()
    //{

    //}

    // Update is called once per frame
    void Update()
    {

        //press shift to move faster
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            speed = 20;

        }
        else
        {
            //if shift is not pressed, reset to default speed
            speed = 10;
        }
        //For the following 'if statements' don't include 'else if', so that the user can press multiple buttons at the same time
        //move camera to the left
        if (Input.GetKey(KeyCode.A))
        {
            transform.position += mainCam.transform.right * -1 * speed * Time.deltaTime;
        }

        //move camera backwards
        if (Input.GetKey(KeyCode.S))
        {
            transform.position += mainCam.transform.forward * -1 * speed * Time.deltaTime;

        }
        //move camera to the right
        if (Input.GetKey(KeyCode.D))
        {
            transform.position += mainCam.transform.right * speed * Time.deltaTime;

        }
        //move camera forward
        if (Input.GetKey(KeyCode.W))
        {

            transform.position += mainCam.transform.forward * speed * Time.deltaTime;
        }
    }
}
