using System;
using UnityEngine;

namespace SSB.Debugger
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public class DebuggerAttribute : Attribute { }
}
