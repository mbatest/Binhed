using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace MP3Library
{
    public class Author
    {
        public override string ToString()
        {
            return AuthorName;
        }
        public string AuthorName;
        public int AuthorCode;
        public List<Album> albums = new List<Album>();
    }
    public class Composer
    {
        public override string ToString()
        {
            return ComposerName;
        }
        public string ComposerName;
        public int ComposerCode;
        public List<MusicFileClass> tracks = new List<MusicFileClass>();
    }
    public class Album
    {
        public bool treated = false;
        public override string ToString()
        {
            return AlbumName + " " + AuthorName;
        }
        public string AlbumName;
        public string AuthorName;
        public int Year;
        public List<Author> Authors;
        public string ComposerName;
        public String LargeImagePath()
        {
            // pictureBox1.Image = null;
            string path = Path.GetDirectoryName(FileName) + @"\AlbumArt_{" + AlbumId.ToString() + "}_Large.jpg";
            return path;
        }
        public String SmallImagePath
        {
            get
            {
                string path = Path.GetDirectoryName(FileName) + @"\AlbumArt_{" + AlbumId.ToString() + "}_Small.jpg";
                return path;
            }
        }   Guid AlbumId;
        string FileName;
        public List<MusicFileClass> mpTrack = new List<MusicFileClass>();
        public Album(string name, Guid Id)
        {
            FileName = name;
            AlbumId = Id;
        }
        public Album()
        { }
    }

}
