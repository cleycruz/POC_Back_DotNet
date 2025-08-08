using System;

namespace CarritoComprasAPI.Core.Domain.Exceptions;

/// <summary>
/// Excepción base para errores del dominio
/// </summary>
public abstract class DomainException : Exception
{
    protected DomainException(string message) : base(message)
    {
    }

    protected DomainException(string message, Exception innerException) : base(message, innerException)
    {
    }
}

/// <summary>
/// Excepción para violaciones de reglas de negocio
/// </summary>
public class BusinessRuleViolationException : DomainException
{
    public BusinessRuleViolationException(string message) : base(message)
    {
    }

    public BusinessRuleViolationException(string message, Exception innerException) : base(message, innerException)
    {
    }
}

/// <summary>
/// Excepción para validaciones de Value Objects
/// </summary>
public class ValueObjectValidationException : DomainException
{
    public ValueObjectValidationException(string message) : base(message)
    {
    }

    public ValueObjectValidationException(string message, Exception innerException) : base(message, innerException)
    {
    }
}

/// <summary>
/// Excepción para entidades no encontradas
/// </summary>
public class EntityNotFoundException : DomainException
{
    public EntityNotFoundException(string entityName, object id) 
        : base($"La entidad '{entityName}' con ID '{id}' no fue encontrada.")
    {
    }

    public EntityNotFoundException(string message) : base(message)
    {
    }
}

/// <summary>
/// Excepción para conflictos de estado en entidades
/// </summary>
public class EntityStateConflictException : DomainException
{
    public EntityStateConflictException(string message) : base(message)
    {
    }

    public EntityStateConflictException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
