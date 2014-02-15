using System;
using System.IO;


namespace mp3info
{
	/// <summary>
	/// Summary description for mp3info.
	/// </summary>
	public class mp3info
	{
		public string filename;
		public long fileSize;
		public long length; // in seconds
		
		public ID3v1 id3v1;
		public ID3v2 id3v2;
		public MPEG mpeg;

		public bool hasID3v1;
		public bool hasID3v2;

		private void Initialize_Components()
		{
			filename = "";
		}



		public mp3info()
		{
			Initialize_Components();
		
		}

		public mp3info(string fileName)
		{
			Initialize_Components();
			this.filename = fileName;
		}

		public void ReadAll()
		{
			if (this.filename.Equals(""))
			{
				// we are fucked we need a filename
			}
			else
			{
				ReadAll (this.filename);
			}
		}

		private void CalculateLength()
		{
			FileInfo fi = new FileInfo(this.filename);
			this.fileSize = fi.Length;
			this.mpeg.audioBytes = this.fileSize - this.mpeg.headerPosition;
			try
			{
				int bitrate = System.Convert.ToInt32(this.mpeg.Bitrate);
				if (bitrate > 0)
				{
					if (this.id3v1.hasTag)
					{
						this.length = ((this.mpeg.audioBytes - 128 )* 8) / (1000 * bitrate);
					} 
					else 
					{
						this.length = (this.mpeg.audioBytes * 8) / (1000 * bitrate);
					}
				}

			}
			catch (Exception e)
			{
				this.length = 0;
			}


		}
		public void ReadAll(string file)
		{
			if (this.filename.Equals(""))
			{
				// we are fucked, we need a filename
				return;
			}
			else
			{
				ReadID3v1(this.filename);
				ReadID3v2(this.filename);
				ReadMPEG(this.filename);
				CalculateLength();
			}
		}

		public void ReadID3v1()
		{
			if (this.filename.Equals(""))
			{
				// we are fucked we need a filename
			}
			else
			{
				ReadID3v1 (this.filename);
			}
		}

		public void ReadID3v1(string file)
		{
			// read id3 stuff
			id3v1 = new ID3v1(file);
			id3v1.Read();
			this.hasID3v1 = id3v1.hasTag;;
		}

		public void ReadID3v2()
		{
			if (this.filename.Equals(""))
			{
				// we are fucked we need a filename
			}
			else
			{
				ReadID3v2 (this.filename);
			}
		}

		public void ReadID3v2(string file)
		{
			// read id3 stuff
			id3v2 = new ID3v2(file);
			id3v2.Read();
			this.hasID3v2 = id3v2.hasTag;
		}

		public void ReadMPEG()
		{
			if (this.filename.Equals(""))
			{
				// we are fucked we need a filename
			}
			else
			{
				ReadMPEG (this.filename);
			}
		}

		public void ReadMPEG(string file)
		{
			// read id3 stuff
			mpeg = new MPEG(file);
			mpeg.Read();
		}

	}
}
