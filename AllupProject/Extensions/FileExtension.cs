namespace AllupProject.Extensions;

public static class FileExtension
{
    public static string SaveFile(this IFormFile file,string rootPath, string folderName)
    {
        string fileName = file.FileName;
        if (fileName.Length > 14)
        {
            fileName = fileName.Substring(fileName.Length - 14, 14);
        }
        fileName = Guid.NewGuid().ToString() + fileName;
        string path = Path.Combine(rootPath, folderName, fileName);
        using (FileStream fileStream = new(path, FileMode.Create))
        {
            file.CopyTo(fileStream);
        }
        return fileName;
    }
    public static void DeleteFile(string rootPath, string folderName, string fileName)
    {
        string path = Path.Combine(rootPath,folderName,fileName);
        if (File.Exists(path))
        {
            File.Delete(path);
        }
    }
}
