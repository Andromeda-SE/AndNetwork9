﻿namespace AndNetwork9.Shared.Enums
{
    public enum TaskStatus
    {
        Failed = -3,
        Rejected = -2,
        Canceled = -1,
        New,
        Analysis,
        ToDo,
        InProgress,
        Postponed,
        Resolved,
        Done,
    }
}