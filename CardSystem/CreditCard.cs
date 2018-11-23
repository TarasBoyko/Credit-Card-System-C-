using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardSystem
{
    // The class stores the number of a credit card and provides methods for
    // verification the number, getting next number and getting the vendor of the
    // credit card.
    // All methods of the class are thread unsafe.
    class CreditCard
    {
        // PUBLIC SECTION:
        public CreditCard(string number)
        {
            m_number = number;
        }

        // property of the "m_number" field
        public string Number
        {
            get
            {
                return m_number;
            }
            set
            {
                m_number = value;
            }
        }

        // Retunrs credit card vendor.
        // If succeeded to find out the vendor - returns the name of the vendor, otherwise - returns "Unknown".
        public CardVendor GetVendor()
        {
            RemoveSpacesFromNumber();

            string first2 = m_number.Substring(0, 2); // first two digit in a credit card number
            string first3 = m_number.Substring(0, 3); // first three digit in a credit card number
            string first4 = m_number.Substring(0, 4); // first four digit in a credit card number
            string first6 = m_number.Substring(0, 6); // first six digit in a credit card number

            if (first2 == "34" || first2 == "37")
            {
                return CardVendor.AmericanExpress;
            }
            else if (first2 == "50" ||
                      first3 == "639" ||
                      first2 == "67" ||
                      (("56".CompareTo(first2) <= 0) && (first2.CompareTo("58") <= 0))
                      )
            {
                return CardVendor.Maestro;
            }
            else if (("51".CompareTo(first2) <= 0 && first2.CompareTo("55") <= 0) || ("222100".CompareTo(first6) <= 0 && first6.CompareTo("272099") <= 0))
            {
                return CardVendor.MasterCard;
            }
            else if (m_number[0] == '4')
            {
                return CardVendor.Visa;
            }
            else if (("3528".CompareTo(first4) <= 0) && (first4.CompareTo("3589") <= 0))
            {
                return CardVendor.JCB;
            }
            else
            {
                return CardVendor.Unknown;
            }
        }

        // Returns credit card vendor as a string.
        public string GetVendorAsString()
        {
            CardVendor cardVendor = GetVendor();
            if (cardVendor == CardVendor.AmericanExpress)
            {
                return "American Express";
            }
            else
            {
                return cardVendor.ToString();
            }
        }

        // If the credit card number is valid - returns true, otherwise - return false
        // The verification is done using Luhn algorithm.
        public bool IsNumberValid()
        {
            RemoveSpacesFromNumber();
            return CalculateSumOfAllDigits(m_number, true) % 10 == 0;
        }

        // Returns next credit card number with the same issuer identification number.
        // If there is a next credit card number with the same issuer identification number -
        // returns next credit card number with the same issuer identification number,
        // otherwise - is OverflowException is thrown out.
        public string ReturnNextNumber()
        {
            RemoveSpacesFromNumber();
            string individualAccountIdentifier = m_number;

            // discard issuer identification number and check digit
            string issuerIdentificationNumber = individualAccountIdentifier.Substring(0, kIINdigits);
            individualAccountIdentifier = individualAccountIdentifier.Substring(kIINdigits);
            individualAccountIdentifier = individualAccountIdentifier.Substring(0, individualAccountIdentifier.Length - 1);

            // check individual account identifier for the highest number
            string limit = new string('9', individualAccountIdentifier.Length);
            if (individualAccountIdentifier == limit)
            {
                throw new OverflowException("individual account identifier already has the highest number");
            }

            // get next individual account identifier
            int x = int.Parse(individualAccountIdentifier);
            x++;
            individualAccountIdentifier = x.ToString();

            // get check digit
            uint checkDigit = (uint)(CalculateSumOfAllDigits(issuerIdentificationNumber + individualAccountIdentifier, false) * 9) % 10;

            return issuerIdentificationNumber + individualAccountIdentifier + checkDigit.ToString();
        }

        // PROTECTED SECTION:

        // Implements "calculate the sum of all the digits" step of Luhn algorithm.
        // The function retunrs the sum of all the digits by @individualAccountIdentifier.
        // @individualAccountIdentifier specifies individual account identifier with or without check digit.
        // @withCheckDigit specifies specifies if whether a check digit is appended to @individualAccountIdentifier.
        protected int CalculateSumOfAllDigits(string individualAccountIdentifier, bool withCheckDigit)
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
        protected void RemoveSpacesFromNumber()
        {
            m_number = m_number.Replace(" ", "");
            foreach (char symbol in m_number)
            {
                Debug.Assert(Char.IsDigit(symbol), "credit card comtains invalid symbols");
            }
        }

        protected string m_number; // number of a credit card
        protected const int kIINdigits = 6; // number of digits of issuer identification number
    } // CreditCard
} // CardSystem


