using Rocket.API.Commands;
using Rocket.API.Plugins;
using Rocket.API.User;
using System;

namespace persiafighter.Plugins.Jobs.Commands
{
    public class CommandJobLeave : ICommand
    {
        private readonly RocketJobs _rocketJobs;

        public CommandJobLeave(IPlugin plugin)
        {
            _rocketJobs = (RocketJobs)plugin;
        }

        public bool SupportsUser(Type user) => user is IUser;
        public string Name => "LeaveJob";
        public string Summary => "Leaves the job you are currently in.";
        public string Description => "Leaves the job you are currently in.";
        public string Permission => "LeaveJob";
        public string Syntax => null;
        public string[] Aliases => new string[] { "LJ", "LJob", "LeaveJ" };

        public IChildCommand[] ChildCommands => null;

        public void Execute(ICommandContext context) => _rocketJobs.Helper.RemovePlayerFromJob(context.User, caller: context.User);
    }
}
