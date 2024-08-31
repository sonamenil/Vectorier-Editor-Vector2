using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Vectorier/DynamicVec2")]
public class DynamicVec2 : MonoBehaviour
{
    public string TransformationName = "Transform_name";

    [Serializable]
    public class UseCheck
    {
        public bool UseMovement1 = true;
        public bool UseMovement2 = false;
        public bool UseMovement3 = false;
        public bool UseMovement4 = false;
        public bool UseMovement5 = false;
        [Tooltip("Uses size interval instead of move interval")] public bool IsSizeInterval = false;
        [TextArea(5, 50)]
        public string matrixProperties =  @"<Matrix A=""0.001"" B=""0"" C=""0"" D=""0.001"" Tx=""0"" Ty=""0"" />";
    }
    

    [Serializable]
    public class Movement1
    {
        [Tooltip("Move Duration in Second")] public float MoveDuration = 1.5f;
        public bool UseDelay = false;
        [Tooltip("Move Delay in Second")] public float Delay = 0f;
        [Tooltip("Value should be half of the amount of Move X")] public float SupportXAxis = 0.0f;
        [Tooltip("Value should be half of the amount of Move Y")] public float SupportYAxis = 0.0f;
        [Tooltip("How much to move on X Axis")] public float MoveXAxis = 0.0f;
        [Tooltip("How much to move on Y Axis")] public float MoveYAxis = 0.0f;
        [Tooltip("How much to resize on X Axis")] public float ReziseXAxis = 0.0f;
        [Tooltip("How much to resize on Y Axis")] public float ReziseYAxis = 0.0f;
    }

    [Serializable]
    public class Movement2
    {
        [Tooltip("Move Duration in Second")] public float MoveDuration = 1.5f;
        public bool UseDelay = false;
        [Tooltip("Move Delay in Second")] public float Delay = 0f;
        [Tooltip("Value should be half of the amount of Move X")] public float SupportXAxis = 0.0f;
        [Tooltip("Value should be half of the amount of Move Y")] public float SupportYAxis = 0.0f;
        [Tooltip("How much to move on X Axis")] public float MoveXAxis = 0.0f;
        [Tooltip("How much to move on Y Axis")] public float MoveYAxis = 0.0f;
        [Tooltip("How much to resize on X Axis")] public float ReziseXAxis = 0.0f;
        [Tooltip("How much to resize on Y Axis")] public float ReziseYAxis = 0.0f;
    }


    [Serializable]
    public class Movement3
    {
        [Tooltip("Move Duration in Second")] public float MoveDuration = 1.5f;
        public bool UseDelay = false;
        [Tooltip("Move Delay in Second")] public float Delay = 0f;
        [Tooltip("Value should be half of the amount of Move X")] public float SupportXAxis = 0.0f;
        [Tooltip("Value should be half of the amount of Move Y")] public float SupportYAxis = 0.0f;
        [Tooltip("How much to move on X Axis")] public float MoveXAxis = 0.0f;
        [Tooltip("How much to move on Y Axis")] public float MoveYAxis = 0.0f;
        [Tooltip("How much to resize on X Axis")] public float ReziseXAxis = 0.0f;
        [Tooltip("How much to resize on Y Axis")] public float ReziseYAxis = 0.0f;
    }

    [Serializable]
    public class Movement4
    {
        [Tooltip("Move Duration in Second")] public float MoveDuration = 1.5f;
        public bool UseDelay = false;
        [Tooltip("Move Delay in Second")] public float Delay = 0f;
        [Tooltip("Value should be half of the amount of Move X")] public float SupportXAxis = 0.0f;
        [Tooltip("Value should be half of the amount of Move Y")] public float SupportYAxis = 0.0f;
        [Tooltip("How much to move on X Axis")] public float MoveXAxis = 0.0f;
        [Tooltip("How much to move on Y Axis")] public float MoveYAxis = 0.0f;
        [Tooltip("How much to resize on X Axis")] public float ReziseXAxis = 0.0f;
        [Tooltip("How much to resize on Y Axis")] public float ReziseYAxis = 0.0f;
    }

    [Serializable]
    public class Movement5
    {
        [Tooltip("Move Duration in Second")] public float MoveDuration = 1.5f;
        public bool UseDelay = false;
        [Tooltip("Move Delay in Second")] public float Delay = 0f;
        [Tooltip("Value should be half of the amount of Move X")] public float SupportXAxis = 0.0f;
        [Tooltip("Value should be half of the amount of Move Y")] public float SupportYAxis = 0.0f;
        [Tooltip("How much to move on X Axis")] public float MoveXAxis = 0.0f;
        [Tooltip("How much to move on Y Axis")] public float MoveYAxis = 0.0f;
        [Tooltip("How much to resize on X Axis")] public float ReziseXAxis = 0.0f;
        [Tooltip("How much to resize on Y Axis")] public float ReziseYAxis = 0.0f;
    }

    [SerializeField] public UseCheck MovementUsage;
    [SerializeField] public Movement1 MoveInterval1;
    [SerializeField] public Movement2 MoveInterval2;
    [SerializeField] public Movement3 MoveInterval3;
    [SerializeField] public Movement4 MoveInterval4;
    [SerializeField] public Movement5 MoveInterval5;
}
