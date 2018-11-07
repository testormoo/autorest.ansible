using System;
using System.Collections;
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

        private int _currentOperation = -1;
        private int _currentMethod = 0;
        IEnumerator<System.Collections.Generic.KeyValuePair<string, Newtonsoft.Json.Linq.JToken>> _examples = null;

        public bool SelectFirstExample()
        {
            _currentOperation = 0;
            _currentMethod = -1;
            _examples = null;
            return SelectNextExample();
        }

        public bool SelectNextExample()
        {
            if (_examples != null)
            {
                if (_examples.MoveNext())
                    return true;
                    
                _examples = null;
            }

            _currentMethod++;

            while (_examples == null)
            {
                var operation = (_currentOperation >= 0 && _currentOperation < Operations.Count) ? Operations[_currentOperation] : null;

                if (operation != null)
                {
                    this.Map.Info.Add(" .... OPERATION IS NOT NULL");
                    var method = (_currentMethod >= 0 && _currentMethod < operation.Methods.Count) ? operation.Methods[_currentMethod] : null;
                    if (method != null)
                    {
                        this.Map.Info.Add(" .... METHOD IS NOT NULL");
                        var d = method.Extensions.GetValue<Newtonsoft.Json.Linq.JObject>(AutoRest.Core.Model.XmsExtensions.Examples.Name);
                        
                        if (d != null)
                        {
                            this.Map.Info.Add(" .... FOUND EXAMPLES");
                            _examples = d.GetEnumerator();
                        }
                        else
                        {
                            this.Map.Info.Add(" .... NO EXAMPLES");
                        }
                    }
                    else
                    {
                        this.Map.Info.Add(" .... METHOD IS NULL " + _currentMethod.ToString());
                    }
                }
                else
                {
                    this.Map.Info.Add(" .... OPERATION IS NULL " + _currentOperation.ToString());
                }

                if ((_examples == null) || !_examples.MoveNext())
                {
                    _currentMethod++;

                    if (_currentMethod >= Operations[_currentOperation].Methods.Count)
                    {
                        _currentOperation++;
                        _currentMethod = 0;

                        if (_currentOperation >= Operations.Count)
                            return false;
                    }

                    _examples = null;
                }
            }

            this.Map.Info.Add("CURRENT EXAMPLE NAME " + GetExampleName());
            return true;
        }

        public string GetExampleName()
        {
            return _examples.Current.Key;
        }

        public string[] GetYaml()
        {
            List<string> template = new List<string>();
            List<string> ignore = new List<string>();

            ignore.Add("api-version");

            var method = Operations[_currentOperation].Methods[_currentMethod];
            var example = _examples.Current.Value;
            string[] url = method.Url.Split("/");

            template.Add("- hosts: localhost");
            template.Add("  tasks:");
            template.Add("");
            template.Add("    - name: Call REST API");
            // add method and use appropriate module
            if (method.HttpMethod.ToString().ToLower() == "get")
            {
                template.Add("      azure_rm_resource_facts:");
            }
            else
            {
                template.Add("      azure_rm_resource:");

                if (method.HttpMethod.ToString().ToLower() != "put")
                {
                    template.Add("        method: " + method.HttpMethod.ToString().ToUpper());
                }
            }
            // add api version

            // handle url
            // XXX
            template.Add("        api_version: '" + ApiVersion + "'");

            bool resource = false;
            bool subresource = false;
            bool subresourceAdded = false;

            template.Add("        # url: " + method.Url);

            for (int i = 0; i < url.Length; i++)
            {
                if (url[i].StartsWith("{"))
                {
                    string p = url[i].Substring(1, url[i].Length - 2);

                    if (url[i - 1] == "resourceGroups")
                    {
                        template.Add("        resource_group: " + example["parameters"][p]);
                    }
                    else if (subresource)
                    {
                        template.Add("            name:" +   example["parameters"][p]);
                    }
                    else if (resource)
                    {
                        template.Add("        resource_name: " +  example["parameters"][p]);
                        subresource = true;
                    }

                    // XXX - resource
                    // XXX - subresource

                    ignore.Add(p);
                }
                else
                {
                    if ((i > 0) && url[i - 1] == "providers")
                    {
                        template.Add("        provider: " + url[i].Split(".").Last());
                        resource = true;
                    }
                    else if (subresource)
                    {
                        if (!subresourceAdded)
                        {
                            template.Add("        subresource:" );
                            subresourceAdded = true;
                        }

                        template.Add("          - type:" +  url[i]);
                    }
                    else if (resource)
                    {
                        template.Add("        resource_type: " + url[i]);
                    }
                }
            }

            template.Add("        body:");
            template.AddRange(GetRestExampleBodyYaml(example["parameters"], "          ", ignore.ToArray()));
            return template.ToArray();
        }

        private string[] GetRestExampleBodyYaml(Newtonsoft.Json.Linq.JToken v, string prefix, string[] ignoreFields)
        {
            List<string> template = new List<string>();
            // check if dict or list, or value
            Newtonsoft.Json.Linq.JObject vo = v as Newtonsoft.Json.Linq.JObject;
            Newtonsoft.Json.Linq.JArray va = v as Newtonsoft.Json.Linq.JArray;

            //if (sampleValueArray != null && sampleValueArray.Count > 0)
        
            if (vo != null)
            {
                // dictionary -- 
                foreach (var pp in vo.Properties())
                {
                    if (ignoreFields != null)
                    {
                        bool ignore = false;
                        for (int i = 0; i < ignoreFields.Length; i++)
                        {
                            if (ignoreFields[i] == pp.Name) ignore = true;
                        }

                        if (ignore) continue;
                    }

                    Newtonsoft.Json.Linq.JValue subv = pp.Value as  Newtonsoft.Json.Linq.JValue;

                    if (subv != null)
                    {
                        template.Add(prefix + pp.Name + ": " + subv.ToString());
                    }
                    else
                    {
                        template.Add(prefix + pp.Name + ":");
                        template.AddRange(GetRestExampleBodyYaml(pp.Value, prefix + "  ", null));
                    }
                }
            }
            else if (va != null)
            {
                // va.Count
                for (int i = 0; i < va.Count; i++)
                {
                    string[] subitem = GetRestExampleBodyYaml(va[i], "", null);
                    for (int j = 0; j < subitem.Length; j++)
                    {
                        template.Add(prefix + ((j == 0) ? "- " : "  ") + subitem[j]);
                    }
                }
            }

            return template.ToArray();
        }

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
                    module.Options = (ModuleCreateOrUpdateMethod != null) ? CreateMethodOptions(ModuleCreateOrUpdateMethod.Name) : CreateMethodOptions(ModuleCreateMethod.Name);

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
                    module.ObjectNamePlural = ObjectName + "s";
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
                    module.ObjectNamePlural = ObjectName + "s";

                    UpdateResourceNameFields(module);

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

        private void UpdateResourceNameFields(MapAnsibleModule m)
        {
            // try to set default resource name fields
            string firstSuggestedName = ModuleOperationNameSingular + "_name";
            m.ResourceNameFieldInRequest = firstSuggestedName;
            m.ResourceNameFieldInResponse = firstSuggestedName;


            // Verify that field exists in options, if not, we will find last field ending with "_name"
            if (m.Options != null)
            {
                bool found = false;
                string lastNameField = null;

                foreach (var o in m.Options)
                {
                    if (o.Name.EndsWith("_name"))
                        lastNameField = o.Name;
                    
                    if (o.Name == m.ResourceNameFieldInRequest)
                        found = true;
                }

                if (!found)
                {
                    m.ResourceNameFieldInRequest = lastNameField;
                    m.ResourceNameFieldInResponse = lastNameField;
                }
            }

            // Check if response fields contains "name". If it does that will be our resource field name in response
            if (m.ResponseFields != null)
            {
                foreach (var rf in m.ResponseFields)
                    if (rf.Name == "name")
                        m.ResourceNameFieldInResponse = "name";
            }

            Map.Info.Add("  resource name fields: " +  firstSuggestedName + " " + m.ResourceNameFieldInRequest + " " + m.ResourceNameFieldInResponse);
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

        public string ModuleOperationNameSingular
        {
            get
            {
                string name = ModuleOperation.Name.ToPythonCase();
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
            if (method != null && method.ReturnType != null && method.ReturnType.Body != null)
            {
                string responseModel = method.ReturnType.Body.ClassName;
                var suboptions = GetResponseFieldsForModel(responseModel, 0, v, alwaysInclude);
                fields.AddRange(suboptions);
            }

            return fields.ToArray();
        }

        private ModuleOption[] CreateMethodOptions(string methodName)
        {
            var option = new List<ModuleOption>();
            var method = ModuleFindMethod(methodName);
            var examples = ModuleFindMethodSamples(methodName);
            Newtonsoft.Json.Linq.JToken v = null;

            // XXX - for now get first example
            var example = examples.IsNullOrEmpty() ? null : examples.First().Value;

            if (!examples.IsNullOrEmpty())
            {
                this.Map.Info.Add("---SFM--- " + methodName);
                foreach(KeyValuePair<string, Newtonsoft.Json.Linq.JToken> kvp in example.Parameters) {
                    this.Map.Info.Add(" .... " + kvp.Key);
                }
            }
            else
            {
                this.Map.Info.Add("---SNF--- " + methodName);
            }

            if (method != null)
            {
                foreach (var p in method.Parameters)
                {
                    if (p.Name != "self.config.subscription_id" && p.Name != "api_version" && p.Name != "tags")
                    {
                        // QQQQQQQQQQ
                        string type = ModelTypeNameToYamlTypeName(p.ModelType);
                        if (example != null)
                        {                 
                            example.Parameters.TryGetValue(p.SerializedName, out v);
                            if (v == null)
                            {
                                this.Map.Info.Add(" .... NOT FOUND: " + p.SerializedName);
                            }
                            else
                            {
                                this.Map.Info.Add(" .... FOUND: " + p.SerializedName);
                            }
                        }

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
                            var suboption = new ModuleOption(p.Name, type, p.IsRequired ? "True" : "False", "dict()");
                            suboption.IsList = (p.ModelTypeName == "list");
                            suboption.Documentation = p.Documentation;

                            if (suboption.Name.EndsWith("_parameters") || suboption.Name == "parameters")
                            {
                                suboption.NameAlt = "parameters";
                                suboption.Collapsed = true;
                            }

                            if (suboption.Name == "properties")
                            {
                                suboption.Collapsed = true;
                            }

                            this.Map.Info.Add("--------- GETTING SUBOPTIONS OF " + suboption.Name);
                            suboption.SubOptions = GetModelOptions(suboption.IsList ? ((p.ModelType as SequenceType).ElementType.Name.FixedValue) : p.ModelTypeName, 0, v);
                            option.Add(suboption);
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

            this.Map.Info.Add(">>>>>>>>>>>>>>>>>>>>> SUB: " + modelName);
            CompositeTypePy model = GetModelTypeByName(modelName);
            var options = new List<ModuleOption>();
            AutoRest.Core.Model.Parameter p;
            bool idOnly = false;

            if (level < 5)
            {
                if (model != null)
                {
                    // first check if model contains "id", if it does, most likely rest of the proprties are part of another module and this is just a reference

                    //if (level >= 1)
                    //foreach (Property attr in model.ComposedProperties)
                    //{
                    //    if (attr.Name == "id" && attr.Documentation == "Resource ID.")
                    //    {
                    //        idOnly = true;
                    //        break;
                    //    }
                    //}


                    foreach (Property attr in model.ComposedProperties)
                    {
                        if (idOnly && attr.Name != "id")
                            continue;

                        if (attr.Name != "tags" && !attr.IsReadOnly)
                        {
                            this.Map.Info.Add("--------- PROCESSING " + attr.Name);

                            string attrName = attr.Name;
                            try { attrName = attr.XmlName; } catch (Exception e) { }

                            this.Map.Info.Add("--------- PROCESSING " + attr.Name + "/" + attrName);
                            Newtonsoft.Json.Linq.JToken subSampleValue = null;
                            Newtonsoft.Json.Linq.JObject sampleValueObject = sampleValue as Newtonsoft.Json.Linq.JObject;
                            Newtonsoft.Json.Linq.JArray sampleValueArray = sampleValue as Newtonsoft.Json.Linq.JArray;

                            if (sampleValueArray != null && sampleValueArray.Count > 0)
                            {
                                sampleValueObject = sampleValueArray[0] as Newtonsoft.Json.Linq.JObject;
                            }

                            if (sampleValue != null)
                            {
                                this.Map.Info.Add("--------- SAMPLE: " + sampleValue.ToString());
                            }

                            if (sampleValue == null)
                            {
                                this.Map.Info.Add("--------- NO SAMPLE VALUE");
                            }

                            if (sampleValueObject == null)
                            {
                                this.Map.Info.Add("--------- NO SAMPLE VALUE OBJECT");
                            }

                            if (sampleValueObject != null)
                            {
                                foreach (var pp in sampleValueObject.Properties())
                                {
                                    this.Map.Info.Add(" .... SAMPLE: " + pp.Name + " - " + pp.Value.ToString());
                                    //look += " " + pp.Name; 
                                    if (pp.Name == attrName)
                                    {
                                        this.Map.Info.Add(" .... FOUND");
                                        subSampleValue = pp.Value;
                                    }
                                    else if (pp.Name == "properties")
                                    {
                                        Newtonsoft.Json.Linq.JObject properties = pp.Value as Newtonsoft.Json.Linq.JObject;

                                        foreach (var ppp in properties.Properties())
                                        {
                                            this.Map.Info.Add(" .... SAMPLE (PP): " + ppp.Name + " - " + ppp.Value.ToString());
                                            //look += " " + pp.Name; 
                                            if (ppp.Name == attrName)
                                            {
                                                this.Map.Info.Add(" .... FOUND");
                                                subSampleValue = ppp.Value;
                                            }
                                        }
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

                            // some options should be marked as collapsed by default
                            if (option.Name == "properties")
                            {
                                option.Collapsed = true;
                            }

                            options.Add(option);
                        }
                    }
                }
            }
            this.Map.Info.Add("<<<<<<<<<<<<<<<<<<<<<");
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
                if (m.Name.StartsWith("list_by") || m.Name == "get"/* || m.Name == "list"*/)
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
                    case (char)0x2013:
                        a[i] = '-';
                        break;
                }
            }

            return new string(a);
        }
    }
}
