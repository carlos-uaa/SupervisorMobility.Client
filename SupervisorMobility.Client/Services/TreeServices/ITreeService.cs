using SupervisorMobility.Client.Data.Entities.TreeStruct;

namespace SupervisorMobility.Client.Services.TreeServices
{
    public interface ITreeService
    {
        public TreeItemData ConstruirArbolCCP(List<FolderCCP> elementos);

        public TreeItemData ConstruirArbolHOE(List<FolderHOE> elementos);

        public TreeItemData ConstruirArbolGOS(List<FolderGOS> elementos);

        public TreeItemData FindNodeByPath(TreeItemData rootNode, string path);
    }
}
