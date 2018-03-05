using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalManager {

    public static bool music = true;
    public static float[] micNoise = new float[1024];
    public static float[] inputSpectrum = new float[1024];

    public static IEnumerator CalibrateNoise() {
        MenuManager.main.enabled = false;
        MenuManager.main.musicSource.volume = 0f;
        Debug.LogWarning ("Calibrating Noise...");
        float calibrateNoiseTime = 4f;
        float updateRate = 30f;

        float time = calibrateNoiseTime;
        int ticks = 0;
        while (time > 0f) {
            GetSpectrum ();

            for (int i = 0; i < inputSpectrum.Length; i++) {
                micNoise[i] += inputSpectrum[i];
            }
            ticks++;

            time -= 1f / updateRate;

            MenuManager.main.UpdateMicLine (time / calibrateNoiseTime);

            yield return new WaitForSeconds (1f / updateRate);
        }
        MenuManager.main.UpdateMicLine (0f);
        for (int i = 0; i < micNoise.Length; i++) {
            micNoise[i] /= (float)ticks;
        }
        Debug.LogWarning ("Noise Calibrated");
        MenuManager.main.enabled = true;
        MenuManager.main.musicSource.volume = 0.7f;
    }

    private static void GetSpectrum() {
        bool normalizeSpectrum = true;
        float normlaizedAmount = 10f;

        float[] newSpectrum = new float[inputSpectrum.Length];
        //24000
        MenuManager.main.micInputSource.GetSpectrumData (newSpectrum, 0, FFTWindow.Triangle);

        float normalisedPoint = 1f;

        for (int i = 0; i < 256; i++) {
            if (normalizeSpectrum)
                normalisedPoint = Mathf.Clamp (1f + ((float)i / 80f), 1f, 2f) * normlaizedAmount;
            inputSpectrum[i] = Mathf.Lerp (inputSpectrum[i], newSpectrum[i] * normalisedPoint, 1f);
        }
    }

}
