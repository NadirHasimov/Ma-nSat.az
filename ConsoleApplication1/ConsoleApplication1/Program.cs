using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            int count = 0;
            for(int i = 0; i <= 299; i ++)
            {
                if(i%4==0)
                {

                    count++;
                    Console.WriteLine(i);

                }
            }
            Console.WriteLine(count);
            Console.ReadKey();
        }
    }
}
