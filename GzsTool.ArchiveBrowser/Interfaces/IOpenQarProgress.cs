namespace GzsTool.ArchiveBrowser.Interfaces
{
    public interface IOpenQarProgress
    {
        void Report(double value, double maximum, double progressBase, double progressFactor);
        void Report(string text);
    }
}
