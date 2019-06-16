using dnlib.DotNet;
using dnlib.DotNet.Emit;
using dnlib.DotNet.Writer;
using System;

namespace ControlFlow
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = ".NET ControlFlow";
            Console.WriteLine("Basic .NET ControlFlow based in dnlib");

            if (args[0] == "" || args[0] == null)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Invalid args.");
                Console.ReadLine();
            }
            else
            {
                string path = args[0];
                AssemblyDef assembly = AssemblyDef.Load(path);
                Context ctx = new Context(assembly);
                ControlFlow(ctx);
                var Options = new ModuleWriterOptions(ctx.ManifestModule);
                Options.Logger = DummyLogger.NoThrowInstance;
                string pathloc = path.Replace(".exe", "");
                assembly.Write(pathloc + "-WithControlFlow.exe", Options);
                Console.WriteLine("Saved in -> " + pathloc + " - WithControlFlow.exe");
                Console.ReadLine();
            }
        }
        public static void ControlFlow(Context ctx)
        {
            foreach (ModuleDef module in ctx.Assembly.Modules)
            {
                foreach (TypeDef typeDef in module.Types)
                {
                    foreach (MethodDef methodDef in typeDef.Methods)
                    {
                        ControlFlowPhase(methodDef);
                    }
                }
            }
        }

        public static void ControlFlowPhase(MethodDef method)
        {
            for (int i = 0; i < method.Body.Instructions.Count; i++)
            {
                if (method.Body.Instructions[i].IsLdcI4())
                {
                    int numorig = new Random(Guid.NewGuid().GetHashCode()).Next();
                    int div = new Random(Guid.NewGuid().GetHashCode()).Next();
                    int num = numorig ^ div;

                    Instruction nop = OpCodes.Nop.ToInstruction();

                    Local local = new Local(method.Module.ImportAsTypeSig(typeof(int)));
                    method.Body.Variables.Add(local);

                    method.Body.Instructions.Insert(i + 1, OpCodes.Stloc.ToInstruction(local));
                    method.Body.Instructions.Insert(i + 2, Instruction.Create(OpCodes.Ldc_I4, method.Body.Instructions[i].GetLdcI4Value() - sizeof(float)));
                    method.Body.Instructions.Insert(i + 3, Instruction.Create(OpCodes.Ldc_I4, num));
                    method.Body.Instructions.Insert(i + 4, Instruction.Create(OpCodes.Ldc_I4, div));
                    method.Body.Instructions.Insert(i + 5, Instruction.Create(OpCodes.Xor));
                    method.Body.Instructions.Insert(i + 6, Instruction.Create(OpCodes.Ldc_I4, numorig));
                    method.Body.Instructions.Insert(i + 7, Instruction.Create(OpCodes.Bne_Un, nop));
                    method.Body.Instructions.Insert(i + 8, Instruction.Create(OpCodes.Ldc_I4, 2));
                    method.Body.Instructions.Insert(i + 9, OpCodes.Stloc.ToInstruction(local));
                    method.Body.Instructions.Insert(i + 10, Instruction.Create(OpCodes.Sizeof, method.Module.Import(typeof(float))));
                    method.Body.Instructions.Insert(i + 11, Instruction.Create(OpCodes.Add));
                    method.Body.Instructions.Insert(i + 12, nop);
                    i += 12;
                }
            }
        }
    }
}
