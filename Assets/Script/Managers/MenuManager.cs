using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour {

    public MenuTrigger playTrigger, exitTrigger, musicTrigger;

    public AudioSource musicSource;

    public GameObject musicCross;

    public static MenuManager main;

    private void Awake() {
        main = this;

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
                        musicCross.SetActive (true);
                    } else {
                        musicSource.enabled = true;
                        GlobalManager.music = true;
                        musicCross.SetActive (false);
                    }
                    break;
            }
        }
    }

}
