using System.Threading.Tasks;

namespace Laraue.Core.Mail
{   
    /// <summary>
    /// Each mail should implement this interface.
    /// </summary>
    public interface IMail
    {
        /// <summary>
        /// Mail subject.
        /// </summary>
        public string Subject { get; }

        /// <summary>
        /// Get mail body.
        /// </summary>
        /// <returns></returns>
        public Task<string> GetBodyAsync();
    }
}
