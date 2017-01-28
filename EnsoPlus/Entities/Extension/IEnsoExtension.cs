using System;

namespace Extension
{
    public interface IEnsoExtension
    {
        void Load(IEnsoService enso);

        void OnCommand(EnsoCommand command, string postfix);

        void Unload();
    }
}
