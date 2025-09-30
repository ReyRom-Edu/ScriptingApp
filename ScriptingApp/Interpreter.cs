using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

namespace ScriptingApp
{
    public class Interpreter
    {
        public CancellationToken CancellationToken { get; } = new CancellationTokenSource().Token;
        public Interpreter()
        {
            _ = Task.Run(()=>RunServer(CancellationToken));
        }

        public async Task ExecuteAsync(string code)
        {
            var scriptOptions = ScriptOptions.Default
                .WithReferences(AppDomain.CurrentDomain.GetAssemblies())
                .WithImports("System", "System.Collections.Generic", "System.Linq");
            var globals = new Automation();
            var script = CSharpScript.Create(code, scriptOptions, globals.GetType());
            
            await script.RunAsync(globals);
        }

        public async Task RunServer(CancellationToken cancellationToken)
        {
            var server = new NamedPipeServerStream("mypipe", PipeDirection.In);
            server.WaitForConnection();
            using var reader = new StreamReader(server);
            while (true)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    break;
                }
                var data = await reader.ReadToEndAsync();
                if (data == null) continue;
                await ExecuteAsync(data);
            }
        }
    }
}
