using Doctor_Appointment_System.FileServices;

namespace Doctor_Appointment_System.FileServices;

public interface IFileService
{
    Task<string> Upload(IFormFile file, string folderName);
    void DeleteFile(string fileNameWithExtension, string folderName);
}

public class FileService : IFileService
{
    private readonly string _rootPath;

    public FileService()
    {
        // Use the current directory as the application root
        _rootPath = Directory.GetCurrentDirectory();
    }

    public void DeleteFile(string fileNameWithExtension, string folderName)
    {
        if (string.IsNullOrEmpty(fileNameWithExtension))
            throw new ArgumentNullException(nameof(fileNameWithExtension));

        var path = Path.Combine(_rootPath, "wwwroot", folderName, fileNameWithExtension);

        if (!File.Exists(path))
            throw new FileNotFoundException($"File not found at path: {path}");

        File.Delete(path);
    }

    public async Task<string> Upload(IFormFile file, string folderName)
    {
        if (file is null || file.Length == 0)
            return string.Empty;

        var uploadPath = Path.Combine(_rootPath, "wwwroot", folderName);
        if (!Directory.Exists(uploadPath))
            Directory.CreateDirectory(uploadPath);

        var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
        var filePath = Path.Combine(uploadPath, fileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        return fileName;
    }
}
