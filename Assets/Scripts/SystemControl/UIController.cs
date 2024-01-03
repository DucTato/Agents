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
    [SerializeField]
    private GameObject pausePanel;
    private Camera mainCam;
    // Start is called before the first frame update
    void Start()
    { 
        // Populate the scene options in the menu
        mapSelections.Remove(gameObject.scene.name);
        sceneSelectionMenu.AddOptions(mapSelections);
        sceneSelectionMenu.captionText.text = gameObject.scene.name;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyUp(KeyCode.Escape))
        {
            if (pausePanel.activeInHierarchy)
            {
                Cursor.lockState = CursorLockMode.Locked;
                pausePanel.SetActive(false);
                mainCam.GetComponent<MouseLook>().enabled = true;
                mainCam.GetComponent<Spectator>().enabled = true;  
            }
            else
            {
                pausePanel.SetActive(true);
                mainCam.GetComponent<MouseLook>().enabled = false;
                mainCam.GetComponent<Spectator>().enabled = false;
                Cursor.lockState = CursorLockMode.None;
            }
        }
    }
    public void OnSceneSelected(int option)
    {
        SceneManager.LoadScene(mapSelections[option]);
        Debug.Log("Load scene #" + option);
    }
    public void SetMainCam(Camera cam)
    {
        mainCam = cam;
    }
}
