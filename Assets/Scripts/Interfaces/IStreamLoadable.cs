using System.IO;

public interface IStreamLoadable
{
    void LoadFromStream(Stream stream);
}