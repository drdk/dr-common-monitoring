using System.Collections.Generic;

namespace DR.Common.Monitoring.Models
{
    public class TestRunResult
    {
        public bool? Success { get; }
        public string Message { get; }
        public IEnumerable<Reaction> Reactions { get; }
        public IEnumerable<dynamic> Details { get; }

        public TestRunResult(bool? success = null, string message = null, IEnumerable<Reaction> reactions = null, IEnumerable<dynamic> details = null)
        {
            Success = success;
            Message = message;
            Reactions = reactions;
            Details = details;
        }
    }
}