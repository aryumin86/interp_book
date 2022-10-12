using System;
using System.Text;
public static class Program {
    public static void Main(string[] args){
        if(args.Length != 1){
            Console.WriteLine("Uasge: dotnet run -- generate_ast <output directory>");
            Environment.Exit(64);
        }
        var outputDir = args.First();
        DefineAst(outputDir, "Expr", new string[]{
            "Binary : Expr left, Token operator, Expr right",
            "Grouping : Expr expression",
            "Literal : object value",
            "Unary : Token operator, Expr right"
        });
    }

    private static void DefineAst(string outputDir, string baseName, IEnumerable<string> types){
        var path = $"{outputDir}/{baseName}.cs";
        if(File.Exists(path)) return;

        File.Create(path);
        var sb = new StringBuilder();
        sb.AppendLine("using ru.aryumin.Lox;");
        sb.AppendLine();
        sb.AppendLine($"abstract class {baseName} {{");

        foreach(var type in types){
            var className = type.Split(':')[0].Trim();
            var fields = type.Split(':')[1].Trim();
            DefineType(sb, baseName, className, fields);
        }

        sb.AppendLine("}");
    }

    private static void DefineType(StringBuilder sb, string baseName, string className, string fieldsList){
        sb.AppendLine($"\tpublic class {className} : {baseName} {{");
        sb.AppendLine($"\t\t{className} ({fieldsList} {{");
        var fields = fieldsList.Split(',');

        // properties
        sb.AppendLine();
        foreach(var f in fields){
            (string fType, string fName) prop = 
                f.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(f => (f[0], f[1])); //error
            prop.fName = char.ToUpper(prop.fName[0]) + prop.fName.Substring(1);
            sb.AppendLine($"\tpublic {prop.fType} {prop.fName} {{get; set;}}");
        }

        // constructor
        foreach(var f in fields){
            var fName = f.Split(' ', StringSplitOptions.RemoveEmptyEntries)[1];
            sb.AppendLine($"\t\t{char.ToUpper(fName[0])}{fName.Substring(1)} = {fName}");
        }
        sb.AppendLine("\t}");
    }
}
