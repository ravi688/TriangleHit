
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;

public static class ExtensionMethods
{
    public static void SaveTo<T>(this T[] array, string file_path)
    {
        using (FileStream file_stream = File.OpenWrite(file_path))
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            binaryFormatter.Serialize(file_stream, array);
            file_stream.Close();
        }
    }
    public static void LoadFrom<T>(this T[] array, out T[] out_array, string file_path)
    {
        using (FileStream file_stream = File.OpenRead(file_path))
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            out_array = (T[])binaryFormatter.Deserialize(file_stream);
            file_stream.Close();
        }
    }

    public static void LoadFrom<T>(this List<T> list, out List<T> out_list, string file_path)
    {
        using (FileStream file_stream = File.OpenRead(file_path))
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            out_list = (List<T>)binaryFormatter.Deserialize(file_stream);
            file_stream.Close();
        }
    }
    public static void SaveTo<T>(this List<T> list, string file_path)
    {
        using (FileStream file_stream = File.OpenWrite(file_path))
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            binaryFormatter.Serialize(file_stream, list);
            file_stream.Close();
        }
    }
}