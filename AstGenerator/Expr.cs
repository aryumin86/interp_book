using System;
using ru.aryumin.Lox;

namespace ru.aryumin.Lox {

	public abstract class Expr {
	}
	public class Binary : Expr {

		public Expr Left {get; set;}
		public Token Operator {get; set;}
		public Expr Right {get; set;}
		public Binary (Expr left, Token @operator, Expr right) {
			Left = left;
			Operator = @operator;
			Right = right;
		}
	}

	public class Grouping : Expr {

		public Expr Expression {get; set;}
		public Grouping (Expr expression) {
			Expression = expression;
		}
	}

	public class Literal : Expr {

		public object Value {get; set;}
		public Literal (object value) {
			Value = value;
		}
	}

	public class Unary : Expr {

		public Token Operator {get; set;}
		public Expr Right {get; set;}
		public Unary (Token @operator, Expr right) {
			Operator = @operator;
			Right = right;
		}
	}

}
