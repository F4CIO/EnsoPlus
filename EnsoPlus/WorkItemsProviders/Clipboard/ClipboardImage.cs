using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using Common;
using EnsoPlus.Entities;
using System.Windows.Forms;
using System.Drawing;

namespace EnsoPlus.WorkItemsProviders.Clipboard
{
    class ClipboardImage : IWorkItemsProvider
    {
        #region IParameterTypeProvider Members

        public bool SuggestionsCachingAllowed()
        {
            return false;
        }

        public Dictionary<string, IWorkItem> GetSuggestions()
        {
            Dictionary<string, IWorkItem> suggestions = new Dictionary<string, IWorkItem>();

			if (CraftSynth.BuildingBlocks.IO.Clipboard.GetClipboardFormatName() == DataFormats.Bitmap)
            {
                ImageWorkItem imageWorkItem = new ImageWorkItem();
                imageWorkItem.filePath = "[Image from clipboard]";
                imageWorkItem.provider = this;
                suggestions.Add("[Image from clipboard]", imageWorkItem);
            }

            return suggestions;
        }

        public IWorkItem GetParameterFromSelectedSuggestion(IWorkItem selectedSuggestion)
        {
            ImageWorkItem selectedImageWorkItem = (selectedSuggestion as ImageWorkItem);
			selectedImageWorkItem.image = CraftSynth.BuildingBlocks.IO.Clipboard.GetImageFromClipboard();
            FormImagePreview.Execute(selectedImageWorkItem.image);
            return selectedSuggestion;
        }

        public void Remove(IWorkItem workItemToRemove)
        {
            throw new ApplicationException("Operation not supported.");
        }

        #endregion
    }
}
