using System;
using System.Globalization;
using System.IO;
using System.Windows.Media.Imaging;
using Microsoft.Win32;

namespace OrgTechApp
{
    public static class PhotoHelper
    {
        public const string NoPhoto = "Assets/Images/no-photo.png";

        public static string Resolve(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                path = NoPhoto;

            path = path.Replace('/', Path.DirectorySeparatorChar).Replace('\\', Path.DirectorySeparatorChar);
            return Path.IsPathRooted(path) ? path : Path.Combine(AppContext.BaseDirectory, path);
        }

        public static BitmapImage Load(string path)
        {
            try
            {
                var fullPath = Resolve(path);
                if (!File.Exists(fullPath))
                    fullPath = Resolve(NoPhoto);

                var image = new BitmapImage();
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.UriSource = new Uri(fullPath, UriKind.Absolute);
                image.EndInit();
                image.Freeze();
                return image;
            }
            catch
            {
                return null;
            }
        }

        public static string ChooseAndCopy()
        {
            var dialog = new OpenFileDialog
            {
                Title = "Выберите фотографию оргтехники",
                Filter = "Изображения|*.jpg;*.jpeg;*.png;*.bmp;*.webp|Все файлы|*.*"
            };

            if (dialog.ShowDialog() != true)
                return null;

            var folder = Path.Combine(AppContext.BaseDirectory, "Photos");
            Directory.CreateDirectory(folder);

            var extension = Path.GetExtension(dialog.FileName).ToLowerInvariant();
            if (string.IsNullOrWhiteSpace(extension))
                extension = ".jpg";

            var fileName = Guid.NewGuid().ToString("N") + extension;
            var destination = Path.Combine(folder, fileName);
            File.Copy(dialog.FileName, destination, true);

            return "Photos/" + fileName;
        }
    }
}
