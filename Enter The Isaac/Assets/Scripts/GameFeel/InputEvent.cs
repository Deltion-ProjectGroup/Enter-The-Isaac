using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class InputEvent : MonoBehaviour {
	
	[SerializeField] UnityEvent uEvent;
	[SerializeField] string input = "Roll_P1";
	[SerializeField] bool checkInUpdate = false;

	public void CheckInput () {
		if(Input.GetButtonDown(input) == true){
			uEvent.Invoke();
			//print(input);
		}
	}

	void Update(){
		if(checkInUpdate == true){
			CheckInput();
		}
	}
}
