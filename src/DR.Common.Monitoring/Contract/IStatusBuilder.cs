using System;
using System.Collections.Generic;
using System.Text;
using DR.Common.Monitoring.Models;

namespace DR.Common.Monitoring.Contract
{
    public interface IStatusBuilder
    {
        StringBuilder MessageBuilder { get; }
        bool? Passed { get; set; }
        Level CurrentLevel { get; set; }
        void AddReaction(Reaction reaction);
        void AddReaction(IEnumerable<Reaction> reactions);
        object Payload { get; set; }
        Exception Exception { get; set; }
        Status Status { get; }
    }
}