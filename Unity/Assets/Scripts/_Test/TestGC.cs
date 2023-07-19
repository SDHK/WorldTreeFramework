using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;
using WorldTree;
using static UnityEngine.Rendering.DebugUI;

public class TestGC : MonoBehaviour
{
    public int y;
    public int dataY;
    public int byteIndex;
    
    public int notRemainderY;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        Profiler.BeginSample("SDHK");
        byteIndex = (y % 8); //求余数 
        notRemainderY = y + 1 - byteIndex; //y去余数

        dataY = (int)(notRemainderY * 0.125f); //得到y轴下标

        Profiler.EndSample();
    }
}