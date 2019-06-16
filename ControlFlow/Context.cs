using dnlib.DotNet;

namespace ControlFlow
{
    class Context
    {
        public AssemblyDef Assembly;
        public ModuleDef ManifestModule;
        public TypeDef GlobalType;
        public Importer Imp;
        public MethodDef cctor;

        public Context(AssemblyDef asm)
        {
            Assembly = asm;
            ManifestModule = asm.ManifestModule;
            GlobalType = ManifestModule.GlobalType;
            Imp = new Importer(ManifestModule);
            cctor = GlobalType.FindOrCreateStaticConstructor();
        }
    }
}

