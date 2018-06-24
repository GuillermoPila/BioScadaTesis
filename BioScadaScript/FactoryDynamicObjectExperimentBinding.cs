using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.ComponentModel;
using Microsoft.CSharp;

namespace BioScadaScript
{
    public class FactoryDynamicObjectExperimentBinding
    {
        public static  object CreateDynamicObjectExperimentBinding(string NameClassExperiment, Dictionary<string, object> vars)
        {
            object returnData = null;

            CodeDomProvider codeProvider = new CSharpCodeProvider();


            CodeCompileUnit compileUnit = new CodeCompileUnit();

            CodeNamespace _NameSpace = new CodeNamespace("Tempur");
            // Add the new namespace to the compile unit.
            compileUnit.Namespaces.Add(_NameSpace);

            // Add the new namespace import for the System namespace.
            _NameSpace.Imports.Add(new CodeNamespaceImport("System"));
            _NameSpace.Imports.Add(new CodeNamespaceImport("System.Collections.Generic"));
            _NameSpace.Imports.Add(new CodeNamespaceImport("System.Reflection"));
            _NameSpace.Imports.Add(new CodeNamespaceImport("System.ComponentModel"));


            CodeTypeDeclaration _ClassTemp = new CodeTypeDeclaration(NameClassExperiment);
            _ClassTemp.BaseTypes.Add(new CodeTypeReference(typeof(INotifyPropertyChanged)));
            // Add the new type to the namespace type collection.
            _NameSpace.Types.Add(_ClassTemp);

            foreach (KeyValuePair<string, object> variable in vars)
            {
                CodeMemberField field = new CodeMemberField(variable.Value.GetType(),"_"+variable.Key);
                field.Attributes = MemberAttributes.Private;
                CodeMemberProperty prop = new CodeMemberProperty();
                prop.Name = variable.Key;
                prop.Type = new CodeTypeReference(variable.Value.GetType());
                prop.Attributes = MemberAttributes.Public;
                prop.GetStatements.Add(new CodeMethodReturnStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"_"+variable.Key)));
                prop.SetStatements.Add(new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), "_" + variable.Key), new CodePropertySetValueReferenceExpression()));
                prop.SetStatements.Add(new CodeSnippetStatement("if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("+'"'+variable.Key+'"'+"));"));
                
                _ClassTemp.Members.Add(field);
                _ClassTemp.Members.Add(prop);
            }

            CodeMemberEvent even = new CodeMemberEvent();
            even.Name = "PropertyChanged";
            even.Attributes = MemberAttributes.Public;
            even.Type = new CodeTypeReference(typeof(PropertyChangedEventHandler));

            _ClassTemp.Members.Add(even);

            //IndentedTextWriter tw = new IndentedTextWriter(new StreamWriter(NameClassExperiment, false), "    ");
            //// Generate source code using the code generator.

            //codeProvider.GenerateCodeFromCompileUnit(compileUnit, tw, new CodeGeneratorOptions());
            //// Close the output file.
            //tw.Close();

            //string PathDir = Environment.CurrentDirectory + "\\Script\\" + NameClassScript + ".dll";

            CompilerParameters parameters = new CompilerParameters
            {
                GenerateInMemory = true
            };
            parameters.ReferencedAssemblies.Add("System.dll");

            CompilerResults results = codeProvider.CompileAssemblyFromDom(parameters, new CodeCompileUnit[] { compileUnit });

            if (results.Errors.Count > 0)
            {
                //var errorMessage = new StringBuilder();
                //foreach (CompilerError error in results.Errors)
                //{
                //    errorMessage.AppendFormat("{0} {1}", error.Line,
                //    error.ErrorText);
                //}
                returnData = null;
                //returnData = errorMessage.ToString();
            }
            else
            {
                Assembly _Assembly = results.CompiledAssembly;
                Type classType = _Assembly.GetType("Tempur." + NameClassExperiment); ;

                returnData = classType.InvokeMember(null,
                    BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic |
            BindingFlags.Instance | BindingFlags.CreateInstance, null, null, null);
                
            }

            return returnData;
        }
    }
}
