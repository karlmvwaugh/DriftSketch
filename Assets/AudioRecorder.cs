using UnityEngine;
using System;
using System.Collections;

public class AudioRecorder : MonoBehaviour {
	private AudioClip audioClip;
	private bool _recording;
	public GameObject audioGameObject;
	public MicSelector micSelector;
	private AudioSource currentAudioSource; 
	private AudioDecay currentAudioDecay;
	private bool playWhilstRecording = false;
	private DateTime batchTime;
	private string micName;
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	}

	public bool StartRecording (DateTime _batchTime)
	{	
		if (_recording) return false;
		_recording = true;
		
		micName = micSelector.GetMicName();
		
		batchTime = _batchTime;
		
		CreateNewPlayer();
		try {
			audioClip = Microphone.Start(micName, false, 60, 44100);
		}
		catch (Exception e) {
			_recording = false;
			return false; 
		}
		
		if (audioClip != null && playWhilstRecording){
			currentAudioSource.clip = audioClip;
			currentAudioSource.volume = 1f;
			currentAudioSource.pitch = 1f;
			currentAudioSource.Play();
			
		}
		return true; 
	}


	public AudioDecay StopRecording (float fadeTime)
	{
		if (_recording == false){
			return null;
		}

		if ((! (Microphone.GetPosition(micName) > 0)) || (! Microphone.IsRecording(micName)))
		{
			_recording = false;
			return null;
		}

		var position = Microphone.GetPosition(micName);
		while(Microphone.GetPosition(micName) == position){}
		Microphone.End (micName); 
		while(Microphone.IsRecording(micName)){}

		audioClip = TrimAndAdjust(audioClip, position); 

		if (audioClip != null){
			currentAudioSource.clip = audioClip;
			currentAudioSource.volume = 1f;
			currentAudioSource.pitch = 1f;
			currentAudioSource.Play();
			currentAudioDecay.Init(fadeTime, batchTime);

		}
		_recording = false;

		return currentAudioDecay;
	}

	private AudioClip TrimAndAdjust(AudioClip recordedClip, int position){
		
		var soundData = new float[recordedClip.samples * recordedClip.channels];
		recordedClip.GetData (soundData, 0);
		
		var recordedLength = position; 
		//Create shortened array for the data that was used for recording
		var newData = new float[recordedLength * recordedClip.channels];
		
		//Copy the used samples to a new array
		for (int i = 0; i < newData.Length; i++) {
			newData[i] = soundData[i];
		}
		
		//One does not simply shorten an AudioClip,
		//    so we make a new one with the appropriate length
		var newClip = AudioClip.Create (recordedClip.name,
		                                recordedLength,
		                                recordedClip.channels,
		                                recordedClip.frequency,
		                                false,
		                                false);
		
		newClip.SetData (newData, 0);        //Give it the data from the old clip
		
		//Replace the old clip
		AudioClip.Destroy (recordedClip);
		return newClip; 
	}

	private void CreateNewPlayer(){
		var go = (GameObject)Instantiate(audioGameObject);
		currentAudioSource = go.GetComponent<AudioSource>();
		currentAudioDecay = go.GetComponent<AudioDecay>();
	}
}
