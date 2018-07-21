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
    class CommandJobKick : IRocketCommand
    {
        public AllowedCaller AllowedCaller
        {
            get { return AllowedCaller.Player; }
        }

        public string Name
        {
            get { return "KickJob"; }
        }

        public string Help
        {
            get { return "Kicks a player from your job."; }
        }

        public string Syntax
        {
            get { return "<player name>"; }
        }

        public List<string> Aliases
        {
            get { return new List<string>() { "KJ", "KJob", "KickJ" }; }
        }

        public void Execute(IRocketPlayer caller, string[] command)
        {
            RocketPermissionsManager Permissions = R.Instance.GetComponent<RocketPermissionsManager>();
            if (caller != null)
            {
                if (command.Length != 1)
                {
                    UnturnedChat.Say(caller, RocketJobs.Instance.Translate("kick_usage"));
                    return;
                }
                else if (command.Length == 1)
                {
                    UnturnedPlayer Player = (UnturnedPlayer)caller;
                    CSteamID ID = Player.CSteamID;
                    UnturnedPlayer Target = UnturnedPlayer.FromName(command[0].ToLower());
                    CSteamID TargetID = Target.CSteamID;

                    foreach (PrivateJobs Jobs in RocketJobs.Instance.ConfigPrivJobs)
                    {
                        RocketPermissionsGroup g = Permissions.GetGroup(Jobs.LeaderPermissionGroup);
                        if (g.Members.Exists(k => k == ID.ToString()))
                        {
                            RocketPermissionsGroup Group = Permissions.GetGroup(Jobs.PermissionGroup);
                            if (Group != null && Group.Members.Exists(k => k == TargetID.ToString()))
                            {
                                Permissions.RemovePlayerFromGroup(Group.Id, Target);
                                UnturnedChat.Say(Target, RocketJobs.Instance.Translate("notification_left_job", Jobs.JobName));
                                UnturnedChat.Say(caller, RocketJobs.Instance.Translate("notification_kicked", Target.DisplayName));
                            }
                        }
                    }
                }
            }
        }

        public List<string> Permissions
        {
            get
            {
                return new List<string>
                {
                    "KickJob"
                };
            }
        }
    }
}
