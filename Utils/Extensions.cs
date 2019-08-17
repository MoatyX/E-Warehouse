using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Warehouse.Utils
{
    public static class Extensions
    {
        /// <summary>
        /// removes whitespaces from an input string
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string RemoveWhitespace(this string input)
        {
            return new string(input.ToCharArray().Where(c => !char.IsWhiteSpace(c)).ToArray());
        }

        public static void PrintColoredLine(ConsoleColor color, string value)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(value);
            Console.ResetColor();
        }
    }
}
