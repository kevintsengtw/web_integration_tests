using System.Text;
using Newtonsoft.Json;

namespace Sample.WebApplicationIntegrationTests.Utilities
{
    /// <summary>
    /// Class RequestUtilities
    /// </summary>
    public class RequestUtilities
    {
        /// <summary>
        /// Convert To StringContent.
        /// </summary>
        /// <param name="parameter">parameter</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static StringContent ConvertToStringContent<T>(T parameter) where T : class
        {
            var result = ConvertToStringContent(parameter, Encoding.UTF8);
            return result;
        }

        /// <summary>
        /// Convert To StringContent
        /// </summary>
        /// <param name="parameter">parameter</param>
        /// <param name="encoding">encoding</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static StringContent ConvertToStringContent<T>(T parameter, Encoding encoding) where T : class
        {
            var result = ConvertToStringContent(parameter, encoding, "application/json");
            return result;
        }

        /// <summary>
        /// Convert To StringContent
        /// </summary>
        /// <param name="parameter">parameter</param>
        /// <param name="encoding">encoding</param>
        /// <param name="mediaType">mediaType</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static StringContent ConvertToStringContent<T>(T parameter, Encoding encoding, string mediaType) where T : class
        {
            var result = new StringContent(JsonConvert.SerializeObject(parameter), encoding, mediaType);
            return result;
        }
    }
}