using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Rename Voxel Data
public class BlockGen : MonoBehaviour
{
    private const int chunkSize = 256;
    private const int chunkDepth = 512;

    enum BlockType
    {
        Air,
        Ground,
    }

    BlockType GetBlock(int x, int y, int z)
    {
        var surfaceY = 100;

        return (y < surfaceY) ? BlockType.Ground : BlockType.Air;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
