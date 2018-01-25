using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class Drawer : BaseControl {
	public GameObject point; 
	public Scatterer scatterer; 
	public AudioRecorder recorder;

	private bool previousClick;

	private int shares = 5;

	private CommonTimer commonTimer = new CommonTimer();
	private List<Floating> currentBatch = new List<Floating>();
	private DateTime currentBatchTime;

	private bool currentlyBeingTouched = false;

	private bool currentlyRecording = false;
	private AudioDecay currentAudioSource = null;
	private DateTime startedRecordingTime;

	private ValueMemory valueMemory;

	// Use this for initialization
	void Start () {
		Screen.sleepTimeout = SleepTimeout.NeverSleep;
		valueMemory = new ValueMemory(shares); 
		valueMemory.InputValue("xDrift", scatterer.getX);
		valueMemory.InputValue("yDrift", scatterer.getY);
		valueMemory.InputValue("death", scatterer.getDeathTime);
		valueMemory.InputValue("timeDiff", () => (float)(DateTime.Now - currentBatchTime).TotalMilliseconds);
		valueMemory.InputValue("x", () => lastX);
		valueMemory.InputValue("y", () => lastY);
	}
	
	// Update is called once per frame
	void Update () {
		if (IsTouch()){
			RecordingOn();
			valueMemory.SetCurrent();
			DealWithPrevious();

			SpawnPoint(lastX, lastY, valueMemory.GetCurrent("xDrift"), valueMemory.GetCurrent("yDrift"), valueMemory.GetCurrent("death"), valueMemory.GetCurrent("timeDiff"));

			PrepareForNext();

			DoWeNeedToStopRecording();
		} else {
			RecordingOff();
			previousClick = false; 
		}
	}

	void RecordingOn(){
		if (! currentlyBeingTouched){
			currentlyBeingTouched = true;
			currentBatchTime = DateTime.Now; 

			if (! currentlyRecording){
				var success = recorder.StartRecording(currentBatchTime);
				if (success) {
					currentlyRecording = true;
					startedRecordingTime = DateTime.Now;
				}
			}
		}
	}

	void RecordingOff(){
		if (currentlyBeingTouched){
			currentlyBeingTouched = false;

			if (currentlyRecording){
				currentAudioSource = recorder.StopRecording(scatterer.getDeathTime());
				currentlyRecording = false;
			}

			commonTimer.AddBatch(currentBatch, currentAudioSource);
			currentBatch = new List<Floating>(); 
		}
	} 

	void DoWeNeedToStopRecording(){
		if (currentlyRecording){
			var now = DateTime.Now;
			if ( (now - startedRecordingTime).TotalSeconds > 50){
				currentAudioSource = recorder.StopRecording(scatterer.getDeathTime());
				currentlyRecording = false;
			}
		}
	}

	void DealWithPrevious(){
		if (previousClick){
			for (var i =1; i<shares; i++){
				SpawnMidPoint(i);
			}
		}
	}

	void SpawnMidPoint(int i){
		var shareX = valueMemory.GetShare("x", i);
		var shareY = valueMemory.GetShare("y", i);
		var shareXDrift = valueMemory.GetShare("xDrift", i);
		var shareYDrift = valueMemory.GetShare("yDrift", i);
		var shareDeathTime = valueMemory.GetShare("death", i);
		var shareTimeDiff = valueMemory.GetShare("timeDiff", i);
		
		SpawnPoint(shareX, shareY, shareXDrift, shareYDrift, shareDeathTime, shareTimeDiff);
	}
	
	
	void PrepareForNext(){
		previousClick = true; 
		valueMemory.RememberValues();
	}

	void SpawnPoint(float x, float y, float xDrift, float yDrift, float deathTime, float timeDiff){
		var go = (GameObject)Instantiate(point);
		var newCoord = Camera.main.ScreenToWorldPoint(new Vector3(x, y));
		go.transform.position = new Vector3(newCoord.x, newCoord.y, -1f); 
		var floating = go.GetComponent<Floating>();
		floating.Init(xDrift, yDrift, deathTime, timeDiff); 

		currentBatch.Add(floating);
	}
}


public class BaseControl : MonoBehaviour{
	protected float lastX;
	protected float lastY;
	private bool mouseOn = false;
	private bool touchScreen;
	
	public 	bool IsTouch(){
		lastX = -1f;
		lastY = -1f; 

		var isMouse = isMouseTouch();
		if (isMouse) return isMouse; 

		return isPhoneTouch(); 
	}
	
	private bool isMouseTouch(){		
		if (Input.GetMouseButton(0)){
			lastX = Input.mousePosition.x;
			lastY = Input.mousePosition.y; 
			return true;
		}
		
		return false; 
	}
	
	private bool isPhoneTouch(){
		if (Input.touchCount > 0) {
			Touch tou = Input.GetTouch(0);
			lastX = tou.position.x;
			lastY = tou.position.y; 
			return true;
		}

		return false;
	}
	
	void Start(){}
	
	void Update(){}
}

public class ValueMemory{
	private Dictionary<string, Func<float>> funcs;
	private Dictionary<string, float> previousValues;
	private Dictionary<string, float> currentValues;
	private int _shares;

	public ValueMemory(int shares){
		_shares = shares;
		funcs = new Dictionary<string, Func<float>>();
		previousValues = new Dictionary<string, float>();
		currentValues = new Dictionary<string, float>();
	}
	public void InputValue(string name, Func<float> Func){
		funcs[name] = Func;
	}

	public void SetCurrent(){
		foreach(var kvp in funcs){
			currentValues[kvp.Key] = kvp.Value();
		}
	}

	public void RememberValues(){
		foreach(var kvp in currentValues){
			previousValues[kvp.Key] = kvp.Value;
		}
	}

	public float GetShare(string name, int step){
		var first = previousValues[name];
		var second = currentValues[name];
		return first + step*(second - first)/_shares;
	}

	public float GetCurrent(string name){
		return currentValues[name];
	}
}

public class CommonTimer {
	private int countOfConstantMembers = 5;
	private List<ActivePair> everythingActive = new List<ActivePair>();
	private Queue<ActivePair> everythingQ = new Queue<ActivePair>();

	public void AddBatch(List<Floating> incomingList, AudioDecay source){
		everythingQ.Enqueue(new ActivePair(){
			activePoints = incomingList,
			activeAudio = source
		});

		if (everythingQ.Count > countOfConstantMembers){
			trigger(everythingQ.Dequeue());
		}
	}

	private void trigger(ActivePair current){
		if (current == null) return;

		if (current.activePoints != null){
			foreach(var floating in current.activePoints){
				floating.StartCountDown();
			}
		}
		
		if (current.activeAudio != null){
			current.activeAudio.StartCountDown();
		}
	}

	public class ActivePair {
		public List<Floating> activePoints;
		public AudioDecay activeAudio;
	}
}