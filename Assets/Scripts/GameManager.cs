using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Level State")]
    public int[] maxStrokesPerLevel;
    private int currentLevelIndex = 0;
    private int strokesUsed = 0;

    [Header("Collectibles")]
    private int starsInLevel = 0;
    private int starsCollected = 0;
    public TextMeshProUGUI strokesText;
    public TextMeshProUGUI starsText;

    private void Awake()
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

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // logic runs every time a scene is loaded.
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Reset stroke and star counts
        strokesUsed = 0;
        starsCollected = 0;
        
        // Find all stars in the newly loaded level
        starsInLevel = FindObjectsOfType<Star>().Length;
        
        // Update the level number
        currentLevelIndex = scene.buildIndex;
        UIManager.Instance.UpdateLevelText(currentLevelIndex + 1);
        UpdateUI();
    }

    public void CollectStar()
    {
        starsCollected++;
        UpdateUI();
    }

    public bool AllStarsCollected()
    {
        return starsCollected >= starsInLevel;
    }

    public void IncrementStrokes()
    {
        strokesUsed++;
        UpdateUI();

        if (strokesUsed >= maxStrokesPerLevel[currentLevelIndex])
        {
            UIManager.Instance.ShowMessage("Out of Strokes!");
            Invoke("RestartLevel", 2f);
        }
    }
    
    public void LevelComplete()
    {
        UIManager.Instance.ShowMessage("Level Complete!");
        strokesUsed = 9999;
        Invoke("LoadNextLevel", 2f);
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(currentLevelIndex);
    }

    private void LoadNextLevel()
    {
        int nextLevelIndex = currentLevelIndex + 1;
        if (nextLevelIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextLevelIndex);
        }
        else
        {
            UIManager.Instance.ShowMessage("You beat the game!");
        }
    }

    public void UpdateUI()
    {
        if (strokesText != null)
        {
            strokesText.text = $"Strokes: {strokesUsed} / {maxStrokesPerLevel[currentLevelIndex]}";
        }
        if (starsText != null)
        {
            starsText.text = $"Stars: {starsCollected} / {starsInLevel}";
        }
    }
}