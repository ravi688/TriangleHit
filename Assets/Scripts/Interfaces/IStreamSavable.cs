using System.IO;

public interface IStreamSavable
{
    void SaveToStream(Stream stream);
}