using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Vectorier/Object Reference")]
public class ObjectReference : MonoBehaviour
{   
    public enum Filename{
    triggers,
    traps_placeholder,
    traps,
    shadows,
    wall_props,
    bonus,
    obstacles,
    obstacles_moving,
    doors,
    doors_service
    }

    public Filename FileName;
    public bool useCustomVariables;
    [TextArea(5, 50)]
    public string CustomVariables =  @"<Static>
                     <OverrideVariable>
                        <Variable Name=""ImageName"" Value=""~ImageName"" />
                        <Variable Name=""Rarity"" Value=""~Rarity"" />
                     </OverrideVariable>
                  </Static>";
    
}