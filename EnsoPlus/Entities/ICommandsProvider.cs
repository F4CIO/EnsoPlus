using System;
using System.Collections.Generic;
using System.Text;

namespace EnsoPlus.Entities
{
    public interface ICommandsProvider
    {
        List<Command> GetCommands();
        void ProcessingBeforeParameterInput(Command command, ref bool cancel);
        void ExecuteCommand(Extension.IEnsoService service, Command command);
    }
}
