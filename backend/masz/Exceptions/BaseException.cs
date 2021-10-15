#nullable enable

using System;

namespace masz.Exceptions
{
    public class BaseException : Exception
    {
        public BaseException(string? message) : base(message) { }
    }
}