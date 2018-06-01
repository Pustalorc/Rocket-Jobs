using Rocket.API.Commands;
using Rocket.API.Plugins;
using Rocket.API.User;
using Rocket.Core.Commands;
using System;

namespace persiafighter.Plugins.Jobs.Commands
{
    public class CommandJobAccept : ICommand
    {
        private readonly RocketJobs _rocketJobs;
        
        public CommandJobAccept(IPlugin plugin)
        {
            _rocketJobs = (RocketJobs)plugin;
        }
            
        public bool SupportsUser(Type user) => user is IUser;
        public string Name => "JobAccept";
        public string Summary => "Accepts a player requesting to join a job.";
        public string Description => "Accepts a player requesting to join a job.";
        public string Permission => "JobAccept";
        public string Syntax => "<player>";
        public string[] Aliases => new string[] { "AJ", "AJob", "AcceptJ" };

        public IChildCommand[] ChildCommands => null;

        public void Execute(ICommandContext context)
        {
            if (context.Parameters.Length != 1)
                throw new CommandWrongUsageException();
            
            IUserInfo toGiveRank = context.Parameters.Get<IUserInfo>(0);

            _rocketJobs.Helper.AcceptApplication(toGiveRank, context.User);
        }
    }
}
