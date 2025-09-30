using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using System.IO;
using System.IO.Pipes;

namespace WpfApp1
{
    public class Interpreter
    {
        public CancellationToken CancellationToken { get; } = new CancellationTokenSource().Token;

        ScriptOptions scriptOptions;
        public Interpreter()
        {
            scriptOptions = ScriptOptions.Default
                .WithReferences(AppDomain.CurrentDomain.GetAssemblies())
                .WithImports("System", "System.Collections.Generic", "System.Linq");

            _ = Task.Run(() => RunServer(CancellationToken));
        }

        public async Task ExecuteAsync(string code)
        {
            await CSharpScript.EvaluateAsync(code, scriptOptions, new Automation());
        }


        public async Task RunServer(CancellationToken cancellationToken)
        {
            while (true)
            {
                using (NamedPipeServerStream server = new NamedPipeServerStream("mypipe", PipeDirection.In))
                {
                    server.WaitForConnection();

                    await HandleClient(server);
                }
            }
        }

        async Task HandleClient(NamedPipeServerStream server)
        {
            using var reader = new StreamReader(server);
            var data = await reader.ReadToEndAsync();

            await ExecuteAsync(data);
        }
    }
}
