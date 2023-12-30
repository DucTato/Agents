
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
    [SerializeField]
    private StackGroundScript groundScript;
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
    public Vector3 GetStackLocation()
    {
        //Debug.Log(transform.GetChild(0).name);
        return groundScript.transform.position;
    }
    public Vector3 GetButtonLocation()
    {
        return transform.parent.GetChild(2).transform.position;
    }
    public int GetCurrentTowerStack()
    {
        return groundScript.GetCurrentHeight();
    }
    public Vector3 GetStackTop()
    {
        return groundScript.GetCurrentTop();
    }
}
public enum StackType
{
    Tower,
    Pyramid
}
