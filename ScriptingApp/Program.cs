
namespace ScriptingApp
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            Interpreter interpreter = new Interpreter();

            Console.WriteLine("Scripting App started. Type your C# code to execute.");

            while (true)
            {
                Console.Write("> ");
                string code = Console.ReadLine();
                if (string.IsNullOrEmpty(code)) break;
                try
                {
                    await interpreter.ExecuteAsync(code);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }
        }
    }
}
