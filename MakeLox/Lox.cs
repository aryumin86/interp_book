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
            var parser = new Parser(tokens);
            Expr expression = parser.Parse();

            Console.WriteLine(hadError);
            if(hadError) return;

            //foreach(var token in tokens){
            //    Console.WriteLine(token);
            //}

            Console.WriteLine(new AstPrinter().Print(expression));
        }

        public static void Error(int line, string message){
            Report(line, string.Empty, message);
        }

        public static void Error(Token token, string message){
            if(token.TokenType == TokenType.EOF)
                Report(token.Line, " at end", message);
            else
                Report(token.Line, $" at '{token.Lexeme}'", message);
        }

        private static void Report(int line, string where, string message){
            Console.WriteLine($"[line {line}] Error {where}: {message}");
        }
    }
}

