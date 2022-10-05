using System;

namespace ru.aryumin.Lox {
    public class Scanner {

        private readonly string _source;
        private readonly List<Token> _tokens = new List<Token>();
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

            _tokens.Add(new Token(TokenType.EOF, string.Empty, null, _line));
            return _tokens;
        }

        private bool IsAtEnd() =>  _current >= _source.Length;
        private char Advance() => _source[_current++];

        private void AddToken(TokenType tokenType) => AddToken(tokenType, null);
        private void AddToken(TokenType tokenType, object literal){
            var text = _source.Substring(_start, _current);
            _tokens.Add(new Token(tokenType, text, literal, _line));
        }

        private bool Match(char expected){
            if(IsAtEnd()) return false;
            if(_source[_current] != expected) return false;

            _current++;
            return true;
        }

        private char Peek() {
            if(IsAtEnd()) return '\0';
            return _source[_current];
        }

        private void @String() {
            while (Peek() != '"' && !IsAtEnd()) {
                if(Peek() == '\n') _line++;
                Advance();
            }

            if (IsAtEnd()){
                Lox.Error(_line, "Unterminated string");
                return;
            }

            Advance();

            var value = _source.Substring(_start + 1, _current - 1);
            AddToken(TokenType.STRING, value);
        }

        private bool IsDigit(char c) => c >= '0' && c <= '9';

        private void Number() {
            while(IsDigit(Peek())) Advance();

            if(Peek() == '.' && IsDigit(PeekNext())){
                Advance();
                while (IsDigit(Peek())) Advance();
            }

            AddToken(TokenType.NUMBER, Double.Parse(_source.Substring(_start, _current)));
        }

        private char PeekNext() {
            if(_current + 1 >= _source.Length) return '\0';
            return _source[_current+1];
        }

        private void ScanToken(){
            var c = Advance();
            switch (c) {
                case '(': AddToken(TokenType.LEFT_PAREN); break;
                case ')': AddToken(TokenType.RIGHT_PAREN); break;
                case '{': AddToken(TokenType.LEFT_BRACE); break;
                case '}': AddToken(TokenType.RIGHT_BRACE); break;
                case ',': AddToken(TokenType.COMMA); break;
                case '.': AddToken(TokenType.DOT); break;
                case '-': AddToken(TokenType.MINUS); break;
                case '+': AddToken(TokenType.PLUS); break;
                case ';': AddToken(TokenType.SEMICOLON); break;
                case '*': AddToken(TokenType.STAR); break;
                case '!': AddToken(Match('=') ? TokenType.BANG_EQUAL : TokenType.BANG); break;
                case '=': AddToken(Match('=') ? TokenType.EQUAL_EQUAL : TokenType.EQUAL); break;
                case '<': AddToken(Match('=') ? TokenType.LESS_EQUAL : TokenType.LESS); break;
                case '>': AddToken(Match('=') ? TokenType.GREATER_EQUAL : TokenType.GREATER); break;
                
                case '/': 
                    if(Match('/')) {
                    while (Peek() !=  '\n' && !IsAtEnd()) Advance();
                    }
                    else {
                        AddToken(TokenType.SLASH);
                    }
                    break;

                case ' ':
                case '\r':
                case '\t':
                    break;

                case '\n': _line++; break;

                case '"': @String(); break;

                default: 
                    if(IsDigit(c)){
                        Number();
                    }
                    else{
                        Lox.Error(_line, "Unexpected charcter");
                    }
                    break;
            }
        }
    }
}