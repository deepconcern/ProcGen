using System;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
    private List<ChunkCoord> _activeChunks = new List<ChunkCoord>();
    [SerializeField]
    private BiomeAttributes _biome;
    private Chunk[,] _chunks = new Chunk[VoxelData.WorldSizeInChunks, VoxelData.WorldSizeInChunks];
    private ChunkCoord _playerLastChunkCoord;
    [SerializeField]
    private int _seed;
    private Vector3 _spawnLocation;

    public BlockType[] BlockTypes;
    public Material Material;
    public Transform Player;

    public byte GetVoxel(Vector3 pos)
    {
        var y = Mathf.FloorToInt(pos.y);

        /* Immutable pass */
        
        // If outside world, return air.
        if (!IsVoxelInWorld(pos)) return 0;

        // If bottom block of chunk, return bedrock.
        if (y == 0) return 1;

        /* Basic terrain pass */

        var terrainHeight = Mathf.FloorToInt(_biome.TerrainHeight * Noise.Get2dNoise(new Vector2(pos.x, pos.z), 0, _biome.TerrainScale)) + _biome.SolidGroundHeight;

        if (y == terrainHeight) return 3;
        if (y < terrainHeight && y > terrainHeight - 4) return 6;
        if (y < terrainHeight) return 2;

        return 0;
    }

    public void Start()
    {
        UnityEngine.Random.InitState(_seed);
        _spawnLocation = new Vector3((VoxelData.WorldSizeInChunks * VoxelData.ChunkWidth) / 2f, VoxelData.ChunkHeight + 10, (VoxelData.WorldSizeInChunks * VoxelData.ChunkWidth) / 2f);

        GenerateWorld();

        _playerLastChunkCoord = GetChunkCoordFromVector3(Player.position);
    }

    // Update is called once per frame
    public void Update()
    {
        var playerCurrentChunkCoord = GetChunkCoordFromVector3(Player.position);

        if (playerCurrentChunkCoord != _playerLastChunkCoord)
        {
            CheckViewDistance();
            _playerLastChunkCoord = playerCurrentChunkCoord;
        }
    }

    private void CheckViewDistance()
    {
        var coord = GetChunkCoordFromVector3(Player.position);

        var previouslyActiveChunks = new List<ChunkCoord>(_activeChunks);

        for (int x = coord.X - VoxelData.ViewDistanceInChunks; x < coord.X + VoxelData.ViewDistanceInChunks; x++)
        {
            for (int z = coord.Z - VoxelData.ViewDistanceInChunks; z < coord.Z + VoxelData.ViewDistanceInChunks; z++)
            {
                var chunkCord = new ChunkCoord(x, z);
                if (IsChunkInWorld(chunkCord))
                {
                    if (_chunks[x, z] == null)
                    {
                        CreateChunk(x, z);
                    }
                    else if(!_chunks[x, z].IsActive)
                    {
                        var chunk = _chunks[x, z];
                        chunk.IsActive = true;
                        _activeChunks.Add(chunk.Coord);
                    }
                }

                for (int i = 0; i < previouslyActiveChunks.Count; i++)
                {
                    if (previouslyActiveChunks[i] == chunkCord)
                    {
                        previouslyActiveChunks.RemoveAt(i);
                    }
                }
            }
        }

        foreach (var c in previouslyActiveChunks)
        {
            _chunks[c.X, c.Z].IsActive = false;
        }
    }

    private void CreateChunk(int x, int z)
    {
        var chunkCord = new ChunkCoord(x, z);

        _chunks[x, z] = new Chunk(chunkCord, this);
        _activeChunks.Add(chunkCord);
    }

    private void GenerateWorld()
    {
        for (int x = (VoxelData.WorldSizeInChunks / 2) - VoxelData.ViewDistanceInChunks; x < (VoxelData.WorldSizeInChunks / 2) + VoxelData.ViewDistanceInChunks; x++)
        {
            for (int z = (VoxelData.WorldSizeInChunks / 2) - VoxelData.ViewDistanceInChunks; z < (VoxelData.WorldSizeInChunks / 2) + VoxelData.ViewDistanceInChunks; z++)
            {
                CreateChunk(x, z);
            }
        }

        Player.position = _spawnLocation;
    }

    ChunkCoord GetChunkCoordFromVector3(Vector3 pos)
    {
        int x = Mathf.FloorToInt(pos.x / VoxelData.ChunkWidth);
        int z = Mathf.FloorToInt(pos.z / VoxelData.ChunkWidth);

        return new ChunkCoord(x, z);
    }

    private bool IsChunkInWorld(ChunkCoord coord)
    {
        if (coord.X < 0 || coord.X >= VoxelData.WorldSizeInChunks) return false;
        if (coord.Z < 0 || coord.Z >= VoxelData.WorldSizeInChunks) return false;

        return true;
    }

    private bool IsVoxelInWorld(Vector3 pos)
    {
        if (pos.x < 0 || pos.x >= VoxelData.WorldSizeInVoxels) return false;
        if (pos.y < 0 || pos.y >= VoxelData.ChunkHeight) return false;
        if (pos.z < 0 || pos.z >= VoxelData.WorldSizeInVoxels) return false;

        return true;
    }
}

[Serializable]
public class BlockType
{
    public string Name;
    public bool IsSolid;

    [Header("Texture Values")]
    public int BackFaceTexture;
    public int BottomFaceTexture;
    public int FrontFaceTexture;
    public int LeftFaceTexture;
    public int RightFaceTexture;
    public int TopFaceTexture;

    public int GetTextureId(int faceIndex)
    {
        switch (faceIndex)
        {
            case 0:
                return BackFaceTexture;
            case 1:
                return FrontFaceTexture;
            case 2:
                return TopFaceTexture;
            case 3:
                return BottomFaceTexture;
            case 4:
                return LeftFaceTexture;
            case 5:
                return RightFaceTexture;
            default:
                throw new ArgumentException($"Invalid argument: {faceIndex}");
        }
    }
}
