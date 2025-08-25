using System;
using System.Collections.Generic;
using UnityEngine;

namespace Modifiables
{
    /// <summary>
    /// Used to any attribute that can receive temporary modification, 
    /// </summary>
    /// <example>
    /// like temporary upgrades over max life.
    /// </example>
    /// <typeparam name="T">_value type</typeparam>
    public class ModifiableValue<T> where T : struct, IConvertible
    {
        private readonly Dictionary<object, Modification<T>> _modifications = new();
        
        private T _baseValue;
        private T _modifiedValue = default;

        public T Value
        {
            get => _modifiedValue;
        }

        public ModifiableValue(T value)
        {
            _modifiedValue = _baseValue = value;
        }

        #region Base Value

        public void SetBaseValue(T value)
        {
            _baseValue = value;
            UpdateValue();
        }

        public void UpdateValue()
        {
            _modifiedValue = _baseValue;
            ApplyAllModifications();
        }

        #endregion

        #region Apply Modification

        private void ApplyModification(T value, Operation operation)
        {
            var currentValue = Convert.ToSingle(_modifiedValue);
            var modificationValue = Convert.ToSingle(value);

            switch (operation)
            {
                case Operation.Add:
                    _modifiedValue = (T)Convert.ChangeType(currentValue + modificationValue, typeof(T));
                    break;
                case Operation.Subtract:
                    _modifiedValue = (T)Convert.ChangeType(currentValue - modificationValue, typeof(T));
                    break;
                case Operation.Multiply:
                    _modifiedValue = (T)Convert.ChangeType(currentValue * modificationValue, typeof(T));
                    break;
                case Operation.Divide:
                    _modifiedValue = (T)Convert.ChangeType(currentValue / modificationValue, typeof(T));
                    break;
                case Operation.AddPercent:
                    _modifiedValue = (T)Convert.ChangeType(currentValue * (1f + modificationValue * 0.01f), typeof(T));
                    break;
                case Operation.SubtractPercent:
                    _modifiedValue = (T)Convert.ChangeType(currentValue * (1f - modificationValue * 0.01f), typeof(T));
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void ApplyModification(Modification<T> modification) 
            => ApplyModification(modification.Value, modification.CurrentOperation);

        private void ApplyAllModifications()
        {
            foreach (var modification in _modifications.Values)
                ApplyModification(modification);
        }

        #endregion

        #region Get, Add and Remove Modifications

        /// <summary>
        /// Get existing value, or default if ID is not found.
        /// </summary>
        /// <param name="id">The object id passed when the modification was applied.</returns>
        public Modification<T> GetModification(object id) 
            => _modifications.GetValueOrDefault(id);

        /// <summary>
        /// Add Modification in the list and recalculate values.
        /// </summary>
        /// <param name="id">The object id used as Key to get modification.</returns>
        /// <param name="modification">The modification applied.</param>
        public void AddModification(object id, Modification<T> modification)
        {
           if (_modifications.ContainsKey(id)) 
                RemoveModification(id);
            
            ApplyModification(modification);
            _modifications.Add(id, modification);
            
#if UNITY_EDITOR
            if (_modifiedValue is < 0.01f) 
                Debug.LogError($"AddModification: Value < 0.01f! Id = {id}");
#endif
        }

        /// <summary>
        /// Remove Modification from the list and recalculate values.
        /// </summary>
        /// <param name="id">The object id used as Key to get modification.</returns>
        /// <param name="modification">The modification applied.</param>
        public void RemoveModification(object id)
        {
            if (_modifications.Remove(id))
            {
                UpdateValue();
            }
        }

        /// <summary>
        /// Remove all modifications from the list and reset initial value.
        /// </summary>
        /// <param name="id">The object id used as Key to get modification.</returns>
        /// <param name="modification">The modification applied.</param>
        public void RemoveAllModifications()
        {
            _modifications.Clear();
            UpdateValue();
        }

        #endregion
    }
}
