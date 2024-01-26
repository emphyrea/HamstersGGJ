using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class CinematicTracker : MonoBehaviour
{
    [SerializeField] int timesPlayedCinematic = 0;
    //[SerializeField] int timesPlayedCinematic = 0;
    [SerializeField] PlayableDirector timeline;

    public static Action<bool> OnCinematicFinish;


    private void Start()
    {

        if (PlayerPrefs.GetInt("Died") >= 1)
        {
            OnCinematicFinish?.Invoke(false);
            timeline.Stop();
            OnCinematicFinish?.Invoke(true);
            Debug.Log("died, invoke");
        }
        else if (PlayerPrefs.GetInt("Died") <= 0)
        {
            timeline.time = 0;
            timeline.Evaluate();
            timeline.Play();
            OnCinematicFinish?.Invoke(true);
            //OnCinematicFinish?.Invoke(true);
            if (timeline.time == timeline.playableAsset.duration)
            {
                {
                    OnCinematicFinish?.Invoke(false);
                    timeline.Stop();
                }
            }
        }
    }
}



