namespace ru.aryumin.Lox {
    public class Parser {
        private readonly Token[] _tokens;
        private int _current;

        public Parser(IEnumerable<Token> tokens){
            _tokens = tokens.ToArray();
        }

        private Expr Expression(){
            return Equality();
        }

        private Expr Equality()
        {
            Expr expr = this.Comparison();
            while(Match(TokenType.BANG_EQUAL, TokenType.EQUAL_EQUAL)){
                Token @operator = Previous();
                Expr right = Comparison();
                expr = new Binary(expr, @operator, right);
            }
            return expr;
        }

        private bool Match(params TokenType[] types)
        {
            foreach(var type in types){
                if(Check(type)){
                    Advance();
                    return true;
                }
            }
            return false;
        }

        private Token Advance()
        {
            if(!IsAtEnd()) _current++;
            return Previous();
        }

        private bool Check(TokenType type)
        {
            if(IsAtEnd()) return false;
            return Peek().TokenType == type;
        }

        private bool IsAtEnd() => Peek().TokenType == TokenType.EOF;

        private Token Peek() => _tokens[_current];

        private Token Previous() => _tokens[_current-1];

        private Expr Comparison()
        {
            Expr expr = Term();
            while(Match(TokenType.GREATER, TokenType.GREATER_EQUAL, TokenType.LESS, TokenType.LESS_EQUAL)){
                Token @operator = Previous();
                Expr right = Term();
                expr = new Binary(expr, @operator, right);
            }
            return expr;
        }


        // Just for fun (as alternative for Comparison(), Equality(), Term() etc)
        private Expr ParseOperators<T>(IEnumerable<TokenType> operators, Func<T> exprCreatorFn) where T: Expr, new(){
            Expr expr = Term();
            while(Match(operators.ToArray())){
                Token @operator = Previous();
                Expr right = exprCreatorFn();
                expr = new Binary(expr, @operator, right);
            }

            return expr;
        }

        private Expr Term()
        {
            throw new NotImplementedException();
        }
    }
}