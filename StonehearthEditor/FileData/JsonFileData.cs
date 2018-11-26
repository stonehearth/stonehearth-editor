using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.IO;

namespace StonehearthEditor
{
    public enum JSONTYPE
    {
        ERROR = 0,
        NONE = 1,
        ENTITY = 2,
        BUFF = 3,
        AI_PACK = 4,
        EFFECT = 5,
        RECIPE = 6,
        COMMAND = 7,
        ANIMATION = 8,
        ENCOUNTER = 9,
        JOB = 10,
        MONSTER_TUNING = 11,
        MIXIN = 12,
    }

    public class JsonFileData : FileData, IModuleFileData
    {
        public int RecommendedMaxNetWorth { get; set; } = -1;

        public int RecommendedMinNetWorth { get; set; } = -1;

        private ModuleFile mOwner;
        private JSONTYPE mJsonType = JSONTYPE.NONE;
        private JObject mJson;
        private string mDirectory;
        private bool mSaveJsonAfterParse = false;
        private int mNetWorth = -1;

        public string Category = null;

        public JsonFileData(string path)
            : base(path)
        {
            mDirectory = JsonHelper.NormalizeSystemPath(System.IO.Path.GetDirectoryName(Path));
        }

        public void AddMixin(string mixin)
        {
            JToken mixins = mJson.SelectToken("mixins");
            if (mixins == null)
            {
                mJson.AddFirst(new JProperty("mixins", mixin));
                mSaveJsonAfterParse = true;
                AddError("mixin added");
            }
            else
            {
                JArray mixinsArray = mixins as JArray;
                if (mixinsArray == null)
                {
                    string existingMixin = mixins.ToString();
                    if (existingMixin != mixin)
                    {
                        mixinsArray = new JArray();
                        mJson["mixins"] = mixinsArray;
                        mixinsArray.Add(mixins.ToString());
                        mSaveJsonAfterParse = true;
                        AddError("mixin added");
                    }
                }

                if (mixinsArray != null)
                {
                    HashSet<string> allMixins = new HashSet<string>();
                    foreach (JToken property in mixinsArray.Children())
                    {
                        allMixins.Add(property.ToString());
                    }

                    if (!allMixins.Contains(mixin))
                    {
                        mSaveJsonAfterParse = true;
                        AddError("mixin added");
                    }

                    allMixins.Add(mixin);
                    mixinsArray.Clear();
                    foreach (string hashsetMixin in allMixins)
                    {
                        mixinsArray.Add(hashsetMixin);
                    }
                }
            }
        }

        public string GetJsonFileString()
        {
            string jsonString = JsonHelper.GetFormattedJsonString(mJson);
            if (jsonString == null)
            {
                Console.WriteLine("Could not convert {0} to json string", Path);
            }

            return jsonString != null ? jsonString : "INVALID JSON";
        }

        public override void AddError(string error)
        {
            base.AddError(error);
            switch (mJsonType)
            {
                case JSONTYPE.ENCOUNTER:
                    return;
            }

            ModuleDataManager.GetInstance().AddErrorFile(this);
        }

        public override bool HasErrors
        {
            get
            {
                if (base.HasErrors)
                {
                    return true;
                }

                foreach (FileData file in OpenedFiles)
                {
                    if (file.Errors != null)
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        // Returns true if should show parent node
        public override bool UpdateTreeNode(TreeNode node, string filter)
        {
            base.UpdateTreeNode(node, filter);
            mTreeNode = node;
            node.Tag = this;

            if (!HasErrors)
            {
                node.SelectedImageIndex = (int)JsonType;
                node.ImageIndex = (int)JsonType;
            }

            bool filterMatchesSelf = true;
            ModuleFile owner = GetModuleFile();
            if (!string.IsNullOrEmpty(filter) && owner != null && !owner.Alias.Contains(filter))
            {
                filterMatchesSelf = false;
            }

            bool hasChildMatchingFilter = false;
            if (JsonType == JSONTYPE.JOB)
            {
                if (OpenedFiles.Count > 1)
                {
                    FileData recipeJsonData = OpenedFiles[1];
                    TreeNode recipes = new TreeNode(recipeJsonData.FileName);
                    recipeJsonData.UpdateTreeNode(recipes, filter);

                    IOrderedEnumerable<KeyValuePair<string, FileData>> sortedRecipes = recipeJsonData.LinkedFileData.OrderBy(recipe => recipe.Key);
                    foreach (KeyValuePair<string, FileData> recipe in sortedRecipes)
                    {
                        string recipePath = recipe.Key;
                        string recipeName = System.IO.Path.GetFileNameWithoutExtension(recipePath);
                        if (string.IsNullOrEmpty(filter) || recipeName.Contains(filter) || filterMatchesSelf)
                        {
                            TreeNode recipeNode = new TreeNode(recipeName);
                            recipe.Value.UpdateTreeNode(recipeNode, filter);
                            recipes.Nodes.Add(recipeNode);
                            hasChildMatchingFilter = true;
                        }
                    }

                    node.Nodes.Add(recipes);
                }
            }

            if (!hasChildMatchingFilter && !filterMatchesSelf)
            {
                if (!filter.Contains("error") || !HasErrors)
                {
                    return false;
                }
            }

            return true;
        }

        public override bool Clone(string newPath, CloneObjectParameters parameters, HashSet<string> alreadyCloned, bool execute)
        {
            if (JsonType == JSONTYPE.RECIPE)
            {
                string newNameToUse = parameters.TransformParameter(GetNameForCloning()).Replace("_recipe", "");
                if (execute)
                {
                    JsonFileData recipesList = RelatedFiles[RelatedFiles.Count - 1] as JsonFileData;
                    // Only add the recipe to the recipe index if we're inside the same module
                    if (recipesList.Path.Contains(parameters.TargetModule))
                    {
                        JObject json = recipesList.mJson;
                        JToken foundParent = null;
                        foreach (JToken token in json["craftable_recipes"].Children())
                        {
                            if (foundParent != null)
                            {
                                break;
                            }

                            foreach (JToken recipe in token.First["recipes"].Children())
                            {
                                if (recipe.Last.ToString().Contains(FileName))
                                {
                                    foundParent = token.First["recipes"];
                                    break;
                                }
                            }
                        }

                        if (foundParent != null)
                        {
                            string recipeFileName = System.IO.Path.GetFileName(newPath);
                            (foundParent as JObject).Add(newNameToUse, JObject.Parse("{\"recipe\": \"file(" + recipeFileName + ")\"}"));
                            recipesList.TrySetFlatFileData(recipesList.GetJsonFileString());
                            recipesList.TrySaveFile();
                        }
                    }
                }
            }

            return base.Clone(newPath, parameters, alreadyCloned, execute);
        }

        public override bool ShouldCloneDependency(string dependencyName, CloneObjectParameters parameters)
        {
            if (JsonType == JSONTYPE.RECIPE)
            {
                JToken produces = mJson["produces"];
                if (produces != null)
                {
                    foreach (JToken child in produces.Children())
                    {
                        if (child["item"] != null && child["item"].ToString().Equals(dependencyName))
                        {
                            return true;
                        }
                    }
                }
            }

            return base.ShouldCloneDependency(dependencyName, parameters);
        }

        public override string GetNameForCloning()
        {
            string fileName = FileName;
            switch (JsonType)
            {
                case JSONTYPE.RECIPE:
                    fileName = fileName.Replace("_recipe", "");
                    break;
                case JSONTYPE.JOB:
                    fileName = fileName.Replace("_description", "");
                    break;
            }

            return fileName;
        }

        // TODO: make this less brittle. it just takes the first linked image file it references and uses it as the image
        public string GetImageForFile()
        {
            foreach (FileData openedFile in OpenedFiles)
            {
                foreach (KeyValuePair<string, FileData> linkedFile in openedFile.LinkedFileData)
                {
                    if ((linkedFile.Value is ImageFileData) && System.IO.File.Exists(linkedFile.Value.Path))
                    {
                        return linkedFile.Value.Path;
                    }
                }
            }

            return string.Empty;
        }

        public void SetModuleFile(ModuleFile moduleFile)
        {
            mOwner = moduleFile;
        }

        public ModuleFile GetModuleFile()
        {
            return mOwner;
        }

        public JSONTYPE JsonType
        {
            get { return mJsonType; }
        }

        public string Directory
        {
            get { return mDirectory; }
        }

        public JObject Json
        {
            get { return mJson; }
        }

        public int NetWorth
        {
            get { return mNetWorth; }
        }

        protected override void LoadInternal()
        {
            try
            {
                OpenedFiles.Add(this);
                string jsonString = FlatFileData;
                mJson = JObject.Parse(jsonString);
                JToken typeObject = mJson["type"];
                if (typeObject != null)
                {
                    string typeString = typeObject.ToString().Trim().ToUpper();
                    foreach (JSONTYPE jsonType in Enum.GetValues(typeof(JSONTYPE)))
                    {
                        if (typeString.Equals(jsonType.ToString()))
                        {
                            mJsonType = jsonType;
                        }
                    }
                }

                ParseLinkedAliases(jsonString);
                ParseLinkedFiles(jsonString);
                ParseJsonSpecificData();
            }
            catch (Exception e)
            {
                AddError("Failed to load json file " + Path + ". Error: " + e.Message);
            }
        }

        protected override void PostLoad()
        {
            if (mSaveJsonAfterParse)
            {
                TrySetFlatFileData(GetJsonFileString());
                //TrySaveFile();
                mSaveJsonAfterParse = false;
            }
        }

        protected override bool TryChangeFlatFileData(string newData, out string newFlatFileData)
        {
            try
            {
                JObject json = JObject.Parse(newData);
                newFlatFileData = JsonHelper.GetFormattedJsonString(json);
                mJson = json;
            }
            catch (Exception e)
            {
                MessageBox.Show("Unable to change flat file data. Invalid Json: " + e.Message);
                newFlatFileData = null;
                return false;
            }

            return true;
        }

        private void CheckForNoNetWorth()
        {
            // check for errors
            JToken netWorthData = mJson.SelectToken("entity_data.stonehearth:net_worth");
            if (netWorthData == null)
            {
                if (!Directory.Contains("mixins") && mJson.SelectToken("mixins") == null)
                {
                    AddError("No net worth even though object is an item! Add an entity_data.stonehearth:net_worth!");
                    JObject netWorth = JObject.Parse(StonehearthEditor.Properties.Resources.defaultNetWorth);
                    JObject entityData = mJson["entity_data"] as JObject;
                    if (entityData == null)
                    {
                        entityData = new JObject();
                        mJson["entity_data"] = entityData;
                    }

                    entityData.Add("stonehearth:net_worth", netWorth);
                    mSaveJsonAfterParse = true;
                }
            }
            else
            {
                // mNetWorth
                string netWorthString = netWorthData["value_in_gold"].ToString();
                if (!string.IsNullOrEmpty(netWorthString))
                {
                    mNetWorth = int.Parse(netWorthString);
                    /*
                    if (!mSaveJsonAfterParse)
                    {
                        JToken materialToken = mJson.SelectToken("components.stonehearth:material");
                        if (materialToken != null && !materialToken["tags"].ToString().Contains("food"))
                        {
                            int newNetWorth = mNetWorth / 2;
                            if (newNetWorth <= 0 && mNetWorth > 0)
                            {
                                newNetWorth = mNetWorth;
                            }
                            mNetWorth = newNetWorth;
                            netWorthData["value_in_gold"] = mNetWorth;
                            ModuleDataManager.GetInstance().ModifiedFiles.Add(this);
                            mSaveJsonAfterParse = true;
                        }
                    }*/
                }
            }
        }

        private void ParseJsonSpecificData()
        {
            string directory = Directory;
            string categoryOverride = null;
            switch (mJsonType)
            {
                case JSONTYPE.ENTITY:
                    JToken entityFormsComponent = mJson.SelectToken("components.stonehearth:entity_forms");
                    if (entityFormsComponent != null)
                    {
                        JToken iconicForm = entityFormsComponent["iconic_form"];
                        if (iconicForm != null)
                        {
                            string iconicFilePath = JsonHelper.GetFileFromFileJson(iconicForm.ToString(), directory);
                            iconicFilePath = JsonHelper.NormalizeSystemPath(iconicFilePath);
                            JsonFileData iconic = new JsonFileData(iconicFilePath);
                            iconic.Load();
                            categoryOverride = iconic.Category;
                            Category = iconic.Category;
                            OpenedFiles.Add(iconic);
                        }

                        // Look for stonehearth:entity_forms
                        JToken ghostForm = entityFormsComponent["ghost_form"];
                        if (ghostForm != null)
                        {
                            string ghostFilePath = JsonHelper.GetFileFromFileJson(ghostForm.ToString(), directory);
                            ghostFilePath = JsonHelper.NormalizeSystemPath(ghostFilePath);
                            JsonFileData ghost = new JsonFileData(ghostFilePath);
                            ghost.Category = categoryOverride;
                            ghost.Load();
                            ghost.AddMixin("stonehearth:mixins:placed_object");
                            OpenedFiles.Add(ghost);
                            ghost.PostLoad();
                        }

                        CheckForNoNetWorth();
                    }
                    else
                    {
                        // Make sure to check if items have no net worth or set mNetWorth from their net worth data
                        JToken catalog_data = mJson.SelectToken("entity_data.stonehearth:catalog.is_item");
                        if (catalog_data != null && catalog_data.Value<bool>())
                        {
                            CheckForNoNetWorth();
                        }

                        if (GetModuleFile() != null && mJson.SelectToken("components.item") != null)
                        {
                            CheckForNoNetWorth();
                        }

                        if (mJson.SelectToken("entity_data.stonehearth:net_worth") != null) {
                            CheckForNoNetWorth();
                        }
                    }

                    break;
                case JSONTYPE.JOB:
                    // Parse crafter stuff
                    JToken crafter = mJson["crafter"];
                    if (crafter != null)
                    {
                        // This is a crafter, load its recipes
                        string recipeListLocation = crafter["recipe_list"].ToString();
                        recipeListLocation = JsonHelper.GetFileFromFileJson(recipeListLocation, directory);
                        if (recipeListLocation != null)
                        {
                            JsonFileData recipes = new JsonFileData(recipeListLocation);
                            recipes.Load();
                            foreach (FileData recipe in recipes.LinkedFileData.Values)
                            {
                                recipes.RelatedFiles.Add(recipe);
                            }

                            OpenedFiles.Add(recipes);
                            recipes.ReferencedByFileData[GetAliasOrFlatName()] = this;
                        }
                    }

                    JObject jobEquipment = mJson["equipment"] as JObject;
                    if (jobEquipment != null)
                    {
                        foreach (JToken equippedItem in jobEquipment.Children())
                        {
                            string equipmentJson = equippedItem.Last.ToString();
                            FileData fileData = null;
                            if (equipmentJson.IndexOf(':') >= 0)
                            {
                                foreach (ModuleFile alias in LinkedAliases)
                                {
                                    if (alias.FullAlias.Equals(equipmentJson))
                                    {
                                        fileData = alias.FileData;
                                    }
                                }
                            }
                            else
                            {
                                equipmentJson = JsonHelper.GetFileFromFileJson(equipmentJson, directory);
                                LinkedFileData.TryGetValue(equipmentJson, out fileData);
                            }

                            JsonFileData jsonFileData = fileData as JsonFileData;
                            if (jsonFileData != null)
                            {
                                JToken equipment_piece = jsonFileData.mJson.SelectToken("components.stonehearth:equipment_piece");
                                if (equipment_piece != null)
                                {
                                    JToken ilevel = equipment_piece["ilevel"];
                                    if (ilevel != null && ilevel.Value<int>() > 0)
                                    {
                                        JToken noDrop = equipment_piece["no_drop"];
                                        if (noDrop == null || !noDrop.Value<bool>())
                                        {
                                            AddError("Equipment piece " + equipmentJson + " is not set to no_drop=true even through it is a job equipment!");
                                        }
                                    }
                                }
                            }
                        }
                    }

                    ////AlterJobExperience();
                    break;
                case JSONTYPE.RECIPE:
                    JToken portrait = mJson["portrait"];
                    if (portrait != null)
                    {
                        string portraitImageLocation = portrait.ToString();
                        portraitImageLocation = JsonHelper.GetFileFromFileJson(portraitImageLocation, directory);
                        ImageFileData image = new ImageFileData(portraitImageLocation);
                        LinkedFileData.Add(portraitImageLocation, image);
                    }

                    break;
                case JSONTYPE.AI_PACK:
                    JArray actions = mJson["actions"] as JArray;
                    if (actions != null)
                    {
                        foreach (JToken action in actions)
                        {
                            string fullAlias = action.ToString();
                            ModuleFile linkedAlias = ModuleDataManager.GetInstance().GetModuleFile(fullAlias);
                            if (linkedAlias == null)
                            {
                                AddError("links to alias " + fullAlias + " which does not exist!");
                            }
                        }
                    }
                    break;
                case JSONTYPE.MONSTER_TUNING:
                    JToken experience = mJson.SelectToken("attributes.exp_reward");
                    if (experience != null)
                    {
                        //Console.WriteLine(GetAliasOrFlatName() + "\t" + experience.ToString());
                    }
                    break;
            }

            FixUnitInfo();
            CheckDisplayName();
            FixUpAllCombatData();
        }

        private void FixUpAllCombatData()
        {
            FixupCombatData("stonehearth:combat:melee_attacks");
            FixupCombatData("stonehearth:combat:melee_defenses");
            FixupCombatData("stonehearth:combat:ranged_attacks");
        }

        private static Dictionary<string, int> kCombatTuning = new Dictionary<string, int>();

        private void FixupCombatData(string dataName)
        {
            JArray actions = mJson.SelectToken("entity_data." + dataName) as JArray;
            if (actions != null)
            {
                bool hasError = false;
                foreach (JToken actionToken in actions.Children())
                {
                    JObject action = actionToken as JObject;
                    string name = action["name"].ToString();
                    if (action["effect"] == null)
                    {
                        action.Remove("name");
                        action.AddFirst(new JProperty("effect", name));
                        action.AddFirst(new JProperty("name", name));
                        hasError = true;
                    }

                    int cooldown = 0;
                    JToken cooldownToken = action["cooldown"];
                    if (cooldownToken != null)
                    {
                        cooldown = cooldownToken.Value<int>();
                    }

                    if (kCombatTuning.ContainsKey(name))
                    {
                        int existingCooldown = kCombatTuning[name];
                        if (existingCooldown != cooldown)
                        {
                            AddError("Action has a cooldown in a different file that is different");
                        }
                    }
                    else
                    {
                        kCombatTuning[name] = cooldown;
                    }
                }

                if (hasError)
                {
                    AddError("Action name and effect need to be separated");
                    mSaveJsonAfterParse = true;
                }
            }
        }

        private bool mHasAlteredExperience = false;

        private void AlterJobExperience()
        {
            JObject xpRewards = mJson.SelectToken("xp_rewards") as JObject;
            if (xpRewards != null && !mHasAlteredExperience)
            {
                foreach (JToken xp in xpRewards.Children())
                {
                    JProperty property = xp as JProperty;
                    JValue xpValue = property.Value as JValue;
                    int value = xpValue.Value<int>();
                    int newValue = value - 5;
                    xpValue.Value = newValue;
                }

                AddError("Needs Save after job experience altered");
                mSaveJsonAfterParse = true;
                mHasAlteredExperience = true;
            }
        }


        private void CheckDisplayName()
        {
            CheckStringKeyExists(mJson.SelectToken("entity_data.stonehearth:catalog.display_name"), "entity_data.stonehearth:catalog.display_name");
            CheckStringKeyExists(mJson.SelectToken("entity_data.stonehearth:catalog.description"), "entity_data.stonehearth:catalog.description");

            // Check for display name and description in recipes
            switch (mJsonType)
            {
                case JSONTYPE.RECIPE:
                    CheckStringKeyExists(mJson.SelectToken("recipe_name"), "recipe_name");
                    CheckStringKeyExists(mJson.SelectToken("description"), "description");
                    break;
                case JSONTYPE.BUFF:
                    JToken invisibleToPlayer = mJson.SelectToken("invisible_to_player");
                    bool isInvisible = invisibleToPlayer != null ? invisibleToPlayer.Value<bool>() : false;
                    if (!isInvisible)
                    {
                        CheckStringKeyExists(mJson.SelectToken("display_name"), "display_name");
                        CheckStringKeyExists(mJson.SelectToken("description"), "description");
                    }

                    break;
                case JSONTYPE.COMMAND:
                    CheckStringKeyExists(mJson.SelectToken("display_name"), "display_name");
                    CheckStringKeyExists(mJson.SelectToken("description"), "description");
                    break;
            }
        }

        private void FixUnitInfo()
        {
            // make sure display name appears before the description field
            if (!mSaveJsonAfterParse)
            {
                JObject unitInfo = mJson.SelectToken("components.unit_info") as JObject;
                JObject entityData = mJson.SelectToken("entity_data") as JObject;

                if (unitInfo != null)
                {
                    if (entityData == null)
                    {
                        entityData = new JObject();
                        mJson.Add("entity_data", entityData);
                    }
                    mSaveJsonAfterParse = true;
                    (mJson["components"] as JObject).Remove("unit_info");

                    if (Category != null)
                    {
                        unitInfo.Add("category", Category);
                    }

                    if (unitInfo["name"] != null)
                    {
                        AddError("unit Info has name instead of display_name!!");
                        unitInfo.Add("display_name", unitInfo.GetValue("name").ToString());
                        unitInfo.Remove("name");
                    }

                    entityData.Add("stonehearth:catalog", unitInfo);
                }

                JObject item = mJson.SelectToken("components.item") as JObject;
                if (item != null)
                {
                    (mJson["components"] as JObject).Remove("item");

                    if (entityData == null)
                    {
                        entityData = new JObject();
                        mJson.Add("entity_data", entityData);
                    }

                    unitInfo = entityData["stonehearth:catalog"] as JObject;
                    if (unitInfo == null)
                    {
                        unitInfo = new JObject();
                        entityData.Add("stonehearth:catalog", unitInfo);
                    }

                    unitInfo.Add("is_item", true);

                    if (item.GetValue("category") != null && Category == null)
                    {
                        Category = item.GetValue("category").ToString();
                        unitInfo.Add("category", Category);
                    }

                    mSaveJsonAfterParse = true;
                }
            }

        }

        private void CheckStringKeyExists(JToken token, string tokenName)
        {
            if (token == null)
            {
                ////AddError("The string for " + tokenName + " does not exist! Please add localization support");
                return;
            }

            string key = token.ToString();
            Regex matcher = new Regex(@"i18n\(([^)]+)\)");
            Match locMatch = matcher.Match(key);
            if (locMatch.Success)
            {
                key = locMatch.Groups[1].Value;
                if (!ModuleDataManager.GetInstance().HasLocalizationKey(key))
                {
                    AddError("Localization Key Not Found! " + key + " is not in en.json");
                }
            }
            else
            {
                if (key.Length > 0)
                {
                    AddError("The string for " + tokenName + " is '" + key + "', which is not a valid localization key. Please add localization support");
                }
            }
        }

        private void ParseLinkedFiles(string jsonString)
        {
            string directory = Directory;
            Regex matcher = new Regex("file\\([\\S]+\\)");
            foreach (Match match in matcher.Matches(jsonString))
            {
                string matchValue = match.Value;

                // Sigh, special case these because they're more like folders instead of files
                if (matchValue != "file(animations)" && matchValue != "file(effects)")
                {
                    string linkedFile = JsonHelper.GetFileFromFileJson(match.Value, directory);
                    linkedFile = JsonHelper.NormalizeSystemPath(linkedFile);

                    if (!System.IO.File.Exists(linkedFile) && !System.IO.Directory.Exists(linkedFile))
                    {
                        AddError("Links to non-existent file " + linkedFile);
                        continue;
                    }

                    if (LinkedFileData.ContainsKey(linkedFile))
                    {
                        continue;
                    }

                    FileData linkedFileData = GetFileDataFactory(linkedFile);
                    if (linkedFileData != null)
                    {
                        LinkedFileData.Add(linkedFile, linkedFileData);
                        linkedFileData.ReferencedByFileData[GetAliasOrFlatName()] = this;
                    }
                }
            }
        }

        private FileData GetFileDataFactory(string path)
        {
            string extension = System.IO.Path.GetExtension(path);
            switch (extension)
            {
                case ".qb":
                    QubicleFileData qubicleFile = new QubicleFileData(path);
                    qubicleFile.AddLinkingJsonFile(this);
                    qubicleFile.RelatedFiles.Add(this);
                    qubicleFile.Load();
                    return qubicleFile;
                case ".png":
                    ImageFileData imageFile = new ImageFileData(path);
                    imageFile.AddLinkingJsonFile(this);
                    imageFile.RelatedFiles.Add(this);
                    return imageFile;
                case ".json":
                    JsonFileData jsonFileData = new JsonFileData(path);
                    jsonFileData.Load();
                    jsonFileData.RelatedFiles.Add(this);
                    return jsonFileData;
            }

            return null;
        }

        private string GetAliasOrFlatName()
        {
            ModuleFile moduleFile = GetModuleFile();
            string flatPath = ModuleDataManager.GetInstance().TryReplaceModsDirectory(Path, "", "");
            flatPath = ModuleDataManager.GetInstance().TryReplaceSteamUploadsDirectory(flatPath, "", "");
            return (moduleFile != null) ? moduleFile.FullAlias : flatPath;
        }

        private void ParseLinkedAliases(string jsonString)
        {
            Regex matcher = new Regex("\"([A-z|_|-]+\\:[\\S]*)\"");
            foreach (Match match in matcher.Matches(jsonString))
            {
                string fullAlias = match.Groups[1].Value;
                ModuleFile linkedAlias = ModuleDataManager.GetInstance().GetModuleFile(fullAlias);
                if (linkedAlias == null)
                {
                    continue;
                }

                LinkedAliases.Add(linkedAlias);
                linkedAlias.AddReference(GetAliasOrFlatName(), this);
            }
        }

        internal JsonFileData CreateFileWithMixinsApplied()
        {
            Dictionary<string, string> unusedPropertySourceByPath;
            return CreateFileWithMixinsApplied(out unusedPropertySourceByPath);
        }

        public JsonFileData CreateFileWithMixinsApplied(out Dictionary<string, string> propertySourceByPath)
        {
            propertySourceByPath = new Dictionary<string, string>();

            string newFilePath = System.IO.Path.GetTempPath() + Guid.NewGuid().ToString() + ".json";
            using (StreamWriter writer = new StreamWriter(newFilePath, false, System.Text.Encoding.UTF8))
            {
                writer.Write(ParseNodeMixin(Json, Directory, propertySourceByPath).ToString());
            }

            JsonFileData result = new JsonFileData(newFilePath);
            result.Load();
            return result;
        }

        // WARNING: Keep in sync with ResourceManager2::ParseNodeMixin() in the SH codebase!
        private static JObject ParseNodeMixin(JObject json, string contextFolder, Dictionary<string, string> propertySourceByPath)
        {
            var mixins = json.SelectToken("mixins");
            List<string> mixinsArray;
            if (mixins != null && mixins.Type == JTokenType.String)
            {
                mixinsArray = new List<string> { mixins.ToString() };
            }
            else
            {
                mixinsArray = (mixins as JArray)?.Select(t => t.ToString()).ToList();
            }

            if (mixinsArray != null && mixinsArray.Count > 0)
            {
                // This iteration order is probably not what the user wants, but it matches ResourceManager2::ParseNodeMixin().
                // TODO: Find out the reason behind this, and document it.
                foreach (var mixin in mixinsArray)
                {
                    json = ApplyMixin(json, mixin, contextFolder, propertySourceByPath);
                }
            }

            return json;
        }

        // WARNING: Keep in sync with ResourceManager2::ApplyMixin() in the SH codebase!
        private static JObject ApplyMixin(JObject primaryJson, string mixinJsonPathOrAlias, string contextFolder, Dictionary<string, string> propertySourceByPath)
        {
            ModuleFile moduleFile;
            var foundMixin = ModuleDataManager.GetInstance().TryGetModuleFile(mixinJsonPathOrAlias, contextFolder, out moduleFile);
            if (!foundMixin)
            {
                throw new Exception("Could not find mixin: " + mixinJsonPathOrAlias);
            }

            moduleFile.TryLoad();
            JsonFileData mixinJsonFile = moduleFile.FileData as JsonFileData;
            var mixinJson = ParseNodeMixin(mixinJsonFile.Json, mixinJsonFile.Directory, propertySourceByPath).DeepClone();
            primaryJson = primaryJson.DeepClone() as JObject;
            (primaryJson as JObject)?.Remove("mixins");
            return ExtendNode(mixinJson, primaryJson, mixinJsonFile.Path, propertySourceByPath) as JObject;
        }

        // WARNING: Keep in sync with ResourceManager2::ExtendNode() in the SH codebase!
        private static JToken ExtendNode(JToken baseJson, JToken overrideJson, string baseSourceName, Dictionary<string, string> propertySourceByPath)
        {
            switch (baseJson.Type)
            {
                case JTokenType.Object:
                    if (overrideJson.Type != JTokenType.Object)
                    {
                        throw new Exception("Trying to mixin a non-object into an object.");
                    }

                    var overrideObject = overrideJson as JObject;
                    if (overrideObject.Property("mixintypes") != null)
                    {
                        throw new Exception("Mixin preview does not support 'mixintypes'.");
                    }

                    foreach (var item in baseJson as JObject)
                    {
                        var name = item.Key;
                        var current = overrideObject.Property(name);
                        if (current != null)
                        {
                            current.Value = ExtendNode(item.Value, current.Value, baseSourceName, propertySourceByPath);
                        }
                        else
                        {
                            if (!propertySourceByPath.ContainsKey(item.Value.Path))
                            {
                                propertySourceByPath[item.Value.Path] = baseSourceName;
                            }

                            overrideObject.Add(name, item.Value);
                        }
                    }

                    return overrideJson;
                case JTokenType.Array:
                    foreach (var item in baseJson as JArray)
                    {
                        if (!propertySourceByPath.ContainsKey(item.Path))
                        {
                            propertySourceByPath[item.Path] = baseSourceName;
                        }
                    }

                    foreach (var item in overrideJson as JArray)
                    {
                        (baseJson as JArray).Add(item);
                    }

                    return baseJson;
                default:
                    if (propertySourceByPath.ContainsKey(overrideJson.Path))
                    {
                        propertySourceByPath.Remove(overrideJson.Path);
                    }

                    return overrideJson;
            }
        }
    }
}
