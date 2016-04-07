using System.Collections.Generic;
using System.Windows.Forms;

namespace StonehearthEditor
{
    internal class ImageFileData : FileData
    {
        private string mDirectory;

        public ImageFileData(string path)
            : base(path)
        {
            mDirectory = JsonHelper.NormalizeSystemPath(System.IO.Path.GetDirectoryName(Path));
        }

        public override bool UpdateTreeNode(TreeNode node, string filter)
        {
            return false; // Qubicle files
        }

        public override void Load()
        {
            // do not actually load the binary
            return;
        }

        public void AddLinkingJsonFile(JsonFileData file)
        {
            RelatedFiles.Add(file);
        }

        public override bool Clone(string newPath, CloneObjectParameters parameters, HashSet<string> alreadyCloned, bool execute)
        {
            // Just pure file copy
            if (execute)
            {
                string newDirectory = System.IO.Path.GetDirectoryName(newPath);
                System.IO.Directory.CreateDirectory(newDirectory);
                if (!System.IO.File.Exists(newPath))
                {
                    System.IO.File.Copy(Path, newPath);
                }
            }

            return true;
        }

        protected override void LoadInternal()
        {
            return; // Do nothing
        }
    }
}
