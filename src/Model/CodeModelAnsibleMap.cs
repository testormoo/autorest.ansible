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
using System.Text.RegularExpressions;

namespace AutoRest.Ansible.Model
{
    public class CodeModelAnsibleMap
    {
        public CodeModelAnsibleMap(MapAnsible map, string[] mergeReport, int method)
        {
            Map = map;
            MergeReport = mergeReport;
            IsSnakeToCamelNeeded = false;

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

        public string[] MetadataTemplate
        {
            get
            {
                List<string> template = new List<string>();

                AppendMetadataTemplateForModules(template);
                return template.ToArray();
            }
        }

        private void AppendMetadataTemplateForModules(List<string> template)
        {
            foreach (var m in Map.Modules)
            {
                template.Add("- " + m.ModuleName + ":");
                AppendMetadataTemplateForOptions(template, "- " + m.ModuleName + ".", m.Options);
                AppendMetadataTemplateForResponse(template, "- " + m.ModuleName + ".response.", m.ResponseFields);
            }
        }

        private void AppendMetadataTemplateForOptions(List<string> template, string prefix, ModuleOption[] options)
        {
            if (options == null)
                return;

            foreach (var o in options)
            {
                template.Add(prefix + o.Name + ":");
                AppendMetadataTemplateForOptions(template, prefix + o.Name + ".", o.SubOptions);
            }
        }

        private void AppendMetadataTemplateForResponse(List<string> template, string prefix, ModuleResponseField[] fields)
        {
            if (fields == null)
                return;

            foreach (var f in fields)
            {
                template.Add(prefix + f.Name + ":");
                AppendMetadataTemplateForResponse(template, prefix + f.Name + ".", f.SubFields);
            }
        }

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

        public string VersionAdded
        {
            get
            {
                try
                {
                    return (_selectedMethod < Map.Modules.Length) ? Map.Modules[_selectedMethod].VersionAdded : "INVALID";
                }
                catch (Exception e)
                {
                    return "EXCEPTION";
                }
            }
        }

        public string YearAdded
        {
            get
            {
                try
                {
                    return (_selectedMethod < Map.Modules.Length) ? Map.Modules[_selectedMethod].YearAdded : "INVALID";
                }
                catch (Exception e)
                {
                    return "EXCEPTION";
                }
            }
        }

        public string Author
        {
            get
            {
                try
                {
                    return (_selectedMethod < Map.Modules.Length) ? Map.Modules[_selectedMethod].Author : "INVALID";
                }
                catch (Exception e)
                {
                    return "EXCEPTION";
                }
            }
        }

        public string AuthorEmail
        {
            get
            {
                try
                {
                    return (_selectedMethod < Map.Modules.Length) ? Map.Modules[_selectedMethod].AuthorEmail : "INVALID";
                }
                catch (Exception e)
                {
                    return "EXCEPTION";
                }
            }
        }

        public string AuthorIRC
        {
            get
            {
                try
                {
                    return (_selectedMethod < Map.Modules.Length) ? Map.Modules[_selectedMethod].AuthorIRC : "INVALID";
                }
                catch (Exception e)
                {
                    return "EXCEPTION";
                }
            }
        }

        public string MgmtClient
        {
            get
            {
                try
                {
                    return (_selectedMethod < Map.Modules.Length) ? Map.Modules[_selectedMethod].MgmtClient : "INVALID";
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

        public bool NeedsForceUpdate
        {
            get
            {
                return Map.Modules[_selectedMethod].NeedsForceUpdate;
            }
        }

        public bool HasResourceGroup()
        {
            return (Array.Find(ModuleOptions, element => (element.Name == "resource_group_name")) != null);
        }

        public bool HasTags()
        {
            if (ModuleResponseFields != null)
            {
                return (Array.Find(ModuleResponseFields, element => (element.Name == "tags")) != null);
            }
            else
            {
                return false;
            }
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
                var parameters = Array.Find(ModuleOptions, e => (e.Name == "parameters") || (e.Name.EndsWith("_parameters")));

                var location = (parameters != null && parameters.SubOptions != null) ? Array.Find(parameters.SubOptions, e => (e.Name == "location")) : null;
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
        public string[] GetModuleArgSpec(bool appendMainModuleOptions)
        {
            var argSpec = new List<string>();
            var options = GetCollapsedOptions(ModuleOptions);

            string[] l = GetModuleArgSpecFromOptions(options);

            if (appendMainModuleOptions || HasTags())
            {
                l[l.Length - 1] += ",";
            }

            argSpec.AddRange(l);

            if (appendMainModuleOptions)
            {
                if (NeedsForceUpdate)
                {
                    argSpec.Add("force_update=dict(");
                    argSpec.Add("    type='bool'");
                    argSpec.Add("),");
                }

                argSpec.Add("state=dict(");
                argSpec.Add("    type='str',");
                argSpec.Add("    default='present',");
                argSpec.Add("    choices=['present', 'absent']");
                argSpec.Add(")");
            }
            else if (HasTags())
            {
                argSpec.Add("tags=dict(");
                argSpec.Add("    type='list'");
                argSpec.Add(")");
            }

            return argSpec.ToArray();
        }

        private string[] GetModuleArgSpecFromOptions(ModuleOption[] options)
        {
            var argSpec = new List<string>();

            for (int i = 0; i < options.Length; i++)
            {
                var option = options[i];
                if (!option.IncludeInArgSpec)
                    continue;
                bool defaultOrRequired = (option.DefaultValue != null) || (option.Required == "True");
                bool choices = (option.EnumValues != null) && option.EnumValues.Length > 0;

                if (argSpec.Count > 0)
                {
                    string n = argSpec.Last() + ",";
                    argSpec.RemoveAt(argSpec.Count - 1);
                    argSpec.Add(n);
                }

                argSpec.Add(option.NameAlt + "=dict(");
                argSpec.Add("    type='" + (option.IsList ? "list" : option.Type) + "'" + ((option.NoLog || defaultOrRequired || choices || (option.SubOptions != null && option.SubOptions.Length > 0)) ? "," : ""));

                if (option.NoLog)
                {
                    argSpec.Add("    no_log=True" + ((defaultOrRequired || choices || (option.SubOptions != null && option.SubOptions.Length > 0)) ? "," : ""));
                }

                if (option.SubOptions != null && option.SubOptions.Length > 0)
                {
                    string[] subspec = GetModuleArgSpecFromOptions(option.SubOptions);
                    argSpec.Add("    options=dict(");
                    for (int j = 0; j < subspec.Length; j++)
                    {
                        argSpec.Add("        " + subspec[j]);
                    }
                    argSpec.Add("    )");
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

        //
        // used by facts tests
        //
        public string[] GetModuleResponseFieldsPaths()
        {
            List<string> paths = new List<string>();

            if (ModuleResponseFields != null)
            {
                paths.AddRange(AddModuleResponseFieldPaths("", ModuleResponseFields));
            }

            return paths.ToArray();
        }

        private string[] AddModuleResponseFieldPaths(string prefix, ModuleResponseField[] fields)
        {
            List<string> paths = new List<string>();
            foreach (var f in fields)
            {
                if (f.Returned == "always")
                {
                    if (f.Type == "complex")
                    {
                        paths.AddRange(AddModuleResponseFieldPaths(prefix + f.NameAlt + ".", f.SubFields));
                    }
                    else if (f.NameAlt != "x")
                    {
                        paths.Add(prefix + f.NameAlt);
                    }
                }
            }

            return paths.ToArray();
        }

        //
        // NOT USED
        //
        public ModuleOption[] ModuleOptionsUnflattened
        {
            get
            {
                var m = GetModuleMap(ModuleName);
                IEnumerable<ModuleOption> options = from option in m.Options where (option.Disposition == "dictionary") || (option.Disposition == "default") select option;
                return options.ToArray();
            }
        }

        //
        // Variables that are used to store top level options
        //
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

        //
        // Code to expand options to actual structure
        //
        public void PrepareAdjustmentStatements()
        {
            IsCamelizeNeeded = false;
            IsMapNeeded = false;
            IsUpperNeeded = false;
            IsRenameNeeded = false;
            IsExpandNeeded = false;
            IsResourceIdNeeded = false;

            _AdjustmentStatements = GetAdjustmentStatements(ModuleOptionsSecondLevel, new List<string>().ToArray(), null);
        }

        public string[] AdjustmentStatements {
            get
            {
                if (_AdjustmentStatements == null)
                {
                    PrepareAdjustmentStatements();
                }

                return _AdjustmentStatements;
            }
        }

        private string[] _AdjustmentStatements = null;
        public bool IsCamelizeNeeded { get; set; }
        public bool IsMapNeeded { get; set; }
        public bool IsUpperNeeded { get; set; }
        public bool IsRenameNeeded { get; set; }
        public bool IsExpandNeeded { get; set; }
        public bool IsResourceIdNeeded { get; set; }

        private string[] GetAdjustmentStatements(ModuleOption[] options, string[] path, string expand)
        {
            var statements = new List<string>();

            foreach (var option in options)
            {
                // option not included in arg spec.... just skip it
                if (!option.IncludeInArgSpec)
                    continue;

                string newExpand = null;
                string[] newPath = path;
                string[] newPathX = path;
                var parameters = new List<string>();
                string new_name = "";
                string exceptions = "";

                List<string> tempPath = new List<string>(path);
                tempPath.Add(option.NameAlt);
                newPathX = tempPath.ToArray();

                if (option.Collapsed)
                {
                    // if option is collapsed we will have to expand it
                    newExpand = option.Name;
                }
                else
                {
                    // if not collapsed add it to the path
                    newPath = newPathX;
                }

                if (option.Name != option.NameAlt)
                {
                    parameters.Add("rename");
                    new_name = option.Name;
                    IsRenameNeeded = true;
                }

                if (expand != null)
                {
                    parameters.Add("expand");  
                    new_name = expand;
                    IsExpandNeeded = true;
                }

                if (option.SubOptions != null)
                {
                    statements.AddRange(GetAdjustmentStatements(option.SubOptions, newPath, newExpand));
                }

                if (option.Name == "id" || option.Name.EndsWith("_id"))
                {
                    parameters.Add("resource_id");
                    IsResourceIdNeeded = true;
                }

                // translate boolean value
                if (option.ValueIfFalse != null && option.ValueIfTrue != null)
                {
                    exceptions="True: '" + option.ValueIfTrue + "', False: '" + option.ValueIfFalse + "'";
                }
                // translate enum values
                else if (option.EnumValues != null && option.EnumValues.Length > 0)
                {
                    bool camelize = false;
                    exceptions = "";
                    string exceptionsNormalCamel = "";
                    string exceptionsLowerCamel = "";
                    string exceptionsUpper = "";
                    // if option contains enum value, check if it has to be translated
                    foreach (var enumValue in option.EnumValues)
                    {
                        if (enumValue.Key != enumValue.Value)
                        {
                            camelize = true;
                            // can it be translated using snake_to_camel?
                            string camelUpper = CamelCase(enumValue.Key, false);
                            string camelLower = CamelCase(enumValue.Key, true);
                            string upper = enumValue.Key.ToUpper();
                            if (camelUpper != enumValue.Value)
                            {
                                if (exceptionsNormalCamel != "") exceptionsNormalCamel += ", ";
                                exceptionsNormalCamel += "'" + enumValue.Key + "': '" + enumValue.Value + "'";
                            }

                            if (camelLower != enumValue.Value)
                            {
                                if (exceptionsLowerCamel != "") exceptionsLowerCamel += ", ";
                                exceptionsLowerCamel += "'" + enumValue.Key + "': '" + enumValue.Value + "'";
                            }

                            if (upper != enumValue.Value)
                            {
                                if (exceptionsUpper != "") exceptionsUpper += ", ";
                                exceptionsUpper += "'" + enumValue.Key + "': '" + enumValue.Value + "'";
                            }
                        }
                    }

                    if (camelize)
                    {
                        string selected = "camelize";
                        exceptions = exceptionsNormalCamel;

                        if (exceptions.Length > exceptionsLowerCamel.Length)
                        {
                            selected = "camelize_lower";
                            exceptions = exceptionsLowerCamel;
                        }

                        if (exceptions.Length > exceptionsUpper.Length)
                        {
                            selected = "upper";
                            exceptions = exceptionsUpper;
                        }

                        parameters.Add(selected);

                        if (selected == "upper")
                        {
                            IsUpperNeeded = true;
                        }
                        else
                        {
                            IsCamelizeNeeded = true;
                        }
                    }
                }

                if (exceptions != "")
                {
                    parameters.Add("map");
                    IsMapNeeded = true;
                }

                // after suboptions are handled, add current parameter transformation
                if (parameters.Count > 0)
                {
                    foreach (var p in parameters)
                    {
                        string variable = "dict_";
                        switch (p)
                        {
                            case "camelize_lower":
                                variable += "camelize";
                                break;
                            case "camelize":
                            case "expand":
                            case "upper":
                            case "rename":
                            case "map":
                            case "resource_id":
                                variable += p;
                                break;
                        }
                        variable += "(self." + ParametersOptionName + ", [";

                        for (int i = 0; i < newPathX.Length; i++)
                        {
                            variable += "'" + newPathX[i] + "'";
                            variable += (i != newPathX.Length - 1) ? ", " : "";
                        }

                        variable += "]";

                        switch (p)
                        {
                            case "camelize_lower":
                                variable += ", False";
                                break;
                            case "camelize":
                                variable += ", True";
                                break;
                            case "expand":
                            case "upper":
                                break;
                            case "rename":
                                variable += ", '" + new_name + "'";
                                break;
                            case "map":
                                variable += ", {" + exceptions + "}";
                                break;
                            case "resource_id":
                                variable += ", subscription_id=self.subscription_id, resource_group=self.resource_group";
                                break;
                        }

                        variable += ")";
                        statements.Add(variable);
                    }
                }
            }
                
            return statements.ToArray();
        }

        // "parameters" is flattened by default
        // in fact what we are looking here is content of "parameters"
        // so most likely all of the options returned will have "disposition" == "parameters"
        public ModuleOption[] ModuleOptionsSecondLevel
        {
            get
            {
                var m = GetModuleMap(ModuleName);
                List<ModuleOption> options = new List<ModuleOption>();

                foreach (var o in m.Options)
                {
                    if (o.SubOptions != null && o.SubOptions.Length > 0)
                    {
                        foreach (var oo in o.SubOptions)
                        {
                            oo.Disposition = o.NameAlt;
                        }

                        options.AddRange(o.SubOptions);
                    }
                }
                return options.ToArray();
            }
        }

        public string ParametersOptionName
        {
            get
            {
                var m = GetModuleMap(ModuleName);
                foreach (var o in m.Options)
                {
                    if (o.SubOptions != null && o.SubOptions.Length > 0)
                    {
                        return o.Name;
                    }
                }
                return "parameters";
            }
        }

        //
        // Module idempotency check
        //
        public string[] ModuleIdempotencyCheck
        {
            get
            {
                foreach (var option in ModuleOptions)
                {
                    if (option.Name.Contains("parameters"))
                    {
                        return GetIdempotencyCheck(option.SubOptions, "", "", 0);
                    }
                }

                return new string[] { };
            }
        }

        private string[] GetIdempotencyCheck(ModuleOption[] options, string statementPrefix, string dictPrefix, int level)
        {
            List<string> statements = new List<string>();

            foreach (var option in options)
            {
                string optionStatementPrefix = statementPrefix;

                if (option.UpdateRule != null && option.UpdateRule != "none")
                {
                    if (option.UpdateRule != "compare")
                    {
                        statements.Add("'" + statementPrefix + "/" + option.NameAlt + "': '" + option.UpdateRule + "',");
                    }
                }
                else
                {
                    // there's no rule for this option
                    // check suboptions
                    if (option.SubOptions != null)
                    {
                        string[] subStatements = GetIdempotencyCheck(option.SubOptions, statementPrefix + "/" + option.NameAlt, "", level + 1);

                        if (subStatements.Length > 0)
                        {
                            statements.AddRange(subStatements);
                        }
                    }
                    else
                    {
                        //statements.Add("OPTION " + option.Name + " DOESN'T HAVE SUBOPTIONS");
                    }
                }
            }

            // remove coma from the last string
            string[] a = statements.ToArray();
            if (a.Length > 0)
            {
                string last = statements.Last();
                last = last.Substring(0, last.Length - 1);
                a[a.Length - 1] = last;
            }
            return a;
        }

        //
        // Module documentation -- used by both main and facts module
        //
        public string[] ModuleDocumentation
        {
            get
            {
                return GetHelpFromOptions(GetCollapsedOptions(ModuleOptions), "    ");
            }
        }

        //
        // Module documentation -- return value
        //
        public string[] ModuleDocumentationReturn
        {
            get
            {
                return GetHelpFromResponseFields(ModuleResponseFields, "");
            }
        }

        //
        // Module documentation -- return value -- facts
        //
        public string[] ModuleDocumentationReturnFacts
        {
            get
            {
                List<string> help = new List<string>();
                help.Add(ModuleOperationName + ":");
                help.Add("    description: A list of dictionaries containing facts for " + ObjectName + "."); 
                help.Add("    returned: always");
                help.Add("    type: complex");
                help.Add("    contains:"); 
                help.AddRange(GetHelpFromResponseFields(ModuleResponseFields, "        "));
                return help.ToArray();
            }
        }

        public string[] ModuleExamples
        {
            get
            {
                var m = GetModuleMap(ModuleName);
                List<string> samples = new List<string>(GetPlaybook("Create (or update)", ModuleOptions, "  ", "default"));
                samples.AddRange(m.AdditionalSampleLines);
                return samples.ToArray();
            }
        }

        public string[] ModuleExamplesFacts
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

        //
        // This returns code to create dictionary for format_response() function
        //
        public string[] ModuleReturnResponseDictionary
        {
            get
            {
                return GetResponseDictionary(ModuleResponseFields, "", "d");
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

        public string[] GetModuleFactTest(int idx, string instanceNamePostfix = "")
        {
            var m = GetModuleMap(ModuleName);
            return GetModuleTest(0, "Gather facts", m.Methods[idx].Name, false, instanceNamePostfix);
        }

        public bool IsModuleFactsTestMulti(int idx)
        {
            var m = GetModuleMap(ModuleName);
            return m.Methods[idx].Name != "get";
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

        public string[] GetModuleTestPrerequisites(bool parent, bool grandparent, string instancePostfix = "")
        {
            List<string> prePlaybook = new List<string>();
            string prerequisites = Map.Modules[_selectedMethod].TestPrerequisitesModule;

            if ((prerequisites != null) && (prerequisites != ""))
            {
                var subModel = new CodeModelAnsibleMap(Map, null, prerequisites);
                if (grandparent)
                {
                    prePlaybook.AddRange(subModel.GetModuleTestPrerequisites(true, true));
                }

                if (parent)
                {
                    prePlaybook.AddRange(subModel.GetModuleTest(1, "Create", "", false, instancePostfix));
                }
            }

            if (parent)
            {
                if (Map.Modules[_selectedMethod].TestPrerequisites != null)
                {
                    string[] preRequisites = Map.Modules[_selectedMethod].TestPrerequisites.Clone() as string[];
                    for (int i = 0; i < preRequisites.Length; i++) preRequisites[i] = preRequisites[i].Replace("$postfix$", instancePostfix);
                    prePlaybook.AddRange(preRequisites);
                }
            }

            return prePlaybook.ToArray();
        }
        public bool GetModuleCanDeletePrerequisites()
        {
            string prerequisites = Map.Modules[_selectedMethod].TestPrerequisitesModule;

            if ((prerequisites != null) && (prerequisites != ""))
            {
                var subModel = new CodeModelAnsibleMap(Map, null, prerequisites);
                if (!subModel.CanDelete())
                    return false;
            }

            return true;
        }

        public string[] GetModuleTestDeleteClearPrerequisites(bool includeParentPrerequisites, bool includeGrandparentPrerequisites, string instancePostfix = "")
        {
            List<string> prePlaybook = new List<string>();

            string prerequisites = Map.Modules[_selectedMethod].TestPrerequisitesModule;

            if ((prerequisites != null) && (prerequisites != ""))
            {
                var subModel = new CodeModelAnsibleMap(Map, null, prerequisites);

                // parent prerequisites from submodule
                if (includeParentPrerequisites)
                {
                    if (subModel.CanDelete())
                    {
                        prePlaybook.AddRange(subModel.GetModuleTest(0, "Delete instance of", "delete", false, instancePostfix));
                    }
                }

                // grandparent prerequisites from submodule
                if (includeGrandparentPrerequisites)
                {
                    prePlaybook.AddRange(subModel.GetModuleTestDeleteClearPrerequisites(true, true));
                }
            }

            // this are parent prerequisites defined like playbook
            if (includeParentPrerequisites)
            {
                if (Map.Modules[_selectedMethod].TestPostrequisites != null)
                {
                    string[] postRequisites = Map.Modules[_selectedMethod].TestPostrequisites.Clone() as string[];
                    for (int i = 0; i < postRequisites.Length; i++) postRequisites[i] = postRequisites[i].Replace("$postfix$", instancePostfix);
                    prePlaybook.AddRange(postRequisites);
                }
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

        private string[] GetModuleTest(int level, string testType, string methodType, bool isCheckMode, string instanceNamePostfix = "")
        {
            List<string> prePlaybook = new List<string>();
            string postfix = isCheckMode ? " -- check mode" : "";

            prePlaybook.AddRange(GetPlaybook(testType, ((methodType == "") ? ModuleOptions : GetMethodOptions(methodType)), "", "test:default", postfix, instanceNamePostfix));

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
                    case "BatchManagementClient": return "azure.mgmt.batch";
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

        //
        // This function generates API call for specified method
        //
        public string[] ModuleGenerateApiCall(string indent, string methodName)
        {
            var response = new List<string>();
            string line = indent + "response = self." + (MgmtClient != null ? MgmtClient : "mgmt_client") + "." + ModuleOperationName + "." + methodName + "(";
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

        public string ObjectNameNoSpaces
        {
            get
            {
                return GetModuleMap(ModuleName).ObjectName.Replace(" ", "");
            }
        }

        public string ObjectNamePlural
        {
            get
            {
                return GetModuleMap(ModuleName).ObjectNamePlural;
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

        public ModuleOption FindOptionByName(string name)
        {
            foreach (var o in ModuleOptions)
            {
                if (o.Name == name || o.NameAlt ==  name)
                    return o;
            }

            return null;
        }

        private class MethodIf
        {
            public MethodIf(string name, string[] parameters)
            {
                Name = name;
                Parameters = parameters;
            }
            public string Name;
            public string[] Parameters;
        }

        public string[] GenerateFactsMainIfStatement()
        {

            var response = new List<string>();
            bool firstMethod = true;

            var methods = new List<MethodIf>();

            foreach (var f in ModuleMethods)
            {
                string[] ps = GetMethodRequiredOptionNames(f.Name);
                var tmpOptions = new List<string>();
                for (int idx = 0; idx < ps.Length; idx++)
                {
                    string optionName = ps[idx]; if (optionName == "resource_group_name") { optionName = "resource_group"; }
                    var o = FindOptionByName(optionName);

                    if (o == null || o.Required == "True")
                        continue;

                    tmpOptions.Add(o.NameAlt);
                }
                ps = tmpOptions.ToArray();

                methods.Add(new MethodIf(f.Name, ps));
            }

            methods.Sort(delegate (MethodIf x, MethodIf y) { int xl = x.Parameters.Length; int yl = y.Parameters.Length; if (xl > yl) return -1; else if (xl < yl) return 1; else return 0; });

            foreach (var f in methods)
            {
                if (methods.Count > 1)
                {
                    string[] ps = f.Parameters;

                    if (ps.Length > 0)
                    {
                        for (int idx = 0; idx < ps.Length; idx++)
                        {
                            string l = "";
                            if (idx == 0)
                            {
                                l += firstMethod ? "if " : "elif ";
                                if (ps.Length > 1) l += "(";
                            }
                            else
                            {
                                l += "        ";
                            }

                            l += "self." + ps[idx] + " is not None";

                            if (idx != ps.Length - 1)
                            {
                                l += " and";
                            }
                            else
                            {
                                if (ps.Length > 1) l += ")";
                                l += ":";
                            }

                            response.Add(l);
                        }
                    }
                    else
                    {
                        response.Add("else:");
                    }
                }

                response.Add((ModuleMethods.Length > 1 ? "    " : "") + "self.results['" + ModuleOperationName +"'] = self." + f.Name + "()");
                firstMethod = false;
            }

            return response.ToArray();
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
        private string[] GetPlaybook(string operation, ModuleOption[] options, string padding, string playbookType, string operationPostfix = "", string instanceNamePostfix = "")
        {
            List<string> help = new List<string>();

            help.Add(padding + "- name: " + operation + " " + ObjectName + operationPostfix);
            help.Add(padding + "  " + ModuleNameAlt + ":");
            help.AddRange(GetPlaybookFromOptions(options, padding + "    ", playbookType, instanceNamePostfix));
            return help.ToArray();
        }

        private string[] GetPlaybookFromOptions(ModuleOption[] options, string padding, string playbookType, string instanceNamePostfix = "")
        {
            List<string> help = new List<string>();
            foreach (var option in GetCollapsedOptions(options))
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

                    // XXX - this is just a temporary hack for now
                    if (predefined.Contains("$postfix$"))
                    {
                        predefined = predefined.Replace("$postfix$", instanceNamePostfix);
                    }
                    else if (option.NameAlt == "name")
                    {
                        predefined += instanceNamePostfix;
                    }

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
            List<KeyValuePair<string, string>> allChoices = new List<KeyValuePair<string, string>>();
            // XXX - collect all possible choices
            foreach (var option in options)
            {
                if (option.EnumValues != null)
                {
                    foreach (var choice in option.EnumValues)
                    {
                        allChoices.Add(choice);
                    }
                }
            }

            List<string> help = new List<string>();
            foreach (var option in options)
            {
                // check if option should be included in documentation
                if (!option.IncludeInDocumentation)
                    continue;

                string doc = option.Documentation;

                doc = NormalizeString(option.Documentation);

                if (option.DocumentationMarkKeywords)
                {
                    // try to replace all mentioned option names with I()
                    foreach (var oo in options)
                    {
                        string name = oo.Name.ToCamelCase();

                        if (oo.Name == "name" || oo.Name == "location" || oo.Name == "id" || oo.Name == "edition" || oo.Name == option.Name)
                            continue;

                        doc = Regex.Replace(doc, "\\b" + name + "\\b", "I(" + oo.NameAlt + ")", RegexOptions.IgnoreCase);
                    }

                    // replace all mentioned option names with C()
                    foreach (var choice in allChoices)
                    {
                        doc = Regex.Replace(doc, "\\b" + choice.Value + "\\b", "C(" + choice.Key + ")", RegexOptions.IgnoreCase);
                    }
                }

                help.Add(padding + option.NameAlt + ":");
                help.Add(padding + "    description:");

                string[] docParagraphs = doc.Split("\n");

                foreach (var paragraph in docParagraphs)
                {
                    if (paragraph.Trim() != "")
                    {
                        help.AddRange(WrapString(padding + "        - ", paragraph));
                    }
                }

                // write only if true
                if (option.Required == "True")
                {
                    help.Add(padding + "    required: " + option.Required);
                }
                else if (option.Required == "Create")
                {
                    help.Add(padding + "        - Required when C(state) is I(present).");
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
                    help.AddRange(GetHelpFromOptions(GetCollapsedOptions(option.SubOptions), padding + "        "));
                }
            }

            return help.ToArray();
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
                    if (field.NameAlt == "" || field.NameAlt.ToLower() == "x")
                        continue;

                    if (!field.Collapsed)
                    {
                        string doc = NormalizeString(field.Description);
                        help.Add(padding + field.NameAlt + ":");
                        help.Add(padding + "    description:");
                        help.AddRange(WrapString(padding + "        - ", doc));

                        help.Add(padding + "    returned: " + field.Returned);
                        help.Add(padding + "    type: " + field.Type);

                        help.AddRange(WrapString(padding + "    sample: ", field.SampleValue));
                    }

                    if (field.SubFields != null && field.SubFields.Length > 0)
                    {
                        if (field.Collapsed)
                        {
                            help.AddRange(GetHelpFromResponseFields(field.SubFields, padding));
                        }
                        else
                        {
                            help.Add(padding + "    contains:");
                            help.AddRange(GetHelpFromResponseFields(field.SubFields, padding + "        "));
                        }
                    }
                }
            }

            return help.ToArray();
        }

        public string[] GetResponseDictionary(ModuleResponseField[] fields, string padding, string srcPrefix)
        {
            List<string> help = new List<string>();

            if (fields != null)
            {
                bool coma = false;
                for (int i = 0; i < fields.Length; i++)
                {
                    ModuleResponseField field = fields[i];
                    bool last = (i == fields.Length - 1);
                    // setting nameAlt to empty or "x" will remove the field
                    if (field.NameAlt == "" || field.NameAlt.ToLower() == "x")
                        continue;

                    if (coma)
                    {
                        help[help.Count - 1] += ",";
                        coma = false;
                    }

                    if (field.SubFields != null && field.SubFields.Length > 0)
                    {
                        if (!field.Collapsed)
                        {
                            help.Add(padding + "'" + field.NameAlt + "': {" );
                            help.AddRange(GetResponseDictionary(field.SubFields, padding + "    ", srcPrefix + ".get('" + field.Name + "', {})"));
                            help.Add(padding + "}");
                        }
                        else
                        {
                            help.AddRange(GetResponseDictionary(field.SubFields, padding, srcPrefix + ".get('" + field.Name + "', {})"));
                        }
                    }
                    else
                    {
                        help.Add(padding + "'" + field.NameAlt + "': " + srcPrefix + ".get('" + field.Name + "', None)");
                    }
                    coma = true;
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

        public bool IsSnakeToCamelNeeded
        {
            get; set;
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
                int docLength = content.Length;
                List<string> words = new List<string>(content.Split(' '));

                int wordIdx = 1;
                while (wordIdx < words.Count)
                {
                    words[wordIdx] = " " + words[wordIdx];
                    wordIdx++;
                }

                wordIdx = 0;

                while (wordIdx < words.Count)
                {
                    string word = words[wordIdx];

                    // make sure words are no longer than max width
                    if (word.Length > chunkLength)
                    {
                        words.Insert(wordIdx + 1, word.Substring(chunkLength));
                        words[wordIdx] = word.Substring(0, chunkLength);
                    }

                    wordIdx++;
                }

                bool first = true;

                wordIdx = 0;

                while (wordIdx < words.Count)
                {
                    string chunk = words[wordIdx++];

                    // if the word still longer than line -- break it down

                    while (wordIdx < words.Count)
                    {
                        if (chunk.Length + 1 + words[wordIdx].Length > chunkLength)
                            break;

                        chunk += words[wordIdx++];
                    }

                    if (first)
                    {
                        // first line -- add quotes
                        output.Add(prefix + "\"" + chunk + ((wordIdx != words.Count) ? "" : "\""));
                        prefix = new String(prefix.Select(r => ' ').ToArray());
                        first = false;
                    }
                    else if (wordIdx != words.Count)
                    {
                        // everything in the middle
                        output.Add(prefix + chunk);
                    }
                    else
                    {
                        // last line -- add closing quotes
                        output.Add(prefix + chunk + "\"");
                    }
                }
            }

            return output.ToArray();
        }

        private static string NormalizeString(string s)
        {
            if (s == null)
                return "";
                
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
                    case (char)0x2013:
                        a[i] = '-';
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
        public string CamelCase(string name, bool firstLower)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return name;
            }

            if (name[0] == '_')
            // Preserve leading underscores.
            {
                return '_' + CamelCase(name.Substring(1), firstLower);
            }

            return
                name.Split('_', '-', ' ')
                    .Where(s => !string.IsNullOrEmpty(s))
                    .Select((s, i) => FormatCase(s, firstLower)) // Pass true/toLower for just the first element.
                    .DefaultIfEmpty("")
                    .Aggregate(string.Concat);
        }
        private string FormatCase(string name, bool toLower)
        {
            if (!string.IsNullOrEmpty(name))
            {
                if ((name.Length < 2) || ((name.Length == 2) && char.IsUpper(name[0]) && char.IsUpper(name[1])))
                {
                    name = toLower ? name.ToLowerInvariant() : name.ToUpperInvariant();
                }
                else
                {
                    name =
                    (toLower
                        ? char.ToLowerInvariant(name[0])
                        : char.ToUpperInvariant(name[0])) + name.Substring(1, name.Length - 1);
                }
            }
            return name;
        }

        private static ModuleOption[] GetCollapsedOptions(ModuleOption[] options)
        {
            var collapsed = new List<ModuleOption>();

            foreach (var o in options)
            {
                // XXX why?
                if (o != null)
                {
                    if (o.Collapsed)
                    {
                        collapsed.AddRange(GetCollapsedOptions(o.SubOptions));
                    }
                    else
                    {
                        collapsed.Add(o);
                    }
                }
            }

            return collapsed.ToArray();
        }
    }
}
