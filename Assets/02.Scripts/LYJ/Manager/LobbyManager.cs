using UnityEngine;

namespace LJY
{
    public class LobbyManager : MonoBehaviour
    {
        private static LobbyManager instance;
        public static LobbyManager Instance { get { return instance; } }

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