using Laraue.Core.Exceptions.Web;

namespace Laraue.Core.DataAccess.Helpers
{
    public static class ObjectHelper
    {
        /// <summary>
        /// Throw an exception if value is null for object ot values is default for structs
        /// </summary>
        /// <param name="value"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="NotFoundException"></exception>
        public static T EnsureNotDefaultValue<T>(T value)
        {
            if (value is null)
            {
                throw new NotFoundException();
            }
            
            return value.Equals(default(T))
                ? throw new NotFoundException()
                : value;
        }
    }
}