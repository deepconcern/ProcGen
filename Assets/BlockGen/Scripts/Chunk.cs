using System.Collections.Generic;
using UnityEngine;

public class Chunk
{
    private GameObject _chunkObject;
    private ChunkCoord _coord;
    private MeshRenderer _meshRenderer;
    private MeshFilter _meshFilter;
    private List<int> _triangles = new List<int>();
    private List<Vector2> _uvs = new List<Vector2>();
    private int _vertexIndex = 0;
    private List<Vector3> _vertices = new List<Vector3>();
    private byte[,,] _voxelMap = new byte[VoxelData.ChunkWidth, VoxelData.ChunkHeight, VoxelData.ChunkWidth];
    private World _world;

    public Chunk(ChunkCoord coord, World world)
    {
        _coord = coord;
        _world = world;
        _chunkObject = new GameObject();

        _meshFilter = _chunkObject.AddComponent<MeshFilter>();
        _meshRenderer = _chunkObject.AddComponent<MeshRenderer>();

        _meshRenderer.material = _world.Material;

        _chunkObject.name = $"Chunk({coord.X}, {coord.Z})";
        _chunkObject.transform.SetParent(_world.transform);
        _chunkObject.transform.position = new Vector3(coord.X * VoxelData.ChunkWidth, 0f, coord.Z * VoxelData.ChunkWidth);

        PopulateVoxelMap();
        CreateMeshData();
        CreateMesh();
    }

    public ChunkCoord Coord {
        get { return _coord; }
    }

    public bool IsActive
    {
        get { return _chunkObject.activeSelf; }
        set { _chunkObject.SetActive(value); }
    }

    public Vector3 Position
    {
        get { return _chunkObject.transform.position; }
    }

    private void AddTexture(int textureId)
    {
        float y = textureId / VoxelData.TextureAtlasSizeInBlocks;
        float x = textureId - (y * VoxelData.TextureAtlasSizeInBlocks);

        x *= VoxelData.NormalizedBlockTextureSize;
        y *= VoxelData.NormalizedBlockTextureSize;

        y = 1f - y - VoxelData.NormalizedBlockTextureSize;

        _uvs.Add(new Vector2(x, y));
        _uvs.Add(new Vector2(x, y + VoxelData.NormalizedBlockTextureSize));
        _uvs.Add(new Vector2(x + VoxelData.NormalizedBlockTextureSize, y));
        _uvs.Add(new Vector2(x + VoxelData.NormalizedBlockTextureSize, y + VoxelData.NormalizedBlockTextureSize));
    }

    private void AddVoxelDataToChunk(Vector3 pos)
    {
        for (int j = 0; j < VoxelData.VoxelTris.GetLength(0); j++)
        {
            if (!CheckVoxel(pos + VoxelData.FaceChecks[j]))
            {
                byte blockId = _voxelMap[(int)pos.x, (int)pos.y, (int)pos.z];

                for (int i = 0; i < VoxelData.VoxelTris.GetLength(1); i++)
                {
                    _vertices.Add(pos + VoxelData.VoxelVerts[VoxelData.VoxelTris[j, i]]);
                }

                AddTexture(_world.BlockTypes[blockId].GetTextureId(j));

                _triangles.Add(_vertexIndex);
                _triangles.Add(_vertexIndex + 1);
                _triangles.Add(_vertexIndex + 2);
                _triangles.Add(_vertexIndex + 2);
                _triangles.Add(_vertexIndex + 1);
                _triangles.Add(_vertexIndex + 3);

                _vertexIndex += 4;
            }
        }
    }

    private bool CheckVoxel(Vector3 pos)
    {
        int x = Mathf.FloorToInt(pos.x);
        int y = Mathf.FloorToInt(pos.y);
        int z = Mathf.FloorToInt(pos.z);

        if (!IsVoxelInChunk(x, y, z)) return _world.BlockTypes[_world.GetVoxel(pos + Position)].IsSolid;

        return _world.BlockTypes[_voxelMap[x, y, z]].IsSolid;
    }

    private void CreateMesh()
    {
        var mesh = new Mesh();

        mesh.vertices = _vertices.ToArray();
        mesh.triangles = _triangles.ToArray();
        mesh.uv = _uvs.ToArray();

        mesh.RecalculateNormals();

        _meshFilter.mesh = mesh;
    }

    private void CreateMeshData()
    {
        for (int y = 0; y < VoxelData.ChunkHeight; y++)
        {
            for (int x = 0; x < VoxelData.ChunkWidth; x++)
            {
                for (int z = 0; z < VoxelData.ChunkWidth; z++)
                {
                    if (_world.BlockTypes[_voxelMap[x, y, z]].IsSolid)
                        AddVoxelDataToChunk(new Vector3(x, y, z));
                }
            }
        }
    }

    private bool IsVoxelInChunk(int x, int y, int z)
    {
        if (x < 0 || x > VoxelData.ChunkWidth - 1) return false;
        if (y < 0 || y > VoxelData.ChunkHeight - 1) return false;
        if (z < 0 || z > VoxelData.ChunkWidth - 1) return false;

        return true;
    }

    private void PopulateVoxelMap()
    {
        for (int y = 0; y < VoxelData.ChunkHeight; y++)
        {
            for (int x = 0; x < VoxelData.ChunkWidth; x++)
            {
                for (int z = 0; z < VoxelData.ChunkWidth; z++)
                {
                    _voxelMap[x, y, z] = _world.GetVoxel(new Vector3(x, y, z) + Position);
                }
            }
        }
    }
}

public class ChunkCoord
{
    public int X;
    public int Z;

    public ChunkCoord(int x, int z)
    {
        X = x;
        Z = z;
    }

    public static bool operator ==(ChunkCoord a, ChunkCoord b)
    {
        return a.Equals(b);
    }

    public static bool operator !=(ChunkCoord a, ChunkCoord b)
    {
        return !(a == b);
    }

    public override int GetHashCode()
    {
        int hash = 27;

        hash = (13 * hash) + X.GetHashCode();
        hash = (13 * hash) + Z.GetHashCode();
        
        return hash;
    }

    public override bool Equals(System.Object o)
    {
        if (o == null) return false;

        if (this.GetType() != o.GetType()) return false;

        var other = o as ChunkCoord;

        return X == other.X && Z == other.Z;
    }
}