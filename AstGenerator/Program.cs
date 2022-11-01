using System;
using System.Text;
using ru.aryumin.Lox;

public static class Program {
    public static void Main(string[] args){

            ru.aryumin.Lox.AstGenerator.Expr expression = new ru.aryumin.Lox.AstGenerator.Binary(
                new ru.aryumin.Lox.AstGenerator.Unary(
                    new Token(ru.aryumin.Lox.TokenType.MINUS, "-", null, 1),
                    new ru.aryumin.Lox.AstGenerator.Literal(123)),
                new Token(ru.aryumin.Lox.TokenType.STAR, "*", null, 1),
                new ru.aryumin.Lox.AstGenerator.Grouping(new ru.aryumin.Lox.AstGenerator.Literal(45.67)));

            Console.WriteLine(new ru.aryumin.Lox.AstGenerator.AstPrinter().Print(expression));

        // Generating classes
        /*
        if(args.Length != 1){
            Console.WriteLine("Uasge: dotnet run -- generate_ast <output directory>");
            Environment.Exit(64);
        }
        var outputDir = args.First();
        DefineAst(outputDir, "Expr", new string[]{
            "Binary : Expr left, Token @operator, Expr right",
            "Grouping : Expr expression",
            "Literal : object value",
            "Unary : Token @operator, Expr right"
        });
        */
    }

    private static void DefineAst(string outputDir, string baseName, IEnumerable<string> types){
        var path = $"{outputDir}/{baseName}.cs";
        if(File.Exists(path)) File.Delete(path);

        var sb = new StringBuilder();
        sb.AppendLine("using System;");
        sb.AppendLine("using ru.aryumin.Lox;");
        sb.AppendLine();
        sb.AppendLine("namespace ru.aryumin.Lox {");
        sb.AppendLine();
        sb.AppendLine($"\tpublic abstract class {baseName} {{");     
        sb.AppendLine();
        sb.AppendLine("\t\tpublic abstract R Accept<R>(Visitor<R> visitor);"); 
        sb.AppendLine("\t}");
        sb.AppendLine();

        DefineVisitor(sb, baseName, types);

        foreach(var type in types){
            var className = type.Split(':')[0].Trim();
            var fields = type.Split(':')[1].Trim();
            DefineType(sb, baseName, className, fields);
        }
        
        sb.AppendLine("}");
        File.WriteAllText(path, sb.ToString());
    }

    private static void DefineVisitor(StringBuilder sb, string baseName, IEnumerable<string> types){
        sb.AppendLine("\tpublic interface Visitor<R> {");
        foreach(var t in types){
            var typeName = t.Split(':')[0].Trim();
            sb.AppendLine($"\t\t R Visit{typeName + baseName}({typeName} {baseName.ToLowerInvariant()});");            
        }
        sb.AppendLine("\t}");
    }

    private static void DefineType(StringBuilder sb, string baseName, string className, string fieldsList){
        var fields = fieldsList.Split(',');
        sb.AppendLine($"\tpublic class {className} : {baseName} {{");

        // properties
        sb.AppendLine();
        foreach(var f in fields){
            var propData = f.Split(' ', StringSplitOptions.RemoveEmptyEntries); //error
            (string fType, string fName) prop = (propData[0], propData[1]);
            prop.fName = prop.fName[0] == '@' 
                ? char.ToUpper(prop.fName[1]) + prop.fName.Substring(2)
                : char.ToUpper(prop.fName[0]) + prop.fName.Substring(1);
            sb.AppendLine($"\t\tpublic {prop.fType} {prop.fName} {{get; set;}}");
        }

        // constructor
        sb.AppendLine($"\t\tpublic {className} ({fieldsList}) {{");
        foreach(var f in fields){
            var fName = f.Split(' ', StringSplitOptions.RemoveEmptyEntries)[1];
            var propName = fName[0] == '@'
                ? char.ToUpper(fName[1]) + fName.Substring(2)
                : char.ToUpper(fName[0]) + fName.Substring(1);
            sb.AppendLine($"\t\t\t{propName} = {fName};");
        }
        sb.AppendLine("\t\t}"); 
        sb.AppendLine($"\t\tpublic override R Accept<R>(Visitor<R> visitor){{");
        sb.AppendLine($"\t\t\treturn visitor.Visit{className + baseName}(this);");
        sb.AppendLine("\t\t}");    
        sb.AppendLine("\t}");
        sb.AppendLine();
    }
}
