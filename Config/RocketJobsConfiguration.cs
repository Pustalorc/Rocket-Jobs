using System.Collections.Generic;
using persiafighter.Plugins.Jobs.Jobs;

namespace persiafighter.Plugins.Jobs.Config
{
    public class JobsConfiguration
    {
        public List<PublicJob> PublicJobs { get; set; } = new List<PublicJob>
        {
            new PublicJob { JobName = "Taxi", PermissionGroup = "Taxi" },
            new PublicJob { JobName = "Cook", PermissionGroup = "Cook" },
            new PublicJob { JobName = "Trader", PermissionGroup = "Trader" },
            new PublicJob { JobName = "Farmer", PermissionGroup = "Farmer" }
        };
        public List<PrivateJob> PrivateJobs { get; set; } = new List<PrivateJob>
        {
            new PrivateJob { JobName = "Military", PermissionGroup = "Military", LeaderPermissionGroup = "Military Leader"},
            new PrivateJob { JobName = "Police", PermissionGroup = "Police", LeaderPermissionGroup = "Police Leader"},
            new PrivateJob { JobName = "Special Operations", PermissionGroup = "Spec Ops", LeaderPermissionGroup = "Spec Ops Leader"},
            new PrivateJob { JobName = "Gun Seller", PermissionGroup = "Guns", LeaderPermissionGroup = "Guns Leader"}
        };
    }
}
