using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Simulator_RISCV
{
    class Alg_operation : INotifyPropertyChanged
    {
        Memory memory;
        public bool Stage;
        string pc;
        public string PC
        {
            get { return pc; }
            set
            {
                pc = value;
                RaisePropertyChanged("PC");
            }
        }
        string console;
        public string Console
        {
            get { return console; }
            set
            {
                console = value;
                RaisePropertyChanged("Console");
            }
        }
        int clk;
        public int CLK
        {
            get { return clk; }
            set
            {
                clk = value;
                RaisePropertyChanged("CLK");
            }
        }


        public Dictionary<string, string[]> Registers
        {
            get; set;
        }

        public static Dictionary<string, string> Data_seg
        {
            get; set;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public Alg_operation()
        {
            memory = new Memory();
            PC = "00000000";
            Stage = true;
            Registers = new Dictionary<string, string[]>
            {
                { "x0", new string[] {"zero", "0x00000000"} },
                { "x1", new string[] { "ra", "0x00000000" } },
                { "x2", new string[] { "sp", "0x00007FF0" } },
                { "x3", new string[] { "gp", "0x00001000" } },
                { "x4", new string[] { "tp", "0x00000000" } },
                { "x5", new string[] { "t0", "0x00000000" } },
                { "x6", new string[] { "t1", "0x00000000" } },
                { "x7", new string[] { "t2", "0x00000000" } },
                { "x8", new string[] { "s0", "0x00000000" } },
                { "x9", new string[] { "s1", "0x00000000" } },
                { "x10", new string[] { "a0", "0x00000000" } },
                { "x11", new string[] { "a1", "0x00000000" } },
                { "x12", new string[] { "a2", "0x00000000" } },
                { "x13", new string[] { "a3", "0x00000000" } },
                { "x14", new string[] { "a4", "0x00000000" } },
                { "x15", new string[] { "a5", "0x00000000" } },
                { "x16", new string[] { "a6", "0x00000000" } },
                { "x17", new string[] { "a7", "0x00000000" } },
                { "x18", new string[] { "s2", "0x00000000" } },
                { "x19", new string[] { "s3", "0x00000000" } },
                { "x20", new string[] { "s4", "0x00000000" } },
                { "x21", new string[] { "s5", "0x00000000" } },
                { "x22", new string[] { "s6", "0x00000000" } },
                { "x23", new string[] { "s7", "0x00000000" } },
                { "x24", new string[] { "s8", "0x00000000" } },
                { "x25", new string[] { "s9", "0x00000000" } },
                { "x26", new string[] { "s10", "0x00000000" } },
                { "x27", new string[] { "s11", "0x00000000" } },
                { "x28", new string[] { "t3", "0x00000000" } },
                { "x29", new string[] { "t4", "0x00000000" } },
                { "x30", new string[] { "t5", "0x00000000" } },
                { "x31", new string[] { "t6", "0x00000000" } }
            };

        }
        public void Reg_init()
        {
            int i = 0;
            while (i < 31)
            {
                if (i == 2)
                    Registers["x2"][1] = "0x00007FF0";
                else
                if(i == 3)
                    Registers["x3"][1] = "0x00001000";
                else
                Registers["x" + i][1] = "0x00000000";
                i++;
            }
        }
        public string Comand_Real(string inst)
        {
            Operation my_inst = new Operation();
            uint inp1;
            uint inp2;
            try
            {
                my_inst.name_op = inst.Split(' ')[0].ToLower();
                my_inst.op1 = inst.Split(' ')[1];
                my_inst.op2 = inst.Split(' ')[2];
                my_inst.op3 = inst.Split(' ')[3];
            }

            catch (Exception)
            {
                my_inst.op3 = "";
            }
            switch (my_inst.name_op)
            {
                case ("lui"):
                    my_inst.number1 = (my_inst.op2 + "000").PadLeft(8, '0');
                    PC = Convert.ToString(Convert.ToUInt32(PC, 16) + 4, 16).PadLeft(8, '0');
                    if (my_inst.op1 != "x0")
                        return "0x" + my_inst.number1.ToUpper() + " " + my_inst.op1;
                    return "";

                case ("auipc"):
                    my_inst.number1 = Convert.ToString(Convert.ToUInt32(PC, 16) + Convert.ToInt32(my_inst.op2, 16), 16).PadLeft(8, '0');
                    PC = Convert.ToString((Convert.ToUInt32(PC, 16) + 4), 16).PadLeft(8, '0'); // rd = pc + length(inst) pc + 4          
                    if (my_inst.op1 != "x0")
                        return  "0x" + my_inst.number1.ToUpper() +  " " + my_inst.op1;
                    return "";

                case ("jal"):
                    my_inst.number1 = (Convert.ToUInt32(Registers[my_inst.op2][1], 16) + Convert.ToUInt32(PC, 16)).ToString("X");
                    PC = my_inst.number1.PadLeft(8, '0');
                    if (my_inst.op1 != "x0")
                        return "0x" + Convert.ToString(Convert.ToUInt32(PC, 16) + 4, 16).PadLeft(8, '0').ToUpper() + " " + my_inst.op1;
                    return "";

                case ("jalr")://???? Обращение в память, не понимаю как работает
                    my_inst.number1 = (Convert.ToUInt32(Registers[my_inst.op2][1], 16) + Convert.ToUInt32(my_inst.op3, 16)).ToString("X");
                    my_inst.number2 = "0x" + Convert.ToString(Convert.ToUInt32(PC, 16) + 4, 16).PadLeft(8, '0').ToUpper();
                    PC = my_inst.number1.PadLeft(8, '0');
                    if (my_inst.op1 != "x0")
                        return  my_inst.number2 + " " + my_inst.op1;
                    return "";

                case ("beq")://???? Обращение в память
                    my_inst.number1 = Registers[my_inst.op1][1].Substring(2, 8);
                    my_inst.number2 = Registers[my_inst.op2][1].Substring(2, 8);
                    if (my_inst.number1 == my_inst.number2)
                        PC = (Convert.ToUInt32(PC, 16) + Convert.ToUInt32(my_inst.op3, 16)).ToString("X").PadLeft(8, '0');
                    else
                        PC = Convert.ToString(Convert.ToUInt32(PC, 16) + 4, 16).PadLeft(8, '0'); // rd = pc + length(inst) pc + 4
                    return "";

                case ("bne")://???? Обращение в память
                    my_inst.number1 = Registers[my_inst.op1][1].Substring(2, 8);
                    my_inst.number2 = Registers[my_inst.op2][1].Substring(2, 8);
                    if (my_inst.number1 != my_inst.number2)
                        PC = (Convert.ToUInt32(PC, 16) + Convert.ToUInt32(my_inst.op3, 16)).ToString("X").PadLeft(8, '0');//(pc(2cc)->(10cc) + offset(16cc)->(10cc))->16cc
                    else
                        PC = Convert.ToString(Convert.ToUInt32(PC, 16) + 4, 16).PadLeft(8, '0'); // rd = pc + length(inst) pc + 4
                    return "";

                case ("blt")://???? Обращение в память
                    my_inst.number1 = Registers[my_inst.op1][1].Substring(2, 8);
                    my_inst.number2 = Registers[my_inst.op2][1].Substring(2, 8);
                    if (my_inst.op3[0] > '8')
                        my_inst.op3 = my_inst.op3.PadLeft(8, 'F');
                    if (String.Compare(my_inst.number1, my_inst.number2) < 0)
                        PC = (Convert.ToUInt32(PC, 16) + Convert.ToUInt32(my_inst.op3, 16)).ToString("X").PadLeft(8, '0');//(pc(2cc)->(10cc) + offset(16cc)->(10cc))->16cc
                    else
                        PC = Convert.ToString(Convert.ToUInt32(PC, 16) + 4, 16).PadLeft(8, '0'); // rd = pc + length(inst) pc + 4
                    return "";

                case ("bge")://???? Обращение в память
                    my_inst.number1 = Registers[my_inst.op1][1].Substring(2, 8);
                    my_inst.number2 = Registers[my_inst.op2][1].Substring(2, 8);
                    if (String.Compare(my_inst.number1, my_inst.number2) > 0 || String.Compare(my_inst.number1, my_inst.number2) == 0)
                        PC = (Convert.ToUInt32(PC, 16) + Convert.ToUInt32(my_inst.op3, 16)).ToString("X").PadLeft(8, '0');//(pc(2cc)->(10cc) + offset(16cc)->(10cc))->16cc
                    else
                        PC = Convert.ToString(Convert.ToUInt32(PC, 16) + 4, 16).PadLeft(8, '0'); // rd = pc + length(inst) pc + 4
                    return "";

                case ("bltu")://???? Обращение в память
                    my_inst.number1 = Registers[my_inst.op1][1].Substring(2, 8);
                    my_inst.number2 = Registers[my_inst.op2][1].Substring(2, 8);
                    if (my_inst.number1[0] >= '8')// если условие выполняется, значет это не отрицательное число
                    {
                        inp1 = Convert.ToUInt32(my_inst.number1, 16);
                        inp1 = ~inp1 + 1;
                    }
                    else
                    {
                        inp1 = Convert.ToUInt32(my_inst.number1, 16);
                    }

                    if (my_inst.number2[0] >= '8')
                    {
                        inp2 = Convert.ToUInt32(my_inst.number2, 16);
                        inp2 = ~inp2 + 1;
                    }
                    else
                    {
                        inp2 = Convert.ToUInt32(my_inst.number2, 16);
                    }
                    if (inp1 < inp2)
                    {
                        PC = (Convert.ToUInt32(PC, 16) + Convert.ToUInt32(my_inst.op3, 16)).ToString("X").PadLeft(8, '0');//(pc(2cc)->(10cc) + offset(16cc)->(10cc))->16cc
                    }
                    else
                        PC = Convert.ToString(Convert.ToUInt32(PC, 16) + 4, 16).PadLeft(8, '0'); // rd = pc + length(inst) pc + 4
                    return "";

                case ("bgeu")://???? Обращение в память
                    my_inst.number1 = Registers[my_inst.op1][1].Substring(2, 8);
                    my_inst.number2 = Registers[my_inst.op2][1].Substring(2, 8);
                    if (my_inst.number1[0] >= '8')
                    {
                        inp1 = Convert.ToUInt32(my_inst.number1, 16);
                        inp1 = ~inp1 + 1;
                    }
                    else
                    {
                        inp1 = Convert.ToUInt32(my_inst.number1, 16);
                    }

                    if (my_inst.number2[0] >= '8')
                    {
                        inp2 = Convert.ToUInt32(my_inst.number2, 16);
                        inp2 = ~inp2 + 1;
                    }
                    else
                    {
                        inp2 = Convert.ToUInt32(my_inst.number2, 16);
                    }
                    if (inp1 >= inp2)
                    {
                        PC = (Convert.ToUInt32(PC, 16) + Convert.ToUInt32(my_inst.op3, 16)).ToString("X").PadLeft(8, '0');//(pc(2cc)->(10cc) + offset(16cc)->(10cc))->16cc
                    }
                    else
                        PC = Convert.ToString(Convert.ToUInt32(PC, 16) + 4, 16).PadLeft(8, '0'); // rd = pc + length(inst) pc + 4
                    return "";

                case ("lb"):
                    my_inst.number2 = Registers[my_inst.op2][1].Substring(2, 8);//rs1
                    my_inst.number2 = Convert.ToString(Convert.ToUInt32(my_inst.number2, 16) + Convert.ToUInt32(my_inst.op3, 16), 16).PadLeft(8, '0');
                    PC = Convert.ToString(Convert.ToUInt32(PC, 16) + 4, 16).PadLeft(8, '0');
                    return "0x" + my_inst.number2.ToUpper() + " " + my_inst.op1;

                case ("lh"):
                    my_inst.number2 = Registers[my_inst.op2][1].Substring(2, 8);
                    my_inst.number2 = Convert.ToString(Convert.ToUInt32(my_inst.number2, 16) + Convert.ToUInt32(my_inst.op3, 16), 16).PadLeft(8, '0');
                    PC = Convert.ToString(Convert.ToUInt32(PC, 16) + 4, 16).PadLeft(8, '0');
                    return "0x" + my_inst.number2.ToUpper() + " " + my_inst.op1;

                case ("lw"):
                    my_inst.number2 = Registers[my_inst.op2][1].Substring(2, 8);//rs1
                    if (my_inst.op3[0] >= '8')
                        my_inst.op3 = my_inst.op3.PadLeft(8, 'f');
                    my_inst.number2 = Convert.ToString(Convert.ToUInt32(my_inst.number2, 16) + Convert.ToUInt32(my_inst.op3, 16), 16).PadLeft(8, '0');
                    PC = Convert.ToString(Convert.ToUInt32(PC, 16) + 4, 16).PadLeft(8, '0');
                    return "0x" + my_inst.number2.ToUpper() + " " + my_inst.op1;

                case ("lbu"):
                    my_inst.number2 = Registers[my_inst.op2][1].Substring(2, 8);//rs1
                    my_inst.number2 = Convert.ToString(Convert.ToUInt32(my_inst.number2, 16) + Convert.ToUInt32(my_inst.op3, 16), 16).PadLeft(8, '0');
                    PC = Convert.ToString(Convert.ToUInt32(PC, 16) + 4, 16).PadLeft(8, '0');
                    return "0x" + my_inst.number2.PadLeft(8, '0').ToUpper() + " " + my_inst.op1;

                case ("lhu"):
                    my_inst.number2 = Registers[my_inst.op2][1].Substring(2, 8);
                    my_inst.number2 = Convert.ToString(Convert.ToUInt32(my_inst.number2, 16) + Convert.ToUInt32(my_inst.op3, 16), 16).PadLeft(8, '0');
                    PC = Convert.ToString(Convert.ToUInt32(PC, 16) + 4, 16).PadLeft(8, '0');
                    return "0x" + my_inst.number2.ToUpper() + " " + my_inst.op1;

                case ("sb"):
                    my_inst.number2 = Registers[my_inst.op2][1].Substring(2, 8);
                    my_inst.number2 = Convert.ToString(Convert.ToUInt32(my_inst.number2, 16) + Convert.ToUInt32(my_inst.op3, 16), 16).PadLeft(8, '0');
                    my_inst.number1 = Registers[my_inst.op1][1].Substring(8, 2);
                    PC = Convert.ToString(Convert.ToUInt32(PC, 16) + 4, 16).PadLeft(8, '0'); // rd = pc + length(inst) pc + 4
                    return "0x" + my_inst.number2 + " " + my_inst.number1;
                    //memory.Write_data(my_inst.number2, my_inst.number1);

                case ("sh"):
                    my_inst.number2 = Registers[my_inst.op2][1].Substring(2, 8);
                    my_inst.number2 = Convert.ToString(Convert.ToUInt32(my_inst.number2, 16) + Convert.ToUInt32(my_inst.op3, 16), 16).PadLeft(8, '0');
                    my_inst.number1 = Registers[my_inst.op1][1].Substring(6, 4);
                    PC = Convert.ToString(Convert.ToUInt32(PC, 16) + 4, 16).PadLeft(8, '0'); // rd = pc + length(inst) pc + 4
                    return my_inst.number2 + " " + my_inst.number1;
                    //memory.Write_data(my_inst.number2, my_inst.number1);

                case ("sw"):
                    my_inst.number2 = Registers[my_inst.op2][1].Substring(2, 8);
                    my_inst.number2 = Convert.ToString(Convert.ToUInt32(my_inst.number2, 16) + Convert.ToUInt32(my_inst.op3, 16), 16).PadLeft(8, '0');
                    my_inst.number1 = Registers[my_inst.op1][1].Substring(2, 8);
                    PC = Convert.ToString(Convert.ToUInt32(PC, 16) + 4, 16).PadLeft(8, '0'); // rd = pc + length(inst) pc + 4
                    return my_inst.number2 + " " + my_inst.number1;
                    //memory.Write_data(my_inst.number2, my_inst.number1);

                case ("addi"):
                    my_inst.op2 = Registers[my_inst.op2][1].Substring(2, 8);
                    if (my_inst.op3.Length == 3)
                        if (my_inst.op3[0] >= '8')
                            my_inst.op3 = my_inst.op3.PadLeft(8, 'F');
                    my_inst.number1 = Convert.ToString(Convert.ToUInt32(my_inst.op2, 16) + Convert.ToUInt32(my_inst.op3, 16), 16).PadLeft(8, '0');
                    PC = Convert.ToString(Convert.ToUInt32(PC, 16) + 4, 16).PadLeft(8, '0');
                    return "0x" + my_inst.number1.ToUpper() + " " + my_inst.op1;

                case ("slti"):
                    my_inst.op2 = Registers[my_inst.op2][1].Substring(2, 8);
                    if (my_inst.op3[0] >= '8')
                        my_inst.op3 = my_inst.op3.PadLeft(8,'f');
                    if (String.Compare(my_inst.op2, my_inst.op3.PadLeft(8, '0')) < 0)
                        my_inst.number1 = "00000001";
                    else
                        my_inst.number1 = "00000000";
                    PC = Convert.ToString(Convert.ToUInt32(PC, 16) + 4, 16).PadLeft(8, '0'); // rd = pc + length(inst) pc + 4*/
                    return "0x" + my_inst.number1.ToUpper() + " " + my_inst.op1;

                case ("sltiu"):
                    my_inst.number2 = Registers[my_inst.op2][1].Substring(2, 8);
                    if (my_inst.number2 == "00000000")
                        my_inst.number1 = "00000001";
                    else
                    {
                        if (my_inst.number2[0] >= '8')
                        {
                            inp1 = Convert.ToUInt32(my_inst.number2, 16);
                            inp1 = ~inp1 + 1;
                        }
                        else
                        {
                            inp1 = Convert.ToUInt32(my_inst.number2, 16);
                        }

                        if (my_inst.op3[0] >= '8')
                            my_inst.op3 = my_inst.op3.PadLeft(8, 'F');
                        else
                            my_inst.op3 = my_inst.op3.PadLeft(8, '0');

                        if (inp1 < Convert.ToUInt32(my_inst.op3))
                            my_inst.number1 = "00000001";
                        //Записать в память
                        else
                            my_inst.number1 = "00000000";
                        //Записать в память
                    }
                    PC = Convert.ToString(Convert.ToUInt32(PC, 16) + 4, 16).PadLeft(8, '0'); // rd = pc + length(inst) pc + 4
                    return "0x" + my_inst.number1.ToUpper() + " " + my_inst.op1;

                case ("xori"):
                    my_inst.number2 = Registers[my_inst.op2][1].Substring(2, 8);
                    PC = Convert.ToString(Convert.ToUInt32(PC, 16) + 4, 16).PadLeft(8, '0');
                    if (my_inst.op3 == "ffffffff")
                    {
                        my_inst.number2 = (~Convert.ToUInt32(my_inst.number2, 16)).ToString("X");
                        //Registers[my_inst.op1][1] = "0x" + my_inst.number2;
                        return "0x" + my_inst.number2.ToUpper() + " " + my_inst.op1;
                    }
                    if (my_inst.op3[0] >= '8')
                        my_inst.op3 = my_inst.op3.PadLeft(8, 'F');
                    else
                        my_inst.op3 = my_inst.op3.PadLeft(8, '0');
                    // rd = pc + length(inst) pc + 4
                    my_inst.number1 = (Convert.ToUInt32(my_inst.number2, 16) ^ Convert.ToUInt32(my_inst.op3, 16)).ToString("X").PadLeft(8, '0').ToUpper();
                    return "0x" + my_inst.number1 + " " + my_inst.op1;
                    
                case ("ori"):
                    my_inst.number2 = Registers[my_inst.op2][1].Substring(2, 8);
                    if (my_inst.op3[0] >= '8')
                        my_inst.op3 = my_inst.op3.PadLeft(8, 'F');
                    else
                        my_inst.op3 = my_inst.op3.PadLeft(8, '0');
                    PC = Convert.ToString((Convert.ToUInt32(PC, 16) + 4), 16).PadLeft(8, '0'); // rd = pc + length(inst) pc + 4
                    my_inst.number1 = (Convert.ToUInt32(my_inst.number2, 16) | Convert.ToUInt32(my_inst.op3, 16)).ToString("X").PadLeft(8, '0').ToUpper();
                    return "0x" + my_inst.number1 + " " + my_inst.op1;
                    

                case ("andi"):
                    my_inst.number2 = Registers[my_inst.op2][1].Substring(2, 8);
                    if (my_inst.op3[0] >= '8')
                        my_inst.op3 = my_inst.op3.PadLeft(8, 'F');
                    else
                        my_inst.op3 = my_inst.op3.PadLeft(8, '0');
                    PC = Convert.ToString(Convert.ToUInt32(PC, 16) + 4, 16).PadLeft(8, '0'); // rd = pc + length(inst) pc + 4
                    my_inst.number1 = (Convert.ToUInt32(my_inst.number2, 16) & Convert.ToUInt32(my_inst.op3, 16)).ToString("X").PadLeft(8, '0').ToUpper();
                    return "0x" + my_inst.number1 + " " + my_inst.op1;

                case ("slli"):
                    my_inst.number2 = Registers[my_inst.op2][1].Substring(2, 8);
                    if (my_inst.op3[0] >= '8')
                        my_inst.op3 = my_inst.op3.PadLeft(8, 'F');
                    PC = Convert.ToString((Convert.ToUInt32(PC, 16) + 4), 16).PadLeft(8, '0'); // rd = pc + length(inst) pc + 4
                    my_inst.number1 = (Convert.ToUInt32(my_inst.number2, 16) << Convert.ToInt32(my_inst.op3, 16)).ToString("X").PadLeft(8, '0').ToUpper();
                    return "0x" + my_inst.number1 + " " + my_inst.op1;

                case ("srli"):
                    my_inst.number2 = Registers[my_inst.op2][1].Substring(2, 8);
                    if (my_inst.op3[0] >= '8')
                        my_inst.op3 = my_inst.op3.PadLeft(8, 'F');
                    PC = Convert.ToString((Convert.ToUInt32(PC, 16) + 4), 16).PadLeft(8, '0'); // rd = pc + length(inst) pc + 4
                    my_inst.number1 = (Convert.ToUInt32(my_inst.number2, 16) >> Convert.ToInt32(my_inst.op3, 16)).ToString("X").PadLeft(8, '0').ToUpper();
                    return "0x" + my_inst.number1 + " " + my_inst.op1;

                case ("srai"):///
                    my_inst.number2 = Registers[my_inst.op2][1].Substring(2, 8);
                    if (my_inst.op3[0] >= '8')
                        my_inst.op3 = my_inst.op3.PadLeft(8, 'F');
                    my_inst.number1 = Convert.ToString(Convert.ToUInt32(my_inst.number2, 16) >> Convert.ToInt32(my_inst.op3, 16), 16);
                    if (my_inst.number2[0] >= '8')
                    {
                        my_inst.number1 = Convert.ToString(Convert.ToUInt32(my_inst.number1, 16), 2).PadLeft(32, '0').Substring(Convert.ToInt32(my_inst.op3, 16), 32 - Convert.ToInt32(my_inst.op3, 16)).PadLeft(32, '1');
                        my_inst.number1 = Convert.ToString(Convert.ToUInt32(my_inst.number1, 2), 16).PadLeft(8, '0');
                    }
                    PC = Convert.ToString(Convert.ToUInt32(PC, 16) + 4, 16).PadLeft(8, '0'); // rd = pc + length(inst) pc + 4
                    return "0x" + my_inst.number1 + " " + my_inst.op1;

                case ("add"):
                    my_inst.number1 = Registers[my_inst.op2][1].Substring(2, 8);
                    my_inst.number2 = Registers[my_inst.op3][1].Substring(2, 8);
                    try
                    {
                        my_inst.number1 = Convert.ToString(checked(Convert.ToUInt32(my_inst.number1, 16) + Convert.ToUInt32(my_inst.number2, 16)), 16);
                        //Записать в память
                        PC = Convert.ToString(Convert.ToUInt32(PC, 16) + 4, 16).PadLeft(8, '0');
                        return "0x" + my_inst.number1.PadLeft(8, '0').ToUpper() + " " + my_inst.op1;
                    }
                    catch (Exception)
                    {
                        my_inst.exeption = "Возникло переполнение при выполнении оперции сложения";
                        return "";
                    }

                case ("sub"):
                    my_inst.number1 = Registers[my_inst.op2][1].Substring(2, 8);
                    my_inst.number2 = Registers[my_inst.op3][1].Substring(2, 8);
                    try
                    {
                        my_inst.number1 = Convert.ToString(checked(Convert.ToUInt32(my_inst.number1, 16) - Convert.ToUInt32(my_inst.number2, 16)), 16);
                        PC = Convert.ToString(Convert.ToUInt32(PC, 16) + 4, 16).PadLeft(8, '0'); // rd = pc + length(inst) pc + 4
                        return "0x" + my_inst.number1.PadLeft(8, '0').ToUpper() + " " + my_inst.op1;
                    }
                    catch (Exception)
                    {
                        my_inst.exeption = "Возникло переполнение при выполнении оперции вычитания";
                        return "";
                    }

                case ("sll"):
                    my_inst.number1 = Registers[my_inst.op2][1].Substring(2, 8);
                    my_inst.number2 = Registers[my_inst.op3][1].Substring(2, 8);
                    PC = Convert.ToString(Convert.ToUInt32(PC, 16) + 4, 16).PadLeft(8, '0');
                    my_inst.number1 = (Convert.ToUInt32(my_inst.number1, 16) << Convert.ToInt32(my_inst.number2, 16)).ToString("X").PadLeft(8, '0').ToUpper();
                    return "0x" + my_inst.number1 + " " + my_inst.op1;

                case ("slt"):
                    my_inst.number1 = Registers[my_inst.op2][1].Substring(2, 8);
                    my_inst.number2 = Registers[my_inst.op3][1].Substring(2, 8);
                    PC = Convert.ToString(Convert.ToUInt32(PC, 16) + 4, 16).PadLeft(8, '0');
                    if (String.Compare(my_inst.number1, my_inst.number2) < 0)
                        return "0x00000001" + " " + my_inst.op1;
                    else
                        return "0x00000000" + " " + my_inst.op1;

                case ("sltu"):
                    my_inst.number1 = Registers[my_inst.op2][1].Substring(2, 8);
                    my_inst.number2 = Registers[my_inst.op3][1].Substring(2, 8);
                    if (my_inst.number1 == "00000000")
                        my_inst.number1 = "00000001";
                    else
                    {
                        if (my_inst.number1[0] >= '8' && my_inst.number1[0] <= 'f')
                        {
                            inp1 = Convert.ToUInt32(my_inst.number1, 16);
                            inp1 = ~inp1 + 1;
                        }
                        else
                        {
                            inp1 = Convert.ToUInt32(my_inst.number1, 16);
                        }

                        if (my_inst.number2[0] >= '8' && my_inst.number2[0] <= 'f')
                        {
                            inp2 = Convert.ToUInt32(my_inst.number2, 16);
                            inp2 = ~inp2 + 1;
                        }
                        else
                        {
                            inp2 = Convert.ToUInt32(my_inst.number2, 16);
                        }

                        if (inp1 < inp2)
                            my_inst.number1 = "00000001";
                        else
                            my_inst.number1 = "00000000";
                    }
                    PC = Convert.ToString(Convert.ToUInt32(PC, 16) + 4, 16).PadLeft(8, '0'); // rd = pc + length(inst) pc + 4
                    return "0x" + my_inst.number1 + " " + my_inst.op1;

                case ("xor"):
                    my_inst.number1 = Registers[my_inst.op2][1].Substring(2, 8);
                    my_inst.number2 = Registers[my_inst.op3][1].Substring(2, 8);
                    PC = Convert.ToString(Convert.ToUInt32(PC, 16) + 4, 16).PadLeft(8, '0');
                    my_inst.number1 = (Convert.ToUInt32(my_inst.number1, 16) ^ Convert.ToUInt32(my_inst.number2, 16)).ToString("X").PadLeft(8, '0').ToUpper();
                    return "0x" + my_inst.number1 + " " + my_inst.op1;

                case ("srl"):
                    my_inst.number1 = Registers[my_inst.op2][1].Substring(2, 8);
                    my_inst.number2 = Registers[my_inst.op3][1].Substring(2, 8);
                    PC = Convert.ToString((Convert.ToUInt32(PC, 16) + 4), 16).PadLeft(8, '0'); // rd = pc + length(inst) pc + 4
                    my_inst.number1 = (Convert.ToUInt32(my_inst.number1, 16) >> Convert.ToInt32(my_inst.number2, 16)).ToString("X").PadLeft(8, '0').ToUpper();
                    return "0x" + my_inst.number1 + " " + my_inst.op1;

                case ("sra"):
                    my_inst.number1 = Registers[my_inst.op2][1].Substring(2, 8);
                    my_inst.number2 = Registers[my_inst.op3][1].Substring(2, 8);
                    my_inst.number1 = Convert.ToString(Convert.ToUInt32(my_inst.number1, 16) >> Convert.ToInt32(my_inst.number2, 16), 16);
                    if (my_inst.number2[0] >= '8')
                    {
                        my_inst.number1 = Convert.ToString(Convert.ToUInt32(my_inst.number1, 16), 2).PadLeft(32, '0').Substring(Convert.ToInt32(my_inst.op3, 16), 32 - Convert.ToInt32(my_inst.op3, 16)).PadLeft(32, '1');
                        my_inst.number1 = Convert.ToString(Convert.ToUInt32(my_inst.number1, 2), 16).PadLeft(8, '0');
                    }
                    PC = Convert.ToString(Convert.ToUInt32(PC, 16) + 4, 16).PadLeft(8, '0'); // rd = pc + length(inst) pc + 4
                    return "0x" + my_inst.number1 + " " + my_inst.op1;

                case ("or"):
                    my_inst.number1 = Registers[my_inst.op2][1].Substring(2, 8);
                    my_inst.number2 = Registers[my_inst.op3][1].Substring(2, 8);
                    PC = Convert.ToString(Convert.ToUInt32(PC, 16) + 4, 16).PadLeft(8, '0');
                    my_inst.number1 = (Convert.ToUInt32(my_inst.number1, 16) | Convert.ToUInt32(my_inst.number2, 16)).ToString("X").PadLeft(8, '0').ToUpper();
                    return "0x" + my_inst.number1 + " " + my_inst.op1;

                case ("and"):
                    my_inst.number1 = Registers[my_inst.op2][1].Substring(2, 8);
                    my_inst.number2 = Registers[my_inst.op3][1].Substring(2, 8);
                    PC = Convert.ToString(Convert.ToUInt32(PC, 16) + 4, 16).PadLeft(8, '0');
                    my_inst.number1 = (Convert.ToUInt32(my_inst.number1, 16) + Convert.ToUInt32(my_inst.number2, 16)).ToString("X").PadLeft(8, '0').ToUpper();
                    return "0x" + my_inst.number1 + " " + my_inst.op1;

                case "ecall":
                    my_inst.number1 = Registers["x10"][1].Substring(2, 8);
                    if (Convert.ToInt32(my_inst.number1, 16) != 10)
                    {
                        PC = Convert.ToString(Convert.ToUInt32(PC, 16) + 4, 16).PadLeft(8, '0');
                        switch (Convert.ToInt32(my_inst.number1, 16))
                        {
                            case 1:
                                {
                                    my_inst.number2 = Registers["x11"][1].Substring(2, 8);
                                    Console += Convert.ToInt32(my_inst.number2, 16).ToString();
                                    return "";
                                }
                            case 11:
                                {
                                    my_inst.number2 = Registers["x11"][1].Substring(8, 2);
                                    byte buf = Convert.ToByte(Convert.ToInt32(my_inst.number2, 16));
                                    Console += Encoding.GetEncoding(1251).GetString((new byte[] { buf }));
                                    return "";
                                }
                            default:
                                {
                                    Console += "\necall don't worked with a0 = " + my_inst.number1;
                                    return "ecall exit";
                                }
                        }
                    }
                    else
                    {
                        Console += "\nДостигнут конец программы!! :D";
                        return "ecall exit";
                    }
                default:
                        Console += "\nДостигнут конец программы!! :D";
                        return "";
            }
        }
    }

    class Operation
    {
        public string Inst { get; set; } = ""; // istraction 
        public string name_op { get; set; } = "";
        public string op1 { get; set; } = "";
        public string number1 { get; set; } = "";//fffffff6
        public string op2 { get; set; } = "";
        public string number2 { get; set; } = "";//00000006
        public string op3 { get; set; } = "";
        public string exeption { get; set; } = "";
    }
}

