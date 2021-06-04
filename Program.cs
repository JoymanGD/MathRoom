using System;

namespace MathRoom
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            MathRoom.Instance.Run();
        }
    }
}
