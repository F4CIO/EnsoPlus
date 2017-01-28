using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Drawing;

namespace Extension
{
	public class ParameterInput
	{
		private static List<string> _suggestions;
		private static bool _offerAllSuggestions;
		public static bool _readOnly;
		public static bool _acceptOnlySuggested;
		public static bool _caseSensitive;
		private static List<string> processedSuggestions;
		private static List<string> suggestionsToDisplay;

		public static List<string> suggestionsDisplayed;
		private const int numberOfSuggestionsDisplayed = 10;
		public static int suggestionsOffset = -1;
		public static int suggestionsSelectionIndex = -1;


		public static Color itemBackground = Color.FromArgb(32, 32, 32);
		public static Color itemBackgroundSelected = Color.FromArgb(64, 64, 64);
		public static Color itemForeground = Color.OliveDrab;
		public static Color itemForegroundHighlighted = Color.FromArgb(185, 220, 0);

		private static FormBackPanel formBackPanel;
		private static FormParameterBox formParameterBox;
		private static FormSuggestionsList formSuggestionsList;

		private static string result;

		public delegate void OnCloseDelegate(string result, object contextData);
		public static event OnCloseDelegate OnClose;
		private static object _contextData;

		public static void Init()
		{
			formSuggestionsList = new FormSuggestionsList();
			formBackPanel = new FormBackPanel();
			formParameterBox = new FormParameterBox();
			//formParameterBox.Show();
			//formParameterBox.Hide();
		}

		public static void InitInNewThread(object p)
		{
			formSuggestionsList = new FormSuggestionsList();
			formBackPanel = new FormBackPanel();
			formParameterBox = new FormParameterBox();
			//Application.Run(formParameterBox);
			formParameterBox.ShowDialog();
		}

		public static string Display(ParameterInputArguments args, OnCloseDelegate onClose, object contextData, Screen screen = null)
		{
			return Display(args.caption, args.suggestions, args.offerAllSuggestions, args.readOnly, args.predefinedValue, args.acceptOnlySuggested, args.caseSensitive, onClose, contextData, screen);
		}

		/// <summary>
		/// Only this is allowed for use.
		/// </summary>
		/// <param name="suggestions"></param>
		/// <param name="offerAllSuggestions"></param>
		/// <param name="readOnly"></param>
		/// <param name="predefinedValue"></param>
		/// <param name="acceptOnlySuggested"></param>
		/// <returns></returns>
		public static string Display(string caption, List<string> suggestions, bool offerAllSuggestions, bool readOnly, string predefinedValue, bool acceptOnlySuggested, bool caseSensitive, OnCloseDelegate onClose, object contextData, Screen screen = null)
		{
			_suggestions = (suggestions == null) ? new List<string>() : suggestions;
			_readOnly = readOnly;
			_offerAllSuggestions = _readOnly || offerAllSuggestions;
			_caseSensitive = caseSensitive;
			if (_readOnly && _suggestions.Count == 0)
			{
				return null;
			}
			_acceptOnlySuggested = acceptOnlySuggested;
			OnClose = onClose;
			_contextData = contextData;

			CenterFormOnScreen(formBackPanel, screen);
			formBackPanel.TopMost = true;
			formBackPanel.Show();
			formBackPanel.TopMost = false;
			
			formParameterBox.lblCaption.Text = caption;
			formParameterBox.tbParameter.Text = string.Empty;
			result = null;
			suggestionsSelectionIndex = -1;
			suggestionsOffset = 0;
			oldEnteredText = null;
			oldOffset = 0;
		
			//TODO: check this verification
			//if (_readOnly)
			//{
			if (!string.IsNullOrEmpty(predefinedValue))
			{
				formParameterBox.tbParameter.Text = predefinedValue;
				suggestionsSelectionIndex = _suggestions.IndexOf(predefinedValue);
				if (suggestionsSelectionIndex != -1)
				{
					suggestionsOffset = suggestionsSelectionIndex - (int)((numberOfSuggestionsDisplayed / 2));
					while (suggestionsOffset > _suggestions.Count - numberOfSuggestionsDisplayed) suggestionsOffset--;
					if (suggestionsOffset < 0) suggestionsOffset = 0;
				}
			}
			//}    

			CenterFormOnScreen(formParameterBox,screen);
			formParameterBox.TopMost = true;
			formParameterBox.Show();
			return result;
		}

		private static void CenterFormOnScreen(Form f, Screen screen)
		{
			var s = screen ?? Screen.PrimaryScreen;
			int x = s.Bounds.X+((s.Bounds.Width - f.Width)/2);
			int y = s.Bounds.Y+(s.Bounds.Height - f.Height)/2;
			f.DesktopLocation = new Point(x, y);
		}

		private static List<string> ProcessSuggestions(List<string> suggestions, string keyword)
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

		private static List<KeyValuePair<string, float>> RankSuggestions(List<string> suggestions, string keyword)
		{
			List<KeyValuePair<string, float>> rankedSuggestions = new List<KeyValuePair<string, float>>();

			int occurrences = 0;
			int nextSearchPosition = 0;
			float weight;
			StringComparison stringComparasionType = (ParameterInput._caseSensitive) ? StringComparison.InvariantCulture : StringComparison.InvariantCultureIgnoreCase;
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
				weight = ((float)keyword.Length * (float)occurrences) / (float)suggestion.Length;
				rankedSuggestions.Add(new KeyValuePair<string, float>(suggestion, weight));
			}

			return rankedSuggestions;
		}

		private static List<KeyValuePair<string, float>> FilterSuggestions(List<KeyValuePair<string, float>> rankedSuggestions)
		{
			rankedSuggestions.RemoveAll(WithZeroRank);
			return rankedSuggestions;
		}

		private static bool WithZeroRank(KeyValuePair<string, float> pair)
		{
			return pair.Value == 0;
		}

		private static List<string> SortSuggestionsByRank(List<KeyValuePair<string, float>> rankedSuggestions)
		{
			List<string> sortedSuggestions = new List<string>();

			rankedSuggestions.Sort(CompareByRank);

			foreach (KeyValuePair<string, float> pair in rankedSuggestions)
			{
				sortedSuggestions.Add(pair.Key);
			}

			return sortedSuggestions;
		}

		private static int CompareByRank(KeyValuePair<string, float> pair1, KeyValuePair<string, float> pair2)
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


		private static string oldEnteredText;
		private static int oldOffset;

		public static List<string> GetSuggestionsToDisplay(string enteredText)
		{
			if (oldEnteredText != enteredText || oldOffset != suggestionsOffset)
			{
				suggestionsToDisplay = new List<string>();

				if (!string.IsNullOrEmpty(enteredText) || _readOnly)
				{
					if (_readOnly)
					{
						processedSuggestions = new List<string>();
						processedSuggestions.AddRange(_suggestions);
					}
					else
					{

						if (oldEnteredText != enteredText)
						{
							processedSuggestions = ProcessSuggestions(_suggestions, formParameterBox.tbParameter.Text);
						}
					}

					if (suggestionsOffset > processedSuggestions.Count - 1)
					{
						suggestionsOffset = -1;
					}

					if (suggestionsOffset == -1 && processedSuggestions.Count > 0)
					{
						suggestionsOffset = 0;
					}

					int i = suggestionsOffset;
					while (i <= processedSuggestions.Count - 1 && i - suggestionsOffset < numberOfSuggestionsDisplayed && i >= 0)
					{
						suggestionsToDisplay.Add(processedSuggestions[i]);
						i++;
					}
				}

				if (suggestionsToDisplay.Count > 0 && suggestionsSelectionIndex == -1)
				{
					suggestionsSelectionIndex = 0;
					formParameterBox.parameterBoxRectangle.Invalidate();
				}

				oldOffset = suggestionsOffset;
				oldEnteredText = enteredText;
			}


			return suggestionsToDisplay;
		}



		internal static void MoveUpSuggesstionSelection()
		{
			if (suggestionsSelectionIndex > -1)
			{
				suggestionsSelectionIndex--;
				if (suggestionsSelectionIndex == -1)
				{
					formParameterBox.parameterBoxRectangle.Invalidate();
				}

				if (suggestionsOffset > 0 && suggestionsOffset > suggestionsSelectionIndex)
				{
					suggestionsOffset = suggestionsSelectionIndex;
					suggestionsDisplayed = GetSuggestionsToDisplay(formParameterBox.tbParameter.Text);
				}

				if (_readOnly && suggestionsSelectionIndex > -1)
				{
					formParameterBox.tbParameter.Text = processedSuggestions[suggestionsSelectionIndex];
				}

				formSuggestionsList.DrawAll(suggestionsDisplayed, suggestionsOffset, formParameterBox.tbParameter.Text, suggestionsSelectionIndex);
			}
		}

		internal static void MoveDownSuggesstionSelection()
		{
			if (processedSuggestions != null && suggestionsSelectionIndex < processedSuggestions.Count - 1)
			{

				suggestionsSelectionIndex++;
				if (suggestionsSelectionIndex == 0)
				{
					formParameterBox.parameterBoxRectangle.Invalidate();
				}

				if (suggestionsOffset < processedSuggestions.Count - numberOfSuggestionsDisplayed && suggestionsOffset <= suggestionsSelectionIndex - numberOfSuggestionsDisplayed)
				{
					suggestionsOffset = suggestionsSelectionIndex - numberOfSuggestionsDisplayed + 1;
					suggestionsDisplayed = GetSuggestionsToDisplay(formParameterBox.tbParameter.Text);
				}

				if (_readOnly)
				{
					formParameterBox.tbParameter.Text = processedSuggestions[suggestionsSelectionIndex];
				}

				formSuggestionsList.DrawAll(suggestionsDisplayed, suggestionsOffset, formParameterBox.tbParameter.Text, suggestionsSelectionIndex);
			}
		}

		internal delegate void CancelDelegate();
		public static void Cancel()
		{
			if (formParameterBox.InvokeRequired)
			{
				formParameterBox.Invoke(new CancelDelegate(Cancel));
			}
			else
			{
				if (formParameterBox.Visible)
				{
					result = null;
					formSuggestionsList.Visible = false;
					formBackPanel.Visible = false;
					formParameterBox.Visible = false;
					if (OnClose != null)
					{
						OnClose.Invoke(result, _contextData);
					}
				}
			}

		}

		internal static void Accept()
		{
			result = (suggestionsSelectionIndex == -1) ? formParameterBox.tbParameter.Text : processedSuggestions[suggestionsSelectionIndex];
			if (result == string.Empty) result = null;

			if (_acceptOnlySuggested)
			{
				if (suggestionsSelectionIndex > -1) Exit();
			}
			else
			{
				Exit();
			}
			
		}

		private static void Exit()
		{
			formSuggestionsList.Visible = false;
			formBackPanel.Visible = false;
			formParameterBox.Visible = false;
			//formParameterBox.Close();
			if (OnClose != null)
			{
				OnClose.Invoke(result, _contextData);
			}
		}

		private static object lock_object = new object();
		internal static void EnteredTextChanged()
		{
			lock (lock_object)
			{
				ParameterInput.suggestionsDisplayed = ParameterInput.GetSuggestionsToDisplay(formParameterBox.tbParameter.Text);

				if (ParameterInput.suggestionsDisplayed.Count > 0)
				{
					formSuggestionsList.Top = formParameterBox.parameterBoxRectangle.Bounds.Bottom + formParameterBox.Top;
					formSuggestionsList.Left = formParameterBox.parameterBoxRectangle.Bounds.Left + formParameterBox.Left;
					formSuggestionsList.TopMost = true;
					//if (ParameterInput.suggestionsSelectionIndex > suggestionsOffset + suggestionsDisplayed.Count - 1)
					//{
					ParameterInput.suggestionsSelectionIndex = suggestionsOffset;
					//suggestionsOffset + suggestionsDisplayed.Count - 1;
					//}
					formSuggestionsList.Show();
					formSuggestionsList.DrawAll(ParameterInput.suggestionsDisplayed, suggestionsOffset, formParameterBox.tbParameter.Text, ParameterInput.suggestionsSelectionIndex);
					formParameterBox.tbParameter.Focus();
				}
				else
				{
					ParameterInput.suggestionsSelectionIndex = -1;
					if (formSuggestionsList != null)
					{
						formSuggestionsList.Hide();
					}
				}
			}
		}
	}
}
