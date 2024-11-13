using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
#endif

public enum GameState
{
    NotStarted,
    Started,
    Ended
}

public class GameManager : MonoBehaviour
{
    public int SPACE_JUNK_POINTS = 10; // Points to add for each space junk
    public float regularRayDamage = 0.001f;
    public float specialRayDamage = 0.005f;
    // Singleton instance
    public static GameManager Instance { get; private set; }

    // Player and enemy health (values between 0 and 1)
    private float playerHealth = 1f;
    private float enemyHealth = 1f;
    public UIController uiController;
    
    // Player points
    private int points = 0;

    // Events to trigger on health and points update
    public UnityEvent<float> OnPlayerHealthChanged;
    public UnityEvent<float> OnEnemyHealthChanged;
    public UnityEvent<int> OnPointsChanged;
    public UnityEvent OnGameWon;
    public UnityEvent OnGameLost;
    public UnityEvent PerformAttack;

    public UnityEvent OnStartProcedures;
    public UnityEvent OnGameStart;
    public UnityEvent OnGameNotStarted;

    public GameState gameState;

    // Timer for attack
    private float attackTimer;
    private float attackInterval;

    private void Awake()
    {
        // Enforce singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Instance.PerformAttack = PerformAttack;
            Instance.OnGameWon = OnGameWon;
            Instance.OnGameLost = OnGameLost;
            Instance.OnStartProcedures = OnStartProcedures;
            Instance.OnGameStart = OnGameStart;
            Instance.OnGameNotStarted = OnGameNotStarted;
            Instance.uiController = uiController;

            Destroy(gameObject);
            return;
        }

#if !UNITY_EDITOR
        gameState = GameState.NotStarted;
#endif

        if(gameState == GameState.NotStarted){
            OnGameNotStarted?.Invoke();
        }

    }
    void Start(){
        ResetAttackTimer();
    }

    private void Update()
    {
        switch(gameState){
            case GameState.NotStarted:
            Debug.Log("L42: Game not started");
            break;
            case GameState.Started:
                Debug.Log("L42: Game started");
                attackTimer -= Time.deltaTime;
                if (attackTimer <= 0)
                {
                    PerformAttack?.Invoke();
                    ResetAttackTimer();
                }
            break;
            case GameState.Ended:
            Debug.Log("L42: Game ended");
            break;
            default:
            Debug.Log("L42: Game state not recognized");
            break;
        }
    }

    public void ReducePlayerHealth()
    {
        SetPlayerHealth(playerHealth - 0.2f);
        uiController.ChangeHealth(playerHealth);

    }
    public void ReduceEnemyHealthRegular()
    {
        SetEnemyHealth(enemyHealth -regularRayDamage);
    }
    public void ReduceEnemyHealthSpecial()
    {
        SetEnemyHealth(enemyHealth - specialRayDamage);
    }

    // Public methods to modify and retrieve values
    public void SetPlayerHealth(float value)
    {
        playerHealth = Mathf.Clamp01(value);

        // Check for loss condition
        if (playerHealth <= 0)
        {
            HandleGameLost();
        }
    }

    public float GetPlayerHealth() => playerHealth;

    public void SetEnemyHealth(float value)
    {
        enemyHealth = Mathf.Clamp01(value);

        uiController.ChangeTitanhealth(enemyHealth);
        // Check for win condition
        if (enemyHealth <= 0)
        {
            HandleGameWon();
        }
    }

    public float GetEnemyHealth() => enemyHealth;

    public void AddPoints(int amount)
    {
        points += amount;
        uiController.ChangePoints(points);
    }

    public int GetPoints() => points;
    public void SpaceJunkPoints()
    {
        AddPoints(SPACE_JUNK_POINTS);

    }

    // Private methods for win/loss handling
    public void HandleGameWon()
    {
        if(gameState != GameState.Ended){
            Debug.Log("Game Won!");
            OnGameWon?.Invoke();
            gameState = GameState.Ended;
        }
        // Additional logic for winning the game can go here
    }

    public void HandleGameLost()
    {

        if(gameState != GameState.Ended){
            Debug.Log("Game Lost!");
            gameState = GameState.Ended;
            OnGameLost?.Invoke();
        }
    }

    public void TriggerStartProcedures()
    {
        if(gameState != GameState.Started){
            OnStartProcedures?.Invoke();
        }
    }

    public void TriggerActualGameStart()
    {
        if(gameState != GameState.Started){
            OnGameStart?.Invoke();
            gameState = GameState.Started;
        }
    }

    // Resets the attack timer with a random interval between 15 and 30 seconds
    private void ResetAttackTimer()
    {
        attackInterval = Random.Range(15f, 30f);
        attackTimer = attackInterval;
    }

    public void ResetGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);

        //reload current scene
        Debug.Log("L42: Resetting game");
        playerHealth = 1f;
        enemyHealth = 1f;
        points = 0;
        uiController.ChangeHealth(playerHealth);
        uiController.ChangeTitanhealth(enemyHealth);
        uiController.ChangePoints(points);
        gameState = GameState.Started;
        ResetAttackTimer();
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(GameManager))]
public class GameManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GameManager manager = (GameManager)target;

        if (GUILayout.Button("Trigger Win"))
        {
            manager.HandleGameWon();
        }

        if (GUILayout.Button("Trigger Lost"))
        {
            manager.HandleGameLost();
        }
        if (GUILayout.Button("Trigger Start"))
        {
            manager.TriggerStartProcedures();
        }
        if(GUILayout.Button("Trigger Reset"))
        {
            manager.ResetGame();
        }
    }
}
#endif
