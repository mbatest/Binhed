using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.ComponentModel;
using Utils;
namespace BookReader
{
    /*
     * %!PS-Adobe-2.0
%%creator: dvips(k) 5.95a Copyright 2005 Radical Eye Software
%%title: texput.dvi
%%Pages: 1
%%PageOrder: Ascend
%%BoundingBox: 0 0 612 792
%%DocumentPaperSizes: Letter
%%EndComments
     */
    public class PostScriptReader:LOCALIZED_DATA
    {
        List<string> contents = new List<string>();
        List<string> pages = new List<string>();
        List<string> comments = new List<string>();
        [CategoryAttribute("Ps"), DescriptionAttribute("Pages")]

        public List<string> Comments
        {
            get { return comments; }
            set { comments = value; }
        }
        [CategoryAttribute("Ps"), DescriptionAttribute("Pages")]
        public List<string> Pages
        {
            get { return pages; }
            set { pages = value; }
        }
        [CategoryAttribute("Ps"), DescriptionAttribute("Content")]
        public List<string> Contents
        {
            get { return contents; }
            set { contents = value; }
        }
        public PostScriptReader(string FileName)
        {
            string title;
            StreamReader FS = new StreamReader(FileName);

            string lineRead = FS.ReadLine();
            int lineNumber = 1;
            AddLine(lineRead, ref lineNumber);
            while (!FS.EndOfStream)
            {

                try
                {
                    lineRead = FS.ReadLine();
                    lineNumber++;
                    string start = "";
                    if (lineRead.Length > 2)
                    {
                        start = lineRead.Substring(0, 2);
                        switch (start)
                        {
                            case "%%":
                                #region Comments
                                AddLine(lineRead, ref lineNumber);
                                string[] data = lineRead.Split(':');
                                if (data.Length > 1)
                                {
                                    switch (data[0].ToLower())
                                    {
                                        case "%%begindefault":
                                            while (!lineRead.ToLower().StartsWith("%%enddefault"))
                                            {
                                                lineRead = FS.ReadLine();
                                                lineNumber++;
                                                if (lineRead.StartsWith("%"))
                                                {
                                                    AddLine(lineRead, ref lineNumber);
                                                }
                                            }
                                            break;
                                        case "%%beginsetup":
                                            while (!lineRead.ToLower().StartsWith("%%endsetup"))
                                            {
                                                lineRead = FS.ReadLine();
                                                lineNumber++;
                                                if (lineRead.StartsWith("%"))
                                                {
                                                    AddLine(lineRead, ref lineNumber);
                                                }
                                            }
                                            break;
                                        case "%%beginprolog":
                                            while (!lineRead.ToLower().StartsWith("%%endprolog"))
                                            {
                                                lineRead = FS.ReadLine();
                                                lineNumber++;
                                                if (lineRead.StartsWith("%"))
                                                {
                                                    AddLine(lineRead, ref lineNumber);
                                                }
                                            }
                                            break;
                                        case "%%beginprocset":
                                            while (!lineRead.ToLower().StartsWith("%%endprocset"))
                                            {
                                                lineRead = FS.ReadLine();
                                                lineNumber++;
                                                if (lineRead.StartsWith("%"))
                                                {
                                                    AddLine(lineRead, ref lineNumber);
                                                }
                                            }
                                            break;
                                        case "%%title":
                                            title = data[1];
                                            break;
                                        case "%%version":
                                            break;
                                        case "%%creator":
                                            break;
                                        case "%%creationdate":
                                            break;
                                        case "%%documentdata":
                                            break;
                                        case "%%languagelevel":
                                            break;
                                        case "%%boundingbox":
                                            break;
                                        case "%%hiresboundingbox":
                                            break;
                                        case "%%pages":
                                            break;
                                        case "%%pageorder":
                                            break;
                                        case "%%documentprocesscolors":
                                            break;
                                        case "%%documentsuppliedresources":
                                            break;
                                        case "%%feature":
                                            break;
                                        case "%%papersize":
                                            break;
                                        case "%%page":
                                            string text = "";
                                            while (!lineRead.Contains(" eop"))
                                            {
                                                lineRead = FS.ReadLine();
                                                text += InnerText(lineRead);
                                                lineNumber++;
                                            }
                                            pages.Add(text);
                                            break;
                                    }
                                }
                                #endregion
                                break;
                            case "%!":
                                break;
                            case "% ":
                                comments.Add(lineRead);
                                break;
                            default:
                                break;
                        }
                    }
                }
                catch (Exception e) { }
            }

        }
        private string InnerText(string t)
        {
            string toShow = "";
            int start = 0;
            while (start < t.Length)
            {
                if (toShow.Contains("suf"))
                {
                }
                start = t.IndexOf('(', start) + 1;
                if (start > 0)
                {
                    int f = t.IndexOf(')', start);
                    if (f > -1)
                        toShow += t.Substring(start, f - start);
                    else
                        break;
                    start++;
                }
                else
                    break;
            }
            return toShow;
        }

        private void AddLine(string s, ref int number)
        {
            contents.Add(number.ToString() + " : " + s);
        }
    }
}
