using Rocket.API;
using Rocket.API.Serialisation;
using Rocket.Core;
using Rocket.Core.Permissions;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Steamworks;
using System.Collections.Generic;

namespace Rocket_Jobs
{
    class CommandJobAccept : IRocketCommand
    {
        public AllowedCaller AllowedCaller
        {
            get { return AllowedCaller.Player; }
        }

        public string Name
        {
            get { return "AcceptJob"; }
        }

        public string Help
        {
            get { return "Accepts a join request to your group."; }
        }

        public string Syntax
        {
            get { return "<player name>"; }
        }

        public List<string> Aliases
        {
            get { return new List<string>() { "AJ", "AJob", "AcceptJ" }; }
        }

        public void Execute(IRocketPlayer caller, string[] command)
        {
            RocketPermissionsManager Permissions = R.Instance.GetComponent<RocketPermissionsManager>();
            if (caller != null)
            {
                UnturnedPlayer Player = (UnturnedPlayer)caller;
                CSteamID ID = Player.CSteamID;
                if (command.Length != 1)
                {
                    UnturnedChat.Say(caller, RocketJobs.Instance.Translate("accept_usage"));
                    return;
                }
                else if (command.Length == 1)
                {
                    UnturnedPlayer Target = UnturnedPlayer.FromName(command[0].ToLower());
                    if (Appliances.Applications.ContainsKey(Target.CSteamID))
                    {
                        string JobName = Appliances.Applications[Target.CSteamID];
                        foreach (PrivateJobs Job in RocketJobs.Instance.ConfigPrivJobs)
                        {
                            if (Job.JobName.ToLower() == JobName.ToLower())
                            {
                                RocketPermissionsGroup Group = Permissions.GetGroup(Job.LeaderPermissionGroup);
                                foreach (string IDS in Group.Members)
                                {
                                    if (IDS == ID.ToString())
                                    {
                                        Permissions.AddPlayerToGroup(Job.PermissionGroup, Target);
                                        UnturnedChat.Say(caller, RocketJobs.Instance.Translate("notification_accepted_application", Target.CharacterName));
                                        UnturnedChat.Say(Target, RocketJobs.Instance.Translate("notification_quiet_joined_job", Job.JobName));
                                        if (RocketJobs.Instance.Configuration.Instance.AnnounceJobJoin)
                                        {
                                            UnturnedChat.Say(RocketJobs.Instance.Translate("notification_global_joined_job", Target.CharacterName, Job.JobName));
                                        }
                                        return;
                                    }
                                }
                                UnturnedChat.Say(caller, RocketJobs.Instance.Translate("error_not_leader_of_job", Job.JobName));
                                return;
                            }
                        }
                        UnturnedChat.Say(caller, RocketJobs.Instance.Translate("error_invalid_job_in_storage"));
                        return;
                    }
                    UnturnedChat.Say(caller, RocketJobs.Instance.Translate("error_player_not_applying"));
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
                    "AcceptJob"
                };
            }
        }
    }
}
