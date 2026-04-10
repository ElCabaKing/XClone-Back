namespace Domain.Exceptions;

public class BadConfigurationException(string message) : Exception(message)
{
}