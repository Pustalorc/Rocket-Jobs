using persiafighter.Plugins.Jobs.Config;
using Rocket.API.DependencyInjection;
using Rocket.API.Logging;
using Rocket.API.Permissions;
using Rocket.API.User;
using Rocket.Core.Plugins;
using System.Collections.Generic;

namespace persiafighter.Plugins.Jobs
{
    public class RocketJobs : Plugin<Configuration>
    {
        public JobHelper Helper { get; private set; }
        private IPermissionProvider PermissionProvider { get; set; }
        private IUserManager UserManager { get; set; }

        public RocketJobs(IDependencyContainer container, IPermissionProvider permissionProvider, IUserManager userManager) : base("Jobs", container)
        {
            PermissionProvider = permissionProvider;
            UserManager = userManager;
        }

        protected override void OnLoad(bool isFromReload)
        {
            if (Helper == null)
                Helper = new JobHelper(ConfigurationInstance, Translations, PermissionProvider, UserManager);
            else
                Helper.Reload(ConfigurationInstance, Translations);

            Logger.Log("RocketJobs, by persiafigther, has been sucessfully loaded!", LogLevel.Information);
        }
        protected override void OnUnload()
        {
            Helper.ClearAll();
            Logger.Log("RocketJobs, by persiafighter, has been successfully unloaded!", LogLevel.Information);
        }

        public override Dictionary<string, string> DefaultTranslations => new Dictionary<string, string>
        {
            /*{ "list_usage", "/jobs <private | public> <page>"  },
            { "join_apply_usage", "/jjob <job name>" },
            { "accept_usage", "/ajob <player name>" },
            { "admin_usage", "/joba <add | remove | clear> <job name> <player name>" },
            { "format_error", "Unable to convert {0} to a number." },
            { "overflow_error", "{0} is too big of a number." },
            { "next_page_notification", "Next page: \"/Jobs {0} {1}\"." },
            { "pub_job_notification", "{0}" },
            { "priv_job_notification", "{0}" },
            { "end_of_list", "You have reached the end of the {0} jobs." },
            { "unexistant_page", "That page does not exist." },
            { "error_already_in_a_job", "You already are in a job!" },
            { "error_already_in_a_job_admin", "You cannot make a player have 2 jobs!" },
            { "error_leader_of_a_job", "You are the leader of a job, and may not join other jobs!" },
            { "error_leader_of_a_job_admin", "That player is the leader of a job and may not join other jobs." },
            { "error_already_applying", "You are still applying to another job!" },
            { "error_leader_offline", "Unable to send request to join {0}, no leader is online." },
            { "error_job_not_found", "Unable to find a job known as {0}" },
            { "error_not_in_a_job", "You have not yet joined a job!" },
            { "error_player_not_applying", "That player is not applying to your job." },
            { "error_invalid_job_in_storage", "Stored job that player was applying to does not exist anymore." },
            { "error_not_leader_of_job", "You are not the leader of {0}. You may not accept that request." },
            { "error_player_not_in_job", "Unable to remove player from the job {0}." },
            { "error_contact_admin", "An error has occured, please contact an admin about this." },
            { "notification_quiet_joined_job", "You have joined the job {0}." },
            { "notification_quiet_joined_job_admin", "{0} has joined the job {1}." },
            { "notification_global_joined_job", "{0} has joined the job {1}." },
            { "notification_player_applying", "{0} wants to join your job." },
            { "notification_applied_to_job", "You have sent a request to join the job {0}." },
            { "notification_left_job", "You left the job {0}." },
            { "notification_accepted_application", "You have accepted the application of {0}." },
            { "notification_job_cleared", "You have successfully cleared the job {0}." }*/
        };
    }
}
