namespace persiafighter.Plugins.Jobs.Jobs
{
    public interface IJob
    {
        string JobName { get; set; }
        string PermissionGroup { get; set; }
    }
}
