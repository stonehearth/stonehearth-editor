using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Microsoft.Msagl.Drawing;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Windows.Forms;
using System.Linq;

namespace StonehearthEditor
{
    public enum GameMasterNodeType
    {
        UNSET = 0,
        CAMPAIGN = 1,
        ARC = 2,
        ENCOUNTER = 3,
        CAMP_PIECE = 4,
        UNKNOWN = 5,
    }

    public class GameMasterNode
    {
        private static int kNodeIndex = 0;
        public static readonly Color kPurple = new Color(224, 210, 227);
        public static readonly Color kGreen = new Color(196, 243, 177);
        private string mPath;
        private string mDirectory;
        private string mFileName;
        private JsonFileData mJsonFileData;
        private NodeData mNodeData;
        public GameMasterNode Owner;
        private string mModule;

        public bool IsModified = false;

        private Dictionary<string, string> mEncounters = new Dictionary<string, string>();
        private GameMasterNodeType mNodeType = GameMasterNodeType.UNSET;
        public GameMasterNode(string module, string filePath)
        {
            mModule = module;
            mPath = filePath;
            mDirectory = JsonHelper.NormalizeSystemPath(System.IO.Path.GetDirectoryName(mPath));
            mFileName = System.IO.Path.GetFileNameWithoutExtension(mPath);
            kNodeIndex++;
        }

        public string Id
        {
            get { return mPath; }
        }

        public string Name
        {
            get { return mFileName; }
        }

        public string Path
        {
            get { return mPath; }
        }
        public string Directory
        {
            get { return mDirectory; }
        }
        public GameMasterNodeType NodeType
        {
            get { return mNodeType; }
        }

        public NodeData NodeData
        {
            get { return mNodeData; }
        }

        public FileData FileData
        {
            get { return mJsonFileData; }
        }

        public JObject Json
        {
            get { return mJsonFileData.Json; }
        }
        public string Module
        {
            get { return mModule; }
        }

        public void Load(Dictionary<string, GameMasterNode> allNodes)
        {
            try
            {
                mJsonFileData = new JsonFileData(mPath);
                mJsonFileData.Load();
                OnFileChanged(allNodes);
            }
            catch (Exception e)
            {
                MessageBox.Show("Unable to load " + mPath + ". Error: " + e.Message);
            }
            if (mNodeData != null)
            {
                mNodeData.PostLoadFixup();
            }
        }

        public void OnFileChanged(Dictionary<string, GameMasterNode> allNodes)
        {
            JToken fileTypeToken = Json["type"];
            string fileType = fileTypeToken != null ? fileTypeToken.ToString().ToUpper() : "";
            GameMasterNodeType newNodeType = GameMasterNodeType.UNKNOWN;
            foreach (GameMasterNodeType nodeType in Enum.GetValues(typeof(GameMasterNodeType)))
            {
                if (fileType.Equals(nodeType.ToString()))
                {
                    newNodeType = nodeType;
                }
            }
            if (newNodeType != mNodeType)
            {
                mNodeType = newNodeType;
                switch (mNodeType)
                {
                    case GameMasterNodeType.ENCOUNTER:
                        mNodeData = new EncounterNodeData();
                        break;
                    case GameMasterNodeType.ARC:
                        mNodeData = new ArcNodeData();
                        break;
                    case GameMasterNodeType.CAMPAIGN:
                        mNodeData = new CampaignNodeData();
                        break;
                    case GameMasterNodeType.CAMP_PIECE:
                        mNodeData = new CampPieceNodeData();
                        break;
                    default:
                        Console.WriteLine("unknown encounter node type for file " + Path);
                        mNodeData = new UnknownNodeData();
                        break;
                }
            }
            if (mNodeData != null)
            {
                mNodeData.NodeFile = this;
                mNodeData.LoadData(allNodes);
            }
        }

        /**
        Note: this is expensive!
        **/
        public string GetJsonFileString()
        {
            try
            {
                StringWriter stringWriter = new StringWriter();
                using (JsonTextWriter jsonTextWriter = new JsonTextWriter(stringWriter))
                {
                    jsonTextWriter.Formatting = Newtonsoft.Json.Formatting.Indented;
                    jsonTextWriter.Indentation = 3;
                    jsonTextWriter.IndentChar = ' ';

                    JsonSerializer jsonSeralizer = new JsonSerializer();
                    jsonSeralizer.Serialize(jsonTextWriter, Json);
                }
                return stringWriter.ToString();
            }
            catch (Exception e)
            {
                Console.WriteLine("Could not convert " + mPath + " to string because of exception " + e.Message);
            }
            return "INVALID JSON";
        }

        public bool TryModifyJson(string newJsonString)
        {
            try
            {
                JObject newJson = JObject.Parse(newJsonString);
                if (newJson != null)
                {
                    if (newJson.ToString().Equals(Json.ToString()))
                    {
                        return false; // not modified because jsons are equivalent
                    }
                    mJsonFileData.TrySetFlatFileData(newJsonString);
                    IsModified = true;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Unable to modify json. Error: " + e.Message);
                return false;
            }
            return true;
        }

        public void SaveIfNecessary()
        {
            if (IsModified)
            {
                try
                {
                    using (StreamWriter wr = new StreamWriter(mPath, false, new UTF8Encoding(false)))
                    {
                        string jsonAsString = GetJsonFileString();
                        wr.Write(jsonAsString);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Could not write to file " + mPath + " because of exception: " + e.Message);
                }
            }
        }
        public GameMasterNode Clone(string newFileName)
        {
            try
            {
                string newPath = mDirectory + '/' + newFileName + ".json";
                GameMasterNode newNode = new GameMasterNode(mModule, newPath);
                newNode.IsModified = true;
                NodeData newNodeData = NodeData.Clone(newNode);
                newNodeData.NodeFile = newNode;
                newNode.mNodeData = newNodeData;
                newNode.mNodeType = NodeType;

                newNode.mJsonFileData.TrySetFlatFileData(Json.ToString());
                return newNode;
            }
            catch (Exception e)
            {
                MessageBox.Show("Unable to clone Game Master Node to " + newFileName + ". Error: " + e.Message);
            }
            return null;
        }
    }
}
