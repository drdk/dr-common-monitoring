using System;
using System.Collections.Generic;
using System.Linq;
using DR.Common.Monitoring.Contract;
using DR.Common.Monitoring.Models;

namespace DR.Common.Monitoring
{
    /// <summary>
    /// The global collection of every registred IHealthCheck implementations.
    /// Use structure map to bootstrap (already added in Global.asax.cs for the web-project):
    ///  a.For&lt;IEnumerable&lt;DR.Common.Monitoring.Contract.IHealthCheck&gt;&gt;().Use(x => x.GetAllInstances&lt;DR.Common.Monitoring.Contract.IHealthCheck&gt;());
    /// </summary>
    public class SystemStatus : ISystemStatus
    {
        private readonly IHealthCheck[] _checks;
        private readonly string[] _names;
        
        /// <summary>
        /// Construtor for System status
        /// </summary>
        /// <param name="checks">List of checks to register.</param>
        public SystemStatus(IEnumerable<IHealthCheck> checks)
        {
            _checks = checks.ToArray();
            _names = _checks.Select(c => c.Name).ToArray();
        }

        /// <inheritdoc />
        public IEnumerable<KeyValuePair<string,Status>> RunAllChecks(bool isPrivileged = false)
        {
            return _checks.Select(c => new KeyValuePair<string, Status>(c.Name, c.GetStatus(isPrivileged)));
        }

        /// <inheritdoc />
        public Status RunCheck(string name, bool isPrivileged = false)
        {
            var check = _checks.FirstOrDefault(c => c.Name == name);
            if (check == null)
                throw new KeyNotFoundException("No check named: "+ name);
            return check.GetStatus(isPrivileged);
        }

        /// <inheritdoc />
        public Status RunProbeCheck(string name, string node)
        {
            var check = _checks.FirstOrDefault(c => c.Name == name);
            if (check == null)
                throw new KeyNotFoundException("No check named: " + name);
            var probe = check as IClusterProbe;
            if(probe == null)
                throw new InvalidCastException("Check: "+ name +" is not a cluster probe");

            return probe.GetStatus(node);
        }

        /// <inheritdoc />
        public IEnumerable<string> Names { get { return _names; } }
    }
}
