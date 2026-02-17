using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float walkSpeed = 5f;
    public float runSpeed = 10f;
    public float jumpForce = 5f;
    public float mouseSensitivity = 2f;
    public float reachDistance = 5f;
    
    public World world;
    public Transform cameraTransform;
    public LayerMask chunkLayer;
    
    private float verticalRotation = 0;
    private Rigidbody rb;
    private bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // Поворот камеры мышкой
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;
        
        transform.Rotate(Vector3.up * mouseX);
        
        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -90f, 90f);
        cameraTransform.localRotation = Quaternion.Euler(verticalRotation, 0, 0);

        // Проверка земли для прыжка
        isGrounded = Physics.Raycast(transform.position, Vector3.down, 1.1f);

        // Взаимодействие с блоками
        if (Input.GetMouseButtonDown(0)) // ЛКМ - сломать
        {
            TryBreakBlock();
        }
        
        if (Input.GetMouseButtonDown(1)) // ПКМ - поставить
        {
            TryPlaceBlock();
        }
    }

    void FixedUpdate()
    {
        // Движение
        float speed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;
        
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");
        
        Vector3 move = transform.right * moveX + transform.forward * moveZ;
        move = move.normalized * speed;
        
        // Сохраняем вертикальную скорость (гравитация)
        move.y = rb.velocity.y;
        rb.velocity = move;

        // Прыжок
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    void TryBreakBlock()
    {
        RaycastHit hit;
        if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit, reachDistance, chunkLayer))
        {
            Vector3 blockPos = hit.point - hit.normal * 0.1f;
            int x = Mathf.FloorToInt(blockPos.x);
            int y = Mathf.FloorToInt(blockPos.y);
            int z = Mathf.FloorToInt(blockPos.z);
            
            world.SetBlock(x, y, z, BlockType.Air);
        }
    }

    void TryPlaceBlock()
    {
        RaycastHit hit;
        if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit, reachDistance, chunkLayer))
        {
            Vector3 blockPos = hit.point + hit.normal * 0.1f;
            int x = Mathf.FloorToInt(blockPos.x);
            int y = Mathf.FloorToInt(blockPos.y);
            int z = Mathf.FloorToInt(blockPos.z);
            
            // Нельзя ставить блок в то место, где стоит игрок
            Vector3 playerBlockPos = new Vector3(
                Mathf.FloorToInt(transform.position.x),
                Mathf.FloorToInt(transform.position.y),
                Mathf.FloorToInt(transform.position.z)
            );
            
            if (new Vector3(x, y, z) != playerBlockPos)
            {
                world.SetBlock(x, y, z, BlockType.Stone); // Ставим камень для примера
            }
        }
    }
}
