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
using AutoRest.Core.Logging;

namespace AutoRest.Ansible.Model
{
    public class CodeModelAnsible : CodeModelPy
    {

        public MapAnsible Map
        {
            get
            {
                if (_map == null) CreateMap();
                return _map;
            }
        }

        private MapAnsible _map = null;

        private string _namespace_upper;

        private string AnsibleModuleName
        {
            get
            {
                string multi = (Operations.Count > 1) ? Namespace + "_" : "";
                return "azure_rm_" + multi + Operations[CurrentOperationIndex].Name.ToLower();
            }
        }

        private string AnsibleModuleNameFacts
        {
            get
            {
                return AnsibleModuleName + "_facts";
            }
        }

        private void CreateMap()
        {
            _map = new MapAnsible();

            var modules = new List<MapAnsibleModule>();
            var operations = new List<string>();

            var oldIndex = CurrentOperationIndex;
            for (int idx = 0; idx < Operations.Count; idx++)
            {
                CurrentOperationIndex = idx;
                string op = Operations[CurrentOperationIndex].Name + "[";
                foreach (var m in Operations[CurrentOperationIndex].Methods)
                {
                    op += " " + m.Name;
                }

                var module = new MapAnsibleModule();

                if ((ModuleCreateOrUpdateMethod != null) || (ModuleCreateMethod != null))
                {
                    op += " MAIN";
                    module.ModuleName = AnsibleModuleName;
                    module.ModuleNameAlt = module.ModuleName;
                    module.Options = (ModuleCreateOrUpdateMethod != null) ? CreateMethodOptions(ModuleCreateOrUpdateMethod.Name, true) : CreateMethodOptions(ModuleCreateMethod.Name, true);

                    var methods = new List<ModuleMethod>();

                    if (ModuleGetMethod != null)
                    {
                        var method = new ModuleMethod();
                        method.Name = ModuleGetMethod.Name;
                        method.Options = GetMethodOptionNames(ModuleGetMethod.Name, false);

                        method.RequiredOptions = GetMethodOptionNames(ModuleGetMethod.Name);
                        methods.Add(method);
                    }

                    if (ModuleCreateOrUpdateMethod != null)
                    {
                        var method = new ModuleMethod();
                        method.Name = ModuleCreateOrUpdateMethod.Name;
                        method.Options = GetMethodOptionNames(ModuleCreateOrUpdateMethod.Name, false);
                        method.RequiredOptions = GetMethodOptionNames(ModuleCreateOrUpdateMethod.Name);
                        methods.Add(method);

                        // XXX - make sure this should be here
                        module.ResponseFields = CreateMethodResponseFields(method.Name);
                    }

                    if (ModuleCreateMethod != null)
                    {
                        var method = new ModuleMethod();
                        method.Name = ModuleCreateMethod.Name;
                        method.Options = GetMethodOptionNames(ModuleCreateMethod.Name, false);
                        method.RequiredOptions = GetMethodOptionNames(ModuleCreateMethod.Name);
                        methods.Add(method);

                        // XXX - make sure this should be here
                        module.ResponseFields = CreateMethodResponseFields(method.Name);
                    }

                    if (ModuleUpdateMethod != null)
                    {
                        var method = new ModuleMethod();
                        method.Name = ModuleUpdateMethod.Name;
                        method.Options = GetMethodOptionNames(ModuleUpdateMethod.Name, false);
                        method.RequiredOptions = GetMethodOptionNames(ModuleUpdateMethod.Name);
                        methods.Add(method);

                        // XXX - make sure this should be here
                        module.ResponseFields = CreateMethodResponseFields(method.Name);
                    }

                    if (ModuleDeleteMethod != null)
                    {
                        var method = new ModuleMethod();
                        method.Name = ModuleDeleteMethod.Name;
                        method.Options = GetMethodOptionNames(ModuleDeleteMethod.Name, false);
                        method.RequiredOptions = GetMethodOptionNames(ModuleDeleteMethod.Name);
                        methods.Add(method);
                    }
                    module.Methods = methods.ToArray();

                    module.AssertStateVariable = AssertStateVariable;
                    module.AssertStateExpectedValue = AssertStateExpectedValue;
                    module.TestPrerequisitesModule = "";
                    module.ModuleOperationName = ModuleOperationName;
                    module.ModuleOperationNameUpper = ModuleOperationNameUpper;
                    module.ObjectName = ModuleOperationNameUpper;

                    modules.Add(module);
                }

                var factMethods = GetModuleFactsMethods();

                if (factMethods.Length > 0)
                {
                    op += " FACTS";
                    module = new MapAnsibleModule();
                    module.ModuleName = AnsibleModuleNameFacts;
                    module.ModuleNameAlt = module.ModuleName;
                    module.Options = CreateModuleFactsOptions();

                    var methods = new List<ModuleMethod>();

                    foreach (var m in GetModuleFactsMethods())
                    {
                        var method = new ModuleMethod();
                        method.Name = m.Name;
                        method.Options = GetMethodOptionNames(m.Name, false);
                        method.RequiredOptions = GetMethodOptionNames(m.Name);
                        methods.Add(method);
                    }

                    module.Methods = methods.ToArray();

                    module.AssertStateVariable = AssertStateVariable;
                    module.AssertStateExpectedValue = AssertStateExpectedValue;
                    module.ModuleOperationName = ModuleOperationName;
                    module.ModuleOperationNameUpper = ModuleOperationNameUpper;
                    module.ObjectName = ModuleOperationNameUpper;

                    modules.Add(module);
                }
                operations.Add(op += " ]");
            }

            CurrentOperationIndex = oldIndex;

            Map.Modules = modules.ToArray();

            Map.Namespace = Namespace;
            Map.NamespaceUpper = NamespaceUpper;
            Map.Name = Name;

            foreach (var group in Operations)
            {
                foreach (var method in group.Methods)
                {
                    var examplesRaw = method.Extensions.GetValue<Newtonsoft.Json.Linq.JObject>(AutoRest.Core.Model.XmsExtensions.Examples.Name);
                    var examples = AutoRest.Core.Model.XmsExtensions.Examples.FromJObject(examplesRaw);
                    foreach (var example in examples)
                    {
                        operations.Add("EXAMPLE -- " + example.Key);
                    }
                }
            }

            Map.Operations = operations.ToArray();
        }

        public override string Namespace
        {
            get { return base.Namespace; }
            set
            {
                value = value.Split(".").Last();
                _namespace_upper = value;
                base.Namespace = value;
            }
        }

        private string NamespaceUpper
        {
            get { return _namespace_upper; }
        }

        public string ModuleOperationName
        {
            get { return ModuleOperation.Name.ToPythonCase(); }
        }

        public string ModuleOperationNameUpper
        {
            get { return ModuleOperation.Name; }
        }

        public int CurrentOperationIndex
        {
            get; set;
        }

        public MethodGroup ModuleOperation
        {
            get { return Operations[CurrentOperationIndex]; }
        }

        public Method ModuleCreateOrUpdateMethod
        {
            get
            {
                return ModuleFindMethod("create_or_update");
            }
        }

        public Method ModuleCreateMethod
        {
            get
            {
                return ModuleFindMethod("create");
            }
        }

        public Method ModuleUpdateMethod
        {
            get
            {
                return ModuleFindMethod("update");
            }
        }

        private string ModelTypeNameToYamlTypeName(IModelType type)
        {
            string name = type.Name;
            if (type.Name == "list")
            {
                SequenceType list = type as SequenceType;
                name = list.ElementType.Name;
            }

            if (name != "str" && name != "int" && name != "float" && name != "list" && name != "dict" && name != "long" && name != "datetime" && name != "long")
            {
                // if it's in the list of models we'll return dictionary, otherwise string
                foreach (var mt in ModelTypes)
                {
                    if (mt.Name == name)
                    {
                        name = "dict";
                        break;
                    }
                }

                if (name != "dict") name = "str";

            }

            return name;
        }

        private ModuleResponseField[] CreateMethodResponseFields(string methodName)
        {
            var fields = new List<ModuleResponseField>();
            var method = ModuleFindMethod(methodName);

            if (method != null)
            {
                string responseModel = method.ReturnType.Body.ClassName;

                var suboptions = GetModelFields(responseModel);
                fields.AddRange(suboptions);
            }

            return fields.ToArray();
        }

        private ModuleOption[] CreateMethodOptions(string methodName, bool flatten = false)
        {
            var option = new List<ModuleOption>();
            var method = ModuleFindMethod(methodName);

            if (method != null)
            {
                foreach (var p in method.Parameters)
                {
                    if (p.Name != "self.config.subscription_id" && p.Name != "api_version")
                    {
                        string type = ModelTypeNameToYamlTypeName(p.ModelType);

                        if (type != "dict")
                        {
                            var newParam = new ModuleOption(p.Name, type, p.IsRequired ? "True" : "False", "None");

                            newParam.IsList = (p.ModelTypeName == "list");
                            newParam.Documentation = p.Documentation;
                            option.Add(newParam);
                        }
                        else
                        {
                            if (flatten)
                            {
                                // add hidden dictionary option here anyway to store all flattened values
                                var suboption = new ModuleOption(p.Name, "dict", "False", "dict()");
                                suboption.Disposition = "dictionary";
                                suboption.Documentation = p.Documentation;
                                suboption.IsList = false;
                                option.Add(suboption);
                                var suboptions = GetModelOptions(p.ModelTypeName);
                                foreach (var o in suboptions) o.Disposition = p.Name;
                                option.AddRange(suboptions);
                            }
                            else
                            {
                                var suboption = new ModuleOption(p.Name, type, p.IsRequired ? "True" : "False", "dict()");
                                suboption.IsList = (p.ModelTypeName == "list");
                                suboption.Documentation = p.Documentation;
                                suboption.SubOptions = GetModelOptions(suboption.IsList ? ((p.ModelType as SequenceType).ElementType.Name.FixedValue) : p.ModelTypeName);
                                option.Add(suboption);
                            }
                        }
                    }
                }
            }

            return option.ToArray();
        }

        private ModuleOption[] GetModelOptions(string modelName)
        {
            // [ZKK] this is a very bad hack for SQL Server
            if (modelName == "ServerPropertiesForCreate")
                modelName = "ServerPropertiesForDefaultCreate";

            CompositeTypePy model = GetModelTypeByName(modelName);
            var options = new List<ModuleOption>();

            if (model != null)
            {
                foreach (var attr in model.ComposedProperties)
                {
                    if (!attr.IsReadOnly)
                    {
                        string type = ModelTypeNameToYamlTypeName(attr.ModelType);
                        string modelTypeName = attr.ModelTypeName;
                        var option = new ModuleOption(attr.Name, type, attr.IsRequired ? "True" : "False", "None");
                        if (attr.ModelTypeName == "list")
                        {
                            string subtype = ModelTypeNameToYamlTypeName(attr.ModelType);
                            option.IsList = true;
                           
                            SequenceType list = attr.ModelType as SequenceType;

                            modelTypeName = list.ElementType.Name;
                        }
                        else
                        {
                            modelTypeName = attr.ModelTypeName;
                        }
                        option.Documentation = attr.Documentation;
                        option.SubOptions = GetModelOptions(modelTypeName);
                        options.Add(option);
                    }
                }
            }

            return options.ToArray();
        }

        private ModuleResponseField[] GetModelFields(string modelName)
        {
            CompositeTypePy model = GetModelTypeByName(modelName);
            var fields = new List<ModuleResponseField>();

            if (model != null)
            {
                foreach (var attr in model.ComposedProperties)
                {
                    string type = ModelTypeNameToYamlTypeName(attr.ModelType);
                    type = (type == "dict") ? "complex" : type;
                    string modelTypeName = attr.ModelTypeName;
                    var field = new ModuleResponseField(attr.Name, type, attr.Documentation, attr.Name);
                    field.Returned = "always";
                    if (attr.ModelTypeName == "list")
                    {
                        string subtype = ModelTypeNameToYamlTypeName(attr.ModelType);
                        //option.IsList = true;

                        SequenceType list = attr.ModelType as SequenceType;

                        modelTypeName = list.ElementType.Name;
                    }
                    else
                    {
                        modelTypeName = attr.ModelTypeName;
                    }
                    field.Description = attr.Documentation;
                    field.SubFields = GetModelFields(modelTypeName);
                    fields.Add(field);
                }
            }

            return fields.ToArray();
        }

        private string[] GetMethodOptionNames(string methodName, bool required = true)
        {
            var options = new List<string>();
            var method = ModuleFindMethod(methodName);

            if (method != null)
            {
                foreach (var p in method.Parameters)
                {
                    if (p.Name != "self.config.subscription_id" && p.Name != "api_version" && (p.IsRequired == true || !required))
                    {
                        options.Add(p.Name);
                    }
                }
            }

            return options.ToArray();
        }

        private ModuleOption[] CreateModuleFactsOptions()
        {
            var options = new Dictionary<string, ModuleOption>();
            Method[] methods = GetModuleFactsMethods();

            foreach (var m in methods)
            {
                foreach (var p in m.Parameters)
                {
                    if (p.Name != "self.config.subscription_id" && p.Name != "api_version")
                    {
                        string type = ModelTypeNameToYamlTypeName(p.ModelType);

                        if (!options.Keys.Contains<string>(p.Name))
                        {
                            options[p.Name] = new ModuleOption(p.Name, type, p.IsRequired ? "True" : "False", "None");
                            options[p.Name].Documentation = p.Documentation.RawValue;
                            //options[p.Name].ModelTypeName = p.ModelTypeName;
                        }

                        if (p.IsRequired) options[p.Name].RequiredCount++;
                    }
                }
            }

            var arr = new List<ModuleOption>(options.Values).ToArray();

            foreach (var p in arr)
            {
                p.Required = (p.RequiredCount == methods.Length) ? "True" : "False";
            }

            return arr;
        }

        private Method ModuleFindMethod(string name)
        {
            foreach (var m in ModuleOperation.Methods)
            {
                if (m.Name == name)
                    return m;
            }

            return null;
        }


        public Method ModuleGetMethod
        {
            get
            {
                return ModuleFindMethod("get");
            }
        }

        public Method ModuleDeleteMethod
        {
            get
            {
                return ModuleFindMethod("delete");
            }
        }

        private Method[] GetModuleFactsMethods()
        {
            var l = new List<Method>();

            foreach (var m in ModuleOperation.Methods)
            {
                if (m.Name.StartsWith("list_") || m.Name == "get")
                {
                    l.Add(m);
                }
            }

            var sorted = (from e in l orderby -e.Parameters.Count select e).ToList();

            return sorted.ToArray();
        }

        public string AssertStateVariable
        {
            get
            {
                switch (Namespace)
                {
                    case "databases": return "status";
                }

                return "state";
            }
        }

        public string AssertStateExpectedValue
        {
            get
            {
                switch (Namespace)
                {
                    case "databases": return "Online";
                }

                return "Ready";

            }
        }

        private CompositeTypePy GetModelTypeByName(string name)
        {
            foreach (var m in ModelTemplateModels)
            {
                if (m.Name == name)
                    return m;
            }
            return null;
        }

        public static string Indent(string original)
        {
            char[] a = original.ToCharArray();

            for (int i = 0; i < a.Length; i++) a[i] = ' ';

            return new string(a);
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

            return new string(a);
        }
    }
}
