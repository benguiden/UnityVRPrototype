using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioToControl : MonoBehaviour {

    #region Public Variables
    [Header("Audio Input")]
    public float k = 200f;
    public FFTWindow fftWindow;
    public bool normalizeSpectrum;
    public float normlaizedAmount = 4f;
    public float updateRate = 30f;

    [Header("Calibration")]
    public float calibrateNoiseTime = 3f;

    [Header("Shooting")]
    public float bangShootThreshold;
    public float lowBangFreq, highBangFreq;
    public ShootingControl shootingControl;

    [Header("Debugging")]
    public LineRenderer lineRendererClean;
    public Transform bangIndicator;
    public TextMesh worldText;
    #endregion

    #region Private Variables
    private AudioSource audioSource;

    private float[] inputSpectrum = new float[1024];
    private float[] noiseSpectrum = new float[1024];

    private float bangVolume;
	private bool isShooting = false;
    #endregion

    #region Mono Methods
    void Awake() {
        audioSource = GetComponent<AudioSource> ();
        if (lineRendererClean)
            lineRendererClean.positionCount = 128;
    }

    void Start() {
        StartCoroutine(CalibrateNoise());
    }
    #endregion

    #region Audio Methods
    private void GetSpectrum() {
        float[] newSpectrum = new float[inputSpectrum.Length];
        //24000
        audioSource.GetSpectrumData (newSpectrum, 0, fftWindow);

        float normalisedPoint = 1f;
        
        for (int i=0; i<256; i++) {
            if (normalizeSpectrum)
                normalisedPoint = Mathf.Clamp(1f + ((float)i / 80f), 1f, 2f) * normlaizedAmount;
            inputSpectrum[i] = Mathf.Lerp(inputSpectrum[i], newSpectrum[i] * normalisedPoint, 1f);
        }
    }

    private IEnumerator UpdateSpectrum() {
        yield return null;
        while (this.enabled) {
            GetSpectrum();

            bangVolume = GetBangVolume();

            if (worldText != null)
                worldText.text = (Mathf.Round(bangVolume * 10000) / 100f).ToString();

            if (lineRendererClean != null)
                DrawCleanLine(lineRendererClean, true);

            if (bangIndicator != null) {
                Vector3 newBangPos = bangIndicator.transform.position;
                newBangPos.y = bangVolume * k;
                bangIndicator.transform.position = newBangPos;
            }

            if (bangVolume >= bangShootThreshold / k) {
                if (!isShooting)
                    shootingControl.Shoot();
                isShooting = true;
            } else {
                isShooting = false;
            }
            yield return new WaitForSeconds(1f / updateRate);
        }
    }

    private IEnumerator CalibrateNoise() {
        Debug.LogWarning("Calibrating Noise...");
        float time = calibrateNoiseTime;
        int ticks = 0;
        while (time > 0f) {
            GetSpectrum();

            if (lineRendererClean != null)
                DrawCleanLine(lineRendererClean, false);

            for (int i=0; i<inputSpectrum.Length; i++) {
                noiseSpectrum[i] += inputSpectrum[i];
            }
            ticks++;

            time -= 1f / updateRate;
            yield return new WaitForSeconds(1f / updateRate);
        }
        for (int i=0; i<noiseSpectrum.Length; i++) {
            noiseSpectrum[i] /= (float)ticks;
        }
        Debug.LogWarning("Noise Calibrated");
        StartCoroutine(UpdateSpectrum());
    }

    private float GetBangVolume() {
        float freqStep = 24000f / (float)inputSpectrum.Length;
        int lowIndex = Mathf.RoundToInt (lowBangFreq / freqStep);
        int highIndex = Mathf.RoundToInt (highBangFreq / freqStep);
        if (highIndex < lowIndex) {
            Debug.LogError ("Error high freq lower than low freq. High: " + highIndex.ToString () + " Low: " + lowIndex.ToString ());
        } else {
            if (highIndex >= inputSpectrum.Length)
                highIndex = inputSpectrum.Length - 1;

            float sum = 0f;
            float indexedVolume = 0f;
            for (int i = lowIndex; i <= highIndex; i++) {
                indexedVolume = inputSpectrum[i] - noiseSpectrum[i];
                if (indexedVolume > sum)
                    sum = indexedVolume;
            }

            return sum;
        }
        return 0f;
    }
    #endregion

    #region Debug Methods
    private void DrawCleanLine(LineRenderer lineRenderer, bool includeNoise) {
        if (includeNoise) {
            for (int i = 0; i < lineRenderer.positionCount; i++) {
                float y = k * (inputSpectrum[i * 2] - noiseSpectrum[i * 2]);
                if (y < 0f)
                    y = 0f;
                lineRenderer.SetPosition(i, new Vector3(i / 4f, y, 0f));
            }
        } else {
            for (int i = 0; i < lineRenderer.positionCount; i++) {
                float y = k * (inputSpectrum[i * 2]);
                if (y < 0f)
                    y = 0f;
                lineRenderer.SetPosition(i, new Vector3(i / 4f, y, 0f));
            }
        }
    }
    #endregion

}
