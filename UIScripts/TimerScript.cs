using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class TimerScript : MonoBehaviour 
{
    public Text timerText;
    private float timeRemaining = 600f;

    private void Update() {
        if(timeRemaining > 0f) {
            timeRemaining -= Time.deltaTime;
            UpdateTimerText();
        }
        else {
            timeRemaining = 0f;
            // end game
        }
    }

    private void UpdateTimerText() {
        TimeSpan timeSpan = TimeSpan.FromSeconds(timeRemaining);
        timerText.text = string.Format("{0:D2}:{1:D2}", timeSpan.Minutes, timeSpan.Seconds);
    }
}