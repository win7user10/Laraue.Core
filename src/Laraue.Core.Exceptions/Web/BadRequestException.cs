using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;

namespace Laraue.Core.Exceptions.Web
{
    /// <summary>
    /// Exception with 400 code
    /// </summary>
    public class BadRequestException : HttpException
    {
        /// <summary>
        /// Errors dictionary
        /// </summary>
        public IReadOnlyDictionary<string, string[]> Errors { get; }

        private const string ErrorMessage = "Bad request";

        public BadRequestException(IReadOnlyDictionary<string, string[]> errors)
            : base(HttpStatusCode.BadRequest, ErrorMessage)
        {
            Errors = errors;
        }
        
        public BadRequestException(List<ValidationResult> errors)
            : base(HttpStatusCode.BadRequest, ErrorMessage)
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