using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioToControl : MonoBehaviour {

    #region Public Variables
    public Transform[] points;
    public float k = 200f;
    public LineRenderer lineRendererClean;
    public float lowBangFreq, highBangFreq;
    public Transform bangIndicator;
    public float bangShootVolume;
    [Range(0f, 0.1f)]
    public float bangSoftener;
	public ShootingControl shootingControl;
    #endregion

    #region Private Variables
    private AudioSource audioSource;
    private float[] samples = new float[1024];
    private float bangVolume;
	private bool isShooting = false;
    #endregion

    #region Mono Methods
    void Awake() {
        audioSource = GetComponent<AudioSource> ();
        if (lineRendererClean)
            lineRendererClean.positionCount = 128;
    }

    void Update() {
        GetSpectrum ();

        bangVolume = GetBangVolume ();

        if (lineRendererClean != null)
            DrawCleanLine (lineRendererClean);

		if (bangIndicator != null) {
			Vector3 newBangPos = bangIndicator.transform.position;
			newBangPos.y = bangVolume * k;
			bangIndicator.transform.position = newBangPos;

			if (bangVolume > bangShootVolume / k) {
				bangIndicator.GetComponent<SpriteRenderer> ().color = Color.red;
			} else {
				bangIndicator.GetComponent<SpriteRenderer> ().color = Color.white;
			}
		}
			
		if (bangVolume >= bangShootVolume / k) {
			if (!isShooting)
				shootingControl.Shoot ();
			isShooting = true;
		} else {
			isShooting = false;
		}

    }
    #endregion

    #region Audio Methods
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
                if (samples[i] >= bangSoftener)
                    freqCount++;
            }
            if (freqCount > 0)
                sum /= (float)freqCount;
            return sum;
        }
        return 0f;
    }
    #endregion

    #region Debug Methods
    private void DrawCleanLine(LineRenderer lineRenderer) {
        for (int i = 0; i < lineRenderer.positionCount; i++) {
            lineRenderer.SetPosition (i, new Vector3 (i / 4f, k * samples[i * 2], 0f));
        }
    }
    #endregion

}
