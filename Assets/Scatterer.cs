using UnityEngine;
using System;
using System.Collections;

public class Scatterer : MonoBehaviour {
	private Oscillator x1;
	private Oscillator y1;
	private Oscillator death;
	// Use this for initialization
	void Start () {	
		x1 = new Oscillator(){
			max = 0.15f,
			min = -0.15f,
			speed = 0.083f
		};

		y1 = new Oscillator(){
			max = 0.15f,
			min = -0.15f,
			speed = 0.075f
		};

		death = new Oscillator(){
			max = 60000f,
			min = 30000f,
			speed = 0.0516f
		};


		/*death = new Oscillator(){
			max = 6000f,
			min = 3000f,
			speed = 0.0516f
		};*/
	}
	
	// Update is called once per frame
	void Update () {
	
	}


	public float getX(){
		return x1.GetValue() * 0.5f;
	}

	public float getY(){
		return y1.GetValue() * 0.5f;
	}

	public float getDeathTime(){
		return death.GetValue();
	}
}


public class Oscillator {
	public float max;
	public float min;
	public float speed;
	
	private float theta;
	private DateTime _lastTime;
	
	private float deltaMultiplier; // = 0.001f;
	// Use this for initialization
	public Oscillator () {
		theta = 0f;
		_lastTime = DateTime.Now;

		deltaMultiplier = 2f*(float)Math.PI / 1000f;
	}
	



	public float GetValue(){
		Update ();
		return (Mathf.Sin(theta)*(max - min) + max + min ) / 2f;
	}
	
	// Update is called once per frame
	void Update () {
		var delta = GetDelta();
		theta += (delta * speed * deltaMultiplier);
		
		_lastTime = DateTime.Now;
	}

	public void SetToStartAtTop() {
		theta = (float)Math.PI / 2;
	}


	float GetDelta(){
		var now = DateTime.Now;
		return (float)(now - _lastTime).TotalMilliseconds;
	}
}
