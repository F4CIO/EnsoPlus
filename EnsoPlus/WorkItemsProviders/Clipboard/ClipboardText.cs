using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using Common;
using EnsoPlus.Entities;
using System.Windows.Forms;

namespace EnsoPlus.WorkItemsProviders.Clipboard
{
    class ClipboardText : IWorkItemsProvider
    {
        #region IParameterTypeProvider Members

        public bool SuggestionsCachingAllowed()
        {
            return false;
        }

        public Dictionary<string, IWorkItem> GetSuggestions()
        {
            Dictionary<string, IWorkItem> suggestions = new Dictionary<string, IWorkItem>();
		
			if (CraftSynth.BuildingBlocks.IO.Clipboard.GetClipboardFormatName() == DataFormats.Text ||
				CraftSynth.BuildingBlocks.IO.Clipboard.GetClipboardFormatName() == DataFormats.UnicodeText ||
				CraftSynth.BuildingBlocks.IO.Clipboard.GetClipboardFormatName() == DataFormats.StringFormat ||
				CraftSynth.BuildingBlocks.IO.Clipboard.GetClipboardFormatName() == DataFormats.OemText)
			{
				StringWorkItem item = new StringWorkItem(CraftSynth.BuildingBlocks.IO.Clipboard.GetTextFromClipboard());//new StringWorkItem(CraftSynth.Win32Clipboard.ClipboardProxy.GrabClipboardDataStripped(false, null).AsUnicodeText);
				item.provider = this;
				suggestions.Add("[Clipboard Text]", item);
			}

            return suggestions;
        }

        public IWorkItem GetParameterFromSelectedSuggestion(IWorkItem selectedSuggestion)
        {
            
            //return new StringWorkItem(Helper.GetTextFromClipboard());
            return selectedSuggestion;
        }

        public void Remove(IWorkItem workItemToRemove)
        {
            throw new ApplicationException("Operation not supported.");
        }

        #endregion
    }
}
