using System.Collections.Generic;

namespace StonehearthEditor
{
    public class CloneObjectParameters
    {
        private Dictionary<string, string> mStringReplacements = new Dictionary<string, string>();
        private Dictionary<string, string> mAliasReplacements = new Dictionary<string, string>();
        private string mSourceModuleName;
        private string mTargetModuleName;
        private string mManifestEntryType;

        public string TargetModule
        {
            get { return mTargetModuleName; }
        }

        public string SourceModule
        {
            get { return mSourceModuleName; }
        }

        public string manifestEntryType
        {
            get { return mManifestEntryType; }
        }

        public void SetmanifestEntryType(string name)
        {
            mManifestEntryType = name;
        }

        public void SetTargetModule(string name)
        {
            mTargetModuleName = name;
        }

        public void SetSourceModule(string name)
        {
            mSourceModuleName = name;
        }

        public void AddStringReplacement(string original, string replacement)
        {
            mStringReplacements[original] = replacement;
        }

        public void AddAliasReplacement(string original, string replacement)
        {
            mAliasReplacements[original] = replacement;
        }

        public string TransformModPath(string param)
        {
            if (SourceModule != null && TargetModule != null)
            {
                return param.Replace(SourceModule, TargetModule);
            }

            return param;
        }

        public string TransformParameter(string param)
        {
            string newString = param;
            foreach (KeyValuePair<string, string> replacement in mStringReplacements)
            {
                newString = newString.Replace(replacement.Key, replacement.Value);
            }

            return newString;
        }

        public string TransformAlias(string alias)
        {
            string newString = TransformParameter(alias);
            foreach (KeyValuePair<string, string> replacement in mAliasReplacements)
            {
                newString = newString.Replace(replacement.Key, replacement.Value);
            }

            return newString;
        }

        public bool IsDependency(string dependencyName)
        {
            foreach (string originalName in mStringReplacements.Keys)
            {
                if (dependencyName.Contains(originalName))
                {
                    return true;
                }
            }

            foreach (string originalName in mAliasReplacements.Keys)
            {
                if (dependencyName.Contains(originalName))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
