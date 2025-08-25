using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Inventory
{
    /// <summary>
    /// An amount of a respective unit.
    /// </summary>
    [System.Serializable]
    public class Amount<T>
    {
        [SerializeField] private T _unit;
        [SerializeField] private int _quantity;

        public T Unit { get => _unit; }
        public int Quantity { get => _quantity; }

        public Amount(T unit, int quantity)
        {
            _unit = unit;
            _quantity = quantity;
        }

        #region Operators

        public static bool operator ==(Amount<T> a1, Amount<T> a2)
        {
            if (a1 is null) 
                return (a2 is null);
            
            if (/* a1 is not null && */ a2 is null)
                return false;

            if (!a1.Unit.Equals(a2.Unit))
                return false;

            return a1.Quantity == a2.Quantity;
        }

        public static bool operator !=(Amount<T> a1, Amount<T> a2)
        {
            return !(a1.Quantity == a2.Quantity);
        }

        public static Amount<T> operator +(Amount<T> a1, Amount<T> a2)
        {
            if (!a1.Unit.Equals(a2.Unit))
                throw new InvalidOperationException("Both Unit instances must be the same.");

            return new Amount<T>(a1.Unit, a1.Quantity + a2.Quantity);
        }

        public static Amount<T> operator -(Amount<T> a1, Amount<T> a2)
        {
            if (!a1.Unit.Equals(a2.Unit))
                throw new InvalidOperationException("Both Unit instances must be the same.");

            return new Amount<T>(a1.Unit, a1.Quantity - a2.Quantity);
        }

        public static Amount<T> operator +(Amount<T> amount, int integer)
            => new Amount<T>(amount.Unit, amount.Quantity + integer);

        public static Amount<T> operator -(Amount<T> amount, int integer)
            => new Amount<T>(amount.Unit, amount.Quantity - integer);

        public static Amount<T> operator *(Amount<T> amount, int integer)
            => new Amount<T>(amount.Unit, amount.Quantity * integer);

        public static Amount<T> operator /(Amount<T> amount, int integer)
            => new Amount<T>(amount.Unit, amount.Quantity / integer);

        #endregion

        #region Object overrides

        /// <summary>
        /// Value equality over reference equality check
        /// </summary>
        public override bool Equals(object other)
        {
            if (other.GetType() != typeof(Amount<T>)) return false;
            return (this == other);
        }

        public override string ToString()
        {
            return $"'{Quantity} {Unit.ToString()} amount'";
        }

        #endregion
    }
}