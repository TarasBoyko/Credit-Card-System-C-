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
            Number = number;

            m_cardFirstNumberValidatonMap = new Dictionary<CardVendor, Predicate<string>>
            {
                [CardVendor.AmericanExpress] = (creditCard) => (m_first2 == "34" || m_first2 == "37"),
                [CardVendor.Maestro] = (creditCard) => (m_first2 == "50" ||
                                                        m_first3 == "639" ||
                                                        m_first2 == "67" ||
                                                        (("56".CompareTo(m_first2) <= 0) && (m_first2.CompareTo("58") <= 0))
                                                        ),
                [CardVendor.MasterCard] = (creditCard) => (("51".CompareTo(m_first2) <= 0 && m_first2.CompareTo("55") <= 0) || ("222100".CompareTo(m_first6) <= 0 && m_first6.CompareTo("272099") <= 0)),
                [CardVendor.Visa] = (creditCard) => (m_number[0] == '4'),
                [CardVendor.JCB] = (creditCard) => (("3528".CompareTo(m_first4) <= 0) && (m_first4.CompareTo("3589") <= 0)),
                [CardVendor.Unknown] = (creditCard) => (true)
            };
            foreach (CardVendor cardVendor in (CardVendor[])Enum.GetValues(typeof(CardVendor)))
            {
                Debug.Assert( m_cardFirstNumberValidatonMap.ContainsKey(cardVendor), "\"m_cardFirstNumberValidatonMap\" does not cover all card vendors");
            }

            // specifies special card number lengths
            m_cardNumberRangeValidatonMap = new Dictionary<CardVendor, Predicate<int>>
            {
                [CardVendor.JCB] = (numberOfDigits) => (numberOfDigits == 16)
            };

            // specifies common card number length
            foreach (CardVendor cardVendor in (CardVendor[])Enum.GetValues(typeof(CardVendor)))
            {
                if ( !m_cardNumberRangeValidatonMap.ContainsKey(cardVendor) )
                {
                    m_cardNumberRangeValidatonMap.Add(cardVendor, (numberOfDigits) => (kMinCardNumberLength <= numberOfDigits && numberOfDigits <= kMaxCardNumberLength));
                }
            }
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
                m_first2 = m_number.Substring(0, 2);
                m_first3 = m_number.Substring(0, 3);
                m_first4 = m_number.Substring(0, 4);
                m_first6 = m_number.Substring(0, 6);
            }
        }

        // Retunrs credit card vendor.
        // If succeeded to find out the vendor - returns the name of the vendor, otherwise - returns "Unknown".
        public CardVendor GetVendor()
        {
            RemoveSpacesFromNumber();

            foreach (CardVendor cardVendor in (CardVendor[])Enum.GetValues(typeof(CardVendor)))
            {
                if ( m_cardFirstNumberValidatonMap[cardVendor](m_number) && m_cardNumberRangeValidatonMap[cardVendor](m_number.Length) )
                {
                    return cardVendor;
                }
            }

            return CardVendor.Unknown;
        }

        // Returns credit card vendor as a string.
        public string GetVendorAsString()
        {
            CardVendor cardVendor = GetVendor();
            switch (cardVendor)
            {
                case CardVendor.AmericanExpress:
                    return "American Express";
                default:
                    return cardVendor.ToString();
            }
        }

        // If the credit card number is valid - returns true, otherwise - return false
        // The verification is done using length of the card, the vendor of the card and Luhn algorithm.
        public bool IsNumberValid()
        {
            RemoveSpacesFromNumber();

            foreach (CardVendor cardVendor in (CardVendor[])Enum.GetValues(typeof(CardVendor)))
            {
                if ( m_cardFirstNumberValidatonMap[cardVendor](m_number) )
                {
                    if (!m_cardNumberRangeValidatonMap[cardVendor](m_number.Length))
                    {
                        return false;
                    } 
                }
            }

            return CalculateSumOfAllDigits(m_number, true) % 10 == 0;
        }

        // Returns next credit card number.
        // If there is a next credit card number,
        // returns next credit card number,
        // otherwise - is OverflowException is thrown out.
        // "m_number" have to be valid.
        public string ReturnNextNumber()
        {
            RemoveSpacesFromNumber();
            CreditCard generatingCardNumber = new CreditCard(Number);

            do
            {
                if ( generatingCardNumber.Number.Length > kMaxCardNumberLength )
                {
                    throw new OverflowException("the card number already has the highest number");
                }
                string cardNumberWithoutCheckdigit = generatingCardNumber.Number.Substring(0, generatingCardNumber.Number.Length - 1);

                ulong x = ulong.Parse(cardNumberWithoutCheckdigit);
                x++;
                cardNumberWithoutCheckdigit = x.ToString();

                uint checkDigit = (uint)(CalculateSumOfAllDigits(cardNumberWithoutCheckdigit, false) * 9) % 10;

                generatingCardNumber.Number = cardNumberWithoutCheckdigit + checkDigit;
            } while (!generatingCardNumber.IsNumberValid());

            return generatingCardNumber.Number;
        }

        // PROTECTED SECTION:

        // Implements "calculate the sum of all the digits" step of Luhn algorithm.
        // The function retunrs the sum of all the digits by @cardNumber.
        // @cardNumber specifies card number with or without check digit.
        // @withCheckDigit specifies specifies if whether a check digit is appended to @cardNumber.
        protected int CalculateSumOfAllDigits(string cardNumber, bool withCheckDigit)
        {
            int sum = 0;
            uint nDigits = (uint)cardNumber.Length;
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
                cDigit[0] = cardNumber[(int)i - 1];
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

        Dictionary<CardVendor, Predicate<string>> m_cardFirstNumberValidatonMap; // map, that implements the relation (vendor) => (first credit card digits)
        Dictionary<CardVendor, Predicate<int>> m_cardNumberRangeValidatonMap; // map, that implements the relation (vendor) => (range of number of card number digits)

        protected string m_first2; // first two digits in a credit card number
        protected string m_first3; // first three digits in a credit card number
        protected string m_first4; // first four digits in a credit card number
        protected string m_first6; // first six digits in a credit card number

        protected string m_number; // number of a credit card
        protected const int kIINdigits = 6; // number of digits of issuer identification number

        protected const int kMinCardNumberLength = 13; // minimum credit card number length
        protected const int kMaxCardNumberLength = 19; // maximum credit card number length
    } // CreditCard
} // CardSystem


