using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class RequestValidatorAttribute : Attribute
{
    public Type RequestType { get; }

    public RequestValidatorAttribute(Type requestTypeIn)
    {
        RequestType = requestTypeIn;
    }
}

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class StateUpdateAttribute : Attribute
{
    public Type UpdateType { get; }

    public StateUpdateAttribute(Type updateTypeIn)
    {
        UpdateType = updateTypeIn;
    }
}
