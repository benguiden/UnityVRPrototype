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
    public TextMesh worldText;
    public bool normalizeSpectrum;
    public float normlaizedAmount = 4f;
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

        if (worldText != null)
            worldText.text = (Mathf.Round(bangVolume * 10000) / 100f).ToString();

        if (lineRendererClean != null)
            DrawCleanLine (lineRendererClean);

		if (bangIndicator != null) {
			Vector3 newBangPos = bangIndicator.transform.position;
			newBangPos.y = bangVolume * k;
			bangIndicator.transform.position = newBangPos;

			/*if (bangVolume > bangShootVolume / k) {
				bangIndicator.GetComponent<SpriteRenderer> ().color = Color.red;
			} else {
				bangIndicator.GetComponent<SpriteRenderer> ().color = Color.white;
			}*/
            
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
        float[] newSamples = new float[samples.Length];
        //24000
        audioSource.GetSpectrumData (newSamples, 0, FFTWindow.Rectangular);

        float normalisedPoint = 1f;
        
        for (int i=0; i<256; i++) {
            if (normalizeSpectrum)
                normalisedPoint = Mathf.Clamp(1f + ((float)i / 80f), 1f, 2f) * normlaizedAmount;
            samples[i] = Mathf.Lerp(samples[i], newSamples[i] * normalisedPoint, 1f);
        }
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
            /*int freqCount = 0;
            for (int i=lowIndex; i<=highIndex; i++) {
                sum += samples[i];
                if (samples[i] >= bangSoftener)
                    freqCount++;
            }
            if (freqCount > 0)
                sum /= (float)freqCount;*/

            for (int i = lowIndex; i <= highIndex; i++) {
                if ((samples[i] >= bangSoftener) && (samples[i] > sum))
                    sum = samples[i];
            }

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
