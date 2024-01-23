using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class CinematicTracker : MonoBehaviour
{
    //[SerializeField] int timesPlayedCinematic = 0;
    [SerializeField] PlayableDirector timeline;
  /*  public void SaveTimesCinematicPlayed()
    {
        this.timesPlayedCinematic++;
        PlayerPrefs.SetInt("Played",timesPlayedCinematic);
        PlayerPrefs.Save();
        Debug.Log("Times played saved!");
        Debug.Log(PlayerPrefs.GetInt("Played"));
    }
    public void ResetTimesCinematicPlayed()
    {
        this.timesPlayedCinematic = 0;
        PlayerPrefs.SetInt("Played", timesPlayedCinematic);
        PlayerPrefs.Save();
        Debug.Log("Times played saved!");
        Debug.Log(PlayerPrefs.GetInt("Played"));
    }*/

    public static Action<bool> OnCinematicFinish;


    /*private void OnEnable()
    {
        WinScreen.OnWinCinematicReset += ResetTimesCinematicPlayed;
    }
    private void OnDisable()
    {
        WinScreen.OnWinCinematicReset -= ResetTimesCinematicPlayed;
    }*/

    private void Start()
    {

        if(PlayerPrefs.GetInt("Died") >= 1)
        {
            timeline.Stop();
            OnCinematicFinish?.Invoke(true);
        }
        else if(PlayerPrefs.GetInt("Died") <= 0)
        {
            timeline.time = 0;
            timeline.Evaluate();
            timeline.Play();
            //OnCinematicFinish?.Invoke(true);
            if (timeline.time == timeline.playableAsset.duration)
            {             
                OnCinematicFinish?.Invoke(false);
                timeline.Stop();
            }
        }
    }
}
