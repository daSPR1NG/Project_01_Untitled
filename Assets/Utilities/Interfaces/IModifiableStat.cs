using System;
using System.Collections.Generic;

namespace dnSR_Coding
{
    public interface IModifiableStat
    {
        public List<StatModifier> StatModifiers { get; set; }
    }
}