using System.Collections.Generic;
using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Core.Logging;
using System;

namespace Rocket_Jobs
{
    public class CommandJobs : IRocketCommand
    {
        public AllowedCaller AllowedCaller
        {
            get { return AllowedCaller.Both; }
        }

        public string Name
        {
            get { return "Jobs"; }
        }

        public string Help
        {
            get { return "Lists the available jobs."; }
        }

        public string Syntax
        {
            get { return "<private | public> <page number>"; }
        }

        public List<string> Aliases
        {
            get { return new List<string>() { "LJ", "LJobs", "ListJobs" }; }
        }

        public void Execute(IRocketPlayer caller, string[] command)
        {
            if (caller is ConsolePlayer)
            {
                Logger.LogWarning("Public Jobs:");
                foreach (PublicJobs a in RocketJobs.Instance.ConfigPubJobs)
                {
                    Logger.LogWarning("Name: " + a.JobName + ", Permission Group: " + a.PermissionGroup);
                }
                Logger.LogWarning("Private Jobs:");
                foreach (PrivateJobs a in RocketJobs.Instance.ConfigPrivJobs)
                {
                    Logger.LogWarning("Name: " + a.JobName + ", Permission Group: " + a.PermissionGroup);
                }
            }
            else if (!(caller is ConsolePlayer))
            {
                if (command.Length == 0 || command.Length > 2)
                {
                    UnturnedChat.Say(caller, RocketJobs.Instance.Translate("list_usage"));
                    return;
                }
                else if (command.Length == 1)
                {
                    switch (command[0].ToLower())
                    {
                        case "public":
                            if (RocketJobs.Instance.ConfigPubJobs.Count > 0 && RocketJobs.Instance.ConfigPubJobs.Count <= 4)
                            {
                                foreach (PublicJobs a in RocketJobs.Instance.ConfigPubJobs)
                                {
                                    UnturnedChat.Say(caller, RocketJobs.Instance.Translate("pub_job_notification", a.JobName));
                                }
                                if (RocketJobs.Instance.ConfigPubJobs.Count != 4)
                                {
                                    UnturnedChat.Say(caller, RocketJobs.Instance.Translate("end_of_list", command[0].ToLower()));
                                }
                                return;
                            }
                            else if (RocketJobs.Instance.ConfigPubJobs.Count > 4)
                            {
                                UnturnedChat.Say(caller, RocketJobs.Instance.Translate("pub_job_notification", RocketJobs.Instance.ConfigPubJobs[0].JobName));
                                UnturnedChat.Say(caller, RocketJobs.Instance.Translate("pub_job_notification", RocketJobs.Instance.ConfigPubJobs[1].JobName));
                                UnturnedChat.Say(caller, RocketJobs.Instance.Translate("pub_job_notification", RocketJobs.Instance.ConfigPubJobs[2].JobName));
                                UnturnedChat.Say(caller, RocketJobs.Instance.Translate("next_page_notification", command[0].ToLower(), 2));
                            }
                            break;
                        case "private":
                            if (RocketJobs.Instance.ConfigPrivJobs.Count > 0 && RocketJobs.Instance.ConfigPrivJobs.Count <= 4)
                            {
                                foreach (PrivateJobs a in RocketJobs.Instance.ConfigPrivJobs)
                                {
                                    UnturnedChat.Say(caller, RocketJobs.Instance.Translate("priv_job_notification", a.JobName));
                                }
                                if (RocketJobs.Instance.ConfigPrivJobs.Count != 4)
                                {
                                    UnturnedChat.Say(caller, RocketJobs.Instance.Translate("end_of_list", command[0].ToLower()));
                                }
                                return;
                            }
                            else if (RocketJobs.Instance.ConfigPrivJobs.Count > 4)
                            {
                                UnturnedChat.Say(caller, RocketJobs.Instance.Translate("priv_job_notification", RocketJobs.Instance.ConfigPrivJobs[0].JobName));
                                UnturnedChat.Say(caller, RocketJobs.Instance.Translate("priv_job_notification", RocketJobs.Instance.ConfigPrivJobs[1].JobName));
                                UnturnedChat.Say(caller, RocketJobs.Instance.Translate("priv_job_notification", RocketJobs.Instance.ConfigPrivJobs[2].JobName));
                                UnturnedChat.Say(caller, RocketJobs.Instance.Translate("next_page_notification", command[0].ToLower(), 2));
                            }
                            break;
                        default:
                            UnturnedChat.Say(caller, RocketJobs.Instance.Translate("list_usage"));
                            break;
                    }
                }
                else if (command.Length == 2)
                {
                    switch (command[0].ToLower())
                    {
                        case "public":
                            int PageNumber;
                            try
                            {
                                PageNumber = Convert.ToInt32(command[1]);
                            }
                            catch (FormatException)
                            {
                                UnturnedChat.Say(caller, RocketJobs.Instance.Translate("format_error", command[1]));
                                return;
                            }
                            catch (OverflowException)
                            {
                                UnturnedChat.Say(caller, RocketJobs.Instance.Translate("overflow_error", command[1]));
                                return;
                            }
                            int Min = (PageNumber * 4) - 4;
                            int Max = (PageNumber * 4) - 1;
                            int MaxPages = (RocketJobs.Instance.ConfigPubJobs.Count / 3) + 1;
                            if (PageNumber < MaxPages)
                            {
                                for (int i = Min; i <= Max; i++)
                                {
                                    if (i == Max)
                                    {
                                        UnturnedChat.Say(caller, RocketJobs.Instance.Translate("next_page_notification", command[0].ToLower(), (PageNumber + 1)));
                                        break;
                                    }
                                    else
                                    {
                                        UnturnedChat.Say(caller, RocketJobs.Instance.Translate("pub_job_notification", RocketJobs.Instance.ConfigPubJobs[i - (PageNumber - 1)].JobName));
                                    }
                                }
                            }
                            else if (PageNumber == MaxPages)
                            {
                                int CurrentJob = 1;
                                int JobCount = RocketJobs.Instance.ConfigPubJobs.Count % 3;
                                for (int i = Min; i <= Max; i++)
                                {
                                    if (CurrentJob > JobCount)
                                    {
                                        UnturnedChat.Say(caller, RocketJobs.Instance.Translate("end_of_list", command[0].ToLower()));
                                        break;
                                    }
                                    else
                                    {
                                        UnturnedChat.Say(caller, RocketJobs.Instance.Translate("pub_job_notification", RocketJobs.Instance.ConfigPubJobs[i - (PageNumber - 1)].JobName));
                                    }
                                }
                            }
                            else if (PageNumber > MaxPages)
                            {
                                UnturnedChat.Say(caller, RocketJobs.Instance.Translate("unexistant_page"));
                            }
                            break;
                        case "private":
                            try
                            {
                                PageNumber = Convert.ToInt32(command[1]);
                            }
                            catch (FormatException)
                            {
                                UnturnedChat.Say(caller, RocketJobs.Instance.Translate("format_error", command[1]));
                                return;
                            }
                            catch (OverflowException)
                            {
                                UnturnedChat.Say(caller, RocketJobs.Instance.Translate("overflow_error", command[1]));
                                return;
                            }
                            Min = (PageNumber * 4) - 4;
                            Max = (PageNumber * 4) - 1;
                            MaxPages = (RocketJobs.Instance.ConfigPrivJobs.Count / 3) + 1;
                            if (PageNumber < MaxPages)
                            {
                                for (int i = Min; i <= Max; i++)
                                {
                                    if (i == Max)
                                    {
                                        UnturnedChat.Say(caller, RocketJobs.Instance.Translate("next_page_notification", command[0].ToLower(), (PageNumber + 1)));
                                        break;
                                    }
                                    else
                                    {
                                        UnturnedChat.Say(caller, RocketJobs.Instance.Translate("priv_job_notification", RocketJobs.Instance.ConfigPrivJobs[i - (PageNumber - 1)].JobName));
                                    }
                                }
                            }
                            else if (PageNumber == MaxPages)
                            {
                                int CurrentJob = 1;
                                int JobCount = RocketJobs.Instance.ConfigPrivJobs.Count % 3;
                                for (int i = Min; i <= Max; i++)
                                {
                                    if (CurrentJob > JobCount)
                                    {
                                        UnturnedChat.Say(caller, RocketJobs.Instance.Translate("end_of_list", command[0].ToLower()));
                                        break;
                                    }
                                    else
                                    {
                                        UnturnedChat.Say(caller, RocketJobs.Instance.Translate("priv_job_notification", RocketJobs.Instance.ConfigPrivJobs[i - (PageNumber - 1)].JobName));
                                    }
                                }
                            }
                            else if (PageNumber > MaxPages)
                            {
                                UnturnedChat.Say(caller, RocketJobs.Instance.Translate("unexistant_page"));
                            }
                            break;
                        default:
                            UnturnedChat.Say(caller, RocketJobs.Instance.Translate("list_usage"));
                            break;
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
                    "Jobs"
                };
            }
        }
    }
}
