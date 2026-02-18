using UnityEngine;

public static class BiomeGenerator
{
    public static BlockType GetBlock(float worldX, float worldY, float worldZ)
    {
        float height = worldY;
        
        // Генерация биомов на основе температуры и влажности
        float temperature = Mathf.PerlinNoise(worldX * 0.001f, worldZ * 0.001f);
        float humidity = Mathf.PerlinNoise(worldX * 0.001f + 1000, worldZ * 0.001f + 1000);
        
        // Основная высота ландшафта
        float terrainHeight = Mathf.PerlinNoise(worldX * 0.01f, worldZ * 0.01f) * 20;
        terrainHeight += Mathf.PerlinNoise(worldX * 0.05f, worldZ * 0.05f) * 5;
        terrainHeight += 30; // Базовый уровень
        
        int groundLevel = Mathf.FloorToInt(terrainHeight);
        
        // Определяем тип блока
        if (height < groundLevel - 5)
            return BlockType.Stone;
        else if (height < groundLevel - 1)
            return BlockType.Dirt;
        else if (height < groundLevel)
        {
            // Выбираем тип травы в зависимости от биома
            if (temperature > 0.6f && humidity > 0.5f)
                return BlockType.Grass; // Джунгли (зеленая трава)
            else if (temperature < 0.3f)
                return BlockType.Dirt; // Тундра (замерзшая)
            else
                return BlockType.Grass; // Обычная трава
        }
        
        return BlockType.Air;
    }

    // Генерация деревьев
    public static void GenerateTree(BlockType[,,] blocks, int x, int groundY, int z)
    {
        int treeHeight = Random.Range(4, 7);
        
        // Ствол
        for (int y = 1; y <= treeHeight; y++)
        {
            if (groundY + y < blocks.GetLength(1))
                blocks[x, groundY + y, z] = BlockType.Wood;
        }
        
        // Листва
        for (int ly = -2; ly <= 1; ly++)
        {
            for (int lx = -2; lx <= 2; lx++)
            {
                for (int lz = -2; lz <= 2; lz++)
                {
                    if (Mathf.Abs(lx) + Mathf.Abs(lz) <= 3)
                    {
                        int bx = x + lx;
                        int bz = z + lz;
                        int by = groundY + treeHeight + ly;
                        
                        if (bx >= 0 && bx < blocks.GetLength(0) && 
                            bz >= 0 && bz < blocks.GetLength(2) &&
                            by >= 0 && by < blocks.GetLength(1))
                        {
                            if (blocks[bx, by, bz] == BlockType.Air)
                                blocks[bx, by, bz] = BlockType.Leaves;
                        }
                    }
                }
            }
        }
    }
}
