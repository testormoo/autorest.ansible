﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace AutoRest.Ansible
{
    public abstract class Tweak
    {
        public abstract bool Apply(Model.MapAnsible map);

        public abstract bool ApplyOnModule(Model.MapAnsibleModule m);

        public string log = null;

        public static Tweak CreateTweak(string item, string name, string parameter)
        {
            string[] path = item.Split('.');

            if (path.Length == 1)
            {
                // module level tweak
                switch (name)
                {
                    case "rename": return new Tweak_Module_Rename(path[0], parameter);
                    case "samples-append-line": return new Tweak_Module_SampleAppendLine(path[0], parameter);
                    case "test-prerequisites-module": return new Tweak_Module_TestPrerequisitesModule(path[0], parameter, null, null);
                }
            }
            else if (path[1] == "response")
            {
                switch(name)
                {
                case "collapse":                    return new Tweak_Response_CollapseField(path[0], String.Join('.', path, 2, path.Length - 2));
                case "rename":                      return new Tweak_Response_RenameField(path[0], String.Join('.', path, 2, path.Length - 2), parameter);
                case "remove":                      return new Tweak_Response_RemoveField(path[0], String.Join('.', path, 2, path.Length - 2));
                case "add":                         return new Tweak_Response_RenameField(path[0], String.Join('.', path, 2, path.Length - 2), parameter);
                }
            }
            else
            {
                // option level tweak
                switch (name)
                {
                case "rename":                      return new Tweak_Option_Rename(path[0], String.Join('.', path, 1, path.Length - 1), parameter);
                case "required":                    return new Tweak_Option_Required(path[0], String.Join('.', path, 1, path.Length - 1), parameter == "yes");
                case "type":                        return new Tweak_Option_SetType(path[0], String.Join('.', path, 1, path.Length - 1), parameter);
                case "exclude":                     return new Tweak_Option_Exclude(path[0], String.Join('.', path, 1, path.Length - 1), parameter == "yes", parameter == "yes");
                case "default":                     return new Tweak_Option_DefaultValue(path[0], String.Join('.', path, 1, path.Length - 1), parameter);
                case "test":                        return new Tweak_Option_DefaultValueTest(path[0], String.Join('.', path, 1, path.Length - 1), parameter);
                case "sample":                      return new Tweak_Option_DefaultValueSample(path[0], String.Join('.', path, 1, path.Length - 1), parameter);
                case "documentation":               return new Tweak_Option_Documentation(path[0], String.Join('.', path, 1, path.Length - 1), parameter);
                case "documentation-append":        return new Tweak_Option_DocumentationAppend(path[0], String.Join('.', path, 1, path.Length - 1), parameter);
                case "documentation-append-line":   return new Tweak_Option_DocumentationAppend(path[0], String.Join('.', path, 1, path.Length - 1), "\n" + parameter);
                case "documentation-replace":       return new Tweak_Option_DocumentationReplace(path[0], String.Join('.', path, 1, path.Length - 1), parameter.Split(">>>")[0], parameter.Split(">>>")[1]);
                case "documentation-cut-after":     return new Tweak_Option_DocumentationCutAfter(path[0], String.Join('.', path, 1, path.Length - 1), parameter);
                case "documentation-mark-keywords": return new Tweak_Option_DocumentationMarkKeywords(path[0], String.Join('.', path, 1, path.Length - 1), parameter == "yes");
                case "flatten":                     return new Tweak_Option_Flatten(path[0], String.Join('.', path, 1, path.Length - 1), parameter);
                }
            }

            return null;
        }

    }

    public abstract class Tweak_Module : Tweak
    {
        public override bool Apply(Model.MapAnsible map)
        {
            bool applied = false;
            var modules = new List<Model.MapAnsibleModule>();

            if (_module == "*")
            {
                modules.AddRange(map.Modules); ;
            }
            else if (_module.EndsWith("*"))
            {
                string modulePrefix = _module.Substring(0, _module.Length - 1);
                foreach (var m in map.Modules)
                {
                    if (m.ModuleName.StartsWith(modulePrefix))
                    {
                        modules.Add(m);
                    }
                }
            }
            else
            {
                foreach (var m in map.Modules)
                {
                    if (m.ModuleName == _module)
                    {
                        modules.Add(m);
                    }
                }
            }

            _map = map;

            foreach (var m in modules)
            {
                if (ApplyOnModule(m))
                    applied = true;
            }

            return applied;
        }

        protected Model.MapAnsible _map;
        protected string _module;
    }

    public abstract class Tweak_Option : Tweak_Module
    {
        protected Model.ModuleOption GetOption(Model.MapAnsibleModule m, string[] path)
        {
            Model.ModuleOption option = null;

            if (m != null)
            {
                foreach (var o in m.Options)
                    if (o.Name == path[0])
                        option = o;

                if (option != null)
                {
                    for (int i = 1; i < path.Length; i++)
                    {
                        var subOptions = option.SubOptions;
                        option = null;

                        if (subOptions != null)
                        {
                            foreach (var o in subOptions)
                            {
                                if (o.Name == path[i])
                                {
                                    option = o;
                                    break;
                                }
                            }
                        }
                        if (option == null)
                            break;
                    }
                }
            }

            return option;
        }
    }

    public abstract class Tweak_Response : Tweak_Module
    {
        protected Model.ModuleResponseField GetResultField(Model.MapAnsibleModule m, Model.MapAnsible map, string[] path)
        {
            Model.ModuleResponseField field = null;

            if (m != null)
            {
                var fields = m.ResponseFields;

                for (int i = 0; i < path.Length; i++)
                {
                    if (fields == null)
                        return null;

                    foreach (var f in fields)
                    {
                        if (f.Name == path[i])
                        {
                            field = f;
                            break;
                        }
                    }

                    if (field == null)
                        return null;

                    fields = field.SubFields;
                }
            }

            return field;
        }
    }

    class Tweak_Module_Rename : Tweak_Module
    {
        public Tweak_Module_Rename(string originalName, string newName)
        {
            _module = originalName;
            _newName = newName;
        }

        public override bool ApplyOnModule(Model.MapAnsibleModule m)
        {
            if (_module.EndsWith("*"))
            {
                string prefix = _module.Substring(0, _module.Length - 1);
                string postfix = m.ModuleName.Substring(prefix.Length);
                m.ModuleNameAlt = _newName.Substring(0, _newName.Length - 1) + postfix;
            }
            else
            {
                m.ModuleNameAlt = _newName;
            }

            return true;
        }

        private string _newName;
    }

    class Tweak_Module_SampleAppendLine : Tweak_Module
    {
        public Tweak_Module_SampleAppendLine(string module, string newLine)
        {
            _module = module;
            _newLine = newLine;
        }

        public override bool ApplyOnModule(Model.MapAnsibleModule m)
        {
            List<string> newLines = new List<string>(m.AdditionalSampleLines);
            newLines.Add(_newLine);
            m.AdditionalSampleLines = newLines.ToArray();
            return true;
        }

        private string _newLine;
    }

    class Tweak_Module_FlattenParametersDictionary : Tweak_Module
    {
        public Tweak_Module_FlattenParametersDictionary(string module)
        {
            _module = module;
        }

        public override bool ApplyOnModule(Model.MapAnsibleModule m)
        {
            Model.ModuleMethod method = null;
            List<string> methodOptions = null;
            string dictionaryName = "";
            bool applied = false;

            foreach (var mth in m.Methods)
            {
                if (mth.Name == "create_or_update" || mth.Name == "create" || mth.Name == "update")
                {
                    method = mth;
                    methodOptions = method.RequiredOptions.ToList();

                    foreach (var o in m.Options)
                    {
                        if (o.Disposition == "dictionary" || o.Disposition == "__none")
                        {
                            o.Disposition = "__none";
                            dictionaryName = o.Name;
                            // remove from create_or_update function method list
                             
                            methodOptions.Remove(o.Name);
                        }
                        else if (o.Disposition == dictionaryName || o.Disposition == "__default")
                        {
                            o.Disposition = "__default";
                            methodOptions.Add(o.Name);
                        }
                    }
                    
                    // XXX - this is a terrible, terrible hack
                    methodOptions.RemoveAll(element => element.EndsWith("_parameters"));
                    method.RequiredOptions = methodOptions.ToArray();

                    applied = true;
                }
            }
            foreach (var o in m.Options)
            {
                if (o.Disposition == "__none") o.Disposition = "none";
                else if (o.Disposition == "__default") o.Disposition = "default";
            }

            return applied;
        }
    }

    class Tweak_Module_NeedsDeleteBeforeUpdate : Tweak_Module
    {
        public Tweak_Module_NeedsDeleteBeforeUpdate(string module)
        {
            _module = module;
        }

        public override bool ApplyOnModule(Model.MapAnsibleModule m)
        {
            m.NeedsDeleteBeforeUpdate = true;
            return true;
        }
    }

    class Tweak_Module_NeedsForceUpdate : Tweak_Module
    {
        public Tweak_Module_NeedsForceUpdate(string module)
        {
            _module = module;
        }

        public override bool ApplyOnModule(Model.MapAnsibleModule m)
        {
            m.NeedsForceUpdate = true;
            return true;
        }
    }

    class Tweak_Module_CannotTestUpdate : Tweak_Module
    {
        public Tweak_Module_CannotTestUpdate(string module)
        {
            _module = module;
        }

        public override bool ApplyOnModule(Model.MapAnsibleModule m)
        {
            m.CannotTestUpdate = false;
            return true;
        }
    }

    class Tweak_Module_AssertStateVariable : Tweak_Module
    {
        public Tweak_Module_AssertStateVariable(string module, string newValue)
        {
            _module = module;
            _newValue = newValue;
        }

        public override bool ApplyOnModule(Model.MapAnsibleModule m)
        {
            m.AssertStateVariable = _newValue;
            return true;
        }

        private string _newValue;
    }

    class Tweak_Module_AssertStateExpectedValue : Tweak_Module
    {
        public Tweak_Module_AssertStateExpectedValue(string module, string newValue)
        {
            _module = module;
            _newValue = newValue;
        }

        public override bool ApplyOnModule(Model.MapAnsibleModule m)
        {
            m.AssertStateExpectedValue = _newValue;
            return true;
        }

        private string _newValue;
    }

    class Tweak_Module_ObjectName : Tweak_Module
    {
        public Tweak_Module_ObjectName(string module, string newValue)
        {
            _module = module;
            _newValue = newValue;
        }

        public override bool ApplyOnModule(Model.MapAnsibleModule m)
        {
            m.ObjectName = _newValue;
            return true;
        }

        private string _newValue;
    }

    class Tweak_Module_TestPrerequisitesModule : Tweak_Module
    {
        public Tweak_Module_TestPrerequisitesModule(string module, string value, string replaceFrom, string replaceTo)
        {
            _module = module;
            _newValue = value;
            _replaceFrom = replaceFrom;
            _replaceTo = replaceTo;
        }

        public override bool ApplyOnModule(Model.MapAnsibleModule m)
        {
            m.TestPrerequisitesModule = _newValue;
            m.TestReplaceStringFrom = _replaceFrom;
            m.TestReplaceStringTo = _replaceTo;
            return true;
        }

        private string _newValue;
        private string _replaceFrom;
        private string _replaceTo;
    }

    class Tweak_Module_TestPrerequisites : Tweak_Module
    {
        public Tweak_Module_TestPrerequisites(string module, string[] pre, string[] post)
        {
            _module = module;
            _pre = pre;
            _post = post;
        }

        public override bool ApplyOnModule(Model.MapAnsibleModule m)
        {
            m.TestPrerequisites = _pre;
            m.TestPostrequisites = _post;
            return true;
        }

        private string[] _pre;
        private string[] _post;
    }

    class Tweak_Module_AddUpdateRule : Tweak_Module
    {
        public Tweak_Module_AddUpdateRule(string module, string parameterPath, string responsePath)
        {
            _module = module;
            _parameterPath = parameterPath.Split(":");
            _responsePath = responsePath.Split(":");
        }

        public override bool ApplyOnModule(Model.MapAnsibleModule m)
        {
            List<Model.UpdateComparisonRule> rules = (m.UpdateComparisonRules != null) ? m.UpdateComparisonRules.ToList() : new List<Model.UpdateComparisonRule>();
            rules.Add(new Model.UpdateComparisonRule(_parameterPath, _responsePath));
            m.UpdateComparisonRules = rules.ToArray();
            return true;
        }

        private string[] _parameterPath;
        private string[] _responsePath;
    }

    class Tweak_Option_Rename : Tweak_Option
    {
        public Tweak_Option_Rename(string module, string path, string newName, int levelChange = 0)
        {
            _module = module;
            _path = path.Split(".");
            _newName = newName;
            _levelChange = levelChange;
        }

        public override bool ApplyOnModule(Model.MapAnsibleModule m)
        {
            log = "RENAMING " + string.Join(".", _path) + " -- ";
            Model.ModuleOption option = GetOption(m, _path);
            if (option != null)
            {
                log += "A";
                option.NameAlt = _newName;
            }
            // XXX - level change

            return (option != null);
        }

        private string[] _path;
        private string _newName;
        private int _levelChange;
    }

    class Tweak_Option_Required : Tweak_Option
    {
        public Tweak_Option_Required(string module, string path, bool newValue)
        {
            _module = module;
            _path = path.Split(".");
            _newValue = newValue;
        }

        public override bool ApplyOnModule(Model.MapAnsibleModule m)
        {
            Model.ModuleOption option = GetOption(m, _path);
            if (option != null) option.Required = _newValue ? "True" : "False";
            // XXX - level change


            return (option != null);
        }

        private string[] _path;
        private bool _newValue;
    }

    class Tweak_Option_SetType : Tweak_Option
    {
        public Tweak_Option_SetType(string module,
                                        string path,
                                        string newType)
        {
            _module = module;
            _path = path.Split(".");
            _newType = newType;
        }

        public override bool ApplyOnModule(Model.MapAnsibleModule m)
        {
            Model.ModuleOption option = GetOption(m, _path);
            if (option != null)
            {
                option.Type = _newType;
            }

            return option != null;
        }

        private string[] _path;
        private string _newType;
    }

    class Tweak_Option_MakeBoolean : Tweak_Option
    {
        public Tweak_Option_MakeBoolean(string module,
                                        string path,
                                        string valueIfTrue,
                                        string valueIfFalse,
                                        bool defaultValue = false,
                                        string newDescription = null)
        {
            _module = module;
            _path = path.Split(".");
            _valueIfTrue = valueIfTrue;
            _valueIfFalse = valueIfFalse;
            _defaultValue = defaultValue;
            _newDescription = newDescription;
        }

        public override bool ApplyOnModule(Model.MapAnsibleModule m)
        {
            Model.ModuleOption option = GetOption(m, _path);
            if (option != null)
            {
                option.ValueIfFalse = _valueIfFalse;
                option.ValueIfTrue = _valueIfTrue;
                option.Type = "bool";
                option.DefaultValue = _defaultValue ? "True" : null;
                option.Documentation = option.Documentation.Split(" Possible values include:")[0];
                if (_newDescription != null) option.Documentation = _newDescription;
                option.EnumValues = null;
            }

            return (option != null);
        }

        private string[] _path;
        private string _valueIfTrue;
        private string _valueIfFalse;
        private bool _defaultValue;
        private string _newDescription;
    }

    class Tweak_Option_Exclude : Tweak_Option
    {
        public Tweak_Option_Exclude(string module,
                                        string path,
                                        bool excludeFromDocumentation,
                                        bool excludeFromArgSpec)
        {
            _module = module;
            _path = path.Split(".");
            _excludeFromDocumentation = excludeFromDocumentation;
            _excludeFromArgSpec = excludeFromArgSpec;
        }

        public override bool ApplyOnModule(Model.MapAnsibleModule m)
        {
            Model.ModuleOption option = GetOption(m, _path);
            if (option != null)
            {
                option.IncludeInDocumentation = !_excludeFromDocumentation;
                option.IncludeInArgSpec = !_excludeFromArgSpec;
            }

            return (option != null);
        }

        private string[] _path;
        private bool _excludeFromDocumentation;
        private bool _excludeFromArgSpec;
    }

    class Tweak_Option_DefaultValue : Tweak_Option
    {
        public Tweak_Option_DefaultValue(string module, string path, string value)
        {
            _module = module;
            _path = path.Split(".");
            _value = value;
        }

        public override bool ApplyOnModule(Model.MapAnsibleModule m)
        {
            Model.ModuleOption option = GetOption(m, _path);
            if (option != null) option.DefaultValue = _value;

            return (option != null);
        }

        private string[] _path;
        private string _value;
    }

    class Tweak_Option_DefaultValueTest : Tweak_Option
    {
        public Tweak_Option_DefaultValueTest(string module, string path, string newValue)
        {
            _module = module;
            _path = path.Split(".");
            _newValue = newValue;
        }

        public override bool ApplyOnModule(Model.MapAnsibleModule m)
        {
            Model.ModuleOption option = GetOption(m, _path);
            if (option != null) option.DefaultValueSample["test:default"] = _newValue;
            // XXX - level change

            return (option != null);
        }

        private string[] _path;
        private string _newValue;
    }

    class Tweak_Option_DefaultValueSample : Tweak_Option
    {
        public Tweak_Option_DefaultValueSample(string module, string path, string newValue)
        {
            _module = module;
            _path = path.Split(".");
            _newValue = newValue;
        }

        public override bool ApplyOnModule(Model.MapAnsibleModule m)
        {
            Model.ModuleOption option = GetOption(m, _path);
            if (option != null) option.DefaultValueSample["default"] = _newValue;
            // XXX - level change

            return option != null;
        }

        private string[] _path;
        private string _newValue;
    }

    class Tweak_Option_Documentation : Tweak_Option
    {
        public Tweak_Option_Documentation(string module, string path, string newText)
        {
            _module = module;
            _path = path.Split(".");
            _newText = newText;
        }

        public override bool ApplyOnModule(Model.MapAnsibleModule m)
        {
            Model.ModuleOption option = GetOption(m, _path);
            if (option != null) option.Documentation = _newText;
            // XXX - level change

            return option != null;
        }

        private string[] _path;
        private string _newText;
    }

    class Tweak_Option_DocumentationAppend : Tweak_Option
    {
        public Tweak_Option_DocumentationAppend(string module, string path, string appendedText)
        {
            _module = module;
            _path = path.Split(".");
            _appendedText = appendedText;
        }

        public override bool ApplyOnModule(Model.MapAnsibleModule m)
        {
            Model.ModuleOption option = GetOption(m, _path);
            if (option != null) option.Documentation += _appendedText;
            // XXX - level change

            return option != null;
        }

        private string[] _path;
        private string _appendedText;
    }

    class Tweak_Option_DocumentationCutAfter : Tweak_Option
    {
        public Tweak_Option_DocumentationCutAfter(string module, string path, string cutAfter)
        {
            _module = module;
            _path = path.Split(".");
            _cutAfter = cutAfter;
        }

        public override bool ApplyOnModule(Model.MapAnsibleModule m)
        {
            Model.ModuleOption option = GetOption(m, _path);
            if (option != null)
            {
                int idx = option.Documentation.IndexOf(_cutAfter);
                if (idx >= 0)
                {
                    option.Documentation = option.Documentation.Substring(idx + _cutAfter.Length);
                    return true;
                }
            }
            return false;
        }

        private string[] _path;
        private string _cutAfter;
    }

    class Tweak_Option_DocumentationReplace : Tweak_Option
    {
        public Tweak_Option_DocumentationReplace(string module, string path, string replaceFrom, string replaceTo)
        {
            _module = module;
            _path = path.Split(".");
            _replaceFrom = replaceFrom;
            _replaceTo = replaceTo;
        }

        public override bool ApplyOnModule(Model.MapAnsibleModule m)
        {
            Model.ModuleOption option = GetOption(m, _path);
            if (option != null)
            {
                string newString = option.Documentation.Replace(_replaceFrom, _replaceTo);

                if (newString != option.Documentation)
                {
                    option.Documentation = newString;
                    return true;
                }
            }
            return false;
        }

        private string[] _path;
        private string _replaceFrom;
        private string _replaceTo;
    }

    class Tweak_Option_DocumentationMarkKeywords : Tweak_Option
    {
        public Tweak_Option_DocumentationMarkKeywords(string module, string path, bool markKeywords)
        {
            _module = module;
            _path = path.Split(".");
            _markKeywords = markKeywords;
        }

        public override bool ApplyOnModule(Model.MapAnsibleModule m)
        {
            Model.ModuleOption option = GetOption(m, _path);
            if (option != null)
            {
                if (option.DocumentationMarkKeywords != _markKeywords)
                {
                    option.DocumentationMarkKeywords = _markKeywords;
                    return true;
                }
            }
            return false;
        }

        private string[] _path;
        private bool _markKeywords;
    }

    class Tweak_Option_Flatten : Tweak_Option
    {
        public Tweak_Option_Flatten(string module, string path, string namePrefix)
        {
            _module = module;
            _path = path.Split(".");
            _namePrefix = namePrefix;
        }

        public override bool ApplyOnModule(Model.MapAnsibleModule m)
        {
            log = "FLATTENING " + string.Join(".", _path) + " -- ";

            Model.ModuleOption option = GetOption(m, _path);

            if (option == null)
                return false;

            log += "A";

            if (option.IsList)
                return false;

            log += "B";

            if (option.Type != "dict")
                return false;

            log += "C";

            // remove suboptions from options
            Model.ModuleOption[] suboptions = option.SubOptions;
            option.SubOptions = null;
            
            // update suboptions disposition, so code generator knows that they needs to be stucked into dictionary
            foreach (var suboption in suboptions)
            {
                suboption.Disposition = ((option.Disposition != "default") ? option.Disposition : "") + ":" + option.Name;
                suboption.NameAlt = _namePrefix + suboption.NameAlt;

                // XXX - setting required to false, as it may go to level 0
                suboption.Required = "False";
            }

            log += "D";
            // flattened option becomes just hidden dictionary for its suboptions
            option.Disposition += ":dictionary";
            log += "E";

            if (_path.Length == 1)
            {
                List<Model.ModuleOption> o = m.Options.ToList();
                o.AddRange(suboptions);
                m.Options = o.ToArray();
            }
            else
            {
                string[] subPath = new string[_path.Length - 1];
                Array.Copy(_path, subPath, _path.Length - 1);
                Model.ModuleOption parent = GetOption(m, subPath);

                List<Model.ModuleOption> o = parent.SubOptions.ToList();
                o.AddRange(suboptions);
                parent.SubOptions = o.ToArray();
            }

            return true;
        }

        private string[] _path;
        private string _namePrefix;
    }

    class Tweak_Response_RenameField : Tweak_Response
    {
        public Tweak_Response_RenameField(string module, string path, string newName, int levelChange = 0)
        {
            _module = module;
            _path = path.Split(".");
            _newName = newName;
            _levelChange = levelChange;
        }

        public override bool ApplyOnModule(Model.MapAnsibleModule m)
        {
            Model.ModuleResponseField field = GetResultField(m, _map, _path);
            if (field != null) field.NameAlt = _newName;
            // XXX - level change

            return (field != null);
        }

        private string[] _path;
        private string _newName;
        private int _levelChange;
    }

    class Tweak_Response_CollapseField : Tweak_Response
    {
        public Tweak_Response_CollapseField(string module, string path)
        {
            _module = module;
            _path = path.Split(".");
        }

        public override bool ApplyOnModule(Model.MapAnsibleModule m)
        {
            Model.ModuleResponseField field = GetResultField(m, _map, _path);
            if (field != null) field.Collapsed = true;
            return (field != null);
        }

        private string[] _path;
    }

    class Tweak_Response_RemoveField : Tweak_Response_RenameField
    {
        public Tweak_Response_RemoveField(string module, string path) : base(module, path, "x", 0)
        {
        }
    }

    class Tweak_Response_AddField : Tweak_Response
    {
        public Tweak_Response_AddField(string module, string path)
        {
            _module = module;
            _path = path.Split(".");
        }

        public override bool ApplyOnModule(Model.MapAnsibleModule m)
        {
            Model.ModuleResponseField field = GetResultField(m, _map, _path);
            if (field != null) field.NameAlt = field.Name;

            return field != null;
        }

        private string[] _path;
    }

    class Tweak_Response_FieldSampleValue : Tweak_Response
    {
        public Tweak_Response_FieldSampleValue(string module, string path, string newValue, int levelChange = 0)
        {
            _module = module;
            _path = path.Split(".");
            _newValue = newValue;
            _levelChange = levelChange;
        }

        public override bool ApplyOnModule(Model.MapAnsibleModule m)
        {
            Model.ModuleResponseField field = GetResultField(m, _map, _path);
            if (field != null) field.SampleValue = _newValue;
            // XXX - level change

            return field != null;
        }

        private string[] _path;
        private string _newValue;
        private int _levelChange;
    }

    class Tweak_Response_FieldReturned : Tweak_Response
    {
        public Tweak_Response_FieldReturned(string module, string path, string returned)
        {
            _module = module;
            _path = path.Split(".");
            _returned = returned;
        }

        public override bool ApplyOnModule(Model.MapAnsibleModule m)
        {
            Model.ModuleResponseField field = GetResultField(m, _map, _path);
            if (field != null) field.Returned = _returned;
            // XXX - level change

            return field != null;
        }

        private string[] _path;
        private string _returned;
    }
}

