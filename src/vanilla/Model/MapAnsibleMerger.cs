using System;
using System.Collections.Generic;
using System.Text;

namespace AutoRest.Ansible.Model
{
    class MapAnsibleMerger
    {
        public MapAnsibleMerger(MapAnsible oldMap, MapAnsible newMap)
        {
            _old = oldMap;
            _new = newMap;

            _merged = new MapAnsible();
            _output = new List<string>();

            _output.Add("Used existing template file");

            MergeMaps();
        }

        public MapAnsible MergedMap
        {
            get
            {
                return _merged;
            }
        }

        public string[] Report
        {
            get
            {
                return _output.ToArray();
            }
        }

        private void MergeMaps()
        {
            // XXX - reconsider these parameters
            _merged.Name = _new.Name;
            _merged.Namespace = _new.Namespace;
            _merged.NamespaceUpper = _new.NamespaceUpper;
            _merged.Operations = _new.Operations;

            _merged.Modules = MergeModuleLists(_old.Modules, _new.Modules, "");
        }

        private MapAnsibleModule[] MergeModuleLists(MapAnsibleModule[] o, MapAnsibleModule[] n, string prefix)
        {
            List<MapAnsibleModule> m = new List<MapAnsibleModule>();

            foreach (var oldModule in o)
            {
                var newModule = Array.Find(n, e => (e.ModuleName == oldModule.ModuleName));
                if (newModule != null)
                {
                    m.Add(MergeModules(oldModule, newModule, prefix + "Modules[" + oldModule.ModuleName + "]"));
                }
                else
                {
                    //m.Add(oldModule);
                    _output.Add("# missing module '" + oldModule.ModuleName + "'");
                }
            }

            foreach (var newModule in n)
            {
                var oldModule = Array.Find(o, e => (e.ModuleName == newModule.ModuleName));
                if (oldModule == null)
                {
                    m.Add(newModule);
                    _output.Add("Module '" + newModule.ModuleName + "' in new template only - ADDING");
                    _output.Add("# new module '" + oldModule.ModuleName + "'");
                }
            }

            return m.ToArray();
        }

        private MapAnsibleModule MergeModules(MapAnsibleModule o, MapAnsibleModule n, string prefix)
        {
            MapAnsibleModule m = new MapAnsibleModule();

            m.ModuleName = n.ModuleName;        // Module name cannot be altered by changing it in the template
            m.ModuleNameAlt = o.ModuleNameAlt;  // it can be changed by changing ModuleNameAlt
            m.ModuleOperationName = o.ModuleOperationName;
            m.ModuleOperationNameUpper = o.ModuleOperationNameUpper;
            m.ObjectName = (o.ObjectName != null) ? o.ObjectName : n.ObjectName;
            m.AssertStateVariable = o.AssertStateVariable;
            m.TestPrerequisitesModule = (o.TestPrerequisitesModule != null) ? o.TestPrerequisitesModule : n.TestPrerequisitesModule;
            m.AssertStateExpectedValue = o.AssertStateExpectedValue;
            m.Options = MergeOptionLists(o.Options, n.Options, prefix + ":Options");
            m.ResponseFields = MergeResponseFieldLists(o.ResponseFields, n.ResponseFields);

            // methods are not modifiable, so will always come from the new version
            m.Methods = n.Methods;

            if (o.ModuleNameAlt != n.ModuleNameAlt)
                _output.Add(prefix + ":ModuleNameAlt > " + n.ModuleNameAlt);

            if (o.ModuleOperationName != n.ModuleOperationName)
                _output.Add(prefix + ":ModuleOperationName > " + o.ModuleOperationName);

            if (o.AssertStateVariable != n.AssertStateVariable)
                _output.Add(prefix + ":AssertStateVariable > " + o.AssertStateVariable);

            if (o.AssertStateExpectedValue != n.AssertStateExpectedValue)
                _output.Add(prefix + ":AssertStateExpectedValue > " + o.AssertStateExpectedValue);
            return m;
        }

        private ModuleOption[] MergeOptionLists(ModuleOption[] o, ModuleOption[] n, string prefix)
        {
            if ((o == null) && (n == null))
                return null;

            List<ModuleOption> m = new List<ModuleOption>();

            if (o != null)
            {
                foreach (var oldOption in o)
                {
                    var newOption = (n != null) ? Array.Find(n, e => (e.Name == oldOption.Name)) : null;
                    if (newOption != null)
                    {
                        m.Add(MergeOptions(oldOption, newOption, prefix + "[" + newOption.Name + "]"));
                    }
                    else
                    {
                        m.Add(oldOption);
                        //_output.Add("Option '" + oldOption.Name + "' in old template only - KEEPING");
                    }
                }
            }

            if (n != null)
            {
                foreach (var newOption in n)
                {
                    var oldOption = (o != null) ? Array.Find(o, e => (e.Name == newOption.Name)) : null;
                    if (oldOption == null)
                    {
                        m.Add(newOption);
                        //_output.Add("Option '" + newOption.Name + "' in new template only - ADDING");
                    }
                }
            }

            return m.ToArray();
        }

        private ModuleOption MergeOptions(ModuleOption o, ModuleOption n, string prefix)
        {
            ModuleOption m = new ModuleOption(n.Name, n.Type, o.Required, o.VariableValue);

            m.Disposition = o.Disposition;
            m.Documentation = o.Documentation;
            m.IsList = o.IsList;
            m.NameAlt = o.NameAlt;
            m.DefaultValueSample = o.DefaultValueSample;
            m.DefaultValueTest = o.DefaultValueTest;
            m.SubOptions = MergeOptionLists(o.SubOptions, n.SubOptions, prefix + ":SubOptions");

            if (n.Disposition != m.Disposition)
                _output.Add(prefix + " > " + m.Disposition);

            if (n.Documentation != m.Documentation)
                _output.Add(prefix + " > " + m.Documentation);

            if (n.NameAlt != m.NameAlt)
                _output.Add(prefix + " > " + m.NameAlt);

            if (n.Required != m.Required)
                _output.Add(prefix + " > " + m.Required);

            if (n.IsList != m.IsList)
                _output.Add(prefix + " > " + m.IsList);

            if (n.Type != m.Type)
                _output.Add(prefix + " > " + m.Type);

            if (n.VariableValue != m.VariableValue)
                _output.Add(prefix + " > " + m.VariableValue);

            if (n.DefaultValueSample != m.DefaultValueSample)
                _output.Add(prefix + " > " + m.DefaultValueSample);

            if (n.DefaultValueTest != m.DefaultValueTest)
                _output.Add(prefix + " > " + m.DefaultValueTest);

            return m;
        }

        private ModuleResponseField[] MergeResponseFieldLists(ModuleResponseField[] o, ModuleResponseField[] n)
        {
            if ((o == null) && (n == null))
                return null;

            List<ModuleResponseField> m = new List<ModuleResponseField>();

            if (o != null)
            {
                foreach (var oldOption in o)
                {
                    var newOption = (n != null) ? Array.Find(n, e => (e.Name == oldOption.Name)) : null;
                    if (newOption != null)
                    {
                        m.Add(MergeResponseFields(oldOption, newOption));
                    }
                    else
                    {
                        m.Add(oldOption);
                        _output.Add("Option '" + oldOption.Name + "' in old template only - KEEPING");
                    }
                }
            }

            if (n != null)
            {
                foreach (var newOption in n)
                {
                    var oldOption = (o != null) ? Array.Find(o, e => (e.Name == newOption.Name)) : null;
                    if (oldOption == null)
                    {
                        m.Add(newOption);
                        _output.Add("Option '" + newOption.Name + "' in new template only - ADDING");
                    }
                }
            }

            return m.ToArray();
        }

        private ModuleResponseField MergeResponseFields(ModuleResponseField o, ModuleResponseField n)
        {
            ModuleResponseField m = new ModuleResponseField(n.Name, n.Type, n.Description, n.SampleValue);

            m.Description = o.Description;
            m.NameAlt = o.NameAlt;
            m.SampleValue = o.SampleValue;
            m.SubFields = MergeResponseFieldLists(o.SubFields, n.SubFields);
            m.Returned = (o.Returned != null) ? o.Returned : n.Returned;

            if (o.Description != n.Description)
                _output.Add("Option '" + o.Name + "' Description - KEEPING OLD");

            if (o.NameAlt != n.NameAlt)
                _output.Add("Option '" + o.Name + "' NameAlt - KEEPING OLD");

            if (o.Type != n.Type)
                _output.Add("Option '" + o.Name + "' Type - KEEPING OLD");

            if (o.SampleValue != n.SampleValue)
                _output.Add("Option '" + o.Name + "' SampleValue - KEEPING OLD");

            if (o.Returned != n.Returned)
                _output.Add("Option '" + o.Name + "' Returned - KEEPING OLD");

            return m;
        }

        private MapAnsible _old;
        private MapAnsible _new;
        private MapAnsible _merged;

        private List<string> _output;

    }
}
