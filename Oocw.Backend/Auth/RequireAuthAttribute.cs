using System;

namespace Oocw.Backend.Auth;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class RequireAuthAttribute : Attribute
{
    public string AccessType { get; }

    public RequireAuthAttribute(string accessType = "")
    {
        AccessType = accessType;
    }
}