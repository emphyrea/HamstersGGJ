using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class CinematicTracker : MonoBehaviour
{

    [SerializeField] PlayableDirector timeline;

    public static Action<bool> OnCinematicFinish;

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
