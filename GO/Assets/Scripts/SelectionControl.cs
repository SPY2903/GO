using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectionControl : MonoBehaviour
{
    [SerializeField] private Button playButton;
    [SerializeField] private Button settingButton;
    [SerializeField] private Button exitButton;
    [SerializeField] private Sprite selectionSprite;
    [SerializeField] private GameObject decidePanel;
    [SerializeField] private GameObject selectionPanel;
    [SerializeField] private GameObject settingPanel;
    private int currentSelection = 1;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
        {
            currentSelection++;
            if(currentSelection > 3)
            {
                currentSelection = 1;
            }
            SetSelection();
        }
        if(Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
        {
            currentSelection--;
            if(currentSelection < 1)
            {
                currentSelection = 3;
            }
            SetSelection();
        }
        if (Input.GetKeyDown(KeyCode.Return))
        {
            AccessSelection();
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (decidePanel.activeInHierarchy)
            {
                decidePanel.SetActive(false);
            }
            if (selectionPanel.activeInHierarchy)
            {
                selectionPanel.SetActive(false);
                decidePanel.SetActive(true);
            }
            if (settingPanel.activeInHierarchy)
            {
                settingPanel.SetActive(false);
            }
        }
    }
    public void SetCurrentSelection(int value)
    {
        currentSelection = value;
    }
    public void SetDefault(Button btn)
    {
        btn.GetComponent<Image>().sprite = null;
        btn.GetComponent<Image>().color = new Color(255,255,255,0);
    }
    public void SetSelection(Button btn)
    {
        btn.GetComponent<Image>().sprite = selectionSprite;
        btn.GetComponent<Image>().color = new Color(255, 255, 255, 255);
    }
    public void PlaySelection()
    {
        selectionPanel.SetActive(false);
    }
    public void SetSelection()
    {
        if (currentSelection == 1)
        {
            SetSelection(playButton);
            SetDefault(settingButton);
            SetDefault(exitButton);
        }
        if (currentSelection == 2)
        {
            SetDefault(playButton);
            SetSelection(settingButton);
            SetDefault(exitButton);
        }
        if (currentSelection == 3)
        {
            SetDefault(playButton);
            SetDefault(settingButton);
            SetSelection(exitButton);
        }
    }
    public void AccessSelection() 
    {
        if (currentSelection == 1)
        {
            decidePanel.SetActive(true);
        }
        if (currentSelection == 2)
        {
            settingPanel.SetActive(true);
        }
        if (currentSelection == 3)
        {
            Application.Quit();
        }
    }
}
