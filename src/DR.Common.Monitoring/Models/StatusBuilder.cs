using System;
using System.Collections.Generic;
using System.Text;
using DR.Common.Monitoring.Contract;

namespace DR.Common.Monitoring.Models
{
    /// <summary>
    /// Status Builder, used in RunTest-methods
    /// </summary>
    public class StatusBuilder 
    {
        private List<Reaction> _reactions;
        private readonly IHealthCheck _sourceHealthCheck;
        private readonly bool _isPrivileged;
        private SeverityLevel _currentLevel;
        private readonly System.Diagnostics.Stopwatch _stopwatch = new System.Diagnostics.Stopwatch();

        internal StatusBuilder(IHealthCheck sourceHealthCheck, bool isPrivileged)
        {
            MessageBuilder = new StringBuilder();
            _sourceHealthCheck = sourceHealthCheck;
            CurrentLevel = sourceHealthCheck.MaximumSeverityLevel;
            _isPrivileged = isPrivileged;
            _stopwatch.Start();
        }
        
        /// <summary>
        /// Message builder
        /// </summary>
        public StringBuilder MessageBuilder { get; }

        /// <summary>
        /// This property is true if the check passed. If the check can neither fail or pass this property can be null. Defaults to null.
        /// </summary>
        public bool? Passed { get; set; } = null;

        /// <summary>
        /// The current level of the status, is always less than or equal to the MaximumSeverityLevel
        /// </summary>
        public SeverityLevel CurrentLevel
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

        /// <summary>
        /// Add a single Reaction
        /// </summary>
        /// <param name="reaction"></param>
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

        /// <summary>
        /// Add a range of Reactions
        /// </summary>
        /// <param name="reactions"></param>
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

        /// <summary>
        /// Optional payload, must be serializable to and from json. Defaults to null. 
        /// </summary>
        public object Payload { get; set; } = null;

        /// <summary>
        /// Optional Exception. Defaults to null. 
        /// </summary>
        public Exception Exception { get; set; } = null;

        /// <summary>
        /// Status Factory method
        /// </summary>
        public Status Status {

            get
            {
                var msg = MessageBuilder.ToString();
                if (msg == string.Empty)
                    msg = null;
                return
                    new Status(
                        _sourceHealthCheck,
                        Passed,
                        CurrentLevel,
                        _stopwatch.Elapsed,
                        msg,
                        _isPrivileged ? Exception : null,
                        _reactions?.ToArray(),
                        Payload);
            }
        }
    }
}
