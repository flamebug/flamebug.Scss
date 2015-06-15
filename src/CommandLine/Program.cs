using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;

using flamebug.Scss;

namespace CommandLine
{
    public class Program
    {
        public void Main(string[] args)
        {
            Console.WriteLine("Parsing File");
            Console.WriteLine("*********************************");

            var outputfile = Directory.GetCurrentDirectory() + "\\foo.css";
            var inputfile = Directory.GetCurrentDirectory() + "\\foo.scss";

            try {
                File.WriteAllText(outputfile, Parser.Parse(inputfile));
            }
            catch(Exception ex)
            {
                Console.Write(ex.Message);
            }
        }
    }
}
