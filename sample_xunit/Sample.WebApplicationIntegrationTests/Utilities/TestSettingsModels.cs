﻿namespace Sample.WebApplicationIntegrationTests.Utilities
{
    public class TestSettings
    {
        /// <summary>
        /// The Environment Name.
        /// </summary>
        public int EnvironmentName { get; set; }

        /// <summary>
        /// Mssql
        /// </summary>
        public Mssql Mssql { get; set; }
    }

    public class ContainerSetting
    {
        public string Image { get; set; }

        public string Tag { get; set; }

        public string ContainerName { get; set; }

        public string ContainerReadyMessage { get; set; }

        public Dictionary<string, string> EnvironmentSettings { get; set; }
    }

    public class Mssql : ContainerSetting
    {
        public Mssql()
        {
            this.HostPort = 0;
            this.ContainerPort = 1433;
        }

        public ushort HostPort { get; set; }

        public ushort ContainerPort { get; set; }

        public string SaPassword { get; set; }
    }

    public class Redis : ContainerSetting
    {
        public Redis()
        {
            this.HostPort = 0;
            this.ContainerPort = 6329;
        }

        public ushort HostPort { get; set; }

        public ushort ContainerPort { get; set; }
    }
}