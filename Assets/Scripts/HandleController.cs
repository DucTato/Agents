using System.Collections;
using System.Collections.Generic;
using TreeEditor;
using Unity.VisualScripting;
using UnityEngine;

public class HandleController : MonoBehaviour
{
    [SerializeField]
    private bool isExtruding, isIntruding, isReturning;
    [SerializeField]
    private float handleSpeed, intrudeMax, extrudeMax;
    [SerializeField]
    private Vector3 movementVector, restVector;
    [SerializeField]
    // Start is called before the first frame update
    void Start()
    {
        isExtruding = false;
        isIntruding = false;
        handleSpeed = 1.1f;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            StartExtrude();
        }
        if(isExtruding)
        {
            transform.localPosition += movementVector * Time.deltaTime;
            if (transform.localPosition.z <= extrudeMax)
            {
                StopAndReturn();
            }
        }
        if(isIntruding)
        {
            transform.localPosition += movementVector * Time.deltaTime;
            if (transform.localPosition.z >= intrudeMax)
            {
                StopAndReturn();
            }
        }
        if(isReturning)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, restVector, 1.1f * Time.deltaTime);
            if (transform.localPosition == restVector) isReturning = false;
        }
    }
    private void StopAndReturn()
    {
        Stop();
        isReturning = true;
    }
    private void Stop()
    {
        isExtruding = false;
        isIntruding = false;
        isReturning = false;
    }
    public void StartExtrude(float Speed)
    {
        Stop();
        isExtruding = true;
        handleSpeed = Speed;
        movementVector = Vector3.back * handleSpeed;
    }
    public void StartExtrude()
    {
        Stop();
        isExtruding = true;
        movementVector = Vector3.back * handleSpeed;
    }
    public void StartIntrude(float Speed)
    {
        Stop();
        isIntruding = true;
        handleSpeed = Speed;
        movementVector = Vector3.forward * handleSpeed;
    }
    public void StartIntrude()
    {
        Stop();
        isIntruding = true;
        movementVector = Vector3.forward * handleSpeed;
    }
}
