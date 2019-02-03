using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour {

    public Slider sliderInstance;

    private void Start()
    {
        sliderInstance.value = AudioManager.instance.curVol;
    }

    public void PlayGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void LoadGame()
    {
        LevelManager.getInstance().LoadData();
    }

    public void QuitGame()
    {
        Debug.Log("Game exited.");
        Application.Quit();
    }

    public void SetVolume(float vol)
    {
        AudioManager.instance.SetVolume(vol);
    }
}
