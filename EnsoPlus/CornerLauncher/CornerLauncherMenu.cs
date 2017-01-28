using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CraftSynth.BuildingBlocks.Common;
using Extension;
using EnsoPlus.Entities;

namespace ControlCornerLauncher
{
	public class CornerLauncherMenu
	{
		#region Private Members

		private const string initialDescription = "While holding CAPS LOCK key type [command] name...";
		private GuiElements _guiElements;
		private FormCornerLauncher _formCornerLauncher;
		//private Dictionary<string, MergedCommand> _mergedCommands;
		private Dictionary<string, Command> _allSyntaxesAndCommands;
		private SuggestionsProcessor _suggestionsProcessor;
		private string _typedText;
		#endregion

		#region Properties
		#endregion

		#region Public Methods
		public void Show()
		{
			ParameterInput.Cancel();
			this._guiElements.lines.Clear();
			this._guiElements.lines.Add(new CornerLauncherLine(this._guiElements, CornerLauncherLineKind.Description, 0, initialDescription, this._typedText, false));
			Graphics g = this._formCornerLauncher.CreateGraphics();
			this._formCornerLauncher.CustomShow();
		}

		public void AddTypedCharacter(char c)
		{
			if (this._typedText.EndsWith(" ") && c == ' ')
			{
				//skip double spaces
			}else
			{
				this._typedText += c;
				this._suggestionsProcessor.GetSuggestionsToDisplay(this._typedText, this._allSyntaxesAndCommands.Keys.ToList(),this._suggestionsProcessor.GetSelectedItemText());

				RefreshSuggestionsList();
			}
		}

		public void RemoveLastTypedCharacter()
		{
			this._typedText = this._typedText.RemoveLastXChars(1);
			this._suggestionsProcessor.GetSuggestionsToDisplay(this._typedText, this._allSyntaxesAndCommands.Keys.ToList(), this._suggestionsProcessor.GetSelectedItemText());
			
			RefreshSuggestionsList();
		}

		public void SelectPrevious()
		{
			this._suggestionsProcessor.MoveUpSuggesstionSelection();
			RefreshSuggestionsList();
		}
		public void SelectNext()
		{
			this._suggestionsProcessor.MoveDownSuggesstionSelection();
			RefreshSuggestionsList();
		}

		public void SelectWhole()
		{
			if (this._suggestionsProcessor.GetSelectedIndexAmongAllSuggestions() >= 0 && this._suggestionsProcessor.GetSelectedItemText().IsNOTNullOrWhiteSpace())
			{
				var selectedCommand =this._allSyntaxesAndCommands[this._suggestionsProcessor.GetSelectedItemText()];
				this._typedText = selectedCommand.Name+" ";
				this._suggestionsProcessor.GetSuggestionsToDisplay(this._typedText, this._allSyntaxesAndCommands.Keys.ToList(), null);

				RefreshSuggestionsList();
			}
		}

		public KeyValuePair<Command, string>? GetSelectedCommandAndPostfix()
		{
			KeyValuePair<Command, string>? selectedCommand = null;
			if (!string.IsNullOrEmpty(this._suggestionsProcessor.GetSelectedItemText()))
			{
				var c = this._allSyntaxesAndCommands[this._suggestionsProcessor.GetSelectedItemText()];

				string postfix = string.Empty;
				if (this._typedText.StartsWith(c.Name, StringComparison.OrdinalIgnoreCase))
				{
					postfix = _typedText.Substring(c.Name.Length).Trim().ToLower();
				}
				selectedCommand = new KeyValuePair<Command, string>(c,postfix);
			}
			return selectedCommand;
		} 

		public void Hide()
		{
			this._formCornerLauncher.CustomHide();
			this._guiElements.lines.Clear();
			this._formCornerLauncher.Invalidate();
			
			this._suggestionsProcessor.ClearPosition();
			this._typedText = string.Empty;
			RefreshSuggestionsList();
		}
		#endregion

		#region Constructors And Initialization

		public CornerLauncherMenu(Dictionary<string, MergedCommand> mergedCommands)
		{
			//this._mergedCommands = mergedCommands;
			this._allSyntaxesAndCommands = new Dictionary<string, Command>();
			foreach (KeyValuePair<string, MergedCommand> mergedCommand in mergedCommands)
			{
				foreach (var command in mergedCommand.Value.sourceCommands)
				{
					_allSyntaxesAndCommands.Add(command.Name+" "+command.Postfix, command);
				}
			}
			this._suggestionsProcessor = new SuggestionsProcessor(_allSyntaxesAndCommands.Keys.ToList(), 10, false, false, null, false, false);
			this._guiElements = new GuiElements();
			this._guiElements.lines = new List<CornerLauncherLine>();
			this._formCornerLauncher = new FormCornerLauncher(this._guiElements);
			this._typedText = string.Empty;
		}
		#endregion

		#region Deinitialization And Destructors
		#endregion

		#region Event Handlers
		#endregion

		#region Private Methods	
		private void RefreshSuggestionsList()
		{
			this._guiElements.lines.Clear();
			if ((this._suggestionsProcessor.GetDisplayedSuggestions()==null || this._suggestionsProcessor.GetDisplayedSuggestions().Count == 0) && this._typedText.IsNOTNullOrWhiteSpace())
			{
				this._guiElements.lines.Add(new CornerLauncherLine(this._guiElements, CornerLauncherLineKind.Description, 0, initialDescription, this._typedText, false));
				this._guiElements.lines.Add(new CornerLauncherLine(this._guiElements, CornerLauncherLineKind.Suggestion, 1, this._typedText.ToLower(), this._typedText.ToLower(), true));
			}
			else { 
				this._guiElements.lines.Add(new CornerLauncherLine(this._guiElements, CornerLauncherLineKind.Description, 0, initialDescription, this._typedText, false));
				int index = 0;
				int selectedIndex = this._suggestionsProcessor.GetSelectedIndexAmongDisplayedSuggestions();
				string text = null;
				string suggestionName = null;
				var displayedSuggestions = this._suggestionsProcessor.GetDisplayedSuggestions();
				if (displayedSuggestions != null)
				{
					foreach (string s in displayedSuggestions)
					{
						text = s;
						if (index == selectedIndex)
						{
							if (s.IndexOf(Syntax.startOfSyntaxParameter) >= 0)
							{
								suggestionName = s.Substring(0, s.IndexOf(Syntax.startOfSyntaxParameter)).TrimEnd() + " ";
							}
							else
							{
								suggestionName = s.TrimEnd() + " ";
							}
							text = _typedText.Length > suggestionName.Length ? this._typedText.ToLower() : s;
						}

						this._guiElements.lines.Add(new CornerLauncherLine(this._guiElements, CornerLauncherLineKind.Suggestion, index + 1,
							text, this._typedText, index == selectedIndex));
						index++;
					}
				}
			}
			this._formCornerLauncher.Invalidate();
		}
		#endregion

		#region Helpers
		#endregion
	}
}
