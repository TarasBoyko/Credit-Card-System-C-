using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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
            for ( int i = 0; i < 6; i++)
            Console.WriteLine(Enum.GetValues(typeof(CardVendor)).GetValue(i));
            Console.ReadLine();
            CreditCard creditCard = new CreditCard("9999999999999999907");
            Console.WriteLine(creditCard.IsNumberValid() + " " + creditCard.GetVendorAsString() + " " + creditCard.ReturnNextNumber());
            
            try
            {
                for (int i = 0; i < 10; i++)
                {
                    creditCard.Number = creditCard.ReturnNextNumber();
                    Console.WriteLine(creditCard.IsNumberValid() + " " + creditCard.GetVendorAsString() + " " + creditCard.Number);
                }
            }
            catch(OverflowException e)
            {
                Console.WriteLine("Exception!!! " + e.Message);

            }

            
            Console.ReadLine();


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
                //"3530111333300000",
               // "3566002020360505",
                "5555555555554444",
                "5105105105105100",
                "4111111111111111",
                "4012888888881881",
               // "4222222222222",
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
               // "899990006",
              //  "899990003",
                "899990008",
                "899990007",
                "899990004",
                "899990002",
              //  "890002",
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
            bool isAllOK = true;

            for (int i = 0;  i < arr.Length; i++ )
            {
                string card = arr[i].Substring(0, 6) + hughNumbers[i];
                creditCard.Number = arr[i].Substring(0, 6) + hughNumbers[i];
                Console.WriteLine(card);
                try
                {
                    for ( int j = 0; j < 100000; j++)
                    {
                        Console.WriteLine((isAllOK &= creditCard.IsNumberValid()) + " " + card + " " + creditCard.GetVendorAsString());
                        if ( !isAllOK )
                        {
                            Console.WriteLine("test is not passed");
                            Console.ReadLine();
                        }
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

                //Console.ReadKey();

            }

            creditCard.Number = "3530 1113 3330 0000 1";

            isAllOK &= creditCard.IsNumberValid();
            Console.WriteLine("All test are passed " + isAllOK);

            Console.ReadLine();
        } // Main
    }
}
