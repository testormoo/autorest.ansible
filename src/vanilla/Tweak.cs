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
}
