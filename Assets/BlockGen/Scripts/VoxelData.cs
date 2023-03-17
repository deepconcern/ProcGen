using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class VoxelData
{
    public const int ChunkHeight = 128;
    public const int ChunkWidth = 16;

    public static readonly Vector3[] FaceChecks = new Vector3[6] {
        new Vector3(0f, 0f,-1f), // Check back face
        new Vector3(0f, 0f, 1f), // Check front face
        new Vector3(0f, 1f, 0f), // Check top face
        new Vector3(0f, -1f, 0f), // Check bottom face
        new Vector3(-1f, 0f, 0f), // Check left face
        new Vector3(1f, 0f, 0f), // Check right face
    };

    public static float NormalizedBlockTextureSize
    {
        get { return 1f / (float)TextureAtlasSizeInBlocks; }
    }

    public const int TextureAtlasSizeInBlocks = 4;

    public const int ViewDistanceInChunks = 5;

    public static readonly Vector3[] VoxelVerts = new Vector3[8] {
        new Vector3(0f, 0f, 0f),
        new Vector3(1f, 0f, 0f),
        new Vector3(1f, 1f, 0f),
        new Vector3(0f, 1f, 0f),
        new Vector3(0f, 0f, 1f),
        new Vector3(1f, 0f, 1f),
        new Vector3(1f, 1f, 1f),
        new Vector3(0f, 1f, 1f),
    };

    public static readonly int[,] VoxelTris = new int[6, 4] {
        // 0 1 2 2 1 3 (pattern)
        {0, 3, 1, 2}, // Back Face
        {5, 6, 4, 7}, // Front Face
        {3, 7, 2, 6}, // Top Face
        {1, 5, 0, 4}, // Bottom Face
        {4, 7, 0, 3}, // Left Face
        {1, 2, 5, 6}, // Right Face
    };

    public static readonly Vector2[] VoxelUvs = new Vector2[4] {
        new Vector2(0f, 0f), // Bottom-left
        new Vector2(0f, 1f), // Top-left
        new Vector2(1f, 0f), // Bottom-right
        // new Vector2(1f, 0f), // Bottom-right
        // new Vector2(0f, 1f), // Top-left
        new Vector2(1f, 1f), // Top-right
    };

    public const int WorldSizeInChunks = 10;

    public static int WorldSizeInVoxels {
        get { return WorldSizeInChunks * ChunkWidth; }
    }
}
