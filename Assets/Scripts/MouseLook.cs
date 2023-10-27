using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
    private Spectator camObject;
    private Camera mainCamera;
    Vector2 rotation = Vector2.zero;
    public float lookSpeed = 3f;
    private bool disabled = false;
    // Start is called before the first frame update
    void Start()
    {
        camObject = Spectator.instance;
        mainCamera = Camera.main;
        Cursor.lockState = CursorLockMode.Locked; 

    }

    // Update is called once per frame
    void Update()
    {
        //press Middle Mouse Button (the Wheel) to toggle MouseLook ON/OFF
        if (Input.GetMouseButtonDown(2) && (disabled == false))
        {
            disabled = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else if (Input.GetMouseButtonDown(2) && (disabled == true))
        {
            disabled = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
        if (!disabled)
        {
            //move camera upwards
            if (Input.GetMouseButtonDown(0) || Input.GetMouseButton(0))
            {
                transform.position = transform.position + mainCamera.transform.up * camObject.speed * Time.deltaTime;
            }
            //move camera downwards
            if (Input.GetMouseButtonDown(1) || Input.GetMouseButton(1))
            {
                transform.position = transform.position + mainCamera.transform.up * -1 * camObject.speed * Time.deltaTime;
            }
            rotation.y += Input.GetAxis("Mouse X");
            rotation.x += -Input.GetAxis("Mouse Y");
            transform.eulerAngles = (Vector2)(rotation * lookSpeed);
        }
    }
}
