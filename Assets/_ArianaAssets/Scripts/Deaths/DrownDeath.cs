using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrownDeath : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            ThirdPersonMovement playerScript = other.GetComponent<ThirdPersonMovement>();
            playerScript.Drown();
        }
    }
}
