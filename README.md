# AI-Path-Game 🚀

## Overview 🎮
AI-Path-Game is a Unity-based game where the player navigates through a guarded path while avoiding detection. The game includes AI path-following mechanics, player movement, and game-over conditions based on visibility.

🔗 Video Trailer

https://youtu.be/PFBZKoA1RIU


![AI Path](https://user-images.githubusercontent.com/62818241/204101177-d2371fe5-0d21-4ec6-a1e1-b87344feeca6.PNG)

## Player Movement 🎯
The **Player_Movement** script handles the player's movement, smooth rotations, and detects level completion.

### Key Features 🔑
- Smooth movement with acceleration and deceleration.
- Player rotation based on input direction.
- Triggers game-over on reaching the finish line.
- Listens for AI detection event.

```csharp
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Movement : MonoBehaviour {

    public event System.Action OnReachedEndOfLevel;
    public float moveSpeed = 7;
    public float smoothMoveTime = .1f;
    public float turnSpeed = 8;
    float angle;
    float smoothInputMagnitude;
    float smoothMoveVelocity;
    Vector3 velocity;
    new Rigidbody rigidbody;
    bool disabled;

    void Start() {
        rigidbody = GetComponent<Rigidbody>();
        Guard_Movement.OnGuardHasSpottedPlayer += Disable;
    }

    void Update () {
        Vector3 inputDirection = Vector3.zero;
        if (!disabled)
        {
            inputDirection = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;
        }
        float inputMagnitude = inputDirection.magnitude;
        smoothInputMagnitude = Mathf.SmoothDamp(smoothInputMagnitude, inputMagnitude, ref smoothMoveVelocity, smoothMoveTime);
        float targetAngle = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg;
        angle = Mathf.LerpAngle(angle, targetAngle, Time.deltaTime * turnSpeed * inputMagnitude);
        velocity = transform.forward * moveSpeed * smoothInputMagnitude;
    }

    void OnTriggerEnter(Collider hitCollider) {
        if (hitCollider.tag == "Finish") {
            Disable();
            if (OnReachedEndOfLevel != null) {
                OnReachedEndOfLevel();
            }
        }
    }

    void Disable() {
        disabled = true;
    }

    void FixedUpdate() {
        rigidbody.MoveRotation(Quaternion.Euler(Vector3.up * angle));
        rigidbody.MovePosition(rigidbody.position + velocity * Time.deltaTime);
    }

    void OnDestroy() {
        Guard_Movement.OnGuardHasSpottedPlayer -= Disable;
    }
}
```

## UI Management 📺
The **UI** script manages the game-over UI, detects when the game ends, and allows restarting the level.

### Key Features 🔑
- Displays win/lose screens.
- Detects game-over conditions.
- Reloads scene on spacebar press.

```csharp
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UI : MonoBehaviour {

    public GameObject gameLoseUI;
    public GameObject gameWinUI;
    bool gameIsOver;

    void Start () {
        Guard_Movement.OnGuardHasSpottedPlayer += ShowGameLoseUI;
        FindObjectOfType<Player_Movement>().OnReachedEndOfLevel += ShowGameWinUI;
    }
    
    void Update () {
        if (gameIsOver) {
            if (Input.GetKeyDown(KeyCode.Space)) {
                SceneManager.LoadScene(0);
            }
        }
    }

    void ShowGameWinUI() {
        OnGameOver(gameWinUI);
    }

    void ShowGameLoseUI() {
        OnGameOver(gameLoseUI);
    }

    void OnGameOver(GameObject gameOverUI) {
        gameOverUI.SetActive(true);
        gameIsOver = true;
        Guard_Movement.OnGuardHasSpottedPlayer -= ShowGameLoseUI;
        FindObjectOfType<Player_Movement>().OnReachedEndOfLevel -= ShowGameWinUI;
    }
}
```

## Guard AI 🕵️‍♂️
The **Guard_Movement** script controls the AI guard's movement and detection system.

### Key Features 🔑
- Moves along predefined waypoints.
- Uses a spotlight to detect the player.
- Triggers game-over when spotting the player.

```csharp
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Guard_Movement : MonoBehaviour {

    public static event System.Action OnGuardHasSpottedPlayer;
    public float speed = 5;
    public float waitTime = .3f;
    public float turnSpeed = 90;
    public float timeToSpotPlayer = .5f;
    public Light spotlight;
    public float viewDistance;
    public LayerMask viewMask;
    float viewAngle;
    float playerVisibleTimer;
    public Transform pathHolder;
    Transform player;
    Color originalSpotlightColour;

    void Start() {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        viewAngle = spotlight.spotAngle;
        originalSpotlightColour = spotlight.color;
        Vector3[] waypoints = new Vector3[pathHolder.childCount];
        for (int i = 0; i < waypoints.Length; i++) {
            waypoints[i] = pathHolder.GetChild(i).position;
            waypoints[i] = new Vector3(waypoints[i].x, transform.position.y, waypoints[i].z);
        }
        StartCoroutine(FollowPath(waypoints));
    }

    void Update() {
        if (CanSeePlayer()) {
            playerVisibleTimer += Time.deltaTime;
        } else {
            playerVisibleTimer -= Time.deltaTime;
        }
        playerVisibleTimer = Mathf.Clamp(playerVisibleTimer, 0, timeToSpotPlayer);
        spotlight.color = Color.Lerp(originalSpotlightColour, Color.red, playerVisibleTimer / timeToSpotPlayer);

        if (playerVisibleTimer >= timeToSpotPlayer) {
            if (OnGuardHasSpottedPlayer != null) {
                OnGuardHasSpottedPlayer();
            }
        }
    }
}
```

## Conclusion 🎯
AI-Path-Game is a simple AI-driven navigation game with smooth player movement, intelligent AI behavior, and engaging game-over scenarios. 🚀 Let me know if you need any additions or refinements! 😃

