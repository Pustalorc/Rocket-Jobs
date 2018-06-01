using persiafighter.Plugins.Jobs.JobTypes;
using System.Collections.Generic;

namespace persiafighter.Plugins.Jobs.Config
{
    public class Configuration
    {
        public List<IJob> Jobs { get; set; } = new List<IJob>
        {
            new PublicJob { JobName = "Taxi", PermissionGroup = "Taxi" },
            new PublicJob { JobName = "Cook", PermissionGroup = "Cook" },
            new PublicJob { JobName = "Trader", PermissionGroup = "Trader" },
            new PublicJob { JobName = "Farmer", PermissionGroup = "Farmer" },
            new PrivateJob { JobName = "Military", PermissionGroup = "Military", LeaderPermissionGroup = "Military Leader"},
            new PrivateJob { JobName = "Police", PermissionGroup = "Police", LeaderPermissionGroup = "Police Leader"},
            new PrivateJob { JobName = "Special Operations", PermissionGroup = "Spec Ops", LeaderPermissionGroup = "Spec Ops Leader"},
            new PrivateJob { JobName = "Gun Seller", PermissionGroup = "Guns", LeaderPermissionGroup = "Guns Leader"}
        };
    }
}
