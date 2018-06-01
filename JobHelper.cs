using persiafighter.Plugins.Jobs.Classes;
using persiafighter.Plugins.Jobs.Config;
using persiafighter.Plugins.Jobs.JobTypes;
using Rocket.API.Commands;
using Rocket.API.I18N;
using Rocket.API.Permissions;
using Rocket.API.User;
using Rocket.Core.I18N;
using Rocket.Core.Permissions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace persiafighter.Plugins.Jobs
{
    public class JobHelper
    {
        private List<IJob> AvailableJobs { get; set; }
        private List<Application> Applicants { get; set; }
        private IUserManager GlobalUserManager { get; set; }
        private ITranslationCollection Translations { get; set; }
        private IPermissionProvider PermissionProvider { get; set; }

        public JobHelper(Configuration config, ITranslationCollection translationCollection, IPermissionProvider permissionProvider, IUserManager userManager)
        {
            if (AvailableJobs == null)
                AvailableJobs = new List<IJob>();

            AvailableJobs.AddRange(config.Jobs);
            AvailableJobs.RemoveAll(k =>
            {
                if (k is PrivateJob @private)
                    return permissionProvider.GetGroup(@private.LeaderPermissionGroup) == null;

                return permissionProvider.GetGroup(k.PermissionGroup) == null;
            });

            if (Applicants == null)
                Applicants = new List<Application>();

            PermissionProvider = permissionProvider;
            GlobalUserManager = userManager;
            Translations = translationCollection;
        }

        public void RemovePlayerFromJob(IUserInfo Target, string job = null, IUser caller = null)
        {
            IUser giveRank = Target.UserManager.Users.FirstOrDefault(c => string.Equals(c.Id, Target.Id, StringComparison.OrdinalIgnoreCase));

            IJob tJob = null;
            if (job != null)
                tJob = AvailableJobs.Find(k => k.JobName.Equals(job, StringComparison.OrdinalIgnoreCase));
            else
                tJob = GetPlayerJob(giveRank);

            if (tJob != null)
            {
                IPermissionGroup TargetGroup = PermissionProvider.GetGroup(tJob.PermissionGroup);

                if (PermissionProvider.RemoveGroup(giveRank, TargetGroup))
                    GlobalUserManager.BroadcastLocalized(Translations, "left_job", Target.Name, tJob.JobName);
                else if (caller != null)
                    GlobalUserManager.SendLocalizedMessage(Translations, caller, "failed_remove_group");
            }
            else
            {
                if (Target == caller)
                    GlobalUserManager.SendLocalizedMessage(Translations, caller, "not_in_job", job);
                else if (caller != null)
                    GlobalUserManager.SendLocalizedMessage(Translations, caller, "not_in_job_admin", job, Target.Name);
            }
        }
        public void Reload(Configuration config, ITranslationCollection translationCollection)
        {
            if (AvailableJobs == null)
                AvailableJobs = new List<IJob>();

            AvailableJobs.AddRange(config.Jobs);
            AvailableJobs.RemoveAll(k =>
            {
                if (k is PrivateJob @private)
                    return PermissionProvider.GetGroup(@private.LeaderPermissionGroup) == null;

                return PermissionProvider.GetGroup(k.PermissionGroup) == null;
            });

            if (Applicants == null)
                Applicants = new List<Application>();

            Translations = translationCollection;
        }
        public void AddPlayerToJob(IUserInfo Target, string job, IUser caller = null)
        {
            IUser giveRank = Target.UserManager.Users.FirstOrDefault(c => string.Equals(c.Id, Target.Id, StringComparison.OrdinalIgnoreCase));

            IJob tJob = AvailableJobs.Find(k => k.JobName.Equals(job, StringComparison.OrdinalIgnoreCase));
            IJob PlayerJob = GetPlayerJob(giveRank);

            if (PlayerJob == null)
            {
                if (tJob != null)
                {
                    if (caller == Target && tJob is PrivateJob @private)
                    {
                        if (Applicants.Exists(k => k.ID == Target.Id))
                            GlobalUserManager.SendLocalizedMessage(Translations, caller, "already_applying");
                        else
                        {
                            var players = GetOnlinePlayersInGroup(@private.LeaderPermissionGroup);
                            if (players != null && players.Count > 0)
                            {
                                Applicants.Add(new Application() { ID = Target.Id, Target = @private });
                                var player = GlobalUserManager.Users.ToList().Find(k => k.Id == players.First().Id);
                                GlobalUserManager.SendLocalizedMessage(Translations, player, "player_applying", Target.Name, @private.JobName);
                                GlobalUserManager.SendLocalizedMessage(Translations, caller, "job_applied");
                            }
                            else
                                GlobalUserManager.SendLocalizedMessage(Translations, caller, "no_leaders");
                        }
                    }
                    else
                    {
                        IPermissionGroup TargetGroup = PermissionProvider.GetGroup(tJob.PermissionGroup);
                        ClearApplications(giveRank);

                        if (PermissionProvider.AddGroup(giveRank, TargetGroup))
                            GlobalUserManager.BroadcastLocalized(Translations, "joined_job", Target.Name, tJob.JobName);
                        else if (caller != null)
                            GlobalUserManager.SendLocalizedMessage(Translations, caller, "failed_add_group");
                    }
                }
                else if (caller != null)
                    GlobalUserManager.SendLocalizedMessage(Translations, caller, "job_destroyed", job);
            }
            else
            {
                if (Target == caller)
                    GlobalUserManager.SendLocalizedMessage(Translations, caller, "already_in_job");
                else if (caller != null)
                    GlobalUserManager.SendLocalizedMessage(Translations, caller, "already_in_job_admin", Target.Name);
            }
        }
        public void AcceptApplication(IUserInfo Target, IUser Caller)
        {
            IUser giveRank = Target.UserManager.Users.FirstOrDefault(c => string.Equals(c.Id, Target.Id, StringComparison.OrdinalIgnoreCase));
            Application App = Applicants.Find(k => k.ID == giveRank.Id);

            if (App != null)
            {
                IJob job = AvailableJobs.Find(k => k.JobName.Equals(App.Target.JobName, StringComparison.OrdinalIgnoreCase));
                if (job != null)
                {
                    if (job is PrivateJob @private)
                    {
                        IPermissionGroup LeaderGroup = PermissionProvider.GetGroup(@private.LeaderPermissionGroup);
                        List<IPermissionGroup> permissionGroups = PermissionProvider.GetGroups(Caller).ToList();

                        if (permissionGroups.Contains(LeaderGroup))
                        {
                            IPermissionGroup TargetGroup = PermissionProvider.GetGroup(job.PermissionGroup);

                            if (PermissionProvider.AddGroup(giveRank, TargetGroup))
                                GlobalUserManager.BroadcastLocalized(Translations, "joined_job", Target.Name, job.JobName);
                            else
                                GlobalUserManager.SendLocalizedMessage(Translations, Caller, "failed_add_group");
                        }
                        else
                            GlobalUserManager.SendLocalizedMessage(Translations, Caller, "not_leader_of_group", job.JobName);
                    }
                    else
                        GlobalUserManager.SendLocalizedMessage(Translations, Caller, "special_error");
                }
                else
                    GlobalUserManager.SendLocalizedMessage(Translations, Caller, "job_destroyed", App.Target);
            }
            else
                GlobalUserManager.SendLocalizedMessage(Translations, Caller, "not_applying", Target.Name);
        }
        public void ClearApplications(IUser caller = null)
        {
            Applicants.Clear();
            if (caller != null)
                GlobalUserManager.SendLocalizedMessage(Translations, caller, "applications_cleared");
        }
        public void PlayerDisconnected(string ID)
        {
            Applicants.RemoveAll(k => ID.Equals(k.ID, System.StringComparison.OrdinalIgnoreCase));
        }
        public void ListJobs(IUser caller)
        {
            string AllJobs = "";
            foreach (IJob j in AvailableJobs)
            {
                if (AvailableJobs.Last() == j)
                    AllJobs += j.JobName + ".";
                else
                    AllJobs += j.JobName + ", ";
            }

            GlobalUserManager.SendLocalizedMessage(Translations, caller, "list_jobs", AllJobs);
        }
        public void ClearAll()
        {
            ClearApplications();
            AvailableJobs.Clear();
        }

        public IJob GetPlayerJob(IUser Target)
        {
            List<IPermissionGroup> groups = PermissionProvider.GetGroups(Target).ToList();

            return AvailableJobs.Find(k => groups.Exists(l => k.PermissionGroup.Equals(l.Id, StringComparison.OrdinalIgnoreCase)));
        }
        private List<IUser> GetOnlinePlayersInGroup(string GroupName)
        {
            var users = GlobalUserManager.Users.ToList();
            users.RemoveAll(k => k is IConsole);
            users.RemoveAll(k => !PermissionProvider.GetGroups(k).ToList().Exists(l => l.Id.Equals(GroupName, StringComparison.OrdinalIgnoreCase)));
            return users;
        }
    }
}
