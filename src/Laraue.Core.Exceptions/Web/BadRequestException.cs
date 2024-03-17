﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;

namespace Laraue.Core.Exceptions.Web
{
    /// <summary>
    /// Exception with 400 code and errors list.
    /// </summary>
    public class BadRequestException : HttpExceptionWithErrors
    {
        private const string ErrorMessage = "Bad request";

        public BadRequestException(string propertyKey, string error)
            : this(new Dictionary<string, string?[]>
            {
                [propertyKey] = [error]
            })
        {
        }
        
        public BadRequestException(IReadOnlyDictionary<string, string?[]> errors)
            : base(ErrorMessage, HttpStatusCode.BadRequest, errors)
        {
        }
        
        public BadRequestException(List<ValidationResult> errors)
            : base(ErrorMessage, HttpStatusCode.BadRequest)
        {
            var errorsDictionary = new Dictionary<string, List<string?>>();
            
            foreach (var error in errors)
            {
                foreach (var member in error.MemberNames)
                {
                    if (!errorsDictionary.ContainsKey(member))
                    {
                        errorsDictionary.Add(member, new List<string?>());
                    }
                    errorsDictionary[member].Add(error.ErrorMessage);
                }
            }

            Errors = errorsDictionary.ToDictionary(x => x.Key, x => x.Value.ToArray());
        }
    }
}