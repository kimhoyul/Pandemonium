using System;
using System.Collections;
using System.Collections.Generic;
using BestHTTP.JSON.LitJson;
using UnityEngine;


public struct UID
{
    private long _value;

    public bool IsValid => _value != 0;

    public UID(string value)
    {
        if (!long.TryParse(value, out _value))
            _value = 0;
    }
    
    public static implicit operator long(UID uid)
    {
        return uid._value;
    }

    public static implicit operator JsonData(UID uid)
    {
        return uid.ToString();
    }

    public override string ToString() => _value.ToString();
}
