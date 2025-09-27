using DG.Tweening;
using EnumTypes;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

namespace LYJ
{
    public class UIManager : MonoBehaviour
    {
        private static UIManager instance;
        public static UIManager Instance { get { return instance; } }

        public CanvasGroup canvasGroup;
        private GameObject canvas;
        private Image tideImage;
        public Sprite[] tideImages;

        private Slider healthSlider;

        private Text turnText;
        private Text interestDDayText;
        private Text moneyText;
        private Text DebuText;
        private Text interestText;

        private Button tideButton;
        private Text tideText;

        private Button restButton;

        private Button lowBackButton;
        private Button highBackButton;

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;
            DontDestroyOnLoad(gameObject);
            Init();
        }

        public void Init()
        {
            canvas = GameObject.Find("Canvas").gameObject;
            canvasGroup = canvas.GetComponent<CanvasGroup>();
            tideImage = canvas.transform.GetChild(0).GetChild(0).GetComponent<Image>();

            healthSlider = canvas.transform.GetChild(0).GetChild(6).GetComponent<Slider>();

            turnText = canvas.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Text>();
            interestDDayText = canvas.transform.GetChild(0).GetChild(0).GetChild(1).GetComponent<Text>();
            moneyText = canvas.transform.GetChild(0).GetChild(7).GetChild(0).GetComponent<Text>();
            DebuText = canvas.transform.GetChild(0).GetChild(8).GetChild(0).GetComponent<Text>();
            interestText = canvas.transform.GetChild(0).GetChild(8).GetChild(1).GetComponentInChildren<Text>();

            restButton = canvas.transform.GetChild(0).GetChild(2).GetComponent<Button>();
            restButton.onClick.AddListener(() => { LYJ.GameManager.Instance.OnRest(); });

            tideButton = canvas.transform.GetChild(0).GetChild(1).GetComponent<Button>();
            tideButton.onClick.AddListener(() => { LYJ.GameManager.Instance.StartTideAction(); });
            tideText = tideButton.GetComponentInChildren<Text>();
        }

        public void LowTideSceneInit()
        {
            lowBackButton = GameObject.Find("lowBackButton").GetComponent<Button>();
            lowBackButton.onClick.AddListener(() => {
                SceneManager.LoadScene(0);
            });
        }

        public void HighTideSceneInit()
        {
            lowBackButton = GameObject.Find("highBackButton").GetComponent<Button>();
            lowBackButton.onClick.AddListener(() => {
                SceneManager.LoadScene(0);
            });
        }

        public void SetTurnText(int _turn)
        {
            turnText.text = _turn.ToString();
        }

        public void SetInterestDDayText(int _dday)
        {
            interestDDayText.text = _dday.ToString();
        }

        public void SetMoneyText(int _money)
        {
            moneyText.text = string.Format("{0:N0} 원", _money);
        }

        public void SetDebuText(int _debu)
        {
            DebuText.text = string.Format("{0:N0} 원", _debu);
            DebtManager.Instance.debtText.text = string.Format("{0:N0} 원", _debu);
        }

        public void SetInterestText(int _currentInterest, int _nextInterest)
        {
            interestText.text = string.Format("{0:N0} 원 (+ {1:N0} 원)", _currentInterest, _nextInterest);
        }

        public void SetTideText(string _tide)
        {
            tideText.text = _tide;
        }

        public void SetTideImage(DayType _dayType)
        {
            tideImage.sprite = tideImages[(int)_dayType];
        }

        public void SetHealthSlider(int _health)
        {
            healthSlider.value = _health / 100;
        }

        public void FadeInOut()
        {
            StartCoroutine(Fade(false));
        }

        private IEnumerator Fade(bool isFadeIn)
        {
            canvasGroup.alpha = 1;
            Tween tween = canvasGroup.DOFade(0f, 1f);
            yield return tween.WaitForCompletion();
            canvasGroup.gameObject.SetActive(false);

            yield return new WaitForSeconds(3.0f);

            canvasGroup.alpha = 0;
            canvasGroup.gameObject.SetActive(true);
            canvasGroup.DOFade(1f, 1f);

            SetHealthSlider(LYJ.GameManager.Instance.health);
            LYJ.GameManager.Instance.OnTurn();
        }
    }
}