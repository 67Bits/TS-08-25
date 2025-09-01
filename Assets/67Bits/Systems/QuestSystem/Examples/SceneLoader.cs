using UnityEngine;
using UnityEngine.SceneManagement;

namespace SSB.Quests.Examples
{
    public class SceneLoader : MonoBehaviour
    {
        [SerializeField] private int sceneIndex;
        private bool _isLoaded = false;

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out PlayerController player))
            {
                ChangeToAddictiveScene();
            }
        }
        public void ChangeToAddictiveScene()
        {
            if (_isLoaded)
                return;
            _isLoaded = true;
            SceneManager.LoadSceneAsync(sceneIndex, LoadSceneMode.Additive);
        }

        public void LoadScene()
        {
            SceneManager.LoadScene(sceneIndex);
        }
    }
}