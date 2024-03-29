using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public class FPSSetter : MonoBehaviour
{
    const int FPS = 60;
    void Start()
    {
        Application.targetFrameRate=FPS;

    }

}
