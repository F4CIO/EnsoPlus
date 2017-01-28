using System;

namespace Extension
{
    public class EnsoCommand : IComparable<EnsoCommand>
    {
        public EnsoCommand()
        {

        }

        //Should be deleted
        public EnsoCommand(string name, string postfix, string description, string help, EnsoPostfixType postfixType)
        {
            if (name == null)
                throw new ArgumentNullException("command");

            if (postfix == null && (postfixType == EnsoPostfixType.Arbitrary || postfixType == EnsoPostfixType.Bounded))
                throw new ArgumentNullException("postfix");

            if (description == null)
                throw new ArgumentNullException("description");

            if (help == null)
                throw new ArgumentNullException("help");

            this.Name = name;
            this.Postfix = postfix;
            this.Description = description;
            this.Help = help;
            this.PostfixType = postfixType;
        }

        public string Name;
       
        public string Postfix;

        public string Description;

        public string Help;

        public EnsoPostfixType PostfixType;

        

        public override string ToString()
        {
            string expression = null;
                if (PostfixType == EnsoPostfixType.None)
                    expression = Name.ToLower();
                else
                    expression = String.Format("{0} {{{1}}}", Name.ToLower(), Postfix.ToLower());
            
            return expression;
        }

        public override bool Equals(Object obj)
        {
            if (Object.ReferenceEquals(this, obj))
                return true;
            EnsoCommand other = obj as EnsoCommand;
            if (other == null)
                return false;
            return String.Equals(ToString(), other.ToString());
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        #region IComparable<EnsoCommand> Members

        int IComparable<EnsoCommand>.CompareTo(EnsoCommand other)
        {
            return String.CompareOrdinal(ToString(), other.ToString());
        }

        #endregion
    }
}
