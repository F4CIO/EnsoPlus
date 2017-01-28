using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using CraftSynth.BuildingBlocks.Common;
using Extension;

namespace ControlCornerLauncher
{
	public class SuggestionsProcessor
	{
		#region Private Members
		private List<string> _allSuggestions;
		private bool _offerAllSuggestions;
		private List<string> _processedSuggestions;
		private bool _readOnly;
		private bool _acceptOnlySuggested;
		private bool _caseSensitive;
		private List<string> _suggestionsToDisplay;
		private string _enteredText;
		private string oldEnteredText;
		private int oldOffset;

		private int _maxNumberOfSuggestionsDisplayed = 10;
		private int _suggestionsOffset = -1;
		private int _suggestionsSelectionIndex = -1;
		private bool userNavigated = false;
		private int _backspaceEventsCount = 0;
		#endregion

		#region Properties
		#endregion

		#region Public Methods

		public void ClearPosition()
		{
			if (this._processedSuggestions != null)
			{
				this._processedSuggestions.Clear();
			}

			if (this._suggestionsToDisplay != null)
			{
				this._suggestionsToDisplay.Clear();
			}

			this._enteredText = string.Empty;
		}

		public List<string> GetSuggestionsToDisplay(string enteredText, List<string> suggestions, string oldSelectedItemText)
		{
			if (oldEnteredText != enteredText || oldOffset != _suggestionsOffset)
			{
				_enteredText = enteredText;
				_suggestionsToDisplay = new List<string>();

				//when narroving suggestions by typing characters we stay on first line if user was there
				//when narroving suggestions by typing characters once user navidated down we try to keep item selected
				//if backspace is pressed several times reset position
				if (oldEnteredText.ToNonNullString().StartsWith(enteredText) && oldEnteredText.ToNonNullString().Length == enteredText.Length + 1)
				{
					_backspaceEventsCount++;
				}
				else
				{
					_backspaceEventsCount = 0;
				}
				if (enteredText.Length == 0 || _backspaceEventsCount>3)//oldEnteredText.ToNonNullString().Length>enteredText.Length)
				{
					userNavigated = false;
					_suggestionsSelectionIndex = 0;
				}

				if (!string.IsNullOrEmpty(enteredText) || _readOnly)
				{
					if (_readOnly)
					{
						_processedSuggestions = new List<string>();
						_processedSuggestions.AddRange(_allSuggestions);
					}
					else
					{

						if (oldEnteredText != enteredText)
						{
							_processedSuggestions = ProcessSuggestions(_allSuggestions, enteredText);
						}
					}

					if (_suggestionsOffset > _processedSuggestions.Count - 1)
					{
						_suggestionsOffset = -1;
					}

					if (_suggestionsOffset == -1 && _processedSuggestions.Count > 0)
					{
						_suggestionsOffset = 0;
					}

					int i = _suggestionsOffset;
					while (i <= _processedSuggestions.Count - 1 && i - _suggestionsOffset < _maxNumberOfSuggestionsDisplayed && i >= 0)
					{
						_suggestionsToDisplay.Add(_processedSuggestions[i]);
						i++;
					}
				}

				if (_suggestionsToDisplay.Count > 0 && _suggestionsSelectionIndex == -1)
				{
					_suggestionsSelectionIndex = 0;
				}
				if (_suggestionsToDisplay.Count > 0 && _suggestionsOffset>=0 && 
					_suggestionsSelectionIndex > _suggestionsOffset+_suggestionsToDisplay.Count - 1)
				{
						_suggestionsSelectionIndex = _suggestionsOffset + _suggestionsToDisplay.Count - 1;
				}

				if(oldSelectedItemText=="resetselection")
				{
					if (this._suggestionsToDisplay.Count > 0)
					{
						_suggestionsSelectionIndex = 0;
					}
					else
					{
						_suggestionsSelectionIndex = -1;
					}
				}
				else if (!string.IsNullOrEmpty(oldSelectedItemText) && userNavigated)
				{
					_suggestionsSelectionIndex = this._suggestionsToDisplay.IndexOf(oldSelectedItemText);
				}

				if (this._suggestionsSelectionIndex == -1 && this._suggestionsToDisplay.Count > 0)
				{
					_suggestionsSelectionIndex = 0;
				}

				//if more text is typed after whole suggestion
				if (this._suggestionsSelectionIndex == 0 && this._suggestionsToDisplay.Count == 0 && enteredText.Length>0)
				{
					int longestBeginingIndex = this.FindLongestBeginingIndex(enteredText, suggestions, this._caseSensitive?StringComparison.Ordinal:StringComparison.OrdinalIgnoreCase);
				
					if (longestBeginingIndex >=0)
					{
						this._suggestionsToDisplay.Add(suggestions[longestBeginingIndex]);
						this._suggestionsSelectionIndex = 0;
						//this._suggestionsOffset = suggestions.IndexOf(suggestions[longestBeginingIndex]);
					}
				}
				
				oldOffset = _suggestionsOffset;
				oldEnteredText = enteredText;
				
			}


			return _suggestionsToDisplay;
		}
		
		public void MoveUpSuggesstionSelection()
		{
			if (_suggestionsSelectionIndex > 0)
			{
				_suggestionsSelectionIndex--;
				userNavigated = true;
				
				if (_suggestionsOffset > 0 && _suggestionsOffset > _suggestionsSelectionIndex)
				{
					_suggestionsOffset = _suggestionsSelectionIndex;
					_suggestionsToDisplay = GetSuggestionsToDisplay(_enteredText, _allSuggestions, null);
				}

				if (_readOnly && _suggestionsSelectionIndex > -1)
				{
					_enteredText = _processedSuggestions[_suggestionsSelectionIndex];
				}
			}
		}

		public void MoveDownSuggesstionSelection()
		{
			if (_processedSuggestions != null && _suggestionsSelectionIndex < _processedSuggestions.Count - 1)
			{

				_suggestionsSelectionIndex++;
				userNavigated = true;
			
				if (_suggestionsOffset < _processedSuggestions.Count - _maxNumberOfSuggestionsDisplayed && _suggestionsOffset <= _suggestionsSelectionIndex - _maxNumberOfSuggestionsDisplayed)
				{
					_suggestionsOffset = _suggestionsSelectionIndex - _maxNumberOfSuggestionsDisplayed + 1;
					_suggestionsToDisplay = GetSuggestionsToDisplay(_enteredText, _allSuggestions, null);
				}

				if (_readOnly)
				{
					_enteredText = _processedSuggestions[_suggestionsSelectionIndex];
				}
			}
		}	
		
		public List<string> GetAllSuggestions()
		{
			return this._allSuggestions;
		}

		public List<string> GetDisplayedSuggestions()
		{
			return this._suggestionsToDisplay;
		}

		public int GetSelectedIndexAmongDisplayedSuggestions()
		{
			int r = -1;
			if (this._suggestionsSelectionIndex >= 0)
			{
				if (_suggestionsOffset < 0)
				{
					r = this._suggestionsSelectionIndex;
				}
				else
				{
					r = this._suggestionsSelectionIndex - _suggestionsOffset;
				}
			}
			return r;
		}

		public string GetSelectedItemText()
		{
			int i = this.GetSelectedIndexAmongDisplayedSuggestions();
			if (i >= 0 && this._suggestionsToDisplay.Count>i)
			{
				return this._suggestionsToDisplay[i];
			}
			else
			{
				return null;
			}
		}

		public int GetSelectedIndexAmongAllSuggestions()
		{
			return this._suggestionsSelectionIndex;
		}
		#endregion

		#region Constructors And Initialization
		public SuggestionsProcessor(List<string> suggestions, int maxNumberOfSuggestionsDisplayed, bool offerAllSuggestions,  bool readOnly, string predefinedValue, bool acceptOnlySuggested,bool caseSensitive)
		{
			_allSuggestions = suggestions ?? new List<string>();
			_readOnly = readOnly;
			_offerAllSuggestions = _readOnly || offerAllSuggestions;
			_maxNumberOfSuggestionsDisplayed = maxNumberOfSuggestionsDisplayed;
			_caseSensitive = caseSensitive;
			//if (_readOnly && _suggestions.Count == 0)
			//{
			//	return null;
			//}
			_acceptOnlySuggested = acceptOnlySuggested;

			//TODO: check this verification
			//if (_readOnly)
			//{
			if (!string.IsNullOrEmpty(predefinedValue))
			{
				_suggestionsSelectionIndex = _allSuggestions.IndexOf(predefinedValue);
				if (_suggestionsSelectionIndex != -1)
				{
					_suggestionsOffset = _suggestionsSelectionIndex - (int)((_maxNumberOfSuggestionsDisplayed / 2));
					while (_suggestionsOffset > _allSuggestions.Count - _maxNumberOfSuggestionsDisplayed) _suggestionsOffset--;
					if (_suggestionsOffset < 0) _suggestionsOffset = 0;
				}
			}
			//}   
		}
		#endregion

		#region Deinitialization And Destructors
		#endregion

		#region Event Handlers
		#endregion

		#region Private Methods
		private List<string> ProcessSuggestions(List<string> suggestions, string keyword)
		{
			List<string> processedSuggestions = null;

			List<KeyValuePair<string, float>> rankedSuggestions = RankSuggestions(suggestions, keyword);
			if (!_offerAllSuggestions)
			{
				rankedSuggestions = FilterSuggestions(rankedSuggestions);
			}
			processedSuggestions = SortSuggestionsByRank(rankedSuggestions);

			return processedSuggestions;
		}

		private List<KeyValuePair<string, float>> RankSuggestions(List<string> suggestions, string keyword)
		{
			List<KeyValuePair<string, float>> rankedSuggestions = new List<KeyValuePair<string, float>>();

			int occurrences = 0;
			int nextSearchPosition = 0;
			float weight;
			StringComparison stringComparasionType = (_caseSensitive) ? StringComparison.InvariantCulture : StringComparison.InvariantCultureIgnoreCase;
			foreach (string suggestion in suggestions)
			{
				occurrences = 0;
				nextSearchPosition = 0;
				while (nextSearchPosition < suggestion.Length - 1 && (nextSearchPosition = suggestion.IndexOf(keyword, nextSearchPosition, stringComparasionType)) != -1)
				{
					nextSearchPosition += keyword.Length;
					//if (keyword.Length > 1) nextSearchPosition--;
					occurrences++;
				}
				weight = ((float)keyword.Length * ((float)occurrences>0?1:0) ) / (float)suggestion.Length;
				bool suggestionBeginsWithKeyword = suggestion.IndexOf(keyword, stringComparasionType) ==0;
				if (suggestionBeginsWithKeyword)
				{
					weight = weight*10;
				}
				if (occurrences > 0)
				{
					weight = weight + ((float)0.0001 * (float)occurrences);
				}
				rankedSuggestions.Add(new KeyValuePair<string, float>(suggestion, weight));
			}
			//foreach (var rankedSuggestion in rankedSuggestions)
			//{
			//	Trace.WriteLine(rankedSuggestion.Key+"="+rankedSuggestion.Value);
			//}
			return rankedSuggestions;
			
		}

		private List<KeyValuePair<string, float>> FilterSuggestions(List<KeyValuePair<string, float>> rankedSuggestions)
		{
			rankedSuggestions.RemoveAll(WithZeroRank);
			return rankedSuggestions;
		}

		private bool WithZeroRank(KeyValuePair<string, float> pair)
		{
			return pair.Value == 0;
		}

		private List<string> SortSuggestionsByRank(List<KeyValuePair<string, float>> rankedSuggestions)
		{
			List<string> sortedSuggestions = new List<string>();

			rankedSuggestions.Sort(CompareByRank);

			foreach (KeyValuePair<string, float> pair in rankedSuggestions)
			{
				sortedSuggestions.Add(pair.Key);
			}

			return sortedSuggestions;
		}

		private int CompareByRank(KeyValuePair<string, float> pair1, KeyValuePair<string, float> pair2)
		{
			if (pair1.Value < pair2.Value)
			{
				return 1;
			}
			else if (pair1.Value > pair2.Value)
			{
				return -1;
			}
			else
			{
				return 0;
			}
		}


		#endregion

		#region Helpers

		/// <summary>
		/// Example: from beginings:
		///   ABC
		///   ABGGCD
		///   ABCDE
		/// and s=ABCDEFG
		/// returns: ABCDE
		/// </summary>
		/// <param name="s"></param>
		/// <param name="beginings"></param>
		/// <param name="stringComparison"></param>
		/// <returns></returns>
		private int FindLongestBeginingIndex(string s, List<string> beginings, StringComparison stringComparison)
		{
			int longestBeginingIndex = -1;
			string longestBegining = string.Empty;
			string beginingTrimmed;
			int i = 0;
			foreach (string begining in beginings)
			{
				if (begining.IndexOf(Syntax.startOfSyntaxParameter) >= 0)
				{
					beginingTrimmed = begining.Substring(0, begining.IndexOf(Syntax.startOfSyntaxParameter)).TrimEnd()+" ";
				}
				else
				{
					beginingTrimmed = begining.TrimEnd()+" ";
				}
				if (beginingTrimmed.Length > longestBegining.Length && s.StartsWith(beginingTrimmed, stringComparison))
				{
					longestBegining = beginingTrimmed;
					longestBeginingIndex = i;
				}
				i++;
			}

			if (longestBegining == string.Empty)
			{
				return -1;
			}
			else
			{
				return longestBeginingIndex;
			}
		}
		#endregion
	}
}
