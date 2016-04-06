using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace StonehearthEditor
{
    internal class LuaFileData : FileData, IModuleFileData
    {
        private string mDirectory;
        private ModuleFile mOwner;

        public LuaFileData(string path)
            : base(path)
        {
            mDirectory = JsonHelper.NormalizeSystemPath(System.IO.Path.GetDirectoryName(Path));
        }

        public override void AddError(string error)
        {
            base.AddError(error);
            Console.WriteLine(error);
            ModuleDataManager.GetInstance().AddErrorFile(this);
        }

        public override bool UpdateTreeNode(TreeNode node, string filter)
        {
            base.UpdateTreeNode(node, filter);
            mTreeNode = node;
            node.Tag = this;
            bool filterMatchesSelf = true;
            ModuleFile owner = GetModuleFile();
            if (!string.IsNullOrEmpty(filter) && owner != null && !owner.Name.Contains(filter))
            {
                filterMatchesSelf = false;
            }
            if (!HasErrors)
            {
                node.SelectedImageIndex = 1;
                node.ImageIndex = 1;
            }
            if (!filterMatchesSelf)
            {
                if (!filter.Contains("error") || !HasErrors)
                {
                    return false;
                }
            }
            return true;
        }

        protected override void LoadInternal()
        {
            return; // Do nothing
        }

        public override bool Clone(string newPath, CloneObjectParameters parameters, HashSet<string> alreadyCloned, bool execute)
        {
            return base.Clone(newPath, parameters, alreadyCloned, execute);
        }

        public void SetModuleFile(ModuleFile moduleFile)
        {
            mOwner = moduleFile;
        }

        public ModuleFile GetModuleFile()
        {
            return mOwner;
        }
    }
}
