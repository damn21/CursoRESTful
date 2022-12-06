using FluentValidation.Results;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Exceptions
{
    public class ValidationExceptions : Exception
    {
        public List<string> Errors { get; }
        public ValidationExceptions() : base("Se han producido uno o más errores de validación") 
        {
            Errors = new List<String>();
        }

        public ValidationExceptions(IEnumerable<ValidationFailure> failures) : this()
        {
            foreach (var failure in failures)
            {
                Errors.Add(failure.ErrorMessage);
            }

        }
    }
}
