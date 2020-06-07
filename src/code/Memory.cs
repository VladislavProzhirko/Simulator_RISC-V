using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace Simulator_RISCV 
{
    class Memory
    {

        StreamReader Reg_reader { get; set; }
        public string Begin_stack_mem { get; } = "00007FF0";
        public string Begin_data_mem { get; } = "00001000";
        public string Code_pointer { get; } = "00000000";
        public int X0 { get; } = 0;

        public Memory()
        {
            //MainWindow.Reader = new StreamReader(File.Open(Path_memory, FileMode.Open));
            //Reader.ReadLine(); //skip @00000000
            //Data_memory = Code_pointer;
        }
        public string Read_data_byte(string address)
        {
            string row = address.Substring(2, 7) + "0";
            int offset = Convert.ToInt32(address, 16) % 16;
            string[] buf = Alg_operation.Data_seg[row].Split(' ');
            return buf[offset];
        }

        //funct load word to memory
        public string Read_data_hw(string address)
        {
            string row = address.Substring(2, 7) + "0";
            int offset = Convert.ToInt32(address, 16) % 16;
            string[] buf = Alg_operation.Data_seg[row].Split(' ');
            if (offset != 15)
                return buf[offset + 1] + buf[offset];
            else
                return Alg_operation.Data_seg[(Convert.ToInt32(row,16) + 16).ToString("X").PadLeft(8, '0')].Split(' ')[0] + buf[offset];
        }   

        //funct load word to memory
        public string Read_data_word(string address)
        {
            string row = address.Substring(2, 7) + "0";
            int offset = Convert.ToInt32(address, 16) % 16;
            string[] buf = Alg_operation.Data_seg[row.ToLower()].Split(' ');

            switch (offset)
            {
                case 13:
                    return Alg_operation.Data_seg[(Convert.ToInt32(row, 16) + 16).ToString("X").PadLeft(8, '0')].Split(' ')[0] + buf[offset + 2] + buf[offset + 1] + buf[offset];
                case 14:
                    return Alg_operation.Data_seg[(Convert.ToInt32(row, 16) + 16).ToString("X").PadLeft(8, '0')].Split(' ')[1] + Alg_operation.Data_seg[(Convert.ToInt32(row, 16) + 16).ToString("X").PadLeft(8, '0')].Split(' ')[0] + buf[offset + 1] + buf[offset];
                case 15:
                    return Alg_operation.Data_seg[(Convert.ToInt32(row, 16) + 16).ToString("X").PadLeft(8, '0')].Split(' ')[2] + Alg_operation.Data_seg[(Convert.ToInt32(row, 16) + 16).ToString("X").PadLeft(8, '0')].Split(' ')[1] + Alg_operation.Data_seg[(Convert.ToInt32(row, 16) + 16).ToString("X").PadLeft(8, '0')].Split(' ')[0] + buf[offset];
                default:
                    return buf[offset + 3] + buf[offset + 2] + buf[offset + 1] + buf[offset];
                    
            }
        }
        //funct load to memory
        //string Read_data(string address, int type)
        //{
            
        //    char[] buf = new char[12];
        //    int i;
        //    address = address.ToUpper();
        //    //if address = {0x1000 to 0xf100}
        //    if (address == Data_memory)
        //    {
        //        if (type == 1) // byte
        //        {
                    
        //            //MainWindow.Reader.Read(buf, 0, 3);
        //            //Data_memory = (Convert.ToInt32(Data_memory, 16) + 1).ToString("X").PadLeft(8, '0');
        //            //if (Convert.ToInt32(Data_memory, 16) % 16 == 0)
        //            //    MainWindow.Reader.ReadLine();
        //            return new string(buf).Substring(0, 2);
        //        }
        //        else
        //        if (type == 2) // halfword
        //        {
        //            for (int j = 0; j < 2; j++)
        //            {
        //                MainWindow.Reader.Read(buf, 3 - j * 3, 3);
        //                Data_memory = (Convert.ToInt32(Data_memory, 16) + 1).ToString("X").PadLeft(8, '0');
        //                if (Convert.ToInt32(Data_memory, 16) % 16 == 0)
        //                    MainWindow.Reader.ReadLine();
        //            }
        //            return new string(buf).Replace(" ", "");
        //        }
        //        else // word
        //        {
        //            for (int j = 0; j < 4; j++)
        //            {
        //                MainWindow.Reader.Read(buf, 12 - j * 3, 3);
        //                Data_memory = (Convert.ToInt32(Data_memory, 16) + 1).ToString("X").PadLeft(8, '0');
        //                if (Convert.ToInt32(Data_memory, 16) % 16 == 0)
        //                    MainWindow.Reader.ReadLine();
        //            }
        //            return new string(buf).Replace(" ", "");
        //        }
        //    }
        //    else
        //    if (String.Compare(address, Begin_data_mem) >= 0)
        //    {
        //        if (String.Compare(address, Data_memory) < 0  || String.Compare(Data_memory,Begin_data_mem)<0)
        //        {
        //            MainWindow.Reader.BaseStream.Position = 0;
        //            MainWindow.Reader.DiscardBufferedData(); // clear buf_read_file
        //            Data_memory = Begin_data_mem;
        //            i = 1;
        //            //skip to address line
        //            while (i < 259)
        //            {
        //                MainWindow.Reader.ReadLine();
        //                i++;
        //            }
        //            Data_memory = address.Remove(7) + "0";

        //            // if address = 0x1000
        //            if (address == Data_memory)
        //            {
        //                if (type == 1)
        //                {
        //                    MainWindow.Reader.Read(buf, 0, 3);
        //                    Data_memory = (Convert.ToInt32(Data_memory, 16) + 1).ToString("X").PadLeft(8, '0');
        //                    if (Convert.ToInt32(Data_memory, 16) % 16 == 0)
        //                        MainWindow.Reader.ReadLine();
        //                    return new string(buf).Substring(0, 2);
        //                }
        //                else //halfword
        //                if (type == 2)
        //                {
        //                    for (int j = 0; j < 2; j++)
        //                    {
        //                        MainWindow.Reader.Read(buf, 3 - j * 3, 3);
        //                        Data_memory = (Convert.ToInt32(Data_memory, 16) + 1).ToString("X").PadLeft(8, '0');
        //                        if (Convert.ToInt32(Data_memory, 16) % 16 == 0)
        //                            MainWindow.Reader.ReadLine();
        //                    }
        //                    return new string(buf).Replace(" ", "");
        //                }
        //                else//word
        //                {
        //                    for (int j = 0; j < 4; j++)
        //                    {
        //                        MainWindow.Reader.Read(buf, 9 - j * 3, 3);
        //                        Data_memory = (Convert.ToInt32(Data_memory, 16) + 1).ToString("X").PadLeft(8, '0');
        //                        if (Convert.ToInt32(Data_memory, 16) % 16 == 0)
        //                            MainWindow.Reader.ReadLine();
        //                    }
        //                    return new string(buf).Replace(" ", "");
        //                }
        //            }
        //        }
        //    }

        //        int row = ((Convert.ToInt32(address, 16) - Convert.ToInt32(Data_memory, 16)) / 16);// row to address from 0x1000
        //        i = 0;
        //        if(row > 0)
        //        Data_memory = address.Remove(7) + "0";
        //        while (i < row)
        //        {
        //            MainWindow.Reader.ReadLine();
        //            i++;
        //        }
                
        //        int offset_words = Convert.ToInt32(address.Substring(7,1), 16) / 4; // skip words to address
        //        i = 0;
        //        while (i < offset_words)
        //        {
        //            MainWindow.Reader.Read(buf, 0 , 12);
        //            Data_memory = (Convert.ToInt32(Data_memory, 16) + 4).ToString("X").PadLeft(8, '0');
        //            if (Convert.ToInt32(Data_memory, 16) % 16 == 0)
        //                MainWindow.Reader.ReadLine();
        //            i++;
        //        }
        //    if (address.ToUpper() != Data_memory)
        //    //find address
        //    {
        //        while (true)
        //            if (address.ToUpper() != Data_memory)
        //            {
        //                MainWindow.Reader.Read(buf, 0, 3);
        //                Data_memory = (Convert.ToInt32(Data_memory, 16) + 1).ToString("X").PadLeft(8, '0');
        //                // if end line (\r\n)
        //                if (Convert.ToInt32(Data_memory, 16) % 16 == 0)
        //                    MainWindow.Reader.ReadLine();
        //            }
        //            else break;
        //    }
          
        //        if (type == 1) // byte
        //        {
        //            MainWindow.Reader.Read(buf, 0, 3);
        //            Data_memory = (Convert.ToInt32(Data_memory, 16) + 1).ToString("X").PadLeft(8, '0');
        //            if (Convert.ToInt32(Data_memory, 16) % 16 == 0)
        //                MainWindow.Reader.ReadLine();
        //            return new string(buf).Substring(0, 2);
        //        }
        //    if (type == 2)
        //    {
        //        for (int j = 0; j < 2; j++)
        //        {
        //            MainWindow.Reader.Read(buf, 3 - j * 3, 3);
        //            Data_memory = (Convert.ToInt32(Data_memory, 16) + 1).ToString("X").PadLeft(8, '0');
        //            if (Convert.ToInt32(Data_memory, 16) % 16 == 0)
        //                MainWindow.Reader.ReadLine();
        //        }
        //        return new string(buf).Replace(" ", "");
        //    }
        //    else//word
        //    {
        //        for (int j = 0; j < 4; j++)
        //        {
        //            MainWindow.Reader.Read(buf, 9 - j * 3, 3);
        //            Data_memory = (Convert.ToInt32(Data_memory, 16) + 1).ToString("X").PadLeft(8, '0');
        //            if (Convert.ToInt32(Data_memory, 16) % 16 == 0)
        //                MainWindow.Reader.ReadLine();
        //        }
        //        return new string(buf).Replace(" ", "");
        //    }

        //}

        //funct store to memory
        public void Write_data(string address, string data)
        {
            string row = address.Substring(0, 7) + "0";
            int offset = Convert.ToInt32(address, 16) % 16;
            string[] buf = Alg_operation.Data_seg[row].Split(' ');


                switch (data.Length)
                {
                    case 2:
                        {
                            buf[offset] = data;
                            Alg_operation.Data_seg[row] = String.Join(" ", buf);
                            break;
                        }
                    case 4:
                        {
                            if (offset != 15)
                            {
                                buf[offset + 1] = data.Substring(2, 2);
                                buf[offset] = data.Substring(0, 2);
                                Alg_operation.Data_seg[row] = String.Join(" ", buf);
                            }
                            else
                            {
                                buf[offset] = data.Substring(2, 2);
                                Alg_operation.Data_seg[row] = String.Join(" ", buf);
                                string[] buf2 = Alg_operation.Data_seg[(Convert.ToInt32(row, 16) + 16).ToString("X").PadLeft(8, '0')].Split(' ');
                                buf2[0] = data.Substring(0, 2);
                                Alg_operation.Data_seg[(Convert.ToInt32(row, 16) + 16).ToString("X").PadLeft(8, '0')] = String.Join(" ", buf2);
                            }
                            break;
                        }
                    case 8:
                        {
                            switch (offset)
                            {
                                case 13:
                                    {
                                    buf[offset + 2] = data.Substring(2, 2);
                                    buf[offset + 1] = data.Substring(4, 2);
                                    buf[offset] = data.Substring(6, 2);
                                    Alg_operation.Data_seg[row] = String.Join(" ", buf);
                                    string[] buf2 = Alg_operation.Data_seg[(Convert.ToInt32(row, 16) + 16).ToString("X").PadLeft(8, '0')].Split(' ');
                                    buf2[0] = data.Substring(0, 2);
                                    Alg_operation.Data_seg[(Convert.ToInt32(row, 16) + 16).ToString("X").PadLeft(8, '0')] = String.Join(" ", buf2);
                                    break;
                                    }
                                case 14:
                                    {
                                        buf[offset + 1] = data.Substring(4, 2);
                                        buf[offset] = data.Substring(6, 2);
                                        Alg_operation.Data_seg[row] = String.Join(" ", buf);
                                        string[] buf2 = Alg_operation.Data_seg[(Convert.ToInt32(row, 16) + 16).ToString("X").PadLeft(8, '0')].Split(' ');
                                        buf2[0] = data.Substring(2, 2);
                                        buf2[1] = data.Substring(0, 2);
                                        Alg_operation.Data_seg[(Convert.ToInt32(row, 16) + 16).ToString("X").PadLeft(8, '0')] = String.Join(" ", buf2);
                                        break;
                                    }
                                case 15:
                                    {
                                        buf[offset] = data.Substring(6, 2);
                                         Alg_operation.Data_seg[row] = String.Join(" ", buf);
                                        string[] buf2 = Alg_operation.Data_seg[(Convert.ToInt32(row, 16) + 16).ToString("X").PadLeft(8, '0')].Split(' ');
                                        buf2[0] = data.Substring(4, 2);
                                        buf2[1] = data.Substring(2, 2);
                                        buf2[2] = data.Substring(0, 2);
                                        Alg_operation.Data_seg[(Convert.ToInt32(row, 16) + 16).ToString("X").PadLeft(8, '0')] = String.Join(" ", buf2);
                                        break;
                                    }
                                default:
                                    {
                                        buf[offset + 3] = data.Substring(0, 2);
                                        buf[offset + 2] = data.Substring(2, 2);
                                        buf[offset + 1] = data.Substring(4, 2);
                                        buf[offset] = data.Substring(6, 2);
                                        Alg_operation.Data_seg[row] = String.Join(" ", buf);
                                        break;
                                    }
                            }
                            break;
                        }
                }
        }

        //public string Read_reg_byte(string reg)
        //{

        //    return Read_reg(reg, 1);
        //}

        //funct load word to reg
        //public string Read_reg_hw(string reg)
        //{

        //    return Read_reg(reg, 2);
        //}

        //funct load word to reg
        //public string Read_reg_word(string reg)
        //{
        //    return Registers[reg];
        //    //return Read_reg(reg, 4);
        //}

        //funct load to reg
        //string Read_reg(string register, int type)
        //{
        //    Registers.TryGetValue(register,out string rez);

        //    using (Reg_reader = new StreamReader(File.Open(Path_register, FileMode.Open)))
        //    {
        //        int number = Convert.ToInt32(register.TrimStart('x'));
        //        int i = 0;
        //        while (i < number)
        //        {
        //            Reg_reader.ReadLine();
        //            i++;
        //        }
        //        string buf = Reg_reader.ReadLine();
        //        Reg_reader.Close();
        //        if (type == 1)
        //            return buf.Substring(6, 2);
        //        else
        //            if (type == 2)
        //            return buf.Substring(4, 4);
        //        else
        //            return buf;
        //    }
        //}

        //public void Write_reg(string register, string data)
        //{
        //    Registers[register] = "0x" + data;
        //}

        //create register file

    }
}
