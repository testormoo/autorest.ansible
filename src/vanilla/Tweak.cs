using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace AutoRest.Ansible
{
    public abstract class Tweak
    {
        public abstract void Apply(Model.MapAnsible map);
    }

    public abstract class TweakModule : Tweak
    {
        protected Model.MapAnsibleModule GetModule(Model.MapAnsible map, string name)
        {
            foreach (var m in map.Modules)
            {
                if (m.ModuleName == name)
                    return m;
            }

            return null;
        }
    }

    public abstract class TweakOption : TweakModule
    {
        protected Model.ModuleOption GetOption(Model.MapAnsible map, string module, string[] path)
        {
            Model.MapAnsibleModule m = GetModule(map, module);
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

    public abstract class TweakResult : TweakModule
    {
        protected Model.ModuleResponseField GetResultField(Model.MapAnsible map, string module, string[] path)
        {
            Model.MapAnsibleModule m = GetModule(map, module);
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

    class Tweak_RenameModule : TweakModule
    {
        public Tweak_RenameModule(string originalName, string newName)
        {
            _module = originalName;
            _newName = newName;
        }

        public override void Apply(Model.MapAnsible map)
        {
            Model.MapAnsibleModule m = GetModule(map, _module);
            if (m != null) m.ModuleNameAlt = _newName;
        }

        protected string _module;
        private string _newName;
    }

    class Tweak_ModuleAssertStateVariable : TweakModule
    {
        public Tweak_ModuleAssertStateVariable(string module, string newValue)
        {
            _module = module;
            _newValue = newValue;
        }

        public override void Apply(Model.MapAnsible map)
        {
            Model.MapAnsibleModule m = GetModule(map, _module);
            if (m != null) m.AssertStateVariable = _newValue;
        }

        protected string _module;
        private string _newValue;
    }

    class Tweak_ModuleAssertStateExpectedValue : TweakModule
    {
        public Tweak_ModuleAssertStateExpectedValue(string module, string newValue)
        {
            _module = module;
            _newValue = newValue;
        }

        public override void Apply(Model.MapAnsible map)
        {
            Model.MapAnsibleModule m = GetModule(map, _module);
            if (m != null) m.AssertStateExpectedValue = _newValue;
        }

        protected string _module;
        private string _newValue;
    }

    class Tweak_ModuleObjectName : TweakModule
    {
        public Tweak_ModuleObjectName(string module, string newValue)
        {
            _module = module;
            _newValue = newValue;
        }

        public override void Apply(Model.MapAnsible map)
        {
            Model.MapAnsibleModule m = GetModule(map, _module);
            if (m != null) m.ObjectName = _newValue;
        }

        protected string _module;
        private string _newValue;
    }

    class Tweak_Module_TestPrerequisitesModule : TweakModule
    {
        public Tweak_Module_TestPrerequisitesModule(string module, string value)
        {
            _module = module;
            _newValue = value;
        }

        public override void Apply(Model.MapAnsible map)
        {
            Model.MapAnsibleModule m = GetModule(map, _module);
            if (m != null) m.TestPrerequisitesModule = _newValue;
        }

        protected string _module;
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

        public override void Apply(Model.MapAnsible map)
        {
            Model.ModuleOption option = GetOption(map, _module, _path);
            if (option != null) option.NameAlt = _newName;
            // XXX - level change
        }

        private string _module;
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

        public override void Apply(Model.MapAnsible map)
        {
            Model.ModuleOption option = GetOption(map, _module, _path);
            if (option != null) option.Required = _newValue ? "True" : "False";
            // XXX - level change
        }

        private string _module;
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

        public override void Apply(Model.MapAnsible map)
        {
            Model.ModuleOption option = GetOption(map, _module, _path);
            if (option != null) option.DefaultValueTest = _newValue;
            // XXX - level change
        }

        private string _module;
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

        public override void Apply(Model.MapAnsible map)
        {
            Model.ModuleOption option = GetOption(map, _module, _path);
            if (option != null) option.DefaultValueSample = _newValue;
            // XXX - level change
        }

        private string _module;
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

        public override void Apply(Model.MapAnsible map)
        {
            Model.ModuleResponseField field = GetResultField(map, _module, _path);
            if (field != null) field.NameAlt = _newName;
            // XXX - level change
        }

        private string _module;
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

        public override void Apply(Model.MapAnsible map)
        {
            Model.ModuleResponseField field = GetResultField(map, _module, _path);
            if (field != null) field.SampleValue = _newValue;
            // XXX - level change
        }

        private string _module;
        private string[] _path;
        private string _newValue;
        private int _levelChange;
    }
}

