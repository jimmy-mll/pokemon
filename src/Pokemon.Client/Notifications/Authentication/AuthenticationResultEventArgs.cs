using Pokemon.Protocol.Enums;
using System;

namespace Pokemon.Client.Notifications.Authentication;

public class AuthenticationResultEventArgs : EventArgs
{
    public bool IsSuccess { get; }
    public IdentificationFailureReasons? ErrorReason { get; }

    public AuthenticationResultEventArgs(bool isSuccess, IdentificationFailureReasons? errorReason = null)
    {
        IsSuccess = isSuccess;
        ErrorReason = errorReason;
    }
}
