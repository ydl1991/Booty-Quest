using System;
using UnityEngine;
 
public class EnumArrayAttribute : PropertyAttribute
{
    public readonly System.Type m_enumType;
    public EnumArrayAttribute(System.Type enumType) { m_enumType = enumType; }

    public string GetResult(int val)
    {
        return System.Enum.GetName(m_enumType, val);
    }
}