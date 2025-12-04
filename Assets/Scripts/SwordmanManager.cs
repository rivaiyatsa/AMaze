using UnityEngine;

public class SwordmanManager : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float walkSpeed = 3f;
    [SerializeField] private float runSpeed = 6f;
    
    private Rigidbody2D rb;
    private Animator animator;
    
    private Vector2 movement;
    private Vector2 lastDirection;
    private bool isRunning;
    private bool isAttacking;
    private bool isHurt;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        
        // Default menghadap depan (membelakangi layar) sesuai yang Anda mau
        lastDirection = Vector2.up; // Vertical = 1 (Depan)
        
        // Set animator ke Idle_Depan
        animator.SetFloat("Horizontal", 0);
        animator.SetFloat("Vertical", 1); // Depan
        animator.SetFloat("Speed", 0);
    }
    
    void Update()
    {
        if (isAttacking || isHurt) return; // Tidak bisa kontrol saat attack/hurt
        
        // Input WASD
        float horizontal = Input.GetAxisRaw("Horizontal"); // A = -1, D = 1
        float vertical = Input.GetAxisRaw("Vertical");     // S = -1, W = 1
        
        movement = new Vector2(horizontal, vertical);
        
        // Simpan arah terakhir (untuk attack/hurt sesuai arah terakhir)
        if (movement.magnitude > 0.1f)
        {
            lastDirection = movement.normalized;
        }
        
        // Normalize diagonal
        if (movement.magnitude > 1)
        {
            movement.Normalize();
        }
        
        // Check run (Left Shift)
        isRunning = Input.GetKey(KeyCode.LeftShift);
        
        // Attack (Left Mouse Click)
        if (Input.GetMouseButtonDown(0) && !isAttacking)
        {
            Attack();
            return;
        }
        
        UpdateAnimator();
    }
    
    void FixedUpdate()
    {
        if (!isAttacking && !isHurt)
        {
            float currentSpeed = isRunning ? runSpeed : walkSpeed;
            rb.MovePosition(rb.position + movement * currentSpeed * Time.fixedDeltaTime);
        }
        else
        {
            rb.linearVelocity = Vector2.zero; // Stop saat attack/hurt
        }
    }
    
    void UpdateAnimator()
    {
        // Update direction berdasarkan input atau lastDirection
        if (movement.magnitude > 0.1f)
        {
            animator.SetFloat("Horizontal", movement.x);
            animator.SetFloat("Vertical", movement.y);
        }
        else
        {
            // Tetap simpan arah terakhir untuk idle
            animator.SetFloat("Horizontal", lastDirection.x);
            animator.SetFloat("Vertical", lastDirection.y);
        }
        
        // Set speed untuk transition
        float speed = movement.magnitude;
        if (isRunning && speed > 0.1f)
        {
            speed = 1f; // Run
        }
        else if (speed > 0.1f)
        {
            speed = 0.3f; // Walk
        }
        else
        {
            speed = 0f; // Idle
        }
        
        animator.SetFloat("Speed", speed);
    }
    
    void Attack()
    {
        isAttacking = true;
        animator.SetBool("IsAttacking", true);
        
        // Attack sesuai arah terakhir
        animator.SetFloat("Horizontal", lastDirection.x);
        animator.SetFloat("Vertical", lastDirection.y);
        
        // Reset setelah animasi selesai (sesuaikan durasi animasi)
        Invoke("ResetAttack", 0.5f);
    }
    
    void ResetAttack()
    {
        isAttacking = false;
        animator.SetBool("IsAttacking", false);
    }
    
    // Dipanggil dari collision/trigger dengan monster/trap
    public void TakeDamage(int damage)
    {
        if (isHurt) return; // Tidak bisa kena damage berturut-turut
        
        isHurt = true;
        animator.SetTrigger("TakeDamage");
        
        // Kurangi HP di sini
        // currentHP -= damage;
        
        // Reset hurt state
        Invoke("ResetHurt", 0.5f);
    }
    
    void ResetHurt()
    {
        isHurt = false;
    }
    
    // Dipanggil saat HP = 0
    public void Die()
    {
        animator.SetTrigger("Death");
        // Disable movement
        this.enabled = false;
        // Game Over logic
    }
}
