using UnityEngine;
using UnityEngine.SceneManagement;

public class Hole : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Mouse"))
        {
            Debug.Log("НОРКА: Мышка коснулась!");

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
                Debug.Log("ПОБЕДА!");
            }
        }
    }
}