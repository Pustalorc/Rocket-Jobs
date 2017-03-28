using Rocket.API;
using Rocket.API.Serialisation;
using Rocket.Core;
using Rocket.Core.Logging;
using Rocket.Core.Permissions;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Steamworks;
using System.Collections.Generic;

namespace Rocket_Jobs
{
    public class CommandJoinApplyJob : IRocketCommand
    {
        public AllowedCaller AllowedCaller
        {
            get { return AllowedCaller.Player; }
        }

        public string Name
        {
            get { return "JoinJob"; }
        }

        public string Help
        {
            get { return "Joins a public job or applies to a private job."; }
        }

        public string Syntax
        {
            get { return "<job name>"; }
        }

        public List<string> Aliases
        {
            get { return new List<string>() { "JJ", "JJob" }; }
        }

        public void Execute(IRocketPlayer caller, string[] command)
        {
            if (caller != null)
            {
                RocketPermissionsManager Permissions = R.Instance.GetComponent<RocketPermissionsManager>();
                UnturnedPlayer Player = (UnturnedPlayer)caller;
                CSteamID ID = Player.CSteamID;
                if (command.Length != 1)
                {
                    UnturnedChat.Say(caller, RocketJobs.Instance.Translate("join_apply_usage"));
                    return;
                }
                else if (command.Length == 1)
                {
                    #region PreventMakingMoreThan1Application
                    if (Appliances.Applications.ContainsKey(Player.CSteamID))
                    {
                        UnturnedChat.Say(caller, RocketJobs.Instance.Translate("error_already_applying"));
                    }
                    #endregion
                    #region PreventDoubleJoining
                    foreach (PublicJobs Job in RocketJobs.Instance.ConfigPubJobs)
                    {
                        RocketPermissionsGroup Group = Permissions.GetGroup(Job.PermissionGroup);
                        if (Group != null)
                        {
                            foreach (string IDS in Group.Members)
                            {
                                if (IDS == ID.ToString())
                                {
                                    UnturnedChat.Say(caller, RocketJobs.Instance.Translate("error_already_in_a_job"));
                                    return;
                                }
                            }
                        }
                        Logger.LogWarning("Permission Group " + Job.PermissionGroup + " does not exist!");
                    }
                    foreach (PrivateJobs Job in RocketJobs.Instance.ConfigPrivJobs)
                    {
                        RocketPermissionsGroup Group = Permissions.GetGroup(Job.PermissionGroup);
                        if (Group != null)
                        {
                            foreach (string IDS in Group.Members)
                            {
                                if (IDS == ID.ToString())
                                {
                                    UnturnedChat.Say(caller, RocketJobs.Instance.Translate("error_already_in_a_job"));
                                    return;
                                }
                            }
                        }
                        else if (Group == null)
                        {
                            Logger.LogWarning("Permission Group " + Job.PermissionGroup + " does not exist!");
                        }
                        RocketPermissionsGroup Group2 = Permissions.GetGroup(Job.LeaderPermissionGroup);
                        if (Group2 != null)
                        {
                            foreach (string IDS in Group2.Members)
                            {
                                if (IDS == ID.ToString())
                                {
                                    UnturnedChat.Say(caller, RocketJobs.Instance.Translate("error_leader_of_a_job"));
                                    return;
                                }
                            }
                        }
                        else if (Group2 == null)
                        {
                            Logger.LogWarning("Permission Group " + Job.LeaderPermissionGroup + " does not exist!");
                        }
                    }
                    #endregion 
                    #region FindAndJoin
                    foreach (PublicJobs Jobs in RocketJobs.Instance.ConfigPubJobs)
                    {
                        if (Jobs.JobName.ToLower() == command[0].ToLower())
                        {
                            RocketPermissionsGroup Group = Permissions.GetGroup(Jobs.PermissionGroup);
                            if (Group != null)
                            {
                                Permissions.AddPlayerToGroup(Jobs.PermissionGroup, caller);
                                UnturnedChat.Say(caller, RocketJobs.Instance.Translate("notification_quiet_joined_job"));
                                if (RocketJobs.Instance.Configuration.Instance.AnnounceJobJoin)
                                {
                                    UnturnedChat.Say(RocketJobs.Instance.Translate("notification_global_joined_job", Player.CharacterName, Jobs.JobName));
                                }
                                return;
                            }
                            else if (Group == null)
                            {
                                UnturnedChat.Say(caller, RocketJobs.Instance.Translate("error_contact_admin"));
                                Logger.LogWarning("Permission Group " + Jobs.PermissionGroup + " does not exist!");
                            }
                        }
                    }
                    foreach (PrivateJobs Jobs in RocketJobs.Instance.ConfigPrivJobs)
                    {
                        if (Jobs.JobName.ToLower() == command[0].ToLower())
                        {
                            RocketPermissionsGroup Group = Permissions.GetGroup(Jobs.LeaderPermissionGroup);
                            if (Group != null)
                            {
                                foreach (string IDS in Group.Members)
                                {
                                    foreach (SteamPlayer player in Provider.clients)
                                    {
                                        if (IDS == player.playerID.steamID.ToString())
                                        {
                                            UnturnedPlayer target = UnturnedPlayer.FromCSteamID(player.playerID.steamID);
                                            Appliances.Applications.Add(ID, Jobs.JobName);
                                            UnturnedChat.Say(target, RocketJobs.Instance.Translate("notification_player_applying", Player.CharacterName));
                                            UnturnedChat.Say(caller, RocketJobs.Instance.Translate("notification_applied_to_job", Jobs.JobName));
                                            return;
                                        }
                                    }
                                }
                                UnturnedChat.Say(caller, RocketJobs.Instance.Translate("error_leader_offline", Jobs.JobName));
                            }
                            else if (Group == null)
                            {
                                UnturnedChat.Say(caller, RocketJobs.Instance.Translate("error_contact_admin"));
                                Logger.LogWarning("Permission Group " + Jobs.PermissionGroup + " does not exist!");
                            }
                        }
                    }
                    #endregion
                    UnturnedChat.Say(caller, RocketJobs.Instance.Translate("error_job_not_found", command[0].ToLower()));
                }
            }
        }

        public List<string> Permissions
        {
            get
            {
                return new List<string>
                {
                    "JoinJob"
                };
            }
        }
    }
}
