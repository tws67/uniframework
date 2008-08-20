namespace Uniframework.Services.db4oProviders
{
    public abstract class DataContainer
    {
        public override bool Equals(object obj)
        {
            return this.ToString().Equals(obj.ToString());
        }

        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }
    }
}