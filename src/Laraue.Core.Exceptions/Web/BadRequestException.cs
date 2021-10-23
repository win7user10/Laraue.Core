using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Laraue.Core.Exceptions.Web
{
    /// <summary>
    /// Exception with 400 code
    /// </summary>
    public class BadRequestException : Exception
    {
        public IReadOnlyDictionary<string, string[]> Errors { get; }

        public BadRequestException(IReadOnlyDictionary<string, string[]> errors)
        {
            Errors = errors;
        }
        
        public BadRequestException(List<ValidationResult> errors)
        {
            var errorsDictionary = new Dictionary<string, List<string>>();
            
            foreach (var error in errors)
            {
                foreach (var member in error.MemberNames)
                {
                    if (!errorsDictionary.ContainsKey(member))
                    {
                        errorsDictionary.Add(member, new List<string>());
                    }
                    errorsDictionary[member].Add(error.ErrorMessage);
                }
            }

            Errors = errorsDictionary.ToDictionary(x => x.Key, x => x.Value.ToArray());
        }
    }
}