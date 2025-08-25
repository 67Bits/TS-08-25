using System;
using UnityEngine;

namespace Modifiables
{
    /// <summary>
    /// Register modifications to be applied in a ModifiableValue
    /// </summary>
    /// <typeparam name="T">Type of value of the ModifiableValue</typeparam>
    [System.Serializable]
    public struct Modification<T> where T : struct
    {
        [SerializeField] private T _value;
        [SerializeField] private Operation _operation;

        public T Value { get => _value; }

        public Operation CurrentOperation { get => _operation; }

        public Modification(T value, Operation currentOperation)
        {
            _value = value;
            _operation = currentOperation;
        }
    }
}