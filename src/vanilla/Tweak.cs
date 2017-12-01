using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace AutoRest.Ansible
{
    public abstract class Tweak
    {
        public abstract void Apply(Model.MapAnsible map);

        public abstract void ApplyOnModule(Model.MapAnsibleModule m);
    }

    public abstract class Tweak_Module : Tweak
    {
        public override void Apply(Model.MapAnsible map)
        {
            var modules = new List<Model.MapAnsibleModule>();

            if (_module == "*")
            {
                modules.AddRange(map.Modules); ;
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
                ApplyOnModule(m);
            }
        }

        protected Model.MapAnsible _map;
        protected string _module;
    }

    public abstract class TweakOption : Tweak_Module
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

    public abstract class TweakResult : Tweak_Module
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

    class Tweak_RenameModule : Tweak_Module
    {
        public Tweak_RenameModule(string originalName, string newName)
        {
            _module = originalName;
            _newName = newName;
        }

        public override void ApplyOnModule(Model.MapAnsibleModule m)
        {
            m.ModuleNameAlt = _newName;
        }

        private string _newName;
    }

    class Tweak_ModuleAssertStateVariable : Tweak_Module
    {
        public Tweak_ModuleAssertStateVariable(string module, string newValue)
        {
            _module = module;
            _newValue = newValue;
        }

        public override void ApplyOnModule(Model.MapAnsibleModule m)
        {
            m.AssertStateVariable = _newValue;
        }

        private string _newValue;
    }

    class Tweak_ModuleAssertStateExpectedValue : Tweak_Module
    {
        public Tweak_ModuleAssertStateExpectedValue(string module, string newValue)
        {
            _module = module;
            _newValue = newValue;
        }

        public override void ApplyOnModule(Model.MapAnsibleModule m)
        {
            m.AssertStateExpectedValue = _newValue;
        }

        private string _newValue;
    }

    class Tweak_ModuleObjectName : Tweak_Module
    {
        public Tweak_ModuleObjectName(string module, string newValue)
        {
            _module = module;
            _newValue = newValue;
        }

        public override void ApplyOnModule(Model.MapAnsibleModule m)
        {
            m.ObjectName = _newValue;
        }

        private string _newValue;
    }

    class Tweak_Module_TestPrerequisitesModule : Tweak_Module
    {
        public Tweak_Module_TestPrerequisitesModule(string module, string value)
        {
            _module = module;
            _newValue = value;
        }

        public override void ApplyOnModule(Model.MapAnsibleModule m)
        {
            m.TestPrerequisitesModule = _newValue;
        }

        private string _newValue;
    }

    class Tweak_RenameOption : TweakOption
    {
        public Tweak_RenameOption(string module, string path, string newName, int levelChange = 0)
        {
            _module = module;
            _path = path.Split(".");
            _newName = newName;
            _levelChange = levelChange;
        }

        public override void ApplyOnModule(Model.MapAnsibleModule m)
        {
            Model.ModuleOption option = GetOption(m, _path);
            if (option != null) option.NameAlt = _newName;
            // XXX - level change
        }

        private string[] _path;
        private string _newName;
        private int _levelChange;
    }

    class Tweak_ChangeOptionRequired : TweakOption
    {
        public Tweak_ChangeOptionRequired(string module, string path, bool newValue)
        {
            _module = module;
            _path = path.Split(".");
            _newValue = newValue;
        }

        public override void ApplyOnModule(Model.MapAnsibleModule m)
        {
            Model.ModuleOption option = GetOption(m, _path);
            if (option != null) option.Required = _newValue ? "True" : "False";
            // XXX - level change
        }

        private string[] _path;
        private bool _newValue;
    }
    class Tweak_ChangeOptionDefaultValueTest : TweakOption
    {
        public Tweak_ChangeOptionDefaultValueTest(string module, string path, string newValue)
        {
            _module = module;
            _path = path.Split(".");
            _newValue = newValue;
        }

        public override void ApplyOnModule(Model.MapAnsibleModule m)
        {
            Model.ModuleOption option = GetOption(m, _path);
            if (option != null) option.DefaultValueTest = _newValue;
            // XXX - level change
        }

        private string[] _path;
        private string _newValue;
    }

    class Tweak_ChangeOptionDefaultValueSample : TweakOption
    {
        public Tweak_ChangeOptionDefaultValueSample(string module, string path, string newValue)
        {
            _module = module;
            _path = path.Split(".");
            _newValue = newValue;
        }

        public override void ApplyOnModule(Model.MapAnsibleModule m)
        {
            Model.ModuleOption option = GetOption(m, _path);
            if (option != null) option.DefaultValueSample = _newValue;
            // XXX - level change
        }

        private string[] _path;
        private string _newValue;
    }
    class Tweak_RenameResultField : TweakResult
    {
        public Tweak_RenameResultField(string module, string path, string newName, int levelChange = 0)
        {
            _module = module;
            _path = path.Split(".");
            _newName = newName;
            _levelChange = levelChange;
        }

        public override void ApplyOnModule(Model.MapAnsibleModule m)
        {
            Model.ModuleResponseField field = GetResultField(m, _map, _path);
            if (field != null) field.NameAlt = _newName;
            // XXX - level change
        }

        private string[] _path;
        private string _newName;
        private int _levelChange;
    }

    class Tweak_RemoveResultField : Tweak_RenameResultField
    {
        public Tweak_RemoveResultField(string module, string path) : base(module, path, "x", 0)
        {
        }
    }

    class Tweak_ResultField_UpdateSampleValue : TweakResult
    {
        public Tweak_ResultField_UpdateSampleValue(string module, string path, string newValue, int levelChange = 0)
        {
            _module = module;
            _path = path.Split(".");
            _newValue = newValue;
            _levelChange = levelChange;
        }

        public override void ApplyOnModule(Model.MapAnsibleModule m)
        {
            Model.ModuleResponseField field = GetResultField(m, _map, _path);
            if (field != null) field.SampleValue = _newValue;
            // XXX - level change
        }

        private string[] _path;
        private string _newValue;
        private int _levelChange;
    }
}

