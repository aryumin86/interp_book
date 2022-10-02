using System;

namespace ru.aryumin.Lox {
    public class Scanner {

        private readonly string _source;

        public Scanner(string source)
        {
            _source = source;
        }
        public IEnumerable<Token> ScanTokens(){
            throw new NotImplementedException();
        }
    }
}