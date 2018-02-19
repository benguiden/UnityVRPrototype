using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioToControl : MonoBehaviour {

    private AudioSource audioSource;
    public Transform[] points;
    public float k = 200f;

    void Awake() {
        audioSource = GetComponent<AudioSource> ();
    }

    void Update() {
        //24000
        float[] samples = new float[1024];
        audioSource.GetSpectrumData (samples, 0, FFTWindow.Rectangular);
        for (int i = 0; i < points.Length; i++) {
            points[i].localPosition = new Vector3 (points[i].localPosition.x, samples[i * 2] * k, 0f);
        }
    }

}
