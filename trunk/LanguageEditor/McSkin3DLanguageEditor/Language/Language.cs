using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Globalization;

namespace MCSkin3DLanguageEditor.Language
{
    public class Language
    {
        public StreamReader TextStream { get; private set; }
        public Dictionary<string, string> OtherTable { get; private set; }
        public Dictionary<string, string> StringTable { get; private set; }
        public ToolStripMenuItem Item { get; set; }
        public static string[] LangVars = { "#Name", "#Version", "#SuppVersion", "#Culture", "#Author", "#AuthorContact", "#AuthorSite" };
        public static CultureInfo[] cultureInfos = CultureInfo.GetCultures(CultureTypes.NeutralCultures);
        public string text;

        public Language()
        {
            StringTable = new Dictionary<string, string>();
            OtherTable = new Dictionary<string, string>();
        }

        public static Language Parse(StreamReader sr)
        {
            Language lang = new Language();
            bool headerFound = false;
            string s = "";

            lang.TextStream = sr;

            while (!sr.EndOfStream)
            {
                string line = sr.ReadLine();
                lang.text = lang.text + line + "\n";
                if (line.StartsWith("//") || string.IsNullOrEmpty(line))
                    continue;

                if (line == "MCSkin3D Language File")
                {
                    headerFound = true;
                    continue;
                }

                if (!headerFound)
                    throw new NoHeaderException("No header");

                if (!line.Contains('='))
                    throw new ParseErrorException("Parse error");
            
                var nfLeft = line.Substring(0, line.IndexOf('='));
                var left = nfLeft.Trim();
                s = s + left + ", ";
                var nfRight = line.Substring(line.IndexOf('=') + 1);
                if (nfRight.StartsWith("\"") && nfRight.EndsWith("\""))
                    nfRight = nfRight.Substring(1, nfRight.Length - 2);
                var right = nfRight.Trim(' ', '\t', '\"', '\'').Replace("\\r", "\r").Replace("\\n", "\n");
                if (right.StartsWith("\"") && right.EndsWith("\""))
                    right = right.Substring(1, right.Length - 2);

                if (left[0] == '#')
                    lang.OtherTable.Add(nfLeft.Remove(0, 1), nfRight);
                else
                    lang.StringTable.Add(nfLeft, nfRight);
            }
            return lang;
        }

        public void SaveLanguage(Language lang, Dictionary<string, string> otherTable, Dictionary<string, string> stringTable, string saveToPath)
        {
            lang.OtherTable = new Dictionary<string, string>();
            lang.StringTable = new Dictionary<string, string>();

            int otherIndex = 0;
            int stringIndex = 0;
            string otherKey = "";
            string otherValue = "";
            string stringKey = "";
            string stringValue = "";
            string newLang = "";

            int i = 0;
            StringReader sr = new StringReader(lang.text);
            
            while (true)
            {
                string line = sr.ReadLine();
                string newLine = line;

                if (line == null)
                    break;

                if (line == "")
                {
                    newLang = newLang + Environment.NewLine;
                    continue;
                }
                else if (line.StartsWith("//") || line == "MCSkin3D Language File")
                {
                    newLang = newLang + newLine + Environment.NewLine;
                    continue;
                }

                var left = line.Substring(0, line.IndexOf('='));
                var right = line.Substring(line.IndexOf('=') + 1).Trim(' ', '\t', '\"', '\'').Replace("\\r", "\r").Replace("\\n", "\n");
                if (right.StartsWith("\"") && right.EndsWith("\""))
                    right = right.Substring(1, right.Length - 2);

                if (left[0] == '#')
                {
                    otherKey = otherTable.ElementAt(otherIndex).Key;
                    otherValue = otherTable.ElementAt(otherIndex).Value;
                    if (otherValue.StartsWith("\"") && otherValue.EndsWith("\""))
                        otherValue = otherValue.Substring(1, otherValue.Length - 2);
                    lang.OtherTable.Add(otherKey, "\"" + otherValue + "\"");

                    newLang = newLang + "#" + otherKey + "=\"" + otherValue + "\"" + Environment.NewLine;
                    otherIndex++;
                }
                else
                {
                    stringKey = stringTable.ElementAt(stringIndex).Key;
                    stringValue = stringTable.ElementAt(stringIndex).Value;
                    if (stringValue.StartsWith("\"") && stringValue.EndsWith("\""))
                        stringValue = stringValue.Substring(1, stringValue.Length - 2);
                    lang.StringTable.Add(stringKey, "\"" + stringValue + "\"");

                    newLang = newLang + stringKey + "=\"" + stringValue + "\"" + Environment.NewLine;
                    stringIndex++;
                }
                i++;
                Application.DoEvents();
            }

            StreamWriter sw = new StreamWriter(File.Create(saveToPath), Encoding.Unicode);
            sw.Write(newLang);
            sw.Close();
        }
    }
}

public class NoHeaderException : Exception
{
    public NoHeaderException(String error)
    {
        throw new Exception(error);
    }
}

public class ParseErrorException : Exception
{
    public ParseErrorException(String error)
    {
        throw new Exception(error);
    }
}