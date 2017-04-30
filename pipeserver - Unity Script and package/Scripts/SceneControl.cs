using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneControl : MonoBehaviour {
	//This script use to pause and start the game 
	public bool IsGamePaused = false;

	public void ControlGame() {
		if (IsGamePaused == false) {
			IsGamePaused = true;
			Time.timeScale = 0;
			Debug.Log ("Pause Game");
		} else {
			IsGamePaused = false;
			Time.timeScale = 1;
			Debug.Log ("Start Game");
		}
	}
}
