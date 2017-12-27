using UnityEngine;
using System;
using System.Collections;

public class AudioDecay : MonoBehaviour {
	private AudioSource audioSource;
	private bool playing = false;
	private float timeToDecay;
	private float bottomFreq = 0.1f;
	private float fractionOfTimeBeforeFadeOut = 0.05f;

	private DateTime initDateTime;

	// Use this for initialization
	void Start () {
		audioSource = GetComponent<AudioSource>();
	}

	// Update is called once per frame
	void Update () {
		if (playing){
			var share = getShare();
			var newFreq = 1.0f - share*(1.0f - bottomFreq);
			audioSource.pitch = newFreq;

			if (share >= (1.0f - fractionOfTimeBeforeFadeOut)){
				var finalShare = (share - (1.0f - fractionOfTimeBeforeFadeOut))*(1.0f / fractionOfTimeBeforeFadeOut);
				var newVolume = 1.0f - finalShare;
				audioSource.volume = newVolume;
				if (newVolume <= 0f){
					Destroy (this.gameObject); 
				}
			}
		}
	}

	public void Init(float fadeTime){
		initDateTime = DateTime.Now;
		timeToDecay = fadeTime; 

		playing = true; 
	}

	private float getShare(){
		var diff = (float)(DateTime.Now - initDateTime).TotalMilliseconds;
		return diff / timeToDecay;

	}
}
