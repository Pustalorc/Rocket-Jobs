using persiafighter.Plugins.Jobs.Config;
using Rocket.API.DependencyInjection;
using Rocket.API.Permissions;
using Rocket.API.User;
using Rocket.Core.Plugins;
using System.Collections.Generic;
using persiafighter.Plugins.Jobs.EventListeners;
using Rocket.Core.Logging;

namespace persiafighter.Plugins.Jobs
{
    public class RocketJobsPlugin : Plugin<JobsConfiguration>
    {
        public JobManager JobManager { get; private set; }
        private readonly IPermissionProvider _permissionProvider;
        private readonly IUserManager _userManager;

        public RocketJobsPlugin(IDependencyContainer container, IPermissionProvider permissionProvider, IUserManager userManager) : base("Jobs", container)
        {
            _permissionProvider = permissionProvider;
            _userManager = userManager;
        }

        protected override void OnLoad(bool isFromReload)
        {
            if (JobManager == null)
                JobManager = new JobManager(ConfigurationInstance, Translations, _permissionProvider, _userManager);
            else
                JobManager.Reload(ConfigurationInstance);

            Logger.LogInformation("RocketJobs, by persiafigther, has been sucessfully loaded!");

            EventManager.AddEventListener(this, new PlayerListener(this));
        }

        protected override void OnUnload()
        {
            JobManager.ClearAll();
            Logger.LogInformation("RocketJobs, by persiafighter, has been successfully unloaded!");
        }

        public override Dictionary<string, string> DefaultTranslations => new Dictionary<string, string>
        {
            { "left_job", "{0} has left the job {1}!" },
            { "failed_remove_job", "An issue occured when removing the player from the job." },
            { "not_in_job", "You are not in the job {0}." },
            { "not_in_job_admin", "The player {1} is not in the job {0}." },
            { "already_in_job", "You already are in a job." },
            { "already_in_job_admin", "The player {0} is already in a job." },
            { "job_destroyed", "Job {0} was not found or has been deleted without the plugin being reloaded." },
            { "already_applying", "You are already applying to a job." },
            { "no_leaders", "The job you are trying to apply to has no leaders online or no leaders at all." },
            { "player_applying", "{0} wants to join the job {1} that you are a leader of!" },
            { "job_applied", "You have applied to the job" },
            { "joined_job", "{0} has joined the job {1}!" },
            { "failed_add_job", "An issue occured when adding the player to the job." },
            { "not_applying", "{0} is not applying to any jobs." },
            { "special_error", "The job the user applied to is not a private job. Please restart the plugin." },
            { "not_leader_of_job", "You are not the leader of the group {0}, you may not accept this application." },
            { "applications_cleared", "Applications to all jobs have been cleared." },
            { "list_jobs", "Available Jobs: {0}" },
            { "jobadmin_usage_add", "Correct use of JobAdmin command: /jobadmin add <job> <player>" },
            { "jobadmin_usage_remove", "Correct use of JobAdmin command: /jobadmin remove <job> <player>" },
            { "jobadmin_usage", "Correct use of JobAdmin command: /Jobadmin <applications clear | add <job name> <player name> | remove <job name> <player name>>" }
        };
    }
}
