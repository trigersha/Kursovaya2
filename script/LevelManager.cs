using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    [Header("Настройки уровней")]
    public int currentLevel = 1;
    public int totalLevels = 3;

    [Header("Требования для перехода")]
    public int requiredCheese = 5;

    private int cheeseCollected = 0;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("LevelManager создан!");
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        cheeseCollected = 0;
    }

    
    public void AddCheese(int amount)
    {
        cheeseCollected += amount;
        Debug.Log("Собрано сыра: " + cheeseCollected + "/" + requiredCheese);
    }

    
    public void CompleteLevel()
    {
        Debug.Log("Уровень пройден!");

        string currentScene = SceneManager.GetActiveScene().name;

        if (currentScene == "Level1")
        {
            SceneManager.LoadScene("Level2");
        }
        else if (currentScene == "Level2")
        {
            SceneManager.LoadScene("Level3");
        }
        else if (currentScene == "Level3")
        {
            Debug.Log("ПОБЕДА! Игра пройдена!");
        }
    }

     
    public int GetCheeseCollected()
    {
        return cheeseCollected;
    }
}