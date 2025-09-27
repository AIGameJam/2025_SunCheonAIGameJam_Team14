using EnumTypes;
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

namespace LYJ
{
    public class GameManager : MonoBehaviour
    {
        private static GameManager instance;
        public static GameManager Instance { get { return instance; } }

        private int turn = 1;
        private int interestDDay = 13;
        private int waringCount = 0;

        public DayType day = DayType.LowTide;

        public int money = 100000;
        public int debt = 1000000;
        public int interest = 0;
        private const float interestRatio = 0.0001f;
        public int health = 100;

        private bool isGameOver = false;
        private bool isFirstLoad = true;

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;
            DontDestroyOnLoad(gameObject);
            Application.targetFrameRate = 60;

            InterestHandler();
            LYJ.UIManager.Instance.SetMoneyText(money);
        }

        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        // 이자 관리
        public  void InterestHandler()
        {
            interest += Mathf.RoundToInt(debt * interestRatio);
            UIManager.Instance.SetInterestText(interest, Mathf.RoundToInt(debt * interestRatio));
        }

        // 간조, 만조 이벤트 시작
        public void StartTideAction()
        {
            if(day == DayType.LowTide)
            {
                SceneManager.LoadScene(1);
            }
            else if(day == DayType.HighTide)
            {
                SceneManager.LoadScene(2);
            }
        }

        // 턴 넘어가기
        public void OnTurn()
        {           
            if(day == DayType.HighTide)
            {
                turn++;
                day = DayType.LowTide;
                UIManager.Instance.SetTideText("땅파러가기");

                if (interestDDay > 0)
                {
                    interestDDay--;
                    InterestHandler();
                }
                else if(interestDDay <= 0)
                {
                    if (money - interest < 0)
                    {
                        if (waringCount >= 1)
                            GameOver();
                        else
                        {
                            waringCount++;
                            interestDDay = 13;
                        }
                    }
                    else
                    {
                        Debug.Log(":: 이자 갚음 ::");
                        money -= interest;
                        interest = 0;
                        interestDDay = 13;
                        LYJ.UIManager.Instance.SetMoneyText(money);
                    }
                }
            }
            else
            {
                day = DayType.HighTide;
                UIManager.Instance.SetTideText("낚시하러가기");
            }

            UIManager.Instance.SetTurnText(turn);
            UIManager.Instance.SetInterestDDayText(interestDDay);
            UIManager.Instance.SetTideImage(day);
        }

        // 휴식하기
        public void OnRest()
        {
            UIManager.Instance.FadeInOut();

            if(health + 50 >= 100)
            {
                health = 100;
            }
            else
            {
                health += 50;
            }
        }

        public void GameOver()
        {
            isGameOver = true;
        }

        public void OnSceneLoaded(Scene _scene, LoadSceneMode _mode)
        {
            if (isFirstLoad)
            {
                // 첫 로드일 때는 실행 안 함
                isFirstLoad = false;
                return;
            }

            StartCoroutine(AfterSceneLoaded(_scene));
        }

        private IEnumerator AfterSceneLoaded(Scene _scene)
        {
            // 1프레임 기다려서 모든 Awake/Start 실행 후
            yield return null;

            if(_scene.name == "LobbyScene")
            {
                UIManager.Instance.Init();
                LYJ.UIManager.Instance.SetMoneyText(money);
                LYJ.UIManager.Instance.SetDebuText(debt);
                UIManager.Instance.SetInterestText(interest, Mathf.RoundToInt(debt * interestRatio));
                OnTurn();
            }
            else if (_scene.name == "LowTideScene")
            {
                Debug.Log(":: LowTide Scene ::");
                UIManager.Instance.LowTideSceneInit();
            }
            else if (_scene.name == "HighTideScene")
            {
                UIManager.Instance.HighTideSceneInit();
            }
        }
    }
}