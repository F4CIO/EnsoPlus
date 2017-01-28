using System;
using System.Collections.Generic;
using System.Text;

namespace EnsoPlus.Entities
{
    public class MergedCommand: Command
    {        
        public List<ICommandsProvider> commandsProviders;
        public List<Command> sourceCommands;

        public MergedCommand()
        {
        }

    }
}
