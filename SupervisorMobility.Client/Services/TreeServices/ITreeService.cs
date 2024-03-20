using SupervisorMobility.Client.Data.Entities.TreeStruct;
using System.Runtime.CompilerServices;

namespace SupervisorMobility.Client.Services.TreeServices
{
    public interface ITreeService
    {
        Task<AsyncVoidMethodBuilder> InitializeTreeData();
        TreeItemData Make_Tree_CCP(List<FolderCCP> elementos);

        TreeItemData Make_Tree_HOE(List<FolderHOE> elementos);

        TreeItemData Make_Tree_GOS(List<FolderGOS> elementos);

        TreeItemData FindNodeByPath(TreeItemData rootNode, string path);

        bool HasCCP_Data();
        bool HasGOS_Data();
        bool HasHOE_Data();

        Task<SOSCodePath> getCodePath(int id);

        Task<AsyncVoidMethodBuilder> setNodesByPath(SOSCodePath codePath);
        Task<AsyncVoidMethodBuilder> GetFilesInNodeCCP(TreeItemData node);
        Task<AsyncVoidMethodBuilder> GetFilesInNodeGOS(TreeItemData node);
        Task<AsyncVoidMethodBuilder> GetFilesInNodeHOE(TreeItemData node);

        Task<TreeItemData> getRootCCP();
        Task<TreeItemData> getRootGOS();
        Task<TreeItemData> getRootHOE();
        Task<TreeItemData> getNodeCCP();
        Task<TreeItemData> getNodeGOS();
        Task<TreeItemData> getNodeHOE();
        Task<TreeItemData> getNodeCCP_CD();
        Task<TreeItemData> getNodeGOS_CD();
        Task<TreeItemData> getNodeHOE_CD();
    }
}
