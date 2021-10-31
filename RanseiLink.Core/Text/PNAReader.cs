/*
 * PNAReader code by pleonex. Small modifications by Deijin
 * 
 * Copyright (C) 2012  pleonex
 *
 *   This program is free software: you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation, either version 3 of the License, or
 *   (at your option) any later version.
 *
 *   This program is distributed in the hope that it will be useful,
 *   but WITHOUT ANY WARRANTY; without even the implied warranty of
 *   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *   GNU General Public License for more details.
 *
 *   You should have received a copy of the GNU General Public License
 *   along with this program.  If not, see <http://www.gnu.org/licenses/>.
 * 
 *
 * Creado por SharpDevelop.
 * Fecha: 18/03/2012
 *
 */
using System;
using System.IO;
using System.Text;
using System.Collections.Generic;


namespace RanseiLink.Core.Text
{
    public class PNAReader
    {
        static PNAReader()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }

        private readonly bool nameMode = false;

        // Variable
        byte[] data;
        int pos;
        string text;
        byte currb;                         // Current byte

        int end;                            // If [R0,#0xA40] == 0 => Stop signal, store 0 and exit; Not always, check by the next offset
        bool sjis2;                         // If [R0,#0xA38] != 0 => First byte of SJS is 0x83, otherwise 0x82
        bool furigana;						// If it's writting a furigana
        bool sjis3;                         // Value [#0xA3C]

        Dictionary<ushort, char> char_table;
        bool useTable;

        // Constant
        static byte[] table = new byte[]	// Table used for SJIS chars
		{
            0x92, 0x40, 0x42, 0x44, 0x46, 0x48, 0x83, 0x85, 0x87, 0x62,
            0x00, 0x41, 0x43, 0x45, 0x47, 0x49, 0x4A, 0x4C, 0x4E, 0x50,
            0x52, 0x54, 0x56, 0x58, 0x5A, 0x5C, 0x5E, 0x60, 0x63, 0x65,
            0x67, 0x69, 0x6A, 0x6B, 0x6C, 0x6D, 0x6E, 0x71, 0x74, 0x77,
            0x7A, 0x7D, 0x7E, 0x80, 0x81, 0x82, 0x84, 0x86, 0x88, 0x89,
            0x8A, 0x8B, 0x8C, 0x8D, 0x8F, 0x93
        };

        void Error(string t) { Console.WriteLine("Error!\t" + t); Console.ReadKey(true); }

        public PNAReader(string fileIn, int i, bool useTable)
        {
            this.useTable = useTable;
            char_table = new Dictionary<ushort, char>();
            if (useTable)
                Read_Table();

            text = "";
            data = File.ReadAllBytes(fileIn);

            // Get offset from the table
            pos = (i + 1) * 4;
            pos = (int)BitConverter.ToUInt32(data, pos);

            end = (int)BitConverter.ToUInt32(data, 0);  // Num of strings
            if ((i + 1) != end)
                end = (int)BitConverter.ToUInt32(data, (i + 2) * 4);
            else
                end = data.Length;

            sjis2 = false;
            furigana = false;
            sjis3 = false;

            Read_Text();
        }
        public PNAReader(byte[] fileIn, int i, bool useTable)
        {
            this.useTable = useTable;
            char_table = new Dictionary<ushort, char>();
            if (useTable)
                Read_Table();

            text = "";
            data = fileIn;

            // Get offset from the table
            pos = (i + 1) * 4;
            pos = (int)BitConverter.ToUInt32(data, pos);

            end = (int)BitConverter.ToUInt32(data, 0);  // Num of strings
            if ((i + 1) != end)
                end = (int)BitConverter.ToUInt32(data, (i + 2) * 4);
            else
                end = data.Length;

            sjis2 = false;
            furigana = false;
            sjis3 = false;

            Read_Text();
        }
        public PNAReader(byte[] fileIn, bool useTable, bool nameMode = false)
        {
            this.nameMode = nameMode;
            this.useTable = useTable;
            char_table = new Dictionary<ushort, char>();
            if (useTable)
                Read_Table();

            text = "";
            data = fileIn;

            pos = 0;
            sjis2 = false;
            end = fileIn.Length;
            furigana = false;
            sjis3 = false;

            Read_Text();
        }
        void Read_Table()
        {
            throw new Exception("not supporting table atm in pna reader");
            string table_file = "";//System.Windows.Forms.Application.StartupPath + Path.DirectorySeparatorChar + "table.tbl";
            if (!File.Exists(table_file))
            {
                useTable = false;
                return;
            }

            string[] lines = File.ReadAllLines(table_file);
            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].Length != 6)
                    continue;

                ushort v = Convert.ToUInt16(lines[i].Substring(0, 4), 16);
                char_table.Add(v, lines[i][5]);
            }
        }

        void Read_Text()
        {
            while (pos < end)
            {
                currb = data[pos++];

                if (currb < 0x20)
                    Get_ControlCode();
                else if (currb < 0xA6 || currb > 0xDF)
                    Get_ASCII();
                else
                    Get_SJIS();
            }

            #region Deijin Change. Remove this for name parsing
            if (!nameMode)
            {
                text = text.Remove(text.Length - 1);
            }
            #endregion
        }

        void Get_ControlCode()
        {
            sjis2 = false;
            if (currb > 0xA)    // Only 0x1B
            {
                if (currb != 0x1B)
                    Error("0x1B");
                Get_VariableParam();
                return;
            }

            switch (currb)
            {
                case 0x01:
                    Get_C1();
                    return;

                case 0x02:
                    Get_C2();
                    return;

                case 0x05:
                    Get_C5();
                    return;

                case 0x08:      // Block 22
                case 0x09:
                    text += "|" + currb.ToString("x").PadLeft(2, '0');
                    return;

                case 0x0A:
                    text += (char)currb;
                    return;
            }
        }

        void Get_C1()
        {
            byte c1 = data[pos++];

            if (c1 < 0x43)
            {
                if (c1 != 0x22)
                    Error("0x22 - C1");

                // Always 0x22
                text += "{C1}";
                return;
            }

            // Uknown
            if (c1 == 0x53)
            {
                byte b1 = data[pos++];	// Always 0x25
                if (b1 != 0x25)
                    Error("0x25 - C1");

                text += "{Unk1 P:";
                text += Get_VariableParamASCII();
                pos += 3;
                text += " P:" + Get_VariableParamASCII() + '}';
            }
        }
        void Get_C2()
        {
            text += "|";
            int text_pos = text.Length;
            pos--;
            Get_Variable();

            if (text[text_pos] == ',')
                text = text.Remove(text_pos, 1);
            else
                text = text.Remove(text_pos - 1, 1);
        }
        void Get_C5()
        {
            pos--;
            byte id = Get_ID5();

            if (id == 0x24)
            {
                text += "{End}\n";
            }
            else if (id == 0x69)
            {
                text += "{Unk2 P:";
                pos += 3;
                text += Get_VariableParamASCII() + '}';
            }
            else
                Error("C5");
        }
        byte Get_ID5()
        {
            byte id = data[pos++];
            if (id != 0x05)
            {
                Error("0x05 - ID5");
                pos--;
                return 0xFF;
            }

            byte b1 = data[pos++];
            if (b1 != 0x05)
            {
                Error("2 0x05 - ID5");
                pos -= 2;
                return 0xFF;
            }

            byte b2 = data[pos++];
            if (b2 > 0x9)
            {
                Error("0x09 - ID5");
                pos -= 3;
                return 0xFF;
            }

            switch (b2)
            {
                case 0x04:
                    return 0x69;
                case 0x05:
                    return 0x24;
            }

            Error("0xFF - ID5");
            return 0xFF;
        }

        void Get_Variable()
        {
            byte b1 = data[pos++];

            // If it's not control code 2
            if (b1 != 0x02)
            {
                text += ',' + b1.ToString("x").PadLeft(2, '0');
                Console.ReadKey(true);
                return;
            }

            byte b2 = data[pos++];
            byte cm = b2;
            byte id = Get_VarID(ref b2);

            switch (id)
            {
                case 0x00:
                    byte c0 = data[pos++];
                    if (c0 < 0x20)
                    {
                        pos--;
                        text += "{Turns}";
                    }
                    else if (c0 == 0x64)
                        text += ",02," + cm.ToString("x").PadLeft(2, '0') + ",64";
                    else if (c0 == 0x65)
                        text += ",02," + cm.ToString("x").PadLeft(2, '0') + ",65";
                    else if (c0 == 0x66)
                        text += ",02," + cm.ToString("x").PadLeft(2, '0') + ",66";
                    else
                        Error("Case 0 - Var");
                    break;

                case 0x01:
                    byte c1 = data[pos++];
                    if (c1 < 0x20)
                    {
                        pos--;
                        text += "{Attack}";
                    }
                    else if (c1 == 0x64)
                        text += ",02," + cm.ToString("x").PadLeft(2, '0') + ",64";
                    else
                        Error("Case 1 - Var");
                    break;

                case 0x02:
                    byte c2 = data[pos++];
                    if (c2 < 0x20)
                    {
                        Error("Case 2 < 0x20 - Var");
                        pos--;
                    }
                    if (c2 == 0x64 && cm == 0x46)
                        text += "{Poke}";
                    else if (c2 == 0x64)
                        text += ",02," + cm.ToString("x").PadLeft(2, '0') + ",64";
                    else if (c2 == 0x32)
                        text += ",02," + cm.ToString("x").PadLeft(2, '0') + ",32";
                    else if (c2 == 0x33)
                        text += ",02," + cm.ToString("x").PadLeft(2, '0') + ",33";
                    else if (c2 == 0x34)
                        text += ",02," + cm.ToString("x").PadLeft(2, '0') + ",34";
                    else if (c2 == 0x65)
                        text += ",02," + cm.ToString("x").PadLeft(2, '0') + ",65";
                    else if (c2 == 0xC8)
                        text += ",02," + cm.ToString("x").PadLeft(2, '0') + ",C8";
                    else
                        Error("Case 2 - Var");
                    break;

                case 0x03:
                    byte c3 = data[pos++];
                    if (c3 < 0x20)
                    {
                        Error("Case 3 < 0x20 - Var");
                        pos--;
                    }
                    if (c3 == 0x64 && cm == 0x50)
                        text += "{Name2}";	// Player name too ¿?
                    else if (c3 == 0x64)
                        text += ",02," + cm.ToString("x").PadLeft(2, '0') + ",64";
                    else if (c3 == 0x65)
                        text += ",02," + cm.ToString("x").PadLeft(2, '0') + ",65";
                    else if (c3 == 0x32)
                        text += ",02," + cm.ToString("x").PadLeft(2, '0') + ",32";
                    else if (c3 == 0x33)
                        text += ",02," + cm.ToString("x").PadLeft(2, '0') + ",33";
                    else if (c3 == 0x34)
                        text += ",02," + cm.ToString("x").PadLeft(2, '0') + ",34";
                    else
                        Error("Case 3 - Var");
                    break;

                case 0x04:
                    byte c4 = data[pos++];
                    if (c4 < 0x20)
                    {
                        Error("Case 4 < 0x20 - Var");
                        pos--;
                    }
                    if (c4 == 0x64 && cm == 0x6E)
                        text += "{Area}";
                    else if (c4 == 0x64)
                        text += ",02," + cm.ToString("x").PadLeft(2, '0') + ",64";
                    else if (c4 == 0x32)
                        text += ",02," + cm.ToString("x").PadLeft(2, '0') + ",32";
                    else if (c4 == 0x33)
                        text += ",02," + cm.ToString("x").PadLeft(2, '0') + ",33";
                    else if (c4 == 0x34)
                        text += ",02," + cm.ToString("x").PadLeft(2, '0') + ",34";
                    else
                        Error("Case 4 - Var");
                    break;

                case 0x05:
                    byte c5 = data[pos++];
                    if (c5 < 0x20)
                    {
                        Error("Case 5 < 0x20 - Var");
                        pos--;
                    }
                    else if (c5 == 0x32)
                        text += ",02," + cm.ToString("x").PadLeft(2, '0') + ",32";
                    else if (c5 == 0x33)
                        text += ",02," + cm.ToString("x").PadLeft(2, '0') + ",33";
                    else if (c5 == 0x34)
                        text += ",02," + cm.ToString("x").PadLeft(2, '0') + ",34";
                    else if (c5 == 0x11)
                        text += ",02," + cm.ToString("x").PadLeft(2, '0') + ",11";
                    else if (c5 == 0x64)
                        text += "{Name3}";
                    else if (c5 == 0x65)
                        text += ",02," + cm.ToString("x").PadLeft(2, '0') + ",65";
                    else
                        Error("Case 5 - Var");
                    break;

                case 0x06:
                    byte c6 = data[pos++];
                    if (c6 < 0x20)
                    {
                        Error("Case 6 < 0x20 - Var");
                        pos--;
                    }
                    else if (c6 == 0x32)
                        text += ",02," + cm.ToString("x").PadLeft(2, '0') + ",32";
                    else if (c6 == 0x33)
                        text += ",02," + cm.ToString("x").PadLeft(2, '0') + ",33";
                    else if (c6 == 0x34)
                        text += ",02," + cm.ToString("x").PadLeft(2, '0') + ",34";
                    else if (c6 == 0x86)
                        text += ",02," + cm.ToString("x").PadLeft(2, '0') + ",86";
                    else if (c6 == 0x64)
                        text += ",02," + cm.ToString("x").PadLeft(2, '0') + ",64";
                    else
                        Error("Case 6 - Var");
                    break;

                case 0x07:
                    byte c7 = data[pos++];
                    if (c7 < 0x20)
                    {
                        Error("Case 7 < 0x20 - Var");
                        pos--;
                    }
                    else if (c7 == 0x32)
                        text += ",02," + cm.ToString("x").PadLeft(2, '0') + ",32";
                    else if (c7 == 0x33)
                        text += ",02," + cm.ToString("x").PadLeft(2, '0') + ",33";
                    else if (c7 == 0x34)
                        text += ",02," + cm.ToString("x").PadLeft(2, '0') + ",34";
                    else if (c7 == 0x94)
                        text += ",02," + cm.ToString("x").PadLeft(2, '0') + ",94";
                    else if (c7 == 0x64)
                        text += ",02," + cm.ToString("x").PadLeft(2, '0') + ",64";
                    else
                        Error("Case 7 - Var");
                    break;

                case 0x08:
                    if (b2 == 3)
                        text += "{Name}";
                    else if (b2 == 4)
                        text += ",02," + cm.ToString("x").PadLeft(2, '0');
                    else
                    {
                        text += ",02," + cm.ToString("x").PadLeft(2, '0');
                        Get_Variable();
                    }
                    break;
            }

        }
        byte Get_VarID(ref byte b)
        {
            if (b < 0x32)
                return 0xFF;
            else if (b >= 0x32 && b < 0x3B)
            {
                b -= 0x32;
                return 0;
            }
            else if (b < 0x3C)
                return 0xFF;
            else if (b < 0x45)
            {
                b -= 0x3C;
                return 1;
            }
            else if (b < 0x46)
                return 0xFF;
            else if (b < 0x4F)
            {
                b -= 0x46;
                return 2;
            }
            else if (b < 0x50)
                return 0xFF;
            else if (b < 0x59)
            {
                b -= 0x50;
                return 3;
            }
            else if (b < 0x5A)
                return 0xFF;
            else if (b < 0x63)
            {
                b -= 0x5A;
                return 5;
            }
            else if (b < 0x64)
                return 0xFF;
            else if (b < 0x67)
            {
                b -= 0x64;
                return 6;
            }
            else if (b < 0x6E)
                return 0xFF;
            else if (b < 0x77)
            {
                b -= 0x6E;
                return 4;
            }
            else if (b < 0x78)
                return 0xFF;
            else if (b < 0x81)
            {
                b -= 0x78;
                return 7;
            }
            else if (b < 0xC8)
                return 0xFF;
            else if (b < 0xD2)
            {
                b -= 0xC8;
                return 8;
            }

            return 0xFF;
        }
        string Get_VariableParamASCII()
        {
            string p = "";
            for (int i = 0; i < 0x0A; i++)
            {
                byte b = data[pos++];
                if (b < 0x30 || b > 0x39)
                {
                    pos--;
                    break;
                }
                p += (char)b;
            }
            return p;
        }

        void Get_VariableParam()
        {
            byte b1 = data[pos++];

            if (b1 == 0x48)
            {
                sjis2 = false;
                sjis3 = false;
            }
            else if (b1 == 0x4B)
            {
                sjis2 = true;
            }
            else if (b1 == 0x6B)
            {
                sjis3 = true;
                sjis2 = true;
            }
            else if (b1 == 0x72)
            {
                furigana = !furigana;
                if (furigana) text += '[';
                else text += ']';
            }
            else if (b1 == 0x77)
                text += "{Wait:" + Get_Byte() + '}';		// Wait some seconds to continue showing text
            else if (b1 == 0x73)
                text += "{NameColor:" + Get_Byte() + '}';	// Color of the name of the current speaking char
            else if (b1 == 0x66)
                text += "{CharImage:" + Get_Byte() + '}';	// Animation / ncer bank / image from the current speaking char
            else if (b1 == 0x63)
                text += "{Color:" + Get_Byte() + '}';
            else if (b1 == 0x56)
                text += "|1b,56";
            else if (b1 == 0x40)							// Current speaking char image
                text += "{Char:" + (char)data[pos++] + (char)data[pos++] + (char)data[pos++] + (char)data[pos++] + '}';
            else if (b1 == 0x43)
                text += "|1b,43," + Get_Byte();
            else
                Error("Variable Param - " + b1.ToString("X"));
        }

        string Get_Byte()
        {
            return data[pos++].ToString("x").PadLeft(2, '0');
        }

        void Get_SJIS()
        {
            if (sjis3)
            {
                text += Decode_Table(currb);
                return;
            }

            byte s1 = data[pos++];
            if (s1 != 0xDE && s1 != 0xDF)
                pos--;

            text += Get_Char(s1);
        }
        char Get_Char(byte s1)
        {
            byte s2 = 0x82;
            if (sjis2)
                s2 = 0x83;

            int index = currb - 0xA6;
            byte value = table[index];

            if (s1 == 0xDE)
            {
                if (currb == 0xB3) value = 0x94;
                else value++;
            }
            else if (s1 == 0xDF) value += 2;

            if (sjis2)
            {
                ushort v1 = 0;
                if (useTable)
                    v1 = BitConverter.ToUInt16(new byte[] { value, s2 }, 0);
                else
                    v1 = BitConverter.ToUInt16(new byte[] { s2, value }, 0);
                return Decode_Table(v1);
            }

            byte sub = 0;
            if (value >= 0x80)
                sub = 1;

            value += 0x5F;
            value -= sub;

            ushort v2 = 0;
            if (useTable)
                v2 = BitConverter.ToUInt16(new byte[] { value, s2 }, 0);
            else
                v2 = BitConverter.ToUInt16(new byte[] { s2, value }, 0);
            return Decode_Table(v2);
        }
        void Get_ASCII()
        {
            if (currb < 0x81 || currb > 0x9F)
            {
                if (currb < 0xE0 || currb > 0xFC)
                {
                    // ASCII char
                    text += Decode_Table(currb);
                    return;
                }
            }

            #region Deijin's Addition

            if (currb == 0x84)
            {
                switch (data[pos])
                {
                    case 0x81:
                        text += 'ū';
                        pos++;
                        return;
                    case 0x90:
                        text += 'ō';
                        pos++;
                        return;
                    default:
                        text += "?";
                        pos++;
                        return;
                }
            }

            #endregion

            // It's a kanji
            ushort v = 0;
            if (useTable)
                v = BitConverter.ToUInt16(new byte[] { data[pos++], currb }, 0);
            else
                v = BitConverter.ToUInt16(new byte[] { currb, data[pos++] }, 0);
            text += Decode_Table(v);
        }

        char Decode_Table(ushort v)
        {
            if (char_table.ContainsKey(v))
                return char_table[v];
            else
            {
                if (!useTable)
                    return Encoding.GetEncoding(932).GetChars(BitConverter.GetBytes(v))[0];

                Error("Decoding ushort: " + v.ToString("X"));
                return '?';
            }
        }

        public string Text
        {
            get { return text; }
        }
    }
}
