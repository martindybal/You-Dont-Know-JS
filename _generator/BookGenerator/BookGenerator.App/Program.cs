using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Net;
using System.Net.Mime;
using System.Threading.Tasks;

namespace BookGenerator.App
{
    class Program
    {
        static void Main(string[] args)
        {
            var bookGenerator = new BookGenerator();
            bookGenerator.GenerateAllBook();
        }
    }
}

