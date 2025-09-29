using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LYJ
{
    public class EncyclopediaManager : MonoBehaviour
    {
        private static EncyclopediaManager instance;
        public static EncyclopediaManager Instance { get { return instance; } }

        public List<ItemScriptableObject> encyclopediaList;

        private GameObject encyclopediaPhanel;
        private Image itemImage;
        private Text itemName;
        private Text habitatText;
        private Text descriptionText;

        private Button rightButton;
        private Button leftButton;
        private Image rightButtonImage;
        private Image leftButtonImage;

        private int currentIndex = 0;

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            Init();
        }

        public void Init()
        {
            encyclopediaPhanel = GameObject.Find("Canvas").transform.GetChild(2).gameObject;
            itemImage = encyclopediaPhanel.transform.GetChild(0).GetChild(0).GetComponent<Image>();
            itemName = itemImage.transform.GetChild(0).GetComponent<Text>();
            habitatText = encyclopediaPhanel.transform.GetChild(1).GetChild(0).GetComponent<Text>();
            descriptionText = encyclopediaPhanel.transform.GetChild(2).GetChild(0).GetComponent<Text>();

            rightButton = encyclopediaPhanel.transform.GetChild(3).GetComponent<Button>();
            leftButton = encyclopediaPhanel.transform.GetChild(4).GetComponent<Button>();
            rightButtonImage = rightButton.gameObject.GetComponent<Image>();
            leftButtonImage = leftButton.gameObject.GetComponent<Image>();

            rightButton.onClick.AddListener(delegate { OnNext(true); });
            leftButton.onClick.AddListener(delegate { OnNext(false); });

            if (encyclopediaList.Count <= 0)
                return;

            currentIndex = 0;
            itemImage.sprite = encyclopediaList[currentIndex].ItemImage;
            itemName.text = encyclopediaList[currentIndex].ItemName;
            habitatText.text = encyclopediaList[currentIndex].ItemHabitat;
            descriptionText.text = encyclopediaList[currentIndex].ItemDescription;
        }

        public void OnNext(bool _isRight)
        {
            if (_isRight)
            {
                if(currentIndex >= encyclopediaList.Count - 1)
                {
                    return;
                }
                else
                {
                    currentIndex++;
                    itemImage.sprite = encyclopediaList[currentIndex].ItemImage;
                    itemName.text = encyclopediaList[currentIndex].ItemName;
                    habitatText.text = encyclopediaList[currentIndex].ItemHabitat;
                    descriptionText.text = encyclopediaList[currentIndex].ItemDescription;
                }
            }
            else
            {
                if (currentIndex <= 0)
                {
                    return;
                }
                else
                {
                    currentIndex--;
                    itemImage.sprite = encyclopediaList[currentIndex].ItemImage;
                    itemName.text = encyclopediaList[currentIndex].ItemName;
                    habitatText.text = encyclopediaList[currentIndex].ItemHabitat;
                    descriptionText.text = encyclopediaList[currentIndex].ItemDescription;
                }
            }
        }

        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.Escape))
            {
                Init();
                encyclopediaPhanel.SetActive(false);
            }
        }

        public void OnPoint(bool _isRight)
        {
            if (_isRight)
                rightButtonImage.color = new Color(255, 255, 255, 255);
            else
                leftButtonImage.color = new Color(255, 255, 255, 255);
        }

        public void OnExit(bool _isRight)
        {
            if (_isRight)
                rightButtonImage.color = new Color(255, 255, 255, 0);
            else
                leftButtonImage.color = new Color(255, 255, 255, 0);
        }
    }
}
