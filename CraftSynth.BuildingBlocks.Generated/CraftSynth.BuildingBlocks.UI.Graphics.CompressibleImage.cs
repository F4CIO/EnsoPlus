using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;

namespace CraftSynth.BuildingBlocks.UI.Graphics
{
	[Serializable]
	public class CompressibleImage:IDisposable
	{
		#region Private Members
		private Image _image;
		private byte[] _pngBytes;
		private Size? _size;
		private bool? _isTransparent = null;
		[NonSerialized] private object _lock = new object();
		#endregion

		#region Properties
		/// <summary>
		/// If outside of this class always use .IsNull() method instead
		/// </summary>
		public bool IsNull
		{
			get
			{
				//TODO: not sure why this in constructor doesen't help for serialized objects 
				if (this._lock == null)
				{
					this._lock = new object();
				}

				lock (this._lock)
				{
					return this._pngBytes == null && this._image == null;
				}
			}
		}

		public bool IsCompressed
		{
			get
			{
				return this.IsNull || (this._pngBytes != null && this._image == null);
			}
		}

		public bool IsTransparent
		{
			get
			{
				if (this._isTransparent == null)
				{
					lock (this._lock)
					{
						using (var image = this.GetAsAutoDisposableImage())
						{
							
							this._isTransparent = IsTransparent_Inner(image.Image, false, 1000);
						}
					}
				}
				return this._isTransparent.Value;
			}
		}
		#endregion

		#region Public Methods	
		public Size? GetSize(bool decompressIfNecessary, bool ifDecompressingOccuredLeaveDecompressed = false)
		{
				lock (this._lock)
				{
					if (this._size != null)
					{
						return this._size;
					}
					else if (this._pngBytes != null)
					{
						Image t = null;
						if (decompressIfNecessary)
						{
							t = CompressibleImage.UncompressFromPng(this._pngBytes);
							this._size = t.Size;
						}
						if (t!=null && ifDecompressingOccuredLeaveDecompressed)
						{
							this._image = t;
						}
						return this._size;
					}
					else
					{
						return null;
					}
				}
		}
		
		/// <summary>
		/// IMPORTANT: You must see GetAsAutoDisposableImage() comment first.
		/// </summary>
		/// <returns></returns>
		public Image GetAsImage()
		{
			lock (this._lock)
			{
				if (this._image != null)
				{
					return this._image;
				}
				else
				{
					if (this._pngBytes != null)
					{
						Image r = CompressibleImage.UncompressFromPng(this._pngBytes);
						this._size = r.Size;
						return r;
					}
					else
					{
						return null;
					}
				} 
			}
		}
		
		/// <summary>
		/// Returns image that is avare of below logic and according to it it will be auto-disposed if neccessary.
		/// Logic: 
		/// If we have image only in compressed version (png) we create and return unpacked version of it. This unpacked version should be disposed after using.
		/// If we have image in uncompressed version (bmp) we return reference to it. Dispose should not be called on that reference.
		/// </summary>
		/// <returns></returns>
		public AutoDisposableImage GetAsAutoDisposableImage()
		{
			lock (this._lock)
			{
				return new AutoDisposableImage(this.GetAsImage(), this._image==null);
			}
		}

		public byte[] GetAsPngBytes(bool canUseImage = true)
		{
			//TODO: not sure why without this lock can be null later 
			if (this._lock == null)
			{
				this._lock = new object();
			}

			lock (this._lock)
			{
				if (this._pngBytes != null)
				{
					return this._pngBytes;
				}
				else
				{
					if (canUseImage && this._image != null)
					{
						this._pngBytes = CompressibleImage.CompressToPng(this._image);
						return this._pngBytes;
					}
					else
					{
						return null;
					}
				} 
			}
		}

		public void Compress()
		{
			lock (this._lock)
			{
				if (this._image != null)
				{
					if (this._pngBytes == null)
					{
						this._pngBytes = CompressibleImage.CompressToPng(this._image);
					}
					this._image.Dispose();
					this._image = null;
				} 
			}
		}

		public void Decompress()
		{
			lock (this._lock)
			{
				if (this._image == null && this._pngBytes!=null)
				{
					this._image = CompressibleImage.UncompressFromPng(this._pngBytes);
					this._size = this._image.Size;
				} 
			}
		}
		#endregion

		#region Constructors And Initialization

		public CompressibleImage(byte[] pngBytes)
		{
			//TODO: not sure why without this lock can be null later 
			if (this._lock == null)
			{
				this._lock = new object();
			}
			lock (this._lock)
			{
				this._size = null;
				this._image = null;
				this._pngBytes = pngBytes;
			}
		}

		public CompressibleImage(Image image)
		{
			//TODO: not sure why without this lock can be null later 
			if (this._lock == null)
			{
				this._lock = new object();
			}
			lock (this._lock)
			{
				this._pngBytes = null;
				this._image = image;
				if (image != null)
				{
					this._size = image.Size;
				}
			}
		}
		#endregion

		#region Deinitialization And Destructors
		// Public implementation of Dispose pattern callable by consumers. 
		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);       
		}

		// Flag: Has Dispose already been called? 
		bool _disposed = false;

		// Protected implementation of Dispose pattern. 
		protected virtual void Dispose(bool disposing)
		{
			if (!this._disposed)
			{

				if (disposing)
				{
					// Free any managed objects here. 
					//
				}

				// Free any unmanaged objects here. 
				//
				if (this._image != null)
				{
					this._image.Dispose();
					this._image = null;
				}

				this._disposed = true;
			}
		}

		~CompressibleImage()
		{
			 this.Dispose(false);
		}
		#endregion        

		#region Event Handlers
		#endregion

		#region Private Methods
		private static byte[] CompressToPng(Image image)
		{
			byte[] bytes;

			using (var memStream = new MemoryStream())
			{
				//CraftSynth.BuildingBlocks.Common.Misc.BeepInNewThread(5000,50);
				image.Save(memStream, ImageFormat.Png);
				bytes = memStream.ToArray();
			}

			return bytes;
		}

		private static Image UncompressFromPng(byte[] bytes)
		{
			Image r;
			//CraftSynth.BuildingBlocks.Common.Misc.BeepInNewThread(3000,50);
			using (var memStream = new MemoryStream())
			{
				memStream.Write(bytes, 0, bytes.Length);
				r = Image.FromStream(memStream, false, false);
			}

			return r;
		}
		#endregion

		#region Helpers
		private bool IsTransparent_Inner(Image image, bool checkAllPixels, int countOfRandomPixelsToCheck)
		{
			//Copied from CraftSynth.BuildingBlocks
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
		#endregion
	}

	public static class CompressibleImageExtender
	{
		public static bool IsNull(this CompressibleImage ci)
		{
			if (ci == null)
			{
				return true;
			}
			else
			{
				return ci.IsNull;
			}
		}
	}

	/// <summary>
	/// ImageThatWillBeDisposedOnlyIfNotKeptByCompressibleImage
	/// </summary>
	public class AutoDisposableImage: IDisposable
	{
		public Image Image;
		private readonly bool _imageShouldBeAutoDisposed;

		public AutoDisposableImage(Image image, bool shouldBeAutoDisposed = true)
		{
			this.Image = image;
			this._imageShouldBeAutoDisposed = shouldBeAutoDisposed;
		}


		// Public implementation of Dispose pattern callable by consumers. 
		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);       
		}

		// Flag: Has Dispose already been called? 
		bool _disposed = false;

		// Protected implementation of Dispose pattern. 
		protected virtual void Dispose(bool disposing)
		{
			if (!this._disposed)
			{

				if (disposing)
				{
					// Free any managed objects here. 
					//
				}

				// Free any unmanaged objects here. 
				//
				if (this._imageShouldBeAutoDisposed)
				{
					this.Image.Dispose();
					this.Image = null;
				}

				this._disposed = true;
			}
		}

		~AutoDisposableImage()
		{
			 this.Dispose(false);
		}
	}
}
