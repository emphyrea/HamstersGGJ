using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.Playables;

public class Timer : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI timerText;
    [SerializeField] float countdown = 300;
    public static event Action OnTimeEnd;
    [SerializeField] bool isRunning = false;

    private void Start()
    {
        float min = Mathf.FloorToInt(countdown / 60);
        float sec = Mathf.FloorToInt(countdown % 60);
        timerText.text = string.Format("{0,00}:{1,00}", min, sec);
    }

    private void OnEnable()
    {
       // CinematicTracker.OnCinematicFinish += _ => SetRunning(true);
        ThirdPersonCharacter.OnDeath += IsDead;
    }

    private void OnDisable()
    {
        //CinematicTracker.OnCinematicFinish -= _ => SetRunning(true);
        ThirdPersonCharacter.OnDeath -= IsDead;
    }

    public void IsDead()
    {
        this.isRunning = false;
    }
    void Update()
    {
        if(isRunning)
        {
            if (countdown > 0)
            {
                countdown -= Time.deltaTime;
            }
            else
            {
                countdown = 0;
                OnTimeEnd?.Invoke();
            }
            float min = Mathf.FloorToInt(countdown / 60);
            float sec = Mathf.FloorToInt(countdown % 60);
            timerText.text = string.Format("{0,00}:{1,00}", min, sec);
        }

    }

    public void SetRunning(bool set)
    {
        isRunning = set;
    }

}
