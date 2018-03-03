using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretAudio : MonoBehaviour {

    #region Public Variables
    public float k = 10f;
    public float volumeCap = 1f;
    [Range (float.Epsilon, 1f)]
    public float changeSmoothness, volumeSmoothness, pitchSmoothness = 0.75f;
    public AnimationCurve volumeCurve, pitchCurve;
    #endregion

    #region Private Variables
    private AudioSource audioSource;
    private Vector3 lastAngle;
    private float angularSpeed;
    #endregion

    #region Mono Methods
    private void Awake() {
        audioSource = GetComponent<AudioSource> ();

        audioSource.volume = volumeCurve.Evaluate (0f) * volumeCap;
        audioSource.pitch = pitchCurve.Evaluate (0f);
    }

    private void Update() {
        Vector3 angularVelocity = transform.localEulerAngles - lastAngle;
        lastAngle = transform.localEulerAngles;
        float newSpeed = Mathf.Clamp01 (angularVelocity.magnitude * k * Time.deltaTime);
        angularSpeed = Mathf.Lerp (angularSpeed, newSpeed, /*Time.deltaTime * 60f * */(1f - changeSmoothness));

        float newVolume = volumeCurve.Evaluate (angularSpeed) * volumeCap;
        float newPitch = pitchCurve.Evaluate (angularSpeed);
        audioSource.volume = Mathf.Lerp (audioSource.volume, newVolume, Time.deltaTime * 60f * (1f - volumeSmoothness));
        audioSource.pitch = Mathf.Lerp (audioSource.pitch, newPitch, Time.deltaTime * 60f * (1f - pitchSmoothness));
    }
    #endregion

}
