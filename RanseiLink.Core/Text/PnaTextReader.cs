#nullable enable
// PnaTextReader.cs
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
using System.Text;

namespace RanseiLink.Core.Text
{

    public class PnaTextReader
    {
        static readonly byte[] KanjiTable = {
            0x92, 0x40, 0x42, 0x44, 0x46, 0x48, 0x83, 0x85, 0x87, 0x62,
            0x00, 0x41, 0x43, 0x45, 0x47, 0x49, 0x4A, 0x4C, 0x4E, 0x50,
            0x52, 0x54, 0x56, 0x58, 0x5A, 0x5C, 0x5E, 0x60, 0x63, 0x65,
            0x67, 0x69, 0x6A, 0x6B, 0x6C, 0x6D, 0x6E, 0x71, 0x74, 0x77,
            0x7A, 0x7D, 0x7E, 0x80, 0x81, 0x82, 0x84, 0x86, 0x88, 0x89,
            0x8A, 0x8B, 0x8C, 0x8D, 0x8F, 0x93,
        };

        readonly BinaryReader stream;

        StringBuilder textBuilder = null!;
        StringBuilder contextBuilder = null!;
        StringBuilder boxBuilder = null!;
        StringBuilder builder = null!;
        bool kanjiSingleByteMode;
        bool kanjiPage2Mode;
        bool furiganaMode;
        bool endString;

        public PnaTextReader(BinaryReader stream)
        {
            this.stream = stream;
        }

        public Message ReadMessage()
        {
            Reset();

            while (!endString && stream.BaseStream.Position < stream.BaseStream.Length)
            {
                byte data = stream.ReadByte();

                if (data < 0x20 && data != 0x0A)
                    ParseControlCode(data);
                else if (data < 0xA6 || data > 0xDF)
                    ParseAsciiText(data);
                else
                    ParseJapaneseText(data);
            }

            Message msg = new Message(
                text: textBuilder.ToString(),
                context: contextBuilder.ToString(),
                boxConfig: boxBuilder.ToString()
            );

            return msg;
        }

        static string GetJapaneseString(params byte[] data)
        {
            return EncodingProvider.ShiftJISExtended.GetString(data);
        }

        void Reset()
        {
            textBuilder = new StringBuilder();
            contextBuilder = new StringBuilder();
            boxBuilder = new StringBuilder();
            builder = contextBuilder;
            kanjiSingleByteMode = false;
            kanjiPage2Mode = false;
            furiganaMode = false;
            endString = false;
        }

        void SetTextBuilder()
        {
            builder = textBuilder;
        }

        void ParseAsciiText(byte ch)
        {
            SetTextBuilder();

            if ((ch >= 0x81 && ch <= 0x9F) || (ch >= 0xE0 && ch <= 0xFC))
            {
                // Japanese kanji
                byte ch2 = stream.ReadByte();
                builder.Append(GetJapaneseString(ch, ch2));
            }
            else
            {
                // ASCII
                builder.Append((char)ch);
            }
        }

        void ParseJapaneseText(byte ch)
        {
            SetTextBuilder();

            if (kanjiSingleByteMode)
            {
                builder.Append(GetJapaneseString(ch));
                return;
            }

            byte firstByte = kanjiPage2Mode ? (byte)0x83 : (byte)0x82;
            byte secondByte = KanjiTable[ch - 0xA6];

            // There may be a second control byte
            byte control = stream.ReadByte();
            if (control == 0xDE)
            {
                secondByte = (ch == 0xB3) ? (byte)0x94 : (byte)(secondByte + 1);
            }
            else if (control == 0xDF)
            {
                secondByte += 2;
            }
            else
            {
                // Wrong data, go back
                stream.BaseStream.Position--;
            }

            if (!kanjiPage2Mode)
            {
                if (secondByte >= 0x80)
                    secondByte--;
                secondByte += 0x5F;
            }

            builder.Append(GetJapaneseString(firstByte, secondByte));
        }

        void ParseControlCode(byte control)
        {
            // Control codes reset the kanji page2 mode
            kanjiPage2Mode = false;

            switch (control)
            {
                case 0x00:
                    builder.Append("{close}");
                    endString = true;
                    break;

                case 0x01:
                    ParseTextStart();
                    break;

                case 0x02:
                    SetTextBuilder();
                    ParseVariables(control);
                    break;

                case 0x05:
                    ParseTextBound();
                    break;

                // weird thing in block 22 of japanese rom
                case 0x09:
                    builder.Append($"{{B22-9:{stream.ReadByte()}}}");
                    break;

                case 0x1B:
                    ParseTextFormat();
                    break;

                default:
                    throw new FormatException("Unsupported control code");
            }
        }

        void ParseTextStart()
        {
            byte code = stream.ReadByte();

            if (code == 0x22)
            {
                // Means start of literals
                // Added when starting SJIS or ASCII text, needs after variable
                builder = boxBuilder;
            }
            else if (code == 0x53)
            {
                builder.Append("{multi-start:");
                ParseVariables(stream.ReadByte());
                builder.Append(",");
                ParseVariables(stream.ReadByte());
                builder.Append("}");
            }
            else
            {
                throw new FormatException("Unsupported Control1 code");
            }
        }

        void ParseTextBound()
        {
            if (stream.ReadByte() != 0x05)
                throw new FormatException("Invalid Control5 code1");

            byte code = stream.ReadByte();
            if (code == 0x04)
            {
                builder.Append("{text-if:");
                ParseVariables(stream.ReadByte());
                builder.Append("}");
            }
            else if (code == 0x05)
            {
                endString = true;
            }
            else
            {
                throw new FormatException("Unsupported Control5 code");
            }
        }

        void ParseTextFormat()
        {
            byte code = stream.ReadByte();
            switch (code)
            {
                case 0x40:
                    string charIdx = ReadParameter();
                    builder.Append($"{{{PnaConstNames.ScenarioWarrior}:{charIdx}}}");
                    break;

                case 0x48:
                    kanjiPage2Mode = false;
                    kanjiSingleByteMode = false;
                    break;

                case 0x4B:
                    kanjiPage2Mode = true;
                    break;

                case 0x63:
                    SetTextBuilder();
                    byte color = stream.ReadByte();
                    builder.Append($"{{{PnaConstNames.Color}:{color & 0b1111}}}"); // other 4 bits is always 0b0011
                    break;

                case 0x66:
                    byte charImgIdx = stream.ReadByte();
                    builder.Append($"{{{PnaConstNames.Emotion}:{charImgIdx & 0b1111}}}"); // other 4 bits is always 0b0011
                    break;

                case 0x6B:
                    kanjiPage2Mode = true;
                    kanjiSingleByteMode = true;
                    break;

                // Indicate the start or end of the furigana
                case 0x72:
                    SetTextBuilder();
                    furiganaMode = !furiganaMode;
                    builder.Append(furiganaMode ? "『" : "』");
                    break;

                case 0x73:
                    byte speakerColor = stream.ReadByte();
                    builder.Append($"{{{PnaConstNames.SpeakerColor}:{speakerColor & 0b1111}}}"); // other 4 bits is always 0b0011
                    break;

                case 0x77:
                    SetTextBuilder();
                    byte wait = stream.ReadByte();
                    builder.Append($"{{{PnaConstNames.Wait}:{wait}}}");
                    break;

                default:
                    throw new FormatException("Unsupported Control1B code");
            }
        }

        void ParseVariables(byte control)
        {
            if (control == 0x25)
            {
                builder.Append(ReadParameter());
                return;
            }

            if (control != 0x02)
                throw new FormatException("Invalid control code for variables");

            byte varId = stream.ReadByte();
            int group = varId / 10;
            int modulo = varId % 10;
            int index = (varId < 200) ? stream.ReadByte() : 0;

            switch (group)
            {
                case 5:
                    if (index < 0x20)
                    {
                        stream.BaseStream.Position--;
                        builder.Append("{turns}");
                    }
                    else
                    {
                        throw new FormatException("Unknown variable group 5");
                    }

                    break;

                case 6:
                    if (index < 0x20)
                    {
                        stream.BaseStream.Position--;
                        builder.Append($"{{variable1:{modulo}}}");
                    }
                    else if (index == 100)
                    {
                        builder.Append($"{{enemy:{modulo}}}");
                    }
                    else
                    {
                        throw new FormatException("Unknown variable group 6");
                    }

                    break;

                case 7:
                    if (index == 100)
                        builder.Append($"{{pokemon:{modulo}}}");
                    else
                        throw new FormatException("Unknown variable group 7");
                    break;

                case 8:
                    if (index == 100 && modulo == 0)
                        builder.Append("{name2}");
                    else if (index == 100)
                        builder.Append("{name_npc}");
                    else if (index == 101)
                        builder.Append($"{{lord_name_var:{modulo}}}");
                    else
                        throw new FormatException("Unknown variable group 8");
                    break;

                case 9:
                    if (index == 100)
                        builder.Append("{name3}");
                    else
                        throw new FormatException("Unknown variable group 9");
                    break;

                case 10:
                    if (index == 100)
                        builder.Append($"{{item:{modulo}}}");
                    else
                        throw new FormatException("Unknown variable group 10");
                    break;

                case 11:
                    if (index == 100)
                        builder.Append($"{{area:{modulo}}}");
                    else
                        throw new FormatException("Unknown variable group 11");
                    break;

                case 12:
                    if (index == 100)
                        builder.Append("{map_obj}");
                    else
                        throw new FormatException("Unknown variable group 12");
                    break;

                case 20:
                    // 0 also call ParseExpression
                    if (modulo == 1)
                        builder.Append($"{{param:{ParseExpression()}}}");
                    else if (modulo == 2)
                        builder.Append($"{{{PnaConstNames.SpeakerId}:{ParseExpression()}}}");
                    else if (modulo == 3)
                        builder.Append("{name1}");
                    else if (modulo == 4)
                        builder.Append("{lord_name}");
                    else
                        throw new FormatException("Unknown variable group 20");
                    break;

                default:
                    throw new FormatException("Invalid variable group");
            }
        }

        string ParseExpression()
        {
            // It should actually call ParseVariable, but we only support
            // constants (code 0x25)
            if (stream.ReadByte() != 0x25)
                throw new FormatException("Unsupported expression");
            string result = ReadParameter();

            // We can do maths if the next code is 0x03 (not supported yet)
            // There would be a next byte (0x25 - 0x2F) with the math op
            // and then another constant to apply the math. Repeat for next 0x03
            if (stream.ReadByte() == 0x03)
                throw new FormatException("Unsupported math expression");
            else
                stream.BaseStream.Position--;

            return result;
        }

        string ReadParameter()
        {
            StringBuilder text = new StringBuilder();
            for (int i = 0; i < 10; i++)
            {
                byte ch = stream.ReadByte();
                if (ch < '0' || ch > '9')
                {
                    stream.BaseStream.Position--;
                    break;
                }

                text.Append((char)ch);
            }

            return text.ToString();
        }
    }
}