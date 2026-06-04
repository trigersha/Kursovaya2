using UnityEngine;
using UnityEngine.SceneManagement;

public class MouseController : MonoBehaviour
{
    public float speed = 7f;
    public float jumpForce = 16f;

    private Rigidbody2D rb;

    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;
    private bool isGrounded;

    private int cheeseCount = 0;
    private int lives = 3;

    private Vector3 startPosition;
    private float originalScaleX;

    //  ЗВУКИ 
    public AudioClip cheeseSound;
    public AudioClip winSound;
    public AudioClip loseSound;
    private AudioSource audioSource;

    // ФОНОВАЯ МУЗЫКА 
    private AudioSource backgroundMusic;

    // ДЛЯ САЛЮТА 
    public GameObject particlePrefab;

    //  ДЛЯ ПОРАЖЕНИЯ 
    private bool isGameOver = false;
    private float restartTimer = 0f;

    //  ДЛЯ ПОБЕДЫ 
    private bool isWin = false;
    private float winTimer = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        startPosition = transform.position;
        originalScaleX = Mathf.Abs(transform.localScale.x);

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        FindAndStartBackgroundMusic();
    }

    void FindAndStartBackgroundMusic()
    {
        GameObject musicObj = GameObject.Find("BackgroundMusic");
        if (musicObj != null)
        {
            backgroundMusic = musicObj.GetComponent<AudioSource>();
            if (backgroundMusic != null && !backgroundMusic.isPlaying)
            {
                backgroundMusic.Play();
                Debug.Log("Фоновая музыка запущена!");
            }
        }
        else
        {
            Debug.LogWarning("Объект BackgroundMusic не найден в сцене!");
        }
    }

    void Update()
    {
        if (isWin)
        {
            winTimer += Time.deltaTime;
            if (winTimer >= 3f)
            {
                ReturnToMenu();
            }
            return;
        }

        if (isGameOver)
        {
            restartTimer += Time.deltaTime;
            if (restartTimer >= 3f)
            {
                ReturnToMenu();
            }
            return;
        }

        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        float moveX = Input.GetAxis("Horizontal");
        rb.velocity = new Vector2(moveX * speed, rb.velocity.y);

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }

        if (moveX > 0)
            transform.localScale = new Vector3(originalScaleX, originalScaleX, 1f);
        else if (moveX < 0)
            transform.localScale = new Vector3(-originalScaleX, originalScaleX, 1f);
    }

    void OnGUI()
    {
        GUIStyle style = new GUIStyle();
        style.fontSize = 40;
        style.normal.textColor = new Color(0.8f, 0.2f, 0.4f);
        style.fontStyle = FontStyle.Bold;

        GUIStyle styleRight = new GUIStyle();
        styleRight.fontSize = 40;
        styleRight.normal.textColor = new Color(0.8f, 0.2f, 0.4f);
        styleRight.fontStyle = FontStyle.Bold;
        styleRight.alignment = TextAnchor.UpperRight;

        if (isWin)
        {
            GUIStyle winStyle = new GUIStyle();
            winStyle.fontSize = 80;
            winStyle.normal.textColor = Color.yellow;
            winStyle.fontStyle = FontStyle.Bold;
            winStyle.alignment = TextAnchor.MiddleCenter;

            GUI.Label(new Rect(0, Screen.height / 2 - 100, Screen.width, 120), "🎉 ТЫ ПОБЕДИЛ! 🎉", winStyle);

            GUIStyle congratsStyle = new GUIStyle();
            congratsStyle.fontSize = 35;
            congratsStyle.normal.textColor = Color.white;
            congratsStyle.alignment = TextAnchor.MiddleCenter;

            GUI.Label(new Rect(0, Screen.height / 2 + 20, Screen.width, 50), "Поздравляю! Ты прошёл всю игру!", congratsStyle);

            GUIStyle exitStyle = new GUIStyle();
            exitStyle.fontSize = 25;
            exitStyle.normal.textColor = Color.gray;
            exitStyle.alignment = TextAnchor.MiddleCenter;

            GUI.Label(new Rect(0, Screen.height / 2 + 80, Screen.width, 50), "Возврат в меню через " + (3 - (int)winTimer) + " секунд...", exitStyle);
        }

        if (isGameOver && !isWin)
        {
            GUIStyle gameOverStyle = new GUIStyle();
            gameOverStyle.fontSize = 60;
            gameOverStyle.normal.textColor = Color.red;
            gameOverStyle.fontStyle = FontStyle.Bold;
            gameOverStyle.alignment = TextAnchor.MiddleCenter;

            GUI.Label(new Rect(0, Screen.height / 2 - 100, Screen.width, 100), "💀 ТЫ ПРОИГРАЛ! 💀", gameOverStyle);

            GUIStyle restartStyle = new GUIStyle();
            restartStyle.fontSize = 30;
            restartStyle.normal.textColor = Color.white;
            restartStyle.alignment = TextAnchor.MiddleCenter;

            GUI.Label(new Rect(0, Screen.height / 2, Screen.width, 50), "Возврат в меню через " + (3 - (int)restartTimer) + " секунд...", restartStyle);
        }

        if (!isGameOver && !isWin)
        {
            GUI.Label(new Rect(20, 20, 250, 60), "🧀 Сыр: " + cheeseCount, style);
        }

        GUI.Label(new Rect(Screen.width - 250, 20, 230, 60), "❤️ Жизни: " + lives, styleRight);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (isGameOver || isWin) return;

        if (other.CompareTag("Cheese"))
        {
            cheeseCount++;
            Destroy(other.gameObject);
            PlayCheeseSound();
        }

        if (other.CompareTag("Hole"))
        {
            string currentScene = SceneManager.GetActiveScene().name;

            if (currentScene == "Level1")
                SceneManager.LoadScene("Level2");
            else if (currentScene == "Level2")
                SceneManager.LoadScene("Level3");
            else if (currentScene == "Level3")
            {
                WinGame();
                StopBackgroundMusic();
                PlayWinSound();
                CreateFirework();
                CreateFirework();
                CreateFirework();
            }
        }

        if (other.CompareTag("Cat"))
        {
            lives--;
            transform.position = startPosition;

            if (lives <= 0)
            {
                GameOver();
                StopBackgroundMusic();
                PlayLoseSound();
            }
        }
    }

    void StopBackgroundMusic()
    {
        if (backgroundMusic != null)
        {
            backgroundMusic.Stop();
            Debug.Log("Фоновая музыка остановлена!");
        }
    }

    void PlayCheeseSound()
    {
        if (cheeseSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(cheeseSound);
        }
    }

    void PlayWinSound()
    {
        if (winSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(winSound);
        }
    }

    void PlayLoseSound()
    {
        if (loseSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(loseSound);
        }
    }

    void CreateFirework()
    {
        if (particlePrefab == null)
        {
            return;
        }

        for (int i = 0; i < 30; i++)
        {
            GameObject p = Instantiate(particlePrefab, transform.position, Quaternion.identity);
            p.transform.position += new Vector3(Random.Range(-2f, 2f), Random.Range(-1f, 2f), 0);

            Rigidbody2D rb = p.AddComponent<Rigidbody2D>();
            rb.velocity = new Vector2(Random.Range(-6f, 6f), Random.Range(4f, 10f));
            rb.gravityScale = 0.8f;

            SpriteRenderer sr = p.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.color = GetRandomColor();
            }

            Destroy(p, 1.5f);
        }
    }

    Color GetRandomColor()
    {
        Color[] colors = { Color.red, Color.yellow, Color.green, Color.blue, Color.magenta, Color.cyan };
        return colors[Random.Range(0, colors.Length)];
    }

    void WinGame()
    {
        isWin = true;
        rb.velocity = Vector2.zero;
    }

    void GameOver()
    {
        isGameOver = true;
        lives = 0;
        rb.velocity = Vector2.zero;
    }

    void ReturnToMenu()
    {
        isGameOver = false;
        isWin = false;
        restartTimer = 0f;
        winTimer = 0f;
        lives = 3;
        cheeseCount = 0;
        SceneManager.LoadScene("MainMenu");
    }

    public void TakeDamage()
    {
        if (isGameOver || isWin) return;

        lives--;
        transform.position = startPosition;

        if (lives <= 0)
        {
            GameOver();
            StopBackgroundMusic();
            PlayLoseSound();
        }
    }

    public int GetCheeseCount()
    {
        return cheeseCount;
    }
}