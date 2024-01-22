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

    public float jumpForce = 0.5f;
    public float jumpCooldown = 0.25f;
    public float airMultiplier = 0.1f;
    bool canJump = true;

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

    [SerializeField] List<Material> skinList;
    Material hamsterSkinMat;
    [SerializeField] SkinnedMeshRenderer furMeshRenderer;
    [SerializeField] SkinnedMeshRenderer eyeMeshRenderer;

    public static event Action OnDeath;
    public static event Action OnWinning;
    public static event Action DeathState;

    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1f;

        RandomizeSkin();

        canInput = true;
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    private void PlayerInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
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

    private void UpdateAnimation()
    {
        float forwardSpeed = Vector3.Magnitude(rb.velocity);
        animator.SetFloat("speed", forwardSpeed);
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

    public void FallDeath()
    {
        animator.SetTrigger("fallDeath");

    }

    public void BurnDeath()
    {
        animator.SetTrigger("burnDeath");
    }

    public void ElectricDeath()
    {
        animator.SetTrigger("electricDeath");

    }

    public void Drown()
    {
        DeathState = DrownDeath;
        InvokeDeath();
    }

    public void DrownDeath()
    {
        animator.SetTrigger("drownDeath");
    }

    public void RandomizeSkin()
    {
        int randomNumber = UnityEngine.Random.Range(0, skinList.Count);
        hamsterSkinMat = skinList[randomNumber];
        furMeshRenderer.material = hamsterSkinMat;
        eyeMeshRenderer.material = hamsterSkinMat;
    }


}
