using System.Text;
using System.IO;

namespace StonehearthEditor
{
    public class EncounterScriptFile
    {
        private string mPath;
        private string mFileName;
        private string mDefaultJson;

        public EncounterScriptFile(string filePath)
        {
            mPath = filePath;
            mFileName = System.IO.Path.GetFileNameWithoutExtension(mPath);
        }

        public void Load()
        {
            if (System.IO.File.Exists(mPath))
            {
                using (StreamReader sr = new StreamReader(mPath, Encoding.UTF8))
                {
                    string line;
                    bool started = false;
                    StringBuilder sb = new StringBuilder();
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (line.StartsWith("</StonehearthEditor>"))
                        {
                            started = false;
                            break;
                        }

                        if (started)
                        {
                            sb.AppendLine(line);
                        }
                        if (line.StartsWith("<StonehearthEditor>"))
                        {
                            started = true;
                        }
                    }
                    mDefaultJson = sb.ToString();
                }
            }
        }

        public void WriteDefaultToFile(string path)
        {
            using (StreamWriter wr = new StreamWriter(path, false, new UTF8Encoding(false)))
            {
                wr.Write(mDefaultJson);
            }
        }

        public string Name
        {
            get { return mFileName; }
        }

        public string DefaultJson
        {
            get { return mDefaultJson; }
        }

        public string Path
        {
            get { return mPath; }
        }
    }
}
