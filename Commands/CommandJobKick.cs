using Rocket.API.Commands;
using Rocket.API.Plugins;
using Rocket.API.User;
using Rocket.Core.Commands;
using System;

namespace persiafighter.Plugins.Jobs.Commands
{
    public class CommandJobKick : IChildCommand
    {
        private readonly RocketJobsPlugin _rocketJobsPlugin;
        
        public CommandJobKick(IPlugin plugin)
        {
            _rocketJobsPlugin = (RocketJobsPlugin)plugin;
        }

        public bool SupportsUser(Type user) => true;
        public string Name => "JobKick";
        public string Summary => "Kicks a player from a job.";
        public string Description => "Kicks a player from a job.";
        public string Permission => "JobKick";
        public string Syntax => "<player>";
        public string[] Aliases => new[] { "KJ", "KJob", "KickJ" };

        public IChildCommand[] ChildCommands => null;

        public void Execute(ICommandContext context)
        {
            if (context.Parameters.Length != 1)
                throw new CommandWrongUsageException();
            
            IUserInfo toTakeRank = context.Parameters.Get<IUserInfo>(0);

            _rocketJobsPlugin.JobManager.KickPlayerFromJob(context.User, toTakeRank);
        }
    }
}
