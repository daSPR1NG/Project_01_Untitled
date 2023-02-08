using System;
using System.Collections.Generic;

namespace dnSR_Coding
{
    public interface IModifiableStatValue
    {
        public List<StatModifier> StatModifiers { get; set; }
    }
}