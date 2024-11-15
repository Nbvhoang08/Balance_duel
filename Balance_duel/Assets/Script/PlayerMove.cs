using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;

public class PlayerMove : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform childObject;
    private Rigidbody2D rb;

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationMultiplier = 50f;
    //[SerializeField] private float continuousRotationSpeed = 20f;

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
    [SerializeField] private Transform weapon;
    [SerializeField] private BoxCollider2D bodyColider;
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
    public bool isDead => math.abs(currentRotation) >90 || isTouchingDeadzone;
    private bool isTouchingDeadzone;
    [SerializeField] private float rotationAmount = 0;
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        StarPos = transform.position;
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
            // Giới hạn rotationAmount trong khoảng -30 đến 30
            rotationAmount = Mathf.Clamp(rotationAmount, -60f, 60f);
            if (!isKnockedBack)
            {
                Vector2 movement = new Vector2(horizontalInput * moveSpeed, rb.velocity.y);
                rb.velocity = movement;
                if (childObject != null && hasStartedMoving)
                {
                
                    float rotationSpeed = 8f; // Tốc độ thay đổi ban đầu của rotationAmount
                    float maxRotationSpeed = 20f; // Tốc độ thay đổi tối đa
                    float acceleration = 2f; // Gia tốc tăng tốc độ thay đổi
                    float holdTime = 0f; // Thời gian giữ nút

                    if (horizontalInput == 1 && !isKnockedBack)
                    {
                        holdTime += Time.deltaTime;
                        rotationSpeed = Mathf.Min(rotationSpeed + acceleration * holdTime, maxRotationSpeed);
                        rotationAmount -= rotationSpeed * rotationMultiplier * Time.deltaTime;
                    }
                    else if (horizontalInput == -1)
                    {
                        holdTime += Time.deltaTime;
                        rotationSpeed = Mathf.Min(rotationSpeed + acceleration * holdTime, maxRotationSpeed);
                        rotationAmount += rotationSpeed * rotationMultiplier * 1.2f * Time.deltaTime;
                    }
                    else
                    {
                        holdTime = 0f; // Reset thời gian giữ nút khi không có đầu vào
                        rotationSpeed = 8f; // Reset tốc độ thay đổi về giá trị ban đầu
                    }   

                    currentRotation += rotationAmount * Time.fixedDeltaTime;
                    childObject.localRotation = Quaternion.Euler(0, 0, currentRotation);
                }
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

    private void ApplyImpactRotation(Vector2 playerPosition,float impactRotationForce)
    {
        // Xác định hướng va chạm
        float hitDirection = transform.position.x - playerPosition.x;
        
        // Tính toán góc xoay dựa trên hướng va chạm
        float impactRotationAmount = impactRotationForce * Mathf.Sign(hitDirection);
        
        // Áp dụng góc xoay ngay lập tức
        currentRotation -= impactRotationAmount;
        rotationAmount += 10;
        
        
        // Cập nhật rotation ngay lập tức
        childObject.localRotation = Quaternion.Euler(0, 0, currentRotation);
    }

    // Xử lý va chạm cho cả object cha và con
    private void HandleEnemyCollision(GameObject collisionObject)
    {
        if (((1 << collisionObject.layer) & enemyLayer) != 0)
        {
            
            ApplyKnockback(collisionObject.transform.position);
          
            SoundManager.Instance.PlayVFXSound(2);
            if(currentRotation <-10)
            {
                ApplyImpactRotation(collisionObject.transform.position, 20);
            }
        }

        if(collisionObject.CompareTag("DeadZone"))
        {
            isTouchingDeadzone = true;
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
        if(score <= 0) return;
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
            newBody.GetComponent<SpriteRenderer>().sprite = data.skinData.body;
            newBody.transform.localPosition = new Vector3(0, i*0.75f, 0); // Đặt vị trí theo trục Y
            spawnedBodies.Add(newBody);
        }
        
        // Cập nhật vị trí của Head
        head.localPosition = new Vector3(head.localPosition.x, (score+1)*0.75f-0.3f, head.localPosition.z);
        weapon.localPosition = new Vector3(weapon.localPosition.x, (score-1)*0.75f, weapon.localPosition.z);
        // Lấy giá trị offset hiện tại
        Vector2 offset = bodyColider.offset;

        // Thay đổi giá trị Y của offset
        offset.y = (3f * score + 7f) / 8f;
        Debug.Log("" + offset.y);
        // Gán lại giá trị offset đã thay đổi
        bodyColider.offset = offset;

        
        // Lấy giá trị size hiện tại
        Vector2 size = bodyColider.size;

        // Thay đổi giá trị Y của size
        size.y = (score+1)*0.75f-0.5f;

        // Gán lại giá trị size đã thay đổi
        bodyColider.size = size;

    }
    
    public void Reset()
    {
        currentRotation = 0;
        transform.position = StarPos;
        UpdateHeightBasedOnScore();
        StartCoroutine(resetStartMove());
        isTouchingDeadzone = false;
        rotationAmount = 0;
    }
    IEnumerator resetStartMove()
    {
        yield return new WaitForSeconds(0.05f);
        hasStartedMoving = false;
    }
    
}
