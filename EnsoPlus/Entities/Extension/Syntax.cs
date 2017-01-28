using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Common;

namespace Extension
{
    public class Syntax
    {
        public static string startOfSyntaxParameter
        {
            get
            {
                return Settings.Current.startOfSyntaxParameter;
            }
        }

        public static string endOfSyntaxParameter
        {
            get
            {
                return Settings.Current.endOfSyntaxParameter;
            }
        }

        public static string startOfPostfixParameter
        {
            get
            {
                return Settings.Current.startOfPostfixParameter;
            }
        }
        public static string endOfPostfixParameter
        {
            get
            {
                return Settings.Current.endOfPostfixParameter;
            }
        }
        public static string selectionInPostfix1
        {
            get
            {
                return Settings.Current.selectionInPostfix1;
            }
        }
        public static string selectionInPostfix2
        {
            get
            {
                return Settings.Current.selectionInPostfix2;
            }
        }
        public static string lastMessageInPostfix
        {
            get
            {
                return Settings.Current.lastMessageInPostfix;
            }
        }
        public static string lastParameterInPostfix
        {
            get
            {
                return Settings.Current.lastParameterInPostfix;
            }
        }

        public static string ExtractName(string syntax)
        {
            string[] words = syntax.Split(' ');
            return words[0].Trim();
        }

        public static string FileSelectionArrayToString(string[] filePaths)
        {
            StringBuilder sbFileSelection = new StringBuilder();
            for (int i = 0; i < filePaths.Length; i++)
            {
                sbFileSelection.Append("\r\n");
                sbFileSelection.Append(filePaths[i]);
            }
	        if (sbFileSelection.Length >= 2)
	        {
		        sbFileSelection.Remove(0, 2);
	        }
	        return sbFileSelection.ToString();
        }

        public static List<string> FileSelectionFromString(string fileSelectionString)
        {
            List<string> fileSelection = new List<string>();

            try
            {
                string[] fileSelectionArray = fileSelectionString.Split(new string[]{"\r\n"}, StringSplitOptions.None);
                for (int i = 0; i < fileSelectionArray.Length; i++)
                {
                    fileSelection.Add(fileSelectionArray[i]);
                }
            }
            catch
            {

            }

            return fileSelection;
        }

        public static string ExtractPostfix(string syntax)
        {
            string result = string.Empty;
            string[] words = syntax.Split(' ');
            int i = 1;
            while (i < words.Length)
            {
                if (i != 1) result += " ";

                result += words[i];

                i++;
            }
            return result.Trim();
        }

        //public static List<string> ExtractElements(string content, string startOfElement, string endOfElement)
        //{
        //    int result = 0;
        //    bool elementOppened = false;
        //    int i = 0;
        //    while (i < content.Length)
        //    {
        //        if (!elementOppened && content.Substring(i, startOfElement.Length) == startOfElement)
        //        {
        //            elementOppened = true;
        //        }
        //        else
        //            if (elementOppened && content.Substring(i, endOfElement.Length) == endOfElement)
        //            {
        //                result++;
        //                elementOppened = false;
        //            }
        //        i++;
        //    }
        //    return result;
        //}

        /// <summary>
        /// Using syntax parses postfix and returns parameters values.
        /// </summary>
        /// <param name="syntax">Example: from <source> to <destination> <what> </param>
        /// <param name="postfix">Example: from English to Dutch selection</param>
        /// <param name="parameterCountInSyntax">Example 3</param>
        /// <returns>Example: Engilsh, Dutch, selected text</returns>
        public static List<string> ExtractParameterValues(string syntax, string postfix, string selectedText, bool allowEmptySpaceInLastSimpleParameter, out int parameterCountInSyntax)
        {
			Trace.WriteLine("syntax="+syntax+" postfix="+postfix+" selText="+selectedText+" allowESpaceInLastParam="+allowEmptySpaceInLastSimpleParameter);
            List<string> parameters = new List<string>();

            if (selectedText == null) selectedText = "selection";

            List<string> syntaxElements = new List<string>();
            bool syntaxParameterOppened = false;
            int i = 0;
            while (i < syntax.Length)
            {
                if (!syntaxParameterOppened && syntax.Substring(i, startOfSyntaxParameter.Length) == startOfSyntaxParameter)
                {//param oppening
                    syntaxParameterOppened = true;
                    syntaxElements.Add("*");
                    i = i + startOfSyntaxParameter.Length;
                }
                else
                    if (syntaxParameterOppened && syntax.Substring(i, endOfSyntaxParameter.Length) == endOfSyntaxParameter)
                    {//param closing                          
                        syntaxParameterOppened = false;
                        i = i + endOfSyntaxParameter.Length;
                    }
                    else
                    {
                        if (syntaxParameterOppened)
                        {//skip char
                            i++;
                        }
                        else
                        {//expand decoration item with char                            
                            if (syntaxElements.Count == 0 || syntaxElements[syntaxElements.Count - 1] == "*")
                            {//start new decoration
                                syntaxElements.Add(syntax[i].ToString());
                            }
                            else
                            {//expand decoration
                                syntaxElements[syntaxElements.Count - 1] = syntaxElements[syntaxElements.Count - 1] + syntax[i];
                            }
                            i++;
                        }
                    }
            }

            int p = 0;
            int s = 0;
            bool postfixParameterOppened = false;
            while (p < postfix.Length && s < syntaxElements.Count)
            {
                if (syntaxElements[s] == "*")
                {//expand postfix parameter with char
                    if (!postfixParameterOppened && p + startOfPostfixParameter.Length - 1 < postfix.Length && postfix.Substring(p, startOfPostfixParameter.Length) == startOfPostfixParameter)
                    {//open brecketed parameter
                        postfixParameterOppened = true;
                        p = p + startOfPostfixParameter.Length;
                    }
                    if ((!postfixParameterOppened && postfix[p].ToString() == " " && s<syntaxElements.Count-2)||
                        (!postfixParameterOppened && postfix[p].ToString() == " " && !allowEmptySpaceInLastSimpleParameter)
                        )
                    {//end simple parameter
                        s++;
                        parameters.Add(string.Empty);
                    }
                    else
                        if (postfixParameterOppened && postfix.Substring(p, endOfPostfixParameter.Length) == endOfPostfixParameter)
                        {//end bracketed parameter
                            s++;
                            p = p + endOfPostfixParameter.Length;
                            parameters.Add(string.Empty);
                            postfixParameterOppened = false;
                        }
                        else
                        {//expand postfix parameter with char
                            if (parameters.Count == 0)
                            {
                                parameters.Add(postfix[p].ToString());
                            }
                            else
                            {
                                parameters[parameters.Count - 1] = parameters[parameters.Count - 1] + postfix[p];
                            }
                            p++;
                        }
                }
                else
                {//skip decoration
                    if (p + syntaxElements[s].Length - 1 < postfix.Length && postfix.Substring(p, syntaxElements[s].Length) == syntaxElements[s])
                    {
                        p = p + syntaxElements[s].Length;
                        s++;
                    }
                    else
                    {//invalid decoration in postfix
                        parameterCountInSyntax = syntaxElements.FindAll(IsParameter).Count;
                        return null;
                    }
                }
            }
            if (p < postfix.Length && s >= syntaxElements.Count && !allowEmptySpaceInLastSimpleParameter)
            {//all syntaxElements parsed but some chars left in postfix
                parameterCountInSyntax = syntaxElements.FindAll(IsParameter).Count;
                return null;
            }
            if (parameters.Count > 0 && parameters[parameters.Count - 1] == string.Empty)
            {
                parameters.RemoveAt(parameters.Count - 1);
            }
            for (int i1 = 0; i1 < parameters.Count; i1++)
            {//replace selection jocker with selected text
                if (string.Compare(parameters[i1], selectionInPostfix1, true) == 0 || string.Compare(parameters[i1], selectionInPostfix2, true) == 0)
                {
                    if (string.IsNullOrEmpty(selectedText))
                    {
                    }
                    else
                    {
                        parameters[i1] = selectedText;
                    }
                }
            }
            if (parameters.Count == syntaxElements.FindAll(IsParameter).Count - 1)
            {
                if (string.IsNullOrEmpty(selectedText))
                {
                    parameters.Add(selectionInPostfix1);
                }
                else
                {
                    parameters.Add(selectedText);
                }
            }

            parameterCountInSyntax = syntaxElements.FindAll(IsParameter).Count;
            return parameters;
        }

        private static bool IsParameter(String s)
        {
            return s == "*";
        }
    }
}
