using System;
using System.Runtime.CompilerServices;

namespace StringInterpolation
{
    public static class ConditionalLog
    {
        public static void Log(
            bool condition,
            [InterpolatedStringHandlerArgument("condition")]ref ConditionalInterpolatedStringHandler message,
            [CallerArgumentExpression("condition")]string paramName = "")
        {
            if (condition)
            {
                Console.WriteLine(message.ToString() + " " + paramName);
            }
        }
    }
}