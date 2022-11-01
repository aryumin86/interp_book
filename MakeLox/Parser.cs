using System;

namespace ru.aryumin.Lox {
    public class Parser {
        private readonly Token[] _tokens;
        private int _current;

        public Parser(IEnumerable<Token> tokens){
            _tokens = tokens.ToArray();
        }

        public Expr Parse() {
            try{
                return Expression();
            }
            catch(ParseError error){
                return null;
            }
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
        private Expr ParseOperators(IEnumerable<TokenType> operators, Func<Expr> exprCreatorFn){
            Expr expr = exprCreatorFn();
            while(Match(operators.ToArray())){
                Token @operator = Previous();
                Expr right = exprCreatorFn();
                expr = new Binary(expr, @operator, right);
            }
            return expr;
        }

        private Expr Term()
        {
            Expr expr = Factor();
            while(Match(TokenType.MINUS, TokenType.PLUS)){
                Token @operator = Previous();
                Expr right = Factor();
                expr = new Binary(expr, @operator, right);
            }
            return expr;
        }

        private Expr Factor()
        {
            Expr expr = this.Unary();
            while(Match(TokenType.SLASH, TokenType.STAR)){
                Token @operator = Previous();
                Expr right = this.Unary();
                expr = new Binary(expr, @operator, right);
            }
            return expr;
        }

        private Expr Unary()
        {
            if(Match(TokenType.BANG, TokenType.MINUS)){
                Token @operator = Previous();
                Expr right = this.Unary();
                return new Unary(@operator, right);
            }
            return Primary();
        }

        private Expr Primary()
        {
            if(Match(TokenType.FALSE)) return new Literal(false);
            if(Match(TokenType.TRUE)) return new Literal(true);
            if(Match(TokenType.NIL)) return new Literal(null);
            if(Match(TokenType.NUMBER, TokenType.STRING)) return new Literal(Previous().Literal);
            if(Match(TokenType.LEFT_PAREN)){
                Expr expr = Expression();
                Consume(TokenType.RIGHT_PAREN, "Expect ')' after expression.");
                return new Grouping(expr);
            }

            throw new Exception(Peek() + "Expect expression");
        }

        private Token Consume(TokenType tokenType, string message)
        {
            if(Check(tokenType)) return Advance();
            throw Error(Peek(), message);
        }

        private ParseError Error(Token token, string message){
            Lox.Error(token, message);
            return new ParseError();
        }

        private void Syncronyze() {
            Advance();
            while(!IsAtEnd()){
                if(Previous().TokenType == TokenType.SEMICOLON) return;
                switch(Peek().TokenType){
                    case TokenType.CLASS:
                    case TokenType.FOR:
                    case TokenType.FUN:
                    case TokenType.IF:
                    case TokenType.PRINT:
                    case TokenType.RETURN:
                    case TokenType.VAR:
                    case TokenType.WHILE:
                        return;
                }
                Advance();
            }
        }
    }

    public class ParseError : Exception {

    }
}