using UnityEngine;

public class Panels : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public SpriteRenderer spriteRenderer;

    public bool useCustomVariables;
    [TextArea(5, 50)]

    [Tooltip("DO NOT put height and width values here")]
    public string CustomVariables = @"";
}
