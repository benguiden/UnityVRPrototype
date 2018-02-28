using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AppController : MonoBehaviour
{
    public static AppController instance = null;              //Static instance which allows it to be accessed by any other scripts.


    // initialization
    void Start ()
    {
        // Singleton initialisation
        if (instance == null)
        {
            //Sets this to not be destroyed when reloading scene
            DontDestroyOnLoad(gameObject);

            instance = this;
        }
        else if (instance != this)
        {
            //Then destroy this. there can only ever be one instance 
            Destroy(gameObject);
        }

        // load menu scene
        SceneManager.LoadScene("Menu");
    }
	
	
}
