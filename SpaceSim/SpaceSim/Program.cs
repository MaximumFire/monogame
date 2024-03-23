using System;

namespace SpaceSim
{
    internal class Program
    {
        static void Main(string[] args)
        {
            using (Main g = new Main())
            {
                g.Run();
            }
        }
    }
}
