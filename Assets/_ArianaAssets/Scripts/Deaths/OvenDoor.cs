using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OvenDoor : MonoBehaviour
{
    private float damp = 5;
    private GameObject player;
    private Quaternion defaultRotation;
    private Quaternion openRot;
    Transform trapTransform;
    private bool playerInTrigger = false;


    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        defaultRotation = transform.rotation;
        transform.Rotate(-130, 0, 0, 0);
        openRot = transform.rotation;
        trapTransform = transform;
    }

    private void Update()
    {
        if (playerInTrigger)
        {
            Close();
        }
        else
        {
            Open();
        }
    }

    private void Close()
    {
        transform.rotation = Quaternion.Slerp(transform.rotation, defaultRotation, Time.deltaTime * damp);
    }
    private void Open()
    {
        transform.rotation = Quaternion.Slerp(transform.rotation, openRot, Time.deltaTime * damp);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            playerInTrigger = true;

            other.transform.parent = trapTransform;

        }

    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            playerInTrigger = false;
            other.transform.parent = null;
            other.transform.localScale = Vector3.one;
        }

    }
}
