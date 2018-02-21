using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectDisposer : MonoBehaviour {

    public static ObjectDisposer main;

    public uint disposeSpeed = 5;
    private List<Object> trash;
    public bool printBin = false;

    private void Awake() {
        main = this;
        trash = new List<Object> ();
    }

    private void Start() {
        StartCoroutine (IDisposeOverTime ());
    }

    private void Update() {
        if ((printBin) && (trash.Count > 0)) {
            string bin = "";
            for (int i=0; i<trash.Count; i++) {
                bin += i.ToString () + ": " + trash[i].ToString () + ",\n";
            }
            Debug.LogWarning (bin);
        } else if (printBin) {
            Debug.LogWarning ("Trash bin empty");
        }
        printBin = false;
    }

    private IEnumerator IDisposeOverTime() {
        while (true) {
            if (trash.Count > 0) {
                Object trashToDispose = trash[0];
                trash.RemoveAt (0);
                Destroy (trashToDispose);
                yield return new WaitForSeconds (1f / (float)disposeSpeed);
            } else {
                yield return null;
            }
        }
    }

    public void DisposeOf(Object objectToDispose) {
        trash.Add (objectToDispose);
    }

}
