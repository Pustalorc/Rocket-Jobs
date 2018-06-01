using System;
using Rocket.API.Commands;
using Rocket.API.Plugins;

namespace persiafighter.Plugins.Jobs.Commands
{
    public class CommandJobs : ICommand
    {
        private readonly RocketJobs _rocketJobs;

        public CommandJobs(IPlugin plugin)
        {
            _rocketJobs = (RocketJobs)plugin;
        }

        public bool SupportsUser(Type user) => true;
        public string Name => "Jobs";
        public string Summary => "Lists the available jobs.";
        public string Description => "Lists the available jobs.";
        public string Permission => "Jobs";
        public string Syntax => null;
        public string[] Aliases => new string[] { "LJ", "LJobs", "ListJobs" };

        public IChildCommand[] ChildCommands => null;

        public void Execute(ICommandContext context)
        {
            _rocketJobs.Helper.ListJobs(context.User);
        }
    }
}
