namespace persiafighter.Plugins.Jobs.Jobs
{
    public class PrivateJob : IJob
    {
        public string JobName { get; set; }
        public string PermissionGroup { get; set; }
        public string LeaderPermissionGroup { get; set; }
    }
}
