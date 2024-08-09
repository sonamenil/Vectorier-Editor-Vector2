using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Vectorier/Animation Properties")]
public class AnimationProperties : MonoBehaviour
{

    public string Width;
    public string Height;
    [Tooltip("1 Is the Default")] public string Type = "1";
    public string Direction;
    public string Acceleration;
    public string ScaleX;
    public string ScaleY;
    [Tooltip("Time in second until the image disappear")] public string Time;
}
