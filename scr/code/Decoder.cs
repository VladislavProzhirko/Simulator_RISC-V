using System;
using System.IO;
using System.Text;

namespace Desh_test
{
    class Decoder
    {
        class AsmInstruction //дешифрованная инструкция
        {
            public string Inst { get; set; } = "";
            public string Op1 { get; set; } = "";
            public string Op2 { get; set; } = "";
            public string Op3 { get; set; } = "";
            public string Full_Comand { get; set; } = "unknown";

            public void Clear_Inst()
            {
                Inst = "";
                Full_Comand = "";
                Op1 = "";
                Op2 = "";
                Op3 = "";
            }

        }
        class DeshInstruction //класс для определения команды
        {
            public string Opcode { get; set; } = "";
            public string Funct3 { get; set; } = "";
            public string Funct7 { get; set; } = "";

        }
        public string ReadInstruction(string path)
        {
            DeshInstruction DeshInst = new DeshInstruction();
            AsmInstruction AsmInst = new AsmInstruction();
            
            StreamReader Read = new StreamReader(path);
            string Instruction = "", s;
            while ((Instruction = Read.ReadLine()) != null)
            {
                //отделяем "0х" от кода команды, переводим в 2сс из 16сс
                Instruction = Convert.ToString(Convert.ToInt32(Instruction.Trim('0', 'x').ToUpper(), 16), 2).PadLeft(32, '0');
                DeshInst.Opcode = Instruction.Substring(25);
                DeshInst.Funct3 = Instruction.Substring(17, 3);
                DeshInst.Funct7 = Instruction.Substring(0, 7);
                AsmInst.Clear_Inst();//очищаем поля от предыдущей инструкции
                string[] inst;

                StreamReader ReadInstName = new StreamReader("Comand_base.txt");
                while (( s = ReadInstName.ReadLine()) != null)//определяем команду исходя из расшифрованных Opcode и Funct3
                {
                    inst = s.Split("\t");
                    if (inst[1] == DeshInst.Opcode)
                    {
                        if (inst.Length >= 3)
                        {
                            if (inst.Length == 3)
                            {
                                if (inst[2] == DeshInst.Funct3)
                                {
                                    AsmInst.Inst = inst[0];
                                    break;
                                }
                            }
                            else
                            {
                                if (inst[3] == DeshInst.Funct7)
                                {
                                    AsmInst.Inst = inst[0];
                                    break;
                                }
                            }
                        }
                        else
                        {
                            AsmInst.Inst = inst[0];
                            break;
                        }      
                    }
                }
                
                switch (AsmInst.Inst)
                {
                    case "LUI":
                        {
                            AsmInst.Op1 = "x" + Convert.ToString(Convert.ToInt32(Instruction.Substring(20, 5), 2), 10);//RD
                            AsmInst.Op2 = "0x" + Convert.ToString(Convert.ToInt32(Instruction.Substring(0, 20), 2), 16);
                            AsmInst.Full_Comand = AsmInst.Inst + " " + AsmInst.Op1 + ", " + AsmInst.Op2;
                            break;
                        }
                    case "AUIPC":
                        {
                            AsmInst.Op1 = "x" + Convert.ToString(Convert.ToInt32(Instruction.Substring(20, 5), 2), 10);//выделение регистра Rd
                            AsmInst.Op2 = "0x" + Convert.ToString(Convert.ToInt32(Instruction.Substring(0, 20), 2), 16);//imm[0:19]
                            AsmInst.Full_Comand = AsmInst.Inst + " " + AsmInst.Op1 + ", " + AsmInst.Op2;
                            break;
                        }
                    case "JAL":
                        {
                            AsmInst.Op1 = "x" + Convert.ToString(Convert.ToInt32(Instruction.Substring(20, 5), 2), 10);
                            AsmInst.Op2 = Instruction.Substring(0, 20);
                            char[] str1 = AsmInst.Op2.Substring(10,8).ToCharArray();
                            char[] str2 = AsmInst.Op2.Substring(1, 10).ToCharArray();
                            Array.Reverse(str1);
                            Array.Reverse(str2);
                            string out2 = new string(str2);
                            string out1 = new string(str1);

                            AsmInst.Op2 = Instruction.Substring(0, 1) + out2 + Instruction.Substring(10, 1) + out1;
                            str1 = AsmInst.Op2.ToCharArray();
                            Array.Reverse(str1);
                            AsmInst.Op2 = new string(str1);

                            if (AsmInst.Op2.Length == 20)
                            AsmInst.Op2 = "0x" + Convert.ToString(Convert.ToInt32(AsmInst.Op2, 2), 16).ToUpper();
                            AsmInst.Full_Comand = AsmInst.Inst + " " + AsmInst.Op1 + ", " + AsmInst.Op2;
                            break;
                        }
                    case "JALR":
                        {
                            AsmInst.Op1 = "x" + Convert.ToString(Convert.ToInt32(Instruction.Substring(20, 5), 2), 10);
                            AsmInst.Op2 = "x" + Convert.ToString(Convert.ToInt32(Instruction.Substring(12, 5), 2), 10);
                            AsmInst.Op3 = "0x" + Convert.ToString(Convert.ToInt32(Instruction.Substring(0, 12), 2), 16).ToUpper();
                            AsmInst.Full_Comand = AsmInst.Inst + " " + AsmInst.Op1 + ", " + AsmInst.Op2 + ", " + AsmInst.Op3; 
                            break;
                        }
                
                }
                if ((AsmInst.Inst == "BEQ" || AsmInst.Inst == "BNE" || AsmInst.Inst == "BLT" || AsmInst.Inst == "BGE" ||
                    AsmInst.Inst == "BLTU" || AsmInst.Inst == "BGEU") && AsmInst.Full_Comand == "")
                {
                    AsmInst.Op1 = "x" + Convert.ToString(Convert.ToInt32(Instruction.Substring(12, 5), 2), 10);
                    AsmInst.Op2 = "x" + Convert.ToString(Convert.ToInt32(Instruction.Substring(7, 5), 2), 10);
                    char[] str1 = Instruction.Substring(1, 6).ToCharArray();
                    char[] str2 = Instruction.Substring(20, 4).ToCharArray();
                    Array.Reverse(str1);
                    Array.Reverse(str2);
                    string out1 = new string(str1);
                    string out2 = new string(str2);
                    AsmInst.Op3 = Instruction.Substring(0, 1) + out1  + out2 + Instruction.Substring(24, 1);

                    AsmInst.Op3 ="0x" + Convert.ToString(Convert.ToInt32(AsmInst.Op3,2), 16);
                    AsmInst.Full_Comand = AsmInst.Inst + " " + AsmInst.Op1 + ", " + AsmInst.Op2 + ", " + AsmInst.Op3;

                }
                if ((AsmInst.Inst == "LB" || AsmInst.Inst == "LH" || AsmInst.Inst == "LW" || AsmInst.Inst == "LBU" ||
                    AsmInst.Inst == "LHU") && AsmInst.Full_Comand == "")
                {
                    AsmInst.Op1 = "x" + Convert.ToString(Convert.ToInt32(Instruction.Substring(20, 5), 2), 10);//RD
                    AsmInst.Op2 = "x" + Convert.ToString(Convert.ToInt32(Instruction.Substring(12, 5), 2), 10);//RS1
                    AsmInst.Op3 = "0x" + Convert.ToString(Convert.ToInt32(Instruction.Substring(0, 12), 2), 16);//Imm
                    AsmInst.Full_Comand = AsmInst.Inst + " " + AsmInst.Op1 + ", " + AsmInst.Op3 + "(" + AsmInst.Op2 + ")";

                }
                if ((AsmInst.Inst == "SB" || AsmInst.Inst == "SH" || AsmInst.Inst == "SW") && AsmInst.Full_Comand == "")
                {
                    AsmInst.Op1 = "x" + Convert.ToString(Convert.ToInt32(Instruction.Substring(7, 5), 2), 10);//RS1
                    AsmInst.Op2 = "x" + Convert.ToString(Convert.ToInt32(Instruction.Substring(12, 5), 2), 10);//RS2
                    AsmInst.Op3 = Convert.ToString(Convert.ToInt32(Instruction.Substring(20, 5) + Instruction.Substring(0, 7),2),16);
                    AsmInst.Full_Comand = AsmInst.Inst + " " + AsmInst.Op1 + ", " + AsmInst.Op3 + "(" + AsmInst.Op2 + ")";
                }
                if ((AsmInst.Inst == "ADDI" || AsmInst.Inst == "SLTI" || AsmInst.Inst == "SLTIU" || AsmInst.Inst == "XORI" ||
                    AsmInst.Inst == "ORI" || AsmInst.Inst == "ANDI") && AsmInst.Full_Comand == "")
                {
                    AsmInst.Op1 = "x" + Convert.ToString(Convert.ToInt32(Instruction.Substring(20, 5), 2), 10);//RD
                    AsmInst.Op2 = "x" + Convert.ToString(Convert.ToInt32(Instruction.Substring(12, 5), 2), 10);//RS1
                    AsmInst.Op3 = Convert.ToString(Convert.ToInt32(Instruction.Substring(0, 12), 2), 16);//Imm
                    AsmInst.Full_Comand = AsmInst.Inst + " " + AsmInst.Op1 + ", " + AsmInst.Op2 + ", " + AsmInst.Op3;
                }
                if((AsmInst.Inst == "SLLI" || AsmInst.Inst == "SRLI" || AsmInst.Inst == "SRAI") && AsmInst.Full_Comand == "")
                {
                    AsmInst.Op1 = "x" + Convert.ToString(Convert.ToInt32(Instruction.Substring(20, 5), 2), 10);//RD
                    AsmInst.Op2 = "x" + Convert.ToString(Convert.ToInt32(Instruction.Substring(12, 5), 2), 10);//RS1
                    AsmInst.Op3 = Convert.ToString(Convert.ToInt32(Instruction.Substring(7, 5), 2), 16);//shamt
                    AsmInst.Full_Comand = AsmInst.Inst + " " + AsmInst.Op1 + ", " + AsmInst.Op2 + ", " + AsmInst.Op3;

                }
                if ((AsmInst.Inst == "ADD" || AsmInst.Inst == "SUB" || AsmInst.Inst == "SLL" || AsmInst.Inst == "SLT" ||
                    AsmInst.Inst == "SLTU" || AsmInst.Inst == "XOR" || AsmInst.Inst == "SRL" || AsmInst.Inst == "SRA" ||
                    AsmInst.Inst == "OR" || AsmInst.Inst == "AND") && AsmInst.Full_Comand == "")
                {
                    AsmInst.Op1 = "x" + Convert.ToString(Convert.ToInt32(Instruction.Substring(20, 5), 2), 10);//RD
                    AsmInst.Op2 = "x" + Convert.ToString(Convert.ToInt32(Instruction.Substring(12, 5), 2), 10);//RS1
                    AsmInst.Op3 = "x" + Convert.ToString(Convert.ToInt32(Instruction.Substring(7, 5), 2), 10);//RS2
                    AsmInst.Full_Comand = AsmInst.Inst + " " + AsmInst.Op1 + ", " + AsmInst.Op2 + ", " + AsmInst.Op3;
                }

            }
            return AsmInst.Full_Comand;
        }

    }
}
