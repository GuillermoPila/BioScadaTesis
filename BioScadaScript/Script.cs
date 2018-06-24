using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Policy;
using System.Text;
using Microsoft.CSharp;
//using Microsoft.VisualC;
using Microsoft.VisualBasic;


namespace BioScadaScript
{
    public class Script : MarshalByRefObject
    {
        private Assembly _Assembly;
        private Type classType;
        public string CompileScript(ref Dictionary<string, Dictionary<string, object>> Exp_Vars, string Code,
            string NameClassScript, LanguageScript Language)
        {
            string returnData = null;
            _NameScript = NameClassScript;

            CodeDomProvider codeProvider = null;

            switch (Language)
            {
                case LanguageScript.C_Sharp:
                    codeProvider = new CSharpCodeProvider();
                    break;
                case LanguageScript.Visual_Basic:
                    codeProvider = new VBCodeProvider();
                    break;
                default:
                    codeProvider = new CSharpCodeProvider();
                    break;
            }

            CodeCompileUnit compileUnit = new CodeCompileUnit();

            CodeNamespace _NameSpace = new CodeNamespace("Tempur");
            // Add the new namespace to the compile unit.
            compileUnit.Namespaces.Add(_NameSpace);

            // Add the new namespace import for the System namespace.
            _NameSpace.Imports.Add(new CodeNamespaceImport("System"));
            _NameSpace.Imports.Add(new CodeNamespaceImport("System.Collections.Generic"));
            _NameSpace.Imports.Add(new CodeNamespaceImport("System.Reflection"));


            CodeTypeDeclaration _ClassTemp = new CodeTypeDeclaration(NameClassScript);
            // Add the new type to the namespace type collection.
            _NameSpace.Types.Add(_ClassTemp);

            foreach (KeyValuePair<string, Dictionary<string, object>> exp in Exp_Vars)
                foreach (KeyValuePair<string, object> variable in exp.Value)
                {
                    CodeMemberField field1 = new CodeMemberField(variable.Value.GetType(), exp.Key + "_" + variable.Key);
                    field1.Attributes = MemberAttributes.Static;
                    CodePrimitiveExpression sd = new CodePrimitiveExpression(variable.Value);
                    field1.InitExpression = sd;
                    _ClassTemp.Members.Add(field1);
                }

            CodeSnippetStatement codeSnippet = new CodeSnippetStatement(Code);
            char comilla = '"';


            CodeMemberMethod Method = new CodeMemberMethod
            {
                Name = "MethodScript",
                Attributes = MemberAttributes.Public
            };
            CodeParameterDeclarationExpression param = null;

            switch (Language)
            {
                case LanguageScript.C_Sharp:
                    param =
                new CodeParameterDeclarationExpression("System.Collections.Generic.Dictionary<String, Dictionary<String, Object>>",
                                                       "Variables");
                    break;
                case LanguageScript.Visual_Basic:
                    param =
                new CodeParameterDeclarationExpression("System.Collections.Generic.Dictionary(Of String, Dictionary(Of String, Object))",
                                                       "Variables");
                    break;
                case LanguageScript.C_Plus_Plus:
                    param =
                new CodeParameterDeclarationExpression("System.Collections.Generic.Dictionary<String, Dictionary<String, Object>>",
                                                       "Variables");
                    break;
                default:
                    param =
                new CodeParameterDeclarationExpression("System.Collections.Generic.Dictionary<String, Dictionary<String, Object>>",
                                                       "Variables");
                    break;
            }

            //param.Direction = FieldDirection.Ref;
            Method.Parameters.Add(param);

            foreach (KeyValuePair<string, Dictionary<string, object>> exp in Exp_Vars)
            {
                string index = "";
                switch (Language)
                {
                    case LanguageScript.C_Sharp:
                        index = "Variables[" + comilla + exp.Key + comilla + "]";
                        break;
                    case LanguageScript.Visual_Basic:
                        index = "Variables(" + comilla + exp.Key + comilla + ")";
                        break;
                    case LanguageScript.C_Plus_Plus:
                        index = "Variables[L" + comilla + exp.Key + comilla + "]";
                        break;
                    default:
                        index = "Variables[" + comilla + exp.Key + comilla + "]";
                        break;
                }

                foreach (KeyValuePair<string, object> variable in exp.Value)
                {
                    CodeVariableReferenceExpression field1 = new CodeVariableReferenceExpression(exp.Key + "_" + variable.Key);

                    CodeArgumentReferenceExpression arg = new CodeArgumentReferenceExpression(index);

                    CodeMethodInvokeExpression cmie = new CodeMethodInvokeExpression(
                        new CodeTypeReferenceExpression("Convert"), "ChangeType", new CodeExpression[]{
                        new CodeArrayIndexerExpression(arg, new CodePrimitiveExpression(variable.Key)), 
                        new CodeTypeOfExpression(new CodeTypeReference(variable.Value.GetType()))});

                    CodeAssignStatement assin = new CodeAssignStatement(field1,
                        new CodeCastExpression(variable.Value.GetType(),
                            cmie));
                    Method.Statements.Add(assin);
                }
            }

            Method.Statements.Add(codeSnippet);
            foreach (KeyValuePair<string, Dictionary<string, object>> exp in Exp_Vars)
                foreach (KeyValuePair<string, object> variable in exp.Value)
                {
                    string index = "";
                    switch (Language)
                    {
                        case LanguageScript.C_Sharp:
                            index = "Variables[" + comilla + exp.Key + comilla + "]";
                            break;
                        case LanguageScript.Visual_Basic:
                            index = "Variables(" + comilla + exp.Key + comilla + ")";
                            break;
                        case LanguageScript.C_Plus_Plus:
                            index = "Variables[L" + comilla + exp.Key + comilla + "]";
                            break;
                        default:
                            index = "Variables[" + comilla + exp.Key + comilla + "]";
                            break;
                    }
                    CodeVariableReferenceExpression field1 = new CodeVariableReferenceExpression(exp.Key + "_" + variable.Key);
                    CodeArgumentReferenceExpression arg = new CodeArgumentReferenceExpression(index);
                    CodeAssignStatement assin = new CodeAssignStatement(new CodeArrayIndexerExpression(arg,
                        new CodePrimitiveExpression(variable.Key)), field1);
                    Method.Statements.Add(assin);
                }
            _ClassTemp.Members.Add(Method);


            CodeEntryPointMethod start = new CodeEntryPointMethod();
            _ClassTemp.Members.Add(start);

            //IndentedTextWriter tw = new IndentedTextWriter(new StreamWriter("Script//" + NameClassScript, false), "    ");
            //// Generate source code using the code generator.

            //codeProvider.GenerateCodeFromCompileUnit(compileUnit, tw, new CodeGeneratorOptions());
            //// Close the output file.
            //tw.Close();

            //string PathDir = Environment.CurrentDirectory + "\\Script\\" + NameClassScript + ".dll";

            CompilerParameters parameters = new CompilerParameters
                                                {
                                                    GenerateInMemory = true
                                                };

            CompilerResults results = codeProvider.CompileAssemblyFromDom(parameters, new CodeCompileUnit[] { compileUnit });

            if (results.Errors.Count > 0)
            {
                var errorMessage = new StringBuilder();
                foreach (CompilerError error in results.Errors)
                {
                    errorMessage.AppendFormat("{0} {1}", error.Line,
                    error.ErrorText);
                }
                returnData = errorMessage.ToString();
            }
            else
            {
                _Assembly = results.CompiledAssembly;
                classType = _Assembly.GetType("Tempur." + NameClassScript); ;

                object o = classType.InvokeMember(null,
                    BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic |
            BindingFlags.Instance | BindingFlags.CreateInstance, null, null, null);

                classType.InvokeMember("MethodScript", BindingFlags.DeclaredOnly |
            BindingFlags.Public | BindingFlags.Instance | BindingFlags.InvokeMethod,
                    null, o, new object[] { Exp_Vars });
            }

            return returnData;
        }

        private string _NameScript = null;
        public void Execute(ref Dictionary<string, Dictionary<string, object>> Exp_Vars)
        {
            if (classType != null)
            {
                //    object o = classType.InvokeMember(null,
                //        BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic |
                //BindingFlags.Instance | BindingFlags.CreateInstance, null, null, null);
                object o = Activator.CreateInstance(classType);

                classType.InvokeMember("MethodScript", BindingFlags.DeclaredOnly |
            BindingFlags.Public | BindingFlags.Instance | BindingFlags.InvokeMethod,
                    null, o, new object[] { Exp_Vars });
                // MethodInfo MethodScript = classType.GetMethod("MethodScript");




                //classType.InvokeMember("MethodScript", BindingFlags.InvokeMethod | BindingFlags.Public,
                //    null, null, new object[] { Exp_Vars });

                // MethodScript.Invoke(o, new object[] { Exp_Vars });
            }

            Assembly[] sasf = AppDomain.CurrentDomain.GetAssemblies();

        }


    }
}
