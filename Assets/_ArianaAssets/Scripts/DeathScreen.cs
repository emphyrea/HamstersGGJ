using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeathScreen : MonoBehaviour
{
    [SerializeField] GameObject deathPanel;

    [SerializeField] Image deathIcon;

    [SerializeField] Sprite electricIcon;
    [SerializeField] Sprite burnIcon;
    [SerializeField] Sprite drownIcon;
    [SerializeField] Sprite ghostIcon;
    [SerializeField] Sprite starveIcon;


    // Start is called before the first frame update
    void Start()
    {
        deathPanel.SetActive(false);
    }

    private void OnEnable()
    {
        ThirdPersonCharacter.OnDeath += OnDeathScreen;
        ThirdPersonCharacter.DeathIcon += ChooseDeathIcon;
    }

    public void ChooseDeathIcon(string choice)
    {
        switch(choice)
        {
            case "burn":
                deathIcon.sprite = burnIcon;
                break;
            case "electric":
                deathIcon.sprite = electricIcon;
                break;
            case "drown":
                deathIcon.sprite = drownIcon;
                break;
            case "starve":
                deathIcon.sprite = starveIcon;
                break;
            default:
                deathIcon.sprite = ghostIcon;
                break;
        }


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
