using System;

namespace ru.aryumin.Lox {
    public static class Lox {

        static bool hadError = false;

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
            if(hadError) 
                Environment.Exit(65);
        }

        private static void RunPrompt(){
            while(true){
                Console.Write("> ");
                var line = Console.ReadLine();
                if(string.IsNullOrWhiteSpace(line))
                    break;
                Run(line);
                hadError = false;
            }
        }

        private static void Run(string source){
            Scanner scanner = new Scanner(source);
            IEnumerable<Token> tokens = scanner.ScanTokens();

            foreach(var token in tokens){
                Console.WriteLine(token);
            }
        }

        static void Error(int line, string message){
            Report(line, string.Empty, message);
        }

        private static void Report(int line, string where, string message){
            Console.WriteLine($"[line {line}] Error {where}: {message}");
        }
    }
}

