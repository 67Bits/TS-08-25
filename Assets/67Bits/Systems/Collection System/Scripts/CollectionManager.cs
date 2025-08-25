using Bello.Unity;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SSB.CollectionUI
{
    public class CollectionManager : MonoBehaviour
    {
        public static CollectionManager Instance;
        [SerializeField] private Transform _colletionButtonsParent;
        [SerializeField] private CollectionButton _buttonPrefab;
        [ReadOnly, SerializeField] private List<CollectionData> _colletionDatas = new();
        private void Awake()
        {
            Instance = this;
            Initialize();
        }
        public void Initialize()
        {
            _colletionButtonsParent.DestroyAllChildern();
            _colletionDatas = GetData();
            for (int i = 0; i < _colletionDatas.Count; i++)
            {
                CollectionData colletionData = _colletionDatas[i];
                var newButton = Instantiate(_buttonPrefab, _colletionButtonsParent);
                newButton.Initialize(colletionData);
            }
        }
        public virtual List<CollectionData> GetData()
        {
            return Resources.LoadAll<CollectionData>("").ToList();
        }
    }
}
