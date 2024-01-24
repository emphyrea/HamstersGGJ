using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodeBouncer : MonoBehaviour
{
    [SerializeField] float bounceForce;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            other.GetComponent<Rigidbody>().AddForce(transform.up * bounceForce, ForceMode.Impulse);
            ThirdPersonCharacter playerScript = other.GetComponent<ThirdPersonCharacter>();
            playerScript.Explode();
        }
    }
}
