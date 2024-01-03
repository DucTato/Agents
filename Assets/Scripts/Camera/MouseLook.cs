using UnityEngine;

public class MouseLook : MonoBehaviour
{
    //private Camera mainCamera;
    Vector2 rotation = Vector2.zero;
    public float lookSpeed = 3f;
    private bool enable;
    [SerializeField]
    private KeyCode[] inputKeycodes;
    [SerializeField]
    private float speed;
    private bool movementUnlocked = true;
    /// <summary>
    /// index       Command
    /// 0           cursorToggle
    /// 1           camUp
    /// 2           camDown
    /// 3           camFwd
    /// 4           camBwd
    /// 5           camLeft
    /// 6           camRight
    /// 7           camSpeed
    /// </summary>
    // Start is called before the first frame update
    void Start()
    {
        //mainCamera = Camera.main;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible= true;
        enable = true;
        SystemController.instance.GetComponent<UIController>().SetMainCam(GetComponent<Camera>());
    }

    // Update is called once per frame
    void Update()
    {
        //press a key to toggle MouseLook ON/OFF
        if (Input.GetKeyDown(inputKeycodes[0]) && !enable)
        {
            enable = true;
            movementUnlocked = enable;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible= false;
        }
        else if (Input.GetKeyDown(inputKeycodes[0]) && enable)
        {
            enable = false;
            movementUnlocked = enable;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible= true;
        }
        if (enable)
        {
            //move camera upwards
            if (Input.GetKey(inputKeycodes[1]) || Input.GetKeyDown(inputKeycodes[1]))
            {
                transform.position +=  transform.up * speed * Time.deltaTime;
            }
            //move camera downwards
            if (Input.GetKey(inputKeycodes[2]) || Input.GetKeyDown(inputKeycodes[2]))
            {
                transform.position += transform.up * -speed * Time.deltaTime;
            }
            rotation.y += Input.GetAxis("Mouse X");
            rotation.x += -Input.GetAxis("Mouse Y");
            transform.eulerAngles = (rotation * lookSpeed);
        }
        if (movementUnlocked)
        {
            //Hold down a key to move faster
            if (Input.GetKey(inputKeycodes[7]))
            {
                speed = 20;
            }
            else
            {
                //reset to default speed after key released
                speed = 10;
            }

            //For the following 'if statements' don't include 'else if', so that the user can press multiple buttons at the same time
            //move camera to the left
            if (Input.GetKey(inputKeycodes[5]))
            {
                transform.position += transform.right * -speed * Time.deltaTime;
            }

            //move camera backwards
            if (Input.GetKey(inputKeycodes[4]))
            {
                transform.position += transform.forward * -speed * Time.deltaTime;

            }
            //move camera to the right
            if (Input.GetKey(inputKeycodes[6]))
            {
                transform.position += transform.right * speed * Time.deltaTime;

            }
            //move camera forward
            if (Input.GetKey(inputKeycodes[3]))
            {

                transform.position += transform.forward * speed * Time.deltaTime;
            }
        }
    }
    public void RemapControl(int index,KeyCode input)
    {
        inputKeycodes[index] = input;
    }
    public KeyCode GetCurrentBind(int index)
    {
        return inputKeycodes[index];
    }
}
