using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SSB.Blink
{
    public class BlinkMaterial : MonoBehaviour
    {
        [Tooltip("Parent that will be blink"), SerializeField, Required]
        private GameObject _blinkParentTarget;
        [Tooltip("Value for repeat blink effect"), SerializeField]
        private int _repeatValue = 3;
        [Tooltip("Duration of blink in seconds"), SerializeField]
        private float _secondsDuration = .5f;
        [Tooltip("Reference of material to change during blink"), SerializeField, Required]
        private Material _blinkMaterial;

        [ReadOnly, SerializeField] private Renderer[] _meshs;

        private Dictionary<Renderer, Material[]> _originalDct = new();
        private Dictionary<Renderer, Material[]> _blinkDct = new();

        private void Awake()
        {
            FindMeshs();
        }
        private void Start()
        {
            SaveRendererMaterials();
        }
        private void SaveRendererMaterials()
        {
            if (_originalDct.Count == 0)
            {
                for (int i = 0; i < _meshs.Length; i++)
                {
                    Renderer mesh = _meshs[i];
                    _originalDct.Add(mesh, mesh.sharedMaterials);
                }
            }

            if (_blinkDct.Count == 0)
            {
                for (int i = 0; i < _meshs.Length; i++)
                {
                    Material[] mat = new Material[_meshs[i].sharedMaterials.Length];

                    for (int k = 0; k < mat.Length; k++)
                        mat[k] = _blinkMaterial;

                    _blinkDct.Add(_meshs[i], mat);
                }
            }
        }
        [Button("Blink", ButtonSizes.Gigantic)]
        public void Blink() => StartCoroutine(BlinkCoroutine());
        private IEnumerator BlinkCoroutine()
        {
            SaveRendererMaterials();
            for (int x = 0; x < _repeatValue; x++)
            {
                // Set blink
                for (int i = 0; i < _meshs.Length; i++)
                    _meshs[i].materials = _blinkDct[_meshs[i]];

                // Delay
                yield return new WaitForSeconds(_secondsDuration / 2);

                // Set original
                for (int i = 0; i < _meshs.Length; i++)
                    _meshs[i].materials = _originalDct[_meshs[i]];

                yield return new WaitForSeconds(_secondsDuration / 2);
            }
        }
        private void OnValidate() => FindMeshs();
        [Button]
        private void FindMeshs()
        {
            _meshs = _blinkParentTarget.GetComponentsInChildren<Renderer>(true).Where(renderer => !(renderer is ParticleSystemRenderer)).ToArray();
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
#endif
        }
    }
}