using System;
using System.Collections.Generic;
using System.Text;

using Extension;

namespace EnsoPlus.Entities
{
    public class Command:EnsoCommand
    {
        public ICommandsProvider provider;
        public List<Type> supportedParameterTypeProviders;
        public bool canUseTextSelectionForParameter;
        public bool canUseFileSelectionForParameter;
        public ParameterInputArguments parameterInputArguments;
        public List<IWorkItem> parametersOnExecute;

        public string caption
        {
            get
            {
                return string.Format("Command: {0} {1}", this.Name, this.Postfix);
            }
        }

        public Command()
        {
        }

        public Command(string name, string postfix, string description, string help, EnsoPostfixType postfixType, ICommandsProvider provider, bool canUseTextSelectionForParameter, bool canUseFileSelectionForParameter, ParameterInputArguments parameterInputArguments, params Type[] supportedParameterTypeProvidersArray)
        {           
            this.Name = name;
            this.Postfix = postfix;
            this.Description = description;
            this.Help = help;
            this.PostfixType = postfixType;
            this.provider = provider;
            this.supportedParameterTypeProviders = new List<Type>();
            this.canUseTextSelectionForParameter = canUseTextSelectionForParameter;
            this.canUseFileSelectionForParameter = canUseFileSelectionForParameter;
            this.supportedParameterTypeProviders.AddRange(supportedParameterTypeProvidersArray);
            this.parameterInputArguments = parameterInputArguments;
            this.parametersOnExecute = new List<IWorkItem>();
        }

        public Command GetClone()
        {
            Command clone = new Command();

            clone.Name = this.Name;
            clone.Postfix = this.Postfix;
            clone.Description = this.Description;
            clone.Help = this.Help;
            clone.PostfixType = this.PostfixType;
            clone.provider = this.provider;
            clone.supportedParameterTypeProviders = this.supportedParameterTypeProviders;
            clone.canUseTextSelectionForParameter = this.canUseTextSelectionForParameter;
            clone.canUseFileSelectionForParameter = this.canUseFileSelectionForParameter;
            clone.supportedParameterTypeProviders = this.supportedParameterTypeProviders;
            clone.parameterInputArguments = this.parameterInputArguments;
            clone.parametersOnExecute = this.parametersOnExecute;

            return clone;
        }

        internal void Execute(IEnsoService service)
        {
            this.provider.ExecuteCommand(service, this);
        }
    }
}
