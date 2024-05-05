using System.Text.Json.Serialization;

namespace Sample.WebApplication.Models.OutputModels
{
    /// <summary>
    /// Class ResponseMessageOutputModel.
    /// </summary>
    public class ResponseMessageOutputModel
    {
        /// <summary>
        /// 訊息
        /// </summary>
        [JsonPropertyName("message")]
        public string Message { get; set; }
    }
}