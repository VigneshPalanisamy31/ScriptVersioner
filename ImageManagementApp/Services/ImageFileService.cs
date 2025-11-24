using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageManagementApp.Services
{
    public class ImageFileService
    {
        private readonly string _pendingDeletionFolder;

        public ImageFileService()
        {
            var projectRoot = AppDomain.CurrentDomain.BaseDirectory;
            _pendingDeletionFolder = Path.Combine(projectRoot, "..", "..", "..", "ScriptData", "PendingDeletion");
        }

        public void CleanupUnusedImages(List<string> usedImagePaths)
        {
            if (!Directory.Exists(_pendingDeletionFolder))
                return;

            var pendingFiles = Directory.GetFiles(_pendingDeletionFolder);
            foreach (var file in pendingFiles)
            {
                if (!usedImagePaths.Any(p => Path.GetFileName(p) == Path.GetFileName(file)))
                {
                    File.Delete(file);
                }
            }
        }
    }
}
