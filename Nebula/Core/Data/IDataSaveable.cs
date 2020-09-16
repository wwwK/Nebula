namespace Nebula.Core.Data
{
    public interface IDataSaveable
    {
        public bool OnSave(IDataMember member);
    }
}