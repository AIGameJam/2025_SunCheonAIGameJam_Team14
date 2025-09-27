using UnityEngine;
using UnityEngine.EventSystems;

namespace LYJ
{
    public class OnNextButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public bool isRight;

        public void OnPointerEnter(PointerEventData eventData)
        {
            EncyclopediaManager.Instance.OnPoint(isRight);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            EncyclopediaManager.Instance.OnExit(isRight);
        }
    }
}
