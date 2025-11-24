using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageManagementApp.Models
{

    public class ImageManager
    {
        private readonly string _assetsFolder;
        private readonly string _imagesFolder;
        private readonly string _pendingDeletionFolder;

        public ImageManager()
        {
            // Point to ScriptData folder at project root
            var projectRoot = AppDomain.CurrentDomain.BaseDirectory;
            _assetsFolder = Path.Combine(projectRoot, "..", "..", "..", "ScriptData");
            _imagesFolder = Path.Combine(_assetsFolder, "Images");
            _pendingDeletionFolder = Path.Combine(_assetsFolder, "PendingDeletion");

            CreateFolders();
        }

        private void CreateFolders()
        {
            Directory.CreateDirectory(_imagesFolder);
            Directory.CreateDirectory(_pendingDeletionFolder);
        }

        public string CreateImage(string stepName)
        {
            var fileName = $"{stepName}_{Guid.NewGuid():N}.png";
            var filePath = Path.Combine(_imagesFolder, fileName);

            // Create a dummy image file
            File.WriteAllText(filePath, $"Mock image data for {stepName}");

            return filePath;
        }

        public void MoveImageToPendingDeletion(string imagePath)
        {
            if (File.Exists(imagePath))
            {
                var fileName = Path.GetFileName(imagePath);
                var destinationPath = Path.Combine(_pendingDeletionFolder, fileName);

                if (File.Exists(destinationPath))
                    File.Delete(destinationPath);

                File.Move(imagePath, destinationPath);
            }
        }

        public void RestoreImageFromPendingDeletion(string imagePath)
        {
            var fileName = Path.GetFileName(imagePath);
            var pendingPath = Path.Combine(_pendingDeletionFolder, fileName);

            if (File.Exists(pendingPath))
            {
                if (File.Exists(imagePath))
                    File.Delete(imagePath);

                File.Move(pendingPath, imagePath);
            }
        }

        public void CleanupPendingDeletion()
        {
            if (Directory.Exists(_pendingDeletionFolder))
            {
                var files = Directory.GetFiles(_pendingDeletionFolder);
                foreach (var file in files)
                {
                    File.Delete(file);
                }
            }
        }

        public int GetActiveImagesCount()
        {
            return Directory.Exists(_imagesFolder) ? Directory.GetFiles(_imagesFolder).Length : 0;
        }

        public int GetPendingDeletionCount()
        {
            return Directory.Exists(_pendingDeletionFolder) ? Directory.GetFiles(_pendingDeletionFolder).Length : 0;
        }

        public List<string> GetGitStatusForImages()
        {
            var status = new List<string>();

            if (Directory.Exists(_imagesFolder))
            {
                var activeImages = Directory.GetFiles(_imagesFolder);
                foreach (var img in activeImages)
                {
                    status.Add($"✓ {Path.GetFileName(img)} (tracked)");
                }
            }

            if (Directory.Exists(_pendingDeletionFolder))
            {
                var pendingImages = Directory.GetFiles(_pendingDeletionFolder);
                foreach (var img in pendingImages)
                {
                    status.Add($"✗ {Path.GetFileName(img)} (deleted - not tracked)");
                }
            }

            return status;
        }
    }
}
