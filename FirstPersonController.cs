using UnityEngine;

public class FirstPersonController : MonoBehaviour
{
    public Camera playerCamera;
    public float reachDistance = 5f; // Дистанция, на которую можно сломать блок
    public LayerMask chunkLayerMask; // Слой, на котором висят чанки

    void Update()
    {
        // --- Передвижение (WASD) ---
        // Тут код движения transform.Translate(...)

        // --- Поворот камеры мышкой ---
        // Тут код вращения камеры по мышке

        // --- Взаимодействие (ЛКМ - сломать блок) ---
        if (Input.GetMouseButtonDown(0)) // Левая кнопка мыши
        {
            BreakBlock();
        }
    }

    void BreakBlock()
    {
        RaycastHit hit;
        // Пускаем луч из центра камеры
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, reachDistance, chunkLayerMask))
        {
            // Мы попали в чанк (в его MeshCollider)
            Chunk hitChunk = hit.collider.GetComponent<Chunk>();
            
            if (hitChunk != null)
            {
                // Конвертируем мировые координаты удара в локальные координаты чанка
                Vector3 localPoint = hitChunk.transform.InverseTransformPoint(hit.point);
                
                // Округляем, чтобы получить индекс блока (тут нужна небольшая коррекция в зависимости от стороны удара)
                int bx = Mathf.FloorToInt(localPoint.x + 0.5f); 
                int by = Mathf.FloorToInt(localPoint.y + 0.5f);
                int bz = Mathf.FloorToInt(localPoint.z + 0.5f);

                // Меняем тип блока на воздух (ломаем)
                if (bx >= 0 && bx < 16 && by >= 0 && by < 256 && bz >= 0 && bz < 16)
                {
                    hitChunk.blocks[bx, by, bz].type = BlockType.Air;
                    hitChunk.BuildMesh(); // ПЕРЕСТРАИВАЕМ чанк (это важно!)
                }
            }
        }
    }
}
