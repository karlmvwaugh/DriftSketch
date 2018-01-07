using UnityEngine;
using System;
using System.Collections;

public class AudioDecay : MonoBehaviour {
	private AudioSource audioSource;
	private bool playing = false;
	private float timeToDecay;
	private float bottomFreq = 0.1f;

	private DateTime countDownTime;
	private bool countDownBegun = false;

	private Oscillator pitchOsc;

	// Use this for initialization
	void Start () {
		audioSource = GetComponent<AudioSource>();
	}

	// Update is called once per frame
	void Update () {
		if (playing){
			changePitch();
			changeVolumeAndKill();
		}
	}

	private void changePitch(){
		//var share = 1f - pitchOsc.GetValue();   //getShare();
		//var newFreq = 1.0f - share*(1.0f - bottomFreq);
		//audioSource.pitch = newFreq;
		audioSource.pitch = pitchOsc.GetValue();
	}

	private void changeVolumeAndKill(){

		if (countDownBegun){
			var share = getShare();
			var newVolume = 1.0f - share;
			audioSource.volume = newVolume;
			if (newVolume <= 0f){
				Destroy (this.gameObject); 
			}
		}


	}

	public void Init(float fadeTime, DateTime batchTime){
		var diff = (float)(DateTime.Now - batchTime).TotalMilliseconds;
		timeToDecay = fadeTime + diff; 

		pitchOsc = new Oscillator(){
			max = 1f,
			min = 0.1f,
			speed = 1000f/fadeTime //
		};

		pitchOsc.SetToStartAtTop();

		playing = true; 
	}

	public void StartCountDown(){
		countDownBegun = true;
		countDownTime = DateTime.Now;
	}

	private float getShare(){
		if (! countDownBegun) return -1f;

		var diff = (float)(DateTime.Now - countDownTime).TotalMilliseconds;
		return diff / timeToDecay;

	}
}
