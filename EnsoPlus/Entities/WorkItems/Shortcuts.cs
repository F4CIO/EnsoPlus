using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;

namespace EnsoPlus.Entities.WorkItems
{
    class Shortcuts:IWorkItem
    {
        public const string extension = "urls.htm";
        public string caption;
        public string shortcutsFilePath;
        public List<Shortcut> shortcuts;

        public Shortcuts()
        {
            this.shortcuts = new List<Shortcut>();
        }

        public Shortcuts(string filePath)
        {
            this.caption = Path.GetFileNameWithoutExtension(filePath);
            this.shortcutsFilePath = filePath;
            this.shortcuts = new List<Shortcut>();
            this.Load(filePath);            
        }

        public Shortcuts(string caption, string xml)
        {
            this.caption = caption;            
            LoadFromString(xml);
        }

        public override string ToString()
        {           
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(@"<?xml version=""1.0"" encoding=""utf-8""?>");
            sb.AppendLine(@"<Shortcuts>");
            foreach (Shortcut shortcut in this.shortcuts)
            {
                sb.AppendLine(string.Format(@"<a href=""{0}"">{1}</a><br/>",shortcut.targetPath,shortcut.caption));
            }
            sb.AppendLine(@"</Shortcuts>");
            return sb.ToString();
        }

        public void Save(string folder)
        {
            string fileName = this.caption+"."+extension;
            string filePath = Path.Combine(folder, fileName);
            File.WriteAllText(filePath, this.ToString(), UTF8Encoding.UTF8);
        }

        public void SaveAsSeparateShortcuts(string folderPath)
        {
            List<string> failed = new List<string>();
            foreach(Shortcut shortcut in this.shortcuts)
            {
                try
                {
                    shortcut.Save(folderPath);
                }
                catch(Exception exception)
                {
					Common.Logging.AddExceptionLog(exception);
                    failed.Add(shortcut.caption);
                }
            }

            if (failed.Count > 0)
            {
                StringBuilder errorMessage = new StringBuilder("Can not save:");
                foreach (string failedItem in failed)
                {
                    errorMessage.AppendLine(failedItem + ",");
                }
                errorMessage.AppendLine("See log for more informations.");
                throw new ApplicationException(errorMessage.ToString());
            }
        }

        private void LoadFromString(string contentXml)
        {
            XmlDocument doc = new XmlDocument();
            try
            {
                TryToLoadContentToXMLDoc(contentXml, doc);
            }
            catch (Exception)
            {
                contentXml = contentXml.Replace(@"&", @"&amp;");
                TryToLoadContentToXMLDoc(contentXml, doc);
            }
            
            if (this.shortcuts == null)
            {
                this.shortcuts = new List<Shortcut>();
            }
            else
            {
                this.shortcuts.Clear();
            }
            XmlNodeList shortcutNodes = doc.SelectNodes("/Shortcuts/a");
            foreach (XmlNode shortcutNode in shortcutNodes)
            {
                    Shortcut shortcut = new Shortcut();
                    shortcut.caption = shortcutNode.InnerText;
                    shortcut.shortcutFilePath = this.shortcutsFilePath;
                    shortcut.targetPath = shortcutNode.Attributes[0].Value;
                    this.shortcuts.Add(shortcut);
            }           
        }

        private static void TryToLoadContentToXMLDoc(string contentXml, XmlDocument doc)
        {
            try
            {
                //try loading with assuming that header is present
                //Preview:
                //  <?xml version=""1.0"" encoding=""utf-8""?>");
                //  <Shortcuts>
                //     <a href="http://www.yahoo.com/">yahoo</a><br/>
                //     <a href="http://www.Google.com/">Google</a><br/>
                //  </Shortcuts>
                doc.LoadXml(contentXml);
            }
            catch (Exception)
            {
                    //try loading with assuming that only <a> elements are present
                    //Preview:
                    //  <a href="http://www.yahoo.com/">yahoo</a><br/>
                    //  <a href="http://www.Google.com/">Google</a><br/>
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine(@"<?xml version=""1.0"" encoding=""utf-8""?>");
                    sb.AppendLine(@"<Shortcuts>");
                    sb.Append(contentXml);
                    sb.AppendLine(@"</Shortcuts>");
                    doc.LoadXml(sb.ToString());              
            }
        }

        private void Load(string filePath)
        {            
            string contentXml = File.ReadAllText(filePath, UTF8Encoding.UTF8);            
            LoadFromString(contentXml); 
        }
        #region IWorkItem Members

        public IWorkItemsProvider provider;
        public IWorkItemsProvider GetProvider()
        {
            return this.provider;
        }

        public string GetCaption()
        {
            return this.caption;
        }

        public string GetValueAsText()
        {
            return this.ToString();
        }

        public object GetValue()
        {
            return this.ToString();
        }

        #endregion
    }
}
