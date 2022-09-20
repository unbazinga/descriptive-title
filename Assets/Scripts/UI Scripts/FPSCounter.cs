using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FPSCounter : MonoBehaviour
{
    public TMP_Text fpsText;

    private float timeLapse;

    public float timer, refresh, avgFps;
    // Update is called once per frame
    void Update()
    {
        timeLapse = Time.smoothDeltaTime;
        timer = timer <= 0 ? refresh : timer -= timeLapse;
        if (timer <= 0) avgFps = (int) (1f / timeLapse);
        fpsText.text = "FPS: " + avgFps;
    }
}
