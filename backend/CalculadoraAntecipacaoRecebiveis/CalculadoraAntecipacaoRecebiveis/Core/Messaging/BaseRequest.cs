using FluentValidation.Results;
using MediatR;
using System.Text.Json.Serialization;

namespace CalculadoraAntecipacaoRecebiveis.Core.Messaging
{
    public abstract class BaseRequest<T> : IRequest<T> where T : Result
    {
        [JsonIgnore]
        public ValidationResult ValidationResult { get; protected set; }

        public abstract bool EhValido();
    }
}
