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
    }

    public void Quit()
    {
        Debug.Log("Application Quit");
        UnityEngine.Application.Quit();
    }
}
