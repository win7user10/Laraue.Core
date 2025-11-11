using System;

namespace Laraue.Core.Ollama;

public class OllamaResponseException : Exception
{
    public OllamaResponseException(string message) : base(message) { }
}