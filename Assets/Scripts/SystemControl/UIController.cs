using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [SerializeField]
    private List<string> mapSelections;
    [SerializeField]
    private Button[] controlButtons;
    [SerializeField]
    private Dropdown sceneSelectionMenu;
    [SerializeField]
    private GameObject pausePanel, mapPanel;
    private Camera mainCam;
    private int buttonIndex;
    // Start is called before the first frame update
    void Start()
    { 
        // Populate the scene options in the menu
        mapSelections.Remove(gameObject.scene.name);
        sceneSelectionMenu.AddOptions(mapSelections);
        sceneSelectionMenu.captionText.text = gameObject.scene.name;

        // Add a listener to each button
        for (int i = 0; i <controlButtons.Length; i++)
        {
            int tempValue = i;
            controlButtons[i].onClick.AddListener(() => ButtonPress(tempValue));
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyUp(KeyCode.Escape))
        {
            if (pausePanel.activeInHierarchy)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                pausePanel.SetActive(false);
                mainCam.GetComponent<MouseLook>().enabled = true;
            }
            else
            {
                pausePanel.SetActive(true);
                mainCam.GetComponent<MouseLook>().enabled = false;
                Cursor.lockState = CursorLockMode.None;
                // Upon activating the pause panel, update all the buttons
                for (int i = 0; i < controlButtons.Length; i++)
                {
                    UpdateButtonText(i);
                }
            }
        }
        if(mapPanel.activeInHierarchy)
        {
            // While the map button panel is being opened, user can press any key to bind value
            foreach (KeyCode pressed in System.Enum.GetValues(typeof(KeyCode)))
            {
                if(Input.GetKey(pressed))
                {
                    // Assign new key value for this command
                    BindValueToKey(buttonIndex, pressed);
                    // Update the UI elements
                    UpdateButtonText(buttonIndex);
                    mapPanel.SetActive(false);
                }
            }
        }
    }

    private void ButtonPress(int index)
    {
        // Store the index of this button for later use
        buttonIndex = index;
        mapPanel.SetActive(true);
    }
    private void UpdateButtonText(int index)
    {
        controlButtons[index].GetComponentInChildren<Text>().text = mainCam.GetComponent<MouseLook>().GetCurrentBind(index).ToString();
    }
    private void BindValueToKey(int index, KeyCode value)
    {
        mainCam.GetComponent<MouseLook>().RemapControl(index, value);
    }
    public void SaveButton()
    {
        pausePanel.SetActive(false);
    }
    public void OnSceneSelected(int option)
    {
        SceneManager.LoadScene(mapSelections[option]);
    }
    public void SetMainCam(Camera cam)
    {
        mainCam = cam;
    }
}
