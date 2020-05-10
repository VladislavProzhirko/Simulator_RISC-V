using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace TestSim
{
    class Memory
    {
        string Path_memory { get; } = "lab_riscv_asm.hex";
        string Path_register { get; } = "register.hex";
        string Path_new_memory { get; } = "new_memory.hex";
        string Mem_pointer { get; set; }
        StreamReader reader { get; set; }
        StreamReader reg_reader { get; set; }
        public Dictionary<string, int> arr_var = new Dictionary<string, int>();
        public string Begin_stack_mem { get; } = "0000FFF0";
        public string Begin_data_mem { get; } = "00001000";
        public string Code_pointer { get; } = "00000000";
        public int X0 { get; } = 0;

        public Memory()
        {
            // create memory
            Process pr = Process.Start(@"toolchain\create_hex.bat", "lab_riscv_asm");
            while (!pr.HasExited) { }
            using (StreamWriter sw = File.AppendText(Path_memory))
            //add stack
            {
                sw.WriteLine("@0000F100");
                for (int i = 0; i < 4; i++)
                {
                    for (int j = 0; j < 16; j++)
                    {
                        sw.Write("00 ");
                    }
                    sw.WriteLine("");
                }
            }
            // set register
            Set_reg();
            reader = new StreamReader(File.Open(Path_memory, FileMode.Open));
            reader.ReadLine(); //skip @00000000
            Mem_pointer = Code_pointer;
        }

        public string Read_code(string PC)
        {
            char[] buf = new char[12];
            // if PC found
            if (PC == Mem_pointer)
            {
                reader.Read(buf, 0, 12); // read 4 bytes
                Mem_pointer = (Convert.ToInt32(Mem_pointer, 16) + 4).ToString("X").PadLeft(8, '0'); // Mem_pointer next (+4)
                return new string(buf);
            }
            else
            {
                //if pc = {0 to 0x1000}
                if (String.Compare(PC, Code_pointer) >= 0 && String.Compare(PC, Begin_data_mem) < 0)
                {
                    // if Mem_pointer < PC
                    if (String.Compare(PC, Mem_pointer) < 0 || String.Compare(Mem_pointer,Begin_data_mem) > 0)
                    {
                        reader.BaseStream.Position = 0;
                        reader.DiscardBufferedData(); // clear buf_read_file
                        reader.ReadLine();
                        Mem_pointer = Code_pointer;

                        if (PC == Mem_pointer)
                        {
                            reader.Read(buf, 0, 12);
                            Mem_pointer = (Convert.ToInt32(Mem_pointer, 16) + 4).ToString("X").PadLeft(8, '0');
                            return new string(buf);
                        }
                    }
                    int row = ((Convert.ToInt32(PC, 16) - Convert.ToInt32(Mem_pointer, 16)) / 16);// row to PC from 0x0000
                    int i = 0;
                    while (i < row)
                    {
                        reader.ReadLine();
                        i++;
                    }
                    Mem_pointer = PC.Substring(0,7) + "0";

                    //find PC
                    while (true)
                        if (PC != Mem_pointer)
                        {
                            reader.Read(buf, 0, 12);
                            Mem_pointer = (Convert.ToInt32(Mem_pointer, 16) + 4).ToString("X").PadLeft(8, '0');
                            // if end line (\r\n)
                            if (Convert.ToInt32(Mem_pointer, 16) % 16 == 0)
                                reader.ReadLine();
                        }
                        else
                        {
                            reader.Read(buf, 0, 12);
                            Mem_pointer = (Convert.ToInt32(Mem_pointer, 16) + 4).ToString("X").PadLeft(8, '0');
                            // if end line (\r\n)
                            if (Convert.ToInt32(Mem_pointer, 16) % 16 == 0)
                                reader.ReadLine();
                            return new string(buf);
                        }
                }
                //if not correct PC
                else
                {
                    Console.WriteLine("PC not correct");
                    return "errror";
                }
            }

        }

        //funct load byte to memory
        public string Read_data_byte(string PC)
        {
            return Read_data(PC, 1);
        }

        //funct load word to memory
        public string Read_data_hw(string PC)
        {
            return Read_data(PC, 2);
        }

        //funct load word to memory
        public string Read_data_word(string PC)
        {
            return Read_data(PC, 4);
        }
        //funct load to memory
        string Read_data(string PC, int type)
        {
            char[] buf = new char[12];
            int i;
            //if PC = {0x1000 to 0xf100}
            if (PC == Mem_pointer)
            {
                if (type == 1) // byte
                {
                    reader.Read(buf, 0, 3);
                    Mem_pointer = (Convert.ToInt32(Mem_pointer, 16) + 1).ToString("X").PadLeft(8, '0');
                    if (Convert.ToInt32(Mem_pointer, 16) % 16 == 0)
                        reader.ReadLine();
                    return new string(buf).Substring(0, 2);
                }
                else
                if (type == 2) // halfword
                {
                    for (int j = 0; j < 2; j++)
                    {
                        reader.Read(buf, 3 - j * 3, 3);
                        Mem_pointer = (Convert.ToInt32(Mem_pointer, 16) + 1).ToString("X").PadLeft(8, '0');
                        if (Convert.ToInt32(Mem_pointer, 16) % 16 == 0)
                            reader.ReadLine();
                    }
                    return new string(buf).Replace(" ", "");
                }
                else // word
                {
                    for (int j = 0; j < 4; j++)
                    {
                        reader.Read(buf, 12 - j * 3, 3);
                        Mem_pointer = (Convert.ToInt32(Mem_pointer, 16) + 1).ToString("X").PadLeft(8, '0');
                        if (Convert.ToInt32(Mem_pointer, 16) % 16 == 0)
                            reader.ReadLine();
                    }
                    return new string(buf).Replace(" ", "");
                }
            }
            else
            if (String.Compare(PC, Begin_data_mem) >= 0 && String.Compare(PC, Begin_stack_mem) < 0)
            {
                if (String.Compare(PC, Mem_pointer) < 0  || String.Compare(Mem_pointer,Begin_data_mem)<0)
                {
                    reader.BaseStream.Position = 0;
                    reader.DiscardBufferedData(); // clear buf_read_file
                    Mem_pointer = Begin_data_mem;
                    i = 1;
                    //skip to PC line
                    while (i < 259)
                    {
                        reader.ReadLine();
                        i++;
                    }
                    Mem_pointer = PC.Remove(7) + "0";

                    // if PC = 0x1000
                    if (PC == Mem_pointer)
                    {
                        if (type == 1)
                        {
                            reader.Read(buf, 0, 3);
                            Mem_pointer = (Convert.ToInt32(Mem_pointer, 16) + 1).ToString("X").PadLeft(8, '0');
                            if (Convert.ToInt32(Mem_pointer, 16) % 16 == 0)
                                reader.ReadLine();
                            return new string(buf).Substring(0, 2);
                        }
                        else //halfword
                        if (type == 2)
                        {
                            for (int j = 0; j < 2; j++)
                            {
                                reader.Read(buf, 3 - j * 3, 3);
                                Mem_pointer = (Convert.ToInt32(Mem_pointer, 16) + 1).ToString("X").PadLeft(8, '0');
                                if (Convert.ToInt32(Mem_pointer, 16) % 16 == 0)
                                    reader.ReadLine();
                            }
                            return new string(buf).Replace(" ", "");
                        }
                        else//word
                        {
                            for (int j = 0; j < 4; j++)
                            {
                                reader.Read(buf, 9 - j * 3, 3);
                                Mem_pointer = (Convert.ToInt32(Mem_pointer, 16) + 1).ToString("X").PadLeft(8, '0');
                                if (Convert.ToInt32(Mem_pointer, 16) % 16 == 0)
                                    reader.ReadLine();
                            }
                            return new string(buf).Replace(" ", "");
                        }
                    }
                }
            }

                int row = ((Convert.ToInt32(PC, 16) - Convert.ToInt32(Mem_pointer, 16)) / 16);// row to PC from 0x1000
                i = 0;
                while (i < row)
                {
                    reader.ReadLine();
                    i++;
                }
                Mem_pointer = PC.Remove(7) + "0";
                int offset_words = Convert.ToInt32(PC.Substring(7,1), 16) / 4; // skip words to PC
                i = 0;
                while (i < offset_words)
                {
                    reader.Read(buf, 0 , 12);
                    Mem_pointer = (Convert.ToInt32(Mem_pointer, 16) + 4).ToString("X").PadLeft(8, '0');
                    if (Convert.ToInt32(Mem_pointer, 16) % 16 == 0)
                        reader.ReadLine();
                    i++;
                }
            if (PC != Mem_pointer)
            //find PC
            {
                while (true)
                    if (PC != Mem_pointer)
                    {
                        reader.Read(buf, 0, 3);
                        Mem_pointer = (Convert.ToInt32(Mem_pointer, 16) + 1).ToString("X").PadLeft(8, '0');
                        // if end line (\r\n)
                        if (Convert.ToInt32(Mem_pointer, 16) % 16 == 0)
                            reader.ReadLine();
                    }
                    else break;
            }
          
                if (type == 1) // byte
                {
                    reader.Read(buf, 0, 3);
                    Mem_pointer = (Convert.ToInt32(Mem_pointer, 16) + 1).ToString("X").PadLeft(8, '0');
                    if (Convert.ToInt32(Mem_pointer, 16) % 16 == 0)
                        reader.ReadLine();
                    return new string(buf).Substring(0, 2);
                }
            if (type == 2)
            {
                for (int j = 0; j < 2; j++)
                {
                    reader.Read(buf, 3 - j * 3, 3);
                    Mem_pointer = (Convert.ToInt32(Mem_pointer, 16) + 1).ToString("X").PadLeft(8, '0');
                    if (Convert.ToInt32(Mem_pointer, 16) % 16 == 0)
                        reader.ReadLine();
                }
                return new string(buf).Replace(" ", "");
            }
            else//word
            {
                for (int j = 0; j < 4; j++)
                {
                    reader.Read(buf, 9 - j * 3, 3);
                    Mem_pointer = (Convert.ToInt32(Mem_pointer, 16) + 1).ToString("X").PadLeft(8, '0');
                    if (Convert.ToInt32(Mem_pointer, 16) % 16 == 0)
                        reader.ReadLine();
                }
                return new string(buf).Replace(" ", "");
            }

        }

        //funct store to memory
        public void Write_data(string address, string data)
        {
            using (StreamWriter sw = new StreamWriter(Path_new_memory))
            {
                reader.BaseStream.Position = 0;
                reader.DiscardBufferedData(); // clear buf_read_file
                //copy to file from 0x1000
                int i = 1;
                int row = ((Convert.ToInt32(address, 16) - Convert.ToInt32(Begin_data_mem, 16)) / 16);// row to address from 0x1000
                while (i < row + 259)
                {
                    sw.WriteLine(reader.ReadLine());
                    i++;
                }
                // skip words to PC
                int offset_words = Convert.ToInt32(address.Substring(7), 16) / 4; 
                i = 0;
                Mem_pointer = address.Substring(0,7) + "0";
                char[] buf = new char[12];
                while (i < offset_words)
                {
                    reader.Read(buf, 0, 12);
                    sw.Write(new string (buf));
                    Mem_pointer = (Convert.ToInt32(Mem_pointer, 16) + 4).ToString("X").PadLeft(8, '0');
                    if (Convert.ToInt32(Mem_pointer, 16) % 16 == 0)
                    {
                        reader.ReadLine();
                        sw.Write("\n");
                    }
                    i++;
                }


                while (true)
                    if (address != Mem_pointer)
                    {
                        reader.Read(buf, 0, 3);
                        sw.Write(new string (buf).Substring(0,3));
                        Mem_pointer = (Convert.ToInt32(Mem_pointer, 16) + 1).ToString("X").PadLeft(8, '0');
                        // if end line (\r\n)
                        if (Convert.ToInt32(Mem_pointer, 16) % 16 == 0)
                        {
                            string s = reader.ReadLine();
                            sw.Write("\n");
                        }
                    }
                    else
                    // load data to memory
                    {
                        if (data.Length == 2) //byte
                        {
                            sw.Write(data + " ");
                            reader.Read(buf, 0, 3);
                            Mem_pointer = (Convert.ToInt32(Mem_pointer, 16) + 1).ToString("X").PadLeft(8, '0');
                            if (Convert.ToInt32(Mem_pointer, 16) % 16 == 0)
                            {
                                reader.ReadLine();
                                sw.Write("\n");
                            }
                        }
                        else 
                        {
                            //halfword
                            if (data.Length == 4)
                            {
                                for (i = 0; i < 2; i++)
                                {
                                    reader.Read(buf, 0, 3);

                                    sw.Write(data.Substring(2 - (i * 2), 2) + " ");

                                    Mem_pointer = (Convert.ToInt32(Mem_pointer, 16) + 1).ToString("X").PadLeft(8, '0');
                                    if (Convert.ToInt32(Mem_pointer, 16) % 16 == 0)
                                    {
                                        reader.ReadLine();
                                        sw.Write("\n");
                                    }
                                }
                            }
                            else // word
                                for (i = 0; i < 4; i++)
                                {
                                    reader.Read(buf, 0, 3);

                                    sw.Write(data.Substring(6 - (i * 2), 2) + " ");

                                    Mem_pointer = (Convert.ToInt32(Mem_pointer, 16) + 1).ToString("X").PadLeft(8, '0');
                                    if (Convert.ToInt32(Mem_pointer, 16) % 16 == 0)
                                    {
                                        reader.ReadLine();
                                        sw.Write("\n");
                                    }
                                }

                        }
                        // load other file
                        sw.Write(reader.ReadToEnd());
                        reader.Close();
                        sw.Close();
                        File.Move(Path_new_memory,Path_memory,true);
                        reader = new StreamReader(Path_memory);
                        break;                    
                    }

            }


        }

        public string Read_reg_byte(string reg)
        {
            return Read_reg(reg, 1);
        }

        //funct load word to reg
        public string Read_reg_hw(string PC)
        {
            return Read_reg(PC, 2);
        }

        //funct load word to reg
        public string Read_reg_word(string reg)
        {
            return Read_reg(reg, 4);
        }

        //funct load to reg
        string Read_reg(string register, int type)
        {
            reg_reader = new StreamReader(File.Open(Path_register,FileMode.Open));
            int number = Convert.ToInt32(register.TrimStart('x'));
            int i = 0;
            while (i < number)
            {
                reg_reader.ReadLine();
                i++;
            }
            string buf = reg_reader.ReadLine();
            reg_reader.Close();
            if (type == 1)
                return buf.Substring(6, 2);
            else
                if (type == 2)
                return buf.Substring(4,4);
                else 
                    return buf;

        }


        public void Write_reg(string register, string data)
        {
            var buf = File.ReadAllLines(Path_register);
            int number = Convert.ToInt32(register.TrimStart('x'));
            if (number != 0)
                buf[number] = data;
        }

        //create register file
        void Set_reg()
        {

            byte[] info;
            using (FileStream fs = File.Create(Path_register))
            {
                for (int i = 0; i < 32; i++)
                {
                    if (i == 2)
                        info = new UTF8Encoding(true).GetBytes(Begin_data_mem + "\n");
                    else
                        if (i == 3)
                        info = new UTF8Encoding(true).GetBytes(Begin_stack_mem + "\n");
                    else
                        info = new UTF8Encoding(true).GetBytes("00000000\n");

                    fs.Write(info, 0, info.Length);
                }
            }
        }
    }
}
