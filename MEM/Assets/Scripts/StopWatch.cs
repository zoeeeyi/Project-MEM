using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StopWatch : MonoBehaviour
{
    TextMeshProUGUI stopWatchUI;
    float stopWatch;
    double stopWatchRounded;

    // Start is called before the first frame update
    void Start()
    {
        stopWatchUI = GetComponent<TextMeshProUGUI>();
        stopWatch = 0;
    }

    // Update is called once per frame
    void Update()
    {
        stopWatch += Time.deltaTime;
        stopWatchRounded = System.Math.Round(stopWatch, 2);
        stopWatchUI.text = stopWatchRounded.ToString();
    }
}
