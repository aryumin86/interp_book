using System;

namespace ru.aryumin.Lox {
    public static class Lox {
        public static void Main(string[] args){
            if(args.Length > 1){
                Console.WriteLine("Usage: jlox [script]");
                Environment.Exit(64);
            }
            else if(args.Length == 1){
                Runfile(args[0]);
            }
            else{
                RunPrompt();
            }
        }

        private static void Runfile(string path){
            var bytes = File.ReadAllBytes(path);
            Run(System.Text.Encoding.Default.GetString(bytes));
        }

        private static void RunPrompt(){
            while(true){
                Console.Write("> ");
                var line = Console.ReadLine();
                if(string.IsNullOrWhiteSpace(line))
                    break;
                Run(line);
            }
        }

        private static void Run(string source){
            Scanner scanner = new Scanner(source);
            IEnumerable<Token> tokens = scanner.ScanTokens();

            foreach(var token in tokens){
                Console.WriteLine(token);
            }
        }
    }
}

