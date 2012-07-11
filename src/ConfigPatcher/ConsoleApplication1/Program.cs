namespace ConfigPatcher
{
    class Program
    {
        static void Main(string[] args)
        {            
            var patcher = new Patcher(new ParseParamaters(args));
            patcher.Patch();
        }
    }
}
