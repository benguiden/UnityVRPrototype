using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioToControl : MonoBehaviour {

    private AudioSource audioSource;
    public Transform[] points;
    public float k = 200f;
    public LineRenderer lineRendererClean;
    public float lowBangFreq, highBangFreq;
    public Transform bangIndicator;
    public float bangShootVolume;
    [Range(0f, 0.1f)]
    public float bangSoftener;

    private float[] samples = new float[1024];
    private float bangVolume;

    void Awake() {
        audioSource = GetComponent<AudioSource> ();
        lineRendererClean.positionCount = 128;
    }

    void Update() {
        GetSpectrum ();

        bangVolume = GetBangVolume ();

        if (lineRendererClean != null)
            DrawCleanLine (lineRendererClean);

        Vector3 newBangPos = bangIndicator.transform.position;
        newBangPos.y = bangVolume * k;
        bangIndicator.transform.position = newBangPos;

        if (bangVolume > bangShootVolume / k) {
            bangIndicator.GetComponent<SpriteRenderer> ().color = Color.red;
        } else {
            bangIndicator.GetComponent<SpriteRenderer> ().color = Color.white;
        }
    }

    private void GetSpectrum() {
        //24000
        audioSource.GetSpectrumData (samples, 0, FFTWindow.Rectangular);
    }

    private float GetBangVolume() {
        float freqStep = 24000f / (float)samples.Length;
        int lowIndex = Mathf.RoundToInt (lowBangFreq / freqStep);
        int highIndex = Mathf.RoundToInt (highBangFreq / freqStep);
        if (highIndex < lowIndex) {
            Debug.LogError ("Error high freq lower than low freq. High: " + highIndex.ToString () + " Low: " + lowIndex.ToString ());
        } else {
            if (highIndex >= samples.Length)
                highIndex = samples.Length - 1;

            float sum = 0f;
            int freqCount = 0;
            for (int i=lowIndex; i<=highIndex; i++) {
                sum += samples[i];
                if (samples[i] > bangSoftener)
                    freqCount++;
            }
            if (freqCount > 0)
                sum /= (float)freqCount;
            return sum;
        }
        return 0f;
    }

    private void DrawCleanLine(LineRenderer lineRenderer) {
        for (int i = 0; i < lineRenderer.positionCount; i++) {
            lineRenderer.SetPosition (i, new Vector3 (i / 4f, k * samples[i * 2], 0f));
        }
    }

}
