using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardSystem
{
    public enum CardVendor
    {
        AmericanExpress,
        Maestro,
        MasterCard,
        Visa,
        JCB,
        Unknown
    }

    class Program
    {
        static void Main(string[] args)
        {
            CreditCard creditCard = new CreditCard("378282246310005");

            string[] arr = new string[]
            {
                "378282246310005",
                "371449635398431",
                "378734493671000",
                "5610591081018250",
                "30569309025904",
                "38520000023237",
                "6011111111111117",
                "6011000990139424",
                "3530111333300000",
                "3566002020360505",
                "5555555555554444",
                "5105105105105100",
                "4111111111111111",
                "4012888888881881",
                "4222222222222",
            };

            string[] hughNumbers = new string[]
           {
                "89999006",
                "89999001",
                "89999000",
                "899990003",
                "8999009",
                "8999907",
                "899990004",
                "899990007",
                "899990006",
                "899990003",
                "899990008",
                "899990007",
                "899990004",
                "899990002",
                "890002",
           };


            foreach (string str in arr)
            {
                creditCard.Number = str;
                Console.WriteLine("Validation " + creditCard.IsNumberValid() + " vendor " + creditCard.GetVendorAsString());

                foreach (char c in new char[]{ '0', '1', '2', '3', '4', '5', '6', '7', '8', '9'} )
                {
                    if ( c != str[str.Length-1] )
                    {
                        StringBuilder st = new StringBuilder(str);

                        st[str.Length-1] = c;

                        creditCard.Number = st.ToString();
                        Console.WriteLine(creditCard.IsNumberValid() + " " + st.ToString());
                    }
                }

            }

            Console.ReadKey();

            Console.WriteLine("\ngeneration\n");

            for (int i = 0;  i < arr.Length; i++ )
            {
                string card = arr[i].Substring(0, 6) + hughNumbers[i];
                creditCard.Number = arr[i].Substring(0, 6) + hughNumbers[i];
                Console.WriteLine(card);
                try
                {
                    while (creditCard.IsNumberValid())
                    {
                        Console.WriteLine(card + " " + creditCard.GetVendorAsString());
                        card = creditCard.ReturnNextNumber();
                        creditCard.Number = card;
                    }
                }
                catch (OverflowException e)
                {
                    Console.WriteLine("overflow: " + e.Message);
                }
                catch (Exception e)
                {
                    Console.WriteLine("exeption: " + e.Message);
                }
                finally
                {
                    Console.WriteLine("finally");
                }

                Console.ReadKey();

            }

            Console.ReadKey();
        }
    }
}
