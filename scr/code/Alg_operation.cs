using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simuliator
{
    class Alg_operation
    {
        public string Inst { get; set; } = "";

        public string name_op { get; set; } = "";
        public string op1 { get; set; } = "";
        public string number1 { get; set; } = "fffffff6";//fffffff6
        public string op2 { get; set; } = "";
        public string number2 { get; set; } = "fffffff6";
        public string op3 { get; set; } = "";       
        public string pc { get; set; } = "1000";

        /*public uint inp1 { get; set; }
        public uint inp2 { get; set; }*/


        public void Comand_Real(string inst)
        {
            Alg_operation my_inst = new Alg_operation();
            uint inp1 = 0;
            uint inp2 = 0;
            try
            {
                my_inst.name_op = inst.Split(' ')[0];
                my_inst.op1 = inst.Split(',')[0].Trim(' ').Substring(my_inst.name_op.Length + 1);
                my_inst.op2 = inst.Split(',')[1].Trim(' ');
                //if ((inst.Count(f => f == ',')) == 3)
                my_inst.op3 = inst.Split(',')[2];
            }

            catch (Exception)
            {
                my_inst.op3 = "";
            }

            switch (my_inst.name_op)
            {
                case ("lui"):
                    if (my_inst.op1.IndexOf("x") != -1)
                        if (my_inst.op2.IndexOf("x") != -1)
                        {      
                            my_inst.number1 = my_inst.op2.Substring(2); //cut "0x"
                            my_inst.number1 = "0x" + (my_inst.op2 + "000").PadLeft(8, '0');
                        }
                        else
                        {
                            my_inst.number1 = "0x" + (Convert.ToInt32(my_inst.op2, 10)).ToString("X").PadLeft(5, '0') + "000";
                        }
                    my_inst.pc = "0x" + Convert.ToString((Convert.ToInt32(my_inst.pc, 2) + 4), 16).PadLeft(8, '0'); // rd = pc + length(inst) pc + 4
                    break;

                case ("auipc"):
                    if (my_inst.op1.IndexOf("x") != -1)
                        if (my_inst.op2.IndexOf("x") != -1)
                        {
                            my_inst.op2 = my_inst.op2.Substring(2); //cut "0x"
                            my_inst.number1 = "0x" + (my_inst.op2).PadLeft(5, '0') + Convert.ToString(Convert.ToInt32(my_inst.pc, 2), 16).PadLeft(3, '0');
                        }
                        else
                        {
                            my_inst.number1 = "0x" + (Convert.ToInt32(my_inst.op2, 10)).ToString("X").PadLeft(5, '0') + Convert.ToString(Convert.ToInt32(my_inst.pc, 2), 16).PadLeft(3, '0');//op2 + pc(16cc);
                        }
                    my_inst.pc = "0x" + Convert.ToString((Convert.ToInt32(my_inst.pc, 2) + 4), 16).PadLeft(8, '0'); // rd = pc + length(inst) pc + 4          
                    break;

                case ("jal"):
                    if (my_inst.op1.IndexOf("x") != -1)
                    {
                        my_inst.op2 = my_inst.op2.Substring(2); //cut "0x"
                        my_inst.number1 = "0x" + Convert.ToString((Convert.ToInt32(my_inst.pc, 2) + 4), 16).PadLeft(8, '0'); // rd = pc + length(inst)
                        my_inst.pc = "0x" + Convert.ToString(Convert.ToInt32(Convert.ToString(Convert.ToInt32(my_inst.pc, 2), 10)) + Convert.ToInt32(Convert.ToString(Convert.ToInt32(my_inst.op2, 16), 10)), 16).PadLeft(8, '0');//pc в 16сс                     
                    }
                    my_inst.pc = "0x" + Convert.ToString((Convert.ToInt32(my_inst.pc, 2) + 4), 16).PadLeft(8, '0'); // rd = pc + length(inst) pc + 4
                    break;

                case ("jalr")://???? Обращение в память
                    if (my_inst.op1.IndexOf("x") != -1)
                    {
                        my_inst.op2 = my_inst.op2.Substring(2); //cut "0x"
                        my_inst.number1 = "0x" + Convert.ToString((Convert.ToInt32(my_inst.pc, 2) + 4), 16).PadLeft(8, '0'); // rd = pc + length(inst)
                        my_inst.pc = "0x" + Convert.ToString(Convert.ToInt32(Convert.ToString(Convert.ToInt32(my_inst.pc, 2), 10)) + Convert.ToInt32(Convert.ToString(Convert.ToInt32(my_inst.op2, 16), 10)), 16).PadLeft(8, '0');//pc в 16сс                     
                    }
                    my_inst.pc = "0x" + Convert.ToString((Convert.ToInt32(my_inst.pc, 2) + 4), 16).PadLeft(8, '0'); // rd = pc + length(inst) pc + 4
                    break;

                case ("beq")://???? Обращение в память
                    if (my_inst.number1 == my_inst.number2)
                        my_inst.pc = "0x" + Convert.ToString(Convert.ToInt32(Convert.ToString(Convert.ToInt32(my_inst.pc, 2), 10)) + Convert.ToInt32(Convert.ToString(Convert.ToInt32(my_inst.op2, 16), 10)), 16).PadLeft(8, '0');//(pc(2cc)->(10cc) + offset(16cc)->(10cc))->16cc
                    my_inst.pc = "0x" + Convert.ToString((Convert.ToInt32(my_inst.pc, 2) + 4), 16).PadLeft(8, '0'); // rd = pc + length(inst) pc + 4
                    break;

                case ("bne")://???? Обращение в память
                    if (my_inst.number1 != my_inst.number2)
                        my_inst.pc = "0x" + Convert.ToString(Convert.ToInt32(Convert.ToString(Convert.ToInt32(my_inst.pc, 2), 10)) + Convert.ToInt32(Convert.ToString(Convert.ToInt32(my_inst.op2, 16), 10)), 16).PadLeft(8, '0');//(pc(2cc)->(10cc) + offset(16cc)->(10cc))->16cc
                    break;

                case ("blt")://???? Обращение в память
                    if (String.Compare(my_inst.number1, my_inst.number2) < 0)
                        my_inst.pc = "0x" + Convert.ToString(Convert.ToInt32(Convert.ToString(Convert.ToInt32(my_inst.pc, 2), 10)) + Convert.ToInt32(Convert.ToString(Convert.ToInt32(my_inst.op2, 16), 10)), 16).PadLeft(8, '0');//(pc(2cc)->(10cc) + offset(16cc)->(10cc))->16cc
                    my_inst.pc = "0x" + Convert.ToString((Convert.ToInt32(my_inst.pc, 2) + 4), 16).PadLeft(8, '0'); // rd = pc + length(inst) pc + 4
                    break;

                case ("bge")://???? Обращение в память                  
                    if (String.Compare(my_inst.number1, my_inst.number2) > 0 || String.Compare(my_inst.number1, my_inst.number2) == 0)
                        my_inst.pc = "0x" + Convert.ToString(Convert.ToInt32(Convert.ToString(Convert.ToInt32(my_inst.pc, 2), 10)) + Convert.ToInt32(Convert.ToString(Convert.ToInt32(my_inst.op2, 16), 10)), 16).PadLeft(8, '0');//(pc(2cc)->(10cc) + offset(16cc)->(10cc))->16cc
                    my_inst.pc = "0x" + Convert.ToString((Convert.ToInt32(my_inst.pc, 2) + 4), 16).PadLeft(8, '0'); // rd = pc + length(inst) pc + 4
                    break;

                case ("bltu")://???? Обращение в память                   
                    if (my_inst.number1[0] >= '8' && my_inst.number1[0] <= 'f')// если условие выполняется, значет это не отрицательное число
                    {
                        inp1 = Convert.ToUInt32(my_inst.number1, 16);
                        inp1 = ~inp1 + 1;
                    }
                    //my_inst.number1 = Convert.ToString(inp, 2);//перевод из 10сс в 2сс
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
                    {
                        my_inst.op3 = my_inst.op3.Trim(' ').Substring(2); //cut "0x"
                        my_inst.pc = "0x" + Convert.ToString(Convert.ToInt32(Convert.ToString(Convert.ToInt32(my_inst.pc, 2), 10)) + Convert.ToInt32(Convert.ToString(Convert.ToInt32(my_inst.op3, 16), 10)), 16).PadLeft(8, '0');//(pc(2cc)->(10cc) + offset(16cc)->(10cc))->16cc
                    }
                    else
                        my_inst.pc = "0x" + Convert.ToString((Convert.ToInt32(my_inst.pc, 2) + 4), 16).PadLeft(8, '0'); // rd = pc + length(inst) pc + 4
                    break;

                case ("bgeu")://???? Обращение в память                   
                    if (my_inst.number1[0] >= '8' && my_inst.number1[0] <= 'f')
                    {
                        inp1 = Convert.ToUInt32(my_inst.number1, 16);
                        inp1 = ~inp1 + 1;
                    }
                    //my_inst.number1 = Convert.ToString(inp, 2);//перевод из 10сс в 2сс
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
                    if (inp1 >= inp2)
                    {
                        my_inst.op3 = my_inst.op3.Trim(' ').Substring(2); //cut "0x"
                        my_inst.pc = "0x" + Convert.ToString(Convert.ToInt32(Convert.ToString(Convert.ToInt32(my_inst.pc, 2), 10)) + Convert.ToInt32(Convert.ToString(Convert.ToInt32(my_inst.op3, 16), 10)), 16).PadLeft(8, '0');//(pc(2cc)->(10cc) + offset(16cc)->(10cc))->16cc
                    }
                    else
                        my_inst.pc = "0x" + Convert.ToString((Convert.ToInt32(my_inst.pc, 2) + 4), 16).PadLeft(8, '0'); // rd = pc + length(inst) pc + 4
                    break;
            }
        }
    }
}

