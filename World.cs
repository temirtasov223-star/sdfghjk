using UnityEngine;
using System.Collections.Generic;

public class World : MonoBehaviour
{
    public int chunkSize = 16;
    public int worldHeight = 64;
    public int viewDistance = 5; // Сколько чанков видно во все стороны
    
    public GameObject chunkPrefab;
    public Transform player;
    
    private Dictionary<Vector2Int, Chunk> chunks = new Dictionary<Vector2Int, Chunk>();
    private Vector2Int lastPlayerChunk;

    void Start()
    {
        GenerateInitialChunks();
    }

    void Update()
    {
        // Определяем, в каком чанке находится игрок
        Vector2Int currentChunk = new Vector2Int(
            Mathf.FloorToInt(player.position.x / chunkSize),
            Mathf.FloorToInt(player.position.z / chunkSize)
        );

        // Если игрок перешел в другой чанк - обновляем мир
        if (currentChunk != lastPlayerChunk)
        {
            UpdateChunksAroundPlayer(currentChunk);
            lastPlayerChunk = currentChunk;
        }
    }

    void GenerateInitialChunks()
    {
        Vector2Int center = Vector2Int.zero;
        for (int x = -viewDistance; x <= viewDistance; x++)
        {
            for (int z = -viewDistance; z <= viewDistance; z++)
            {
                Vector2Int chunkPos = new Vector2Int(center.x + x, center.y + z);
                CreateChunk(chunkPos);
            }
        }
    }

    void UpdateChunksAroundPlayer(Vector2Int playerChunk)
    {
        // Удаляем чанки, которые слишком далеко
        List<Vector2Int> chunksToRemove = new List<Vector2Int>();
        foreach (var chunk in chunks)
        {
            float distance = Vector2Int.Distance(chunk.Key, playerChunk);
            if (distance > viewDistance + 2)
            {
                chunksToRemove.Add(chunk.Key);
                Destroy(chunk.Value.gameObject);
            }
        }
        
        foreach (var pos in chunksToRemove)
        {
            chunks.Remove(pos);
        }

        // Создаем новые чанки вокруг игрока
        for (int x = -viewDistance; x <= viewDistance; x++)
        {
            for (int z = -viewDistance; z <= viewDistance; z++)
            {
                Vector2Int chunkPos = new Vector2Int(playerChunk.x + x, playerChunk.y + z);
                if (!chunks.ContainsKey(chunkPos))
                {
                    CreateChunk(chunkPos);
                }
            }
        }
    }

    void CreateChunk(Vector2Int position)
    {
        GameObject chunkObj = Instantiate(chunkPrefab, 
            new Vector3(position.x * chunkSize, 0, position.y * chunkSize), 
            Quaternion.identity, 
            transform);
        
        Chunk chunk = chunkObj.GetComponent<Chunk>();
        chunk.Initialize(position, this);
        chunks.Add(position, chunk);
    }

    public BlockType GetBlock(int x, int y, int z)
    {
        if (y < 0 || y >= worldHeight) return BlockType.Air;

        Vector2Int chunkPos = new Vector2Int(
            Mathf.FloorToInt(x / (float)chunkSize),
            Mathf.FloorToInt(z / (float)chunkSize)
        );

        if (chunks.ContainsKey(chunkPos))
        {
            int localX = x - chunkPos.x * chunkSize;
            int localZ = z - chunkPos.y * chunkSize;
            
            return chunks[chunkPos].GetBlock(localX, y, localZ);
        }

        return BlockType.Air;
    }

    public void SetBlock(int x, int y, int z, BlockType type)
    {
        Vector2Int chunkPos = new Vector2Int(
            Mathf.FloorToInt(x / (float)chunkSize),
            Mathf.FloorToInt(z / (float)chunkSize)
        );

        if (chunks.ContainsKey(chunkPos))
        {
            int localX = x - chunkPos.x * chunkSize;
            int localZ = z - chunkPos.y * chunkSize;
            
            chunks[chunkPos].SetBlock(localX, y, localZ, type);
        }
    }
}
