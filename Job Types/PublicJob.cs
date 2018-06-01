namespace persiafighter.Plugins.Jobs.JobTypes
{
    public class PublicJob : IJob
    {
        public string JobName { get; set; }
        public string PermissionGroup { get; set; }
    }
}
