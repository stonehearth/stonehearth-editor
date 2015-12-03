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
      UNKNOWN = 0,
      CAMPAIGN = 1,
      ARC = 2,
      ENCOUNTER = 3,
      CAMP_PIECE = 4,
   };

   public class GameMasterNode
   {
      private static int kNodeIndex = 0;
      public static Color kPurple = new Color(224, 210, 227);
      public static Color kGreen = new Color(196, 243, 177);
      private string mPath;
      private string mDirectory;
      private string mFileName;
      private JObject mJson;
      private NodeData mNodeData;
      public GameMasterNode Owner;
      private string mModule;

      public bool IsModified = false;

      private Dictionary<string, string> mEncounters = new Dictionary<string, string>();
      private GameMasterNodeType mNodeType = GameMasterNodeType.UNKNOWN;
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

      public JObject Json
      {
         get { return mJson; }
      }
      public string Module
      {
         get { return mModule; }
      }

      public void Load(Dictionary<string, GameMasterNode> allNodes)
      {
         try
         {
            using (StreamReader sr = new StreamReader(mPath, Encoding.UTF8))
            {
               string fileString = sr.ReadToEnd();

               mJson = JObject.Parse(fileString);
               OnFileChanged(allNodes);
            }
         } catch(Exception e)
         {
            MessageBox.Show("Unable to load " + mPath + ". Error: " + e.Message);
         }
      }

      public void OnFileChanged(Dictionary<string, GameMasterNode> allNodes)
      {
         JToken fileTypeToken = mJson["type"];
         string fileType = fileTypeToken != null? fileTypeToken.ToString().ToUpper() : "";
         GameMasterNodeType newNodeType = mNodeType;
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
               jsonSeralizer.Serialize(jsonTextWriter, mJson);
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
         try {
            JObject newJson = JObject.Parse(newJsonString);
            if (newJson != null)
            {
               if (newJson.ToString().Equals(mJson.ToString()))
               {
                  return false; // not modified because jsons are equivalent
               }
               mJson = newJson;
               IsModified = true;
            }
         } catch(Exception e)
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
            try {
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
         try {
            string newPath = mDirectory + '/' + newFileName + ".json";
            GameMasterNode newNode = new GameMasterNode(mModule, newPath);
            newNode.IsModified = true;
            NodeData newNodeData = NodeData.Clone(newNode);
            newNodeData.NodeFile = newNode;
            newNode.mNodeData = newNodeData;
            newNode.mNodeType = NodeType;

            newNode.mJson = JObject.Parse(mJson.ToString());
            return newNode;
         } catch(Exception e)
         {
            MessageBox.Show("Unable to clone Game Master Node to " + newFileName + ". Error: " + e.Message);
         }
         return null;
      }
   }

   public abstract class NodeData
   {
      public GameMasterNode NodeFile;
      public abstract void LoadData(Dictionary<string, GameMasterNode> allNodes);

      public virtual void UpdateGraph(Graph graph)
      {
         Node graphNode = graph.AddNode(NodeFile.Id);
         graphNode.LabelText = NodeFile.Name;
         UpdateGraphNode(graphNode);
         UpdateOutEdges(graph);
      }
      public virtual void UpdateGraphNode(Node graphNode)
      {
         graphNode.Attr.LabelWidthToHeightRatio = 1;
         graphNode.Attr.Shape = Shape.Box;
         graphNode.Attr.FillColor = GameMasterNode.kGreen;
         graphNode.Attr.LabelMargin = 6;
      }

      protected abstract void UpdateOutEdges(Graph graph);

      public virtual void GetRelatedNodes(HashSet<GameMasterNode> set)
      {
         set.Add(NodeFile);
      }

      public abstract NodeData Clone(GameMasterNode nodeFile);

      public virtual bool AddOutEdge(GameMasterNode nodeFile)
      {
         return false;
      }
      protected void MakeNodePrivate(Node node)
      {
         node.Attr.Shape = Shape.Box;
         node.Attr.LabelMargin = 3;
         node.Attr.FillColor = GameMasterNode.kPurple;
         node.Label.FontSize = 6;
      }
   }

   public class CampaignNodeData : NodeData
   {
      private string mRarity;
      private Dictionary<string, GameMasterNode> mArcTriggers;
      private Dictionary<string, GameMasterNode> mArcChallenges;
      private Dictionary<string, GameMasterNode> mArcClimaxes;
      private int mNumArcNodes = 0;
      public List<GameMasterNode> OrphanedNodes = new List<GameMasterNode>();

      public override void LoadData(Dictionary<string, GameMasterNode> allNodes)
      {
         mArcTriggers = new Dictionary<string, GameMasterNode>();
         mArcChallenges = new Dictionary<string, GameMasterNode>();
         mArcClimaxes = new Dictionary<string, GameMasterNode>();
         mNumArcNodes = 0;
         mRarity = NodeFile.Json["rarity"].ToString();
         JToken arcs = NodeFile.Json["arcs"];

         Dictionary<string, string> triggers = JsonConvert.DeserializeObject<Dictionary<string, string>>(arcs["trigger"].ToString());
         Dictionary<string, string> challenges = JsonConvert.DeserializeObject<Dictionary<string, string>>(arcs["challenge"].ToString());
         Dictionary<string, string> climaxes = JsonConvert.DeserializeObject<Dictionary<string, string>>(arcs["climax"].ToString());

         SetSelfAsOwner(triggers, mArcTriggers, allNodes);
         SetSelfAsOwner(challenges, mArcChallenges, allNodes);
         SetSelfAsOwner(climaxes, mArcClimaxes, allNodes);
      }

      private void SetSelfAsOwner(Dictionary<string, string> children, Dictionary<string, GameMasterNode> toUpdate, Dictionary<string, GameMasterNode> allNodes)
      {
         int lastIndexOfSlash = NodeFile.Path.LastIndexOf('/');
         string nodeFilePathWithoutFileName = NodeFile.Path.Substring(0, lastIndexOfSlash);
         foreach (KeyValuePair<string, string> child in children)
         {
            string absoluteFilePath = JsonHelper.GetFileFromFileJson(child.Value, nodeFilePathWithoutFileName);
            GameMasterNode otherFile = null;
            if (allNodes.TryGetValue(absoluteFilePath, out otherFile))
            {
               // this is a proper edge
               otherFile.Owner = NodeFile;
               toUpdate.Add(child.Key, otherFile);
               mNumArcNodes++;
            }
         }
      }

      public override void UpdateGraphNode(Node graphNode)
      {
         base.UpdateGraphNode(graphNode);
         graphNode.Attr.FillColor = Color.LightBlue;
      }

      public override void GetRelatedNodes(HashSet<GameMasterNode> set)
      {
         base.GetRelatedNodes(set);
         
         foreach (GameMasterNode node in mArcTriggers.Values)
         {
            if (!set.Contains(node))
            {
               set.Add(node);
               node.NodeData.GetRelatedNodes(set);
            }
         }
         foreach (GameMasterNode node in mArcChallenges.Values)
         {
            if (!set.Contains(node))
            {
               set.Add(node);
               node.NodeData.GetRelatedNodes(set);
            }
         }
         foreach (GameMasterNode node in mArcClimaxes.Values)
         {
            if (!set.Contains(node))
            {
               set.Add(node);
               node.NodeData.GetRelatedNodes(set);
            }
         }
         foreach (GameMasterNode node in OrphanedNodes)
         {
            if (!set.Contains(node))
            {
               set.Add(node);
               node.NodeData.GetRelatedNodes(set);
            }
         }
      }

      public override NodeData Clone(GameMasterNode nodeFile)
      {
         return new CampaignNodeData();
      }

      protected override void UpdateOutEdges(Graph graph)
      {
         foreach (GameMasterNode node in mArcTriggers.Values)
         {
            if (node.NodeType == GameMasterNodeType.ARC)
            {
               Node triggerNode = graph.AddNode(NodeFile.Id + "#trigger");
               triggerNode.LabelText = "trigger";
               MakeNodePrivate(triggerNode);
               graph.AddEdge(NodeFile.Id, triggerNode.Id);
               graph.AddEdge(triggerNode.Id, node.Id);
            }
         }
         foreach (GameMasterNode node in mArcChallenges.Values)
         {
            if (node.NodeType == GameMasterNodeType.ARC)
            {
               Node triggerNode = graph.AddNode(NodeFile.Id + "#challenge");
               triggerNode.LabelText = "challenge";
               MakeNodePrivate(triggerNode);
               graph.AddEdge(NodeFile.Id, triggerNode.Id);
               graph.AddEdge(triggerNode.Id, node.Id);
            }
         }
         foreach (GameMasterNode node in mArcClimaxes.Values)
         {
            if (node.NodeType == GameMasterNodeType.ARC)
            {
               Node triggerNode = graph.AddNode(NodeFile.Id + "#climax");
               triggerNode.LabelText = "climax";
               MakeNodePrivate(triggerNode);
               graph.AddEdge(NodeFile.Id, triggerNode.Id);
               graph.AddEdge(triggerNode.Id, node.Id);
            }
         }
      }

      public override bool AddOutEdge(GameMasterNode nodeFile)
      {
         if (nodeFile.NodeType == GameMasterNodeType.ARC)
         {
            mArcTriggers.Add(nodeFile.Name, nodeFile);
            return true;
         }
         return false;
      }
   }

   public class ArcNodeData : NodeData
   {
      private string mRarity;
      private Dictionary<string, string> mEncounters;
      private List<GameMasterNode> mEncounterFiles;
      public override void LoadData(Dictionary<string, GameMasterNode> allNodes)
      {
         mEncounters = new Dictionary<string, string>();
         mEncounterFiles = new List<GameMasterNode>();
         mRarity = NodeFile.Json["rarity"].ToString();
         mEncounters = JsonConvert.DeserializeObject<Dictionary<string, string>>(NodeFile.Json["encounters"].ToString());
         int lastIndexOfSlash = NodeFile.Path.LastIndexOf('/');
         string nodeFilePathWithoutFileName = NodeFile.Path.Substring(0, lastIndexOfSlash);
         foreach(string filename in mEncounters.Values)
         {
            string absoluteFilePath = JsonHelper.GetFileFromFileJson(filename, nodeFilePathWithoutFileName);
            GameMasterNode otherFile = null;
            if (allNodes.TryGetValue(absoluteFilePath, out otherFile))
            {
               // this is a proper edge
               otherFile.Owner = NodeFile;
               mEncounterFiles.Add(otherFile);
            }
         }
      }

      protected override void UpdateOutEdges(Graph graph)
      {
         foreach (GameMasterNode file in mEncounterFiles)
         {
            if (file.NodeType == GameMasterNodeType.ENCOUNTER)
            {
               EncounterNodeData nodeData = file.NodeData as EncounterNodeData;
               if (nodeData.IsStartNode)
               {
                  graph.AddEdge(NodeFile.Id, file.Id);
               }
            }
         }
      }

      public List<GameMasterNode> GetEncountersWithInEdge(string inEdgeName)
      {
         List<GameMasterNode> inEdges = new List<GameMasterNode>();
         foreach (GameMasterNode file in mEncounterFiles)
         {
            if (file.NodeType == GameMasterNodeType.ENCOUNTER)
            {
               EncounterNodeData nodeData = file.NodeData as EncounterNodeData;
               if (nodeData.InEdge.Equals(inEdgeName))
               {
                  inEdges.Add(file);
               }
            }
         }
         return inEdges;
      }

      public override void GetRelatedNodes(HashSet<GameMasterNode> set)
      {
         base.GetRelatedNodes(set);
         foreach (GameMasterNode node in mEncounterFiles)
         {
            if (!set.Contains(node))
            {
               set.Add(node);
               node.NodeData.GetRelatedNodes(set);
            }
         }
      }

      public override NodeData Clone(GameMasterNode nodeFile)
      {
         return new ArcNodeData();
      }
      public void AddEncounter(EncounterNodeData encounter)
      {
         // TODO, get relative path
         GameMasterNode encounterNodeFile = encounter.NodeFile;
         string filePath = encounterNodeFile.Path;
         string selfPath = NodeFile.Directory + '/';
         filePath = "file(" + filePath.Replace(selfPath, "") + ")";
         mEncounters.Add(encounterNodeFile.Name, filePath);
         mEncounterFiles.Add(encounterNodeFile);
         NodeFile.Json["encounters"][encounterNodeFile.Name] = filePath;
         NodeFile.IsModified = true;
      }

      public override bool AddOutEdge(GameMasterNode nodeFile)
      {
         if (nodeFile.NodeType == GameMasterNodeType.ENCOUNTER)
         {
            EncounterNodeData encounterData = nodeFile.NodeData as EncounterNodeData;
            if (encounterData.IsStartNode && !mEncounterFiles.Contains(nodeFile))
            {
               // can only add an edge between this and the encounter if the encounter is a start node
               AddEncounter(encounterData);
               return true;
            }
         }
         return false;
      }
   }

   public class EncounterNodeData: NodeData
   {
      private string mEncounterType;
      private string mInEdge; // This is the name of the node as referred to others by the out edges of others.
      private List<string> mOutEdgeStrings;
      private Dictionary<string, List<string>> mChoiceEdgeInfo; // Map of edge to list of choices
      private bool mIsStartNode = false;

      public bool IsStartNode
      {
         get { return mIsStartNode; }
      }
      public string InEdge
      {
         get { return mInEdge; }
      }
      public string EncounterType
      {
         get { return mEncounterType; }
      }

      private void AddOutEdgesRecursive(JToken outEdgeSpec, List<string> list)
      {
         if (!(outEdgeSpec is JValue))
         {
            string specType = outEdgeSpec["type"].ToString();
            switch (specType)
            {
               case "trigger_one":
               case "trigger_many":
                  JToken outEdges = outEdgeSpec["out_edges"];
                  IList<JToken> results = outEdges.ToList();
                  foreach (JToken child in results)
                  {
                     AddOutEdgesRecursive(child, list);
                  }
                  break;
               case "weighted_edge":
                  JToken outEdge = outEdgeSpec["out_edge"];
                  AddOutEdgesRecursive(outEdge, list);
                  break;
            }
         }
         else
         {
            list.Add(outEdgeSpec.ToString());
         }
      }

      private List<string> ParseOutEdges(JToken outEdgeSpec)
      {
         List<string> returned = new List<string>();
         if (outEdgeSpec != null)
         {
            AddOutEdgesRecursive(outEdgeSpec, returned);
         }
         return returned;
      }

      public override void LoadData(Dictionary<string, GameMasterNode> allNodes)
      {
         mOutEdgeStrings = new List<string>();
         mChoiceEdgeInfo = new Dictionary<string, List<string>>();
         mEncounterType = NodeFile.Json["encounter_type"].ToString();
         mInEdge = NodeFile.Json["in_edge"].ToString();
         if (mInEdge.Equals("start"))
         {
            mIsStartNode = true;
         }
         mOutEdgeStrings = ParseOutEdges(NodeFile.Json["out_edge"]);
         switch(mEncounterType)
         {
            case "generator":
               Dictionary<string, string> generatorInfo = JsonHelper.GetJsonStringDictionary(NodeFile.Json, "generator_info");
               if (generatorInfo.ContainsKey("spawn_edge"))
               {
                  mOutEdgeStrings.Add(generatorInfo["spawn_edge"]);
               }
               break;
            case "collection_quest":
               Dictionary<string, string> collectionEdges = JsonHelper.GetJsonStringDictionary(NodeFile.Json["collection_quest_info"], "out_edges");
               foreach (KeyValuePair<string, string> collectionEdge in collectionEdges)
               {
                  string outEdge = collectionEdge.Value;
                  string choice = collectionEdge.Key;
                  if (!outEdge.Equals("none"))
                  {
                     List<string> list;
                     if (!mChoiceEdgeInfo.TryGetValue(outEdge, out list))
                     {
                        list = new List<string>();
                     }
                     list.Add(choice);
                     mChoiceEdgeInfo[outEdge] = list;
                     if (!mOutEdgeStrings.Contains(outEdge))
                     {
                        mOutEdgeStrings.Add(outEdge);
                     }
                  }
               }
               break;
            case "dialog_tree":
               // Go through all the dialog nodes and add ot edges
               foreach(JToken dialogNode in NodeFile.Json["dialog_tree_info"]["nodes"].Children())
               {
                  JToken choices = dialogNode.First["bulletin"]["choices"];
                  foreach (JToken nodeData in choices.Values())
                  {
                     string parentName = (nodeData.Parent as JProperty).Name;
                     string translatedParentName = ModuleDataManager.GetInstance().LocalizeString(parentName);
                     List<string> outEdges = JsonHelper.GetJsonStringArray(nodeData, "out_edge");
                     foreach(string outEdge in outEdges)
                     {
                        List<string> list;
                        if (!mChoiceEdgeInfo.TryGetValue(outEdge, out list))
                        {
                           list = new List<string>();
                        }
                        list.Add(translatedParentName);
                        mChoiceEdgeInfo[outEdge] = list;
                        if (!mOutEdgeStrings.Contains(outEdge))
                        {
                           mOutEdgeStrings.Add(outEdge);
                        }
                     }
                  }
               }
               break;
            case "counter":
               foreach (JToken nodeData in NodeFile.Json["counter_info"]["out_edges"].Values())
               {
                  mOutEdgeStrings.Add(nodeData.ToString());
               }
               break;
         }
      }

      public override void UpdateGraphNode(Node graphNode)
      {
         switch (mEncounterType)
         {
            case "generator":
               graphNode.Attr.LineWidth = 2;
               graphNode.Attr.Shape = Shape.Circle;
               graphNode.Attr.FillColor = GameMasterNode.kGreen;
               graphNode.Attr.LabelMargin = 6;
               break;
            default:
               base.UpdateGraphNode(graphNode);
               break;
         }
         if (NodeFile.Owner == null)
         {
            graphNode.Attr.Color = Color.Red;
         }
      }
      protected override void UpdateOutEdges(Graph graph)
      {
         GameMasterNode arcFile = NodeFile.Owner;
         if (arcFile != null)
         {
            ArcNodeData arc = arcFile.NodeData as ArcNodeData;
            foreach (string inEdgeName in mOutEdgeStrings)
            {
               List<GameMasterNode> linkedEncounters = arc.GetEncountersWithInEdge(inEdgeName);
               if (linkedEncounters.Count == 1 && linkedEncounters[0].Name.Equals(inEdgeName))
               {
                  if (mChoiceEdgeInfo.ContainsKey(inEdgeName))
                  {
                     foreach (string choice in mChoiceEdgeInfo[inEdgeName])
                     {
                        Node choiceNode = graph.AddNode(NodeFile.Id + "#" + choice);
                        choiceNode.LabelText = '"' + choice + '"';
                        MakeNodePrivate(choiceNode);
                        graph.AddEdge(NodeFile.Id, choiceNode.Id);
                        graph.AddEdge(choiceNode.Id, linkedEncounters[0].Id);
                     }
                  }
                  else
                  {
                     graph.AddEdge(NodeFile.Id, linkedEncounters[0].Id);
                  }
               }
               else
               {
                  Node arcOutNode = graph.AddNode(arcFile.Id + "#" + inEdgeName);
                  arcOutNode.LabelText = inEdgeName;
                  MakeNodePrivate(arcOutNode);
                  if (mChoiceEdgeInfo.ContainsKey(inEdgeName))
                  {
                     foreach (string choice in mChoiceEdgeInfo[inEdgeName])
                     {
                        Node choiceNode = graph.AddNode(NodeFile.Id + "#" + choice);
                        choiceNode.LabelText = '"' + choice + '"';
                        MakeNodePrivate(choiceNode);
                        graph.AddEdge(NodeFile.Id, choiceNode.Id);
                        graph.AddEdge(choiceNode.Id, arcOutNode.Id);
                     }
                  } else {
                     graph.AddEdge(NodeFile.Id, arcOutNode.Id);
                  }

                  foreach (GameMasterNode linkedEncounter in linkedEncounters)
                  {
                     graph.AddEdge(arcOutNode.Id, linkedEncounter.Id);
                  }
               }
            }
         }
      }

      // Returns list of out edges
      private List<string> GetOutEdges()
      {
         List<string> outEdges = new List<string>();
         GameMasterNode arcFile = NodeFile.Owner;
         if (arcFile != null)
         {
            ArcNodeData arc = arcFile.NodeData as ArcNodeData;
            foreach (string inEdgeName in mOutEdgeStrings)
            {
               foreach (GameMasterNode linkedEncounter in arc.GetEncountersWithInEdge(inEdgeName))
               {
                  outEdges.Add(linkedEncounter.Id);
               }
            }
         }
         return outEdges;
      }

      public override NodeData Clone(GameMasterNode nodeFile)
      {
         EncounterNodeData newNodeData = new EncounterNodeData();
         newNodeData.NodeFile = nodeFile;
         newNodeData.mEncounterType = mEncounterType;
         newNodeData.mInEdge = mInEdge;
         newNodeData.mOutEdgeStrings = new List<string>();
         newNodeData.mIsStartNode = mIsStartNode;

         if (NodeFile.Owner != null && NodeFile.Owner.NodeType == GameMasterNodeType.ARC)
         {
            ArcNodeData ownerArcData = NodeFile.Owner.NodeData as ArcNodeData;
            ownerArcData.AddEncounter(newNodeData);
            nodeFile.Owner = NodeFile.Owner;
         }

         return newNodeData;
      }
      public override bool AddOutEdge(GameMasterNode nodeFile)
      {
         if (nodeFile.NodeType != GameMasterNodeType.ENCOUNTER)
         {
            return false;
         }
         EncounterNodeData encounterData = nodeFile.NodeData as EncounterNodeData;
         string inEdge = encounterData.InEdge;

         if (encounterData.IsStartNode)
         {
            // Cannot add start nodes to an encounter. they should be added to arc
            return false;
         }

         List<string> outEdges = GetOutEdges();
         if (outEdges.Contains(nodeFile.Id))
         {
            // This item is already part of the out edges
            return false;
         }
         if (!mOutEdgeStrings.Contains(inEdge))
         {
            // This out edge isn't already in the list of possible out edges, see if we can add it.
            switch (mEncounterType)
            {
               case "generator":
                  // Cannot add more than one edge to generator
                  return false;
               case "random_out_edge":
                  JObject randomOutEdgesDictionary = (NodeFile.Json["random_out_edge_info"]["out_edges"] as JObject);
                  randomOutEdgesDictionary.Add(inEdge, JObject.Parse(@"{""weight"":1 }"));
                  mOutEdgeStrings.Add(inEdge);
                  break;
               case "collection_quest":
                  return false;
               case "dialog_tree":
                  // We can't add to a dialog tree, you have to specify a node.
                  return false;
               case "counter":
                  // Cannot add to a counter because it either does fail or success
                  return false;
               default:
                  NodeFile.Json.Remove("out_edge");
                  mOutEdgeStrings.Add(inEdge);
                  NodeFile.Json.Add("out_edge", JsonConvert.SerializeObject(mOutEdgeStrings));
                  break;
            }
         }

         if (nodeFile.Owner != NodeFile.Owner)
         {
            // make sure encounter is added to this tree
            ArcNodeData ownerArcData = NodeFile.Owner.NodeData as ArcNodeData;
            ownerArcData.AddEncounter(encounterData);
            nodeFile.Owner = NodeFile.Owner;
         }
         NodeFile.IsModified = true;
         return true;
      }
   }

   public class CampPieceNodeData : NodeData
   {
      public override NodeData Clone(GameMasterNode nodeFile)
      {
         throw new NotImplementedException();
      }

      protected override void UpdateOutEdges(Graph graph)
      {
         throw new NotImplementedException();
      }

      public override void LoadData(Dictionary<string, GameMasterNode> allNodes)
      {
      }
   }
}
