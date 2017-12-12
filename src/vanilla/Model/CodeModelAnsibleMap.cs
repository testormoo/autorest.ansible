using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using AutoRest.Core;
using AutoRest.Core.Model;
using AutoRest.Core.Utilities;
using AutoRest.Extensions;
using Newtonsoft.Json;
using System.IO;
using System.Text;

namespace AutoRest.Ansible.Model
{
    public class CodeModelAnsibleMap
    {
        public CodeModelAnsibleMap(MapAnsible map, string[] mergeReport, int method)
        {
            Map = map;
            MergeReport = mergeReport;

            _selectedMethod = method;
        }

        public CodeModelAnsibleMap(MapAnsible map, string[] mergeReport, string module)
        {
            Map = map;
            MergeReport = mergeReport;

            for (_selectedMethod = 0; _selectedMethod < Map.Modules.Length; _selectedMethod++)
            {
                if (Map.Modules[_selectedMethod].ModuleName == module)
                    break;
            }
        }
        public string[] MergeReport { get; set; }

        public string ModuleName
        {
            get
            {
                try
                {
                    return (_selectedMethod < Map.Modules.Length) ? Map.Modules[_selectedMethod].ModuleName : "INVALID";
                }
                catch (Exception e)
                {
                    return "EXCEPTION";
                }
            }
        }

        public string GetModuleReleaseStatus()
        {
            return (_selectedMethod < Map.Modules.Length) ? Map.Modules[_selectedMethod].ReleaseStatus : "";
        }

        public string ModuleNameAlt
        {
            get
            {
                try
                {
                    return (_selectedMethod < Map.Modules.Length) ? Map.Modules[_selectedMethod].ModuleNameAlt : "INVALID";
                }
                catch (Exception e)
                {
                    return "EXCEPTION";
                }
            }
        }

        public bool NeedsDeleteBeforeUpdate
        {
            get
            {
                return Map.Modules[_selectedMethod].NeedsDeleteBeforeUpdate;
            }
        }

        public bool HasResourceGroup()
        {
            return (Array.Find(ModuleOptions, element => (element.Name == "resource_group_name")) != null);
        }

        public bool HasPrerequisites()
        {
            string prerequisites = Map.Modules[_selectedMethod].TestPrerequisitesModule;
            return ((prerequisites != null) && (prerequisites != ""));
        }

        public string LocationDisposition
        {
            get
            {
                var location = Array.Find(ModuleOptions, e => (e.Name == "location"));
                if (location != null)
                {
                    return location.Disposition;
                }
                else
                {
                    return null;
                }
            }
        }

        private int _selectedMethod = 0;

        public ModuleOption[] ModuleOptions
        {
            get
            {
                var m = GetModuleMap(ModuleName);
                IEnumerable<ModuleOption> options = from option in m.Options where !option.Disposition.EndsWith("dictionary") select option;
                return options.ToArray();
            }
        }

        public ModuleResponseField[] ModuleResponseFields
        {
            get
            {
                var m = GetModuleMap(ModuleName);
                return m.ResponseFields;
            }
        }

        public ModuleOption[] ModuleOptionsUnflattened
        {
            get
            {
                var m = GetModuleMap(ModuleName);
                IEnumerable<ModuleOption> options = from option in m.Options where (option.Disposition == "dictionary") || (option.Disposition == "default") select option;
                return options.ToArray();
            }
        }

        public string[] ModuleTopLevelOptionsVariables
        {
            get
            {
                var variables = new List<string>();
                var m = GetModuleMap(ModuleName);
                IEnumerable<ModuleOption> options = from option
                                                    in m.Options
                                                    where option.Disposition == "dictionary" ||
                                                          option.Disposition.EndsWith(":dictionary") ||
                                                          option.Disposition == "default"
                                                    select option;
                foreach (var option in options)
                {
                    if (option.Disposition == "default" || option.Disposition == "dictionary")
                    {
                        variables.Add("self." + option.NameAlt + " = " + option.VariableValue);
                    }
                    else
                    {
                        // XXX - right now just supporting 2 levels
                        //string[] path = option.Disposition.Split(":");
                        //string variable = "self." + path[0] + "['" + option.NameAlt + "'] = dict()";
                        //variables.Add(variable);
                    }
                }

                return variables.ToArray();
            }
        }

        public string[] ModuleSecondLevelOptionsMapStatements
        {
            get
            {
                string prefix = "if";
                var variables = new List<string>();
                var m = GetModuleMap(ModuleName);
                ModuleOption[] options = ModuleOptionsSecondLevel;

                foreach (var option in options)
                {
                    string[] path = option.Disposition.Split(":");
                    string variable = "self." + path[0] + ".update(";

                    if (path.Length > 1)
                    {
                        for (int i = 1; i < path.Length; i++)
                            variable += "{\"" + path[i] + "\": ";
                    }

                    variable += "{\"" + option.Name + "\": kwargs[key]}";

                    if (path.Length > 1)
                    {
                        for (int i = 1; i < path.Length; i++)
                            variable += "}";
                    }

                    variable += ")";

                    variables.Add(prefix + " key == \"" + option.NameAlt + "\":");
                    variables.Add("    " + variable);
                    prefix = "elif";
                }
                    
                return variables.ToArray();
            }
        }


        public ModuleOption[] ModuleOptionsSecondLevel
        {
            get
            {
                var m = GetModuleMap(ModuleName);
                IEnumerable<ModuleOption> options = from option
                                                    in m.Options
                                                    where (option.Disposition != "dictionary") &&
                                                          (option.Disposition != "default") &&
                                                          (option.Disposition != "none") &&
                                                          (!option.Disposition.EndsWith(":dictionary"))
                                                    select option;
                return options.ToArray();
            }
        }

        public string[] ModuleHelp
        {
            get
            {
                return GetHelpFromOptions(ModuleOptions, "    ");
            }
        }

        public string[] ModuleReturnResponseFields
        {
            get
            {
                return GetHelpFromResponseFields(ModuleResponseFields, "");
            }
        }

        public string[] ModuleExamples
        {
            get
            {
                return GetPlaybook("Create (or update)", ModuleOptions, "  ", false);
            }
        }

        public string[] ModuleFactsExamples
        {
            get
            {
                List<string> help = new List<string>();
                foreach (var method in ModuleMethods)
                {
                    if (help.Count > 0) help.Add("");
                    // get parameters of method
                    ModuleOption[] options = GetMethodOptions(method.Name);

                    help.AddRange(GetPlaybook(method.Name == "get" ? "Get instance of" : "List instances of", options, "  ", false));
                }

                return help.ToArray();
            }
        }

        public string[] GetModuleTestCreate()
        {
           return GetModuleTest(0, "Create instance of", "");
        }

        public string[] ModuleTestUpdate
        {
            get { return GetModuleTest(0, "Update instance of", ""); }
        }

        public string[] ModuleTestDelete
        {
            get { return GetModuleTest(0, "Delete instance of", "delete"); }
        }

        public string[] ModuleTestDeleteClearPrerequisites
        {
            get
            {
                List<string> prePlaybook = new List<string>();

                string prerequisites = Map.Modules[_selectedMethod].TestPrerequisitesModule;

                if ((prerequisites != null) && (prerequisites != ""))
                {
                    var subModel = new CodeModelAnsibleMap(Map, null, prerequisites);
                    prePlaybook.AddRange(subModel.ModuleTestDelete);       
                    prePlaybook.AddRange(subModel.ModuleTestDeleteClearPrerequisites);
                }

                string[] arr = prePlaybook.ToArray();

                if (Map.Modules[_selectedMethod].TestReplaceStringFrom != null)
                {
                    for (int i = 0; i < arr.Length; i++)
                    {
                        arr[i] = arr[i].Replace(Map.Modules[_selectedMethod].TestReplaceStringFrom, Map.Modules[_selectedMethod].TestReplaceStringTo);
                    }
                }

                return arr;
            }
        }

        private string[] GetModuleTest(int level, string testType, string methodType)
        {
            List<string> prePlaybook = new List<string>();

            // XXX - remove this hack
            if (testType.StartsWith("Create"))
            {
                string prerequisites = Map.Modules[_selectedMethod].TestPrerequisitesModule;

                if ((prerequisites != null) && (prerequisites != ""))
                {
                    if (level <= 1)
                    {
                        var subModel = new CodeModelAnsibleMap(Map, null, prerequisites);
                        prePlaybook.AddRange(subModel.GetModuleTest(level + 1, "Create", ""));
                    }
                }
            }
            prePlaybook.AddRange(GetPlaybook(testType, ((methodType == "") ? ModuleOptions : GetMethodOptions(methodType)), "", true));

            string[] arr = prePlaybook.ToArray();

            if (Map.Modules[_selectedMethod].TestReplaceStringFrom != null)
            {
                for (int i = 0; i < arr.Length; i++)
                {
                    arr[i] = arr[i].Replace(Map.Modules[_selectedMethod].TestReplaceStringFrom, Map.Modules[_selectedMethod].TestReplaceStringTo);
                }
            }

            return arr;
        }

        public string MgmtClientImportPath
        {
            get
            {
                // this is fixed at the memoment, let's see how we can get it dynamically
                switch (Name)
                {
                    case "SqlManagementClient": return "azure.mgmt.sql";
                    case "MySQLManagementClient": return "azure.mgmt.rdbms.mysql";
                    case "PostgreSQLManagementClient": return "azure.mgmt.rdbms.postgresql";
                    default: return "azure.mgmt." + Namespace;
                }
            }
        }

        public string MgmtClientName
        {
            get
            {
                return Name;
            }
        }

        ModuleMethod GetMethod(string methodName)
        {
            var m = GetModuleMap(ModuleName);

            foreach (var method in m.Methods)
            {
                if (method.Name == methodName)
                    return method;
            }

            return null;
        }

        public bool HasCreateOrUpdate()
        {
            return GetMethod("create_or_update") != null;
        }

        public string[] GetMethodOptionNames(string methodName)
        {
            var m = GetModuleMap(ModuleName);

            foreach (var method in m.Methods)
            {
                if (method.Name == methodName)
                    return method.Options;
            }

            return null;
        }

        public bool CanDelete()
        {
            var m = GetModuleMap(ModuleName);

            foreach (var method in m.Methods)
            {
                if (method.Name == "delete")
                    return true;
            }

            return false;
        }

        public string[] GetMethodRequiredOptionNames(string methodName)
        {
            var m = GetModuleMap(ModuleName);

            foreach (var method in m.Methods)
            {
                if (method.Name == methodName)
                    return method.RequiredOptions;
            }

            return null;
        }

        public ModuleOption[] GetMethodOptions(string methodName)
        {
            string[] optionNames = GetMethodOptionNames(methodName);

            List<ModuleOption> moduleOptions = new List<ModuleOption>(ModuleOptions);

            return (from optionName in optionNames select moduleOptions.Find(element => (element.Name == optionName))).ToArray();
        }

        public string[] GetInfo
        {
            get
            {
                List<string> info = new List<string>();

                //foreach (var operation in Operations)
                //{
                //    info.Add("OPERATIONS: " + operation.Name);
                //    foreach (var method in operation.Methods)
                //    {
                //        info.Add("  METHOD: " + method.Name);

                //        foreach (var param in method.Parameters)
                //        {
                //            info.Add("    PARAMETER: " + param.Name);
                //        }
                //    }
                //}

                //info.Add("QUALIFIER: " + Qualifier);
                //info.Add("PACKAGE NAME: " + PackageName);
                //info.Add("NAME: " + Name);
                //info.Add("NAMESPACE: " + Namespace);
                //info.Add("NAMESPACE UPPER: " + NamespaceUpper);
                //info.Add("MODEL NAME: " + ModelsName);
                //info.Add("URL: " + BaseUrl);

                // just generate here for now
                //CreateMap();

                info.AddRange(JsonConvert.SerializeObject(Map, Formatting.Indented).Split(new[] { "\r", "\n", "\r\n" }, StringSplitOptions.None));

                return info.ToArray();
            }
        }

        public ModuleMethod[] ModuleMethods
        {
            get
            {
                return GetModuleMap(ModuleName).Methods;
            }
        }

        public string[] ModuleGenerateApiCall(string indent, string methodName)
        {
            // XXX - ModuleOperationName
            var response = new List<string>();
            string line = indent + "response = self.mgmt_client." + ModuleOperationName + "." + methodName + "(";
            indent = Indent(line);
            ModuleMethod method = GetMethod(methodName);

            if (method != null)
            {
                foreach (var p in method.RequiredOptions)
                {
                    var o = Array.Find(ModuleOptions, e => (e.Name == p));
                    string optionName = (o != null) ? o.NameAlt : p;

                    if (line.EndsWith("("))
                    {
                        line += "self." + optionName;
                    }
                    else
                    {
                        line += ",";
                        response.Add(line);
                        line = indent + "self." + optionName;
                    }
                }
            }

            line += ")";
            response.Add(line);

            return response.ToArray();
        }

        public string AssertStateVariable
        {
            get
            {
                return GetModuleMap(ModuleName).AssertStateVariable;
            }
        }

        public string AssertStateExpectedValue
        {
            get
            {
                return GetModuleMap(ModuleName).AssertStateExpectedValue;
            }
        }

        public string Namespace
        {
            get
            {
                return Map.Namespace;
            }
        }
        public string NamespaceUpper
        {
            get
            {
                return Map.NamespaceUpper;
            }
        }
        public string ModuleOperationNameUpper
        {
            get
            {
                return GetModuleMap(ModuleName).ModuleOperationNameUpper;
            }
        }
        public string ModuleOperationName
        {
            get
            {
                return GetModuleMap(ModuleName).ModuleOperationName;
            }
        }
        public string ObjectName
        {
            get
            {
                return GetModuleMap(ModuleName).ObjectName;
            }
        }

        public string ObjectNamePythonized
        {
            get
            {
                return String.Join("", GetModuleMap(ModuleName).ObjectName.ToLower().Split(' '));
            }
        }

        public string Name
        {
            get
            {
                return Map.Name;
            }
        }
        public string ModuleResourceGroupName = "resource_group";

        public string ModuleResourceName
        {
            get
            {
                string name = "";

                try
                {
                    name = GetMethod("get").RequiredOptions[GetMethod("get").Options.Length - 1];
                }
                catch (Exception)
                {
                    try
                    {
                        name = GetMethod("delete").Options[GetMethod("delete").Options.Length - 1];
                    }
                    catch (Exception) { }
                }
                var o = Array.Find(ModuleOptions, e => (e.Name == name));
                name = (o != null) ? o.NameAlt : name;

                return name;
            }
        }
        //-----------------------------------------------------------------------------------------------------------------------------------------------------------
        // PRIVATE MAP ACCESS IMPLEMENTATION
        //-----------------------------------------------------------------------------------------------------------------------------------------------------------

        private MapAnsibleModule GetModuleMap(string name)
        {
            foreach (var m in Map.Modules)
                if (m.ModuleName == name)
                    return m;
            return null;
        }

        public MapAnsible Map { get; set; } 

        private string[] GetPlaybook(string operation, ModuleOption[] options, string padding, bool test)
        {
            List<string> help = new List<string>();

            help.Add(padding + "- name: " + operation + " " + ObjectName);
            help.Add(padding + "  " + ModuleNameAlt + ":");
            help.AddRange(GetPlaybookFromOptions(options, padding + "    ", test));
            return help.ToArray();
        }

        private string[] GetPlaybookFromOptions(ModuleOption[] options, string padding, bool test)
        {
            List<string> help = new List<string>();
            foreach (var option in options)
            {
                string propertyLine = padding + option.NameAlt + ":";

                //CompositeTypePy submodel = GetModelTypeByName(option.ModelTypeName);

                if (option.IsList)
                {
                    help.Add(propertyLine);

                    if (option.Type == "dict")
                    {
                        bool first = true;
                        foreach (var line in GetPlaybookFromOptions(option.SubOptions, "", test))
                        {
                            help.Add((first ? padding + "  - " : padding + "    ") + line);
                            first = false;
                        }
                    }
                    else if (option.Type == "list")
                    {
                        help.Add(padding + "  - " + "XXXX - list of lists -- not implemented " + option.Type);
                    }
                    else
                    {
                        help.Add(padding + "  - " + "XXXX - list of values -- not implemented " + option.Type);
                    }
                }
                else if (option.SubOptions != null && option.SubOptions.Length > 0)
                {
                    string[] sub = GetPlaybookFromOptions(option.SubOptions, padding + "  ", test);

                    if (sub.Length > 0)
                    {
                        help.Add(propertyLine);
                        help.AddRange(sub);
                    }
                }
                else
                {
                    string predefined = test ? option.DefaultValueTest : option.DefaultValueSample;

                    if (predefined != "")
                    {
                        help.Add(propertyLine + " " + ((predefined != "") ? predefined : " " + option.Name + ""));
                    }
                }
            }

            return help.ToArray();
        }

        private string[] GetHelpFromOptions(ModuleOption[] options, string padding)
        {
            List<string> help = new List<string>();
            foreach (var option in options)
            {
                string doc = NormalizeString(option.Documentation);
                help.Add(padding + option.NameAlt + ":");
                help.Add(padding + "    description:");
                int indent = (padding + "        - ").Length;

                if ((indent + doc.Length <= 160) && (doc.LastIndexOfAny("'\"\r\n".ToCharArray()) == -1))
                {
                    help.Add(padding + "        - " + doc);
                }
                else
                {
                    doc = String.Join("\\n", doc.Split(new[] { "\r", "\n", "\r\n" }, StringSplitOptions.None));
                    doc = String.Join("'", doc.Split(new[] { "\"" }, StringSplitOptions.None));
                    int chunkLength = 160 - (indent + 4);
                    int currentIdx = 0;
                    int docLength = doc.Length;

                    while (currentIdx < docLength)
                    {
                        if (currentIdx + chunkLength > docLength) chunkLength = docLength - currentIdx;
                        string chunk = doc.Substring(currentIdx, chunkLength);
                        if (currentIdx == 0)
                        {
                            help.Add(padding + "        - \"" + chunk + ((docLength > chunkLength) ? "" : "\""));
                        }
                        else if (currentIdx + chunk.Length != docLength)
                        {
                            help.Add(padding + "           " + chunk);
                        }
                        else
                        {
                            help.Add(padding + "           " + chunk + "\"");
                        }
                        currentIdx += chunkLength;
                    }
                }

                // write only if true
                if (option.Required != "False")
                {
                    help.Add(padding + "    required: " + option.Required);
                }

                if (option.SubOptions != null && option.SubOptions.Length > 0)
                {
                    help.Add(padding + "    suboptions:");
                    help.AddRange(GetHelpFromOptions(option.SubOptions, padding + "        "));
                }
            }

            return help.ToArray();
        }

        public string[] DeleteResponseFieldStatements
        {
            get
            {
                return GetExcludedResponseFieldDeleteStatements(ModuleResponseFields, "self.results");
            }
        }

        private string[] GetExcludedResponseFieldDeleteStatements(ModuleResponseField[] fields, string responseDict)
        {
            List<string> statements = new List<string>();

            foreach (var field in fields)
            {
                if (field.NameAlt == "" || field.NameAlt.ToLower() == "x")
                {
                    string statement = responseDict + ".pop('" + field.Name + "', None)";
                    statements.Add(statement);
                }
                else
                {
                    if (field.SubFields != null)
                    {
                        statements.AddRange(GetExcludedResponseFieldDeleteStatements(field.SubFields, responseDict + "[" + field.Name + "]"));
                    }
                }
            }

            return statements.ToArray();
        }

        public string[] FixParameterStatements
        {
            get
            {
                return GetFixParameterStatements(ModuleOptionsSecondLevel, 0, "", "self.parameters");
            }
        }

        private string[] GetFixParameterStatements(ModuleOption[] options, int level, string statementPrefix, string dictPrefix)
        {
            List<string> statements = new List<string>();

            foreach (var option in options)
            {
                if (option.NameAlt != option.Name)
                {
                    // ignore renaming at level 0 as these parameters are already flattened and treated differently
                    if (level > 0)
                    {
                        string statement = statementPrefix + "self.rename_key(" + dictPrefix + ", '" + option.NameAlt + "', '" + option.Name + "')";
                        statements.Add(statement);
                    }
                }
                else
                {
                    if (option.SubOptions != null)
                    {
                        string[] subStatements = GetFixParameterStatements(option.SubOptions, level + 1, statementPrefix + "    ", dictPrefix + "['" + option.NameAlt + "']");

                        if (subStatements.Length > 0)
                        {
                            statements.Add("if " + dictPrefix + ".get('" + option.NameAlt + "', None) is not None:");
                            statements.AddRange(subStatements);
                        }
                    }
                }
            }

            return statements.ToArray();
        }

        private string[] GetHelpFromResponseFields(ModuleResponseField[] fields, string padding)
        {
            List<string> help = new List<string>();
            foreach (var field in fields)
            {
                // setting nameAlt to empty or "x" will remove the field
                if (field.NameAlt == "" || field.NameAlt.ToLower() == "x")
                    continue;

                string doc = NormalizeString(field.Description);
                help.Add(padding + field.NameAlt + ":");
                help.Add(padding + "    description:");
                int indent = (padding + "        - ").Length;

                if ((indent + doc.Length <= 160) && (doc.LastIndexOfAny("'\"\r\n".ToCharArray()) == -1))
                {
                    help.Add(padding + "        - " + doc);
                }
                else
                {
                    doc = String.Join("\\n", doc.Split(new[] { "\r", "\n", "\r\n" }, StringSplitOptions.None));
                    doc = String.Join("'", doc.Split(new[] { "\"" }, StringSplitOptions.None));
                    int chunkLength = 160 - (indent + 4);
                    int currentIdx = 0;
                    int docLength = doc.Length;

                    while (currentIdx < docLength)
                    {
                        if (currentIdx + chunkLength > docLength) chunkLength = docLength - currentIdx;
                        string chunk = doc.Substring(currentIdx, chunkLength);
                        if (currentIdx == 0)
                        {
                            help.Add(padding + "        - \"" + chunk + ((docLength > chunkLength) ? "" : "\""));
                        }
                        else if (currentIdx + chunk.Length != docLength)
                        {
                            help.Add(padding + "           " + chunk);
                        }
                        else
                        {
                            help.Add(padding + "           " + chunk + "\"");
                        }
                        currentIdx += chunkLength;
                    }
                }

                help.Add(padding + "    returned: " + field.Returned);
                help.Add(padding + "    type: " + field.Type);
                help.Add(padding + "    sample: " + field.SampleValue);

                if (field.SubFields != null && field.SubFields.Length > 0)
                {
                    help.Add(padding + "    suboptions:");
                    help.AddRange(GetHelpFromResponseFields(field.SubFields, padding + "        "));
                }
            }

            return help.ToArray();
        }

        public string[] GetUpdateCheckRules()
        {
            List<string> rules = new List<string>();
            MapAnsibleModule map = GetModuleMap(ModuleName);

            if (null != map.UpdateComparisonRules)
            {
                foreach (var rule in map.UpdateComparisonRules)
                {
                    rules.Add("if (self." + rule.Option[0] + " is not None) and (self." + rule.Option[0] + " != old_response['" + rule.ReturnField[0] + "']):");
                    rules.Add("    self.to_do = Actions.Update");
                }
            }
            else
            {
                rules.Add("self.to_do = Actions.Update");
            }

            return rules.ToArray();
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------------
        // HELPERS
        //-----------------------------------------------------------------------------------------------------------------------------------------------------------

        private static string NormalizeString(string s)
        {
            char[] a = s.ToCharArray();
            int l = a.Length;
            for (int i = 0; i < l; i++)
            {
                switch (a[i])
                {
                    case (char)8216:
                    case (char)8217:
                        a[i] = '\'';
                        break;
                    case (char)8220:
                    case (char)8221:
                        a[i] = '"';
                        break;
                }
            }

            return new string(a);
        }

        public static string Indent(string original)
        {
            char[] a = original.ToCharArray();

            for (int i = 0; i < a.Length; i++) a[i] = ' ';

            return new string(a);
        }
    }
}
