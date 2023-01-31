using System;
using System.Collections.Generic;

namespace dnSR_Coding
{
    public interface IModifiableStatValue<T> 
    {
        public List<StatModifier> StatModifiers { get; set; }
        public Action<T> OnStatValueModified { get; set; }
    }
}