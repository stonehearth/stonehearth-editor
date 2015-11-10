using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StonehearthEditor
{
   public abstract class FileData
   {
      public abstract List<ModuleFile> LinkedAliases { get; }
      public abstract List<FileData> LinkedFiles { get; }

      public abstract string Path { get; }

      public abstract void Load(string jsonString);

      public abstract void UpdateTreeNode(TreeNode node);
   }
}
