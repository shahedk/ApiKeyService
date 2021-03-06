﻿using Microsoft.Extensions.Logging;

namespace ApiGateway.Common.Constants
{
    public class LogEvents
    {
        public static readonly EventId LoginSuccess = new EventId(1001, "Login successful");
        public static readonly EventId NewKeyCreated = new EventId(1002, "Key created");
        public static readonly EventId NewKeyUpdated = new EventId(1003, "Key updated");
        
        public static readonly EventId ApiKeyValidationPassed = new EventId(1004, "Key validation passed");
        public static readonly EventId ApiKeyValidationFailed = new EventId(4001, "Key validation failed");
    }
}