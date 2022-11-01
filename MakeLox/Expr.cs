using ru.aryumin.Lox;

namespace ru.aryumin.Lox {
	public abstract class Expr {

		public abstract R Accept<R>(Visitor<R> visitor);
	}

	public interface Visitor<R> {
		 R VisitBinaryExpr(Binary expr);
		 R VisitGroupingExpr(Grouping expr);
		 R VisitLiteralExpr(Literal expr);
		 R VisitUnaryExpr(Unary expr);
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
		public override R Accept<R>(Visitor<R> visitor){
			return visitor.VisitBinaryExpr(this);
		}
	}

	public class Grouping : Expr {

		public Expr Expression {get; set;}
		public Grouping (Expr expression) {
			Expression = expression;
		}
		public override R Accept<R>(Visitor<R> visitor){
			return visitor.VisitGroupingExpr(this);
		}
	}

	public class Literal : Expr {

		public object Value {get; set;}
		public Literal (object value) {
			Value = value;
		}
		public override R Accept<R>(Visitor<R> visitor){
			return visitor.VisitLiteralExpr(this);
		}
	}

	public class Unary : Expr {

		public Token Operator {get; set;}
		public Expr Right {get; set;}
		public Unary (Token @operator, Expr right) {
			Operator = @operator;
			Right = right;
		}
		public override R Accept<R>(Visitor<R> visitor){
			return visitor.VisitUnaryExpr(this);
		}
	}
}
