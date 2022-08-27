
namespace LoveCooking
{
    public interface IImageSevice
    {
        public Task<String> SaveFile(IFormFile file, String user = "");
        public bool DeleteFile(string path);
    }

    public class ImageSevice : IImageSevice
    {
        public async Task<String> SaveFile(IFormFile file, String user = "")
        {
            string path = Path.Combine(Directory.GetCurrentDirectory(), "files", "images");

            string filename = "image_" + user + DateTime.Now.ToString("yyyyMMddHHmmss") + Path.GetExtension(file.FileName);

            using (var stream = new FileStream(Path.Combine(path, filename), FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return $"https://localhost:7274/files/vratisliku/{filename}";
        }

        /*
                public Task<IFormFile> GetFile(String url)
                {
                    return null;
                }
                public void DeleteFile(String path)
                {

                }

        */
        public bool DeleteFile(string path)
        {
            if (path == "")
                return false;

            List<string> lista = path.Split('/').ToList();

            string fullPath = Path.Combine(Directory.GetCurrentDirectory(), "files", "images", lista.Last());

            if (!File.Exists(fullPath))
                return false;

            File.Delete(fullPath);
            return true;
        }
    }
}