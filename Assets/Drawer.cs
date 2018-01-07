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
	private float previousX;
	private float previousY;
	private int shares = 5;

	private CommonTimer commonTimer = new CommonTimer();
	private List<Floating> currentBatch = new List<Floating>();
	private DateTime currentBatchTime;

	private bool currentlyBeingTouched = false;

	// Use this for initialization
	void Start () {
		Screen.sleepTimeout = SleepTimeout.NeverSleep;

	}
	
	// Update is called once per frame
	void Update () {
		if (IsTouch()){
			RecordingOn();
			DealWithPrevious();
			SpawnPoint(lastX, lastY);
			PrepareForNext();
		} else {
			RecordingOff();
			previousClick = false; 
		}
	}

	void RecordingOn(){
		if (! currentlyBeingTouched){
			currentlyBeingTouched = true;
			currentBatchTime = DateTime.Now; 

			recorder.StartRecording(currentBatchTime);
		}
	}

	void RecordingOff(){
		if (currentlyBeingTouched){
			currentlyBeingTouched = false;
			var audioSource = recorder.StopRecording(scatterer.getDeathTime());

			commonTimer.AddBatch(currentBatch, audioSource);
			currentBatch = new List<Floating>(); 
		}
	}


	void DealWithPrevious(){
		if (previousClick){
			for (var i =1; i<shares; i++){
				var shareX = getShare (previousX, lastX, i);
				var shareY = getShare (previousY, lastY, i);
				SpawnPoint(shareX, shareY);
			}

		}
	}

	float getShare(float first, float second, int step){
		return first + step*(second - first)/shares;
	}


	void PrepareForNext(){
		previousClick = true; 
		previousX = lastX;
		previousY = lastY;
	}

	void SpawnPoint(float x, float y){
		var go = (GameObject)Instantiate(point);
		var newCoord = Camera.main.ScreenToWorldPoint(new Vector3(x, y));
		go.transform.position = new Vector3(newCoord.x, newCoord.y, -1f); 
		var floating = go.GetComponent<Floating>();
		floating.Init(scatterer.getX(), scatterer.getY(), scatterer.getDeathTime(), currentBatchTime); 

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



public class CommonTimer {

	private int countOfConstantMembers = 5;
	private List<ActivePair> everythingActive = new List<ActivePair>();

	public void AddBatch(List<Floating> incomingList, AudioDecay source){
		everythingActive.Add(new ActivePair(){
			activePoints = incomingList,
			activeAudio = source
		}  );

		if (everythingActive.Count > countOfConstantMembers){
			popAndTrigger();
		}
	}

	private void popAndTrigger(){
		var current = everythingActive.First();

		foreach(var floating in current.activePoints){
			floating.StartCountDown();
		}

		current.activeAudio.StartCountDown();

		everythingActive.RemoveAt(0);
	}

	public class ActivePair {
		public List<Floating> activePoints;
		public AudioDecay activeAudio;
	}

}