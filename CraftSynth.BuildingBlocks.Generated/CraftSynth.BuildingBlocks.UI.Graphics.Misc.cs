using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CraftSynth.BuildingBlocks.UI.Graphics
{
	public static class Misc
	{
		public static Point LocationInScreenCoordinates(this Control c)
		{
			return c.PointToScreen(new Point(0, 0));
		}

		public static Rectangle BoundsInScreenCoordinates(this Control c)
		{
			return new Rectangle(LocationInScreenCoordinates(c), c.Bounds.Size);
		}

		public static bool PointIsInsideOfRectangle(Point p, Rectangle r, bool resultIfOnEdge)
		{
			bool isInside;

			isInside = p.X > r.Left && p.X < r.Right && p.Y > r.Top && p.Y < r.Bottom;
			if (p.X == r.Left || p.X == r.Right || p.Y == r.Top || p.Y == r.Bottom)
			{
				isInside = resultIfOnEdge;
			}

			return isInside;
		}

		/// <summary>
		/// Scales oldDimensions to fit rectToTouchFromInside
		/// </summary>
		/// <param name="oldDimmensions"></param>
		/// <param name="rectToTouchFromInside"></param>
		/// <param name="allowEnlarging"></param>
		/// <returns></returns>
		public static Size ResizeToFit(Size oldDimensions, Size rectToTouchFromInside, bool allowEnlarging, bool keepRelation)
		{
			Size newDimensions = new Size(0, 0);

			if (!keepRelation)
			{
				throw new NotImplementedException();
			}
			else
			{
				newDimensions = new Size(0, 0);

				if (!allowEnlarging && oldDimensions.Width <= rectToTouchFromInside.Width && oldDimensions.Height <= rectToTouchFromInside.Height)
				{
					newDimensions.Width = oldDimensions.Width;
					newDimensions.Height = oldDimensions.Height;
				}
				else
				{
					newDimensions.Width = Convert.ToInt32(oldDimensions.Width*rectToTouchFromInside.Height/oldDimensions.Height);
					newDimensions.Height = rectToTouchFromInside.Height;

					if (newDimensions.Width > rectToTouchFromInside.Width)
					{
						newDimensions.Width = rectToTouchFromInside.Width;
						newDimensions.Height = Convert.ToInt32(rectToTouchFromInside.Width*oldDimensions.Height/oldDimensions.Width);
					}
				}
			}

			return newDimensions;
		}


		//not finished nor tested!
		public static void SaveUploadedImage(Stream fileStream, string targetPath, int maxWidth, int maxHeight, bool deleteOriginalUploadedFile, out int resultingImageWidth, out int resultingImageHeight)
		{
			Image image = System.Drawing.Image.FromStream(fileStream);

			decimal lnRatio;
			int lnNewWidth = image.Width;
			int lnNewHeight = image.Height;
			bool resize = true;

			//*** If the image is smaller than a thumbnail just return it
			if (image.Width <= maxWidth && image.Height <= maxHeight)
				resize = false;

			if (resize)
			{
				if (image.Width > image.Height)
				{
					lnRatio = (decimal)maxWidth / image.Width;
					lnNewWidth = maxWidth;
					decimal lnTemp = image.Height * lnRatio;
					lnNewHeight = (int)lnTemp;
				}
				else
				{
					lnRatio = (decimal)maxHeight / image.Height;
					lnNewHeight = maxHeight;
					decimal lnTemp = image.Width * lnRatio;
					lnNewWidth = (int)lnTemp;
				}
			}

			Bitmap bmPhoto = new Bitmap(lnNewWidth + 2, lnNewHeight + 2, PixelFormat.Format24bppRgb);
			System.Drawing.Graphics grPhoto = System.Drawing.Graphics.FromImage(bmPhoto);
			grPhoto.CompositingQuality = CompositingQuality.HighQuality;
			//grPhoto.Clear(Color.Red);
			grPhoto.SmoothingMode = SmoothingMode.HighQuality;
			grPhoto.InterpolationMode = InterpolationMode.HighQualityBicubic;
			grPhoto.DrawImage(image, new Rectangle(0, 0, lnNewWidth + 2, lnNewHeight + 2));
			grPhoto.Dispose();
			bmPhoto.Save(targetPath, ImageFormat.Gif);
			resultingImageWidth = image.Width;
			resultingImageHeight = image.Height;
			image.Dispose();
			bmPhoto.Dispose();

			if (deleteOriginalUploadedFile)
			{
				if (fileStream != null)
				{
					try
					{
						fileStream.Dispose();
					}
					catch (Exception e)
					{
						throw new Exception("Failed to remove temporary image file. Stinks.");
					}
				}
			}
		}
		
		/// <summary>
		/// Clones specified region on clone of passed image.
		/// Source: http://notes.ericwillis.com/2009/10/blur-an-image-with-csharp/
		/// </summary>
		/// <param name="imageToBlur"></param>
		/// <param name="areaToBlur"></param>
		/// <param name="blurIntensity"></param>
		/// <returns></returns>
		public static Bitmap Blur(Bitmap imageToBlur, Int32 blurIntensity, Rectangle? areaToBlur = null)
		{
			imageToBlur = (Bitmap)imageToBlur.Clone();
			if (areaToBlur == null)
			{
				areaToBlur = new Rectangle(0, 0, imageToBlur.Width, imageToBlur.Height);
			}
			//Bitmap blurred = new Bitmap(image.Width, image.Height);

			//// make an exact copy of the bitmap provided
			//using (Graphics graphics = Graphics.FromImage(blurred))
			//	graphics.DrawImage(image, new Rectangle(0, 0, image.Width, image.Height),
			//		new Rectangle(0, 0, image.Width, image.Height), GraphicsUnit.Pixel);

			// look at every pixel in the blur rectangle
			for (Int32 xx = areaToBlur.Value.X; xx < areaToBlur.Value.X + areaToBlur.Value.Width; xx++)
			{
				for (Int32 yy = areaToBlur.Value.Y; yy < areaToBlur.Value.Y + areaToBlur.Value.Height; yy++)
				{
					Int32 avgR = 0, avgG = 0, avgB = 0;
					Int32 blurPixelCount = 0;

					// average the color of the red, green and blue for each pixel in the
					// blur size while making sure you don't go outside the image bounds
					for (Int32 x = xx; (x < xx + blurIntensity && x < imageToBlur.Width); x++)
					{
						for (Int32 y = yy; (y < yy + blurIntensity && y < imageToBlur.Height); y++)
						{
							Color pixel = /*blurred*/imageToBlur.GetPixel(x, y);

							avgR += pixel.R;
							avgG += pixel.G;
							avgB += pixel.B;

							blurPixelCount++;
						}
					}

					avgR = avgR / blurPixelCount;
					avgG = avgG / blurPixelCount;
					avgB = avgB / blurPixelCount;

					// now that we know the average for the blur size, set each pixel to that color
					for (Int32 x = xx; x < xx + blurIntensity && x < imageToBlur.Width && x < areaToBlur.Value.Width; x++)
						for (Int32 y = yy; y < yy + blurIntensity && y < imageToBlur.Height && y < areaToBlur.Value.Height; y++)
							/*blurred*/imageToBlur.SetPixel(x, y, Color.FromArgb(avgR, avgG, avgB));
				}
			}

			return imageToBlur/* blurred*/;
		}

		public static byte[] ToBytes(this Image image, ImageFormat imageFormat = null)
		{
			byte[] bytes;
			using (MemoryStream ms = new MemoryStream())
			{
				image.Save(ms, imageFormat??ImageFormat.Png);
				bytes = StreamToBytes(ms);
			}
			return bytes;
		}

		public static Image ToImage(this byte[] bytes)
		{
			Bitmap theImage;
			using (var ms = new MemoryStream(bytes))
			{
				theImage = (Bitmap)Bitmap.FromStream(ms);
			}
			return theImage;
		}

		/// <summary>
		/// Source: http://stackoverflow.com/questions/221925/creating-a-byte-array-from-a-stream
		/// </summary>
		/// <param name="input"></param>
		/// <param name="streamDoesntChange"></param>
		/// <returns></returns>
	    private static byte[] StreamToBytes(Stream input, bool streamDoesntChange = true)
		{
			using (input)
			{
				input.Seek(0, SeekOrigin.Begin);
				byte[] buffer = streamDoesntChange ? new byte[input.Length] : new byte[16*1024];
				using (MemoryStream ms = new MemoryStream())
				{
					int read;
					while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
					{
						ms.Write(buffer, 0, read);
					}
					return ms.ToArray();
				}
			}
		}

		public static bool IsTransparent(this Image image, bool checkAllPixels, int countOfRandomPixelsToCheck)
		{
			if (!(image is Bitmap))
			{
				throw new Exception("DetermineIfImageHasAlphaChannel: Image is not Bitmap.");
			}
			if (checkAllPixels)
			{
				for (int y = 0; y < image.Height; ++y)
				{
					for (int x = 0; x < image.Width; ++x)
					{
						if (((Bitmap)image).GetPixel(x, y).A != 255)
						{
							return true;
						}
					}
				}
				return false;
			}
			else
			{
				var random = new Random();
				int x;
				int y;
				for (int i = 0; i < Math.Min(countOfRandomPixelsToCheck, image.Size.Width*image.Size.Height); i++)
				{
					x = random.Next(image.Size.Width);
					y = random.Next(image.Size.Height);
					if (((Bitmap)image).GetPixel(x, y).A != 255)
					{
						return true;
					}
				}
				return false;
			}
		}

		/// <summary>
		/// Source: http://www.extensionmethod.net/csharp/draw/fill-draw-roundedrectangle
		/// </summary>
		/// <param name="g"></param>
		/// <param name="pen">Passing null will disable edge drawing.</param>
		/// <param name="brush">Passing null will disable filling enterior or rectngle.</param>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="width"></param>
		/// <param name="height"></param>
		/// <param name="radius"></param>
		/// <param name="roundTopLeft"></param>
		/// <param name="roundTopRight"></param>
		/// <param name="roundBottomLeft"></param>
		/// <param name="roundBottomRight"></param>
		public static void DrawOrFillRoundedRectangle(this System.Drawing.Graphics g, Pen pen, Brush brush, int x, int y, int width, int height, int radius, bool roundTopLeft = true, bool roundTopRight = true, bool roundBottomLeft = true, bool roundBottomRight = true, Color? backgroundForCorners = null)
		{
			int shortestSide = Math.Min(width, height);
			if (radius >= shortestSide)
			{
				radius = shortestSide - 1;
			}
			if (radius <= 0)
			{
				radius = 1;
			}

			float? originalPenWidth = null;
			if (pen != null)
			{
				if (pen.Width * 2 + 3 >= shortestSide)
				{
					originalPenWidth = pen.Width;
					pen.Width = (float)Math.Floor((float)shortestSide / 2) - 3;
				}

				//Below id fixed with g.Clip(...)
				//if (pen.Width * 2 >= radius)
				//{
				//	pen.Width = (float)Math.Floor((float)radius / 2) - 1;
				//}
			}


			float penWidth = pen == null ? 0 : pen.Width;
			int lineAndArcAndPenWidthJunctionFix = (int)Math.Round(penWidth * 1.9);

			var originalGClip = g.Clip;
			g.Clip = new Region(new Rectangle((int)Math.Round(x - penWidth / 2), (int)Math.Round(y - penWidth / 2), (int)Math.Round(width + penWidth), (int)Math.Round(height + penWidth)));

			Rectangle corner = new Rectangle(x, y, radius, radius);
			using (GraphicsPath path = new GraphicsPath())
			{
				Pen bp = null;
				Brush bb = null;
				try
				{
					if (backgroundForCorners != null)
					{
						bp = new Pen(backgroundForCorners.Value, penWidth);
						bb = new SolidBrush(backgroundForCorners.Value);
					}

					if (!roundTopLeft)
					{
						path.AddLine(new Point(corner.X, corner.Y), new Point(corner.X, corner.Y));
					}
					else
					{
						path.AddArc(corner, 180, 90);
						if (backgroundForCorners != null)
						{
							using (GraphicsPath path2 = new GraphicsPath())
							{
								path2.AddArc(corner, 180, 90 - lineAndArcAndPenWidthJunctionFix);
								path2.AddLine(corner.X + corner.Width / 2 - penWidth, corner.Y, corner.X, corner.Y);
								path2.AddLine(corner.X, corner.Y, corner.X, corner.Y + corner.Height / 2);
								g.FillPath(bb, path2);
								g.DrawLine(bp, corner.X + corner.Width / 2 + penWidth / 2, corner.Y, corner.X, corner.Y);
								g.DrawPath(bp, path2);
							}
						}
					}

					corner.X = x + width - radius;
					if (!roundTopRight)
					{
						path.AddLine(new Point(corner.X + corner.Width, corner.Y), new Point(corner.X + corner.Width, corner.Y));
					}
					else
					{
						path.AddArc(corner, 270, 90);
						if (backgroundForCorners != null)
						{
							using (GraphicsPath path2 = new GraphicsPath())
							{
								path2.AddArc(corner, 270, 90 - lineAndArcAndPenWidthJunctionFix);
								path2.AddLine(corner.Right, corner.Bottom - (corner.Height / 2) - penWidth, corner.Right, corner.Y);
								path2.AddLine(corner.Right, corner.Y, corner.Right - (corner.Width / 2), corner.Y);
								g.FillPath(bb, path2);
								g.DrawLine(bp, corner.Right, corner.Bottom - (corner.Height / 2) + penWidth / 2, corner.Right, corner.Y);
								g.DrawPath(bp, path2);
							}
						}
					}

					corner.Y = y + height - radius;
					if (!roundBottomRight)
					{
						path.AddLine(new Point(corner.X + corner.Width, corner.Y + corner.Height), new Point(corner.X + corner.Width, corner.Y + corner.Height));
					}
					else
					{
						path.AddArc(corner, 0, 90);
						if (backgroundForCorners != null)
						{
							using (GraphicsPath path2 = new GraphicsPath())
							{
								path2.AddArc(corner, 0, 90 - lineAndArcAndPenWidthJunctionFix);
								path2.AddLine(corner.Right - (corner.Width / 2) + penWidth, corner.Bottom, corner.Right, corner.Bottom);
								path2.AddLine(corner.Right, corner.Bottom, corner.Right, corner.Bottom - (corner.Height / 2));
								g.FillPath(bb, path2);
								g.DrawLine(bp, corner.Right - (corner.Width / 2) - penWidth / 2, corner.Bottom, corner.Right, corner.Bottom);
								g.DrawPath(bp, path2);
							}
						}

					}

					corner.X = x;
					if (!roundBottomLeft)
					{
						path.AddLine(new Point(corner.X, corner.Y + corner.Height), new Point(corner.X, corner.Y + corner.Height));
					}
					else
					{
						path.AddArc(corner, 90, 90);
						if (backgroundForCorners != null)
						{
							using (GraphicsPath path2 = new GraphicsPath())
							{
								path2.AddArc(corner, 90, 90 - lineAndArcAndPenWidthJunctionFix);
								path2.AddLine(corner.Left, corner.Bottom - corner.Height / 2 + penWidth, corner.Left, corner.Bottom);
								path2.AddLine(corner.Left, corner.Bottom, corner.Left + (corner.Width / 2), corner.Bottom);
								g.FillPath(bb, path2);
								g.DrawLine(bp, corner.Left, corner.Bottom - corner.Height / 2 - penWidth / 2, corner.Left, corner.Bottom);
								g.DrawPath(bp, path2);
							}

						}
					}
				}
				finally
				{
					if (backgroundForCorners != null)
					{
						bb.Dispose();
						bp.Dispose();
					}
				}

				path.CloseFigure();

				if (brush != null)
				{
					g.FillPath(brush, path);
				}

				if (pen != null)
				{
					g.DrawPath(pen, path);
				}
			}

			g.Clip = originalGClip;

			if (originalPenWidth != null)
			{
				pen.Width = originalPenWidth.Value;
			}
		}
	}
}
