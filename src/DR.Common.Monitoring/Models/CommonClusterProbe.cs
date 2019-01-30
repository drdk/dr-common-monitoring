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
        public Status GetStatus(string nodeName, bool isPrivileged = true)
        {
            if (!NodeNames.Contains(nodeName))
                throw new KeyNotFoundException($"No node name : {nodeName}");

            var statusBuilder = new StatusBuilder(this, isPrivileged);
            try
            {
                RunTest(nodeName, statusBuilder);
            }
            catch (Exception e)
            {
                statusBuilder.Passed = false;
                statusBuilder.Exception = e;
                HandleException(e, statusBuilder);
            }
            return statusBuilder.Status;
        }


        /// <summary>
        /// Must be implemented by derived classes. May throw exceptions. Should merge status in statusBuilder. 
        /// </summary>
        protected abstract void RunTest(string node, StatusBuilder statusBuilder);
        

        protected override void RunTest(StatusBuilder statusBuilder)
        {
            foreach (var nodeName in NodeNames)
            {
                statusBuilder.MessageBuilder.AppendLine($"Node: \"{nodeName}\":");
                RunTest(nodeName, statusBuilder);
            }
        }
    }
}
