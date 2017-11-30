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
            return (m != null ) ? (from option in m.Options where option.Name == path[0] select option).First() : null;
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

}
