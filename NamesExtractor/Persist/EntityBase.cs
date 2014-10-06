namespace IndexerLib.Persist
{
    public abstract class EntityBase<TExtend> : EntityBase
    {
        public int Id { get; set; }
    }

    public abstract class EntityBase
    {
        public virtual void BeforeSave() { }
    }
}
