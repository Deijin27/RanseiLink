/*
 * PNAWriter code by pleonex. Small modifications by Deijin
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
 * Fecha: 19/03/2012
 *
 */
using System;
using System.Text;
using System.IO;
using System.Collections.Generic;

namespace Core.Text
{
    public class PNAWriter
    {
        private readonly bool nameMode = false;
        List<byte> data;
        string text;
        int pos;

        byte[] currb;
        char currc;

        bool sjis3;
        bool sjis_sign;
        bool new_line;
        Dictionary<byte, byte> table;

        Dictionary<char, ushort> char_table;
        bool useTable;

        public PNAWriter(string text, bool useTable, bool nameMode = false)
        {
            this.nameMode = nameMode;
            Create_Table();

            this.useTable = useTable;
            char_table = new Dictionary<char, ushort>();
            if (useTable)
                Read_Table();

            this.text = text;
            sjis_sign = false;
            new_line = false;
            sjis3 = false;

            Set_Text();
        }
        private void Create_Table()
        {
            table = new Dictionary<byte, byte>();
            table.Add(0x92, 0x00);
            table.Add(0x40, 0x01);
            table.Add(0x42, 0x02);
            table.Add(0x44, 0x03);
            table.Add(0x46, 0x04);
            table.Add(0x48, 0x05);
            table.Add(0x83, 0x06);
            table.Add(0x85, 0x07);
            table.Add(0x87, 0x08);
            table.Add(0x62, 0x09);
            table.Add(0x00, 0x0A);
            table.Add(0x41, 0x0B);
            table.Add(0x43, 0x0C);
            table.Add(0x45, 0x0D);
            table.Add(0x47, 0x0E);
            table.Add(0x49, 0x0F);
            table.Add(0x4A, 0x10);
            table.Add(0x4C, 0x11);
            table.Add(0x4E, 0x12);
            table.Add(0x50, 0x13);
            table.Add(0x52, 0x14);
            table.Add(0x54, 0x15);
            table.Add(0x56, 0x16);
            table.Add(0x58, 0x17);
            table.Add(0x5A, 0x18);
            table.Add(0x5C, 0x19);
            table.Add(0x5E, 0x1A);
            table.Add(0x60, 0x1B);
            table.Add(0x63, 0x1C);
            table.Add(0x65, 0x1D);
            table.Add(0x67, 0x1E);
            table.Add(0x69, 0x1F);
            table.Add(0x6A, 0x20);
            table.Add(0x6B, 0x21);
            table.Add(0x6C, 0x22);
            table.Add(0x6D, 0x23);
            table.Add(0x6E, 0x24);
            table.Add(0x71, 0x25);
            table.Add(0x74, 0x26);
            table.Add(0x77, 0x27);
            table.Add(0x7A, 0x28);
            table.Add(0x7D, 0x29);
            table.Add(0x7E, 0x2A);
            table.Add(0x80, 0x2B);
            table.Add(0x81, 0x2C);
            table.Add(0x82, 0x2D);
            table.Add(0x84, 0x2E);
            table.Add(0x86, 0x2F);
            table.Add(0x88, 0x30);
            table.Add(0x89, 0x31);
            table.Add(0x8A, 0x32);
            table.Add(0x8B, 0x33);
            table.Add(0x8C, 0x34);
            table.Add(0x8D, 0x35);
            table.Add(0x8F, 0x36);
            table.Add(0x93, 0x37);
        }
        private void Read_Table()
        {
            throw new Exception("table not supported in pna writer atm");
            string table_file = ""; //System.Windows.Forms.Application.StartupPath + Path.DirectorySeparatorChar + "table.tbl";
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
                char_table.Add(lines[i][5], v);
            }
        }

        public void Set_Text()
        {
            data = new List<byte>();
            pos = 0;

            while (pos < text.Length)
            {
                currc = text[pos++];
                currb = Encode_Table(currc);

                if (currc == '|' || currc == '{' || currc == '[' || currc == ']')
                    Set_ControlCode();
                else if (currb[0] < 0xA6 || currb[0] > 0xDF)
                    Set_ASCII();
                else
                    Set_SJIS();
            }

            #region Deijin edit . remove this for writing names
            if (!nameMode)
            {
                if (text.Substring(text.Length - 5) != "{End}")
                {
                    if (sjis_sign)
                        Add_Bytes(0x1B, 0x48);


                    Add_Bytes(0x05, 0x05, 0x05);

                }
            }
            #endregion
        }


        void Set_ControlCode()
        {
            if (sjis_sign)
            {
                Add_Bytes(0x1B, 0x48);
                sjis_sign = false;
            }

            if (currc == '[' || currc == ']')
            {
                Add_Bytes(0x1B, 0x72);
                return;
            }

            if (currc == '|')
            {
                char c = ',';
                pos--;
                while (c == ',')
                {
                    pos++;
                    byte b = Convert.ToByte(text.Substring(pos, 2), 16);
                    data.Add(b);
                    pos += 2;
                    if (pos >= text.Length)
                        break;
                    c = text[pos];
                }
            }
            else        // '{'
            {
                pos--;
                int index = text.IndexOf('}', pos);
                string command = text.Substring(pos, index - pos + 1);

                if (command == "{Attack}")
                    Add_Bytes(0x02, 0x3C);
                else if (command == "{Poke}")
                    Add_Bytes(0x02, 0x46, 0x64);
                else if (command == "{Name2}")
                    Add_Bytes(0x02, 0x50, 0x64);
                else if (command == "{Area}")
                    Add_Bytes(0x02, 0x6E, 0x64);
                else if (command == "{Name3}")
                    Add_Bytes(0x02, 0x5A, 0x64);
                else if (command == "{Name}")
                    Add_Bytes(0x02, 0xCB);
                else if (command == "{C1}")
                    Add_Bytes(0x01, 0x22);
                else if (command == "{Turns}")
                    Add_Bytes(0x02, 0x32);
                else if (command == "{End}")
                {
                    if (sjis_sign)
                        Add_Bytes(0x1B, 0x48);

                    Add_Bytes(0x05, 0x05, 0x05);
                    index++;
                }

                if (command.StartsWith("{Wait:"))
                    Write_Params(command, 6, 1, 0x77);
                else if (command.StartsWith("{Color:"))
                    Write_Params(command, 7, 1, 0x63);
                else if (command.StartsWith("{NameColor:"))
                    Write_Params(command, 11, 1, 0x73);
                else if (command.StartsWith("{CharImage:"))
                    Write_Params(command, 11, 1, 0x66);
                else if (command.StartsWith("{Char:"))
                    Write_ParamsASCII(command, 6, 4, 0x40);

                if (command.StartsWith("{Unk1 P:"))
                {
                    Add_Bytes(0x01, 0x53, 0x25);

                    // First parameters
                    int p = 8;
                    while (true)
                    {
                        char c = command[p++];
                        if (c == ' ')
                            break;
                        data.Add((byte)c);
                    }

                    Add_Bytes(0x02, 0xCA, 0x25);
                    // Second parameters
                    p += 2;
                    while (true)
                    {
                        char c = command[p++];
                        if (c == '}')
                            break;
                        data.Add((byte)c);
                    }
                }
                else if (command.StartsWith("{Unk2 P:"))
                {
                    Add_Bytes(0x05, 0x05, 0x04, 0x02, 0xCA, 0x25);
                    int p = 8;
                    while (true)
                    {
                        char c = command[p++];
                        if (c == '}')
                            break;
                        data.Add((byte)c);
                    }
                }

                pos = index + 1;
            }
        }

        void Set_ASCII()
        {
            if (currb[0] < 0x81 || currb[0] > 0x9F)
            {
                if (currb[0] < 0xE0 || currb[0] > 0xFC)
                {
                    // ASCII char
                    data.Add(currb[0]);
                    //if (b1 == 0x0A && new_line)
                    //	Add_Bytes(0x1B, 0x48);
                    return;
                }
            }

            // It's a kanji
            Add_Bytes(currb);
        }
        void Set_SJIS()
        {
            if (currb.Length == 1)
            {
                sjis_sign = true;
                if (!sjis3)
                    Add_Bytes(0x1B, 0x6B);
                Add_Bytes(currb[0]);
                return;
            }

            byte b1 = currb[0];
            byte b2 = currb[1];

            if (b1 == 0x82 && sjis_sign)
            {
                Add_Bytes(0x1B, 0x48);
                sjis_sign = false;
            }
            else if (b1 == 0x83 && !sjis_sign)
            {
                Add_Bytes(0x1B, 0x4B);
                sjis_sign = true;
                new_line = true;
            }

            if (b1 == 0x82)
            {
                if (b2 >= 0xDE)
                    b2 += 1;
                b2 -= 0x5F;
            }

            if (b2 == 0x94)
            {
                Add_Bytes(0xB3, 0xDE);
                return;
            }

            byte index = 0;
            byte aux = 0;
            if (table.ContainsKey(b2))
                index = table[b2];
            else if (table.ContainsKey((byte)(b2 - 1)))
            {
                index = table[(byte)(b2 - 1)];
                aux = 0xDE;
            }
            else if (table.ContainsKey((byte)(b2 - 2)))
            {
                index = table[(byte)(b2 - 2)];
                aux = 0xDF;
            }
            else
            {
                // It's not the best form to do it but...
                Set_ASCII();
                return;
            }

            index += 0xA6;
            data.Add(index);
            if (aux != 0)
                data.Add(aux);
        }

        void Add_Bytes(params byte[] b)
        {
            for (int i = 0; i < b.Length; i++)
                data.Add(b[i]);
        }
        byte[] Encode_Table(char c)
        {
            if (c == '\n')
                return new byte[1] { 0x0A };

            #region Deijin's Addition

            if (c == 'ū')
            {
                return new byte[] { 0x84, 0x81 };
            }
            if (c == 'ō')
            {
                return new byte[] { 0x84, 0x90 };
            }

            #endregion

            if (char_table.ContainsKey(c))
            {
                byte[] b = BitConverter.GetBytes(char_table[c]);
                byte s = b[1];
                b[1] = b[0]; b[0] = s;

                if (b[0] == 0)
                    return new byte[1] { b[1] };
                else
                    return b;
            }
            else
            {
                if (!useTable)
                    return Encoding.GetEncoding(932).GetBytes(c.ToString());

                Console.WriteLine("Error encoding char {0}", c);
                Console.ReadKey(true);
                return null;
            }
        }
        void Write_Params(string cmd, int len, int num, byte code)
        {
            Add_Bytes(0x1B, code);
            for (int i = 0; i < num; i++)
            {
                string ps = cmd.Substring(len + 3 * i, 2);
                byte p = Convert.ToByte(ps, 16);
                data.Add(p);
            }
        }
        void Write_ParamsASCII(string cmd, int len, int num, byte code)
        {
            Add_Bytes(0x1B, code);
            for (int i = 0; i < num; i++)
            {
                byte c = (byte)cmd[len + i];
                data.Add(c);
            }
        }

        public Byte[] Data
        {
            get { return data.ToArray(); }
        }
    }
}
