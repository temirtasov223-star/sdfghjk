using UnityEngine;

public class BlockLighting : MonoBehaviour
{
    public World world;
    public float lightDecay = 0.8f; // Как быстро затухает свет
    
    private byte[,,] lightLevels; // 0-15 (как в Minecraft)
    
    void Start()
    {
        lightLevels = new byte[world.chunkSize, world.worldHeight, world.chunkSize];
        CalculateLighting();
    }

    void CalculateLighting()
    {
        // Сначала ставим максимальный свет наверху (солнце)
        for (int x = 0; x < world.chunkSize; x++)
        {
            for (int z = 0; z < world.chunkSize; z++)
            {
                PropagateSunlight(x, world.worldHeight - 1, z, 15);
            }
        }
    }

    void PropagateSunlight(int x, int y, int z, byte light)
    {
        if (y < 0 || y >= world.worldHeight) return;
        if (light <= 1) return;
        
        BlockType block = world.GetBlock(x, y, z);
        if (block != BlockType.Air)
        {
            // Непрозрачные блоки останавливают свет
            return;
        }
        
        byte newLight = (byte)(light - 1);
        
        // Распространяем во все стороны
        PropagateSunlight(x + 1, y, z, newLight);
        PropagateSunlight(x - 1, y, z, newLight);
        PropagateSunlight(x, y + 1, z, newLight);
        PropagateSunlight(x, y - 1, z, newLight);
        PropagateSunlight(x, y, z + 1, newLight);
        PropagateSunlight(x, y, z - 1, newLight);
    }
}
