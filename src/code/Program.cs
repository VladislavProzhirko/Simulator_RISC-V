using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;

namespace Simulator_RISCV
{
    class Program
    {

        static public StreamReader Reader { get; set; }
        public static string Pointer { get; set; }
        public static string PC { get; set; } = "00000000";

        static void Main(string[] args)
        {
            //create memory
            Process pr = Process.Start(@"toolchain\create_hex.bat", "lab_riscv_asm1");
            while (!pr.HasExited) { }
            //using (StreamWriter sw = File.AppendText("memory.hex"))
            ////add stack
            //{
            //    //sw.WriteLine("@0000F100");
            //    for (int i = 0; i < 4; i++)
            //    {
            //        for (int j = 0; j < 16; j++)
            //        {
            //            sw.Write("00 ");
            //        }
            //        sw.WriteLine("");
            //    }
            //}
            //File.Create("new_memory1.hex");
            //File.Copy("new_memory.hex", "new_memory1.hex");
            Reader = new StreamReader("memory.hex");
            Reader.ReadLine(); //skip @00000000
            Alg_operation prov = new Alg_operation();
            Decoder decoder = new Decoder();
            string Full, Asm;
            bool step = true;
            string instruction = Read_code();
            while (true)
            {
                Console.WriteLine(PC);
                decoder.DecodeInstruction(instruction, out Full, out Asm);
                Console.WriteLine(instruction + "  " + Asm);
                if (!prov.Comand_Real(Full))
                    break;
                instruction = Read_code();
                if(step)
                while (true)
                {
                    Console.WriteLine("Для того чтобы выполнить следующий шаг нажмите 0, для того чтобы выполнить программу нажмите 1");
                    string mode = Console.ReadLine();
                    if (mode == "0")
                    {
                        step = true;
                        break;
                    }
                    else
                    {
                        step = false;
                        break;
                    }

                }
            }
            Console.WriteLine("Нажмите любую клавишу чтобы завершить программу");
            Console.ReadKey();
            //decoder.DecodeInstruction(instruction, out Full, out Asm);


            //prov.Comand_Real("srai x3 x2 2");//beq x11, x12, 0xFFFFE, lui x1, 0xBEFCA, bgeu x11, x12, 0x000000fc, bltu x11, x12, 0x000000fc, lh x5 x6 0

        }

        static string Read_code()
        {

            char[] buf = new char[12];
            // if PC.ToUpper() found
            if (PC.ToUpper() == Pointer)
            {
                if (Convert.ToInt32(Pointer, 16) % 16 == 0 && Pointer != "00000000")
                    Reader.ReadLine();
                Reader.Read(buf, 0, 12); // read 4 bytes
                Pointer = (Convert.ToInt32(Pointer, 16) + 4).ToString("X").PadLeft(8, '0'); // Pointer next (+4)
                return new string(buf);
            }
            else
            {
                //if PC.ToUpper() = {0 to 0x1000}
                if (String.Compare(PC.ToUpper(), "00000000") >= 0 && String.Compare(PC.ToUpper(), "000001000") < 0)
                {
                    // if Pointer < PC.ToUpper()
                    if (String.Compare(PC.ToUpper(), Pointer) < 0 || String.Compare(Pointer, "00001000") > 0)
                    {
                        Reader.BaseStream.Position = 0;
                        Reader.DiscardBufferedData(); // clear buf_read_file
                        Reader.ReadLine();
                        Pointer = "00000000";

                        if (PC.ToUpper() == Pointer)
                        {
                            Reader.Read(buf, 0, 12);
                            Pointer = (Convert.ToInt32(Pointer, 16) + 4).ToString("X").PadLeft(8, '0');
                            return new string(buf);
                        }
                    }
                    int row = ((Convert.ToInt32(PC.ToUpper(), 16) - Convert.ToInt32(Pointer, 16)) / 16);// row to PC.ToUpper() from 0x0000
                    int i = 0;
                    if(row > 0)
                    Pointer = PC.ToUpper().Substring(0, 7) + "0";
                    while (i < row)
                    {
                        if (Reader.ReadLine() == "")
                            Reader.ReadLine();
                        i++;
                    }
                    if (Convert.ToInt32(Pointer, 16) % 16 == 0 && row == 0)
                        Reader.ReadLine();

                    //find PC.ToUpper()
                    while (true)
                        if (PC.ToUpper() != Pointer)
                        {
                            if (Convert.ToInt32(Pointer, 16) % 16 == 0 && Pointer != "00000000" && row == 0)
                                Reader.ReadLine();
                            Reader.Read(buf, 0, 12);
                            Pointer = (Convert.ToInt32(Pointer, 16) + 4).ToString("X").PadLeft(8, '0');
                            // if end line (\r\n)
                            
                        }
                        else
                        {
                            if (Convert.ToInt32(Pointer, 16) % 16 == 0 && Pointer != "00000000" && row == 0)
                                Reader.ReadLine();
                            Reader.Read(buf, 0, 12);
                            Pointer = (Convert.ToInt32(Pointer, 16) + 4).ToString("X").PadLeft(8, '0');
                            // if end line (\r\n)
                            return new string(buf);
                        }
                }
                //if not correct PC.ToUpper()
                else
                {
                    Console.WriteLine("PC.ToUpper() not correct");
                    return "errror";
                }
            }

        }
    }
}
