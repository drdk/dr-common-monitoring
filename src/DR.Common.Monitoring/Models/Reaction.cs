
namespace DR.Common.Monitoring.Models
{
    /// <summary>
    /// Callback action for a given health check. To be used in web dashboards. 
    /// </summary>
    public class Reaction
    {
        /// <summary>
        /// Callback address
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// HTTP Method to use, POST or GET
        /// </summary>
        public string Method { get; set; }

        /// <summary>
        /// Human readable description
        /// </summary>
        public string VisualDescription { get; set; }

        /// <summary>
        /// JSON formatted payload
        /// </summary>
        public string Payload { get; set; }
    }
}
