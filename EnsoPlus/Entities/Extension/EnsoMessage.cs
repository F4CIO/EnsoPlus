using System;
using System.Collections.Generic;
using System.Text;

namespace Extension
{
    public class EnsoMessage
    {
        private string _text;
        private string _subtext;


        public EnsoMessage(string text, string subtext = null)
        {
            if (text == null)
                throw new ArgumentNullException("text");
            this._text = text;
            this._subtext = subtext;
        }

		public string Text {
			get { return this._text; }
		}

	    public string Subtext
	    {
		    get { return this._subtext; }
	    }

	    public override string ToString()
        {
            if (String.IsNullOrEmpty(_subtext))
                return String.Format("{0}", _text);
            else
                return String.Format("<p>{0}</p><caption>{1}</caption>", _text, _subtext);
        }
    }
}
