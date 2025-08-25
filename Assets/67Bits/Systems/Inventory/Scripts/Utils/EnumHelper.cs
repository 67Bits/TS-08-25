using System;
using UnityEngine;

static public class EnumHelper
{
    static public int Count<T>() where T : System.Enum
    {
        return Enum.GetValues(typeof(ItemID)).Length;
    }

    static public int Count<T>(this T enumItem) where T : System.Enum
    {
        return Enum.GetValues(typeof(ItemID)).Length;
    }
}
