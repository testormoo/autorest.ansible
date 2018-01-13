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
                string multi = (Operations.Count > 1) ? Namespace : "";
                string sub = ModuleOperationNameUpper.ToLower();
                if (sub.StartsWith(multi)) multi = "";
                string name = "azure_rm_" + multi + sub;

                // let's try to be smart here, as all operation names are plural so let's try to make it singular
                if (name.EndsWith("ies"))
                {
                    name = name.Substring(0, name.Length - 3) + "y";
                }
                else if (name.EndsWith('s'))
                {
                    name = name.Substring(0, name.Length - 1);
                }

                return name;
            }
        }

        private string AnsibleModuleNameFacts
        {
            get
            {
                return AnsibleModuleName + "_facts";
            }
        }

        private string ObjectName
        {
            get
            {
                // XXX - handle following rules
                // Nat --> NAT
                // I P --> IP
                // Sql --> SQL

                string name = ModuleOperationNameUpper;

                if (name.EndsWith("ies"))
                {
                    name = name.Substring(0, name.Length - 3) + "y";
                }
                else if (name.EndsWith('s'))
                {
                    name = name.Substring(0, name.Length - 1);
                }

                name = System.Text.RegularExpressions.Regex.Replace(name, "([A-Z])", " $1", System.Text.RegularExpressions.RegexOptions.Compiled).Trim();

                return name;
            }
        }

        private void CreateMap()
        {
            _map = new MapAnsible();

            var modules = new List<MapAnsibleModule>();

            var oldIndex = CurrentOperationIndex;
            for (int idx = 0; idx < Operations.Count; idx++)
            {
                Map.Info.Add("------------------------------- " + this.ObjectName);

                CurrentOperationIndex = idx;
                string op = Operations[CurrentOperationIndex].Name + "[";
                foreach (var m in Operations[CurrentOperationIndex].Methods)
                {
                    op += " " + m.Name;
                    Map.Info.Add("  " + m.Name);
                }

                var module = new MapAnsibleModule();

                if ((ModuleCreateOrUpdateMethod != null) || (ModuleCreateMethod != null))
                {
                    Map.Info.Add("  ** CREATING MAIN MODULE");
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

                        module.ResponseFields = GetResponseFieldsForMethod(method.Name, true);
                    }

                    if (ModuleCreateOrUpdateMethod != null)
                    {
                        var method = new ModuleMethod();
                        method.Name = ModuleCreateOrUpdateMethod.Name;
                        method.Options = GetMethodOptionNames(ModuleCreateOrUpdateMethod.Name, false);
                        method.RequiredOptions = GetMethodOptionNames(ModuleCreateOrUpdateMethod.Name);
                        methods.Add(method);

                        // XXX - make sure this should be here
                        module.ResponseFields = GetResponseFieldsForMethod(method.Name, false);
                    }

                    if (ModuleCreateMethod != null)
                    {
                        var method = new ModuleMethod();
                        method.Name = ModuleCreateMethod.Name;
                        method.Options = GetMethodOptionNames(ModuleCreateMethod.Name, false);
                        method.RequiredOptions = GetMethodOptionNames(ModuleCreateMethod.Name);
                        methods.Add(method);

                        // XXX - make sure this should be here
                        module.ResponseFields = GetResponseFieldsForMethod(method.Name, false);
                    }

                    if (ModuleUpdateMethod != null)
                    {
                        var method = new ModuleMethod();
                        method.Name = ModuleUpdateMethod.Name;
                        method.Options = GetMethodOptionNames(ModuleUpdateMethod.Name, false);
                        method.RequiredOptions = GetMethodOptionNames(ModuleUpdateMethod.Name);
                        methods.Add(method);

                        // XXX - make sure this should be here
                        module.ResponseFields = GetResponseFieldsForMethod(method.Name, false);
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
                    module.ObjectName = ObjectName;

                    modules.Add(module);
                }

                var factMethods = GetModuleFactsMethods();

                if (factMethods.Length > 0)
                {
                    Map.Info.Add("  ** CREATING FACT MODULE");
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

                    if (ModuleGetMethod != null)
                    {
                        module.ResponseFields = GetResponseFieldsForMethod(ModuleGetMethod.Name, true);
                    }

                    module.Methods = methods.ToArray();

                    module.AssertStateVariable = AssertStateVariable;
                    module.AssertStateExpectedValue = AssertStateExpectedValue;
                    module.ModuleOperationName = ModuleOperationName;
                    module.ModuleOperationNameUpper = ModuleOperationNameUpper;
                    module.ObjectName = ObjectName;

                    modules.Add(module);
                }
            }

            CurrentOperationIndex = oldIndex;

            Map.Modules = modules.ToArray();

            Map.Namespace = Namespace;
            Map.NamespaceUpper = NamespaceUpper;
            Map.Name = Name;

            Map.ApiVersion = "";// ApiVersion;
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

        private KeyValuePair<string,string>[] ModelTypeEnumValues(IModelType type)
        {
            List<KeyValuePair<string,string>> list = new List<KeyValuePair<string,string>>();
            if (type.Qualifier == "Enum")
            {
                foreach (var v in (type as EnumType).Values)
                {
                    list.Add(new KeyValuePair<string,string>(v.Name.ToPythonCase(), v.Name));
                }
            }
            return list.ToArray();
        }
        private string ModelTypeNameToYamlTypeName(IModelType type)
        {
            string name = type.Name;
            if (type.Name == "list")
            {
                SequenceType list = type as SequenceType;
                name = list.ElementType.Name;
            }

            //if (type.Qualifier == "Enum")
            //{
            //    name = "enum";
            //}

            if (name != "str" && /*name != "enum" &&*/ name != "int" && name != "float" && name != "list" && name != "dict" && name != "long" && name != "datetime" && name != "long")
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

        private ModuleResponseField[] GetResponseFieldsForMethod(string methodName, bool alwaysInclude)
        {
            var fields = new List<ModuleResponseField>();
            var method = ModuleFindMethod(methodName);

            this.Map.Info.Add("Getting example for: " + methodName);

            // just get first example for this method
            var examples = ModuleFindMethodSamples(methodName);
            Newtonsoft.Json.Linq.JToken v = null;
            var example = examples.IsNullOrEmpty() ? null : examples.First().Value;
            if (example != null)
            {
                this.Map.Info.Add("Found example for: " + methodName);
                AutoRest.Core.Model.XmsExtensions.ExampleResponse r = null;
                example.Responses.TryGetValue("200", out r);

                if (r != null)
                {
                    this.Map.Info.Add("Found response for: " + methodName);
                    v = r.Body;

                    // XXX - this is a hack
                    // XXX - if bodu contains "properties" -- flatten them
                    Newtonsoft.Json.Linq.JObject propertiesAttr = null;
                    Newtonsoft.Json.Linq.JObject vo = v as Newtonsoft.Json.Linq.JObject;

                    if (vo != null)
                    {
                        foreach (var pp in vo.Properties())
                        {
                            if (pp.Name == "properties")
                            {
                                propertiesAttr = pp.Value as Newtonsoft.Json.Linq.JObject;
                                this.Map.Info.Add("Found properties");
                            }
                        }
                    }

                    if (propertiesAttr != null)
                    {
                        foreach (var pp in propertiesAttr.Properties())
                        {
                            try
                            {
                                vo.Add(pp);
                            }
                            catch (Exception e)
                            {
                                Map.Info.Add("Error copying property --- " + pp.Name);
                            }
                        }
                    }

                    if (v != null)
                    {
                        this.Map.Info.Add("Sample after properties copied ---");
                        this.Map.Info.Add(v.ToString());
                    }
                }
            }

            // QQQQQQQQQQ
            if (method != null)
            {
                string responseModel = method.ReturnType.Body.ClassName;

                var suboptions = GetResponseFieldsForModel(responseModel, 0, v, alwaysInclude);
                fields.AddRange(suboptions);
            }

            return fields.ToArray();
        }

        private ModuleOption[] CreateMethodOptions(string methodName, bool flatten = false)
        {
            var option = new List<ModuleOption>();
            var method = ModuleFindMethod(methodName);
            var examples = ModuleFindMethodSamples(methodName);
            Newtonsoft.Json.Linq.JToken v = null;

            // XXX - for now get first example
            var example = examples.IsNullOrEmpty() ? null : examples.First().Value;

            if (method != null)
            {
                foreach (var p in method.Parameters)
                {
                    if (p.Name != "self.config.subscription_id" && p.Name != "api_version" && p.Name != "tags")
                    {
                        // QQQQQQQQQQ
                        string type = ModelTypeNameToYamlTypeName(p.ModelType);
                        if (example != null) example.Parameters.TryGetValue(p.SerializedName, out v);

                        if (type != "dict")
                        {
                            var newParam = new ModuleOption(p.Name, type, p.IsRequired ? "True" : "False", "None");

                            newParam.IsList = (p.ModelTypeName == "list");
                            newParam.Documentation = p.Documentation;
                            newParam.NoLog = p.Name.Contains("password");
                            newParam.DefaultValueSample["default"] = (v != null) ? v.ToString() : "NOT FOUND";
                            newParam.EnumValues = ModelTypeEnumValues(p.ModelType);

                            if (newParam.EnumValues.Length > 0)
                            {
                                newParam.Documentation = newParam.Documentation.Split(" Possible values include:")[0];
                            }

                            newParam.AdditionalInfo = ((p.ModelType.XmlProperties != null) ? p.ModelType.XmlProperties.ToString() : "NO XML PROPERTIES") + " --- " + p.ModelType.Qualifier;
                            option.Add(newParam);
                        }
                        else
                        {
                            if (flatten)
                            {
                                // add hidden dictionary option here anyway to store all flattened values
                               
                                var suboption = new ModuleOption(p.Name, "dict", "False", "dict()");
                                suboption.IsList = (p.ModelTypeName == "list");
                                suboption.Disposition = "dictionary";
                                suboption.Documentation = p.Documentation;
                                suboption.IsList = false;
                                option.Add(suboption);

                                if (suboption.Name.EndsWith("_parameters"))
                                    suboption.NameAlt = "parameters";

                                var suboptions = GetModelOptions(p.ModelTypeName, 0, v);
                                foreach (var o in suboptions) o.Disposition = p.Name;
                                option.AddRange(suboptions);
                            }
                            else
                            {
                                var suboption = new ModuleOption(p.Name, type, p.IsRequired ? "True" : "False", "dict()");
                                suboption.IsList = (p.ModelTypeName == "list");
                                suboption.Documentation = p.Documentation;

                                suboption.SubOptions = GetModelOptions(suboption.IsList ? ((p.ModelType as SequenceType).ElementType.Name.FixedValue) : p.ModelTypeName, 0, v);
                                option.Add(suboption);
                            }
                        }
                    }
                }
            }

            return option.ToArray();
        }

        private ModuleOption[] GetModelOptions(string modelName, int level, Newtonsoft.Json.Linq.JToken sampleValue)
        {
            // [ZKK] this is a very bad hack for SQL Server
            if (modelName == "ServerPropertiesForCreate")
                modelName = "ServerPropertiesForDefaultCreate";

            CompositeTypePy model = GetModelTypeByName(modelName);
            var options = new List<ModuleOption>();
            AutoRest.Core.Model.Parameter p;

            if (level < 5)
            {
                if (model != null)
                {
                    foreach (Property attr in model.ComposedProperties)
                    {
                        if (attr.Name != "tags" && !attr.IsReadOnly)
                        {
                            string attrName = attr.Name;
                            try { attrName = attr.XmlName; } catch (Exception e) { }

                            Newtonsoft.Json.Linq.JToken subSampleValue = null;
                            Newtonsoft.Json.Linq.JObject sampleValueObject = sampleValue as Newtonsoft.Json.Linq.JObject;

                            if (sampleValueObject != null)
                            {
                                foreach (var pp in sampleValueObject.Properties())
                                {
                                    //look += " " + pp.Name; 
                                    if (pp.Name == attrName)
                                    {
                                        subSampleValue = pp.Value;
                                    }
                                }
                            }

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
                            option.NoLog = attr.Name.Contains("password");
                            option.AdditionalInfo = ((attr.ModelType.XmlProperties != null) ? attr.ModelType.XmlProperties.ToString() : "NO XML PROPERTIES") + " --- " + attr.ModelType.Qualifier;
                            option.EnumValues = ModelTypeEnumValues(attr.ModelType);

                            if (option.EnumValues.Length > 0)
                            {
                                option.Documentation = option.Documentation.Split(" Possible values include:")[0];
                            }

                            option.DefaultValueSample["default"] = (subSampleValue != null) ? subSampleValue.ToString() : "";

                            // XXX - get next level of sample value
                            option.SubOptions = GetModelOptions(modelTypeName, level + 1, subSampleValue);
                            options.Add(option);
                        }
                    }
                }
            }

            return options.ToArray();
        }

        private ModuleResponseField[] GetResponseFieldsForModel(string modelName, int level, Newtonsoft.Json.Linq.JToken sampleResponse, bool alwaysInclude)
        {
            CompositeTypePy model = GetModelTypeByName(modelName);
            var fields = new List<ModuleResponseField>();

            if (model != null && level < 5)
            {
                foreach (var attr in model.ComposedProperties)
                {
                    string type = ModelTypeNameToYamlTypeName(attr.ModelType);
                    type = (type == "dict") ? "complex" : type;
                    string modelTypeName = attr.ModelTypeName;

                    //-------------------------------------------------------------
                    Newtonsoft.Json.Linq.JToken sampleResponseField = null;
                    if (sampleResponse != null)
                    {
                        Newtonsoft.Json.Linq.JObject sampleResponseObject = sampleResponse as Newtonsoft.Json.Linq.JObject;

                        if (sampleResponseObject != null)
                        {
                            foreach (var pp in sampleResponseObject.Properties())
                            {
                                if (pp.Name == attr.Name)
                                {
                                    sampleResponseField = pp.Value;
                                }
                            }
                        }
                    }

                    //-------------------------------------------------------------


                    var field = new ModuleResponseField(attr.Name, type, attr.Documentation, attr.Name);

                    // XXX - currently there's a problem with tags
                    if (alwaysInclude && sampleResponseField != null && attr.Name != "tags")
                    {
                        field.NameAlt = attr.Name;
                    }

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
                    field.SubFields = GetResponseFieldsForModel(modelTypeName, level + 1, sampleResponseField, alwaysInclude);
                    field.Info = (sampleResponseField != null) ? sampleResponseField.ToString() : "NO SAMPLE DATA";

                    if (field.SubFields.Length == 0 && sampleResponseField != null && sampleResponseField.ToString() != "")
                    {
                        field.SampleValue = sampleResponseField.ToString();
                    }

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

        private Dictionary<string, Core.Model.XmsExtensions.Example> ModuleFindMethodSamples(string name)
        {
            var method = ModuleFindMethod(name);
            var examplesRaw = method.Extensions.GetValue<Newtonsoft.Json.Linq.JObject>(AutoRest.Core.Model.XmsExtensions.Examples.Name);
            return AutoRest.Core.Model.XmsExtensions.Examples.FromJObject(examplesRaw);
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

                return "";
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