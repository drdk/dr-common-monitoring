using System;
using System.Collections.Generic;
using System.Text;
using DR.Common.Monitoring.Contract;

namespace DR.Common.Monitoring.Models
{
    internal class StatusBuilder : IStatusBuilder
    {
        private List<Reaction> _reactions;
        private readonly IHealthCheck _sourceHealthCheck;
        private readonly bool _isPrivileged;
        private Level _currentLevel;
        private readonly System.Diagnostics.Stopwatch _stopwatch = new System.Diagnostics.Stopwatch();

        public StatusBuilder(IHealthCheck sourceHealthCheck, bool isPrivileged)
        {
            MessageBuilder = new StringBuilder();
            _sourceHealthCheck = sourceHealthCheck;
            CurrentLevel = sourceHealthCheck.MaximumSeverityLevel;
            _isPrivileged = isPrivileged;
            _stopwatch.Start();
        }

        ~StatusBuilder()
        {
            _stopwatch.Stop();
        }

        public StringBuilder MessageBuilder { get; }
        public bool? Passed { get; set; } = null;

        public Level CurrentLevel
        {
            get => _currentLevel;
            set
            {
                if (value > _sourceHealthCheck.MaximumSeverityLevel)
                {
                    _currentLevel = _sourceHealthCheck.MaximumSeverityLevel;
                    MessageBuilder.AppendLine($"currentLevel: {value.ToString()} exceeded maximum level {_sourceHealthCheck.MaximumSeverityLevel.ToString()}, limiting to max.");
                }
                else
                {
                    _currentLevel = value;
                }
            }
        }

        public void AddReaction(Reaction reaction)
        {
            if (_reactions == null)
            {
                _reactions = new List<Reaction> { reaction };
            }
            else
            {
                _reactions.Add(reaction);
            }
        }

        public void AddReaction(IEnumerable<Reaction> reactions)
        {
            if (_reactions == null)
            {
                _reactions = new List<Reaction>(reactions);
            }
            else
            {
                _reactions.AddRange(reactions);
            }
        }

        public object Payload { get; set; } = null;
        public Exception Exception { get; set; } = null;

        public Status Status => 
            new Status(
                _sourceHealthCheck,
                Passed,
                CurrentLevel,
                _stopwatch.Elapsed,
                MessageBuilder.ToString(),
                _isPrivileged ? Exception : null,
                _reactions?.ToArray(),
                Payload);
    }
}
