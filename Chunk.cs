using UnityEngine;
using System.Collections.Generic;

public class Chunk : MonoBehaviour
{
    public BlockType[,,] blocks;
    public Vector2Int chunkPosition;
    public World world;
    
    private MeshFilter meshFilter;
    private MeshCollider meshCollider;
    private bool isDirty = false;

    void Awake()
    {
        meshFilter = GetComponent<MeshFilter>();
        meshCollider = GetComponent<MeshCollider>();
    }

    public void Initialize(Vector2Int position, World worldRef)
    {
        chunkPosition = position;
        world = worldRef;
        blocks = new BlockType[world.chunkSize, world.worldHeight, world.chunkSize];
        
        GenerateTerrain();
        BuildMesh();
    }

    void GenerateTerrain()
    {
        for (int x = 0; x < world.chunkSize; x++)
        {
            for (int z = 0; z < world.chunkSize; z++)
            {
                int worldX = x + chunkPosition.x * world.chunkSize;
                int worldZ = z + chunkPosition.y * world.chunkSize;
                
                // Используем шум Перлина для высоты
                float height = Mathf.PerlinNoise(worldX * 0.01f, worldZ * 0.01f) * 20 + 30;
                
                for (int y = 0; y < world.worldHeight; y++)
                {
                    if (y < height - 4)
                        blocks[x, y, z] = BlockType.Stone;
                    else if (y < height - 1)
                        blocks[x, y, z] = BlockType.Dirt;
                    else if (y < height)
                        blocks[x, y, z] = BlockType.Grass;
                    else
                        blocks[x, y, z] = BlockType.Air;
                }
            }
        }
    }

    public void BuildMesh()
    {
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Vector2> uv = new List<Vector2>();

        for (int x = 0; x < world.chunkSize; x++)
        {
            for (int y = 0; y < world.worldHeight; y++)
            {
                for (int z = 0; z < world.chunkSize; z++)
                {
                    if (blocks[x, y, z] == BlockType.Air)
                        continue;

                    // Проверяем все 6 сторон
                    AddFaceIfNeeded(x, y, z, x, y + 1, z, 1, vertices, triangles, uv); // Верх
                    AddFaceIfNeeded(x, y, z, x, y - 1, z, 0, vertices, triangles, uv); // Низ
                    AddFaceIfNeeded(x, y, z, x + 1, y, z, 2, vertices, triangles, uv); // Справа
                    AddFaceIfNeeded(x, y, z, x - 1, y, z, 3, vertices, triangles, uv); // Слева
                    AddFaceIfNeeded(x, y, z, x, y, z + 1, 4, vertices, triangles, uv); // Спереди
                    AddFaceIfNeeded(x, y, z, x, y, z - 1, 5, vertices, triangles, uv); // Сзади
                }
            }
        }

        Mesh mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uv.ToArray();
        mesh.RecalculateNormals();

        meshFilter.mesh = mesh;
        meshCollider.sharedMesh = mesh;
        isDirty = false;
    }

    void AddFaceIfNeeded(int x, int y, int z, int nx, int ny, int nz, int faceIndex, 
        List<Vector3> verts, List<int> tris, List<Vector2> uv)
    {
        // Проверяем соседний блок
        BlockType neighbor = GetNeighborBlock(nx, ny, nz);
        if (neighbor != BlockType.Air)
            return;

        // Добавляем грань
        BlockData blockData = new BlockData { type = blocks[x, y, z] };
        Vector2[] faceUVs = blockData.GetFaceUVs(faceIndex);

        int vertexCount = verts.Count;
        
        // 4 вершины квадрата
        verts.Add(new Vector3(x, y, z));
        verts.Add(new Vector3(x, y + 1, z));
        verts.Add(new Vector3(x + 1, y, z));
        verts.Add(new Vector3(x + 1, y + 1, z));

        // Треугольники (2 треугольника на квадрат)
        tris.Add(vertexCount);
        tris.Add(vertexCount + 1);
        tris.Add(vertexCount + 2);
        tris.Add(vertexCount + 1);
        tris.Add(vertexCount + 3);
        tris.Add(vertexCount + 2);

        // UV
        uv.Add(faceUVs[0]);
        uv.Add(faceUVs[1]);
        uv.Add(faceUVs[2]);
        uv.Add(faceUVs[3]);
    }

    BlockType GetNeighborBlock(int x, int y, int z)
    {
        if (y < 0 || y >= world.worldHeight)
            return BlockType.Air;

        if (x < 0 || x >= world.chunkSize || z < 0 || z >= world.chunkSize)
        {
            // Запрос к соседнему чанку
            int worldX = x + chunkPosition.x * world.chunkSize;
            int worldZ = z + chunkPosition.y * world.chunkSize;
            return world.GetBlock(worldX, y, worldZ);
        }

        return blocks[x, y, z];
    }

    public BlockType GetBlock(int x, int y, int z)
    {
        if (x < 0 || x >= world.chunkSize || y < 0 || y >= world.worldHeight || z < 0 || z >= world.chunkSize)
            return BlockType.Air;
        return blocks[x, y, z];
    }

    public void SetBlock(int x, int y, int z, BlockType type)
    {
        if (x >= 0 && x < world.chunkSize && y >= 0 && y < world.worldHeight && z >= 0 && z < world.chunkSize)
        {
            blocks[x, y, z] = type;
            isDirty = true;
            BuildMesh();
        }
    }
}
