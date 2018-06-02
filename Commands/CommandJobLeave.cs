using Rocket.API.Commands;
using Rocket.API.Plugins;
using System;

namespace persiafighter.Plugins.Jobs.Commands
{
    public class CommandJobLeave : IChildCommand
    {
        private readonly RocketJobsPlugin _rocketJobsPlugin;

        public CommandJobLeave(IPlugin plugin)
        {
            _rocketJobsPlugin = (RocketJobsPlugin)plugin;
        }

        public bool SupportsUser(Type user) => true;
        public string Name => "LeaveJob";
        public string Summary => "Leaves the job you are currently in.";
        public string Description => "Leaves the job you are currently in.";
        public string Permission => "LeaveJob";
        public string Syntax => null;
        public string[] Aliases => new[] { "LJ", "LJob", "LeaveJ" };

        public IChildCommand[] ChildCommands => null;

        public void Execute(ICommandContext context) => _rocketJobsPlugin.JobManager.RemovePlayerFromJob(context.User, caller: context.User);
    }
}
