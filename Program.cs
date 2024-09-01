using System.Collections.Concurrent;

namespace Hermes{
    public struct Find{
        public string FullPath;
        public string Name;
        public int Distance;
    }

    public static class Program{
        private static readonly ConcurrentBag<Find> ResultBag =[];
        private static float _searchQuality = .25f;
        //%MATCH

        public static int Main(string[] args){
            if (args.Length != 3 || !int.TryParse(args[2].Replace("%", ""), out int precision)){
                Console.WriteLine("Usage: Hermes <path> <file_name> <precision>");
                return 1;
            }

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(Art);
            Console.ResetColor();

            string path = args[0];
            string fileName = args[1];

            string fileExtension = GetFileExtenstion(ref fileName);
            _searchQuality = 1 - Math.Clamp(precision, 0, 100) / 100f;

            if (path == "ALL_DRIVES"){
                Console.WriteLine($"Searching for {fileName + fileExtension} on all drives.");
                foreach (DriveInfo drive in DriveInfo.GetDrives()){
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"Starting search on {drive.RootDirectory.Name}");
                    Console.ResetColor();
                    Search(drive.RootDirectory, fileName, fileExtension);
                }
            }
            else{
                if (Directory.Exists(path)){
                    Console.WriteLine($"Searching for {fileName + fileExtension} in {path}.");
                    DirectoryInfo directory = new(path);
                    Console.WriteLine("Starting search...");
                    Search(directory, fileName, fileExtension);
                }
                else{
                    Console.WriteLine($"Could not find {path} :(");
                    return 2;
                }
            }

            if (ResultBag.IsEmpty){
                Console.WriteLine("Could not find any files :(\nLower the precision and try again.");
                return 3;
            }

            Console.WriteLine($"Search done :)\nFound {ResultBag.Count} items.");
            Console.WriteLine("Starting to sort items...");

            Find[] result = ResultBag.OrderByDescending(x => x.Distance).Reverse().ToArray();
            ResultBag.Clear();

            int amountToShow = Math.Min(result.Length, 10);
            Console.WriteLine($"Done sorting :)\nShowing the {amountToShow} best items");
            for (int i = 0; i < amountToShow; i++){
                string link = TerminalUrl($"{result[i].Name} @ {result[i].FullPath}", result[i].FullPath);
                Console.WriteLine(link);
            }

            return 0;
        }

        private static string TerminalUrl(string caption, string url) => $"\u001B]8;;{url}\a{caption}\u001B]8;;\a";

        private static void Search(DirectoryInfo currentDirectory, string fileName, string fileExtension){
            bool wildCard = fileExtension == ".*";
            try{
                foreach (FileInfo file in currentDirectory.GetFiles()){
                    if (!wildCard && file.Extension != fileExtension) continue;
                    int distance = LevenshteinDistance(file.Name, fileName + fileExtension);
                    float maxDistance =
                        _searchQuality * Math.Max(file.Name.Length, fileName.Length + fileExtension.Length);
                    if (distance > maxDistance) continue;

                    Find find = new(){
                        Name = file.Name,
                        FullPath = file.FullName,
                        Distance = distance
                    };
                    ResultBag.Add(find);
                }
            }
            catch (Exception){
                // ignored
            }

            DirectoryInfo[] directories = currentDirectory.GetDirectories();
            try{
                Parallel.ForEach(directories, directory => { Search(directory, fileName, fileExtension); });
            }
            catch (Exception){
                // ignored
            }
        }


        private static int LevenshteinDistance(string source1, string source2) //O(n*m)
        {
            int source1Length = source1.Length;
            int source2Length = source2.Length;

            int[,] matrix = new int[source1Length + 1, source2Length + 1];

            // First calculation, if one entry is empty return full length
            if (source1Length == 0)
                return source2Length;

            if (source2Length == 0)
                return source1Length;

            // Initialization of matrix with row size source1Length and columns size source2Length
            for (int i = 0; i <= source1Length; matrix[i, 0] = i++){
            }

            for (int j = 0; j <= source2Length; matrix[0, j] = j++){
            }

            // Calculate rows and columns distances
            for (int i = 1; i <= source1Length; i++){
                for (int j = 1; j <= source2Length; j++){
                    int cost = source2[j - 1] == source1[i - 1] ? 0 : 1;

                    matrix[i, j] = Math.Min(
                        Math.Min(matrix[i - 1, j] + 1, matrix[i, j - 1] + 1),
                        matrix[i - 1, j - 1] + cost);
                }
            }

            // return result
            return matrix[source1Length, source2Length];
        }

        private static string GetFileExtenstion(ref string input){
            int extenstionStart = -1;
            for (int i = input.Length - 1; i > 0; i--){
                if (input[i] != '.') continue;
                extenstionStart = i;
                break;
            }

            if (extenstionStart == -1)
                return ".*";

            string extenstion = input[extenstionStart..];
            input = input.Remove(extenstionStart);
            return extenstion;
        }

        private const string Art =
            " _   _                                       \n" +
            "| | | |                                      \n" +
            "| |_| |  ___  _ __  _ __ ___    ___  ___     \n" +
            "|  _  | / _ \\| '__|| '_ ` _ \\  / _ \\/ __| \n" +
            "| | | ||  __/| |   | | | | | ||  __/\\__ \\ \n" +
            "\\_| |_/ \\___||_|   |_| |_| |_| \\___||___/ \n";
    }
}