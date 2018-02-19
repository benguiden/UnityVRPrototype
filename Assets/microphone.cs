using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class microphone : MonoBehaviour {

	void Start() {
		AudioSource aud = GetComponent<AudioSource>();
		Debug.Log (Microphone.devices [0]);
		aud.clip = Microphone.Start(Microphone.devices[0], true, 10, 44100);
		aud.Play();
	}

}
