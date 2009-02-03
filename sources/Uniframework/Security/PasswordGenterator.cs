using System;
using System.Security.Cryptography;
using System.Text;

namespace Uniframework.Security
{
    /// <summary>
    /// 随机密码生成器
    /// </summary>
    public sealed class PasswordGenerator
    {
        public PasswordGenerator()
        {
            Minimum = DefaultMinimum;
            Maximum = DefaultMaximum;
            ConsecutiveCharacters = false;
            RepeatCharacters = true;
            ExcludeSymbols = false;
            Exclusions = null;

            rng = new RNGCryptoServiceProvider();
        }


        /// <summary>
        /// Gets the cryptographic random number.
        /// </summary>
        /// <param name="lBound">The l bound.</param>
        /// <param name="uBound">The u bound.</param>
        /// <returns></returns>
        protected int GetCryptographicRandomNumber(int lBound, int uBound)
        {
            // Assumes lBound >= 0 && lBound < uBound
            // returns an int >= lBound and < uBound
            uint urndnum;
            byte[] rndnum = new Byte[4];
            if (lBound == uBound - 1)
            {
                // test for degenerate case where only lBound can be returned   
                return lBound;
            }

            uint xcludeRndBase = (uint.MaxValue - (uint.MaxValue % (uint)(uBound - lBound)));

            do
            {
                rng.GetBytes(rndnum);
                urndnum = BitConverter.ToUInt32(rndnum, 0);
            } while (urndnum >= xcludeRndBase);

            return (int)(urndnum % (uBound - lBound)) + lBound;
        }

        /// <summary>
        /// Gets the random character.
        /// </summary>
        /// <returns></returns>
        protected char GetRandomCharacter()
        {
            int upperBound = pwdCharArray.GetUpperBound(0);

            if (true == ExcludeSymbols)
            {
                upperBound = UBoundDigit;
            }

            int randomCharPosition = GetCryptographicRandomNumber(pwdCharArray.GetLowerBound(0), upperBound);

            char randomChar = pwdCharArray[randomCharPosition];

            return randomChar;
        }

        /// <summary>
        /// Generates this instance.
        /// </summary>
        /// <returns></returns>
        public string Generate()
        {
            // Pick random length between minimum and maximum   
            int pwdLength = GetCryptographicRandomNumber(Minimum, Maximum);

            StringBuilder pwdBuffer = new StringBuilder();
            pwdBuffer.Capacity = Maximum;

            // Generate random characters
            char lastCharacter, nextCharacter;

            // Initial dummy character flag
            lastCharacter = nextCharacter = '\n';

            for (int i = 0; i < pwdLength; i++)
            {
                nextCharacter = GetRandomCharacter();

                if (false == ConsecutiveCharacters)
                {
                    while (lastCharacter == nextCharacter)
                    {
                        nextCharacter = GetRandomCharacter();
                    }
                }

                if (false == RepeatCharacters)
                {
                    string temp = pwdBuffer.ToString();
                    int duplicateIndex = temp.IndexOf(nextCharacter);
                    while (-1 != duplicateIndex)
                    {
                        nextCharacter = GetRandomCharacter();
                        duplicateIndex = temp.IndexOf(nextCharacter);
                    }
                }

                if ((null != Exclusions))
                {
                    while (-1 != Exclusions.IndexOf(nextCharacter))
                    {
                        nextCharacter = GetRandomCharacter();
                    }
                }

                pwdBuffer.Append(nextCharacter);
                lastCharacter = nextCharacter;
            }

            if (null != pwdBuffer)
            {
                return pwdBuffer.ToString();
            }
            else
            {
                return String.Empty;
            }
        }

        /// <summary>
        /// Gets or sets the exclusions.
        /// </summary>
        /// <value>The exclusions.</value>
        public string Exclusions
        {
            get { return exclusionSet; }
            set { exclusionSet = value; }
        }

        /// <summary>
        /// Gets or sets the minimum.
        /// </summary>
        /// <value>The minimum.</value>
        public int Minimum
        {
            get { return minSize; }
            set
            {
                minSize = value;
                if (DefaultMinimum > minSize)
                {
                    minSize = DefaultMinimum;
                }
            }
        }

        /// <summary>
        /// Gets or sets the maximum.
        /// </summary>
        /// <value>The maximum.</value>
        public int Maximum
        {
            get { return maxSize; }
            set
            {
                maxSize = value;
                if (minSize >= maxSize)
                {
                    maxSize = DefaultMaximum;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [exclude symbols].
        /// </summary>
        /// <value><c>true</c> if [exclude symbols]; otherwise, <c>false</c>.</value>
        public bool ExcludeSymbols
        {
            get { return hasSymbols; }
            set { hasSymbols = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [repeat characters].
        /// </summary>
        /// <value><c>true</c> if [repeat characters]; otherwise, <c>false</c>.</value>
        public bool RepeatCharacters
        {
            get { return hasRepeating; }
            set { hasRepeating = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [consecutive characters].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [consecutive characters]; otherwise, <c>false</c>.
        /// </value>
        public bool ConsecutiveCharacters
        {
            get { return hasConsecutive; }
            set { hasConsecutive = value; }
        }

        private const int DefaultMinimum = 6;
        private const int DefaultMaximum = 10;
        private const int UBoundDigit = 61;

        private RNGCryptoServiceProvider rng;
        private int minSize;
        private int maxSize;
        private bool hasRepeating;
        private bool hasConsecutive;
        private bool hasSymbols;
        private string exclusionSet;

        private char[] pwdCharArray =
            "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789`~!@#$%^&*()-_=+[]{}\\|;:'\",<.>/?".
                ToCharArray();
    }
}