using UnityEngine;

public class Test : MonoBehaviour
{
    public Inventory inventory;
    public ItemScriptableObject item;

    public void GetTest()
    {
        inventory.AcquireItem(item);
    }
}
