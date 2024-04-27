using EasyTransition;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public GameObject currentMenu;
    public TransitionSettings transition;
    public float startDelay = 0.2f;
    public GameObject startingButton;

    public void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        // Set focus to first button found
        EventSystem.current.SetSelectedGameObject(startingButton);
    }

    public void OpenMenu(GameObject loadMenu)
    {
        currentMenu.SetActive(false);
        currentMenu = loadMenu;
        loadMenu.SetActive(true);

        // Find all Buttons in new Menu
        Button[] buttons = loadMenu.GetComponentsInChildren<Button>();

        if (buttons.Length > 0)
        {
            // Set focus to first button found
            EventSystem.current.SetSelectedGameObject(buttons[0].gameObject);

        }
    }

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
        //TransitionManager.Instance().Transition(sceneName, transition, startDelay);
    }

    public void Quit()
    {
        Debug.Log("Application Quit");
        UnityEngine.Application.Quit();
    }
}
