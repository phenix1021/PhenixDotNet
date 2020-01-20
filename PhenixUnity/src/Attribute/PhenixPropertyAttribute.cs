using System;
using UnityEngine;

namespace Phenix.Unity.Attribute
{
    [AttributeUsage(AttributeTargets.All)]
    public sealed class PhenixPropertyAttribute : PropertyAttribute
    {
        public int Flag { get; set; }

        public PhenixPropertyAttribute(int flag)
        {
            Flag = flag;
        }
    }
}