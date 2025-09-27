using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;
    private float moveSpeed = 6.0f;

    private GameObject inventoryBase;
    public DisplayController currentDisplay;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        inventoryBase = GameObject.Find("Inventory").transform.GetChild(0).gameObject;
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.RightArrow))
        {
            rb.MovePosition(rb.position + (Vector2.right * moveSpeed * Time.deltaTime));
        }
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            rb.MovePosition(rb.position + (Vector2.left * moveSpeed * Time.deltaTime));
        }
    }
    private void OnTriggerStay2D(Collider2D coll)
    {
        if (coll.CompareTag("Display"))
        {
            currentDisplay = coll.GetComponent<DisplayController>();

            if (Input.GetKeyDown(KeyCode.E))
            {
                Inventory.inventoryActivated = !Inventory.inventoryActivated;

                if (Inventory.inventoryActivated)
                    inventoryBase.SetActive(true);
                else
                    inventoryBase.SetActive(false);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D coll)
    {
        Debug.Log(":: Exit ::");
        currentDisplay = null;
        inventoryBase.SetActive(false);
    }
}
