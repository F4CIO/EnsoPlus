using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using CraftSynth.BuildingBlocks.Common;

namespace ControlCornerLauncher
{
	internal enum CornerLauncherCharacterKind
	{
		DescriptionNormal,
		DescriptionField,
		Typed,
		TypedSelected,
		SuggestedNormal,
		SuggestedField
	}
	internal class CornerLauncherCharacter
	{
		public CornerLauncherLine parentLine;
		public CornerLauncherCharacterKind kind;
		public char character;
		private int _index;
		private Point _location;
		private Font _font;
		private Color _color;
		private Color _backgroundColor;

		public CornerLauncherCharacter(CornerLauncherLine parentLine, CornerLauncherCharacterKind kind, char character, int index)
		{
			this.parentLine = parentLine;
			this.kind = kind;
			this.character = character;
			this._index = index;
		
			this._font = parentLine.font;
			switch (kind)
			{
				case CornerLauncherCharacterKind.DescriptionNormal:
					this._color = Color.DarkGray;
					this._backgroundColor = Color.DarkOliveGreen;
					break;
				case CornerLauncherCharacterKind.DescriptionField:
					this._color = Color.White;//Color.LightGray;
					this._backgroundColor = Color.DarkOliveGreen;
					break;
				case CornerLauncherCharacterKind.Typed:
					this._color = Color.YellowGreen;
					this._backgroundColor = Color.Black;
					break;	
				case CornerLauncherCharacterKind.TypedSelected:
					this._color = Color.White;
					this._backgroundColor = Color.Black;
					break;
				case CornerLauncherCharacterKind.SuggestedNormal:
					this._color = Color.DarkOliveGreen;
					this._backgroundColor = Color.Black;
					break;
				case CornerLauncherCharacterKind.SuggestedField:
					this._color = Color.FromKnownColor( KnownColor.ControlDarkDark);
					this._backgroundColor = Color.Black;
					break;
			}
		}

		public SizeF GetSize(Graphics g)
		{
			return this.character==' '?new SizeF(25 ,g.MeasureString(this.character.ToString(), this._font).Height): g.MeasureString(this.character.ToString(), this._font);
		}

		public void Draw(Graphics g)
		{
			float x = this.parentLine.location.X;
			int i = 0;
			foreach (CornerLauncherCharacter precedingCharacter in this.parentLine.characters)
			{
				if (i == this._index)
				{
					break;
				}
				x = x+precedingCharacter.GetSize(g).Width+CornerLauncherLine.charactersDistance;
				i++;
			}
			this._location = new Point((int)Math.Round(x), this.parentLine.location.Y);

			//g.FillRectangle(new SolidBrush(Color.DarkOliveGreen), new Rectangle((int)Math.Round(x), this._location.Y, (int)Math.Round(g.MeasureString(this.character.ToString(), this._font).Width), (int)Math.Round(g.MeasureString(this.character.ToString(), this._font).Height)));
			g.FillRectangle(new SolidBrush(this._backgroundColor), new Rectangle((int)Math.Round(x), this._location.Y-(int)Math.Round(GuiElements.linesDistance), (int)Math.Round(this.GetSize(g).Width+30), (int)Math.Round(this.GetSize(g).Height+GuiElements.linesDistance+1)));
			
			g.DrawString(this.character.ToString(), this._font, new SolidBrush(this._color),this._location);
		}
	}

	public enum CornerLauncherLineKind
	{
		Description,
		//FirstSuggestion,
		Suggestion
	}
	internal class CornerLauncherLine
	{
		private const char openFieldChar = '[';
		private const char closeFieldChar = ']';
		public const float charactersDistance = -9;
		public GuiElements parentGuiElements;
		public CornerLauncherLineKind kind;
		public int lineIndex;
		public Point location;
		
		public List<CornerLauncherCharacter> characters;
		public string typedText;
		public bool selected;
		public Font font;

		public CornerLauncherLine(GuiElements parentGuiElements, CornerLauncherLineKind kind, int lineIndex, string text, string typedText, bool selected)
		{
			this.parentGuiElements = parentGuiElements;
			this.kind = kind;
			this.lineIndex = lineIndex;
			this.typedText = typedText;
			this.selected = selected;
			this.font = new Font(FontFamily.GenericSansSerif, 30);

			List<int> indexesOfSelectedParts = new List<int>();
			if (!string.IsNullOrEmpty(text) && !string.IsNullOrEmpty(typedText))
			{
				indexesOfSelectedParts.AddRange(text.IndexesOfOccurrences(typedText, false));
			}

			int index = 0;
			bool inField = false;
			bool inTypedPart = false;
			CornerLauncherCharacterKind ck; 
			this.characters = new List<CornerLauncherCharacter>();
			foreach (char c in text)
			{
				if (!inField && c == openFieldChar)
				{
					inField = true;
				}

				inTypedPart = indexesOfSelectedParts.Any(partStart => partStart <= index && index < partStart + typedText.Length);

				switch (kind)
				{
					case CornerLauncherLineKind.Description:
						if (!inField)
						{
							ck = CornerLauncherCharacterKind.DescriptionNormal;
						}
						else
						{
							ck = CornerLauncherCharacterKind.DescriptionField;
						}
						break;
					case CornerLauncherLineKind.Suggestion:
						if (!inField)
						{
							if (inTypedPart)
							{
								if (!this.selected)
								{
									ck = CornerLauncherCharacterKind.Typed;
								}
								else
								{
									ck = CornerLauncherCharacterKind.TypedSelected;
								}
							}
							else
							{
								ck = CornerLauncherCharacterKind.SuggestedNormal;
							}
						}
						else
						{
							ck = CornerLauncherCharacterKind.SuggestedField;
						}
						break;
					default: throw new NotImplementedException();
				}
				this.characters.Add(new CornerLauncherCharacter(this, ck, c, index));

				if (inField && c == closeFieldChar)
				{
					inField = false;
				}
				index++;
			}
			
		}

		public SizeF GetSize(Graphics g)
		{
			float maxHeight = 0;
			float width = 0;

			foreach (CornerLauncherCharacter c in characters)
			{
				if (c.GetSize(g).Height > maxHeight)
				{
					maxHeight = c.GetSize(g).Height;
				}
				width = width + c.GetSize(g).Width + charactersDistance;
			}

			return new SizeF(width, maxHeight);
		}

		public void Draw(Graphics g)
		{
			float y = 0;
			int i = 0;
			foreach (CornerLauncherLine line in this.parentGuiElements.lines)
			{
				if (i == this.lineIndex)
				{
					break;
				}

				y = y + line.GetSize(g).Height + GuiElements.linesDistance;

				i++;
			}

			this.location = new Point(0, (int)Math.Round(y));

			//g.FillRectangle(new SolidBrush(Color.DarkOliveGreen), new Rectangle(0, (int)Math.Round(y), (int)Math.Round(this.GetSize(g).Width), (int)Math.Round(this.GetSize(g).Height)));
			foreach (CornerLauncherCharacter c in characters)
			{
				c.Draw(g);
			}
		}

		public override string ToString()
		{
			string r = string.Empty;
			this.characters.ForEach(c=>r = r + c.character);
			return r;
		}
	}
	
	internal class GuiElements
	{
		internal const float linesDistance = 1;
		public List<CornerLauncherLine> lines;
		
		public void Draw(Graphics g)
		{
			g.FillRectangle(new SolidBrush(Color.Pink), g.ClipBounds);
			foreach (CornerLauncherLine line in lines)
			{
				line.Draw(g);
			}
		}
	}	
}
