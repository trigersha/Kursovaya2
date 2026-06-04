using UnityEngine;

public class LavaGround : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        // МЫШКА
        if (other.CompareTag("Mouse"))
        {
            MouseController mouse = other.GetComponent<MouseController>();
            if (mouse != null)
            {
                mouse.TakeDamage();
                Debug.Log("Мышка упала в лаву!");
            }
        }

        // КОШКА
        if (other.CompareTag("Cat"))
        {
            CatAI cat = other.GetComponent<CatAI>();
            if (cat != null)
            {
                cat.ReturnToStart();
                Debug.Log("Кошка упала в лаву и вернулась на старт!");
            }
        }
    }
}