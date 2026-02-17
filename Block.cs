// Перечисление типов блоков
public enum BlockType { 
    Air,    // Невидимый
    Grass,  // Трава
    Dirt,   // Земля
    Stone   // Камень
}

[System.Serializable]
public class Block
{
    public BlockType type;

    // Проверка, является ли блок твердым (для коллизий и отрисовки)
    public bool IsSolid() {
        return type != BlockType.Air;
    }
}
