#pragma warning disable 1591
namespace DR.Common.Monitoring.Models
{
    /// <summary>
    /// Classification of importance of the the check. 
    /// </summary>
    public enum SeverityLevel
    {
        Trace = 0,
        Debug = 1,
        Info = 2,
        Warning = 3,
        Error = 4,
        Fatal = 5
    }
}