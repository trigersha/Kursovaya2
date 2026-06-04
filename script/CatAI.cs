using UnityEngine;

public class CatAI : MonoBehaviour
{
    //  НАСТРОЙКИ ДВИЖЕНИЯ 
    public Transform mouse;
    public float patrolSpeed = 2f;
    public float chaseSpeed = 5f;
    public float chaseDistance = 7f;

    // ПАТРУЛЬ 
    public float leftBoundary = -6f;   
    public float rightBoundary = 6f;   
    private bool movingRight = true;    

    // ПРОВЕРКА СТЕН 
    public float wallCheckDistance = 0.5f;  
    public LayerMask wallLayer;            

    // НАСТРОЙКИ ПРЫЖКОВ
    public bool canJump = true;
    public float jumpForce = 12f;
    public float jumpCooldown = 0.5f;
    private float lastJumpTime;

    // КОМПОНЕНТЫ 
    private Rigidbody2D rb;
    private bool isGrounded;
    private float originalScaleX;

    // ВОЗВРАТ НА СТАРТ 
    private Vector3 startPosition;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        originalScaleX = Mathf.Abs(transform.localScale.x);
        startPosition = transform.position;
    }

    void Update()
    {
        if (mouse == null) return;

        CheckIfGrounded();

        float distance = Vector2.Distance(transform.position, mouse.position);

        if (distance < chaseDistance)
            Chase();
        else
            Patrol();

        // Прыжок за мышкой
        if (canJump && isGrounded && Time.time > lastJumpTime + jumpCooldown)
        {
            if (mouse.position.y > transform.position.y + 0.5f)
            {
                Jump();
            }
        }

        // Поворот
        if (rb.velocity.x > 0)
            transform.localScale = new Vector3(originalScaleX, originalScaleX, 1f);
        else if (rb.velocity.x < 0)
            transform.localScale = new Vector3(-originalScaleX, originalScaleX, 1f);
    }

    void CheckIfGrounded()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 0.7f);
        isGrounded = hit.collider != null;
        Debug.DrawRay(transform.position, Vector2.down * 0.7f, isGrounded ? Color.green : Color.red);
    }

    void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        lastJumpTime = Time.time;
        Debug.Log("Кошка прыгнула!");
    }

    
    void Patrol()
    {
        
        bool wallAhead = CheckWallAhead();

        
        bool atBoundary = (movingRight && transform.position.x >= rightBoundary) ||
                          (!movingRight && transform.position.x <= leftBoundary);

        
        if (wallAhead || atBoundary)
        {
            movingRight = !movingRight;
        }

        
        float direction = movingRight ? 1f : -1f;
        rb.velocity = new Vector2(direction * patrolSpeed, rb.velocity.y);
    }

    
    bool CheckWallAhead()
    {
       
        float direction = movingRight ? 1f : -1f;

        
        Vector2 checkPosition = transform.position + new Vector3(direction * 0.5f, 0, 0);

        
        RaycastHit2D hit = Physics2D.Raycast(checkPosition, Vector2.right * direction, wallCheckDistance, wallLayer);

        
        Debug.DrawRay(checkPosition, Vector2.right * direction * wallCheckDistance, Color.blue);

        return hit.collider != null;
    }

    void Chase()
    {
        Vector2 direction = (mouse.position - transform.position).normalized;
        rb.velocity = new Vector2(direction.x * chaseSpeed, rb.velocity.y);
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Mouse"))
        {
            MouseController mouseScript = col.gameObject.GetComponent<MouseController>();
            if (mouseScript != null)
            {
                mouseScript.TakeDamage();
                ReturnToStart();
            }
        }

        
        if (col.gameObject.CompareTag("Wall"))
        {
            movingRight = !movingRight;
        }
    }

    public void ReturnToStart()
    {
        rb.velocity = Vector2.zero;
        transform.position = startPosition;
        Debug.Log("Кошка вернулась на старт!");
    }

    void OnDrawGizmosSelected()
    {
        
        Gizmos.color = Color.green;
        Gizmos.DrawLine(new Vector3(leftBoundary, -5, 0), new Vector3(leftBoundary, 5, 0));
        Gizmos.DrawLine(new Vector3(rightBoundary, -5, 0), new Vector3(rightBoundary, 5, 0));

        
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, chaseDistance);
    }
}