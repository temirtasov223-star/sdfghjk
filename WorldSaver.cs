using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

[System.Serializable]
public class SaveData
{
    public Vector2Int chunkPosition;
    public BlockType[] blocks; // Упрощаем до одномерного массива
}

public class WorldSaver : MonoBehaviour
{
    public World world;
    public string worldName = "MyWorld";
    
    string SavePath
    {
        get { return Application.persistentDataPath + "/" + worldName + "/"; }
    }

    public void SaveAllChunks()
    {
        if (!Directory.Exists(SavePath))
            Directory.CreateDirectory(SavePath);
        
        // Сохраняем каждый чанк отдельно
        foreach (var chunk in world.chunks.Values)
        {
            SaveChunk(chunk);
        }
        
        Debug.Log("Мир сохранен!");
    }

    void SaveChunk(Chunk chunk)
    {
        SaveData data = new SaveData();
        data.chunkPosition = chunk.chunkPosition;
        
        // Конвертируем 3D массив в 1D
        data.blocks = new BlockType[world.chunkSize * world.worldHeight * world.chunkSize];
        
        int index = 0;
        for (int x = 0; x < world.chunkSize; x++)
        {
            for (int y = 0; y < world.worldHeight; y++)
            {
                for (int z = 0; z < world.chunkSize; z++)
                {
                    data.blocks[index++] = chunk.blocks[x, y, z];
                }
            }
        }
        
        // Сохраняем в файл
        string filename = SavePath + "chunk_" + chunk.chunkPosition.x + "_" + chunk.chunkPosition.y + ".bin";
        
        BinaryFormatter formatter = new BinaryFormatter();
        using (FileStream stream = new FileStream(filename, FileMode.Create))
        {
            formatter.Serialize(stream, data);
        }
    }

    public void LoadAllChunks()
    {
        if (!Directory.Exists(SavePath))
        {
            Debug.Log("Нет сохранений");
            return;
        }
        
        string[] files = Directory.GetFiles(SavePath, "*.bin");
        foreach (string file in files)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            using (FileStream stream = new FileStream(file, FileMode.Open))
            {
                SaveData data = (SaveData)formatter.Deserialize(stream);
                LoadChunk(data);
            }
        }
        
        Debug.Log("Мир загружен!");
    }

    void LoadChunk(SaveData data)
    {
        // Создаем чанк если его нет
        if (!world.chunks.ContainsKey(data.chunkPosition))
        {
            world.CreateChunk(data.chunkPosition);
        }
        
        Chunk chunk = world.chunks[data.chunkPosition];
        
        // Загружаем блоки
        int index = 0;
        for (int x = 0; x < world.chunkSize; x++)
        {
            for (int y = 0; y < world.worldHeight; y++)
            {
                for (int z = 0; z < world.chunkSize; z++)
                {
                    chunk.blocks[x, y, z] = data.blocks[index++];
                }
            }
        }
        
        chunk.BuildMesh(); // Перестраиваем
    }
}
