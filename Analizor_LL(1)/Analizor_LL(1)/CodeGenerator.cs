using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.CodeDom;
using System.CodeDom.Compiler;
using Microsoft.CSharp;
using System.Reflection;
using System.IO;

namespace Analizor_LL_1_
{
    public class CodeGenerator
    {
        CodeCompileUnit targetUnit;
        CodeTypeDeclaration targetClass;
        private const string outputFileName = "SampleCode.cs";
        

        public CodeGenerator()
        {
            targetUnit = new CodeCompileUnit();
            CodeNamespace generator = new CodeNamespace("GeneratedCodeNamespace");
            generator.Imports.Add(new CodeNamespaceImport("System"));
            targetClass = new CodeTypeDeclaration("CodeDOMCreatedClass");
            targetClass.IsClass = true;
            targetClass.TypeAttributes = TypeAttributes.Public;
            generator.Types.Add(targetClass);
            targetUnit.Namespaces.Add(generator);
        }

        public void AddFields()
        {
            // Se creeaza un membru de tip CodeMemberField
            CodeMemberField inputStringField = new CodeMemberField
            {

                // Se seteaza atributele
                Attributes = MemberAttributes.Private | MemberAttributes.Static,

                // Se seteaza denumirea membrului
                Name = "inputArrayString",

                // Se seteaza tipul membrului
                Type = new CodeTypeReference(typeof(System.String[]))
            };

            // Se adauga comentarii
            inputStringField.Comments.Add(new CodeCommentStatement("Sirul de intrare dat de utilizator"));

            // Se adauga membrul la targetClass
            targetClass.Members.Add(inputStringField);

            
            CodeMemberField indexField = new CodeMemberField();
            indexField.Attributes = MemberAttributes.Private | MemberAttributes.Static;
            indexField.Name = "stringIndex";
            indexField.Type = new CodeTypeReference(typeof(System.Int32));
            indexField.Comments.Add(new CodeCommentStatement("Index in sirul de intrare"));
            targetClass.Members.Add(indexField);

        }

        public void AddMethods(Gramatica gramatica)
        {
            List<CodeMemberMethod> methodList = new List<CodeMemberMethod>();
            for(int i = 0; i < gramatica.Neterminale.Count; i++)
            {
                CodeMemberMethod tmp = new CodeMemberMethod
                {
                    Attributes = MemberAttributes.Private | MemberAttributes.Static,
                    Name = gramatica.Tab.Rows[i][0].ToString(),
                    ReturnType = new CodeTypeReference(typeof(void)),
                };
                targetClass.Members.Add(tmp);
                CodeSnippetExpression methodValue = new CodeSnippetExpression();
                for(int j = 1; j <= gramatica.Terminale.Count + 1; j++)
                {
                    ReguliDeProductie regula = gramatica.Tab.Rows[i][j] as ReguliDeProductie;
                    if (regula != null)
                    {
                        methodValue.Value += $"if( inputArrayString[stringIndex] == \"{gramatica.Tab.Columns[j].Caption}\" )\n" +
                                                "{\n";

                        if (regula.GetRight().Count() >= 1 && regula.GetRight()[0] != "")
                        {
                            foreach (var symbol in regula.GetRight())
                            {
                                if (gramatica.Neterminale.Contains(symbol))
                                {
                                    methodValue.Value += $"{symbol}();\n";
                                }
                                if (gramatica.Terminale.Contains(symbol))
                               {
                                    methodValue.Value += "stringIndex++;\n";
                                }
                            }
                        }
                        methodValue.Value += "return;\n";
                        
                        methodValue.Value += "}\n";
                    }
                }

                methodValue.Value += $"else\nthrow new Exception($\"Caracter invalid la pozitia {{ stringIndex }}\");\n";
                tmp.Statements.Add(new CodeSnippetStatement(methodValue.Value));
            }
                
        }

        public void AddEntryPoint(Gramatica gramatica)
        {
            CodeEntryPointMethod start = new CodeEntryPointMethod();
            CodeSnippetExpression methodValue = new CodeSnippetExpression();
            methodValue.Value += "stringIndex = 0;\n"+
                                    $" inputArrayString = Console.ReadLine().Split(\' \'); \n" +
                                    "try \n" +
                                   "{\n" +
                                   $"\t { gramatica.Start } (); \n" +
                                   "if( inputArrayString[stringIndex] == \"$\" ) \n" +
                                   "\tConsole.WriteLine(\"Propozitie corecta\");\n" +
                                   "else\n" +
                                   "Console.WriteLine(\"Propozitie Incorecta\");\n" +
                                   "}\n" +
                                   "catch(Exception e)\n" +
                                   "{\n"+
                                   "Console.WriteLine(e.Message);\n" +
                                   "Console.WriteLine(\"Propozitie Incorecta\");\n" +
                                   "}\n" +
                                   "Console.ReadKey();\n"
                                   ;
            start.Statements.Add(new CodeSnippetStatement(methodValue.Value));
            targetClass.Members.Add(start);

        }

        public void GenerateCSharpCode(string fileName)
        {
            CodeDomProvider provider = CodeDomProvider.CreateProvider("CSharp");
            CodeGeneratorOptions options = new CodeGeneratorOptions();
            options.BracingStyle = "C";
            using (StreamWriter sourceWriter = new StreamWriter(fileName))
            {
                provider.GenerateCodeFromCompileUnit(
                    targetUnit, sourceWriter, options);
            }
        }
    }
    
}
