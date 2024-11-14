using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerMove : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform childObject;
    private Rigidbody2D rb;

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationMultiplier = 50f;
    [SerializeField] private float continuousRotationSpeed = 20f;

    [Header("Knockback Settings")]
    [SerializeField] private float knockbackForce = 10f;
    [SerializeField] private float knockbackTime = 0.2f;
    [SerializeField] private LayerMask enemyLayer;

    [Header("Skin Setting")]
    [SerializeField] private Data data;
    [SerializeField] private SpriteRenderer Head;
    [SerializeField] private SpriteRenderer Body;
    [SerializeField] private SpriteRenderer Leg;
    [SerializeField] private SpriteRenderer Weapon;

    [Header("Body Setting")]
    [SerializeField] private GameObject bodyPrefab;
    [SerializeField] private Transform body;
    [SerializeField] private Transform head;
    public int score;
    private List<GameObject> spawnedBodies = new List<GameObject>();

    public Animator anim;
    public float horizontalInput;
    private float currentRotation;
    private float lastMoveDirection;
    private bool hasStartedMoving;
    private bool isKnockedBack;
    private float knockbackTimeCounter;
    private Vector3 StarPos;
    public bool isDead => currentRotation >90 || currentRotation < -90;
    [SerializeField] private float direction =1;
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        StarPos = transform.position;
        if (rb == null)
        {
            Debug.LogError("Rigidbody2D component is missing!");
        }

        if (childObject == null)
        {
            Debug.LogError("Child object reference is missing!");
        }

        // Thêm trigger cho object con nếu cần
        SetupColliders();
        UpdateSkin();
    }

    private void SetupColliders()
    {
        // Đảm bảo object con có collider và là trigger
        Collider2D childCollider = childObject.GetComponent<Collider2D>();
        if (childCollider == null)
        {
            childCollider = childObject.gameObject.AddComponent<CircleCollider2D>();
        }
        childCollider.isTrigger = true;
    }

    private void Update()
    {
        if(!isDead)
        {
            if (!isKnockedBack)
            {
                //horizontalInput = Input.GetAxisRaw("Horizontal");

                if (horizontalInput != 0)
                {
                    lastMoveDirection = horizontalInput;
                    hasStartedMoving = true;
                    direction = -1 ;
                }   
            }
            else
            {
                knockbackTimeCounter -= Time.deltaTime;
                if (knockbackTimeCounter <= 0)
                {
                    isKnockedBack = false;
                    rb.velocity = Vector2.zero;
                }
            }   
            anim.SetBool("moving",rb.velocity.x !=0);
        }
        anim.SetBool("dead",isDead);
    }

    private void FixedUpdate()
    {
        if(!isDead)
        {
            if (!isKnockedBack)
            {
                Vector2 movement = new Vector2(horizontalInput * moveSpeed, rb.velocity.y);
                rb.velocity = movement;
            }

            if (childObject != null && hasStartedMoving)
            {
                float rotationAmount;

                if (rb.velocity.x != 0)
                {
                    rotationAmount = -rb.velocity.x * rotationMultiplier *direction;
                }
                else
                {
                    rotationAmount = -lastMoveDirection * continuousRotationSpeed *direction;
                }

                currentRotation += rotationAmount * Time.fixedDeltaTime;
                childObject.localRotation = Quaternion.Euler(0, 0, currentRotation);
            }
        }
       
    }

    // Va chạm từ object cha
    private void OnCollisionEnter2D(Collision2D collision)
    {
        HandleEnemyCollision(collision.gameObject);
    }

    // Va chạm từ object con (Trigger)
    private void OnTriggerEnter2D(Collider2D other)
    {
        HandleEnemyCollision(other.gameObject);
    }
       private void UpdateSkin()
    {
        Head.sprite = data.skinData.head;
        Body.sprite = data.skinData.body;
        Leg.sprite = data.skinData.leg;
        Weapon.sprite = data.skinData.Weapon;
    }



    // Xử lý va chạm cho cả object cha và con
    private void HandleEnemyCollision(GameObject collisionObject)
    {
        if (((1 << collisionObject.layer) & enemyLayer) != 0)
        {
            
            ApplyKnockback(collisionObject.transform.position);
            if (currentRotation <= 0 && currentRotation > -60)
            {
                direction = 1;
            }
        }
    }

    private void ApplyKnockback(Vector2 enemyPosition)
    {
        Vector2 knockbackDirection = (transform.position - new Vector3(enemyPosition.x, enemyPosition.y, 0)).normalized;
        rb.velocity = Vector2.zero;
        rb.AddForce(knockbackDirection * knockbackForce, ForceMode2D.Impulse);
        
        isKnockedBack = true;
        knockbackTimeCounter = knockbackTime;
    }
    void UpdateHeightBasedOnScore()
    {
        // Xóa các bodyPrefab cũ
        foreach (var spawnedBody in spawnedBodies)
        {
            Destroy(spawnedBody);
        }
        spawnedBodies.Clear();

        // Spawn các bodyPrefab mới dựa trên điểm số
        for (int i = 0; i < score; i++)
        {
            GameObject newBody = Instantiate(bodyPrefab, body);
            newBody.transform.localPosition = new Vector3(0, i + 1, 0); // Đặt vị trí theo trục Y
            spawnedBodies.Add(newBody);
        }

        // Cập nhật vị trí của Head
        head.localPosition = new Vector3(head.localPosition.x, score + 1, head.localPosition.z);
    }
    public void Reset()
    {
        currentRotation = 0;
        transform.position = StarPos;
    }
    
}
