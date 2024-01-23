using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    [SerializeField] Transform orient;
    [SerializeField] Transform player;
    [SerializeField] Transform playerObj;
    [SerializeField] Rigidbody rb;

    public float rotSpeed = 7f;
    bool canRotate;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void OnEnable()
    {
        CinematicTracker.OnCinematicFinish += _ => SetCanRotate(true);
        ThirdPersonCharacter.OnDeath += IsDead;
    }
    private void OnDisable()
    {
        CinematicTracker.OnCinematicFinish -= _ => SetCanRotate(true);
        ThirdPersonCharacter.OnDeath -= IsDead;
    }

    public void IsDead()
    {
       this.canRotate = false;
    }
    public void SetCanRotate(bool set)
    {
        this.canRotate = set;
    }

    // Update is called once per frame
    void Update()
    {
        MoveCamera();
    }

    public void MoveCamera()
    {
        if (canRotate)
        {
            Vector3 viewDir = player.position - new Vector3(transform.position.x, player.position.y, transform.position.z);
            orient.forward = viewDir.normalized;

            float horizontalInput = Input.GetAxis("Horizontal");
            float verticalInput = Input.GetAxis("Vertical");

            Vector3 inputDir = orient.forward * verticalInput + orient.right * horizontalInput;

            if (inputDir != Vector3.zero)
            {
                playerObj.forward = Vector3.Slerp(playerObj.forward, inputDir.normalized, Time.deltaTime * rotSpeed);
            }
        }
    }
}
