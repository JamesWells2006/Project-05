using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    public int playerHealth = 3;
    public int score = 0;
    public int requiredCollectibles = 5;
    public float levelTimeLimit = 300f; // 5 minutes
    
    public TMPro.TextMeshProUGUI scoreText;
    public TMPro.TextMeshProUGUI healthText;
    public TMPro.TextMeshProUGUI timerText;
    public GameObject gameOverScreen;
    public GameObject victoryScreen;
    public GameObject pauseMenu;
    
    private float remainingTime;
    private bool isPaused = false;
    private bool isGameOver = false;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void Start()
    {
        ResetGame();
    }
    
    void Update()
    {
        if (isGameOver) return;
        
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
        
        if (!isPaused)
        {
            UpdateTimer();
            UpdateUI();
        }
    }
    
    public void ResetGame()
    {
        playerHealth = 3;
        score = 0;
        remainingTime = levelTimeLimit;
        isGameOver = false;
        isPaused = false;
        
        if (gameOverScreen != null) gameOverScreen.SetActive(false);
        if (victoryScreen != null) victoryScreen.SetActive(false);
        if (pauseMenu != null) pauseMenu.SetActive(false);
        
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        UpdateUI();
    }
    
    void UpdateTimer()
    {
        remainingTime -= Time.deltaTime;
        if (remainingTime <= 0)
        {
            remainingTime = 0;
            GameOver();
        }
    }
    
    void UpdateUI()
    {
        if (scoreText != null)
            scoreText.text = "Score: " + score;
        
        if (healthText != null)
            healthText.text = "Health: " + playerHealth;
        
        if (timerText != null)
        {
            int minutes = Mathf.FloorToInt(remainingTime / 60);
            int seconds = Mathf.FloorToInt(remainingTime % 60);
            timerText.text = string.Format("Time: {0:00}:{1:00}", minutes, seconds);
        }
    }
    
    public void CollectItem(GameObject collectible)
    {
        score++;
        Destroy(collectible);
        
        // Play sound or particle effect here
        
        if (score >= requiredCollectibles)
        {
            // Enable exit or trigger event
        }
        
        UpdateUI();
    }
    
    public void PlayerHit()
    {
        playerHealth--;
        
        if (playerHealth <= 0)
        {
            GameOver();
        }
        
        UpdateUI();
    }
    
    public void CompleteLevel()
    {
        isGameOver = true;
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        
        if (victoryScreen != null)
            victoryScreen.SetActive(true);
    }
    
    public void GameOver()
    {
        isGameOver = true;
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        
        if (gameOverScreen != null)
            gameOverScreen.SetActive(true);
    }
    
    public void TogglePause()
    {
        isPaused = !isPaused;
        
        if (isPaused)
        {
            Time.timeScale = 0f;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            
            if (pauseMenu != null)
                pauseMenu.SetActive(true);
        }
        else
        {
            Time.timeScale = 1f;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            
            if (pauseMenu != null)
                pauseMenu.SetActive(false);
        }
    }
    
    public void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
    
    public void StartGame()
    {
        ResetGame();
        SceneManager.LoadScene("MazeLevel");
    }
    
    public void RestartGame()
    {
        ResetGame();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    
    public void QuitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}
