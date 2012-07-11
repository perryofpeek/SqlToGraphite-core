using System;

namespace ConfigPatcher
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var patcher = new Patcher(new ParseParamaters(args));
                patcher.Patch();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
                Environment.Exit(1);
            }
        }
    }
}
