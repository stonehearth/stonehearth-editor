using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using NJsonSchema;

namespace StonehearthEditor
{
    public class EncounterScriptFile
    {
        private string mPath;
        private string mFileName;
        private string mDefaultJson;
        private JsonSchema4 mSchema;

        public EncounterScriptFile(string filePath)
        {
            mPath = filePath;
            mFileName = System.IO.Path.GetFileNameWithoutExtension(mPath);
        }

        public void Load()
        {
            mDefaultJson = ExtractTextRange(mPath, "<StonehearthEditor>", "</StonehearthEditor>");
            var schemaText = ExtractTextRange(mPath, "<StonehearthEditorSchema>", "</StonehearthEditorSchema>");
            if (schemaText.Length > 0)
            {
                try
                {
                    mSchema = JsonSchemaTools.ParseSchema(schemaText, Application.StartupPath + "/schemas/encounters/tmp.json");
                }
                catch (System.Exception e)
                {
                    MessageBox.Show("Encounter type specifies a schema, but it isn't valid.\nFile: " + mPath + "\nError: " + e.Message);
                }
            }
        }

        private static string ExtractTextRange(string filePath, string startToken, string endToken)
        {
            if (System.IO.File.Exists(filePath))
            {
                using (StreamReader sr = new StreamReader(filePath, Encoding.UTF8))
                {
                    string line;
                    bool started = false;
                    StringBuilder sb = new StringBuilder();
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (line.TrimStart().StartsWith(endToken))
                        {
                            started = false;
                            break;
                        }

                        if (started)
                        {
                            sb.AppendLine(line);
                        }

                        if (line.TrimStart().StartsWith(startToken))
                        {
                            started = true;
                        }
                    }

                    return sb.ToString();
                }
            }

            return "";
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

        public JsonSchema4 Schema
        {
            get { return mSchema; }
        }
    }
}
