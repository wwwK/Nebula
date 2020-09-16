namespace Nebula.Core.Data
{
    public interface IDataLoadable
    {
        public bool OnLoad(IDataMember member);
    }
}