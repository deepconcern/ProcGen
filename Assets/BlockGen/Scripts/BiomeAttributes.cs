using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BiomeAttributes", menuName = "ProcGen/BlockGen/BiomeAttribute")]
public class BiomeAttributes : ScriptableObject
{
    public string BiomeName;

    public int SolidGroundHeight;
    public int TerrainHeight;
    public float TerrainScale;
}
