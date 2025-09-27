using UnityEngine;

namespace LYJ
{
    public class GameManager : MonoBehaviour
    {
        private static GameManager instance;
        public static GameManager Instance { get { return instance; } }

        public int money = 0;
        public int health = 100;

        private void Awake()
        {
            if (instance != null)
            {
                Destroy(instance);
            }
            else
            {
                instance = this;
            }

            Application.targetFrameRate = 60;
        }
    }
}