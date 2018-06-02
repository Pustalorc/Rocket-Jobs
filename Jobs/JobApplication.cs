namespace persiafighter.Plugins.Jobs.Jobs
{
    public class JobApplication
    {
        public string Id { get; set; }
        public IJob Target { get; set; }
    }
}
