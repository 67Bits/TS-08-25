using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SSB.Quests
{
    public class GameReferences : Singleton<GameReferences>
    {
        public static CinemachineCamera CinemachineCamera
        {
            get
            {
                if (!_cinemachineCamera) _cinemachineCamera = FindFirstObjectByType<CinemachineCamera>();
                return _cinemachineCamera;
            }
            set { _cinemachineCamera = value; }
        }
        private static CinemachineCamera _cinemachineCamera;
        public static Transform PlayerTransform
        {
            get
            {
                if (!_playerTransform) _playerTransform = GameObject.FindWithTag("Player").transform;
                return _playerTransform;
            }
            set { _playerTransform = value; }
        }
        private static Transform _playerTransform;
    }
}
