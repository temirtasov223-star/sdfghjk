using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour
{
    public Block[,,] blocks; // Массив блоков (данные)
    private MeshFilter meshFilter;
    private MeshCollider meshCollider;

    void Start()
    {
        meshFilter = GetComponent<MeshFilter>();
        meshCollider = GetComponent<MeshCollider>();
        GenerateChunkData(); // Заполняем массив данными (например, травой)
        BuildMesh();         // Строим физическую геометрию
    }

    void GenerateChunkData()
    {
        blocks = new Block[16, 256, 16]; // Размер чанка (x, y, z)
        for (int x = 0; x < 16; x++)
        {
            for (int z = 0; z < 16; z++)
            {
                // Простейшая генерация: уровень земли на высоте 50
                int groundHeight = 50; 
                for (int y = 0; y < 256; y++)
                {
                    Block block = new Block();
                    if (y < groundHeight)
                        block.type = BlockType.Stone; // Ниже земли - камень
                    else if (y == groundHeight)
                        block.type = BlockType.Grass; // На уровне земли - трава
                    else
                        block.type = BlockType.Air;   // Выше - воздух

                    blocks[x, y, z] = block;
                }
            }
        }
    }

    void BuildMesh()
    {
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Vector2> uv = new List<Vector2>();

        // Проходим по всем блокам в чанке
        for (int x = 0; x < 16; x++)
        {
            for (int y = 0; y < 256; y++)
            {
                for (int z = 0; z < 16; z++)
                {
                    // Если блок воздух - пропускаем (не рисуем)
                    if (blocks[x, y, z].type == BlockType.Air)
                        continue;

                    // Рисуем только грани, которые соседствуют с воздухом
                    // (Если соседний блок сверху - воздух, рисуем верхнюю грань)
                    if (IsBlockTransparent(x, y + 1, z)) // Верх
                        AddFace(vertices, triangles, uv, new Vector3(x, y, z), Direction.Up, blocks[x, y, z].type);
                    
                    if (IsBlockTransparent(x, y - 1, z)) // Низ
                        AddFace(vertices, triangles, uv, new Vector3(x, y, z), Direction.Down, blocks[x, y, z].type);
                    
                    // Аналогично для сторон (X+, X-, Z+, Z-)
                    // ...
                }
            }
        }

        // Применяем вершины и треугольники к мешу
        Mesh mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uv.ToArray();
        mesh.RecalculateNormals();

        meshFilter.mesh = mesh;
        meshCollider.sharedMesh = mesh; // Для коллизий
    }

    bool IsBlockTransparent(int x, int y, int z) {
        // Проверка выхода за границы чанка (нужна проверка соседних чанков, но тут упрощенно)
        if (x < 0 || x >= 16 || y < 0 || y >= 256 || z < 0 || z >= 16) return true; // За границей считаем воздухом
        return blocks[x, y, z].type == BlockType.Air;
    }

    // Метод AddFace (добавляет 4 вершины для квадрата) - техническая часть построения Mesh
    void AddFace(List<Vector3> verts, List<int> tris, List<Vector2> uv, Vector3 pos, Direction dir, BlockType type) { 
        // ... Код создания квадрата (Quads to Triangles) ...
        // Здесь мы добавляем вершины в зависимости от направления и текстурные координаты.
    }
}

enum Direction { Up, Down, North, East, South, West }
