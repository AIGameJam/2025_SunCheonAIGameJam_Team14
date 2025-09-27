using UnityEngine;

public class Test : MonoBehaviour
{
    public Inventory inventory;
    public ItemScriptableObject[] items;

    public void GetTest()
    {
        inventory.AcquireItem(items[Random.Range(0, items.Length)]);
    }
}
