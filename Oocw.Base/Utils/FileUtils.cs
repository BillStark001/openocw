using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oocw.Base;

public static class FileUtils
{
    public static readonly Encoding DefaultEncoding = Encoding.UTF8;

    public static readonly string BASE_DIR = AppDomain.CurrentDomain.BaseDirectory;

    public const string CACHE_SUFFIX = ".cache";

    public const string BACKUP_SUFFIX = ".bak";


    public const string ILLEGAL_CHARS = "%\\/:*?\"<>|";
    public static readonly ImmutableList<string> IllegalChars = ILLEGAL_CHARS.Select(x => x.ToString()).ToImmutableList();
    public static readonly ImmutableList<string> LegalChars = ILLEGAL_CHARS
        .Select(x => (IllegalChars[0] + ((int)x).ToString("X2"))).ToImmutableList();


    /// <summary>
    /// 
    /// </summary>
    /// <param name="directory"></param>
    /// <returns>true if ensured, false if not</returns>
    public static bool EnsureDirectory(string directory)
    {
        if (Directory.Exists(directory))
            return true;
        if (File.Exists(directory))
            return false;
        try
        {
            Directory.CreateDirectory(directory);
            return true;
        }
        catch
        {
            return false;
        }
    }


    public static bool EnsureFile(string path, Encoding? encoding = null)
    {
        if (File.Exists(path))
            return true;
        if (Directory.Exists(path))
            return false;
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(path) ?? "");
            File.WriteAllLines(path, new string[] { }, encoding: encoding ?? DefaultEncoding);
            return true;
        }
        catch
        {
            return false;
        }
    }


    public static string LegalizeFileName(string ustr)
    {
        foreach (var i in Enumerable.Range(0, ILLEGAL_CHARS.Length))
        {
            ustr = ustr.Replace(IllegalChars[i], LegalChars[i]);
        }
        return ustr;
    }

    public static string RestoreFileName(string ustr)
    {
        foreach (var i in Enumerable.Range(1, ILLEGAL_CHARS.Length))
        {
            ustr = ustr.Replace(LegalChars[i], IllegalChars[i]);
        }
        ustr.Replace(LegalChars[0], IllegalChars[0]);
        return ustr;
    }



    public static Action<string> BackupFile = f => File.Copy(f, f + BACKUP_SUFFIX, true);

    public static Action<string> ClearBackup = f => File.Delete(f + BACKUP_SUFFIX);


    public static void Dump<T>(T obj, string path)
    {
        EnsureDirectory(Path.GetDirectoryName(path) ?? "");
        var serializer = new JsonSerializer();

        using (var sw = new StreamWriter(path))
        using (JsonWriter writer = new JsonTextWriter(sw))
        {
            serializer.Serialize(writer, obj);
        }
    }

    public static T? Load<T>(string path)
    {
        var serializer = new JsonSerializer();

        using (var sw = new StreamReader(path))
        using (var reader = new JsonTextReader(sw))
        {
            return serializer.Deserialize<T>(reader);
        }
    }


    public static void DeleteFilesBefore(string pathDir, int days)
    {
        if (!Directory.Exists(pathDir))
        {
            return;
        }
        string[] pathFiles = Directory.GetFiles(pathDir);
        DateTime now = DateTime.Now;
        foreach (string pathFile in pathFiles)
        {
            if (File.GetLastWriteTime(pathFile).AddDays(days) < now)
            {
                File.Delete(pathFile);
            }
        }
    }
}