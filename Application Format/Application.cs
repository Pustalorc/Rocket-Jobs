using persiafighter.Plugins.Jobs.JobTypes;

namespace persiafighter.Plugins.Jobs.Classes
{
    public class Application
    {
        public string ID { get; set; }
        public IJob Target { get; set; }
    }
}
