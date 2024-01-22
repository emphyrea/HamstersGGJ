using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeathScreen : MonoBehaviour
{
    [SerializeField] GameObject deathPanel;

    // Start is called before the first frame update
    void Start()
    {
        deathPanel.SetActive(false);
    }

    private void OnEnable()
    {
        ThirdPersonCharacter.OnDeath += OnDeathScreen;
    }

    private void OnDisable()
    {
        ThirdPersonCharacter.OnDeath -= OnDeathScreen;
    }

    public void OnDeathScreen()
    {
        deathPanel.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
