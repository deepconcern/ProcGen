using UnityEngine;

public static class Noise
{
    public static float Get2dNoise(Vector2 pos, float offset, float scale)
    {
        return Mathf.PerlinNoise((pos.x + 0.1f) / VoxelData.ChunkWidth * scale + offset, (pos.y + 0.1f) / VoxelData.ChunkWidth * scale + offset);
        // return Mathf.PerlinNoise((pos.x / (VoxelData.ChunkWidth * scale)) + offset, (pos.y / (VoxelData.ChunkWidth * scale)) + offset);
    }

    public static bool Get3dNoise(Vector3 pos, float offset, float scale, float threshold)
    {
        return false;
    }
}
