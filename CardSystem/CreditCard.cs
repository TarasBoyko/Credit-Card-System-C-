using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardSystem
{


    class CreditCard
    {
        const int kIINdigits = 6; // number of digits of issuer identification number

        // Retunrs credit card vendor by @inputCardNumber.
        // If succeeded to find out the vendor - returns the name of the vendor, otherwise - returns "Unknown"
        // @inputCardNumber specifies credit card of the vendor.
        public string GetCreditCardVendor(string inputCardNumber)
        {
            string cardNumber = RemoveSpacesFromCreditCard(inputCardNumber);

            string first2 = cardNumber.Substring(0,2); // first two digit in a credit card
            string first3 = cardNumber.Substring(0, 3); // first three digit in a credit card
            string first4 = cardNumber.Substring(0, 4); // first four digit in a credit card
            string first6 = cardNumber.Substring(0, 6); // first six digit in a credit card

            if (first2 == "34" || first2 == "37")
            {
                return "American Express";
            }
            else if ( first2 == "50" ||
                      first3 == "639" ||
                      first2 == "67" ||
                      ( ("56".CompareTo(first2) <=0) && (first2.CompareTo("58") <= 0) )
                      )
            {
                return "Maestro";
            }
            else if ( ("51".CompareTo(first2) <= 0 && first2.CompareTo("55") <= 0) || ("222100".CompareTo(first6) <=0 && first6.CompareTo("272099")<=0) )
            {
                return "Mastercard";
            }
            else if (cardNumber[0] == '4')
            {
                return "Visa";
            }
            else if ( ("3528".CompareTo(first4) <= 0) && (first4.CompareTo("3589") <= 0) )
            {
                return "JCB";
            }
            else
            {
                return "Unknown";
            }
        }

        // If @inputCardNumber is valid - returns true, otherwise - return false
        // The verification is done using Luhn algorithm.
        // @inputCardNumber specifies credit card of the vendor.
        public bool IsCreditCardNumberValid(string inputCardNumber)
        {
            string cardNumber = RemoveSpacesFromCreditCard(inputCardNumber);
            return CalculateSumOfAllDigits(cardNumber, true) % 10 == 0;
        }

        // Generates next credit card number with the same issuer identification number.
        // If there is a next credit card number with the same issuer identification number -
        // returns next credit card number with the same issuer identification number,
        // otherwise - is OverflowException is thrown out.
        // @inputCardNumber specifies credit card.
        public string GenerateNextCreditCardNumber(string inputCardNumber)
        {
            string individualAccountIdentifier = RemoveSpacesFromCreditCard(inputCardNumber);

            // discard issuer identification number and check digit
            string issuerIdentificationNumber = individualAccountIdentifier.Substring(0, kIINdigits);
            individualAccountIdentifier = individualAccountIdentifier.Substring(kIINdigits);
            individualAccountIdentifier = individualAccountIdentifier.Substring(0, individualAccountIdentifier.Length - 1);


            // check individual account identifier for the highest number
            string limit = new string('9', individualAccountIdentifier.Length); // !!!!
            if (individualAccountIdentifier == limit)
            {
                throw new OverflowException("individual account identifier already has the highest number");
            }
            // get next individual account identifier
            int x = int.Parse(individualAccountIdentifier);
            //int x = atoi(individualAccountIdentifier.c_str());
            x++;
            individualAccountIdentifier = x.ToString();

            // get check digit
            uint checkDigit = (uint)(CalculateSumOfAllDigits(issuerIdentificationNumber + individualAccountIdentifier, false) * 9) % 10;

            return issuerIdentificationNumber + individualAccountIdentifier + checkDigit.ToString();
        }











        // Implements "calculate the sum of all the digits" step of Luhn algorithm.
        // The function retunrs the sum of all the digits by @individualAccountIdentifier.
        // @individualAccountIdentifier specifies individual account identifier with or without check digit.
        // @withCheckDigit specifies specifies if whether a check digit is appended to @individualAccountIdentifier.
        int CalculateSumOfAllDigits(string individualAccountIdentifier, bool withCheckDigit)
        {
            int sum = 0;
            uint nDigits = (uint)individualAccountIdentifier.Length;
            uint nParity = (nDigits - 1) % 2;
            if (withCheckDigit)
            {
                nParity = (nDigits - 1) % 2;
            }
            else
            {
                nParity = (nDigits) % 2;
            }
            char[] cDigit = new char[2] { '\0', '\0' };
            for (uint i = nDigits; i > 0; i--)
            {
                cDigit[0] = individualAccountIdentifier[(int)i - 1];
                int nDigit = int.Parse(new string(cDigit));

                if (nParity == i % 2)
                {
                    nDigit = nDigit * 2;
                }


                sum += nDigit / 10;
                sum += nDigit % 10;
            }
            return sum;
        }

        // Returns @inputCardNumber without spaces.
        // @inputCardNumber specifies credit card number.
        public string RemoveSpacesFromCreditCard(string inputCardNumber)
        {
            string cardNumber = inputCardNumber;
            cardNumber = cardNumber.Replace(" ", "");
            foreach (char symbol in cardNumber)
            {
                Debug.Assert(Char.IsDigit(symbol), "credit card comtains invalid symbols");
            }
            return cardNumber;
        }
    }













}





