using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;


namespace BioScadaScript
{
    public class ScriptInAppDomain
    {
        private AppDomain SecondDomain;
        private Script scr;

        private string code;
        public string Code { get { return code; } set { code = value; } }

        private string nameScript;
        public string NameScript { get { return nameScript; } set { nameScript = value; } }

        private string errors;
        public string Errors { get { return errors; } set { errors = value; } }

        private LanguageScript language;
        public LanguageScript Language { get { return language; } set { language = value; } }

        public ScriptInAppDomain(string code, string nameScript, LanguageScript language)
        {
            this.code = code;
            this.nameScript = nameScript;
            this.language = language;
        }

        public string CompileInAppDomain(Dictionary<string, Dictionary<string, object>> Exp_Vars)
        {
            if (SecondDomain != null)
                AppDomain.Unload(SecondDomain);
            SecondDomain = AppDomain.CreateDomain("ScriptDomain", null);

            scr = (Script)SecondDomain.CreateInstanceAndUnwrap(typeof(Script).Assembly.FullName, typeof(Script).FullName);

            errors = scr.CompileScript(ref Exp_Vars, code, nameScript, language);
            if (errors == null)
                errors = "Correct";

            return errors;
        }

        public void Execute(ref Dictionary<string, Dictionary<string, object>> Exp_Vars)
        {
            try
            {
                scr.Execute(ref Exp_Vars);    
            }catch{}
        }
    }
}
