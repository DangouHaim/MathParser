using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MathParser
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Test("input_1.txt", "output_1.txt");
                Test("input_2.txt", "output_3.txt");
                Test("input_3.txt", "output_3.txt");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            Console.ReadLine();
        }

        static void Test(string _in, string _out)
        {
            using (FileStream f = new FileStream(_in, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                using (StreamReader sr = new StreamReader(f))
                {
                    using (FileStream ff = new FileStream(_out, FileMode.Create, FileAccess.ReadWrite))
                    {
                        using (StreamWriter sw = new StreamWriter(ff))
                        {
                            while (!sr.EndOfStream)
                            {
                                try
                                {
                                    sw.WriteLine(new Calculator(sr.ReadLine()).Calculate());
                                }
                                catch (Exception ex)
                                {
                                    sw.WriteLine(ex.Message);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
