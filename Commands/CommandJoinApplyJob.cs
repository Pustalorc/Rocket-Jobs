using System;
using Rocket.API.Commands;
using Rocket.API.Plugins;
using Rocket.API.User;
using Rocket.Core.Commands;

namespace persiafighter.Plugins.Jobs.Commands
{
    public class CommandJoinJob : ICommand
    {
        private readonly RocketJobs _rocketJobs;

        public CommandJoinJob(IPlugin plugin)
        {
            _rocketJobs = (RocketJobs)plugin;
        }

        public bool SupportsUser(Type user) => user is IUser;
        public string Name => "JoinJob";
        public string Summary => "Joins a public job or applies to a private job.";
        public string Description => "Joins a public job or applies to a private job.";
        public string Permission => "JoinJob";
        public string Syntax => "<job name>";
        public string[] Aliases => new string[] { "JJ", "JJob" };

        public IChildCommand[] ChildCommands => null;

        public void Execute(ICommandContext context)
        {
            if (context.Parameters.Length != 1)
                throw new CommandWrongUsageException();

            string job = context.Parameters.Get<string>(0);

            _rocketJobs.Helper.AddPlayerToJob(context.User, job, context.User);
        }
    }
}
