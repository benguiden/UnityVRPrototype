using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public static GameManager main;

    public float shieldRadius = 10f;

    public int wave;

    private void Awake() {
        main = this;
    }

    public void RestartGame() {
        Debug.Log ("Restarting Game...");
        UnityEngine.SceneManagement.SceneManager.LoadScene (0);
    }

}
