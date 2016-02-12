using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LaserControl.ScriptV2
{
    public class Scanner
    {

        public static List<Token> GenerateTokenList(string input)
        {
            List<Token> tokens = new List<Token>();
            input = Scanner.UpperCaseScan(input)+';';
            //input = input.Replace('\n', ';');
            int pos = 0;
            int length = input.Length;

            int line = 0;

            while (pos < length)
            {
                switch (input[pos])
                {
                    #region Sonderzeichen
                    case '+':
                        tokens.Add(new Token(TT.TT_Plus,line));
                        break;
                    case '-':
                        tokens.Add(new Token(TT.TT_Minus, line));
                        break;
                    case '*':
                        tokens.Add(new Token(TT.TT_Multi, line));
                        break;
                    case '/':
                        ++pos;
                        if (input[pos] == '/')
                        {
                            while (pos < length && input[pos] != '\n')
                            {
                                ++pos;
                            }
                            ++line;
                        }
                        else
                        {
                            tokens.Add(new Token(TT.TT_Divide, line));
                            continue;
                        }
                        break;
                    case '%':
                        tokens.Add(new Token(TT.TT_Percent, line));
                        break;
                    case '^':
                        tokens.Add(new Token(TT.TT_Power, line));
                        break;
                    case '$':
                        Token td = new Token(TT.TT_Dollar, line);
                        tokens.Add(td);
                        break;
                    case ',':
                        tokens.Add(new Token(TT.TT_Comma, line));
                        break;
                    case ';':
                        Token t = new Token(TT.TT_Semicolon, line);
                        tokens.Add(t);
                        break;    
                    case '\n':
                        Token tn = new Token(TT.TT_Semicolon, line);
                        tokens.Add(tn);
                        ++line;
                        break;
                    case '"':
                        ++pos;
                        int s1 = pos;
                        while (pos < length && input[pos] != '"')
                            ++pos;
                        Token tstring = new TokenWithContent<string>(TT.TT_String, line, input.Substring(s1, pos - s1));
                        tokens.Add(tstring);
                        break;
                    case '=':
                        if (input[pos + 1] == '=')
                        {
                            tokens.Add(new Token(TT.TT_EqualTest, line));
                            ++pos;
                        }
                        else
                        {
                            Token tequal = new Token(TT.TT_EqualSign, line);
                            tokens.Add(tequal);
                        }
                        break;
                    case '{':
                        tokens.Add(new Token(TT.TT_BlockBegin, line));
                        break;
                    case '}':
                        tokens.Add(new Token(TT.TT_BlockEnd, line));
                        break;
                    case '(':
                        tokens.Add(new Token(TT.TT_ParenLeft, line));
                        break;
                    case ')':
                        tokens.Add(new Token(TT.TT_ParenRight, line));
                        break;
                    case '<':
                        if (input[pos + 1] == '=')
                        {
                            tokens.Add(new Token(TT.TT_SignLessOrEqual, line));
                            ++pos;
                        }
                        else
                        {
                            tokens.Add(new Token(TT.TT_SignLess, line));
                        }
                        break;
                    case '>':
                        tokens.Add(new Token(TT.TT_SignGreater, line));
                        break;
                    case '!':
                        if (input[pos + 1] == '=')
                        {
                            tokens.Add(new Token(TT.TT_UnEqualSign, line));
                            ++pos;
                        }
                        else
                        {
                            tokens.Add(new Token(TT.TT_ExclaMark, line));
                        }
                        break;
                    #endregion //Sonderzeichen

                    #region Zahlen
                    case '0':
                    case '1':
                    case '2':
                    case '3':
                    case '4':
                    case '5':
                    case '6':
                    case '7':
                    case '8':
                    case '9':
                        int startN = pos;
                        if (input[pos] == '0' && input[pos + 1] == 'X')
                        {
                            char[] vals = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F' };
                            pos += 2;
                            startN = pos;
                            while (vals.Contains(input[pos]))
                                ++pos;
                            int number1 = int.Parse(input.Substring(startN, pos - startN), System.Globalization.NumberStyles.HexNumber);
                            Token n1 = new TokenWithContent<int>(TT.TT_IntNumber, line, number1);
                            tokens.Add(n1);
                            continue;
                        }
                        else
                        {
                            while (Char.IsNumber(input[pos]))
                                ++pos;

                            if (input[pos] == '.')
                            {
                                ++pos;
                                while (Char.IsNumber(input[pos]))
                                    ++pos;
                                double number3 = double.Parse(input.Substring(startN, pos - startN), System.Globalization.NumberFormatInfo.InvariantInfo);
                                Token n3 = new TokenWithContent<double>(TT.TT_DoubleNumber, line, number3);
                                tokens.Add(n3);
                                continue;
                            }
                            else
                            {
                                int number2 = int.Parse(input.Substring(startN, pos - startN));
                                Token n2 = new TokenWithContent<int>(TT.TT_IntNumber, line, number2);
                                tokens.Add(n2);
                                continue;
                            }                            
                        }
                        //break;
                    #endregion

                    #region Buchstaben
                    case 'A':
                    case 'B':
                    case 'C':
                    case 'D':
                    case 'E':
                    case 'F':
                    case 'G':
                    case 'H':
                    case 'I':
                    case 'J':
                    case 'K':
                    case 'L':
                    case 'M':
                    case 'N':
                    case 'O':
                    case 'P':
                    case 'Q':
                    case 'R':
                    case 'S':
                    case 'T':
                    case 'U':
                    case 'V':
                    case 'W':
                    case 'X':
                    case 'Y':
                    case 'Z':
                        int start = pos;
                        while (Char.IsLetterOrDigit(input[pos]) || input[pos] == '-')
                            ++pos;
                        string name = input.Substring(start, pos - start);
                        if (input[pos] == '(')
                        {
                            ++pos;
                            if(name == "CALLC"){                                                            
                                tokens.Add(new Token(TT.TT_IdentCALLC, line));
                                continue;
                            }
                            if (name == "CALLO")
                            {
                                tokens.Add(new Token(TT.TT_IdentCALLO, line));
                                continue;
                            }
                            if (name == "FOR")
                            {
                                tokens.Add(new Token(TT.TT_IdentFOR, line));
                                continue;
                            }
                            if (name == "IF")
                            {
                                tokens.Add(new Token(TT.TT_IdentIF, line));
                                continue;
                            }
                            if (name == "ISSET")
                            {
                                tokens.Add(new Token(TT.TT_IdentISSET, line));
                                continue;
                            }
                            if (name == "INT" || name == "DOUBLE")
                            {
                                tokens.Add(new TokenWithContent<string>(TT.TT_IdentCast, line, name));
                                continue;
                            }
                            if (name == "WHILE")
                            {
                                tokens.Add(new Token(TT.TT_IdentWHILE, line));
                                continue;
                            }
                            if (name == "RETURN")
                            {
                                tokens.Add(new Token(TT.TT_IdentRETURN, line));
                                tokens.Add(new Token(TT.TT_ParenLeft, line));
                                continue;
                            }
                            tokens.Add(new TokenWithContent<string>(TT.TT_IdentInline, line, name));
                            continue;
                        }
                        if (name == "TRUE")
                        {
                            tokens.Add(new TokenWithContent<bool>(TT.TT_IdentTRUEFALSE, line, true));
                            continue;
                        }
                        if (name == "FALSE")
                        {
                            tokens.Add(new TokenWithContent<bool>(TT.TT_IdentTRUEFALSE, line, false));
                            continue;
                        }
                        if (name == "ELSE")
                        {
                            tokens.Add(new Token(TT.TT_IdentElse, line));
                            continue;
                        }
                        if (name == "PAUSE")
                        {
                            tokens.Add(new Token(TT.TT_IdentPAUSE, line));
                            continue;
                        }
                        if (name == "ID")
                        {
                            tokens.Add(new Token(TT.TT_IdentID, line));
                            continue;
                        }
                        if (name == "RETURN")
                        {
                            tokens.Add(new Token(TT.TT_IdentRETURN, line));
                            continue;
                        }
                        Token tident = new TokenWithContent<string>(TT.TT_Ident, line, name);
                        tokens.Add(tident);
                        
                        continue;
                    #endregion //Buchstaben
                }

                ++pos;
            }

                    

            return tokens;
        }

        protected static string UpperCaseScan(string input)
        {
            bool isInString = false;
            StringBuilder sb = new StringBuilder(input.Length);

            foreach (char c in input)
            {
                if (c == '"')
                    isInString = !isInString;

                if (!isInString)
                    sb.Append(Char.ToUpper(c));
                else
                    sb.Append(c);
            }

            input = sb.ToString();
            sb.Clear();
            return input;
        }
    }
}
