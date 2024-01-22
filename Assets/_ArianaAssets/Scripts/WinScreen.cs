using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class WinScreen : MonoBehaviour
{
    [SerializeField] GameObject winPanel;

    public static event Action OnWinCinematicReset;
    public static event Action OnWinDeathReset;

    // Start is called before the first frame update
    void Start()
    {
        winPanel.SetActive(false);
    }

    private void OnEnable()
    {
        ThirdPersonCharacter.OnWinning += OnWin;
    }
    private void OnDisable()
    {
        ThirdPersonCharacter.OnWinning -= OnWin;
    }


    public void OnWin()
    {
        winPanel.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        OnWinDeathReset?.Invoke();
        Time.timeScale = 0;

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
