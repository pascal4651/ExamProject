// <copyright file="Program.cs" company="Monosoft Holding ApS">
// Copyright (c) Monosoft Holding ApS. All rights reserved.
// </copyright>

namespace Monosoft.ServiceStore.DTO
{
    public class ProgamVersion
    {
        public ProgamVersion()
        {
        }

        public ProgamVersion(string version)
        {
            this.Version = version;
        }

        public enum VersionLevel
        {
            NA = -1,
            Major = 0,
            Minor = 1,
            Revision = 2,
            Build = 3
        }

        public string Version
        {
            get
            {
                return string.Format($"{this.Major}.{this.Minor}.{this.Revision}.{this.Build}");
            }

            set
            {
                string[] parts = value.Split('.');
                this.Major = int.Parse(parts[0]);
                this.Minor = parts.Length > 1 ? int.Parse(parts[1]) : 0;
                this.Revision = parts.Length > 2 ? int.Parse(parts[2]) : 0;
                this.Build = parts.Length > 3 ? int.Parse(parts[3]) : 0;
            }
        }

        public int Major { get; set; }

        public int Minor { get; set; }

        public int Revision { get; set; }

        public int Build { get; set; }

        public bool IsNewer(string version)
        {
            ProgamVersion otherVersion = new ProgamVersion(version);
            return this.IsNewer(otherVersion);
        }

        public bool IsNewer(ProgamVersion otherVersion)
        {
            if (this.Major > otherVersion.Major)
            {
                return true;
            }

            if (this.Major < otherVersion.Major)
            {
                return false;
            }

            if (this.Minor > otherVersion.Minor)
            {
                return true;
            }

            if (this.Minor < otherVersion.Minor)
            {
                return false;
            }

            if (this.Revision > otherVersion.Revision)
            {
                return true;
            }

            if (this.Revision < otherVersion.Revision)
            {
                return false;
            }

            if (this.Build > otherVersion.Build)
            {
                return true;
            }

            if (this.Build < otherVersion.Build)
            {
                return false;
            }

            return false;
        }

        public bool IsNewer(string version, VersionLevel level)
        {
            ProgamVersion otherVersion = new ProgamVersion(version);
            return this.IsNewer(otherVersion, level);
        }

        public bool IsNewer(ProgamVersion otherVersion, VersionLevel level)
        {
            if (level == VersionLevel.Build)
            {
                return this.IsNewer(otherVersion);
            }

            if (level == VersionLevel.Revision)
            {
                this.Build = 0;
                otherVersion.Build = 0;
                return this.IsNewer(otherVersion);
            }

            if (level == VersionLevel.Minor)
            {
                this.Build = 0;
                otherVersion.Build = 0;
                this.Revision = 0;
                otherVersion.Revision = 0;
                return this.IsNewer(otherVersion);
            }

            if (level == VersionLevel.Major)
            {
                this.Build = 0;
                otherVersion.Build = 0;
                this.Revision = 0;
                otherVersion.Revision = 0;
                this.Minor = 0;
                otherVersion.Minor = 0;
                return this.IsNewer(otherVersion);
            }

            return false;
        }
    }
}
