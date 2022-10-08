using System;

namespace ru.aryumin.Lox {
    public class Scanner {

        private readonly string _source;
        private readonly List<Token> _tokens = new List<Token>();
        private int _start = 0;
        private int _current = 0;
        private int _line = 1;

        private static readonly Dictionary<string, TokenType> _keywords;

        static Scanner() {
            _keywords = new Dictionary<string, TokenType>();
            _keywords.Add("and", TokenType.AND);
            _keywords.Add("class", TokenType.CLASS);
            _keywords.Add("else", TokenType.ELSE);
            _keywords.Add("false", TokenType.FALSE);
            _keywords.Add("for", TokenType.FOR);
            _keywords.Add("fun", TokenType.FUN);
            _keywords.Add("if", TokenType.IF);
            _keywords.Add("nil", TokenType.NIL);
            _keywords.Add("or", TokenType.OR);
            _keywords.Add("print", TokenType.PRINT);
            _keywords.Add("return", TokenType.RETURN);
            _keywords.Add("super", TokenType.SUPER);
            _keywords.Add("this", TokenType.THIS);
            _keywords.Add("true", TokenType.TRUE);
            _keywords.Add("var", TokenType.VAR);
            _keywords.Add("while", TokenType.WHILE);
        }

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
            var text = _source[_start .. _current];
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

            var value = _source[(_start+1)..(_current-1)];
            AddToken(TokenType.STRING, value);
        }

        private bool IsDigit(char c) => c >= '0' && c <= '9';

        private void Number() {
            while(IsDigit(Peek())) Advance();

            if(Peek() == '.' && IsDigit(PeekNext())){
                Advance();
                while (IsDigit(Peek())) Advance();
            }

            AddToken(TokenType.NUMBER, Double.Parse(_source[_start .. _current]));
        }

        private char PeekNext() {
            if(_current + 1 >= _source.Length) return '\0';
            return _source[_current+1];
        }

        private bool IsAlpha(char c) => (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') || c == '_';
        private bool IsAlphaNumeric(char c) => IsAlpha(c) || IsDigit(c);

        private void Identifier() {
            while (IsAlphaNumeric(Peek())) Advance();
            var text = _source[_start .._current];
            // type will be changed if keywords dictionary contains this key
            TokenType tokenType = TokenType.IDENTIFIER; 
            if(_keywords.TryGetValue(text, out TokenType parsedTokenType)) // may be this is keyword
                tokenType = parsedTokenType;
            AddToken(tokenType);
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
                    else if(IsAlpha(c)) {
                        Identifier();
                    }
                    else{
                        Lox.Error(_line, "Unexpected charcter");
                    }
                    break;
            }
        }
    }
}