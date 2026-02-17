using UnityEngine;
using System.Collections.Generic;

// Перечисление всех типов блоков
public enum BlockType 
{
    Air = 0,
    Grass = 1,
    Dirt = 2,
    Stone = 3,
    Wood = 4,
    Leaves = 5
}

[System.Serializable]
public class BlockData
{
    public BlockType type;
    
    // Текстуры для каждой стороны блока (координаты на атласе текстур)
    private static Dictionary<BlockType, Vector2[]> textureMap = new Dictionary<BlockType, Vector2[]>
    {
        { BlockType.Grass, new Vector2[] { 
            new Vector2(2, 0), // Низ (земля)
            new Vector2(0, 1), // Верх (трава)
            new Vector2(3, 0), // Сторона (земля с травой по бокам)
            new Vector2(3, 0), 
            new Vector2(3, 0), 
            new Vector2(3, 0) 
        }},
        { BlockType.Dirt, new Vector2[] { 
            new Vector2(2, 0), // Все стороны - земля
            new Vector2(2, 0),
            new Vector2(2, 0),
            new Vector2(2, 0),
            new Vector2(2, 0),
            new Vector2(2, 0)
        }},
        { BlockType.Stone, new Vector2[] { 
            new Vector2(1, 0), // Камень
            new Vector2(1, 0),
            new Vector2(1, 0),
            new Vector2(1, 0),
            new Vector2(1, 0),
            new Vector2(1, 0)
        }}
    };

    public bool IsSolid()
    {
        return type != BlockType.Air;
    }

    // Получить UV координаты для конкретной стороны
    public Vector2[] GetFaceUVs(int faceIndex)
    {
        if (textureMap.ContainsKey(type))
        {
            Vector2[] uvs = new Vector2[4];
            Vector2 uvOffset = textureMap[type][faceIndex];
            
            // 4 угла квадрата (0,0 - 1,1 в атласе)
            uvs[0] = new Vector2(0.125f * uvOffset.x, 0.125f * uvOffset.y);
            uvs[1] = new Vector2(0.125f * uvOffset.x, 0.125f * uvOffset.y + 0.125f);
            uvs[2] = new Vector2(0.125f * uvOffset.x + 0.125f, 0.125f * uvOffset.y);
            uvs[3] = new Vector2(0.125f * uvOffset.x + 0.125f, 0.125f * uvOffset.y + 0.125f);
            
            return uvs;
        }
        return new Vector2[4];
    }
}
