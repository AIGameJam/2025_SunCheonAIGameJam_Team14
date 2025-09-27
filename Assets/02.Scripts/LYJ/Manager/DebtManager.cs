using UnityEngine;
using UnityEngine.UI;

namespace LYJ
{
    public class DebtManager : MonoBehaviour
    {
        private static DebtManager instance;
        public static DebtManager Instance { get { return instance; } }

        private InputField input;
        private Button repairedButton;
        public Text debtText;
        public int repaidAmount;

        private void Awake()
        {
            if(instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;
            Init();
        }

        private void Init()
        {
            input = transform.GetChild(1).GetComponent<InputField>();
            input.onValueChanged.AddListener(delegate { InputMoney(); });
            repairedButton = transform.GetChild(2).GetComponent<Button>();
            repairedButton.onClick.AddListener(delegate { OnRepairedButton(); });
            debtText = transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Text>();
            LYJ.UIManager.Instance.SetDebuText(LYJ.GameManager.Instance.debt);
        }

        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.Escape))
            {
                repaidAmount = 0;
                input.text = repaidAmount.ToString();
                transform.gameObject.SetActive(false);
            }
        }

        public void InputMoney()
        {
            if(int.TryParse(input.text, out repaidAmount))
            {
                Debug.Log(repaidAmount);
            }
        }

        public void MoneyUpButton(int _amount)
        {
            if (repaidAmount + _amount > GameManager.Instance.money)
                return;

            repaidAmount += _amount;
            input.text = repaidAmount.ToString();
        }

        public void MoneyDownButton(int _amount)
        {
            if (repaidAmount - _amount < 0)
                return;

            repaidAmount -= _amount;
            input.text = repaidAmount.ToString();
        }

        public void OnRepairedButton()
        {
            LYJ.GameManager.Instance.money -= repaidAmount;
            LYJ.UIManager.Instance.SetMoneyText(LYJ.GameManager.Instance.money);
            LYJ.GameManager.Instance.debt -= repaidAmount;
            LYJ.UIManager.Instance.SetDebuText(LYJ.GameManager.Instance.debt);

            repaidAmount = 0;
            input.text = repaidAmount.ToString();
            transform.gameObject.SetActive(false);
        }
    }
}