using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Laraue.Core.Testing.Asserts
{
    /// <summary>
    /// Extensions with asserts for <see cref="HttpResponseMessage"/>.
    /// </summary>
    public static class HttpResponseMessageAssertExtensions
    {
        /// <summary>
        /// Assert, that response has needed status code.
        /// Dump content message if status code is not suit.
        /// </summary>
        /// <param name="httpResponseMessage"></param>
        /// <param name="httpStatusCode"></param>
        public static async Task<HttpResponseMessage> AssertStatus(this Task<HttpResponseMessage> httpResponseMessageTask, HttpStatusCode httpStatusCode = HttpStatusCode.OK)
        {
            string newLine = Environment.NewLine;
            var httpResponseMessage = await httpResponseMessageTask;
            if (httpResponseMessage.StatusCode != httpStatusCode)
            {
                var contentBytes = httpResponseMessage.Content.ReadAsByteArrayAsync().GetAwaiter().GetResult();
                var content = Encoding.UTF8.GetString(contentBytes);
                var msgStringBuilder = new StringBuilder();
                msgStringBuilder.Append($"Response failed. Excepted {httpStatusCode}, but recieved {httpResponseMessage.StatusCode}");
                msgStringBuilder.Append(newLine)
                    .Append($"Request {httpResponseMessage.RequestMessage.Method}:{httpResponseMessage.RequestMessage.RequestUri}")
                    .Append(newLine);
                var sentContent = httpResponseMessage.RequestMessage.Content?.ReadAsStringAsync().GetAwaiter().GetResult();
                if (httpResponseMessage.RequestMessage.Headers.Count() != 0)
                {
                    msgStringBuilder.Append(newLine)
                        .Append($"Headers:");
                    foreach (var header in httpResponseMessage.RequestMessage.Headers)
                    {
                        msgStringBuilder.Append(newLine)
                            .Append($"{header.Key}: {string.Join(",", header.Value.ToArray())}");
                    }
                    msgStringBuilder.Append(newLine);
                }
                if (!string.IsNullOrEmpty(sentContent))
                {
                    msgStringBuilder.Append(newLine)
                        .Append($"Request data: {sentContent}")
                        .Append(newLine);
                }
                if (!string.IsNullOrEmpty(content))
                {
                    msgStringBuilder.Append(newLine)
                        .Append($"Response: {content}");
                }
                throw new Exception(msgStringBuilder.ToString());
            }
            return httpResponseMessage;
        }

        /// <summary>
        /// Assert, that response has needed status code.
        /// Dump content message if status code is not suit.
        /// Returns deserialized object if status is suit.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="httpResponseMessage"></param>
        /// <param name="httpStatusCode"></param>
        /// <returns></returns>
        public static async Task<T> AssertIs<T>(this Task<HttpResponseMessage> httpResponseMessage, HttpStatusCode httpStatusCode = HttpStatusCode.OK)
        {
            await httpResponseMessage.AssertStatus(httpStatusCode);
            var contentBytes = (await httpResponseMessage).Content.ReadAsByteArrayAsync().GetAwaiter().GetResult();
            dynamic content = Encoding.UTF8.GetString(contentBytes);
            return typeof(T) == typeof(string) ? content : JsonConvert.DeserializeObject<T>(content);
        }
    }
}