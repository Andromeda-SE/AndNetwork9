using System;

namespace And9.Lib.Models.Abstractions.Interfaces;

public interface IConcurrencyToken : ILastChanged
{
    Guid ConcurrencyToken { get; }
}