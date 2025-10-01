using System;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace TrabalhoP2P
{
    public class DirectoryWatcher
    {
        private readonly string _path;
        private readonly FileManager _fileManager;
        private readonly HashSet<string> _recentlyProcessed = new HashSet<string>();
        private readonly object _lock = new object();

        public DirectoryWatcher(string path, FileManager fileManager)
        {
            _path = path;
            _fileManager = fileManager;
        }

        public void Start()
        {
            FileSystemWatcher watcher = new FileSystemWatcher(_path);
            watcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite;

            watcher.Created += OnFileChanged;
            watcher.Changed += OnFileChanged;
            watcher.Deleted += OnFileDeleted;
            // Evento de Renomear foi removido

            watcher.EnableRaisingEvents = true;
            Console.WriteLine($"Monitorando o diretório: {_path}");
        }
        
        private async void OnFileChanged(object sender, FileSystemEventArgs e)
        {
            if (e.Name == null || ShouldIgnoreEvent(e.Name)) return;

            Console.WriteLine($"Alteração detectada: {e.ChangeType} -> {e.FullPath}");

            await Task.Delay(100); 
            await _fileManager.SendFile(e.FullPath);
        }

        private void OnFileDeleted(object sender, FileSystemEventArgs e)
        {
            if (e.Name == null || ShouldIgnoreEvent(e.Name)) return;
            
            Console.WriteLine($"Alteração detectada: {e.ChangeType} -> {e.FullPath}");
            _fileManager.DeleteFile(e.Name);
        }

        private bool ShouldIgnoreEvent(string fileName)
        {
            lock (_lock)
            {
                if (fileName.StartsWith(".") || _fileManager.IsReceivingOperation)
                {
                    return true;
                }

                if (_recentlyProcessed.Contains(fileName))
                {
                    return true;
                }

                _recentlyProcessed.Add(fileName);
                Task.Delay(1000).ContinueWith(t => 
                {
                    lock (_lock)
                    {
                        _recentlyProcessed.Remove(fileName);
                    }
                });

                return false;
            }
        }
    }
}