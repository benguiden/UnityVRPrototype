using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour {

    public MenuTrigger playTrigger, exitTrigger, musicTrigger;

    public AudioSource musicSource;

    public AudioSource micInputSource;

    public Transform micCalibrationLine;
    public float micCalibrationScale;

    public SpriteRenderer micInputRenderer;
    public Sprite micInputSpr, touchInputSpr;

    public static MenuManager main;

    private void Awake() {
        main = this;

        if (!GlobalManager.music) {
            musicSource.enabled = false;
        }
    }

    public void UpdateMicLine(float progress) {
        micCalibrationLine.localScale = new Vector3 (progress * micCalibrationScale, micCalibrationLine.localScale.y, 1f);
    }

    public void Trigger(MenuTrigger.MenuButton button) {
        if (enabled) {
            switch (button) {
                case MenuTrigger.MenuButton.Play:
                    enabled = false;
                    UnityEngine.SceneManagement.SceneManager.LoadScene (1);
                    break;
                case MenuTrigger.MenuButton.Exit:
                    enabled = false;
                    Debug.Log ("Quitting App..");
                    Application.Quit ();
                    break;
                case MenuTrigger.MenuButton.Music:
                    if (GlobalManager.music) {
                        musicSource.enabled = false;
                        GlobalManager.music = false;
                    } else {
                        musicSource.enabled = true;
                        GlobalManager.music = true;
                    }
                    break;
                case MenuTrigger.MenuButton.CalibrateMic:
                    StartCoroutine (GlobalManager.CalibrateNoise ());
                    break;
                case MenuTrigger.MenuButton.MicInput:
                    if (GlobalManager.micInput) {
                        micInputRenderer.sprite = touchInputSpr;
                        GlobalManager.micInput = false;
                    } else {
                        micInputRenderer.sprite = micInputSpr;
                        GlobalManager.micInput = true;
                    }
                    break;
            }
        }
    }

    

}
