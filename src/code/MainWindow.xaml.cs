using System;
using System.Windows;
using System.Diagnostics;
using System.IO;
using Microsoft.Win32;
using System.Collections.Generic;

namespace Simulator_RISCV
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public Dictionary<string, string> Code_seg
        {
            get; set;
        }


        string Path_register { get; } = "register.hex";
        static public StreamReader Reader { get; set; }
        string Full, Asm;
        string instruction;
        Alg_operation Prov { get; set; }
        Decoder Decoder { get; set; }
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Initialized(object sender, EventArgs e)
        {
            run_btn.IsEnabled = step_btn.IsEnabled = reset_btn.IsEnabled = false;
            Prov = new Alg_operation();
            Decoder = new Decoder();
            Prov.Console = "";
            Single.IsChecked = Prov.Stage = true;
            Prov.CLK = 0;
            DataContext = Prov;
            Code_seg = new Dictionary<string, string>();
            Alg_operation.Data_seg = new Dictionary<string, string>();
            Set_default();
            Write_reg();
            grid_code.ItemsSource = Code_seg;
            grid_data.ItemsSource = Alg_operation.Data_seg;
            data_register.ItemsSource = Prov.Registers;
            orig.Text = "";
            mash.Text = "";

        }

        void Set_default()
        {
            int i = 0;
            while (i < 256)
            {
                Code_seg.Add(Convert.ToString(i*16,16).PadLeft(8,'0'), "00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ");
                i++;
            }
            while (i < 2048)
            {
                Alg_operation.Data_seg.Add(Convert.ToString(i * 16, 16).PadLeft(8, '0'), "00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ");
                i++;
            }
        }

        private void Step_btn_Click(object sender, RoutedEventArgs e)
        {
            Single.IsEnabled = Five.IsEnabled = false;
            instruction = Read_code();
            if (Prov.Stage)
                Prov.CLK += 4;
            if (instruction == "00 00 00 00 ")
            {
                
                Prov.PC = "00000000";
                run_btn.IsEnabled = false;
                step_btn.IsEnabled = false;
                Prov.Console = "Error PC:" + Prov.PC;
            }
            if (Decoder.DecodeInstruction(instruction, out Full, out Asm) == 1)
                Prov.Console = Asm + " " + instruction;
            else
            {
                Prov.PC = Prov.PC.ToUpper();
                orig.Text += Prov.PC + "\t" + instruction + " \n";
                mash.Text += Asm + " \n";
                if (!Prov.Comand_Real(Full))
                {
                    step_btn.IsEnabled = false;
                    run_btn.IsEnabled = false;
                }
            }
            grid_data.Items.Refresh();
            data_register.Items.Refresh();

        }

        private void Step_btn_Click_2(object sender, RoutedEventArgs e)
        {
            instruction = Read_code();
            if (instruction == "00 00 00 00 ")
            {

                Prov.PC = "00000000";
                run_btn.IsEnabled = false;
                step_btn.IsEnabled = false;
                Prov.Console = "Error PC:" + Prov.PC;
            }
            if (Decoder.DecodeInstruction(instruction, out Full, out Asm) == 1)
                Prov.Console = Asm + " " + instruction;
            else
            {
                Prov.PC = Prov.PC.ToUpper();
                orig.Text += Prov.PC + " " + instruction + " \n";
                mash.Text += Asm + " \n";
                if (!Prov.Comand_Real(Full))
                {
                    step_btn.IsEnabled = false;
                    run_btn.IsEnabled = false;
                }
            }
            grid_data.Items.Refresh();
            data_register.Items.Refresh();

        }

        private void Run_btn_Click(object sender, RoutedEventArgs e)
        {
            string instruction = Read_code();
            Single.IsEnabled = Five.IsEnabled = false;
            while (true)
            {
                if (Prov.Stage)
                    Prov.CLK += 4;
                if (Decoder.DecodeInstruction(instruction, out Full, out Asm) == 1)
                {
                    Prov.Console += Asm + " " + instruction;
                    run_btn.IsEnabled = false;
                    break;
                }
                else
                {
                    orig.Text += Prov.PC + "\t" + instruction + " \n";
                    mash.Text += Asm + " \n";
                    if (!Prov.Comand_Real(Full))
                        break;
                    Prov.PC = Prov.PC.ToUpper();
                    instruction = Read_code();
                    if (instruction == "")
                    {
                        run_btn.IsEnabled = false;
                        break;
                    }
                }
            }
            Single.IsEnabled = Five.IsEnabled = true;
            grid_data.Items.Refresh();
            data_register.Items.Refresh();
            run_btn.IsEnabled = false;
        }

        string Read_code()
        {
            if (String.Compare(Prov.PC, "00001000") < 0)
            {
                string row = Prov.PC.Substring(0,7) + "0";
                string[] buf = Code_seg[row.ToLower()].Split(' ');
                int offset = Convert.ToInt32(Prov.PC, 16) % 16;
                return buf[offset + 3] + buf[offset + 2] + buf[offset + 1] + buf[offset];
            }
            else return "";
        }
        //string Read_code()
        //{
        //    char[] buf = new char[12];
        //    // if PC.ToUpper() found
        //    if (PC.ToUpper() == Prov.Pointer)
        //    {
        //        if (Convert.ToInt32(Prov.Pointer, 16) % 16 == 0 && Prov.Pointer != "00000000")
        //            Reader.ReadLine();
        //        Reader.Read(buf, 0, 12); // read 4 bytes

        //        Prov.Pointer = (Convert.ToInt32(Prov.Pointer, 16) + 4).ToString("X").PadLeft(8, '0'); // Prov.Pointer next (+4)
        //        return new string(buf);
        //    }
        //    else
        //    {
        //        //if PC.ToUpper() = {0 to 0x1000}
        //        if (String.Compare(PC.ToUpper(), "00000000") >= 0 && String.Compare(PC.ToUpper(), "000001000") < 0)
        //        {
        //            // if Prov.Pointer < PC.ToUpper()
        //            if (String.Compare(PC.ToUpper(), Prov.Pointer) < 0 || String.Compare(Prov.Pointer, "00001000") > 0)
        //            {
        //                Reader.BaseStream.Position = 0;
        //                Reader.DiscardBufferedData(); // clear buf_read_file
        //                Reader.ReadLine();
        //                Prov.Pointer = "00000000";

        //                if (PC.ToUpper() == Prov.Pointer)
        //                {
        //                    Reader.Read(buf, 0, 12);
        //                    Prov.Pointer = (Convert.ToInt32(Prov.Pointer, 16) + 4).ToString("X").PadLeft(8, '0');
        //                    return new string(buf);
        //                }
        //            }
        //            int row = ((Convert.ToInt32(PC.ToUpper(), 16) - Convert.ToInt32(Prov.Pointer, 16)) / 16);// row to PC.ToUpper() from 0x0000
        //            int i = 0;
        //            if (row > 0)
        //                Prov.Pointer = PC.ToUpper().Substring(0, 7) + "0";
        //            while (i < row)
        //            {
        //                if (Reader.ReadLine() == "")
        //                    Reader.ReadLine();
        //                i++;
        //            }
        //            if (Convert.ToInt32(Prov.Pointer, 16) % 16 == 0 && row == 0)
        //                Reader.ReadLine();

        //            //find PC.ToUpper()
        //            while (true)
        //                if (PC.ToUpper() != Prov.Pointer)
        //                {
        //                    if (Convert.ToInt32(Prov.Pointer, 16) % 16 == 0 && Prov.Pointer != "00000000" && row == 0)
        //                        Reader.ReadLine();
        //                    Reader.Read(buf, 0, 12);
        //                    Prov.Pointer = (Convert.ToInt32(Prov.Pointer, 16) + 4).ToString("X").PadLeft(8, '0');
        //                    // if end line (\r\n)

        //                }
        //                else
        //                {
        //                    if (Convert.ToInt32(Prov.Pointer, 16) % 16 == 0 && Prov.Pointer != "00000000" && row == 0)
        //                        Reader.ReadLine();
        //                    Reader.Read(buf, 0, 12);
        //                    Prov.Pointer = (Convert.ToInt32(Prov.Pointer, 16) + 4).ToString("X").PadLeft(8, '0');
        //                    // if end line (\r\n)
        //                    return new string(buf);
        //                }
        //        }
        //        //if not correct PC.ToUpper()
        //        else
        //        {
        //            Console_window.Text = "PC not correct";
        //            return "";
        //        }
        //    }

        //}

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            string filePath;
            Prov.Reg_init();
            OpenFileDialog openFileDialog = new OpenFileDialog();

            //openFileDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            openFileDialog.FilterIndex = 2;
            openFileDialog.RestoreDirectory = true;


            if (openFileDialog.ShowDialog() == true)
            {
                filePath = openFileDialog.FileName;
                
                Process pr = Process.Start(@"toolchain\create_hex.bat", filePath.Substring(0, filePath.Length - 2));
                while (!pr.HasExited) { }
                try
                {
                    Reader = new StreamReader("memory.hex");
                }
                catch (Exception)
                {
                    Prov.Console = "File:" + openFileDialog.SafeFileName + " not corrected";
                    return;
                }
                        string buf;
                        Reader.ReadLine();
                        int i = 0;
                        while (true)
                        {
                            buf = Reader.ReadLine();
                            if (buf != Code_seg[Convert.ToString(i * 16, 16).PadLeft(8, '0')])
                            {
                                Code_seg[Convert.ToString(i * 16, 16).PadLeft(8, '0')] = buf;
                                i++;
                            }
                            else break;
                        }
                        i = 256;
                        grid_code.Items.Refresh();
                        while (true)
                        {
                            if (Reader.ReadLine()[0] == '@')
                                break;
                        }
                        while (true)
                        {
                            buf = Reader.ReadLine();
                            if (buf != Alg_operation.Data_seg[Convert.ToString(i * 16, 16).PadLeft(8, '0')])
                            {
                                Alg_operation.Data_seg[Convert.ToString(i * 16, 16).PadLeft(8, '0')] = buf;
                                i++;
                            }
                            else break;
                        }
                        orig.Text = "";
                        mash.Text = "";
                        Prov.Console = "";
                        Prov.PC = "00000000";
                        Prov.CLK = 0;
                        run_btn.IsEnabled = step_btn.IsEnabled = true;
                        grid_data.Items.Refresh();
                        Reader.Close();
             }

        }

        void Write_reg()
        {
            using (StreamWriter fs = new StreamWriter(Path_register))
            {
                foreach (var buf in Prov.Registers)
                {
                    fs.WriteLine(buf.Key + " \t" + buf.Value);
                }
            }
        }

        private void reset_btn_Click(object sender, RoutedEventArgs e)
        {
            Prov.PC = "00000000";
            run_btn.IsEnabled = true;
            orig.Text = "";
            mash.Text = "";
        }

        private void Single_Click(object sender, RoutedEventArgs e)
        {
            Prov.Stage = Single.IsChecked = true;
            Five.IsChecked = false;
        }

        private void Five_Click(object sender, RoutedEventArgs e)
        {
            Prov.Stage = Single.IsChecked = false;
            Five.IsChecked = true;
        }

        void Write_data()
        {
            
        }
    }
}

