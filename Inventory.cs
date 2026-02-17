using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    public static Inventory Instance;
    
    public int selectedSlot = 0;
    public BlockType[] hotbar = new BlockType[9];
    public GameObject[] slotHighlights;
    public Image[] slotIcons;
    
    public Texture2D blockAtlas; // Твоя текстура с блоками

    void Awake()
    {
        Instance = this;
        
        // Заполняем хотбар стандартными блоками
        hotbar[0] = BlockType.Grass;
        hotbar[1] = BlockType.Dirt;
        hotbar[2] = BlockType.Stone;
        hotbar[3] = BlockType.Wood;
        hotbar[4] = BlockType.Leaves;
        hotbar[5] = BlockType.Grass; // Можно добавить новые типы
        hotbar[6] = BlockType.Stone;
        hotbar[7] = BlockType.Dirt;
        hotbar[8] = BlockType.Grass;
        
        UpdateHotbarUI();
    }

    void Update()
    {
        // Выбор слота колесиком мыши
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0)
        {
            if (scroll > 0)
                selectedSlot = (selectedSlot + 1) % hotbar.Length;
            else
                selectedSlot = (selectedSlot - 1 + hotbar.Length) % hotbar.Length;
            
            UpdateSelection();
        }

        // Выбор слота цифрами
        for (int i = 0; i < 9; i++)
        {
            if (Input.GetKeyDown((i + 1).ToString()))
            {
                selectedSlot = i;
                UpdateSelection();
            }
        }
    }

    void UpdateSelection()
    {
        for (int i = 0; i < slotHighlights.Length; i++)
        {
            slotHighlights[i].SetActive(i == selectedSlot);
        }
    }

    void UpdateHotbarUI()
    {
        for (int i = 0; i < hotbar.Length; i++)
        {
            // Рисуем иконку блока (нужно будет настроить)
            // Это упрощенно - лучше использовать спрайты
        }
    }

    public BlockType GetSelectedBlock()
    {
        return hotbar[selectedSlot];
    }
}
