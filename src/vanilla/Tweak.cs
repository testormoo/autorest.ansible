using System;
using System.Collections.Generic;
using System.Text;

namespace AutoRest.Ansible
{
    public abstract class Tweak
    {
        public abstract void Apply(Model.MapAnsible map);
    }

    class Tweak_RenameModule : Tweak
    {
        public Tweak_RenameModule(string originalName, string newName)
        {
            _oldName = originalName;
            _newName = newName;
        }

        public override void Apply(Model.MapAnsible map)
        {
            foreach (var m in map.Modules)
            {
                if (m.ModuleName == _oldName)
                    m.ModuleNameAlt = _newName;
            }
        }

        private string _oldName;
        private string _newName;
    }
}
