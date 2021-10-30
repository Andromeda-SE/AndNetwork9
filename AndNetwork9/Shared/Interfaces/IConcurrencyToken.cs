using System;

namespace AndNetwork9.Shared.Interfaces;

public interface IConcurrencyToken
{
    Guid ConcurrencyToken { get; set; }
}