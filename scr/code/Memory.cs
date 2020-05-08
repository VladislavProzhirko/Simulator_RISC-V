using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace TestSim
{
    class Memory
    {
        string Path_memory { get; } = @"memory.txt";
        string Path_register { get; } = @"register.txt";
        public Dictionary<string, int> arr_var = new Dictionary<string, int>();
        public string Stack_pointer { get; } ="0xFFF0";
        public string Begin_data_mem { get; } = "0x1000";
        public int X0 { get; } = 0;

        public Memory() 
        {
            Set_reg();
            Clear_mem();
        }

        // write data segments in memory
        public void Write_data(string path_input_file)
        {
            using StreamReader input_file = File.OpenText(path_input_file);
            string s = "";
            while ((s = input_file.ReadLine()) != null)
            {
                if (s == ".data")
                {
                    int j = 0; // offset mem 0x1000
                    int offset_mem = 0;// offset for mem {0, 1, 2, 3}
                    s = input_file.ReadLine();
                    while (s != ".text")
                    {
                        if (!(String.IsNullOrEmpty(s) || String.IsNullOrWhiteSpace(s)))
                        {
                            if (s.IndexOf(':') != -1)
                            {
                                string name = s.TrimEnd(':').Replace(" ", "");
                                int count = 0;
                                while (((s = input_file.ReadLine()).IndexOf(':') == -1) && s != ".text")
                                {
                                    if (!(String.IsNullOrEmpty(s) || String.IsNullOrWhiteSpace(s)))
                                    {
                                        string type = s.Split(' ')[0];
                                        s = s.Replace(" ", "").Substring(5);
                                        string[] els_data = s.Split(',');

                                        for (int i = 0; i < els_data.Length; i++)
                                        {
                                            if (type == ".byte")
                                            {
                                                offset_mem = Store_byte(j, els_data[i], offset_mem);
                                                if ( offset_mem == 0)
                                                    j++;
                                            }
                                            else
                                            {
                                                Store_word(j, els_data[i], offset_mem);
                                                j++;
                                            }

                                        }

                                        count += els_data.Length;
                                    }
                                }
                                arr_var.Add(name, count);
                            }
                        }
                    }

                    break;
                }
            }
        }

        public int Store_byte(int offset_address, string data, int offset)
        {
            int row_address = Convert.ToInt32(Begin_data_mem.Substring(2), 16)/ 4 + offset_address;
            string[] file_mem = File.ReadAllLines(Path_memory);

            if (data.IndexOf('x') == -1)
                data = Convert.ToInt32(data, 10).ToString("X").PadLeft(2, '0');
            else
                data = data.Substring(2).PadLeft(2, '0');

            if (offset == 0) //offset 0 byte
            {
                    file_mem[row_address] = file_mem[row_address].Split(' ')[0] + " " + data + file_mem[row_address].Split(' ')[1][2] + file_mem[row_address].Split(' ')[1][3] + file_mem[row_address].Split(' ')[1][4] + file_mem[row_address].Split(' ')[1][5] + file_mem[row_address].Split(' ')[1][6] + file_mem[row_address].Split(' ')[1][7];
                    offset++;
            }
            else
            if (offset == 1) // offset 1 byte
            {
                    file_mem[row_address] = file_mem[row_address].Split(' ')[0] + " " + file_mem[row_address].Split(' ')[1][0] + file_mem[row_address].Split(' ')[1][1] + data + file_mem[row_address].Split(' ')[1][4] + file_mem[row_address].Split(' ')[1][5] + file_mem[row_address].Split(' ')[1][6] + file_mem[row_address].Split(' ')[1][7];
                    offset++;

            }
            else
            if (offset == 2) //offset 2 byte
                {
                    file_mem[row_address] = file_mem[row_address].Split(' ')[0] + " " + file_mem[row_address].Split(' ')[1][0] + file_mem[row_address].Split(' ')[1][1] + file_mem[row_address].Split(' ')[1][2] + file_mem[row_address].Split(' ')[1][3] + data + file_mem[row_address].Split(' ')[1][6] + file_mem[row_address].Split(' ')[1][7];
                    offset++;
                }

            else // offset 3 byte
            {
                file_mem[row_address] = file_mem[row_address].Split(' ')[0] + " " + file_mem[row_address].Split(' ')[1][0] + file_mem[row_address].Split(' ')[1][1] + file_mem[row_address].Split(' ')[1][2] + file_mem[row_address].Split(' ')[1][3] + file_mem[row_address].Split(' ')[1][4] + file_mem[row_address].Split(' ')[1][5] + data;
                offset = 0;
            }

            StreamWriter streamWriter = new StreamWriter(Path_memory);
            foreach (string el in file_mem)
                streamWriter.WriteLine(el);
            streamWriter.Close();
            return offset;
        }

        public void Store_word(int offset_address, string data, int offset)
        {
            int row_address = Convert.ToInt32(Begin_data_mem.Substring(2), 16) / 4 + offset_address;
            string[] file_mem = File.ReadAllLines(Path_memory);
            if (data.IndexOf('x') == -1)
                data = Convert.ToInt32(data, 10).ToString("X").PadLeft(8, '0');
            else
                data = data.Substring(2).PadLeft(8, '0');

            if (offset == 0) //offset 0 byte
                    file_mem[row_address] = file_mem[row_address].Split(' ')[0] + " " + data[6] + data[7] + data[4] + data[5] + data[2] + data[3] + data[0] + data[1];
            else
                if (offset == 1) // offset 1 byte
                {
                    file_mem[row_address] = file_mem[row_address].Split(' ')[0] + " " + file_mem[row_address].Split(' ')[1][0] + file_mem[row_address].Split(' ')[1][1] + data[6] + data[7] + data[4] + data[5] + data[2] + data[3];
                    row_address++;
                    file_mem[row_address] = file_mem[row_address].Split(' ')[0] + " " + data[0] + data[1] + file_mem[row_address].Split(' ')[1][2] + file_mem[row_address].Split(' ')[1][3] + file_mem[row_address].Split(' ')[1][4] + file_mem[row_address].Split(' ')[1][5] + file_mem[row_address].Split(' ')[1][6] + file_mem[row_address].Split(' ')[1][7];
                }
            else
                if (offset == 2) //offset 2 byte
                {
                    file_mem[row_address] = file_mem[row_address].Split(' ')[0] + " " + file_mem[row_address].Split(' ')[1][0] + file_mem[row_address].Split(' ')[1][1] + file_mem[row_address].Split(' ')[1][2] + file_mem[row_address].Split(' ')[1][3] + data[6] + data[7] + data[4] + data[5];
                    row_address++;
                    file_mem[row_address] = file_mem[row_address].Split(' ')[0] + " " + data[2] + data[3] + data[0] + data[1] + file_mem[row_address].Split(' ')[1][4] + file_mem[row_address].Split(' ')[1][5] + file_mem[row_address].Split(' ')[1][6] + file_mem[row_address].Split(' ')[1][7];
                }

            else // offset 3 byte
                {
                file_mem[row_address] = file_mem[row_address].Split(' ')[0] + " " + file_mem[row_address].Split(' ')[1][0] + file_mem[row_address].Split(' ')[1][1] + file_mem[row_address].Split(' ')[1][2] + file_mem[row_address].Split(' ')[1][3] + file_mem[row_address].Split(' ')[1][5] + file_mem[row_address].Split(' ')[1][5] + data[6] + data[7];
                row_address++;
                file_mem[row_address] = file_mem[row_address].Split(' ')[0] + " " + data[4] + data[5] + data[2] + data[3] + data[0] + data[1] + file_mem[row_address].Split(' ')[1][6] + file_mem[row_address].Split(' ')[1][7];
                }
            StreamWriter streamWriter = new StreamWriter(Path_memory);
            foreach (string el in file_mem)
                streamWriter.WriteLine(el);
            streamWriter.Close();
        }

        // store to stack
        void Push_to_stack(string data, bool type) //type = true (word), = false (byte)
        {
            if (type)
            {

            }
            else 
            { 

            }
        }

        //create register file
        void Set_reg()
        {
            byte[] info;
            using FileStream fs = File.Create(Path_register);
            for (int i = 0; i < 31; i++ )
            {
                if(i == 2)
                    info = new UTF8Encoding(true).GetBytes("x" + i + " " + Begin_data_mem.Substring(2).PadLeft(8,'0') + "\n");
                else
                    if(i == 3)
                    info = new UTF8Encoding(true).GetBytes("x" + i + " " + Stack_pointer.Substring(2).PadLeft(8, '0') + "\n");
                else
                        info = new UTF8Encoding(true).GetBytes("x" + i + " 00000000\n"); 

                fs.Write(info, 0, info.Length);
            }
        }

        // clear memory
        void Clear_mem()
        {
            using FileStream fs = File.Create(Path_memory);
            for (int i = 0; i < 65535; i += 4)
            {
                byte[] info = new UTF8Encoding(true).GetBytes("0x" + Convert.ToString(i, 16).PadLeft(4, '0').ToUpper() + " 00000000\n"); // memory 0xFFFF
                //byte[] info = new UTF8Encoding(true).GetBytes("00 00 00 00\n"); // memory 0xFFFF
                fs.Write(info, 0, info.Length);
            }
        }
    }
}
