using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UI : MonoBehaviour {

	// Initialize Variable
	public GameObject gameLoseUI;
	public GameObject gameWinUI;
	bool gameIsOver;

	// Start Function 
	void Start () {
		// Subscribing method to OnGuardHasSpottedPlayer delegate
		Guard_Movement.OnGuardHasSpottedPlayer += ShowGameLoseUI;
		// Subscribing method to OnReachedEndOfLevel delegate
		FindObjectOfType<Player_Movement> ().OnReachedEndOfLevel += ShowGameWinUI;
	}
	
	// Function for Update
	void Update () {
		if (gameIsOver) {
			if (Input.GetKeyDown(KeyCode.Space)) {
				SceneManager.LoadScene (0);
			}
		}
	}

	// Function for WinGameUI
	void ShowGameWinUI() {
		//Calling GameOver Function
		OnGameOver (gameWinUI);
	}

	// Function for LoseGameUI
	void ShowGameLoseUI() {
		//Calling GameOver Function
		OnGameOver (gameLoseUI);
	}

	// Function for GameOver
	void OnGameOver(GameObject gameOverUI) {
		gameOverUI.SetActive (true); // Activating GameUI
		gameIsOver = true; // GameisOver variable setting to true 
		Guard_Movement.OnGuardHasSpottedPlayer -= ShowGameLoseUI; // Unsubscribing method to OnGuardHasSpottedPlayer delegate
		FindObjectOfType<Player_Movement>().OnReachedEndOfLevel -= ShowGameWinUI; // Unsubscribing method to OnReachedEndOfLevel delegate
	}
}

