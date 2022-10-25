using System;
using System.Text;
using ru.aryumin.Lox;

namespace ru.aryumin.Lox.AstGenerator {
    public class AstPrinter : Visitor<string>
    {
        public string Print(ru.aryumin.Lox.AstGenerator.Expr expr){
            return expr.Accept(this);
        }

        public string VisitBinaryExpr(Binary expr)
        {
            return Parenthesize(expr.Operator.Lexeme, expr.Left, expr.Right);
        }

        public string VisitGroupingExpr(Grouping expr)
        {
            return Parenthesize("group", expr.Expression);
        }

        public string VisitLiteralExpr(Literal expr)
        {
            if(expr.Value == null) return "nil";
            return expr.Value.ToString();
        }

        public string VisitUnaryExpr(Unary expr)
        {
            return Parenthesize(expr.Operator.Lexeme, expr.Right);
        }

        private string Parenthesize(string name, params Expr[] exprs){
            var sb = new StringBuilder();
            sb.Append("(").Append(name);
            foreach(var expr in exprs){
                sb.Append(" ");
                sb.Append(expr.Accept(this));
            }
            sb.Append(")");
            return sb.ToString();
        }
        
        // test printing
        /*
        public static void Main(string[] args){
            Expr expression = new Binary(
                new Unary(
                    new Token(TokenType.MINUS, "-", null, 1),
                    new Literal(123)),
                new Token(TokenType.STAR, "*", null, 1),
                new Grouping(new Literal(45.67)));

            Console.WriteLine(new AstPrinter().Print(expression));
        }
        */
    }
}