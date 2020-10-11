using System.Threading.Tasks;
using RazorLight;

namespace Laraue.Core.Mail.Impl
{
    /// <summary>
    /// Each mail based on razor should implement this class.
    /// </summary>
    public abstract class RazorMail : IMail
    {
        /// <summary>
        /// Here is path to the file with view. For example, it should be looks like
        /// Company.Core.Mails.Views.HapyyBirtdayMail
        /// </summary>
        public abstract string ViewNamespace { get; }

        /// <inheritdoc />
        public abstract string Subject { get; }

        /// <inheritdoc />
        public Task<string> GetBodyAsync()
        {
            var engine = new RazorLightEngineBuilder()
                .UseEmbeddedResourcesProject(GetType().Assembly)
                .Build();

            return engine.CompileRenderAsync(ViewNamespace, this);
        }
    }
}
