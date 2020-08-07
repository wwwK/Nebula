namespace Nebula.Core.Data
{
    public interface IDataFile
    {
        void OnSave();
        void OnLoad();
    }
}