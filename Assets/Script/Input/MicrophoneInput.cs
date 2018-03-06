using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MicrophoneInput : MonoBehaviour {

    private AudioSource audioSource;
    public string audioDevice;
    public TextMesh worldText;

    private void Awake() {
        audioSource = GetComponent<AudioSource> ();
        if (!GlobalManager.micInput) {
            gameObject.SetActive (false);
        }
    }

    private void Start() {
        if (UnityEngine.Microphone.devices != null) {
            PrintAudioDevices ();
            audioDevice = UnityEngine.Microphone.devices[0];

            audioSource.clip = UnityEngine.Microphone.Start (audioDevice, true, 10, 44100);
            audioSource.loop = true;

            while (!(UnityEngine.Microphone.GetPosition (audioDevice) > 0)) { }
            audioSource.Play ();

        } else {
            Debug.LogError ("Error: No audio devices for audio input.");
            Debug.Break ();
        }
    }

    private void PrintAudioDevices() {
        string output = "Audio Devices: ";
        for (int i=0; i<UnityEngine.Microphone.devices.Length; i++) {
            output += UnityEngine.Microphone.devices[i] + ", ";
        }
        output += "\n";
        Debug.Log (output);
        if (worldText != null)
            worldText.text = output;
    }

}
