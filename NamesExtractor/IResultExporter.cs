namespace IndexerLib
{
    public interface IResultExporter
    {
        void ExportPersons(IBook book);
        void ExportPages(IBook book);
    }
}
