using UnityEngine;

public class StartMattressScript : MonoBehaviour
{
    [SerializeField]
    private float spawnDelay;
    private float timerCount;
    private int index;
    [SerializeField]
    private GameObject[] cubesToSpawn;
    private void Awake()
    {
        // Generate Pastel Red color with rgb(223, 130, 108)
        GetComponent<Renderer>().material.color = new Color(0.87f, 0.5f, 0.42f);
    }
    // Start is called before the first frame update
    void Start()
    {
        
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
            index = Random.Range(0, cubesToSpawn.Length);
            if (!cubesToSpawn[index].activeInHierarchy)
            {
                cubesToSpawn[index].SetActive(true);
            }            
            timerCount = spawnDelay;
            //Debug.Log(spawnDelay + " seconds have passed");
        }
    }
}
