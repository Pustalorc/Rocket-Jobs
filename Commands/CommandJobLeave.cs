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
    class CommandJobLeave : IRocketCommand
    {
        public AllowedCaller AllowedCaller
        {
            get { return AllowedCaller.Player; }
        }

        public string Name
        {
            get { return "LeaveJob"; }
        }

        public string Help
        {
            get { return "Leaves the job you are currently in."; }
        }

        public string Syntax
        {
            get { return null; }
        }

        public List<string> Aliases
        {
            get { return new List<string>() { "LJ", "LJob", "LeaveJ" }; }
        }

        public void Execute(IRocketPlayer caller, string[] command)
        {
            RocketPermissionsManager Permissions = R.Instance.GetComponent<RocketPermissionsManager>();
            if (caller != null)
            {
                UnturnedPlayer Player = (UnturnedPlayer)caller;
                CSteamID ID = Player.CSteamID;
                foreach (PublicJobs Job in RocketJobs.Instance.ConfigPubJobs)
                {
                    RocketPermissionsGroup Group = Permissions.GetGroup(Job.PermissionGroup);
                    foreach (string IDS in Group.Members)
                    {
                        if (IDS == ID.ToString())
                        {
                            Permissions.RemovePlayerFromGroup(Job.PermissionGroup, caller);
                            UnturnedChat.Say(caller, RocketJobs.Instance.Translate("notification_left_job"));
                            return;
                        }
                    }
                }
                foreach (PrivateJobs Job in RocketJobs.Instance.ConfigPrivJobs)
                {
                    RocketPermissionsGroup Group = Permissions.GetGroup(Job.PermissionGroup);
                    foreach (string IDS in Group.Members)
                    {
                        if (IDS == ID.ToString())
                        {
                            Permissions.RemovePlayerFromGroup(Job.PermissionGroup, caller);
                            UnturnedChat.Say(caller, RocketJobs.Instance.Translate("notification_left_job"));
                            return;
                        }
                    }
                }
                UnturnedChat.Say(caller, RocketJobs.Instance.Translate("error_not_in_a_job"));
            }
        }

        public List<string> Permissions
        {
            get
            {
                return new List<string>
                {
                    "LeaveJob"
                };
            }
        }
    }
}
