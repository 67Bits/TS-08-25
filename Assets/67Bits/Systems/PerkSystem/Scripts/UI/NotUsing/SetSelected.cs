using UnityEngine;
using UnityEngine.EventSystems;

namespace SSBPerks
{
    public class SetSelected : MonoBehaviour
    {
        public void SelectButton()
        {
            EventSystem.current.SetSelectedGameObject(gameObject);
        }
    }
}
