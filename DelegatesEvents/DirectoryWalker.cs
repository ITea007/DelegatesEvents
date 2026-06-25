using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DelegatesEvents
{
    /// <summary>
    /// Класс для рекурсивного обхода каталога с генерацией событий при нахождении файлов.
    /// </summary>
    public class DirectoryWalker
    {
        private readonly string _directoryPath;
        private bool _cancelRequested;

        public event EventHandler<FileArgs>? FileFound;

        public DirectoryWalker(string directoryPath)
        {
            if (!Directory.Exists(directoryPath))
                throw new DirectoryNotFoundException($"Каталог '{directoryPath}' не найден.");
            _directoryPath = directoryPath;
        }

        /// <summary>
        /// Запускает обход каталога.
        /// </summary>
        public void Search()
        {
            _cancelRequested = false;
            SearchDirectory(_directoryPath);
        }

        private void SearchDirectory(string currentDir)
        {
            // Если отмена уже запрошена, выходим
            if (_cancelRequested)
                return;

            // Получаем файлы в текущем каталоге
            string[] files;
            try
            {
                files = Directory.GetFiles(currentDir);
            }
            catch (UnauthorizedAccessException)
            {
                // Нет доступа — пропускаем каталог
                return;
            }

            foreach (string file in files)
            {
                if (_cancelRequested)
                    break;

                var args = new FileArgs(file);
                OnFileFound(args); // Вызываем событие

                // Проверяем, не запросила ли отмену подписка
                if (args.Cancel)
                {
                    _cancelRequested = true;
                    break;
                }
            }

            if (_cancelRequested)
                return;

            // Получаем подкаталоги
            string[] subDirs;
            try
            {
                subDirs = Directory.GetDirectories(currentDir);
            }
            catch (UnauthorizedAccessException)
            {
                return;
            }

            foreach (string subDir in subDirs)
            {
                SearchDirectory(subDir);
                if (_cancelRequested)
                    break;
            }
        }

        /// <summary>
        /// Защищённый метод для вызова события FileFound.
        /// </summary>
        protected virtual void OnFileFound(FileArgs e)
        {
            FileFound?.Invoke(this, e);
        }
    }
}
