using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace EnsoPlus.Entities
{
    class Shortcut: IWorkItem
    {
        public const string extension = "url";
        public string targetPath;
        public string caption;
        public string shortcutFilePath;

        public void Save(string folderPath)
        {
            string fileName = this.caption + "." + extension;
            string filePath = Path.Combine(folderPath, fileName);
            CraftSynth.BuildingBlocks.IO.FileSystem.SaveAsInternetShortcut(filePath, this.targetPath);
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
            return this.targetPath;
        }

        public object GetValue()
        {
            return this.targetPath;
        }

        #endregion
    }
}
