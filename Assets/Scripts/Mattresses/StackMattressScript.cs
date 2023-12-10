using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StackMattressScript : MonoBehaviour
{
    [SerializeField]
    private StackType type;
    [SerializeField]
    private int spawnCount;
    [SerializeField]
    private float spawnDelay;
    private float timerCount;
    [SerializeField]
    private GameObject cubePrefab;
    [SerializeField]
    private Text cubeCountText;
    [SerializeField]
    private List<GameObject> cubesToSpawn;
    private int index, cubeCount;
    private void Awake()
    {
        
        UpdateUIElement(cubeCount);
        if (type == 0)
        {
            gameObject.tag = "Tower";
            // Generate color of this mattress with rgb(208, 242, 136)
            GetComponent<Renderer>().material.color = new Color(0.8125f, 0.945f, 0.53f);
        }
        else
        {
            gameObject.tag = "Pyramid";
            // Generate color of this mattress with rgb(220, 191, 255)
            GetComponent<Renderer>().material.color = new Color(0.8593f, 0.746f, 1);
        }
        for (int i = 0; i < spawnCount; i++)
        {
            cubesToSpawn.Add(Instantiate(cubePrefab).GetComponent<StackCube>().SetSpawn(gameObject));
        }
    }
    private void Start()
    {
        SystemController.instance.RegisterStackPoint(this);
    }
    // Update is called once per frame
    void Update()
    {
        if (timerCount >= 0f)
        {
            timerCount -= Time.deltaTime;
        }
        else
        {
            //Spawn a cube every timeDelay second
            index = Random.Range(0, cubesToSpawn.Count);
            if (!cubesToSpawn[index].activeInHierarchy)
            {
                cubesToSpawn[index].SetActive(true);
            }
            timerCount = spawnDelay;
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Grabbable"))
        {
            UpdateUIElement(++cubeCount);
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if(collision.gameObject.CompareTag("Grabbable"))
        {
            UpdateUIElement(--cubeCount);
        }
    }
    private void UpdateUIElement(int currentCount)
    {
        cubeCountText.text = currentCount + "/" + spawnCount;
    }
}
public enum StackType
{
    Tower,
    Pyramid
}
