using System.Threading;

namespace CreditCard.UITest
{
    internal static class DemoHelper
    {
        public static void Pause(int secondToPaus = 3000)
        {
            Thread.Sleep(secondToPaus);
        }
    }
}
