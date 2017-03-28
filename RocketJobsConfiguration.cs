using Rocket.API;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Rocket_Jobs
{
    public class RocketJobsConfiguration : IRocketPluginConfiguration
    {
        public bool AnnounceJobJoin;
        public bool EnablePlugin;
        [XmlArrayItem(ElementName = "Jobs")]
        public List<PublicJobs> PublicJobs;
        public List<PrivateJobs> PrivateJobs;

        public void LoadDefaults()
        {
            AnnounceJobJoin = false;
            EnablePlugin = true;
            PublicJobs = new List<PublicJobs>
            {
                new PublicJobs { JobName = "Taxi", PermissionGroup = "Taxi" },
                new PublicJobs { JobName = "Cook", PermissionGroup = "Cook" },
                new PublicJobs { JobName = "Trader", PermissionGroup = "Trader" },
                new PublicJobs { JobName = "Farmer", PermissionGroup = "Farmer" }
            };
            PrivateJobs = new List<PrivateJobs>
            {
                new PrivateJobs { JobName = "Military", PermissionGroup = "Military", LeaderPermissionGroup = "Military Leader"},
                new PrivateJobs { JobName = "Police", PermissionGroup = "Police", LeaderPermissionGroup = "Police Leader"},
                new PrivateJobs { JobName = "Special Operations", PermissionGroup = "Spec Ops", LeaderPermissionGroup = "Spec Ops Leader"},
                new PrivateJobs { JobName = "Gun Seller", PermissionGroup = "Guns", LeaderPermissionGroup = "Guns Leader"}
            };
        }
    }

    public class PublicJobs
    {
        public PublicJobs() { }
        public string JobName;
        public string PermissionGroup;
    }

    public class PrivateJobs
    {
        public PrivateJobs() { }
        public string JobName;
        public string PermissionGroup;
        public string LeaderPermissionGroup;
    }
}
