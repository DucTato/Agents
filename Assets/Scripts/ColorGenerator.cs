using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorGenerator : MonoBehaviour
{
    private Color randomColor;
    // Start is called before the first frame update
    void Start()
    {
        randomColor = Random.ColorHSV(0f, 1f, 1f, 1f, 0f, 1f);
        foreach (Transform child in transform.GetComponentsInChildren<Transform>())
        { 
            if (child.gameObject.CompareTag("Recolorable"))
            {
                child.GetComponent<Renderer>().material.color = randomColor;
            }
        }    

    }
}
