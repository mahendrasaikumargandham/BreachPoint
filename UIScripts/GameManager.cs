using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{   
    public bool isMenuOpened = false;
    public GameObject menuUI;

    private void Update() {
        if(Input.GetKeyDown(KeyCode.Escape) && !isMenuOpened) {
            menuUI.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            isMenuOpened = true;
        }

        else if(Input.GetKeyDown(KeyCode.Escape) && isMenuOpened) {
            menuUI.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            isMenuOpened = false;
        }
    }

    public void LeaveMatch() {
        SceneManager.LoadScene("Lobby");
    }

    public void LeaveGame() {
        Debug.Log("Game Leaved");
        Application.Quit();
    }
}
