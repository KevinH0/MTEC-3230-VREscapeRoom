using System;
using System.Collections.Generic;
using UnityEngine;

namespace JustAssets.Shared.Providers
{
    [Serializable]
    public class ValueOf<TValue, TThis> where TThis : ValueOf<TValue, TThis>, new()
    {
        [SerializeField]
        private TValue _value;

        protected virtual void Validate()
        {
        }

        public TValue Value
        {
            get => _value;
            protected set => _value = value;
        }

        public static TThis From(TValue item)
        {
            TThis x = new TThis();
            x.Value = item;
            x.Validate();

            return x;
        }

        protected virtual bool Equals(ValueOf<TValue, TThis> other)
        {
            return EqualityComparer<TValue>.Default.Equals(Value, other.Value);
        }

        public override bool Equals(object obj)
        {
            if (obj is null)
                return false;

            if (ReferenceEquals(this, obj))
                return true;

            return obj.GetType() == GetType() && Equals((ValueOf<TValue, TThis>)obj);
        }

        public override int GetHashCode()
        {
            return EqualityComparer<TValue>.Default.GetHashCode(Value);
        }

        public static bool operator ==(ValueOf<TValue, TThis> a, ValueOf<TValue, TThis> b)
        {
            if (a is null && b is null)
                return true;

            if (a is null || b is null)
                return false;

            return a.Equals(b);
        }

        public static bool operator !=(ValueOf<TValue, TThis> a, ValueOf<TValue, TThis> b)
        {
            return !(a == b);
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    }}
