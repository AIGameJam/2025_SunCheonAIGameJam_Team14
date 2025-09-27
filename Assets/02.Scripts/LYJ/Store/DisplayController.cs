using UnityEngine;
using UnityEngine.UI;

public class DisplayController : MonoBehaviour
{
    public SpriteRenderer displayImage;
    public Sprite[] displaySprites;

    public ItemScriptableObject item;
    public Image displayItemImage;

    private GameObject priceSettingPhanel;
    private Image priceSettingImage;
    private Text displayItemName;

    private void Awake()
    {
        displayImage = GetComponent<SpriteRenderer>();
    }

    public void OnSetActive()
    {
        displayImage.sprite = displaySprites[1];
    }

    public void OnDisplySetting()
    {
        priceSettingPhanel.SetActive(true);
        priceSettingImage.sprite = item.ItemImage;
        displayItemName.text = item.ItemName;

    }

    public void OnDisplay()
    {
        displayItemImage.color = new Color(255, 255, 255);
        displayItemImage.sprite = item.ItemImage;
    }
}
