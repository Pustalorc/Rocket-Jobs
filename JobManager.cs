using persiafighter.Plugins.Jobs.Config;
using Rocket.API.Commands;
using Rocket.API.I18N;
using Rocket.API.Permissions;
using Rocket.API.User;
using Rocket.Core.I18N;
using System;
using System.Collections.Generic;
using System.Linq;
using persiafighter.Plugins.Jobs.Jobs;

namespace persiafighter.Plugins.Jobs
{
    public class JobManager
    {
        private readonly IPermissionProvider _permissionProvider;
        private readonly IUserManager _globalUserManager;
        private readonly ITranslationCollection _translations;

        private List<IJob> _availableJobs;
        private List<JobApplication> _applicants;

        public JobManager(JobsConfiguration config, ITranslationCollection translationCollection, IPermissionProvider permissionProvider, IUserManager userManager)
        {
            if (_availableJobs == null)
                _availableJobs = new List<IJob>();

            _availableJobs.AddRange(config.Jobs);
            _availableJobs.RemoveAll(k =>
            {
                if (k is PrivateJob @private)
                    return permissionProvider.GetGroup(@private.LeaderPermissionGroup) == null;

                return permissionProvider.GetGroup(k.PermissionGroup) == null;
            });

            if (_applicants == null)
                _applicants = new List<JobApplication>();

            _permissionProvider = permissionProvider;
            _globalUserManager = userManager;
            _translations = translationCollection;
        }

        public void RemovePlayerFromJob(IUserInfo target, string job = null, IUser caller = null)
        {
            IUser giveRank = target.UserManager.OnlineUsers.FirstOrDefault(c => string.Equals(c.Id, target.Id, StringComparison.OrdinalIgnoreCase));

            var tJob = job != null ? _availableJobs.FirstOrDefault(k => k.JobName.Equals(job, StringComparison.OrdinalIgnoreCase)) : GetPlayerJob(giveRank);

            if (tJob != null)
            {
                IPermissionGroup targetGroup = _permissionProvider.GetGroup(tJob.PermissionGroup);

                if (_permissionProvider.RemoveGroup(giveRank, targetGroup))
                    _globalUserManager.BroadcastLocalized(_translations, "left_job", target.Name, tJob.JobName);
                else
                    caller?.SendLocalizedMessage(_translations, "failed_remove_job");
            }
            else
            {
                if (target == caller)
                    caller.SendLocalizedMessage(_translations, "not_in_job", job);
                else
                    caller?.SendLocalizedMessage(_translations, "not_in_job_admin", job, target.Name);
            }
        }
        public void Reload(JobsConfiguration config)
        {
            if (_availableJobs == null)
                _availableJobs = new List<IJob>();

            _availableJobs.AddRange(config.Jobs);
            _availableJobs.RemoveAll(k =>
            {
                if (k is PrivateJob @private)
                    return _permissionProvider.GetGroup(@private.LeaderPermissionGroup) == null;

                return _permissionProvider.GetGroup(k.PermissionGroup) == null;
            });

            if (_applicants == null)
                _applicants = new List<JobApplication>();
        }
        public void AddPlayerToJob(IUserInfo target, string job, IUser caller = null)
        {
            IUser giveRank = target.UserManager.OnlineUsers.FirstOrDefault(c => string.Equals(c.Id, target.Id, StringComparison.OrdinalIgnoreCase));

            IJob tJob = _availableJobs.FirstOrDefault(k => k.JobName.Equals(job, StringComparison.OrdinalIgnoreCase));
            IJob playerJob = GetPlayerJob(giveRank);

            if (playerJob != null)
            {
                if (target == caller)
                    caller.SendLocalizedMessage(_translations, "already_in_job");
                else
                    caller?.SendLocalizedMessage(_translations, "already_in_job_admin", target.Name);
                return;
            }

            if (tJob == null)
            {
                caller?.SendLocalizedMessage(_translations, "job_destroyed", job);
                return;
            }

            if (caller == target && tJob is PrivateJob @private)
            {
                if (_applicants.Any(k => k.Id == target.Id))
                    caller.SendLocalizedMessage(_translations, "already_applying");
                else
                {
                    var players = GetOnlinePlayersInGroup(@private.LeaderPermissionGroup).ToList();
                    if (players.Count <= 0)
                    {
                        caller.SendLocalizedMessage(_translations, "no_leaders");
                        return;
                    }

                    _applicants.Add(new JobApplication() {Id = target.Id, Target = @private});
                    var player = _globalUserManager.OnlineUsers.First(k => k.Id == players.First().Id);
                    player.SendLocalizedMessage(_translations, "player_applying",
                        target.Name, @private.JobName);
                    caller.SendLocalizedMessage(_translations, "job_applied");
                }

                return;
            }

            IPermissionGroup targetGroup = _permissionProvider.GetGroup(tJob.PermissionGroup);
            ClearApplications(giveRank);

            if (_permissionProvider.AddGroup(giveRank, targetGroup))
                _globalUserManager.BroadcastLocalized(_translations, "joined_job", target.Name, tJob.JobName);
            else
                caller.SendLocalizedMessage(_translations, "failed_add_job");
        }
        public void AcceptApplication(IUserInfo target, IUser caller)
        {
            IUser giveRank = target.UserManager.OnlineUsers.First(c => string.Equals(c.Id, target.Id, StringComparison.OrdinalIgnoreCase));
            JobApplication jobApp = _applicants.FirstOrDefault(k => k.Id == giveRank.Id);

            if (jobApp == null)
            {
                caller.SendLocalizedMessage(_translations, "not_applying", target.Name);
                return;
            }

            IJob job = _availableJobs.FirstOrDefault(k =>
                k.JobName.Equals(jobApp.Target.JobName, StringComparison.OrdinalIgnoreCase));
            if (job == null)
            {
                caller.SendLocalizedMessage(_translations, "job_destroyed", jobApp.Target);
                return;
            }

            if (!(job is PrivateJob @private))
            {
                caller.SendLocalizedMessage(_translations, "special_error");
                return;
            }

            IPermissionGroup leaderGroup = _permissionProvider.GetGroup(@private.LeaderPermissionGroup);
            IEnumerable<IPermissionGroup> permissionGroups = _permissionProvider.GetGroups(caller);

            if (!permissionGroups.Contains(leaderGroup))
            {
                caller.SendLocalizedMessage(_translations, "not_leader_of_job",
                    job.JobName);
                return;
            }

            IPermissionGroup targetGroup = _permissionProvider.GetGroup(job.PermissionGroup);

            if (_permissionProvider.AddGroup(giveRank, targetGroup))
                _globalUserManager.BroadcastLocalized(_translations, "joined_job", target.Name,
                    job.JobName);
            else
                caller.SendLocalizedMessage(_translations, "failed_add_job");
        }
        public void ClearApplications(IUser caller = null)
        {
            _applicants.Clear();
            caller?.SendLocalizedMessage(_translations, "applications_cleared");
        }
        public void HandlePlayerDisconnect(string id)
        {
            _applicants.RemoveAll(k => id.Equals(k.Id, StringComparison.OrdinalIgnoreCase));
        }
        public void ListJobs(IUser caller)
        {
            string allJobs = string.Join(", ", _availableJobs.Select(c => c.JobName).ToArray()) + ".";
            caller.SendLocalizedMessage(_translations, "list_jobs", allJobs);
        }
        public void ClearAll()
        {
            ClearApplications();
            _availableJobs.Clear();
        }

        public IJob GetPlayerJob(IUser target)
        {
            IEnumerable<IPermissionGroup> groups = _permissionProvider.GetGroups(target);
            return _availableJobs.FirstOrDefault(k => groups.Any(l => k.PermissionGroup.Equals(l.Id, StringComparison.OrdinalIgnoreCase)));
        }
        private IEnumerable<IUser> GetOnlinePlayersInGroup(string groupName)
        {
            return _globalUserManager.OnlineUsers
                .Where(k => !(k is IConsole))
                .Where(k => _permissionProvider.GetGroups(k)
                    .Any(l => l.Id.Equals(groupName, StringComparison.OrdinalIgnoreCase)));
        }
    }
}
