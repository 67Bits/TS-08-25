using UnityEngine;

namespace SSBLevelProgression
{

    public class EventWatcherExample : MonoBehaviour
    {
        [SerializeField] private string _message;

        public void PrintMessage()
        {
            Debug.Log(_message);
        }
    }
}
