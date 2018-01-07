using UnityEngine;
using System;
using System.Collections;

public class AudioRecorder : MonoBehaviour {
	private AudioClip audioClip;
	private bool _recording;
	public GameObject audioGameObject;
	private AudioSource currentAudioSource; 
	private AudioDecay currentAudioDecay;
	private bool playWhilstRecording = false;
	private DateTime batchTime;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void StartRecording (DateTime _batchTime)
	{
		if (_recording) return;
		_recording = true;

		batchTime = _batchTime;

		CreateNewPlayer();

		audioClip = Microphone.Start("", false, 60, 44100);
			

		
		if (audioClip != null && playWhilstRecording){
			currentAudioSource.clip = audioClip;
			currentAudioSource.volume = 1f;
			currentAudioSource.pitch = 1f;
			currentAudioSource.Play();

		}
		
	}


	public AudioDecay StopRecording (float fadeTime)
	{
		if ((! (Microphone.GetPosition("") > 0)) || (! Microphone.IsRecording("")))
		{
			_recording = false;
			return null;
		}
		//ThisAudio.Stop();
		
		var position = Microphone.GetPosition("");
		while(Microphone.GetPosition("") == position){}
		Microphone.End (""); 
		while(Microphone.IsRecording("")){}

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
