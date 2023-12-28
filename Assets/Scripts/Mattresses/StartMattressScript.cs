using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartMattressScript : MonoBehaviour
{
    [SerializeField]
    private float spawnDelay;
    private float timerCount;
    [SerializeField]
    private int spawnCount;
    private int index;
    [SerializeField]
    private List<GameObject> cubesToSpawn;
    [SerializeField]
    private GameObject cubePrefab;
    private void Awake()
    {
        // Generate Pastel Red color with rgb(223, 130, 108)
        GetComponent<Renderer>().material.color = new Color(0.87f, 0.5f, 0.42f);
        for (int i = 0; i < spawnCount; i++)
        {
            // Spawn a cube from the start of the scene, then add them to a list 
            cubesToSpawn.Add(Instantiate(cubePrefab).GetComponent<PrioritizedCube>().SetSpawn(gameObject));
        }
    }
    private void Start()
    {
        SystemController.instance.RegisterStartPoint(gameObject);
    }
    // Update is called once per frame
    void Update()
    {
        if (timerCount >= 0)
        {
            timerCount -= Time.deltaTime;
        }
        else
        {
            //Spawn a new cube every spawnDelay second
            index = Random.Range(0, cubesToSpawn.Count);
            if (!cubesToSpawn[index].activeInHierarchy)
            {
                cubesToSpawn[index].SetActive(true);
            }            
            timerCount = spawnDelay;
            //Debug.Log(spawnDelay + " seconds have passed");
        }
    }    
}
