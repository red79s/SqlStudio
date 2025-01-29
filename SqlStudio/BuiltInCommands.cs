using System;
using System.Collections.Generic;
using System.Text;

using CfgDataStore;

namespace SqlStudio
{
    class BuiltInCommands
    {
        CommandPrompt.CmdLineControl _cmdLineControl = null;
        IConfigDataStore _cfgDataStore = null;

        public BuiltInCommands(CommandPrompt.CmdLineControl cmdLineControl, IConfigDataStore cfgDataStore)
        {
            _cfgDataStore = cfgDataStore;
            _cmdLineControl = cmdLineControl;
        }

        public bool HandleBuiltIn(ref List<string> cmds)
        {
            var substitutedCommand = false;
            if (cmds.Count < 1)
                return substitutedCommand;

            int i = 0;
            while (true)
            {
                string newCmd = ProcessCommand(cmds[i]);
                if (newCmd == null)
                    cmds.RemoveAt(i);
                else
                {
                    if (cmds[i] != newCmd)
                        substitutedCommand = true;

                    cmds[i] = newCmd;
                    i++;
                }

                if (i >= cmds.Count)
                    return substitutedCommand;
            }
        }

        private string ProcessCommand(string cmd)
        {
            ArgumentsParser args = new ArgumentsParser(cmd);
            switch(args.GetCommand().ToLower())
            {
                case "alias": return ProcessAlias(cmd, args);
                case "history": return ProcessHistory(cmd, args);
                default: return ProcessAliases(cmd, args);
            }
        }

        private string ProcessAliases(string cmd, ArgumentsParser args)
        {
            string alias = _cfgDataStore.GetAlias(args.GetCommand());
            if (alias == null)
                return cmd;
            
            alias = alias.Trim();

            if (args.NumNonNamedArgs > 0)
            {
                alias = string.Format(alias, args.GetNonNamedArgs());
            }

            return alias;
        }

        private string ProcessHistory(string cmd, ArgumentsParser args)
        {
            if (args.GetFlags().Length == 0 || args.FlagIsSet('l'))
            {
                var historyItems = _cmdLineControl.GetHistoryItems();
                if (historyItems.Count < 1)
                {
                    _cmdLineControl.InsertCommandOutput("\nNo history items saved");
                }
                else
                {
                    for (int i = 0; i < historyItems.Count; i++)
                    {
                        _cmdLineControl.InsertCommandOutput($"\n{i.ToString("D3")}:{historyItems[i]}");
                    }
                }
            }
            else if (args.FlagIsSet('c'))
            {
                _cfgDataStore.ClearHistory();
                _cfgDataStore.Save();
                _cmdLineControl.ClearHistoryItems();
            }
            return null;
        }

        private string ProcessAlias(string cmd, ArgumentsParser args)
        {
            if (args.FlagIsSet('r'))
            {
                if (args.NumNonNamedArgs > 0)
                {
                    if (_cfgDataStore.RemoveAlias(args.GetNonNamedArg(0)))
                        _cmdLineControl.InsertCommandOutput(string.Format("\nRemoved alias \"{0}\" successfully", args.GetNonNamedArg(0)));
                    else
                        _cmdLineControl.InsertCommandOutput("\nAlias doesn't exist!");
                }
                else
                    _cmdLineControl.InsertCommandOutput("\nError: no alias specified!");
            }
            else if (args.FlagIsSet('a'))
            {
                if (args.NumNonNamedArgs > 1)
                {
                    _cfgDataStore.AddAlias(args.GetNonNamedArg(0), args.GetNonNamedArg(1));
                    _cmdLineControl.InsertCommandOutput(string.Format("\nAdded alias \"{0}\" successfully", args.GetNonNamedArg(0)));
                }
                else
                {
                    _cmdLineControl.InsertCommandOutput("\nUsage: alias -a alias_name alias_text");
                }
            }
            else if (args.FlagIsSet('l'))
            {
                List<Alias> aliases = null;
                if (args.NumNonNamedArgs > 0)
                {
                    aliases = _cfgDataStore.AliasSearch(args.GetNonNamedArg(0));
                }
                else
                {
                    aliases = _cfgDataStore.GetAliases();
                }

                foreach (var alias in aliases)
                {
                    _cmdLineControl.InsertCommandOutput(string.Format("\n{0} : {1}", alias.alias_name, alias.alias_value));
                }
                if (aliases.Count < 1)
                    _cmdLineControl.InsertCommandOutput("\nNo aliases found");
            }
            
            return null;
        }
    }
}
