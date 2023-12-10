using UnityEngine;

public class MouseLook : MonoBehaviour
{
    private Spectator camObject;
    private Camera mainCamera;
    Vector2 rotation = Vector2.zero;
    public float lookSpeed = 3f;
    private bool enable;
    // Start is called before the first frame update
    void Start()
    {
        camObject = Spectator.instance;
        mainCamera = Camera.main;
        Cursor.lockState = CursorLockMode.Locked;
        enable = true;
    }

    // Update is called once per frame
    void Update()
    {
        //press Middle Mouse Button (the Wheel) to toggle MouseLook ON/OFF
        if (Input.GetMouseButtonDown(2) && !enable)
        {
            enable = true;
            camObject.ToggleMovement(enable);
            Cursor.lockState = CursorLockMode.Locked;
        }
        else if (Input.GetMouseButtonDown(2) && enable)
        {
            enable = false;
            camObject.ToggleMovement(enable);
            Cursor.lockState = CursorLockMode.None;
        }
        if (enable)
        {
            //move camera upwards
            if (Input.GetMouseButtonDown(0) || Input.GetMouseButton(0))
            {
                transform.position +=  mainCamera.transform.up * camObject.speed * Time.deltaTime;
            }
            //move camera downwards
            if (Input.GetMouseButtonDown(1) || Input.GetMouseButton(1))
            {
                transform.position += mainCamera.transform.up * -camObject.speed * Time.deltaTime;
            }
            rotation.y += Input.GetAxis("Mouse X");
            rotation.x += -Input.GetAxis("Mouse Y");
            transform.eulerAngles = (Vector2)(rotation * lookSpeed);
        }
    }
}
