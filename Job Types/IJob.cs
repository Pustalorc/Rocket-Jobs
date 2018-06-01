namespace persiafighter.Plugins.Jobs.JobTypes
{
    public interface IJob
    {
        string JobName { get; set; }
        string PermissionGroup { get; set; }
    }
}
