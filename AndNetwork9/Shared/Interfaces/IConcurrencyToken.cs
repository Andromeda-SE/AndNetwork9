using System;

namespace AndNetwork9.Shared.Interfaces;

public interface IConcurrencyToken : ILastChanged
{
    Guid ConcurrencyToken { get; set; }
}