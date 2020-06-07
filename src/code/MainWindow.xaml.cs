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
        static public Dictionary<string, string> Stage_conv
        {
            get; set;
        }
        public int wait;
        string PC { get; set; }
        string result_ALU;
        string result_MEM;
        string prev_result_ALU;
        string prev_result_MEM;
        string prev_Decode;
        static public StreamReader Reader { get; set; }
        string Full, Asm, Prev_Asm;
        string instruction;
        Alg_operation Prov { get; set; }
        Decoder Decoder { get; set; }
        Memory Mem { get; set; }
        public MainWindow()
        {
            Stage_conv = new Dictionary<string, string>
            {
                { "Fetch", "nop"},
                { "Decode", "nop"},
                { "Execute", "nop"},
                { "Memory", "nop"},
                { "Write back", "nop"},
                { "Wait", "0"}
            };
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
            grid_stage.ItemsSource = Stage_conv;
            Code_seg = new Dictionary<string, string>();
            Alg_operation.Data_seg = new Dictionary<string, string>();
            Set_default();
            Write_reg();
            grid_code.ItemsSource = Code_seg;
            grid_data.ItemsSource = Alg_operation.Data_seg;
            data_register.ItemsSource = Prov.Registers;
            instr.Text = "";
            Mem = new Memory();
        }

        void Set_default()
        {
            int i = 0;
            while (i < 256)
            {
                Code_seg.Add(Convert.ToString(i * 16, 16).PadLeft(8, '0'), "00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ");
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
            Prov.CLK += 4;
            Single.IsEnabled = Five.IsEnabled = false;
            reset_btn.IsEnabled = true;
            instruction = Read_code();
            
            Stage_conv["Fetch"] = instruction;
            if (instruction == "00000000")
            {
                run_btn.IsEnabled = false;
                step_btn.IsEnabled = false;
                Prov.Console = "Error PC:" + Prov.PC;
                grid_data.Items.Refresh();
                data_register.Items.Refresh();
                return;
            }
            if (Decoder.DecodeInstruction(instruction, out Full, out Asm) == 1)
            {
                Prov.Console = Asm + " " + instruction;
                run_btn.IsEnabled = false;
                step_btn.IsEnabled = false;
                Prov.Console = "Error PC:" + Prov.PC;
                grid_data.Items.Refresh();
                data_register.Items.Refresh();
                return;
            }
            else
            {
                Stage_conv["Write back"] = Stage_conv["Memory"] = Stage_conv["Execute"] = Stage_conv["Decode"] = Asm;
                Prov.PC = Prov.PC.ToUpper();
                instr.Text += Prov.PC + "\t" + instruction + "\t" + Asm + "\n";
                result_ALU = Prov.Comand_Real(Full);
                PC = Prov.PC;
                if (result_ALU == "ecall exit")
                {
                    step_btn.IsEnabled = run_btn.IsEnabled = false;
                    reset_btn.IsEnabled = Single.IsEnabled = Five.IsEnabled = true;
                    grid_stage.Items.Refresh();
                    grid_data.Items.Refresh();
                    data_register.Items.Refresh();
                    return;
                }
                if (result_ALU != "")
                {
                    result_MEM = Load_store(result_ALU, Stage_conv["Memory"]);
                }
                //stage WB
                if (result_MEM != "")
                    Prov.Registers[result_MEM.Split(' ')[1]][1] = result_MEM.Split(' ')[0];
            }
            grid_stage.Items.Refresh();
            grid_data.Items.Refresh();
            data_register.Items.Refresh();
        }

        private void Run_btn_Click(object sender, RoutedEventArgs e)
        {
            while (true)
            {
                Prov.CLK += 4;
                Single.IsEnabled = Five.IsEnabled = false;
                reset_btn.IsEnabled = true;
                instruction = Read_code();
                
                Stage_conv["Fetch"] = instruction;
                if (instruction == "00000000")
                {
                    run_btn.IsEnabled = false;
                    step_btn.IsEnabled = false;
                    Prov.Console = "Error PC:" + Prov.PC;
                    grid_data.Items.Refresh();
                    data_register.Items.Refresh();
                    return;
                }
                if (Decoder.DecodeInstruction(instruction, out Full, out Asm) == 1)
                {
                    Prov.Console = Asm + " " + instruction;
                    run_btn.IsEnabled = false;
                    step_btn.IsEnabled = false;
                    Prov.Console = "Error PC:" + Prov.PC;
                    grid_data.Items.Refresh();
                    data_register.Items.Refresh();
                    return;
                }
                else
                {
                    Stage_conv["Write back"] = Stage_conv["Memory"] = Stage_conv["Execute"] = Stage_conv["Decode"] = Asm;
                    Prov.PC = Prov.PC.ToUpper();
                    instr.Text += Prov.PC + "\t" + instruction + "\t" + Asm + "\n";
                    result_ALU = Prov.Comand_Real(Full);
                    PC = Prov.PC;
                    if (result_ALU == "ecall exit")
                    {
                        step_btn.IsEnabled = run_btn.IsEnabled = false;
                        reset_btn.IsEnabled = Single.IsEnabled = Five.IsEnabled = true;
                        grid_stage.Items.Refresh();
                        grid_data.Items.Refresh();
                        data_register.Items.Refresh();
                        return;
                    }
                    if (result_ALU != "")
                    {
                        result_MEM = Load_store(result_ALU, Stage_conv["Memory"]);
                    }
                    //stage WB
                    if (result_MEM != "")
                        Prov.Registers[result_MEM.Split(' ')[1]][1] = result_MEM.Split(' ')[0];
                    
                }
                grid_stage.Items.Refresh();
                grid_data.Items.Refresh();
                data_register.Items.Refresh();
            }
        }

        private void Step_btn_Click2(object sender, RoutedEventArgs e)
        {
            Prov.CLK++;
            Single.IsEnabled = Five.IsEnabled = false;
            reset_btn.IsEnabled = true;
            if (wait != 0)
            {
                if (wait == 1)
                {
                    Prov.PC = Prov.PC.ToUpper();
                    instr.Text += Prov.PC + "\t" + prev_Decode + "\t" + Prev_Asm + "\n";
                    result_ALU = Prov.Comand_Real(Stage_conv["Execute"]);
                    if (result_ALU == "ecall exit")
                    {
                        step_btn.IsEnabled = run_btn.IsEnabled = false;
                        reset_btn.IsEnabled = Single.IsEnabled = Five.IsEnabled = true;
                        grid_stage.Items.Refresh();
                        grid_data.Items.Refresh();
                        data_register.Items.Refresh();
                        return;
                    }
                    if (Stage_conv["Execute"][0] == 'B' || Stage_conv["Execute"][0] == 'J')
                    {
                        Stage_conv["Fetch"] = Stage_conv["Decode"] = "nop";
                        PC = Prov.PC;
                    }
                }
                else // wait 2 steps
                {
                    Stage_conv["Write back"] = Stage_conv["Memory"];
                    Stage_conv["Memory"] = "nop";
                    Prov.Registers[result_MEM.Split(' ')[1]][1] = result_MEM.Split(' ')[0];
                    result_MEM = "";
                    
                }
                wait--;
                Stage_conv["Wait"] = wait.ToString();
                grid_stage.Items.Refresh();
                grid_data.Items.Refresh();
                data_register.Items.Refresh();
            }
            else
            {
                //добавить проверку на конец кода
                instruction = Read_code();
                if (instruction == "00000000")
                    instruction = "nop";
                Stage_conv["Write back"] = Stage_conv["Memory"];
                Stage_conv["Memory"] = Stage_conv["Execute"];
                Stage_conv["Execute"] = Stage_conv["Decode"];
                Stage_conv["Decode"] = Stage_conv["Fetch"];
                Stage_conv["Fetch"] = instruction;
                if (Stage_conv["Decode"] != "nop")
                {
                    Prev_Asm = Asm;
                    prev_Decode = Stage_conv["Decode"];
                    Decoder.DecodeInstruction(Stage_conv["Decode"], out Full, out Asm);
                    Stage_conv["Decode"] = Full;
                }
                prev_result_ALU = result_ALU;
                PC = Convert.ToString(Convert.ToInt32(PC, 16) + 4, 16).PadLeft(8, '0');
                if (Stage_conv["Execute"] != "nop")
                {
                    string[] buf1 = Stage_conv["Execute"].Split(' ');
                    if (buf1[0].ToUpper() == "ECALL")
                    {
                        Array.Resize(ref buf1, 3);
                        buf1[1] = "x10";
                        buf1[2] = "x11";
                    }
                    if (prev_result_ALU != "" && prev_result_ALU.Split(' ')[1][0] == 'x')
                    {
                        int i = 2;
                        if (buf1[0] == "SB" || buf1[0] == "SH" || buf1[0] == "SW" || buf1[0][0] == 'B' || buf1[0][0] == 'E')
                            i--;
                        for (; i < buf1.Length; i++)
                        {
                            if (buf1[i][0] == 'x')
                                if (buf1[i] == prev_result_ALU.Split(' ')[1])
                                {
                                    wait = 2;
                                    Stage_conv["Wait"] = wait.ToString();
                                    break;
                                }
                        }
                    }
                    if (wait != 2)
                        if (prev_result_MEM != "" && prev_result_MEM.Split(' ')[1][0] == 'x')
                        {
                            int i = 2;
                            if (buf1[0] == "SB" || buf1[0] == "SH" || buf1[0] == "SW" || buf1[0][0] == 'B' || buf1[0][0] == 'E')
                                i--;
                            for (; i < buf1.Length; i++)
                            {
                                if (buf1[i][0] == 'x')
                                    if (buf1[i] == prev_result_MEM.Split(' ')[1])
                                    {
                                        wait = 1;
                                        Stage_conv["Wait"] = wait.ToString();
                                        break;
                                    }
                            }
                        }
                    if (wait == 0)
                    {
                        Prov.PC = Prov.PC.ToUpper();
                        instr.Text += Prov.PC + "\t" + prev_Decode + "\t" + Prev_Asm + "\n";
                        result_ALU = Prov.Comand_Real(Stage_conv["Execute"]);
                        if (result_ALU == "ecall exit")
                        {
                            step_btn.IsEnabled = run_btn.IsEnabled = false;
                            reset_btn.IsEnabled = Single.IsEnabled = Five.IsEnabled = true;
                            grid_stage.Items.Refresh();
                            grid_data.Items.Refresh();
                            data_register.Items.Refresh();
                            return;
                        }
                        if (Stage_conv["Execute"][0] == 'B' || Stage_conv["Execute"][0] == 'J')
                        {
                            Stage_conv["Fetch"] = Stage_conv["Decode"] = "nop";
                            PC = Prov.PC;
                        }

                    }

                }
                //stage memory
                prev_result_MEM = result_MEM;
                if (Stage_conv["Memory"] != "nop" && prev_result_ALU != "")
                {
                    result_MEM = Load_store(prev_result_ALU, Stage_conv["Memory"]);
                }
                //stage WB
                if (Stage_conv["Write back"] != "nop" && prev_result_MEM != "")
                    Prov.Registers[prev_result_MEM.Split(' ')[1]][1] = prev_result_MEM.Split(' ')[0];

                grid_stage.Items.Refresh();
                grid_data.Items.Refresh();
                data_register.Items.Refresh();
            }
        }

        private void Run_btn_Click2(object sender, RoutedEventArgs e)
        {
            Single.IsEnabled = Five.IsEnabled = false;
            while (true)
            {
                Prov.CLK++;
                if (wait != 0)
                {
                    if (wait == 1)
                    {
                        Prov.PC = Prov.PC.ToUpper();
                        instr.Text += Prov.PC + "\t" + prev_Decode + "\t" + Prev_Asm + "\n";
                        result_ALU = Prov.Comand_Real(Stage_conv["Execute"]);
                        if (result_ALU == "ecall exit")
                        {
                            reset_btn.IsEnabled = Single.IsEnabled = Five.IsEnabled = true;
                            step_btn.IsEnabled = run_btn.IsEnabled = false;
                            grid_stage.Items.Refresh();
                            grid_data.Items.Refresh();
                            data_register.Items.Refresh();
                            return;
                        }
                        if (Stage_conv["Execute"][0] == 'B' || Stage_conv["Execute"][0] == 'J')
                        {
                            Stage_conv["Fetch"] = Stage_conv["Decode"] = "nop";
                            PC = Prov.PC;
                        }
                    }
                    else // wait 2 steps
                    {
                        Stage_conv["Write back"] = Stage_conv["Memory"];
                        Stage_conv["Memory"] = "nop";
                        Prov.Registers[result_MEM.Split(' ')[1]][1] = result_MEM.Split(' ')[0];
                        result_MEM = "";
                    }
                    Stage_conv["Wait"] = wait.ToString();
                    wait--;
                }
                else
                {
                    //добавить проверку на конец кода
                    instruction = Read_code();
                    if (instruction == "00000000")
                        instruction = "nop";
                    Stage_conv["Write back"] = Stage_conv["Memory"];
                    Stage_conv["Memory"] = Stage_conv["Execute"];
                    Stage_conv["Execute"] = Stage_conv["Decode"];
                    Stage_conv["Decode"] = Stage_conv["Fetch"];
                    Stage_conv["Fetch"] = instruction;
                    if (Stage_conv["Decode"] != "nop")
                    {
                        Prev_Asm = Asm;
                        prev_Decode = Stage_conv["Decode"];
                        Decoder.DecodeInstruction(Stage_conv["Decode"], out Full, out Asm);
                        Stage_conv["Decode"] = Full;
                    }
                    prev_result_ALU = result_ALU;
                    PC = Convert.ToString(Convert.ToInt32(PC, 16) + 4, 16).PadLeft(8, '0');
                    if (Stage_conv["Execute"] != "nop")
                    {
                        string[] buf1 = Stage_conv["Execute"].Split(' ');
                        if (buf1[0].ToUpper() == "ECALL")
                        {
                            Array.Resize(ref buf1, 3);
                            buf1[1] = "x10";
                            buf1[2] = "x11";
                        }
                        if (prev_result_ALU != "" && prev_result_ALU.Split(' ')[1][0] == 'x')
                        {
                            int i = 2;
                            if (buf1[0] == "SB" || buf1[0] == "SH" || buf1[0] == "SW" || buf1[0][0] == 'B' || buf1[0][0] == 'E')
                                i--;
                            for (; i < buf1.Length; i++)
                            {
                                if (buf1[i][0] == 'x')
                                    if (buf1[i] == prev_result_ALU.Split(' ')[1])
                                    {
                                        wait = 2;
                                        Stage_conv["Wait"] = wait.ToString();
                                        break;
                                    }
                            }
                        }
                        if (wait != 2)
                            if (prev_result_MEM != "" && prev_result_MEM.Split(' ')[1][0] == 'x')
                            {
                                int i = 2;
                                if (buf1[0] == "SB" || buf1[0] == "SH" || buf1[0] == "SW" || buf1[0][0] == 'B' || buf1[0][0] == 'E')
                                    i--;
                                for (; i < buf1.Length; i++)
                                {
                                    if (buf1[i][0] == 'x')
                                        if (buf1[i] == prev_result_MEM.Split(' ')[1])
                                        {
                                            Stage_conv["Wait"] = wait.ToString();
                                            wait = 1;
                                            break;
                                        }
                                }
                            }
                        if (wait == 0)
                        {
                            Prov.PC = Prov.PC.ToUpper();
                            instr.Text += Prov.PC + "\t" + prev_Decode + "\t" + Prev_Asm + "\n";
                            result_ALU = Prov.Comand_Real(Stage_conv["Execute"]);
                            if (result_ALU == "ecall exit")
                            {
                                step_btn.IsEnabled = run_btn.IsEnabled = false;
                                reset_btn.IsEnabled = Single.IsEnabled = Five.IsEnabled = true;
                                grid_stage.Items.Refresh();
                                grid_data.Items.Refresh();
                                data_register.Items.Refresh();
                                return;
                            }
                            if (Stage_conv["Execute"][0] == 'B' || Stage_conv["Execute"][0] == 'J')
                            {
                                Stage_conv["Fetch"] = Stage_conv["Decode"] = "nop";
                                PC = Prov.PC;
                            }

                        }

                    }
                    //stage memory
                    prev_result_MEM = result_MEM;
                    if (Stage_conv["Memory"] != "nop" && prev_result_ALU != "")
                        result_MEM = Load_store(prev_result_ALU, Stage_conv["Memory"]);
                    //stage WB
                    if (Stage_conv["Write back"] != "nop" && prev_result_MEM != "")
                        Prov.Registers[prev_result_MEM.Split(' ')[1]][1] = prev_result_MEM.Split(' ')[0];
                }
            }
        }


        string Read_code()
        {
            if (String.Compare(PC, "00001000") < 0)
            {
                string row = PC.Substring(0, 7) + "0";
                string[] buf = Code_seg[row.ToLower()].Split(' ');
                int offset = Convert.ToInt32(PC, 16) % 16;
                return buf[offset + 3] + buf[offset + 2] + buf[offset + 1] + buf[offset];
            }
            else return "";
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            string filePath;
            Prov.Reg_init();
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.FilterIndex = 2;
            openFileDialog.RestoreDirectory = true;
            if (openFileDialog.ShowDialog() == true)
            {
                filePath = openFileDialog.FileName;

                Process pr = Process.Start(@"toolchain\create_hex.bat", filePath.Substring(0, filePath.Length - 2) + " " + Environment.CurrentDirectory + @"\src\data");
                while (!pr.HasExited) { }
                try
                {
                    Reader = new StreamReader(@"src\data\memory.hex");
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
                result_ALU = result_MEM = instr.Text = Prov.Console = prev_result_MEM = prev_result_ALU = Prev_Asm = prev_Decode = "";
                wait = Prov.CLK = 0;
                Prov.PC = PC = "00000000";
                run_btn.IsEnabled = step_btn.IsEnabled = true;
                grid_data.Items.Refresh();
                data_register.Items.Refresh();
                Reader.Close();
            }

        }

        void Write_reg()
        {
            using (StreamWriter fs = new StreamWriter(@"src/data/register.hex"))
            {
                foreach (var buf in Prov.Registers)
                {
                    fs.WriteLine(buf.Key + " \t" + buf.Value);
                }
            }
        }

        private void reset_btn_Click(object sender, RoutedEventArgs e)
        {
            Prov.PC = PC = "00000000";
            Stage_conv["Write back"] = Stage_conv["Memory"] = Stage_conv["Execute"] = Stage_conv["Decode"] = Stage_conv["Fetch"] = "nop";
            Stage_conv["Wait"] = "0";
            int i = 1;
            for (; i < 31; i++)
            {
                if (i == 2)
                    Prov.Registers["x" + i][1] = "0x00007FF0";
                else
                    if(i == 3)
                        Prov.Registers["x" + i][1] = "0x00001000";
                    else
                        Prov.Registers["x" + i][1] = "0x00000000";
            }
            i = 256;
            while (i < 2048)
            {
                Alg_operation.Data_seg[Convert.ToString(i * 16, 16).PadLeft(8, '0')] = "00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ";
                i++;
            }
            try
            {
                Reader = new StreamReader(@"src\data\memory.hex");
            }
            catch (Exception)
            {
                Prov.Console = "File:" + @"src\data\memory.hex" + " not find";
                return;
            }
            string buf;
            Reader.ReadLine();
            i = 0;
            while (true)
            {
                if (Reader.ReadLine()[0] == '@')
                    break;
                i++;
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
            result_ALU = result_MEM = instr.Text = Prov.Console = prev_result_MEM = prev_result_ALU = Prev_Asm = prev_Decode = "";
            wait = Prov.CLK = 0;
            grid_stage.Items.Refresh();
            data_register.Items.Refresh();
            run_btn.IsEnabled = step_btn.IsEnabled = true;
            instr.Text = "";
        }

        private void Single_Click(object sender, RoutedEventArgs e)
        {
            step_btn.Click += Step_btn_Click;
            run_btn.Click += Run_btn_Click;
            step_btn.Click -= Step_btn_Click2;
            run_btn.Click -= Run_btn_Click2;
            Prov.Stage = Single.IsChecked = true;
            Five.IsChecked = false;
        }

        private void Five_Click(object sender, RoutedEventArgs e)
        {
            step_btn.Click += Step_btn_Click2;
            run_btn.Click += Run_btn_Click2;
            step_btn.Click -= Step_btn_Click;
            run_btn.Click -= Run_btn_Click;
            Prov.Stage = Single.IsChecked = false;
            Five.IsChecked = true;
        }


        string Load_store(string data, string instr)
        {
            string[] buf = data.Split(' ');
            string buf2 = instr.Split(' ')[0];
            if (buf2 == "SB" || buf2 == "SH" || buf2 == "SW")
            {
                Mem.Write_data(buf[0], buf[1]);
                return "";
            }
            else
                switch (buf2)
                {
                    case "LB":
                        {
                            if (buf[1] != "x0")
                            {
                                buf[0] = Mem.Read_data_byte(buf[0]);
                                if (buf[0][0] >= '8')
                                    return "0x" + buf[0].PadLeft(8, 'F').ToUpper() + " " + buf[1];
                                else
                                    return "0x" + buf[0].PadLeft(8, '0').ToUpper() + " " + buf[1];
                            }
                            else
                                return "";
                        }
                    case "LH":
                        {
                            if (buf[1] != "x0")
                            {
                                buf[0] = Mem.Read_data_hw(buf[0]);
                                if (buf[0][0] >= '8')
                                    return "0x" + buf[0].PadLeft(8, 'F').ToUpper() + " " + buf[1];
                                else
                                    return "0x" + buf[0].PadLeft(8, '0').ToUpper() + " " + buf[1];
                            }
                            else
                                return "";
                        }
                    case "LW":
                        {
                            if (buf[1] != "x0")
                            {
                                buf[0] = Mem.Read_data_word(buf[0]);
                                return "0x" + buf[0].ToUpper() + " " + buf[1];
                            }
                            else
                                return "";
                        }
                    case "LHU":
                        {
                            if (buf[1] != "x0")
                            {
                                buf[0] = Mem.Read_data_hw(buf[0]);
                                return "0x" + buf[0].PadLeft(8, '0').ToUpper() + " " + buf[1];
                            }
                            else
                                return "";
                        }
                    case "LBU":
                        {
                            if (buf[1] != "x0")
                            {
                                buf[0] = Mem.Read_data_byte(buf[0]);
                                return "0x" + buf[0].PadLeft(8, '0').ToUpper() + " " + buf[1];
                            }
                            else
                                return "";
                        }
                    default:
                        {
                            if (buf2[0] != 'B')
                                return data;
                            else
                                return "";
                        }
                }
        }


        void Write_data()
        {

        }
    }
}

