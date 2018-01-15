﻿using System;
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

        public string ApiVersion
        {
            get
            {
                return Map.ApiVersion;
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
            bool hasPrerequisites = false;
            string prerequisites = Map.Modules[_selectedMethod].TestPrerequisitesModule;
            if ((prerequisites != null) && (prerequisites != ""))
                hasPrerequisites = true;

            if ((Map.Modules[_selectedMethod].TestPrerequisites != null) || (Map.Modules[_selectedMethod].TestPostrequisites != null))
                hasPrerequisites = true;

            return hasPrerequisites;
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

        //---------------------------------------------------------------------------------------------------------------------------------------
        // Return module options as module_arg_spec
        //---------------------------------------------------------------------------------------------------------------------------------------
        public string[] GetModuleArgSpec(bool appendState)
        {
            var argSpec = new List<string>();

            for (int i = 0; i < ModuleOptions.Length; i++)
            {
                var option = ModuleOptions[i];
                bool defaultOrRequired = (option.DefaultValue != null) || (option.Required == "True");
                bool choices = (option.EnumValues != null) && option.EnumValues.Length > 0;
                argSpec.Add(option.NameAlt + "=dict(");
                argSpec.Add("    type='" + (option.IsList ? "list" : option.Type) + "'" + ((option.NoLog || defaultOrRequired || choices) ? "," : ""));

                if (option.NoLog)
                {
                    argSpec.Add("    no_log=True" + ((defaultOrRequired || choices) ? "," : ""));
                }

                if (choices)
                {
                    string choicesList = "    choices=[";

                    for (int ci = 0; ci < option.EnumValues.Length; ci++)
                    {
                        choicesList += "'" + option.EnumValues[ci].Key + "'";

                        if (ci < option.EnumValues.Length - 1)
                        {
                            choicesList += ",";
                        }
                        else
                        {
                            choicesList += "]" + (defaultOrRequired ? "," : "");
                        }
                        argSpec.Add(choicesList);
                        choicesList = "             ";
                    }
                }

                if (defaultOrRequired)
                {
                    if (option.DefaultValue != null)
                    {
                        argSpec.Add("    default=" + option.DefaultValue);
                    }
                    else
                    {
                        argSpec.Add("    required=" + option.Required);
                    }
                }

                argSpec.Add(")" + ((i < ModuleOptions.Length - 1 || appendState) ? "," : ""));
            }

            if (appendState)
            {
                argSpec.Add("state=dict(");
                argSpec.Add("    type='str',");
                argSpec.Add("    default='present',");
                argSpec.Add("    choices=['present', 'absent']");
                argSpec.Add(")");
            }

            return argSpec.ToArray();
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
                    variables.Add(prefix + " key == \"" + option.NameAlt + "\":");

                    string[] path = option.Disposition.Split(":");
                    string variable = "self." + (path[0].EndsWith("_parameters") ? "parameters" : path[0]);

                    if (path.Length > 1)
                    {
                        for (int i = 1; i < path.Length; i++)
                            variable += ".setdefault(\"" + path[i] + "\", {})";
                    }

                    variable += "[\"" + option.Name + "\"] = ";

                    if (option.ValueIfFalse != null && option.ValueIfTrue != null)
                    {
                        variable += "'" + option.ValueIfTrue + "' if kwargs[key] else '" + option.ValueIfFalse + "'";
                    }
                    else
                    {
                        var valueTranslation = new List<string>();
                        string valueTranslationPrefix = "if";

                        if (option.EnumValues != null && option.EnumValues.Length > 0)
                        {
                            // if option contains enum value, check if it has to be translated
                            valueTranslation.Add("    ev = kwargs[key]");
                            foreach (var enumValue in option.EnumValues)
                            {
                                if (enumValue.Key != enumValue.Value)
                                {
                                    valueTranslation.Add("    " + valueTranslationPrefix + " ev == '" + enumValue.Key + "':");
                                    valueTranslation.Add("        ev = '" + enumValue.Value + "'");
                                    valueTranslationPrefix = "elif";
                                }
                            }
                        }
                        else if (option.SubOptions != null)
                        {
                            bool evDeclarationAdded = false;
                            foreach (var suboption in option.SubOptions)
                            {
                                if (suboption.EnumValues != null && suboption.EnumValues.Length > 0)
                                {
                                    bool ifStatementAdded = false;
                                    valueTranslationPrefix = "if";

                                    foreach (var enumValue in suboption.EnumValues)
                                    {
                                        if (enumValue.Key != enumValue.Value)
                                        {
                                            if (!evDeclarationAdded)
                                            {
                                                valueTranslation.Add("    ev = kwargs[key]");
                                                evDeclarationAdded = true;
                                            }

                                            if (!ifStatementAdded)
                                            {
                                                valueTranslation.Add("    if '" + suboption.NameAlt + "' in ev:");
                                                ifStatementAdded = true;
                                            }
                                            valueTranslation.Add("        " + valueTranslationPrefix + " ev['" + suboption.NameAlt + "'] == '" + enumValue.Key + "':");
                                            valueTranslation.Add("            ev['" + suboption.NameAlt + "'] = '" + enumValue.Value + "'");
                                            valueTranslationPrefix = "elif";
                                        }
                                    }
                                }
                            }
                            // XXX - not handling lists yet
                            // check 
                        }

                        if (valueTranslation.Count > 1)
                        {
                            variables.AddRange(valueTranslation);
                            variable += "ev";
                        }
                        else
                        {
                            variable += "kwargs[key]";
                        }
                    }

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

        public string[] ModuleFactsReturnResponseFields
        {
            get
            {
                List<string> help = new List<string>();
                help.Add(ModuleOperationName + ":");
                help.Add("    description: A list of dict results where the key is the name of the " + ObjectName + " and the values are the facts for that " + ObjectName + ".");
                help.Add("    returned: always");
                help.Add("    type: complex");
                help.Add("    contains:");
                help.Add("        " + ObjectNamePythonized + "_name:");
                help.Add("            description: The key is the name of the server that the values relate to.");
                help.Add("            type: complex");
                help.Add("            contains:");
                help.AddRange(GetHelpFromResponseFields(ModuleResponseFields, "                "));
                return help.ToArray();
            }
        }

        public string[] ModuleExamples
        {
            get
            {
                return GetPlaybook("Create (or update)", ModuleOptions, "  ", "default");
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

                    help.AddRange(GetPlaybook(method.Name == "get" ? "Get instance of" : "List instances of", options, "  ", "default"));
                }

                return help.ToArray();
            }
        }

        public string[] GetModuleTestCreate(bool isCheckMode = false)
        {
           return GetModuleTest(0, "Create instance of", "", isCheckMode);
        }

        public string[] ModuleTestUpdate
        {
            get { return GetModuleTest(0, "Create again instance of", "", false); }
        }

        public string[] ModuleTestUpdateCheckMode
        {
            get { return GetModuleTest(0, "Create again instance of", "", true); }
        }

        public string[] GetModuleTestDelete(bool isUnexistingInstance, bool isCheckMode)
        {
            string prefix = isUnexistingInstance ? "Delete unexisting instance of" : "Delete instance of";
            return GetModuleTest(0, prefix, "delete", isCheckMode);
        }

        public int GetModuleFactTestCount()
        {
            var m = GetModuleMap(ModuleName);
            return m.Methods.Length;
        }

        public string[] GetModuleFactTest(int idx)
        {
            var m = GetModuleMap(ModuleName);
            return GetModuleTest(0, "Gather facts", m.Methods[idx].Name, false);
        }

        public string[] ModuleTestDelete
        {
            get { return GetModuleTest(0, "Delete instance of", "delete", false); }
        }

        public string[] ModuleTestDeleteCheckMode
        {
            get { return GetModuleTest(0, "Delete instance of", "delete", true); }
        }

        public string[] ModuleTestDeleteUnexisting
        {
            get { return GetModuleTest(0, "Delete unexisting instance of", "delete", false); }
        }

        public string[] ModuleTestPrerequisites
        {
            get
            {
                List<string> prePlaybook = new List<string>();
                string prerequisites = Map.Modules[_selectedMethod].TestPrerequisitesModule;

                if ((prerequisites != null) && (prerequisites != ""))
                {
                    var subModel = new CodeModelAnsibleMap(Map, null, prerequisites);
                    prePlaybook.AddRange(subModel.ModuleTestPrerequisites);
                    prePlaybook.AddRange(subModel.GetModuleTest(1, "Create", "", false));
                }

                if (Map.Modules[_selectedMethod].TestPrerequisites != null)
                    prePlaybook.AddRange(Map.Modules[_selectedMethod].TestPrerequisites);

                return prePlaybook.ToArray();
            }
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

                    if (subModel.CanDelete())
                    {
                        prePlaybook.AddRange(subModel.ModuleTestDelete);
                    }

                    prePlaybook.AddRange(subModel.ModuleTestDeleteClearPrerequisites);
                }

                if (Map.Modules[_selectedMethod].TestPostrequisites != null)
                    prePlaybook.AddRange(Map.Modules[_selectedMethod].TestPostrequisites);

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

        public string FindResourceNameInTest()
        {
            var m = GetModuleMap(ModuleName);

            string fieldName = m.ResourceNameFieldInRequest;
            
            foreach (var o in ModuleOptions)
            {
                if (o.Name == fieldName)
                {
                    string name = o.DefaultValueSample.GetValueOrDefault("test:default", "");

                    if (name != "")
                    {
                        if (name.StartsWith("\""))
                            name = name.Substring(1, name.Length - 2);

                        return name;
                    }
                }
            }

            return "xxxunknownxxx";
        }

        private string[] GetModuleTest(int level, string testType, string methodType, bool isCheckMode)
        {
            List<string> prePlaybook = new List<string>();
            string postfix = isCheckMode ? " -- check mode" : "";

            prePlaybook.AddRange(GetPlaybook(testType, ((methodType == "") ? ModuleOptions : GetMethodOptions(methodType)), "", "test:default", postfix));

            if (methodType == "delete")
                prePlaybook.Add("    state: absent");

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
                    case "NetworkManagementClient": return "azure.mgmt.network";
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

        public bool CanTestUpdate()
        {
            var m = GetModuleMap(ModuleName);

            return m.CannotTestUpdate;
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

                    // XXX - this is a hack, can we unhack it?
                    if (optionName.EndsWith("_parameters"))
                        optionName = "parameters";

                    if (line.EndsWith("("))
                    {
                        line += p + "=self." + optionName;
                    }
                    else
                    {
                        line += ",";
                        response.Add(line);
                        line = indent + p + "=self." + optionName;
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

        //---------------------------------------------------------------------------------------------------------------------------------
        // PLAYBOOK RELATED FUNCTIONALITY
        //---------------------------------------------------------------------------------------------------------------------------------
        // Used for:
        //  - sample generation
        //  - test generation
        //---------------------------------------------------------------------------------------------------------------------------------
        private string[] GetPlaybook(string operation, ModuleOption[] options, string padding, string playbookType, string operationPostfix = "")
        {
            List<string> help = new List<string>();

            help.Add(padding + "- name: " + operation + " " + ObjectName + operationPostfix);
            help.Add(padding + "  " + ModuleNameAlt + ":");
            help.AddRange(GetPlaybookFromOptions(options, padding + "    ", playbookType));
            return help.ToArray();
        }

        private string[] GetPlaybookFromOptions(ModuleOption[] options, string padding, string playbookType)
        {
            List<string> help = new List<string>();
            foreach (var option in options)
            {
                // XXX - this should not be necessary
                if (option == null)
                    continue;

                string propertyLine = padding + option.NameAlt + ":";

                //CompositeTypePy submodel = GetModelTypeByName(option.ModelTypeName);

                if (option.IsList)
                {
                    if (option.Type == "dict")
                    {
                        string[] sub = GetPlaybookFromOptions(option.SubOptions, "", playbookType);

                        if (sub.Length > 0)
                        {
                            help.Add(propertyLine);

                            bool first = true;
                            foreach (var line in sub)
                            {
                                help.Add((first ? padding + "  - " : padding + "    ") + line);
                                first = false;
                            }
                        }
                    }
                    else if (option.Type == "list")
                    {
                        //help.Add(padding + "  - " + "XXXX - list of lists -- not implemented " + option.Type);
                    }
                    else
                    {
                        //help.Add(padding + "  - " + "XXXX - list of values -- not implemented " + option.Type);
                        string predefined = option.DefaultValueSample.GetValueOrDefault(playbookType, null);

                        if (predefined != "")
                        {
                            bool first = true;
                            help.Add(propertyLine);
                            help.Add((first ? padding + "  - " : padding + "    ") + predefined);
                        }
                    }
                }
                else if (option.SubOptions != null && option.SubOptions.Length > 0)
                {
                    string[] sub = GetPlaybookFromOptions(option.SubOptions, padding + "  ", playbookType);

                    if (sub.Length > 0)
                    {
                        help.Add(propertyLine);
                        help.AddRange(sub);
                    }
                }
                else
                {
                    string predefined = option.DefaultValueSample.GetValueOrDefault(playbookType, null);

                    if (predefined != "")
                    {
                        help.Add(propertyLine + " " + ((predefined != "") ? predefined : " " + option.Name + ""));
                    }
                }
            }

            return help.ToArray();
        }

        //---------------------------------------------------------------------------------------------------------------------------------
        // DOCUMENTATION GENERATION FUNCTIONALITY
        //---------------------------------------------------------------------------------------------------------------------------------
        // Use it to generate module documentation
        //---------------------------------------------------------------------------------------------------------------------------------
        private string[] GetHelpFromOptions(ModuleOption[] options, string padding)
        {
            List<string> help = new List<string>();
            foreach (var option in options)
            {
                // check if option should be included in documentation
                if (!option.IncludeInDocumentation)
                    continue;

                string doc = NormalizeString(option.Documentation);
                help.Add(padding + option.NameAlt + ":");
                help.Add(padding + "    description:");

                help.AddRange(WrapString(padding + "        - ", doc));

                // write only if true
                if (option.Required != "False")
                {
                    help.Add(padding + "    required: " + option.Required);
                }

                // right now just add type if option is a list or bool
                if (option.IsList || option.Type == "bool")
                {
                    help.Add(padding + "    type: " + (option.IsList ? "list" : option.Type));
                }

                if (option.DefaultValue != null)
                {
                    help.Add(padding + "    default: " + option.DefaultValue);
                }

                if (option.EnumValues != null && option.EnumValues.Length > 0)
                {
                    //string line = padding + "    choices: [";
                    string line = padding + "    choices:";
                    help.Add(line);
                    for (int i = 0; i < option.EnumValues.Length; i++)
                    {
                        line = padding + "        - '" + option.EnumValues[i].Key + "'";
                        help.Add(line);
                        //line += "'" + option.EnumValues[i].Key + "'" + ((i < option.EnumValues.Length - 1) ? ", " : "]");
                    }
                    //help.Add(line);
                }

                if (option.SubOptions != null && option.SubOptions.Length > 0)
                {
                    help.Add(padding + "    suboptions:");
                    help.AddRange(GetHelpFromOptions(option.SubOptions, padding + "        "));
                }
            }

            return help.ToArray();
        }

        public string[] DeleteResponseNoLogFields
        {
            get
            {
                return GetDeleteResponseNoLogFields(ModuleResponseFields, "response");
            }
        }

        private string[] GetDeleteResponseNoLogFields(ModuleResponseField[] fields, string responseDict)
        {
            List<string> statements = new List<string>();

            foreach (var field in fields)
            {
                if (field.NameAlt == "nl")
                {
                    string statement = responseDict + ".pop('" + field.Name + "', None)";
                    statements.Add(statement);
                }
                else
                {
                    // XXX - not for now
                    //if (field.SubFields != null)
                    //{
                    //    statements.AddRange(GetExcludedResponseFieldDeleteStatements(field.SubFields, responseDict + "[" + field.Name + "]"));
                    //}
                }
            }

            return statements.ToArray();
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

        public string[] ResponseFieldStatements
        {
            get
            {
                return GetResponseFieldStatements(ModuleResponseFields, "self.results");
            }
        }

        private string[] GetResponseFieldStatements(ModuleResponseField[] fields, string responseDict)
        {
            List<string> statements = new List<string>();

            foreach (var field in fields)
            {
                if (field.NameAlt != "" && field.NameAlt.ToLower() != "x" && field.NameAlt.ToLower() != "nl")
                {
                    string statement = responseDict + "[\"" + field.NameAlt + "\"] = response[\"" + field.Name + "\"]";
                    statements.Add(statement);
                }
                else
                {
                    // XXX - no need now
                    //if (field.SubFields != null)
                    //{
                    //    statements.AddRange(GetExcludedResponseFieldDeleteStatements(field.SubFields, responseDict + "[" + field.Name + "]"));
                    //}
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
                    if (level == 0)
                        continue;

                    // if it's level 1 parameter flattened to level 0
                    if (level == 1 && option.Disposition.Contains(":"))
                        continue;

                    // ignore renaming at level 0 as these parameters are already flattened and treated differently
                    if (level > 0)
                    {
                        string statement = statementPrefix + "self.rename_key(" + dictPrefix + ", '" + option.NameAlt + "', '" + option.Name + "')";
                        statements.Add(statement);
                    }
                }
                else
                {
                    // XXX - if option disposition starts with ":" it should be placed in a dictionary with such name
                    if (option.Disposition.StartsWith(":"))
                    {
                        statements.Add("# option " + option.NameAlt + " must go to dictionary " + option.Disposition + " as " + option.Name);
                    }
                    else if (option.SubOptions != null)
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

            if (fields != null)
            {
                foreach (var field in fields)
                {
                    // setting nameAlt to empty or "x" will remove the field
                    if (field.NameAlt == "" || field.NameAlt.ToLower() == "x" || field.NameAlt.ToLower() == "nl")
                        continue;

                    string doc = NormalizeString(field.Description);
                    help.Add(padding + field.NameAlt + ":");
                    help.Add(padding + "    description:");
                    help.AddRange(WrapString(padding + "        - ", doc));

                    help.Add(padding + "    returned: " + field.Returned);
                    help.Add(padding + "    type: " + field.Type);

                    help.AddRange(WrapString(padding + "    sample: ", field.SampleValue));

                    if (field.SubFields != null && field.SubFields.Length > 0)
                    {
                        help.Add(padding + "    contains:");
                        help.AddRange(GetHelpFromResponseFields(field.SubFields, padding + "        "));
                    }
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
                    if (!rule.Option[0].EndsWith("]"))
                    {
                        rules.Add("if (self." + rule.Option[0] + " is not None) and (self." + rule.Option[0] + " != old_response['" + rule.ReturnField[0] + "']):");
                        rules.Add("    self.to_do = Actions.Update");
                    }
                    else
                    {
                        string[] temp = rule.Option[0].Split("['");
                        string prefix = temp[0];
                        temp = temp[1].Split("']");
                        string param = temp[0];
                        rules.Add("if ('" + param + "' in self." + prefix + ") and (self." + prefix + "['" + param + "'] != old_response['" + rule.ReturnField[0] + "']):");
                        rules.Add("    self.to_do = Actions.Update");
                    }
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

        private static string[] WrapString(string prefix, string content)
        {
            List<string> output = new List<string>();

            if ((prefix.Length + content.Length <= 160) && (content.LastIndexOfAny("'\"\r\n:".ToCharArray()) == -1))
            {
                output.Add(prefix + content);
            }
            else
            {
                content = String.Join("\\n", content.Split(new[] { "\r", "\n", "\r\n" }, StringSplitOptions.None));
                content = String.Join("'", content.Split(new[] { "\"" }, StringSplitOptions.None));
                int chunkLength = 160 - (prefix.Length + 4);
                int currentIdx = 0;
                int docLength = content.Length;

                while (currentIdx < docLength)
                {
                    if (currentIdx + chunkLength > docLength) chunkLength = docLength - currentIdx;
                    string chunk = content.Substring(currentIdx, chunkLength);
                    if (currentIdx == 0)
                    {
                        output.Add(prefix + "\"" + chunk + ((docLength > chunkLength) ? "" : "\""));
                        prefix = new String(prefix.Select(r => ' ').ToArray());
                    }
                    else if (currentIdx + chunk.Length != docLength)
                    {
                        output.Add(prefix + chunk);
                    }
                    else
                    {
                        output.Add(prefix + chunk + "\"");
                    }
                    currentIdx += chunkLength;
                }
            }

            return output.ToArray();
        }

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

            // also replace '' with C()
            string normalized = new string(a);

            string[] ss = normalized.Split('\'');

            normalized = ss[0];
            // now merge and replace
            for (int i = 1; i < ss.Length; i += 2)
            {
                if (ss.Length > i + 1)
                {
                    normalized += "C(" + ss[i] + ")" + ss[i + 1];
                }
                else
                {
                    normalized += "'" + ss[i];
                }
            }

            return normalized;
        }

        public static string Indent(string original)
        {
            char[] a = original.ToCharArray();

            for (int i = 0; i < a.Length; i++) a[i] = ' ';

            return new string(a);
        }
    }
}
