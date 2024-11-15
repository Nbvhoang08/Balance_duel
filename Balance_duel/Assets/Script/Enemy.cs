using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

public class Enemy : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform childObject;
    private Rigidbody2D rb;

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float rotationMultiplier = 50f;
    [SerializeField] private float continuousRotationSpeed = 20f;
    //[SerializeField] private float movementRange = 5f;
    [SerializeField] private float directionChangeTime = 3f;
    
    [Header("Balance Settings")]
    [SerializeField] private float criticalAngleThreshold = 45f;
    [SerializeField] private float stableAngleThreshold = 10f;

    [Header("Knockback Settings")]
    [SerializeField] private float knockbackForce = 10f;
    [SerializeField] private float knockbackTime = 0.2f;
    [SerializeField] private float impactRotationForce = 30f; // Lực xoay khi va chạm
    [SerializeField] private float rotationRecoverySpeed = 40f; // Tốc độ hồi phục rotation
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private LayerMask WeaponLayer;
    [SerializeField] private WeaponCheck weaponCheck;
    [SerializeField]private float moveDirection = 1f;
    [SerializeField]private float direction = 1f;

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
     
    private Vector3 startPosition;
    private float currentRotation;
    
    private float directionTimer;
    private bool isKnockedBack;
    private float knockbackTimeCounter;
    
    private float targetImpactRotation; // Góc xoay mục tiêu khi va chạm
    private bool isRecoveringFromImpact; // Trạng thái đang hồi phục sau va chạm
    
    public bool isDead => math.abs(currentRotation) > 90 || isTouchingDeadzone;
     private bool isTouchingDeadzone;
    public Animator anim;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        startPosition = transform.position;
        
        UpdateSkin();

        SetupColliders();
    }

    private void SetupColliders()
    {
        Collider2D childCollider = childObject.GetComponent<Collider2D>();
        if (childCollider == null)
        {
            childCollider = childObject.gameObject.AddComponent<CircleCollider2D>();
        }
        childCollider.isTrigger = true;
    }

    private void Update()
    {
        if (!isDead)
        {
            if (!isKnockedBack)
            {
                HandleMovementAndBalance();
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
            
            // Xử lý hồi phục rotation sau va chạm
            if (isRecoveringFromImpact)
            {
                float rotationDifference = targetImpactRotation - currentRotation;
                if (Mathf.Abs(rotationDifference) > 0.1f)
                {
                    currentRotation += Mathf.Sign(rotationDifference) * rotationRecoverySpeed * Time.deltaTime;
                    childObject.localRotation = Quaternion.Euler(0, 0, currentRotation);
                }
                else
                {
                    isRecoveringFromImpact = false;
                }
            }
            
            anim?.SetBool("moving", rb.velocity.x != 0);
        }
        anim?.SetBool("dead", isDead);
    }
     private void UpdateSkin()
    {
        Head.sprite = data.skinData.head;
        Body.sprite = data.skinData.body;
        Leg.sprite = data.skinData.leg;
        Weapon.sprite = data.skinData.Weapon;
    }

    private void HandleMovementAndBalance()
    {
        float absRotation = Mathf.Abs(currentRotation);

        if (absRotation > criticalAngleThreshold)
        {
            if (currentRotation > 0)
            {
                moveDirection = 1f;
                direction = 1f;
            }
            else
            {
                moveDirection = -1f;
                direction = 1f;
                
            }
            directionTimer = 0f;
        }
        else if (absRotation < stableAngleThreshold && !isRecoveringFromImpact)
        {
            // float distanceFromStart = transform.position.x - startPosition.x;
            // if (Mathf.Abs(distanceFromStart) > movementRange)
            // {
            //     moveDirection *= -1;
            //     direction = -direction;
            //     directionTimer = 0f;
            // }
            // else
            // {
                
            // }
            // directionTimer += Time.deltaTime;
            if (directionTimer >= directionChangeTime)
            {
                    moveDirection *= -1;
                    direction = -direction;
                    directionTimer = 0f;
            }
        }
    }

    private void FixedUpdate()
    {
        if (!isDead)
        {
            if (!isKnockedBack)
            {
                Vector2 movement = new Vector2(moveDirection * moveSpeed, rb.velocity.y);
                rb.velocity = movement;

                if (childObject != null && !isRecoveringFromImpact)
                {
                    float rotationAmount;
                    if (rb.velocity.x != 0)
                    {
                        rotationAmount = -rb.velocity.x * rotationMultiplier * direction;
                    }
                    else
                    {
                        rotationAmount = -moveDirection * continuousRotationSpeed * direction;
                    }

                    currentRotation += rotationAmount * Time.fixedDeltaTime;
                    childObject.localRotation = Quaternion.Euler(0, 0, currentRotation);
                }
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        HandlePlayerCollision(collision.gameObject);
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        HandlePlayerCollision(other.gameObject);
  
    }

    private void HandlePlayerCollision(GameObject collisionObject)
    {   
        if(weaponCheck.isWeapon)
        {
            if(((1 << collisionObject.layer) & WeaponLayer) != 0 )
            {
                ApplyKnockback(collisionObject.transform.position);
                if(currentRotation <-10)
                {   
                    moveDirection = 1;
                }
            }
            
        }
        else
        {
          
             if (((1 << collisionObject.layer) & playerLayer) != 0)
            {
                ApplyKnockback(collisionObject.transform.position);
                ApplyImpactRotation(collisionObject.transform.position,20f);
                if(currentRotation <-10)
                {   
                    moveDirection = 1;
                    Debug.Log("?");
                }
            }
            else if(((1 << collisionObject.layer) & WeaponLayer) != 0 )
            {
                ApplyKnockback(collisionObject.transform.position);
                if(currentRotation >-20)
                {
                    ApplyImpactRotation(collisionObject.transform.position,impactRotationForce );
                }
                if(currentRotation <-10)
                {   
                    moveDirection = 1;
                }
            }
        }
        if(collisionObject.CompareTag("DeadZone"))
        {
            isTouchingDeadzone = true;
        }
       

    }

    private void ApplyKnockback(Vector2 playerPosition)
    {
        Vector2 knockbackDirection = (transform.position - new Vector3(playerPosition.x, playerPosition.y, 0)).normalized;
        
        rb.velocity = Vector2.zero;
        rb.AddForce(knockbackDirection * knockbackForce, ForceMode2D.Impulse);
        if(currentRotation <-40)
        {
            rb.AddForce(new Vector2(1,0)* knockbackForce, ForceMode2D.Impulse);
        }
        
        isKnockedBack = true;
        knockbackTimeCounter = knockbackTime;
    }

    private void ApplyImpactRotation(Vector2 playerPosition,float impactRotationForce)
    {
        // Xác định hướng va chạm
        float hitDirection = transform.position.x - playerPosition.x;
        
        // Tính toán góc xoay dựa trên hướng va chạm
        float impactRotationAmount = impactRotationForce * Mathf.Sign(hitDirection);
        
        // Áp dụng góc xoay ngay lập tức
        currentRotation -= impactRotationAmount;
        
        // Lưu góc mục tiêu để hồi phục
        targetImpactRotation = currentRotation + impactRotationAmount * 0.2f; // Hồi phục 70% góc va chạm
        
        // Bắt đầu quá trình hồi phục
        isRecoveringFromImpact = true;
        
        // Cập nhật rotation ngay lập tức
        childObject.localRotation = Quaternion.Euler(0, 0, currentRotation);
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
        transform.position = startPosition;
        UpdateHeightBasedOnScore();
        isTouchingDeadzone = false;
        direction = 1;
        //StartCoroutine(resetStartMove());
        
    }
   
    
}
