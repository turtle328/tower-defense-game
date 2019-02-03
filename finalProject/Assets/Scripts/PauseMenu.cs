using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour {

    public static bool isPaused;
    public LevelManager lm;
    public AudioManager am;
    public GameObject pauseMenuUI;
    public float volPercent = 0.2f;

    private void Start()
    {
        lm = LevelManager.getInstance();
        am = AudioManager.instance;
    }

    // Update is called once per frame
    void Update () {
		if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused) Resume();
            else Pause();
        }
	}

    public void Resume()
    {
        am.SetVolPercent(1 / volPercent);
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }

    public void Pause()
    {
        am.SetVolPercent(volPercent);
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
    }

    public void TogglePause()
    {
        if (isPaused)
        {
            isPaused = !isPaused;
            Resume();
        }
        else
        {
            isPaused = !isPaused;
            Pause();
        }
    }

    public void ToggleBats()
    {
        BatHandler.getInstance().ToggleBats();
    }

    public void LoadData()
    {
        lm.LoadData();
    }

    public void SaveData()
    {
        lm.SaveData();
    }

    public void ReloadGame()
    {
        Resume();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadMenu()
    {
        Resume();
        SceneManager.LoadScene("MainMenu");
    }
}
