using System;

namespace ru.aryumin.Lox {
    public class Scanner {

        private readonly string _source;
        private readonly List<Token> tokens = new List<Token>();
        private int _start = 0;
        private int _current = 0;
        private int _line = 1;

        public Scanner(string source)
        {
            _source = source;
        }
        public IEnumerable<Token> ScanTokens(){
            while(!IsAtEnd()){
                _start = _current;
                ScanToken();
            }

            tokens.Add(new Token(TokenType.EOF, string.Empty, null, _line));
            return tokens;
        }

        private bool IsAtEnd(){
            return _current >= _source.Length;
        }

        private void ScanToken(){
            throw new NotImplementedException();
        }
    }
}