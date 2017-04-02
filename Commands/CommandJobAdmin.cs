using Rocket.API;
using Rocket.API.Serialisation;
using Rocket.Core;
using Rocket.Core.Permissions;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using Steamworks;
using System.Collections.Generic;

namespace Rocket_Jobs
{
    class CommandJobAdmin : IRocketCommand
    {
        public AllowedCaller AllowedCaller
        {
            get { return AllowedCaller.Player; }
        }

        public string Name
        {
            get { return "JobAdmin"; }
        }

        public string Help
        {
            get { return "Administers jobs for players. Overrides most of the things."; }
        }

        public string Syntax
        {
            get { return "<add | remove | clear> <job name> <player name>"; }
        }

        public List<string> Aliases
        {
            get { return new List<string>() { "JA", "JAdmin", "JobA" }; }
        }

        public void Execute(IRocketPlayer caller, string[] command)
        {
            if (caller != null)
            {
                RocketPermissionsManager Permissions = R.Instance.GetComponent<RocketPermissionsManager>();
                UnturnedPlayer Player = (UnturnedPlayer)caller;
                CSteamID ID = Player.CSteamID;
                if (command.Length > 3 || command.Length < 2)
                {
                    UnturnedChat.Say(caller, RocketJobs.Instance.Translate("admin_usage"));
                    return;
                }
                else if (command.Length == 2)
                {
                    if (command[0].ToLower() == "clear")
                    {
                        #region FindGroupAndClear
                        foreach (PublicJobs Jobs in RocketJobs.Instance.ConfigPubJobs)
                        {
                            if (Jobs.JobName.ToLower() == command[1].ToLower())
                            {
                                RocketPermissionsGroup Group = Permissions.GetGroup(Jobs.PermissionGroup);
                                if (Group != null)
                                {
                                    Group.Members = new List<string>();
                                    return;
                                }
                                return;
                            }
                        }
                        foreach (PrivateJobs Jobs in RocketJobs.Instance.ConfigPrivJobs)
                        {
                            if (Jobs.JobName.ToLower() == command[1].ToLower())
                            {
                                RocketPermissionsGroup Group = Permissions.GetGroup(Jobs.PermissionGroup);
                                if (Group != null)
                                {
                                    Group.Members = new List<string>();
                                    return;
                                }
                                return;
                            }
                        }
                        #endregion
                        UnturnedChat.Say(caller, RocketJobs.Instance.Translate("error_job_not_found", command[1].ToLower()));
                        return;
                    }
                    else if (command[0].ToLower() == "add" || command[0].ToLower() == "remove")
                    {
                        UnturnedChat.Say(caller, RocketJobs.Instance.Translate("admin_usage"));
                        return;
                    }
                    UnturnedChat.Say(caller, RocketJobs.Instance.Translate("admin_usage"));
                    return;
                }
                else if (command.Length == 3)
                {
                    if (command[0].ToLower() == "clear")
                    {
                        #region FindGroupAndClear
                        foreach (PublicJobs Jobs in RocketJobs.Instance.ConfigPubJobs)
                        {
                            if (Jobs.JobName.ToLower() == command[1].ToLower())
                            {
                                RocketPermissionsGroup Group = Permissions.GetGroup(Jobs.PermissionGroup);
                                if (Group != null)
                                {
                                    Group.Members = new List<string>();
                                    return;
                                }
                                return;
                            }
                        }
                        foreach (PrivateJobs Jobs in RocketJobs.Instance.ConfigPrivJobs)
                        {
                            if (Jobs.JobName.ToLower() == command[1].ToLower())
                            {
                                RocketPermissionsGroup Group = Permissions.GetGroup(Jobs.PermissionGroup);
                                if (Group != null)
                                {
                                    Group.Members = new List<string>();
                                    return;
                                }
                                return;
                            }
                        }
                        #endregion
                        UnturnedChat.Say(caller, RocketJobs.Instance.Translate("error_job_not_found", command[1].ToLower()));
                        return;
                    }
                    else if (command[0].ToLower() == "add")
                    {
                        UnturnedPlayer Target = UnturnedPlayer.FromName(command[2].ToLower());
                        CSteamID TargetID = Target.CSteamID;
                        #region PreventDoubleJoining
                        foreach (PublicJobs Job in RocketJobs.Instance.ConfigPubJobs)
                        {
                            RocketPermissionsGroup Group = Permissions.GetGroup(Job.PermissionGroup);
                            if (Group != null)
                            {
                                foreach (string Members in Group.Members)
                                {
                                    if (Members == TargetID.ToString())
                                    {
                                        UnturnedChat.Say(caller, RocketJobs.Instance.Translate("error_already_in_a_job_admin"));
                                        return;
                                    }
                                }
                            }
                        }
                        foreach (PrivateJobs Job in RocketJobs.Instance.ConfigPrivJobs)
                        {
                            RocketPermissionsGroup Group = Permissions.GetGroup(Job.PermissionGroup);
                            if (Group != null)
                            {
                                foreach (string Members in Group.Members)
                                {
                                    if (Members == TargetID.ToString())
                                    {
                                        UnturnedChat.Say(caller, RocketJobs.Instance.Translate("error_already_in_a_job_admin"));
                                        return;
                                    }
                                }
                            }
                            RocketPermissionsGroup Group2 = Permissions.GetGroup(Job.LeaderPermissionGroup);
                            if (Group2 != null)
                            {
                                foreach (string IDS in Group2.Members)
                                {
                                    if (IDS == ID.ToString())
                                    {
                                        UnturnedChat.Say(caller, RocketJobs.Instance.Translate("error_leader_of_a_job_admin"));
                                        return;
                                    }
                                }
                            }
                        }
                        #endregion
                        #region FindGroupAndAdd
                        foreach (PublicJobs Jobs in RocketJobs.Instance.ConfigPubJobs)
                        {
                            if (Jobs.JobName.ToLower() == command[1].ToLower())
                            {
                                RocketPermissionsGroup Group = Permissions.GetGroup(Jobs.PermissionGroup);
                                if (Group != null)
                                {
                                    Permissions.AddPlayerToGroup(Jobs.PermissionGroup, Target);
                                    UnturnedChat.Say(caller, RocketJobs.Instance.Translate("notification_quiet_joined_job_admin", Target.CharacterName, Jobs.JobName));
                                    UnturnedChat.Say(Target, RocketJobs.Instance.Translate("notification_quiet_joined_job", Jobs.JobName));
                                    if (RocketJobs.Instance.Configuration.Instance.AnnounceJobJoin)
                                    {
                                        UnturnedChat.Say(RocketJobs.Instance.Translate("notification_global_joined_job", Target.CharacterName, Jobs.JobName));
                                    }
                                    return;
                                }
                                return;
                            }
                        }
                        foreach (PrivateJobs Jobs in RocketJobs.Instance.ConfigPrivJobs)
                        {
                            if (Jobs.JobName.ToLower() == command[1].ToLower())
                            {
                                RocketPermissionsGroup Group = Permissions.GetGroup(Jobs.PermissionGroup);
                                if (Group != null)
                                {
                                    Permissions.AddPlayerToGroup(Jobs.PermissionGroup, Target);
                                    UnturnedChat.Say(caller, RocketJobs.Instance.Translate("notification_quiet_joined_job_admin", Target.CharacterName, Jobs.JobName));
                                    UnturnedChat.Say(Target, RocketJobs.Instance.Translate("notification_quiet_joined_job", Jobs.JobName));
                                    if (RocketJobs.Instance.Configuration.Instance.AnnounceJobJoin)
                                    {
                                        UnturnedChat.Say(RocketJobs.Instance.Translate("notification_global_joined_job", Target.CharacterName, Jobs.JobName));
                                    }
                                    return;
                                }
                                return;
                            }
                        }
                        #endregion
                        UnturnedChat.Say(caller, RocketJobs.Instance.Translate("error_job_not_found", command[1].ToLower()));
                        return;
                    }
                    else if (command[0].ToLower() == "remove")
                    {
                        UnturnedPlayer Target = UnturnedPlayer.FromName(command[2].ToLower());
                        CSteamID TargetID = Target.CSteamID;
                        #region FindGroupAndRemove
                        foreach (PublicJobs Jobs in RocketJobs.Instance.ConfigPubJobs)
                        {
                            if (Jobs.JobName.ToLower() == command[1].ToLower())
                            {
                                RocketPermissionsGroup Group = Permissions.GetGroup(Jobs.PermissionGroup);
                                if (Group != null)
                                {
                                    foreach (string Member in Group.Members)
                                    {
                                        if (Member == TargetID.ToString())
                                        {
                                            Permissions.RemovePlayerFromGroup(Group.Id, Target);
                                            UnturnedChat.Say(Target, RocketJobs.Instance.Translate("notification_left_job", Jobs.JobName));
                                        }
                                    }
                                    UnturnedChat.Say(caller, RocketJobs.Instance.Translate("error_player_not_in_job", Jobs.JobName));
                                    return;
                                }
                                return;
                            }
                        }
                        foreach (PrivateJobs Jobs in RocketJobs.Instance.ConfigPrivJobs)
                        {
                            if (Jobs.JobName.ToLower() == command[1].ToLower())
                            {
                                RocketPermissionsGroup Group = Permissions.GetGroup(Jobs.PermissionGroup);
                                if (Group != null)
                                {
                                    foreach (string Member in Group.Members)
                                    {
                                        if (Member == TargetID.ToString())
                                        {
                                            Permissions.RemovePlayerFromGroup(Group.Id, Target);
                                            UnturnedChat.Say(Target, RocketJobs.Instance.Translate("notification_left_job", Jobs.JobName));
                                        }
                                    }
                                    UnturnedChat.Say(caller, RocketJobs.Instance.Translate("error_player_not_in_job", Jobs.JobName));
                                    return;
                                }
                                return;
                            }
                        }
                        #endregion
                        UnturnedChat.Say(caller, RocketJobs.Instance.Translate("error_job_not_found", command[1].ToLower()));
                    }
                    UnturnedChat.Say(caller, RocketJobs.Instance.Translate("admin_usage"));
                    return;
                }
            }
        }

        public List<string> Permissions
        {
            get
            {
                return new List<string>
                {
                    "JobAdmin"
                };
            }
        }
    }
}
