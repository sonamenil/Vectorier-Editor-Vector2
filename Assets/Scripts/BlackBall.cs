using UnityEngine;

public class BlackBall : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public bool useType = true;
    public enum Type
    {
        Low,
        Hurdle,
        Medium,
        High
    
    }

    public Type type;

    public bool enableArea;

    public bool toFly;

    public GameObject ballActivatorTrigger;

    [Tooltip("If the laser has a delay when appearing")]
    public bool useGlobalTimer;

    [Tooltip("In seconds")]
    public float globalTimer;

}
