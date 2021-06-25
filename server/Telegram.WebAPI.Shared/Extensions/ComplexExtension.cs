using System;
using System.Collections.Generic;
using System.Text;

namespace Telegram.WebAPI.Shared.Extensions
{
    public static class ComplexExtension
    {

        public static void LogExceptionToConsole(this Exception e)
        {
            Console.WriteLine($"{e.GetType().Name}: {e.Message}");
        }

    }
}
