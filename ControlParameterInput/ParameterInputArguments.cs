using System;
using System.Collections.Generic;
using System.Text;

namespace Extension
{
    public struct ParameterInputArguments
    {
        public string caption;
        public List<string> suggestions;
        public bool offerAllSuggestions; 
        public bool readOnly; 
        public string predefinedValue; 
        public bool acceptOnlySuggested;
        public bool caseSensitive;

        public ParameterInputArguments(string caption, List<string> suggestions, bool offerAllSuggestions, bool readOnly, string predefinedValue, bool acceptOnlySuggested, bool caseSensitive)
        {
            this.caption = caption;
            this.suggestions = suggestions;
            this.offerAllSuggestions = offerAllSuggestions;
            this.readOnly = readOnly;
            this.predefinedValue = predefinedValue;
            this.acceptOnlySuggested = acceptOnlySuggested;
            this.caseSensitive = caseSensitive;
        }
    }
}
