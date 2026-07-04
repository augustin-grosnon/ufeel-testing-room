using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static bool GameisPaused = false;

    public GameObject PauseMenuUI;
    public FirstPersonController PlayerController;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            if (GameisPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Resume()
    {
        PauseMenuUI.SetActive(false);
        Time.timeScale = 1f;

        PlayerController.enabled = true;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        GameisPaused = false;
    }

    public void Pause()
    {
        PauseMenuUI.SetActive(true);
        Time.timeScale = 0f;

        PlayerController.enabled = false;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        GameisPaused = true;
    }

    public static void GoToLobby()
    {
        if (SceneManager.GetActiveScene().name != "TestingRoom")
        {
            SceneManager.LoadScene("TestingRoom");
        }

        UFeel.UFeelAPI.ToggleOffEverything();
        UFeelDebugHUD.Clear();
    }

    public void LoadLobby()
    {
        Debug.Log("Load Lobby");
        Resume();
        GoToLobby();
    }

    public static void QuitGame()
    {
        Debug.Log("Quit Game");
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
