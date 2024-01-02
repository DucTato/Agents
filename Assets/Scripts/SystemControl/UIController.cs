using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [SerializeField]
    private List<string> mapSelections;
    [SerializeField]
    private Dropdown sceneSelectionMenu;
    // Start is called before the first frame update
    void Start()
    { 
        // Populate the scene options in the menu
        sceneSelectionMenu.AddOptions(mapSelections);
        sceneSelectionMenu.captionText.text = gameObject.scene.name;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnSceneSelected(int option)
    {
        SceneManager.LoadScene(mapSelections[option]);
    }
}
