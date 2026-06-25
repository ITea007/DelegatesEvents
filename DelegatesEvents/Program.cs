namespace DelegatesEvents
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Путь к каталогу для обхода
            string directoryPath = @"C:\Projects";
            // Если каталог недоступен, используем текущую папку
            if (!Directory.Exists(directoryPath))
                directoryPath = Directory.GetCurrentDirectory();

            Console.WriteLine($"Обход каталога: {directoryPath}\n");

            // Создаём обходчик
            var walker = new DirectoryWalker(directoryPath);

            // Подписываемся на событие FileFound
            walker.FileFound += (sender, e) =>
            {
                Console.WriteLine($"Найден файл: {e.FileName}");

                // Условие отмены: останавливаем поиск, если в имени файла встречается "stop" (без учёта регистра)
                if (Path.GetFileName(e.FileName).Contains("stop", StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("  -> Отмена поиска (обнаружен файл с 'stop').");
                    e.Cancel = true;
                }
            };

            // Запускаем обход
            Console.WriteLine("Начинаем обход...\n");
            walker.Search();
            Console.WriteLine("\nОбход завершён.\n");

            // ===== Применяем GetMax для поиска максимального элемента =====
            try
            {
                // Получаем все файлы (включая подкаталоги) для демонстрации
                var allFiles = Directory.GetFiles(directoryPath, "*", SearchOption.AllDirectories);

                if (allFiles.Length == 0)
                {
                    Console.WriteLine("Нет файлов для поиска максимума.");
                    return;
                }

                // Вариант 1: максимальная длина полного пути
                string maxPathLengthFile = allFiles.GetMax(f => (float)f.Length);
                Console.WriteLine($"Файл с максимальной длиной пути: {maxPathLengthFile}");
                Console.WriteLine($"Длина пути: {maxPathLengthFile.Length} символов.");

                // Вариант 2: максимальный размер файла (используем FileInfo)
                var fileInfos = allFiles.Select(f => new FileInfo(f)).ToList();
                // Фильтруем файлы, к которым есть доступ, иначе GetMax может упасть
                var validFileInfos = fileInfos.Where(fi => fi.Exists).ToList();
                if (validFileInfos.Any())
                {
                    var maxSizeFile = validFileInfos.GetMax(fi => fi.Length);
                    Console.WriteLine($"\nФайл с максимальным размером: {maxSizeFile.FullName}");
                    Console.WriteLine($"Размер: {maxSizeFile.Length} байт.");
                }
                else
                {
                    Console.WriteLine("\nНет доступных файлов для определения максимального размера.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при получении файлов: {ex.Message}");
            }

            Console.WriteLine("\nНажмите любую клавишу для выхода...");
            Console.ReadKey();
        }
    }
}
