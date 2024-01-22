using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCharacter : MonoBehaviour
{
    public float moveSpeed = 7f;
    Vector3 limitedVelocity;

    public float playerHeight = 2f;
    public LayerMask groundMask;
    bool isGrounded;
    public float groundDrag = 5f;

    [Header("Jumping")]
    public float jumpForce = 0.5f;
    public float jumpCooldown = 0.25f;
    public float airMultiplier = 0.1f;
    bool canJump = true;

    [Header("Rolling")]
    private bool canRoll = false;
    public float rollSpeed = 20f;
    public float rollDuration = 0.3f;
    private float rollTime = 0f;
    private Vector3 rollDirection;
    public float rollCooldown = 0.25f;

    [SerializeField] float fallThresholdVelocity = 5f;

    bool canInput = true;

    [SerializeField] Transform orient;
    float horizontalInput;
    float verticalInput;
    Vector3 moveDir;
    Rigidbody rb;

    Animator animator;

    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode sprintKey = KeyCode.LeftShift;
    public KeyCode rollKey = KeyCode.LeftControl;

    [Header("Skins")]
    [SerializeField] List<Material> skinList;
    Material hamsterSkinMat;
    [SerializeField] SkinnedMeshRenderer furMeshRenderer;
    [SerializeField] SkinnedMeshRenderer eyeMeshRenderer;

    public static event Action OnDeath;
    public static event Action OnWinning;
    public static event Action DeathState;

    AudioSource audioSource;

    [SerializeField]GameObject fireParticle;
    [SerializeField]GameObject electricityParticle;
    [SerializeField]GameObject bubbleParticle;

    private void OnEnable()
    {
        Timer.OnTimeEnd += Starve;
    }

    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1f;
        audioSource = GetComponent<AudioSource>();

        RandomizeSkin();

        fireParticle.SetActive(false);
        electricityParticle.SetActive(false);
        bubbleParticle.SetActive(false);

        canRoll = false;
        canInput = true;
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    private void PlayerInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
        if(canRoll && Input.GetKeyDown(rollKey))
        {
            Roll();
        }
        if(!canRoll)
        {
            rollTime += Time.deltaTime;
            float rollProgress = rollTime / Time.deltaTime;


            rb.AddForce(rollDirection * rollSpeed, ForceMode.Force);


            if (rollTime >= rollDuration && isGrounded)
            {
                Invoke(nameof(ResetRoll), rollCooldown);
            }

        }
        if (Input.GetKeyDown(jumpKey) && canJump && isGrounded)
        {
            canJump = false;
            Jump();
            Invoke(nameof(ResetJump), jumpCooldown); //invokes using cooldown as delay!!!
        }
        if (Input.GetKeyDown(sprintKey))
        {
            moveSpeed *= 2;
        }
        if (Input.GetKeyUp(sprintKey))
        {
            moveSpeed /= 2;
        }
    }

    public void SetCanInput(bool set)
    {
        canInput = set;
    }

    private void MovePlayer()
    {
        moveDir = orient.forward * verticalInput + orient.right * horizontalInput;

        if (isGrounded)
        {
            rb.AddForce(moveDir.normalized * moveSpeed * 10f, ForceMode.Force);
        }
        else if (!isGrounded)
        {
            rb.AddForce(moveDir.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);
        }
    }

    private void SpeedClamper()
    {
        Vector3 flatVelocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        if (flatVelocity.magnitude > moveSpeed)
        {
            limitedVelocity = flatVelocity.normalized * moveSpeed; //calculate what max velocity WOULD be
            rb.velocity = new Vector3(limitedVelocity.x, rb.velocity.y, limitedVelocity.z); //apply it
        }
    }

    private void Jump()
    {
        animator.SetBool("jump", true);
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    private void ResetJump()
    {
        animator.SetBool("jump", false);
        canJump = true;
    }

    private void Roll()
    {
        canRoll = false;
        animator.SetTrigger("roll");
        rollTime = 0f;
        rollDirection = orient.forward;
    }

    private void ResetRoll()
    {
        canRoll = true;
    }

    private void UpdateAnimation()
    {
        if (!canRoll && isGrounded)
        {   
            float forwardSpeed = Vector3.Magnitude(rb.velocity);
            animator.SetFloat("speed", forwardSpeed);
        }
        if (!isGrounded && rb.velocity.y < 0f)
        {
            animator.SetBool("fall", true);
        }

    }

    // Update is called once per frame
    void Update()
    {
        bool previousGrounded = isGrounded;

        isGrounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, groundMask);
        if (canInput)
        {
            PlayerInput();
        }

        if(previousGrounded && isGrounded)
        {
            if(rb.velocity.y < -fallThresholdVelocity)
            {
                Debug.Log("death");
                DeathState = FallDeath;
                InvokeDeath();
            }    
        }

        if (isGrounded)
        {
            rb.drag = groundDrag;
            animator.SetBool("fall", false);
        }
        else
        {
            rb.drag = 0;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Fire")
        {
            DeathState = BurnDeath;
            InvokeDeath();
        }

        //Check for a match with the specific tag on any GameObject that collides with your GameObject
        if (collision.gameObject.tag == "Electric")
        {
            DeathState = ElectricDeath;
            InvokeDeath();
        }

        if (collision.gameObject.tag == "Food")
        {
            InvokeWin();
        }
    }


        private void FixedUpdate()
    {
        if (canInput)
        {
            MovePlayer();
            SpeedClamper();
            UpdateAnimation();
        }
    }

    public void InvokeDeath()
    {
        SetCanInput(false);
        OnDeath?.Invoke();
        DeathState?.Invoke();
        audioSource.Play();
    }

    public void InvokeWin()
    {
        SetCanInput(false);
        OnWinning?.Invoke();
    }

    public void StopTime() //ANIM EVENT
    {
        Time.timeScale = 0;
    }
    #region Deaths
    public void FallDeath()
    {
        animator.SetTrigger("fallDeath");

    }

    public void BurnDeath()
    {
        animator.SetTrigger("burnDeath");
        fireParticle.SetActive(true);
    }

    public void ElectricDeath()
    {
        animator.SetTrigger("electricDeath");
        electricityParticle.SetActive(true);
    }

    public void Drown()
    {
        DeathState = DrownDeath;
        InvokeDeath();
    }

    public void DrownDeath()
    {
        animator.SetTrigger("drownDeath");
        bubbleParticle.SetActive(true);
    }

    public void Starve()
    {
        DeathState = StarveDeath;
        InvokeDeath();
    }

    public void StarveDeath()
    {
        animator.SetTrigger("starveDeath");
    }
#endregion
    public void RandomizeSkin()
    {
        int randomNumber = UnityEngine.Random.Range(0, skinList.Count);
        hamsterSkinMat = skinList[randomNumber];
        furMeshRenderer.material = hamsterSkinMat;
        eyeMeshRenderer.material = hamsterSkinMat;
    }


}
