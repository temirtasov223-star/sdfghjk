using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterPhysics : MonoBehaviour
{
    public World world;
    public float waterFlowSpeed = 0.5f; // Скорость распространения
    public int maxWaterDistance = 7; // Максимальное расстояние распространения
    
    private Queue<Vector3Int> waterQueue = new Queue<Vector3Int>();
    private HashSet<Vector3Int> waterProcessed = new HashSet<Vector3Int>();

    void Start()
    {
        InvokeRepeating(nameof(ProcessWaterFlow), 0, waterFlowSpeed);
    }

    public void AddWater(Vector3Int position)
    {
        waterQueue.Enqueue(position);
    }

    void ProcessWaterFlow()
    {
        int processCount = waterQueue.Count;
        
        for (int i = 0; i < processCount; i++)
        {
            if (waterQueue.Count == 0) break;
            
            Vector3Int pos = waterQueue.Dequeue();
            ProcessWaterAt(pos);
        }
        
        waterProcessed.Clear();
    }

    void ProcessWaterAt(Vector3Int pos)
    {
        // Проверяем, можно ли поставить воду
        BlockType currentBlock = world.GetBlock(pos.x, pos.y, pos.z);
        if (currentBlock != BlockType.Air) return;
        
        // Проверяем расстояние от источника
        float distanceFromSource = GetDistanceFromWaterSource(pos);
        if (distanceFromSource > maxWaterDistance) return;
        
        // Проверяем соседей
        CheckAndFlow(pos + Vector3Int.down); // Вниз
        CheckAndFlow(pos + Vector3Int.left); // Влево
        CheckAndFlow(pos + Vector3Int.right); // Вправо
        CheckAndFlow(pos + Vector3Int.forward); // Вперед
        CheckAndFlow(pos + Vector3Int.back); // Назад
    }

    void CheckAndFlow(Vector3Int neighborPos)
    {
        if (waterProcessed.Contains(neighborPos)) return;
        
        BlockType neighborBlock = world.GetBlock(neighborPos.x, neighborPos.y, neighborPos.z);
        
        if (neighborBlock == BlockType.Air)
        {
            // Ставим воду
            world.SetBlock(neighborPos.x, neighborPos.y, neighborPos.z, BlockType.Water);
            waterQueue.Enqueue(neighborPos);
            waterProcessed.Add(neighborPos);
        }
    }

    float GetDistanceFromWaterSource(Vector3Int pos)
    {
        // Простая эвристика - ищем ближайший источник воды
        float minDist = maxWaterDistance;
        
        for (int dx = -maxWaterDistance; dx <= maxWaterDistance; dx++)
        {
            for (int dy = -maxWaterDistance; dy <= maxWaterDistance; dy++)
            {
                for (int dz = -maxWaterDistance; dz <= maxWaterDistance; dz++)
                {
                    Vector3Int checkPos = new Vector3Int(pos.x + dx, pos.y + dy, pos.z + dz);
                    if (world.GetBlock(checkPos.x, checkPos.y, checkPos.z) == BlockType.WaterSource)
                    {
                        float dist = Vector3Int.Distance(pos, checkPos);
                        if (dist < minDist)
                            minDist = dist;
                    }
                }
            }
        }
        
        return minDist;
    }
}
