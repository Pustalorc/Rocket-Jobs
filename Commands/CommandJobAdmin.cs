﻿using Rocket.API.Commands;
using Rocket.API.Plugins;
using Rocket.API.User;
using Rocket.Core.Commands;
using Rocket.Core.I18N;
using System;

namespace persiafighter.Plugins.Jobs.Commands
{
    public class CommandJobAdmin : ICommand
    {
        private readonly RocketJobs _rocketJobs;

        public CommandJobAdmin(IPlugin plugin)
        {
            _rocketJobs = (RocketJobs)plugin;
        }

        public bool SupportsUser(Type user) => true;
        public string Name => "JobAdmin";
        public string Summary => "Administrative command for jobs.";
        public string Description => "Controls jobs people are on, applications, jobs that exist, etc.";
        public string Permission => "JobAdmin";
        public string Syntax => "applications clear | add <job name> <player name> | remove <job name> <player name>";
        public string[] Aliases => new string[] { "JA", "JAdmin", "JobA" };

        public IChildCommand[] ChildCommands => null;

        public void Execute(ICommandContext context)
        {
            if (context.Parameters.Length > 3 || context.Parameters.Length < 2)
                throw new CommandWrongUsageException();

            IUserManager globalUserManager = context.Container.Resolve<IUserManager>();

            string arg1 = context.Parameters.Get<string>(0);
            string arg2 = context.Parameters.Get<string>(1);

            if (arg1.Equals("applications", StringComparison.OrdinalIgnoreCase) && arg2.Equals("clear", StringComparison.OrdinalIgnoreCase))
                _rocketJobs.Helper.ClearApplications();
            else if (arg1.Equals("add", StringComparison.OrdinalIgnoreCase))
            {
                if (context.Parameters.Length == 3)
                {
                    IUserInfo target = context.Parameters.Get<IUserInfo>(2);

                    _rocketJobs.Helper.AddPlayerToJob(target, arg2, context.User);
                }
                else
                    globalUserManager.SendLocalizedMessage(_rocketJobs.Translations, context.User, "jobadmin_usage_add");
            }
            else if (arg1.Equals("remove", StringComparison.OrdinalIgnoreCase))
            {
                if (context.Parameters.Length == 3)
                {
                    IUserInfo target = context.Parameters.Get<IUserInfo>(2);

                    _rocketJobs.Helper.RemovePlayerFromJob(target, arg2, context.User);
                }
                else
                    globalUserManager.SendLocalizedMessage(_rocketJobs.Translations, context.User, "jobadmin_usage_remove");
            }
            else
                globalUserManager.SendLocalizedMessage(_rocketJobs.Translations, context.User, "jobadmin_usage");
        }
    }
}
