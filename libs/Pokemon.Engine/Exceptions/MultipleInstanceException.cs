using System;

namespace Pokemon.Engine.Exceptions;

public class MultipleInstanceException : Exception
{
    public MultipleInstanceException(string className) : base($"Two instance of '{className}' has been created.") { }
}
