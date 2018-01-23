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
	private float previousXDrift;
	private float previousYDrift;
	private float previousDeathTime;
	private float currentXDrift;
	private float currentYDrift;
	private float currentDeathTime;

	private int shares = 5;

	private CommonTimer commonTimer = new CommonTimer();
	private List<Floating> currentBatch = new List<Floating>();
	private DateTime currentBatchTime;

	private bool currentlyBeingTouched = false;

	private DateTime currentFrameTime;
	private DateTime previousFrameTime;


	private bool currentlyRecording = false;
	private AudioDecay currentAudioSource = null;
	private DateTime startedRecordingTime;

	// Use this for initialization
	void Start () {
		Screen.sleepTimeout = SleepTimeout.NeverSleep;

	}
	
	// Update is called once per frame
	void Update () {
		if (IsTouch()){
			RecordingOn();
			currentXDrift = scatterer.getX();
			currentYDrift = scatterer.getY();
			currentDeathTime = scatterer.getDeathTime();
			currentFrameTime = DateTime.Now;
			DealWithPrevious();

			var currentTimeDiff = (float)(currentFrameTime - currentBatchTime).TotalMilliseconds;
			SpawnPoint(lastX, lastY, currentXDrift, currentYDrift, currentDeathTime, currentTimeDiff);

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
				recorder.StartRecording(currentBatchTime);
				currentlyRecording = true;
				startedRecordingTime = DateTime.Now;
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
				var shareX = getShare (previousX, lastX, i);
				var shareY = getShare (previousY, lastY, i);
				var shareXDrift = getShare(previousXDrift, currentXDrift, i);
				var shareYDrift = getShare(previousYDrift, currentYDrift, i);
				var shareDeathTime = getShare(previousDeathTime, currentDeathTime, i);

				var previousTimeDiff = (float)(previousFrameTime - currentBatchTime).TotalMilliseconds;
				var currentTimeDiff = (float)(currentFrameTime - currentBatchTime).TotalMilliseconds;
				var shareTimeDiff = getShare(previousTimeDiff, currentTimeDiff, i);

				SpawnPoint(shareX, shareY, shareXDrift, shareYDrift, shareDeathTime, shareTimeDiff);
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
		previousXDrift = currentXDrift;
		previousYDrift = currentYDrift;
		previousDeathTime = currentDeathTime;
		previousFrameTime = currentFrameTime;
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



public class CommonTimer {

	private int countOfConstantMembers = 5;
	private List<ActivePair> everythingActive = new List<ActivePair>();

	private Queue<ActivePair> everythingQ = new Queue<ActivePair>();

	public void AddBatch(List<Floating> incomingList, AudioDecay source){
		/*everythingActive.Add(new ActivePair(){
			activePoints = incomingList,
			activeAudio = source
		}  );

if (everythingActive.Count > countOfConstantMembers){
			popAndTrigger();
		}
		 */

		everythingQ.Enqueue(new ActivePair(){
			activePoints = incomingList,
			activeAudio = source
		});

		if (everythingQ.Count > countOfConstantMembers){
			trigger(everythingQ.Dequeue());
		}

	}

	/*private void popAndTrigger(){
		var current = everythingActive.First();

		trigger(current);


		everythingActive.RemoveAt(0);
	}*/

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