
using System.Collections.Generic;
using UnityEngine;


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
    private List<GameObject> cubesToSpawn;
    private int index;
    private void Awake()
    {
        
       
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
        SystemController.instance.RegisterStackPoint(gameObject);
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
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Grabbable") || other.gameObject.CompareTag("StackedCube"))
        {
            //Debug.Log("Returned a stack cube");
            other.gameObject.SetActive(false);
        }
    }
    public Vector3 GetStackLocation()
    {
        //Debug.Log(transform.GetChild(0).name);
        return transform.GetChild(0).transform.position;
    }
    public Vector3 GetButtonLocation()
    {
        //Debug.Log(transform.GetChild(1).name);
        return transform.GetChild(1).transform.position;
    }
}
public enum StackType
{
    Tower,
    Pyramid
}
