using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MFIService.security
{
    class Security
    {
        private String enc;
        private String decr;

        public String Enc
        {
            get { return enc; }
            set { enc = value; }
        }

        public String Decr
        {
            get { return decr; }
            set { decr = value; }
        }

        public static String Encrypt(String DecryptedText)
        {
            String EncryptedText = "";
            ArrayList EncryptedList = new ArrayList();
            int index;
            int n;
            Random rand1 = new Random();
            Random rand2 = new Random();
            try
            {
                if (!"".Equals(DecryptedText))
                {

                    //1. move the decrpted text to an encrypted list in a reverse order
                    for (int i = (DecryptedText.Length - 1); i >= 0; i--)
                    {
                        EncryptedList.Add("" + DecryptedText[i]);
                    }

                    //2. Insert at 2nd(Index 1) character from the START with an integer
                    EncryptedList.Insert(1, rand1.Next(10).ToString());//randon numbers from 0-to-9

                    //3. Append any character between a-z
                    EncryptedList.Add(Security.getAlphabet(rand1.Next(27)));//randon numbers from 0-to-26

                    //4. Insert at 1st(Index 0) character from the START with a character between a-z
                    EncryptedList.Insert(0, Security.getAlphabet(rand1.Next(27)));//randon numbers from 0-to-26

                    //5. Append any integer between 0-9
                    EncryptedList.Add(rand1.Next(10).ToString());//randon numbers from 0-to-9

                    //6. Add random numbers on odd positions such as from ab to 1a2b
                    index = 0;
                    n = EncryptedList.Count;
                    while (n - index >= 1)
                    {
                        EncryptedList.Insert(index, rand1.Next(10).ToString());//randon numbers from 0-to-9
                        n = EncryptedList.Count;
                        index = index + 2;
                    }

                    //7. Add random characters on odd positions, get rand(1-25) and replace each random no by the equiv a-z
                    index = 0;
                    n = EncryptedList.Count;
                    int rd;
                    while (n - index >= 1)
                    {
                        rd = rand2.Next(27);//randon numbers from 0-to-26
                        EncryptedList.Insert(index, Security.getAlphabet(rd));
                        n = EncryptedList.Count;
                        index = index + 2;
                    }

                    //8. Finally, move the array list to text
                    for (int i = 0; i < EncryptedList.Count; i++)
                    {
                        EncryptedText = EncryptedText + EncryptedList[i].ToString();
                    }
                    return EncryptedText;

                }
                else
                {
                    return "";
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return "";
            }
        }

        public static String Decrypt(String EncryptedText)
        {
            String DecryptedText;
            ArrayList DecryptedList = new ArrayList();
            ArrayList TempDecryptedList = new ArrayList();
            int index;
            int n;
            try
            {
                if (!"".Equals(EncryptedText))
                {
                    //0. move the enc text to dec list
                    for (int i = 0; i < EncryptedText.Length; i++)
                    {
                        DecryptedList.Add("" + EncryptedText[i]);
                    }

                    //1(X7). Pick charcaters at even points, the ones at odd points are invalid
                    index = 1;
                    n = DecryptedList.Count;
                    while (index < n)
                    {
                        TempDecryptedList.Add(DecryptedList[index]);
                        index = index + 2;
                    }
                    DecryptedList.Clear();
                    DecryptedList.AddRange(TempDecryptedList);
                    TempDecryptedList.Clear();

                    //2(X6). Pick integers at even points, the ones at odd points are invalid
                    index = 1;
                    n = DecryptedList.Count;
                    while (index < n)
                    {
                        TempDecryptedList.Add(DecryptedList[index]);
                        index = index + 2;
                    }
                    DecryptedList.Clear();
                    DecryptedList.AddRange(TempDecryptedList);
                    TempDecryptedList.Clear();

                    //3 (X5). Remove last integer
                    n = DecryptedList.Count;
                    DecryptedList.RemoveAt(n - 1);

                    //4 (X4). Remove 1st(Index 0) character from the START
                    DecryptedList.RemoveAt(0);

                    //5 (X3). Remove last character
                    n = DecryptedList.Count;
                    DecryptedList.RemoveAt(n - 1);

                    //6 (X2). Remove 2nd(Index 1) Integer from the START
                    DecryptedList.RemoveAt(1);

                    //7 (X1). Reverse reverse order and move to Text
                    DecryptedText = "";
                    for (int i = (DecryptedList.Count - 1); i >= 0; i--)
                    {
                        TempDecryptedList.Add(DecryptedList[i]);
                        DecryptedText = DecryptedText + DecryptedList[i];
                    }

                    return DecryptedText;
                }
                else
                {
                    return "";
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return "";
            }
        }
        public static String getAlphabet(int i)
        {
            String alph;

            if (i == 0)
            {
                alph = "a";
            }
            else if (i == 1)
            {
                alph = "a";
            }
            else if (i == 2)
            {
                alph = "b";
            }
            else if (i == 3)
            {
                alph = "c";
            }
            else if (i == 4)
            {
                alph = "d";
            }
            else if (i == 5)
            {
                alph = "e";
            }
            else if (i == 6)
            {
                alph = "f";
            }
            else if (i == 7)
            {
                alph = "g";
            }
            else if (i == 8)
            {
                alph = "h";
            }
            else if (i == 9)
            {
                alph = "i";
            }
            else if (i == 10)
            {
                alph = "j";
            }
            else if (i == 11)
            {
                alph = "k";
            }
            else if (i == 12)
            {
                alph = "l";
            }
            else if (i == 13)
            {
                alph = "m";
            }
            else if (i == 14)
            {
                alph = "n";
            }
            else if (i == 15)
            {
                alph = "o";
            }
            else if (i == 16)
            {
                alph = "p";
            }
            else if (i == 17)
            {
                alph = "q";
            }
            else if (i == 18)
            {
                alph = "r";
            }
            else if (i == 19)
            {
                alph = "s";
            }
            else if (i == 20)
            {
                alph = "t";
            }
            else if (i == 21)
            {
                alph = "u";
            }
            else if (i == 22)
            {
                alph = "v";
            }
            else if (i == 23)
            {
                alph = "w";
            }
            else if (i == 24)
            {
                alph = "x";
            }
            else if (i == 25)
            {
                alph = "y";
            }
            else if (i == 26)
            {
                alph = "z";
            }
            else
            {
                alph = "z";
            }

            if (i % 2 == 0)
            {
                alph = alph.ToUpper();
            }
            else
            {
                alph = alph.ToLower();
            }

            return alph;
        }
    }
}
