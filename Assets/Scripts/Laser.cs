using UnityEngine;

public class Laser : MonoBehaviour
{
    public SpriteRenderer laser;

    public GameObject laserActivatorTrigger;

    [Tooltip("If the laser has a delay when appearing")]
    public bool useGlobalTimer;

    [Tooltip("In seconds")]
    public float globalTimer;

    public bool allowFlame = true;

    public bool allowTesla = true;

    public bool allowLaser = true;
}
