using System;
using System.Collections.Generic;
using System.Text;

namespace AutoRest.Python.Model
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

            _merged.Modules = MergeModuleLists(_old.Modules, _new.Modules);
        }

        private MapAnsibleModule[] MergeModuleLists(MapAnsibleModule[] o, MapAnsibleModule[] n)
        {
            List<MapAnsibleModule> m = new List<MapAnsibleModule>();

            foreach (var oldModule in o)
            {
                var newModule = Array.Find(n, e => (e.ModuleName == oldModule.ModuleName));
                if (newModule != null)
                {
                    m.Add(MergeModules(oldModule, newModule));
                }
                else
                {
                    m.Add(oldModule);
                    _output.Add("Module '" + oldModule.ModuleName + "' in old template only - KEEPING");
                }
            }

            foreach (var newModule in n)
            {
                var oldModule = Array.Find(o, e => (e.ModuleName == newModule.ModuleName));
                if (oldModule == null)
                {
                    m.Add(oldModule);
                    _output.Add("Module '" + newModule.ModuleName + "' in new template only - ADDING");
                }
            }

            return m.ToArray();
        }

        private MapAnsibleModule MergeModules(MapAnsibleModule o, MapAnsibleModule n)
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
            m.Options = MergeOptionLists(o.Options, n.Options);

            // methods are not modifiable, so will always come from the new version
            m.Methods = n.Methods;

            if (o.ModuleNameAlt != n.ModuleNameAlt)
                _output.Add("Option '" + o.ModuleName + "' ModuleNameAlt - KEEPING OLD");

            if (o.ModuleOperationName != n.ModuleOperationName)
                _output.Add("Option '" + o.ModuleName + "' ModuleOperationName - KEEPING OLD");

            if (o.AssertStateVariable != n.AssertStateVariable)
                _output.Add("Option '" + o.ModuleName + "' AssertStateVariable - KEEPING OLD");

            if (o.AssertStateExpectedValue != n.AssertStateExpectedValue)
                _output.Add("Option '" + o.ModuleName + "' AssertStateExpectedValue - KEEPING OLD");
            return m;
        }

        private ModuleOption[] MergeOptionLists(ModuleOption[] o, ModuleOption[] n)
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
                        m.Add(MergeOptions(oldOption, newOption));
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

        private ModuleOption MergeOptions(ModuleOption o, ModuleOption n)
        {
            ModuleOption m = new ModuleOption(n.Name, n.Type, o.Required, o.VariableValue);

            m.Disposition = o.Disposition;
            m.Documentation = o.Documentation;
            m.IsList = o.IsList;
            m.NameAlt = o.NameAlt;
            m.DefaultValueSample = o.DefaultValueSample;
            m.DefaultValueTest = o.DefaultValueTest;
            m.SubOptions = MergeOptionLists(o.SubOptions, n.SubOptions);

            if (o.Disposition != n.Disposition)
                _output.Add("Option '" + o.Name + "' Disposition - KEEPING OLD");

            if (o.Documentation != n.Documentation)
                _output.Add("Option '" + o.Name + "' Documentation - KEEPING OLD");

            if (o.NameAlt != n.NameAlt)
                _output.Add("Option '" + o.Name + "' NameAlt - KEEPING OLD");

            if (o.Required != n.Required)
                _output.Add("Option '" + o.Name + "' Required - KEEPING OLD");

            if (o.IsList != n.IsList)
                _output.Add("Option '" + o.Name + "' IsList - KEEPING OLD");

            if (o.Type != n.Type)
                _output.Add("Option '" + o.Name + "' Type - KEEPING OLD");

            if (o.VariableValue != n.VariableValue)
                _output.Add("Option '" + o.Name + "' VariableValue - KEEPING OLD");

            if (o.DefaultValueSample != n.DefaultValueSample)
                _output.Add("Option '" + o.Name + "' DefaultValueSample - KEEPING OLD");

            if (o.DefaultValueTest != n.DefaultValueTest)
                _output.Add("Option '" + o.Name + "' DefaultValueTest - KEEPING OLD");

            return m;
        }

        private MapAnsible _old;
        private MapAnsible _new;
        private MapAnsible _merged;

        private List<string> _output;
    }
}
