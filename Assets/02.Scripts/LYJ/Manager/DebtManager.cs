using UnityEngine;

namespace LJY
{
    public class DebtManager : MonoBehaviour
    {
        private static DebtManager instance;
        public static DebtManager Instance { get { return instance; } }

        private void Awake()
        {
            if(instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
}