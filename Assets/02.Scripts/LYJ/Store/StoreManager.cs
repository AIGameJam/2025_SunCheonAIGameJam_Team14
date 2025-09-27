using UnityEngine;

public class StoreManager : MonoBehaviour
{
    private float openTime = 30.0f;
    private float currentTime = 0;

    public bool isOpen = false;

    private void Start()
    {
        
    }

    private void Update()
    {
        if (isOpen)
        {
            OpenStore();
        }
    }

    public void IsOpen()
    {
        isOpen = true;
    }

    public void OpenStore()
    {
        if(currentTime < openTime)
        {
            currentTime += Time.deltaTime;
        }
        else
        {
            currentTime = 0;
            isOpen = false;
        }
    }
}
