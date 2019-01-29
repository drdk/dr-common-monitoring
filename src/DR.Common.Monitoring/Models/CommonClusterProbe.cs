using System;
using System.Collections.Generic;
using System.Linq;
using DR.Common.Monitoring.Contract;

namespace DR.Common.Monitoring.Models
{
    /// <summary>
    /// Base class for cluster probes.
    /// </summary>
    public abstract class CommonClusterProbe : CommonHealthCheck, IClusterProbe
    {

        protected CommonClusterProbe(string name) : this(name, Level.Error, true, null, null) { }

        /// <summary>
        /// ctor
        /// </summary>
        protected CommonClusterProbe(string name,
            Level maximumSeverityLevel = Level.Error,
            bool includeInScom = true,
            string descriptionText = null,
            Uri descriptionLink = null) : base(name, maximumSeverityLevel, includeInScom, descriptionText, descriptionLink)
        {
        }

        /// <summary>
        /// Collection of registered nodes.
        /// </summary>
        public abstract IEnumerable<string> NodeNames { get; }


        /// <summary>
        /// Wraps call to protected method RunTest(nodeName). Handles exceptions and execution timer.
        /// </summary>
        /// <returns>Status object for RunTest(nodeName)-call</returns>
        public Status GetStatus(string nodeName, bool isPrivileged)
        {
            lock (Stopwatch)
            {
                bool? passed = null;
                Exception exception = null;
                string message = null;
                Reaction[] reactions = null;
                object payload = null;
                Level currentLevel = MaximumSeverityLevel;
                Status result;

                Stopwatch.Restart();
                try
                {
                    passed = RunTest(nodeName, ref message, ref currentLevel, ref reactions, ref payload);
                }
                catch (Exception e)
                {
                    passed = false;
                    exception = e;
                    message = nodeName;
                }
                finally
                {
                    Stopwatch.Stop();
                    result = result = new Status(
                        checkSource: this,
                        passed: passed,
                        currentLevel: currentLevel,
                        duration: Stopwatch.Elapsed,
                        message: message,
                        exception: isPrivileged ? exception : null,
                        reactions: reactions,
                        payload: payload);
                }
                return result;
            }
        }


        /// <summary>
        /// Must be implemented by derived classes. May throw exceptions.
        /// </summary>
        /// <param name="message">Human readable status message.</param>
        /// <param name="currentLevel">Optionally change current severity via parameter, defaults to maximum severity level.</param>
        /// <param name="reactions">Optional return reactions.</param>
        /// <param name="payload">Optional return custom data, should be serializable to json.</param>
        /// <returns>True of success and False for failure. Should throw exceptions if possible.</returns>
        protected abstract bool? RunTest(string node, ref string message, ref Level currentLevel, ref Reaction[] reactions,
            ref object payload);
        

        protected override bool? RunTest(ref string message, ref Level currentLevel, ref Reaction[] reactions, ref object payload)
        {
            bool? result = null;
            var first = true;
            foreach (var nodeName in NodeNames)
            {
                bool? nodePassed = null;
                Exception nodeException = null;
                string nodeMessage = null;
                Reaction[] nodeReactions = null;
                object nodePayload = null;
                Level nodeCurrentLevel = MaximumSeverityLevel;

                nodePassed = RunTest(nodeName, ref nodeMessage, ref nodeCurrentLevel, ref nodeReactions,
                    ref nodePayload);

                message += (first ? string.Empty : "\n---\n") + "Node: \"" + nodeName + "\" :\n" + (nodeMessage ?? "null");

                if (nodePassed.HasValue)
                {
                    result = result.GetValueOrDefault(true) && nodePassed.Value;
                }
                else if (result.HasValue)
                {
                    result = false;
                }

                if (nodeReactions != null && nodeReactions.Any())
                {
                    reactions = reactions == null ? nodeReactions : reactions.Concat(nodeReactions).ToArray();
                }

                if (nodePayload != null)
                {
                    MergePayload(ref payload, nodePayload);
                }
                first = false;
            }
            return result;
        }

        protected virtual void MergePayload(ref object targetPayload, object sourcePayload)
        {
            throw new NotImplementedException();
        }
    }
}
