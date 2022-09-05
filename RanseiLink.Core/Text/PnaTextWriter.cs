// PnaTextWriter.cs
//
// Author:
//       Benito Palacios Sanchez <benito356@gmail.com> (small modifications by Deijin)
//
// Copyright (c) 2019 Benito Palacios Sanchez
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
using System;
using System.IO;
using System.Globalization;
using System.Linq;

namespace RanseiLink.Core.Text
{
    public class PnaTextWriter
    {
        static readonly byte[] KanjiTable = {
            0x92, 0x40, 0x42, 0x44, 0x46, 0x48, 0x83, 0x85, 0x87, 0x62,
            0x00, 0x41, 0x43, 0x45, 0x47, 0x49, 0x4A, 0x4C, 0x4E, 0x50,
            0x52, 0x54, 0x56, 0x58, 0x5A, 0x5C, 0x5E, 0x60, 0x63, 0x65,
            0x67, 0x69, 0x6A, 0x6B, 0x6C, 0x6D, 0x6E, 0x71, 0x74, 0x77,
            0x7A, 0x7D, 0x7E, 0x80, 0x81, 0x82, 0x84, 0x86, 0x88, 0x89,
            0x8A, 0x8B, 0x8C, 0x8D, 0x8F, 0x93,
        };

        readonly BinaryWriter writer;

        string message;
        int pos;
        bool everKanjiPage2Mode;
        bool kanjiPage2Mode;
        bool textMode;

        public PnaTextWriter(BinaryWriter stream)
        {
            writer = stream;
        }

        public void WriteMessage(Message msg, bool multiElements)
        {
            Reset(msg);

            if (message == "{close}")
            {
                Write(0x00);
                return;
            }

            while (pos < message.Length)
            {
                char ch = message[pos++];
                byte[] data = GetCharBytes(ch);

                if (data.Length == 0)
                {
                    continue;
                }
                else if (ch == '{' || (ch == '『' || ch == '』'))
                {
                    WriteControlCode(ch);
                }
                else if (data[0] < 0xA6 || data[0] > 0xDF)
                {
                    WriteAscii(data);
                }
                else
                {
                    WriteShiftJis(data);
                }
            }

            if (message.Length == 0 && multiElements)
            {
                // Don't ask me why -.-'
                WriteTextStartToken();
            }

            // End of segment.
            WriteSjisEndToken();
            Write(0x05, 0x05, 0x05);
        }

        void Reset(Message msg)
        {
            pos = 0;
            message = msg.Context + msg.BoxConfig + msg.Text;

            kanjiPage2Mode = false;
            everKanjiPage2Mode = false;
            textMode = false;
        }

        byte[] GetCharBytes(char ch)
        {
            // Ignore Windows new lines
            if (ch == '\r')
            {
                return Array.Empty<byte>();
            }

            // Fast return on ASCII
            if (ch <= 0x7F)
            {
                return new byte[] { (byte)ch };
            }

            // Encode with ShiftJis
            byte[] encoded = EncodingProvider.ShiftJIS.GetBytes(new[] { ch });

            // Return only the last byte if the first one is 0x00
            return (encoded[0] == 0) ? new[] { encoded[1] } : encoded;
        }

        void WriteAscii(byte[] ch)
        {
            WriteTextStartToken();
            if ((ch[0] >= 0x81 && ch[0] <= 0x9F) || (ch[0] >= 0xE0 && ch[0] <= 0xFC))
            {
                // It's a single byte kanji
                Write(ch);
            }
            else
            {
                // It's English char (ASCII)
                WriteSjisEndToken();
                Write(ch[0]);

                if (ch[0] == '\n')
                {
                    // Bug in original developer tool
                    if (everKanjiPage2Mode)
                    {
                        Write(0x1B, 0x48);
                    }

                    kanjiPage2Mode = false;
                    textMode = false;
                }
            }
        }

        void WriteShiftJis(byte[] ch)
        {
            WriteTextStartToken();

            if (ch.Length == 1)
            {
                kanjiPage2Mode = true;
                everKanjiPage2Mode = true;

                // One-byte-kanji token and page2
                Write(0x1B, 0x6B, ch[0]);
                return;
            }

            if (ch[0] == 0x82)
            {
                WriteSjisEndToken();
                if (ch[1] >= 0xDE)
                {
                    ch[1]++;
                }

                ch[1] -= 0x5F;
            }
            else if (ch[0] == 0x83)
            {
                WriteSJisStartToken();
            }

            if (ch[1] == 0x94)
            {
                Write(0xB3, 0xDE);
                return;
            }

            byte token = 0;
            int index = Array.FindIndex(KanjiTable, x => x == ch[1]);
            if (index == -1)
            {
                token = 0xDE;
                index = Array.FindIndex(KanjiTable, x => x == (ch[1] - 1));

                if (index == -1)
                {
                    token = 0xDF;
                    index = Array.FindIndex(KanjiTable, x => x == (ch[1] - 1));

                    if (index == -1)
                    {
                        WriteAscii(ch);
                    }
                }
            }

            Write((byte)(index + 0xA6));
            if (token != 0)
            {
                Write(token);
            }
        }

        void WriteControlCode(char ch)
        {
            WriteSjisEndToken();

            if (ch == '『' || ch == '』')
            {
                // Furigana start/end token
                Write(0x1B, 0x72);
                return;
            }

            string command = ReadControl();

            // Text format
            if (command.StartsWith($"{PnaConstNames.Color}:"))
            {
                WriteTextStartToken();
                byte color = ReadVariableArgPair(command, $"{PnaConstNames.Color}:");
                Write(0x1B, 0x63, color);
                return;
            }
            else if (command.StartsWith($"{PnaConstNames.ScenarioWarrior}:"))
            {
                WriteTextStartToken();
                Write(0x1B, 0x40);
                writer.Write(command.Substring($"{PnaConstNames.ScenarioWarrior}:".Length).ToCharArray()); // NT
                return;
            }
            else if (command.StartsWith($"{PnaConstNames.SpeakerColor}:"))
            {
                WriteTextStartToken();
                byte color = ReadVariableArgPair(command, $"{PnaConstNames.SpeakerColor}:");
                Write(0x1B, 0x73, color);
                return;
            }
            else if (command.StartsWith($"{PnaConstNames.Emotion}:"))
            {
                WriteTextStartToken();
                byte index = ReadVariableArgPair(command, $"{PnaConstNames.Emotion}:");
                Write(0x1B, 0x66, index);
                return;
            }
            else if (command.StartsWith($"{PnaConstNames.Wait}:"))
            {
                WriteTextStartToken();
                byte wait = ReadVariableArg(command, $"{PnaConstNames.Wait}:");
                Write(0x1B, 0x77, wait);
                return;
            }

            textMode = false;

            // Variables
            if (command == "turns")
            {
                Write(0x02, 50);
            }
            else if (command.StartsWith("variable1:"))
            {
                byte num = ReadVariableArg(command, "variable1:");
                Write(0x02, (byte)(60 + num));
            }
            else if (command.StartsWith("enemy:"))
            {
                byte num = ReadVariableArg(command, "enemy:");
                Write(0x02, (byte)(60 + num), 0x64);
            }
            else if (command.StartsWith("pokemon:"))
            {
                byte num = ReadVariableArg(command, "pokemon:");
                Write(0x02, (byte)(70 + num), 0x64);
            }
            else if (command == "name2")
            {
                Write(0x02, 80, 0x64);
            }
            else if (command == "name_npc")
            {
                Write(0x02, 81, 0x64);
            }
            else if (command.StartsWith("lord_name_var:"))
            {
                byte num = ReadVariableArg(command, "lord_name_var:");
                Write(0x02, (byte)(80 + num), 0x65);
            }
            else if (command == "name3")
            {
                Write(0x02, 90, 0x64);
            }
            else if (command.StartsWith("item:"))
            {
                byte num = ReadVariableArg(command, "item:");
                Write(0x02, (byte)(100 + num), 0x64);
            }
            else if (command.StartsWith("area:"))
            {
                byte num = ReadVariableArg(command, "area:");
                Write(0x02, (byte)(110 + num), 0x64);
            }
            else if (command == "map_obj")
            {
                Write(0x02, 120, 0x64);
            }
            else if (command == "lord_name")
            {
                Write(0x02, 204);
            }
            else if (command == "name1")
            {
                Write(0x02, 203);
            }
            else if (command.StartsWith($"{PnaConstNames.SpeakerId}:"))
            {
                Write(0x02, 202, 0x25);
                writer.Write(command.Substring($"{PnaConstNames.SpeakerId}:".Length).ToCharArray()); // NT
            }
            else if (command.StartsWith("param:"))
            {
                Write(0x02, 201, 0x25);
                writer.Write(command.Substring("param:".Length).ToCharArray()); // NT
            }

            // Text bound
            else if (command.StartsWith("text-if:"))
            {
                Write(0x05, 0x05, 0x04);

                // It should parse any kind of "text variable" but in the
                // practice, only "speaker_id:" follows so:
                string content = command.Substring("text-if:".Length);
                if (!content.StartsWith($"{{{PnaConstNames.SpeakerId}:"))
                {
                    throw new FormatException("Unexpected 'text-if' command");
                }

                int endToken = content.IndexOf('}');
                if (endToken == -1)
                {
                    throw new FormatException("Missing end token");
                }

                int start = $"{{{PnaConstNames.SpeakerId}:".Length;
                string param = content.Substring(start, endToken - start);
                Write(0x02, 202, 0x25);
                writer.Write(param.ToCharArray()); // NT
            }

            // Text start
            else if (command.StartsWith("multi-start:"))
            {
                Write(0x01, 0x53);
                var args = command.Substring("multi-start:".Length).Split(',');

                // It should parse any kind of "text variable" but in the
                // practice, only "51,{param:51}"follows so:
                if (args[0].StartsWith("{"))
                {
                    throw new FormatException("Unexpected token in multi-start");
                }

                Write(0x25);
                writer.Write(args[0].ToCharArray()); // NT

                if (!args[1].StartsWith("{param:"))
                {
                    throw new FormatException("Unexpected token in multi-start");
                }

                int endToken = args[1].IndexOf('}');
                if (endToken == -1)
                {
                    throw new FormatException("Missing end token");
                }

                int start = "{param:".Length;
                string param = args[1].Substring(start, endToken - start);
                Write(0x02, 201, 0x25);
                writer.Write(param.ToCharArray()); // NT
            }
            else if (command.StartsWith("B22-9:"))
            {
                byte num = ReadVariableArg(command, "B22-9:");
                Write(0x09, num);
            }
            else
            {
                throw new FormatException($"Invalid control code: {command}");
            }
        }

        void WriteSJisStartToken()
        {
            if (!kanjiPage2Mode)
            {
                Write(0x1B, 0x4B);
            }

            kanjiPage2Mode = true;
            everKanjiPage2Mode = true;
        }

        void WriteSjisEndToken()
        {
            if (kanjiPage2Mode)
            {
                Write(0x1B, 0x48);
            }

            kanjiPage2Mode = false;
        }

        void WriteTextStartToken()
        {
            if (!textMode)
            {
                Write(0x01, 0x22);
            }

            textMode = true;
        }

        byte ReadVariableArg(string command, string token)
        {
            string arg = command.Substring(token.Length);
            return byte.Parse(arg, NumberStyles.Integer);
        }

        byte ReadVariableArgPair(string command, string token)
        {
            string arg = command.Substring(token.Length);
            var num = byte.Parse(arg, NumberStyles.Integer);
            var final = (num & 0b1111) | (0b0011 << 4); // the other param is always 3, so it is omitted then added back in here.
            return (byte)final;
        }

        string ReadControl()
        {
            int startPos = pos;
            int endPos = -1;

            int levels = 1;
            while (endPos == -1 && pos < message.Length)
            {
                char ch = message[pos++];
                if (ch == '}')
                {
                    levels--;
                }
                else if (ch == '{')
                {
                    levels++;
                }

                if (levels == 0)
                {
                    endPos = pos - 1;
                }
            }

            if (endPos == -1)
            {
                throw new FormatException("Cannot find end of token");
            }

            return message.Substring(startPos, endPos - startPos);
        }

        void Write(params byte[] data)
        {
            writer.Write(data);
        }
    }
}