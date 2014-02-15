using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Code
{
    /************************************************************************
   *              proctab.cpp                                              *
   * Tables of processor instructions. The whole of this file is full of   *
   * tables that I constructed some time ago. The main instruction tables  *
   * include three arguments for instructions, which was necessary to      *
   * easily include instructions like shld ms/m16,r16,CL. I was maybe       *
   * overexcessive with the number of slightly different modr/ms that I    *
   * included but the tables have served well, and when you build them up  *
   * from scratch instruction by instruction you do actually find adressOfCall lot of *
   * slightly different encodings, and the modrm encodings generally       *
   * indicate what is actually being referenced (like adressOfCall 16:32 pointer in   *
   * memory, etc). I included the Z80 processor because I knew the Z80     *
   * processor quite well, but mostly just to show it isnt hard to do.     *
   * At one point I decided that I needed adressOfCall uid for each instruction to    *
   * enable proper reconstruction of the disassembly from adressOfCall saved database *
   * and so each instruction also has adressOfCall uid for this purpose.              *
   * Flags for modrm may seem excessive, but consider encodings like 0fae  *
   * which includes the strange sfence instruction with no arguments and adressOfCall *
   * code of 0fae /7, along with stmxcsr m32 with the encoding 0fae /3     *
   * Hence the flag FLAGS_MODRM indicates adressOfCall modrm byte. Any argument       *
   * encodings may indicate usage of that byte, but may not be present at  *
   * all.                                                                  *
   * Whilst I havent found any errors in here for some time it would need  *
   * adressOfCall very careful analysis to find any which may be present. Certainly   *
   * newer instruction encodings (mmx, kni) may need more work, and it is  *
   * worth looking at decodings which should not be possible (eg forced    *
   * memory operands only may require looking at as some possible          *
   * instructions are illegal)                                             *
   ************************************************************************/

    //processor types
    public enum Proc
    {
        PROC_8086 = 0x0001,
        PROC_80286 = 0x0002,
        PROC_80386 = 0x0004,
        PROC_80486 = 0x0008,
        PROC_PENTIUM = 0x0010,
        PROC_PENTMMX = 0x0020,
        PROC_PENTIUM2 = 0x0080,
        PROC_Z80 = 0x0100,
        PROC_PENTIUMPRO = 0x0200,
        PROC_ALL = 0xffff,
        PROC_FROM8086,
        PROC_FROM80286,
        PROC_FROM80386,
        PROC_FROM80486,
        PROC_FROMPENTIUM, PROC_FROMPENTPRO, PROC_FROMPENTMMX, PROC_FROMPENTIUM2
    }

    /*processor macros
        enum prog_mac{
    Proc.PROC_FROMPENTIUM2 =Proc.PROC_PENTIUM2
    , Proc.PROC_FROMPENTMMX= Proc.PROC_PENTMMX|Proc.PROC_PENTIUM2
    , Proc.PROC_FROMPENTPRO= Proc.PROC_PENTIUMPRO|Proc.PROC_FROMPENTMMX
    , Proc.PROC_FROMPENTIUM= Proc.PROC_PENTIUM|Proc.PROC_FROMPENTPRO
    , Proc.PROC_FROM80486 =  Proc.PROC_80486|Proc.PROC_FROMPENTIUM
    , Proc.PROC_FROM80386 =  Proc.PROC_80386|Proc.PROC_FROM80486
    , Proc.PROC_FROM80286  = Proc.PROC_80286|Proc.PROC_FROM80386
    , Proc.PROC_FROM8086  =  Proc.PROC_8086|Proc.PROC_FROM80286}*/

    public enum flags
    {
        FLAGS_MODRM = 0x00001  //contains mod ms/m byte
        ,
        FLAGS_8BIT = 0x00002  //force 8-bit arguments
            ,
        FLAGS_16BIT = 0x00004  //force 16-bit arguments
            ,
        FLAGS_32BIT = 0x00008  //force 32-bit arguments
            ,
        FLAGS_REAL = 0x00010  //real mode only
            ,
        FLAGS_PMODE = 0x00020  //protected mode only
            ,
        FLAGS_PREFIX = 0x00040  //for lock and rep prefix
            ,
        FLAGS_MMX = 0x00080  //mmx instruction/registers
            ,
        FLAGS_FPU = 0x00100  //fpu instruction/registers
            ,
        FLAGS_CJMP = 0x00200  //codeflow - conditional jump
            ,
        FLAGS_JMP = 0x00400  //codeflow - jump
            ,
        FLAGS_IJMP = 0x00800  //codeflow - indexed jump
            ,
        FLAGS_CALL = 0x01000  //codeflow - call
            ,
        FLAGS_ICALL = 0x02000  //codeflow - indexed call
            ,
        FLAGS_RET = 0x04000  //codeflow - return
            ,
        FLAGS_SEGPREFIX = 0x08000  //segment prefix
            ,
        FLAGS_OPERPREFIX = 0x10000  //immediateOperand prefix
            ,
        FLAGS_ADDRPREFIX = 0x20000  //startAddress prefix
            ,
        FLAGS_OMODE16 = 0x40000  //16-bit immediateOperand mode only
            ,
        FLAGS_OMODE32 = 0x80000  //32-bit immediateOperand mode only

            //Z80 Flags
            , FLAGS_INDEXREG = 0x00001
    }
    public enum argtype
    {
        ARG_REG = 1, ARG_IMM, ARG_NONE, ARG_MODRM, ARG_REG_AX, ARG_REG_ES, ARG_REG_CS,
        ARG_REG_SS, ARG_REG_DS, ARG_REG_FS, ARG_REG_GS, ARG_REG_BX, ARG_REG_CX, ARG_REG_DX,
        ARG_REG_SP, ARG_REG_BP, ARG_REG_SI, ARG_REG_DI, ARG_IMM8, ARG_RELIMM8, ARG_FADDR, ARG_REG_AL,
        ARG_MEMLOC, ARG_SREG, ARG_RELIMM, ARG_16REG_DX, ARG_REG_CL, ARG_REG_DL, ARG_REG_BL, ARG_REG_AH,
        ARG_REG_CH, ARG_REG_DH, ARG_REG_BH, ARG_MODREG, ARG_CREG, ARG_DREG, ARG_TREG_67, ARG_TREG,
        ARG_MREG, ARG_MMXMODRM, ARG_MODRM8, ARG_IMM_1, ARG_MODRM_FPTR, ARG_MODRM_S, ARG_MODRMM512,
        ARG_MODRMQ, ARG_MODRM_SREAL, ARG_REG_ST0, ARG_FREG, ARG_MODRM_PTR, ARG_MODRM_short, ARG_MODRM_SINT,
        ARG_MODRM_EREAL, ARG_MODRM_DREAL, ARG_MODRM_WINT, ARG_MODRM_LINT, ARG_REG_BC, ARG_REG_DE,
        ARG_REG_HL, ARG_REG_DE_IND, ARG_REG_HL_IND, ARG_REG_BC_IND, ARG_REG_SP_IND, ARG_REG_A,
        ARG_REG_B, ARG_REG_C, ARG_REG_D, ARG_REG_E, ARG_REG_H, ARG_REG_L, ARG_IMM16, ARG_REG_AF,
        ARG_REG_AF2, ARG_MEMLOC16, ARG_IMM8_IND, ARG_BIT, ARG_REG_IX, ARG_REG_IX_IND, ARG_REG_IY,
        ARG_REG_IY_IND, ARG_REG_C_IND, ARG_REG_I, ARG_REG_R, ARG_IMM16_A, ARG_MODRM16, ARG_SIMM8,
        ARG_IMM32, ARG_STRING, ARG_MODRM_BCD, ARG_PSTRING, ARG_DOSSTRING, ARG_CUNICODESTRING,
        ARG_PUNICODESTRING, ARG_NONEBYTE, ARG_XREG, ARG_XMMMODRM, ARG_IMM_SINGLE, ARG_IMM_DOUBLE,
        ARG_IMM_LONGDOUBLE
    }
    public class asminstdata         //Asm Instructions userStrings
    {
        string name;              //eg nop,null=subtable/undefined
        byte instbyte;           //   0x90/subtable number
        Proc processor;          //   8086|386|486|pentium,etc bitwise Flags
        flags flags;             //   mod ms/m,8/16/32 bit
        argtype arg1, arg2, arg3;  //   argtypes=reg/none/immediate,etc
        int uniqueid;          //   unique id for reconstructing saved databases
        public asminstdata(string n, byte op, Proc proc, flags f, argtype ar1, argtype ar2, argtype ar3, int id)
        {
            name = n;
            instbyte = op;
            processor = proc;
            flags = f;
            arg1 = ar1;
            arg2 = ar2;
            arg3 = ar3;
            uniqueid = id;
        }
    }
    public class asmtable            //Assembly instruction tables
    {
        asminstdata[] table;      //Pointer to table of instruction encodings
        byte type;               // type - main table/extension
        byte extnum, extnum2;     // bytes= first bytes of instruction
        byte divisor;            // number to divide by for look up
        byte mask;               // bit mask for look up
        byte minlim, maxlim;      // limits on min/max entries.
        byte modrmpos;           // modrm byte PositionOfStructureInFile plus
        public asmtable(asminstdata[] tab, byte t, byte ext, byte ext2, byte div, byte mask, byte min, byte max, byte modr)
        {
            table = tab;
            type = t;
            extnum = ext;
            extnum2 = ext2;
            divisor = div;
            this.mask = mask;
            minlim = min;
            maxlim = max;
            modrmpos = modr;
        }
    }
    public class proctable
    {
        int num;
        string name;
        asmtable[] tab;
        public proctable(Proc p, string na, asmtable[] asm)
        {
            num = (int)p;
            name = na;
            tab = asm;
        }
    }
    public class laclasse
    {
        public static proctable[] procnames = new proctable[]            { new proctable(Proc.PROC_8086,"8086",laclasse.tables86),
 new proctable (Proc.PROC_80286,"80286",tables86),
 new proctable (Proc.PROC_80386,"80386",tables86),
new proctable  (Proc.PROC_80486,"80486",tables86),
new proctable  (Proc.PROC_PENTIUM,"Pentium",tables86),
new proctable  (Proc.PROC_PENTIUMPRO,"Pentium Pro",tables86),
new proctable  (Proc.PROC_PENTMMX,"Pentium MMX",tables86),
new proctable  (Proc.PROC_PENTIUM2,"Pentium II with KNI",tables86),
//new proctable  (Proc.PROC_Z80,"Z-80",tablesz80),
 new proctable (0,"",null)
        };

        string[] reg32ascii = new string[] { "eax", "ecx", "edx", "ebx", "esp", "ebp", "esi", "edi" };
        string[] reg16ascii = new string[] { "ax", "cx", "dx", "bx", "sp", "bp", "si", "di" };
        string[] reg8ascii = new string[] { "al", "cl", "dl", "bl", "ah", "ch", "dh", "bh" };
        string[] regind16ascii = new string[] { "bx+si", "bx+di", "bp+si", "bp+di", "si", "di", "bp", "bx" };
        string[] regfascii = new string[] { "st(0)", "st(1)", "st(2)", "st(3)", "st(4)", "st(5)", "st(6)", "st(7)" };
        string[] regmascii = new string[] { "mm0", "mm1", "mm2", "mm3", "mm4", "mm5", "mm6", "mm7" };
        string[] regxascii = new string[] { "xmm0", "xmm1", "xmm2", "xmm3", "xmm4", "xmm5", "xmm6", "xmm7" };
        string[] regsascii = new string[] { "es", "cs", "ss", "ds", "fs", "gs", "??", "??" };
        string[] regcascii = new string[] { "cr0", "cr1", "cr2", "cr3", "cr4", "cr5", "cr6", "cr7" };
        string[] regdascii = new string[] { "dr0", "dr1", "dr2", "dr3", "dr4", "dr5", "dr6", "dr7" };
        string[] regtascii = new string[] { "tr0", "tr1", "tr2", "tr3", "tr4", "tr5", "tr6", "tr7" };
        string[] regzascii = new string[] { "b", "c", "d", "e", "h", "l", "(hl)", "a" };

        // to be filled in.
        public static asminstdata[] asm86 = new asminstdata[]
{ new asminstdata("add",0x00,Proc.PROC_FROM8086,flags.FLAGS_MODRM|flags.FLAGS_8BIT,argtype.ARG_MODRM,argtype.ARG_REG,argtype.ARG_NONE,1),
  new asminstdata("add",0x01,Proc.PROC_FROM8086,flags.FLAGS_MODRM|flags.FLAGS_OMODE16,argtype.ARG_MODRM,argtype.ARG_REG,argtype.ARG_NONE,2),
  new asminstdata("add",0x01,Proc.PROC_FROM80386,flags.FLAGS_MODRM|flags.FLAGS_OMODE32,argtype.ARG_MODRM,argtype.ARG_REG,argtype.ARG_NONE,3),
  new asminstdata("add",0x02,Proc.PROC_FROM8086,flags.FLAGS_MODRM|flags.FLAGS_8BIT,argtype.ARG_REG,argtype.ARG_MODRM,argtype.ARG_NONE,4),
  new asminstdata("add",0x03,Proc.PROC_FROM8086,flags.FLAGS_MODRM|flags.FLAGS_OMODE16,argtype.ARG_REG,argtype.ARG_MODRM,argtype.ARG_NONE,5),
  new asminstdata("add",0x03,Proc.PROC_FROM80386,flags.FLAGS_MODRM|flags.FLAGS_OMODE32,argtype.ARG_REG,argtype.ARG_MODRM,argtype.ARG_NONE,6),
  new asminstdata("add",0x04,Proc.PROC_FROM8086,flags.FLAGS_8BIT,argtype.ARG_REG_AL,argtype.ARG_IMM8,argtype.ARG_NONE,7),
  new asminstdata("add",0x05,Proc.PROC_FROM8086,flags.FLAGS_OMODE16,argtype.ARG_REG_AX,argtype.ARG_IMM,argtype.ARG_NONE,8),
  new asminstdata("add",0x05,Proc.PROC_FROM80386,flags.FLAGS_OMODE32,argtype.ARG_REG_AX,argtype.ARG_IMM,argtype.ARG_NONE,9),
  new asminstdata("push",0x06,Proc.PROC_FROM8086,0,argtype.ARG_REG_ES,argtype.ARG_NONE,argtype.ARG_NONE,10),
  new asminstdata("pop",0x07,Proc.PROC_FROM8086,0,argtype.ARG_REG_ES,argtype.ARG_NONE,argtype.ARG_NONE,11),
  new asminstdata("or",0x08,Proc.PROC_FROM8086,flags.FLAGS_MODRM|flags.FLAGS_8BIT,argtype.ARG_MODRM,argtype.ARG_REG,argtype.ARG_NONE,12),
  new asminstdata("or",0x09,Proc.PROC_FROM8086,flags.FLAGS_MODRM|flags.FLAGS_OMODE16,argtype.ARG_MODRM,argtype.ARG_REG,argtype.ARG_NONE,13),
  new asminstdata("or",0x09,Proc.PROC_FROM80386,flags.FLAGS_MODRM|flags.FLAGS_OMODE32,argtype.ARG_MODRM,argtype.ARG_REG,argtype.ARG_NONE,14),
  new asminstdata("or",0x0a,Proc.PROC_FROM8086,flags.FLAGS_MODRM|flags.FLAGS_8BIT,argtype.ARG_REG,argtype.ARG_MODRM,argtype.ARG_NONE,15),
  new asminstdata("or",0x0b,Proc.PROC_FROM8086,flags.FLAGS_MODRM|flags.FLAGS_OMODE16,argtype.ARG_REG,argtype.ARG_MODRM,argtype.ARG_NONE,16),
  new asminstdata("or",0x0b,Proc.PROC_FROM80386,flags.FLAGS_MODRM|flags.FLAGS_OMODE32,argtype.ARG_REG,argtype.ARG_MODRM,argtype.ARG_NONE,17),
  new asminstdata("or",0x0c,Proc.PROC_FROM8086,flags.FLAGS_8BIT,argtype.ARG_REG_AL,argtype.ARG_IMM8,argtype.ARG_NONE,18),
  new asminstdata("or",0x0d,Proc.PROC_FROM8086,flags.FLAGS_OMODE16,argtype.ARG_REG_AX,argtype.ARG_IMM,argtype.ARG_NONE,19),
  new asminstdata("or",0x0d,Proc.PROC_FROM80386,flags.FLAGS_OMODE32,argtype.ARG_REG_AX,argtype.ARG_IMM,argtype.ARG_NONE,20),
  new asminstdata("push",0x0e,Proc.PROC_FROM8086,0,argtype.ARG_REG_CS,argtype.ARG_NONE,argtype.ARG_NONE,21),
  new asminstdata(null,0x0f,Proc.PROC_FROM8086,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,22),  //subtable 0x0f
  new asminstdata("adc",0x10,Proc.PROC_FROM8086,flags.FLAGS_MODRM|flags.FLAGS_8BIT,argtype.ARG_MODRM,argtype.ARG_REG,argtype.ARG_NONE,23),
  new asminstdata("adc",0x11,Proc.PROC_FROM8086,flags.FLAGS_MODRM|flags.FLAGS_OMODE16,argtype.ARG_MODRM,argtype.ARG_REG,argtype.ARG_NONE,24),
  new asminstdata("adc",0x11,Proc.PROC_FROM80386,flags.FLAGS_MODRM|flags.FLAGS_OMODE32,argtype.ARG_MODRM,argtype.ARG_REG,argtype.ARG_NONE,25),
  new asminstdata("adc",0x12,Proc.PROC_FROM8086,flags.FLAGS_MODRM|flags.FLAGS_8BIT,argtype.ARG_REG,argtype.ARG_MODRM,argtype.ARG_NONE,26),
  new asminstdata("adc",0x13,Proc.PROC_FROM8086,flags.FLAGS_MODRM|flags.FLAGS_OMODE16,argtype.ARG_REG,argtype.ARG_MODRM,argtype.ARG_NONE,27),
  new asminstdata("adc",0x13,Proc.PROC_FROM80386,flags.FLAGS_MODRM|flags.FLAGS_OMODE32,argtype.ARG_REG,argtype.ARG_MODRM,argtype.ARG_NONE,28),
  new asminstdata("adc",0x14,Proc.PROC_FROM8086,flags.FLAGS_8BIT,argtype.ARG_REG_AL,argtype.ARG_IMM8,argtype.ARG_NONE,29),
  new asminstdata("adc",0x15,Proc.PROC_FROM8086,flags.FLAGS_OMODE16,argtype.ARG_REG_AX,argtype.ARG_IMM,argtype.ARG_NONE,30),
  new asminstdata("adc",0x15,Proc.PROC_FROM80386,flags.FLAGS_OMODE32,argtype.ARG_REG_AX,argtype.ARG_IMM,argtype.ARG_NONE,31),
  new asminstdata("push",0x16,Proc.PROC_FROM8086,0,argtype.ARG_REG_SS,argtype.ARG_NONE,argtype.ARG_NONE,32),
  new asminstdata("pop",0x17,Proc.PROC_FROM8086,0,argtype.ARG_REG_SS,argtype.ARG_NONE,argtype.ARG_NONE,33),
  new asminstdata("sbb",0x18,Proc.PROC_FROM8086,flags.FLAGS_MODRM|flags.FLAGS_8BIT,argtype.ARG_MODRM,argtype.ARG_REG,argtype.ARG_NONE,34),
  new asminstdata("sbb",0x19,Proc.PROC_FROM8086,flags.FLAGS_MODRM|flags.FLAGS_OMODE16,argtype.ARG_MODRM,argtype.ARG_REG,argtype.ARG_NONE,35),
  new asminstdata("sbb",0x19,Proc.PROC_FROM80386,flags.FLAGS_MODRM|flags.FLAGS_OMODE32,argtype.ARG_MODRM,argtype.ARG_REG,argtype.ARG_NONE,36),
  new asminstdata("sbb",0x1a,Proc.PROC_FROM8086,flags.FLAGS_MODRM|flags.FLAGS_8BIT,argtype.ARG_REG,argtype.ARG_MODRM,argtype.ARG_NONE,37),
  new asminstdata("sbb",0x1b,Proc.PROC_FROM8086,flags.FLAGS_MODRM|flags.FLAGS_OMODE16,argtype.ARG_REG,argtype.ARG_MODRM,argtype.ARG_NONE,38),
  new asminstdata("sbb",0x1b,Proc.PROC_FROM80386,flags.FLAGS_MODRM|flags.FLAGS_OMODE32,argtype.ARG_REG,argtype.ARG_MODRM,argtype.ARG_NONE,39),
  new asminstdata("sbb",0x1c,Proc.PROC_FROM8086,flags.FLAGS_8BIT,argtype.ARG_REG_AL,argtype.ARG_IMM8,argtype.ARG_NONE,40),
  new asminstdata("sbb",0x1d,Proc.PROC_FROM8086,flags.FLAGS_OMODE16,argtype.ARG_REG_AX,argtype.ARG_IMM,argtype.ARG_NONE,41),
  new asminstdata("sbb",0x1d,Proc.PROC_FROM80386,flags.FLAGS_OMODE32,argtype.ARG_REG_AX,argtype.ARG_IMM,argtype.ARG_NONE,42),
  new asminstdata("push",0x1e,Proc.PROC_FROM8086,0,argtype.ARG_REG_DS,argtype.ARG_NONE,argtype.ARG_NONE,43),
  new asminstdata("pop",0x1f,Proc.PROC_FROM8086,0,argtype.ARG_REG_DS,argtype.ARG_NONE,argtype.ARG_NONE,44),
  new asminstdata("and",0x20,Proc.PROC_FROM8086,flags.FLAGS_MODRM|flags.FLAGS_8BIT,argtype.ARG_MODRM,argtype.ARG_REG,argtype.ARG_NONE,45),
  new asminstdata("and",0x21,Proc.PROC_FROM8086,flags.FLAGS_MODRM|flags.FLAGS_OMODE16,argtype.ARG_MODRM,argtype.ARG_REG,argtype.ARG_NONE,46),
  new asminstdata("and",0x21,Proc.PROC_FROM80386,flags.FLAGS_MODRM|flags.FLAGS_OMODE32,argtype.ARG_MODRM,argtype.ARG_REG,argtype.ARG_NONE,47),
  new asminstdata("and",0x22,Proc.PROC_FROM8086,flags.FLAGS_MODRM|flags.FLAGS_8BIT,argtype.ARG_REG,argtype.ARG_MODRM,argtype.ARG_NONE,48),
  new asminstdata("and",0x23,Proc.PROC_FROM8086,flags.FLAGS_MODRM|flags.FLAGS_OMODE16,argtype.ARG_REG,argtype.ARG_MODRM,argtype.ARG_NONE,49),
  new asminstdata("and",0x23,Proc.PROC_FROM80386,flags.FLAGS_MODRM|flags.FLAGS_OMODE32,argtype.ARG_REG,argtype.ARG_MODRM,argtype.ARG_NONE,50),
  new asminstdata("and",0x24,Proc.PROC_FROM8086,flags.FLAGS_8BIT,argtype.ARG_REG_AL,argtype.ARG_IMM8,argtype.ARG_NONE,51),
  new asminstdata("and",0x25,Proc.PROC_FROM8086,flags.FLAGS_OMODE16,argtype.ARG_REG_AX,argtype.ARG_IMM,argtype.ARG_NONE,52),
  new asminstdata("and",0x25,Proc.PROC_FROM80386,flags.FLAGS_OMODE32,argtype.ARG_REG_AX,argtype.ARG_IMM,argtype.ARG_NONE,53),
  new asminstdata("es:",0x26,Proc.PROC_FROM8086,flags.FLAGS_SEGPREFIX,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,54),
  new asminstdata("daa",0x27,Proc.PROC_FROM8086,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,55),
  new asminstdata("sub",0x28,Proc.PROC_FROM8086,flags.FLAGS_MODRM|flags.FLAGS_8BIT,argtype.ARG_MODRM,argtype.ARG_REG,argtype.ARG_NONE,56),
  new asminstdata("sub",0x29,Proc.PROC_FROM8086,flags.FLAGS_MODRM|flags.FLAGS_OMODE16,argtype.ARG_MODRM,argtype.ARG_REG,argtype.ARG_NONE,57),
  new asminstdata("sub",0x29,Proc.PROC_FROM80386,flags.FLAGS_MODRM|flags.FLAGS_OMODE32,argtype.ARG_MODRM,argtype.ARG_REG,argtype.ARG_NONE,58),
  new asminstdata("sub",0x2a,Proc.PROC_FROM8086,flags.FLAGS_MODRM|flags.FLAGS_8BIT,argtype.ARG_REG,argtype.ARG_MODRM,argtype.ARG_NONE,59),
  new asminstdata("sub",0x2b,Proc.PROC_FROM8086,flags.FLAGS_MODRM|flags.FLAGS_OMODE16,argtype.ARG_REG,argtype.ARG_MODRM,argtype.ARG_NONE,60),
  new asminstdata("sub",0x2b,Proc.PROC_FROM80386,flags.FLAGS_MODRM|flags.FLAGS_OMODE32,argtype.ARG_REG,argtype.ARG_MODRM,argtype.ARG_NONE,61),
  new asminstdata("sub",0x2c,Proc.PROC_FROM8086,flags.FLAGS_8BIT,argtype.ARG_REG_AL,argtype.ARG_IMM8,argtype.ARG_NONE,62),
  new asminstdata("sub",0x2d,Proc.PROC_FROM8086,flags.FLAGS_OMODE16,argtype.ARG_REG_AX,argtype.ARG_IMM,argtype.ARG_NONE,63),
  new asminstdata("sub",0x2d,Proc.PROC_FROM80386,flags.FLAGS_OMODE32,argtype.ARG_REG_AX,argtype.ARG_IMM,argtype.ARG_NONE,64),
  new asminstdata("cs:",0x2e,Proc.PROC_FROM8086,flags.FLAGS_SEGPREFIX,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,65),
  new asminstdata("das",0x2f,Proc.PROC_FROM8086,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,66),
  new asminstdata("xor",0x30,Proc.PROC_FROM8086,flags.FLAGS_MODRM|flags.FLAGS_8BIT,argtype.ARG_MODRM,argtype.ARG_REG,argtype.ARG_NONE,67),
  new asminstdata("xor",0x31,Proc.PROC_FROM8086,flags.FLAGS_MODRM|flags.FLAGS_OMODE16,argtype.ARG_MODRM,argtype.ARG_REG,argtype.ARG_NONE,68),
  new asminstdata("xor",0x31,Proc.PROC_FROM80386,flags.FLAGS_MODRM|flags.FLAGS_OMODE32,argtype.ARG_MODRM,argtype.ARG_REG,argtype.ARG_NONE,69),
  new asminstdata("xor",0x32,Proc.PROC_FROM8086,flags.FLAGS_MODRM|flags.FLAGS_8BIT,argtype.ARG_REG,argtype.ARG_MODRM,argtype.ARG_NONE,70),
  new asminstdata("xor",0x33,Proc.PROC_FROM8086,flags.FLAGS_MODRM|flags.FLAGS_OMODE16,argtype.ARG_REG,argtype.ARG_MODRM,argtype.ARG_NONE,71),
  new asminstdata("xor",0x33,Proc.PROC_FROM80386,flags.FLAGS_MODRM|flags.FLAGS_OMODE32,argtype.ARG_REG,argtype.ARG_MODRM,argtype.ARG_NONE,72),
  new asminstdata("xor",0x34,Proc.PROC_FROM8086,flags.FLAGS_8BIT,argtype.ARG_REG_AL,argtype.ARG_IMM8,argtype.ARG_NONE,73),
  new asminstdata("xor",0x35,Proc.PROC_FROM8086,flags.FLAGS_OMODE16,argtype.ARG_REG_AX,argtype.ARG_IMM,argtype.ARG_NONE,74),
  new asminstdata("xor",0x35,Proc.PROC_FROM80386,flags.FLAGS_OMODE32,argtype.ARG_REG_AX,argtype.ARG_IMM,argtype.ARG_NONE,75),
  new asminstdata("ss:",0x36,Proc.PROC_FROM8086,flags.FLAGS_SEGPREFIX,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,76),
  new asminstdata("aaa",0x37,Proc.PROC_FROM8086,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,77),
  new asminstdata("cmp",0x38,Proc.PROC_FROM8086,flags.FLAGS_MODRM|flags.FLAGS_8BIT,argtype.ARG_MODRM,argtype.ARG_REG,argtype.ARG_NONE,78),
  new asminstdata("cmp",0x39,Proc.PROC_FROM8086,flags.FLAGS_MODRM|flags.FLAGS_OMODE16,argtype.ARG_MODRM,argtype.ARG_REG,argtype.ARG_NONE,79),
  new asminstdata("cmp",0x39,Proc.PROC_FROM80386,flags.FLAGS_MODRM|flags.FLAGS_OMODE32,argtype.ARG_MODRM,argtype.ARG_REG,argtype.ARG_NONE,80),
  new asminstdata("cmp",0x3a,Proc.PROC_FROM8086,flags.FLAGS_MODRM|flags.FLAGS_8BIT,argtype.ARG_REG,argtype.ARG_MODRM,argtype.ARG_NONE,81),
  new asminstdata("cmp",0x3b,Proc.PROC_FROM8086,flags.FLAGS_MODRM|flags.FLAGS_OMODE16,argtype.ARG_REG,argtype.ARG_MODRM,argtype.ARG_NONE,82),
  new asminstdata("cmp",0x3b,Proc.PROC_FROM80386,flags.FLAGS_MODRM|flags.FLAGS_OMODE32,argtype.ARG_REG,argtype.ARG_MODRM,argtype.ARG_NONE,83),
  new asminstdata("cmp",0x3c,Proc.PROC_FROM8086,flags.FLAGS_8BIT,argtype.ARG_REG_AL,argtype.ARG_IMM8,argtype.ARG_NONE,84),
  new asminstdata("cmp",0x3d,Proc.PROC_FROM8086,flags.FLAGS_OMODE16,argtype.ARG_REG_AX,argtype.ARG_IMM,argtype.ARG_NONE,85),
  new asminstdata("cmp",0x3d,Proc.PROC_FROM80386,flags.FLAGS_OMODE32,argtype.ARG_REG_AX,argtype.ARG_IMM,argtype.ARG_NONE,86),
  new asminstdata("ds:",0x3e,Proc.PROC_FROM8086,flags.FLAGS_SEGPREFIX,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,87),
  new asminstdata("aas",0x3f,Proc.PROC_FROM8086,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,88),
  new asminstdata("inc",0x40,Proc.PROC_FROM8086,flags.FLAGS_OMODE16,argtype.ARG_REG_AX,argtype.ARG_NONE,argtype.ARG_NONE,89),
  new asminstdata("inc",0x40,Proc.PROC_FROM80386,flags.FLAGS_OMODE32,argtype.ARG_REG_AX,argtype.ARG_NONE,argtype.ARG_NONE,90),
  new asminstdata("inc",0x41,Proc.PROC_FROM8086,flags.FLAGS_OMODE16,argtype.ARG_REG_CX,argtype.ARG_NONE,argtype.ARG_NONE,91),
  new asminstdata("inc",0x41,Proc.PROC_FROM80386,flags.FLAGS_OMODE32,argtype.ARG_REG_CX,argtype.ARG_NONE,argtype.ARG_NONE,92),
  new asminstdata("inc",0x42,Proc.PROC_FROM8086,flags.FLAGS_OMODE16,argtype.ARG_REG_DX,argtype.ARG_NONE,argtype.ARG_NONE,93),
  new asminstdata("inc",0x42,Proc.PROC_FROM80386,flags.FLAGS_OMODE32,argtype.ARG_REG_DX,argtype.ARG_NONE,argtype.ARG_NONE,94),
  new asminstdata("inc",0x43,Proc.PROC_FROM8086,flags.FLAGS_OMODE16,argtype.ARG_REG_BX,argtype.ARG_NONE,argtype.ARG_NONE,95),
  new asminstdata("inc",0x43,Proc.PROC_FROM80386,flags.FLAGS_OMODE32,argtype.ARG_REG_BX,argtype.ARG_NONE,argtype.ARG_NONE,96),
  new asminstdata("inc",0x44,Proc.PROC_FROM8086,flags.FLAGS_OMODE16,argtype.ARG_REG_SP,argtype.ARG_NONE,argtype.ARG_NONE,97),
  new asminstdata("inc",0x44,Proc.PROC_FROM80386,flags.FLAGS_OMODE32,argtype.ARG_REG_SP,argtype.ARG_NONE,argtype.ARG_NONE,98),
  new asminstdata("inc",0x45,Proc.PROC_FROM8086,flags.FLAGS_OMODE16,argtype.ARG_REG_BP,argtype.ARG_NONE,argtype.ARG_NONE,99),
  new asminstdata("inc",0x45,Proc.PROC_FROM80386,flags.FLAGS_OMODE32,argtype.ARG_REG_BP,argtype.ARG_NONE,argtype.ARG_NONE,100),
  new asminstdata("inc",0x46,Proc.PROC_FROM8086,flags.FLAGS_OMODE16,argtype.ARG_REG_SI,argtype.ARG_NONE,argtype.ARG_NONE,101),
  new asminstdata("inc",0x46,Proc.PROC_FROM80386,flags.FLAGS_OMODE32,argtype.ARG_REG_SI,argtype.ARG_NONE,argtype.ARG_NONE,102),
  new asminstdata("inc",0x47,Proc.PROC_FROM8086,flags.FLAGS_OMODE16,argtype.ARG_REG_DI,argtype.ARG_NONE,argtype.ARG_NONE,103),
  new asminstdata("inc",0x47,Proc.PROC_FROM80386,flags.FLAGS_OMODE32,argtype.ARG_REG_DI,argtype.ARG_NONE,argtype.ARG_NONE,104),
  new asminstdata("dec",0x48,Proc.PROC_FROM8086,flags.FLAGS_OMODE16,argtype.ARG_REG_AX,argtype.ARG_NONE,argtype.ARG_NONE,105),
  new asminstdata("dec",0x48,Proc.PROC_FROM80386,flags.FLAGS_OMODE32,argtype.ARG_REG_AX,argtype.ARG_NONE,argtype.ARG_NONE,106),
  new asminstdata("dec",0x49,Proc.PROC_FROM8086,flags.FLAGS_OMODE16,argtype.ARG_REG_CX,argtype.ARG_NONE,argtype.ARG_NONE,107),
  new asminstdata("dec",0x49,Proc.PROC_FROM80386,flags.FLAGS_OMODE32,argtype.ARG_REG_CX,argtype.ARG_NONE,argtype.ARG_NONE,108),
  new asminstdata("dec",0x4a,Proc.PROC_FROM8086,flags.FLAGS_OMODE16,argtype.ARG_REG_DX,argtype.ARG_NONE,argtype.ARG_NONE,109),
  new asminstdata("dec",0x4a,Proc.PROC_FROM80386,flags.FLAGS_OMODE32,argtype.ARG_REG_DX,argtype.ARG_NONE,argtype.ARG_NONE,110),
  new asminstdata("dec",0x4b,Proc.PROC_FROM8086,flags.FLAGS_OMODE16,argtype.ARG_REG_BX,argtype.ARG_NONE,argtype.ARG_NONE,111),
  new asminstdata("dec",0x4b,Proc.PROC_FROM80386,flags.FLAGS_OMODE32,argtype.ARG_REG_BX,argtype.ARG_NONE,argtype.ARG_NONE,112),
  new asminstdata("dec",0x4c,Proc.PROC_FROM8086,flags.FLAGS_OMODE16,argtype.ARG_REG_SP,argtype.ARG_NONE,argtype.ARG_NONE,113),
  new asminstdata("dec",0x4c,Proc.PROC_FROM80386,flags.FLAGS_OMODE32,argtype.ARG_REG_SP,argtype.ARG_NONE,argtype.ARG_NONE,114),
  new asminstdata("dec",0x4d,Proc.PROC_FROM8086,flags.FLAGS_OMODE16,argtype.ARG_REG_BP,argtype.ARG_NONE,argtype.ARG_NONE,115),
  new asminstdata("dec",0x4d,Proc.PROC_FROM80386,flags.FLAGS_OMODE32,argtype.ARG_REG_BP,argtype.ARG_NONE,argtype.ARG_NONE,116),
  new asminstdata("dec",0x4e,Proc.PROC_FROM8086,flags.FLAGS_OMODE16,argtype.ARG_REG_SI,argtype.ARG_NONE,argtype.ARG_NONE,117),
  new asminstdata("dec",0x4e,Proc.PROC_FROM80386,flags.FLAGS_OMODE32,argtype.ARG_REG_SI,argtype.ARG_NONE,argtype.ARG_NONE,118),
  new asminstdata("dec",0x4f,Proc.PROC_FROM8086,flags.FLAGS_OMODE16,argtype.ARG_REG_DI,argtype.ARG_NONE,argtype.ARG_NONE,119),
  new asminstdata("dec",0x4f,Proc.PROC_FROM80386,flags.FLAGS_OMODE32,argtype.ARG_REG_DI,argtype.ARG_NONE,argtype.ARG_NONE,120),
  new asminstdata("push",0x50,Proc.PROC_FROM8086,flags.FLAGS_OMODE16,argtype.ARG_REG_AX,argtype.ARG_NONE,argtype.ARG_NONE,121),
  new asminstdata("push",0x50,Proc.PROC_FROM80386,flags.FLAGS_OMODE32,argtype.ARG_REG_AX,argtype.ARG_NONE,argtype.ARG_NONE,122),
  new asminstdata("push",0x51,Proc.PROC_FROM8086,flags.FLAGS_OMODE16,argtype.ARG_REG_CX,argtype.ARG_NONE,argtype.ARG_NONE,123),
  new asminstdata("push",0x51,Proc.PROC_FROM80386,flags.FLAGS_OMODE32,argtype.ARG_REG_CX,argtype.ARG_NONE,argtype.ARG_NONE,124),
  new asminstdata("push",0x52,Proc.PROC_FROM8086,flags.FLAGS_OMODE16,argtype.ARG_REG_DX,argtype.ARG_NONE,argtype.ARG_NONE,125),
  new asminstdata("push",0x52,Proc.PROC_FROM80386,flags.FLAGS_OMODE32,argtype.ARG_REG_DX,argtype.ARG_NONE,argtype.ARG_NONE,126),
  new asminstdata("push",0x53,Proc.PROC_FROM8086,flags.FLAGS_OMODE16,argtype.ARG_REG_BX,argtype.ARG_NONE,argtype.ARG_NONE,127),
  new asminstdata("push",0x53,Proc.PROC_FROM80386,flags.FLAGS_OMODE32,argtype.ARG_REG_BX,argtype.ARG_NONE,argtype.ARG_NONE,128),
  new asminstdata("push",0x54,Proc.PROC_FROM8086,flags.FLAGS_OMODE16,argtype.ARG_REG_SP,argtype.ARG_NONE,argtype.ARG_NONE,129),
  new asminstdata("push",0x54,Proc.PROC_FROM80386,flags.FLAGS_OMODE32,argtype.ARG_REG_SP,argtype.ARG_NONE,argtype.ARG_NONE,130),
  new asminstdata("push",0x55,Proc.PROC_FROM8086,flags.FLAGS_OMODE16,argtype.ARG_REG_BP,argtype.ARG_NONE,argtype.ARG_NONE,131),
  new asminstdata("push",0x55,Proc.PROC_FROM80386,flags.FLAGS_OMODE32,argtype.ARG_REG_BP,argtype.ARG_NONE,argtype.ARG_NONE,132),
  new asminstdata("push",0x56,Proc.PROC_FROM8086,flags.FLAGS_OMODE16,argtype.ARG_REG_SI,argtype.ARG_NONE,argtype.ARG_NONE,133),
  new asminstdata("push",0x56,Proc.PROC_FROM80386,flags.FLAGS_OMODE32,argtype.ARG_REG_SI,argtype.ARG_NONE,argtype.ARG_NONE,134),
  new asminstdata("push",0x57,Proc.PROC_FROM8086,flags.FLAGS_OMODE16,argtype.ARG_REG_DI,argtype.ARG_NONE,argtype.ARG_NONE,135),
  new asminstdata("push",0x57,Proc.PROC_FROM80386,flags.FLAGS_OMODE32,argtype.ARG_REG_DI,argtype.ARG_NONE,argtype.ARG_NONE,136),
  new asminstdata("pop",0x58,Proc.PROC_FROM8086,flags.FLAGS_OMODE16,argtype.ARG_REG_AX,argtype.ARG_NONE,argtype.ARG_NONE,137),
  new asminstdata("pop",0x58,Proc.PROC_FROM80386,flags.FLAGS_OMODE32,argtype.ARG_REG_AX,argtype.ARG_NONE,argtype.ARG_NONE,138),
  new asminstdata("pop",0x59,Proc.PROC_FROM8086,flags.FLAGS_OMODE16,argtype.ARG_REG_CX,argtype.ARG_NONE,argtype.ARG_NONE,139),
  new asminstdata("pop",0x59,Proc.PROC_FROM80386,flags.FLAGS_OMODE32,argtype.ARG_REG_CX,argtype.ARG_NONE,argtype.ARG_NONE,140),
  new asminstdata("pop",0x5a,Proc.PROC_FROM8086,flags.FLAGS_OMODE16,argtype.ARG_REG_DX,argtype.ARG_NONE,argtype.ARG_NONE,141),
  new asminstdata("pop",0x5a,Proc.PROC_FROM80386,flags.FLAGS_OMODE32,argtype.ARG_REG_DX,argtype.ARG_NONE,argtype.ARG_NONE,142),
  new asminstdata("pop",0x5b,Proc.PROC_FROM8086,flags.FLAGS_OMODE16,argtype.ARG_REG_BX,argtype.ARG_NONE,argtype.ARG_NONE,143),
  new asminstdata("pop",0x5b,Proc.PROC_FROM80386,flags.FLAGS_OMODE32,argtype.ARG_REG_BX,argtype.ARG_NONE,argtype.ARG_NONE,144),
  new asminstdata("pop",0x5c,Proc.PROC_FROM8086,flags.FLAGS_OMODE16,argtype.ARG_REG_SP,argtype.ARG_NONE,argtype.ARG_NONE,145),
  new asminstdata("pop",0x5c,Proc.PROC_FROM80386,flags.FLAGS_OMODE32,argtype.ARG_REG_SP,argtype.ARG_NONE,argtype.ARG_NONE,146),
  new asminstdata("pop",0x5d,Proc.PROC_FROM8086,flags.FLAGS_OMODE16,argtype.ARG_REG_BP,argtype.ARG_NONE,argtype.ARG_NONE,147),
  new asminstdata("pop",0x5d,Proc.PROC_FROM80386,flags.FLAGS_OMODE32,argtype.ARG_REG_BP,argtype.ARG_NONE,argtype.ARG_NONE,148),
  new asminstdata("pop",0x5e,Proc.PROC_FROM8086,flags.FLAGS_OMODE16,argtype.ARG_REG_SI,argtype.ARG_NONE,argtype.ARG_NONE,149),
  new asminstdata("pop",0x5e,Proc.PROC_FROM80386,flags.FLAGS_OMODE32,argtype.ARG_REG_SI,argtype.ARG_NONE,argtype.ARG_NONE,150),
  new asminstdata("pop",0x5f,Proc.PROC_FROM8086,flags.FLAGS_OMODE16,argtype.ARG_REG_DI,argtype.ARG_NONE,argtype.ARG_NONE,151),
  new asminstdata("pop",0x5f,Proc.PROC_FROM80386,flags.FLAGS_OMODE32,argtype.ARG_REG_DI,argtype.ARG_NONE,argtype.ARG_NONE,152),
  new asminstdata("pusha",0x60,Proc.PROC_FROM80286,flags.FLAGS_OMODE16,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,153),
  new asminstdata("pushad",0x60,Proc.PROC_FROM80386,flags.FLAGS_OMODE32,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,154),
  new asminstdata("popa",0x61,Proc.PROC_FROM80286,flags.FLAGS_OMODE16,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,155),
  new asminstdata("popad",0x61,Proc.PROC_FROM80386,flags.FLAGS_OMODE32,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,156),
  new asminstdata("bound",0x62,Proc.PROC_FROM80286,flags.FLAGS_MODRM,argtype.ARG_REG,argtype.ARG_MODRM,argtype.ARG_NONE,157),
  new asminstdata("arpl",0x63,Proc.PROC_FROM80286,flags.FLAGS_PMODE|flags.FLAGS_16BIT|flags.FLAGS_MODRM,argtype.ARG_REG,argtype.ARG_MODRM,argtype.ARG_NONE,158),
  new asminstdata("fs:",0x64,Proc.PROC_FROM80386,flags.FLAGS_SEGPREFIX,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,159),
  new asminstdata("gs:",0x65,Proc.PROC_FROM80386,flags.FLAGS_SEGPREFIX,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,160),
  new asminstdata("",0x66,Proc.PROC_FROM80386,flags.FLAGS_OPERPREFIX,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,161),
  new asminstdata("",0x67,Proc.PROC_FROM80386,flags.FLAGS_ADDRPREFIX,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,162),
  new asminstdata("push",0x68,Proc.PROC_FROM80286,flags.FLAGS_OMODE16,argtype.ARG_IMM,argtype.ARG_NONE,argtype.ARG_NONE,163),
  new asminstdata("push",0x68,Proc.PROC_FROM80386,flags.FLAGS_OMODE32,argtype.ARG_IMM,argtype.ARG_NONE,argtype.ARG_NONE,164),
  new asminstdata("imul",0x69,Proc.PROC_FROM80386,flags.FLAGS_MODRM,argtype.ARG_REG,argtype.ARG_MODRM,argtype.ARG_IMM,165),
  new asminstdata("push",0x6a,Proc.PROC_FROM80286,0,argtype.ARG_IMM8,argtype.ARG_NONE,argtype.ARG_NONE,166),
  new asminstdata("imul",0x6b,Proc.PROC_FROM80386,flags.FLAGS_MODRM,argtype.ARG_REG,argtype.ARG_MODRM,argtype.ARG_IMM8,167),
  new asminstdata("insb",0x6c,Proc.PROC_FROM80286,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,168),
  new asminstdata("insw",0x6d,Proc.PROC_FROM80286,flags.FLAGS_OMODE16,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,169),
  new asminstdata("insd",0x6d,Proc.PROC_FROM80386,flags.FLAGS_OMODE32,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,170),
  new asminstdata("outsb",0x6e,Proc.PROC_FROM80286,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,171),
  new asminstdata("outsw",0x6f,Proc.PROC_FROM80286,flags.FLAGS_OMODE16,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,172),
  new asminstdata("outsd",0x6f,Proc.PROC_FROM80386,flags.FLAGS_OMODE32,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,173),
  new asminstdata("jo",0x70,Proc.PROC_FROM8086,flags.FLAGS_CJMP,argtype.ARG_RELIMM8,argtype.ARG_NONE,argtype.ARG_NONE,174),
  new asminstdata("jno",0x71,Proc.PROC_FROM8086,flags.FLAGS_CJMP,argtype.ARG_RELIMM8,argtype.ARG_NONE,argtype.ARG_NONE,175),
  new asminstdata("jc",0x72,Proc.PROC_FROM8086,flags.FLAGS_CJMP,argtype.ARG_RELIMM8,argtype.ARG_NONE,argtype.ARG_NONE,176),
  new asminstdata("jnc",0x73,Proc.PROC_FROM8086,flags.FLAGS_CJMP,argtype.ARG_RELIMM8,argtype.ARG_NONE,argtype.ARG_NONE,177),
  new asminstdata("jz",0x74,Proc.PROC_FROM8086,flags.FLAGS_CJMP,argtype.ARG_RELIMM8,argtype.ARG_NONE,argtype.ARG_NONE,178),
  new asminstdata("jnz",0x75,Proc.PROC_FROM8086,flags.FLAGS_CJMP,argtype.ARG_RELIMM8,argtype.ARG_NONE,argtype.ARG_NONE,179),
  new asminstdata("jbe",0x76,Proc.PROC_FROM8086,flags.FLAGS_CJMP,argtype.ARG_RELIMM8,argtype.ARG_NONE,argtype.ARG_NONE,180),
  new asminstdata("ja",0x77,Proc.PROC_FROM8086,flags.FLAGS_CJMP,argtype.ARG_RELIMM8,argtype.ARG_NONE,argtype.ARG_NONE,181),
  new asminstdata("js",0x78,Proc.PROC_FROM8086,flags.FLAGS_CJMP,argtype.ARG_RELIMM8,argtype.ARG_NONE,argtype.ARG_NONE,182),
  new asminstdata("jns",0x79,Proc.PROC_FROM8086,flags.FLAGS_CJMP,argtype.ARG_RELIMM8,argtype.ARG_NONE,argtype.ARG_NONE,183),
  new asminstdata("jpe",0x7a,Proc.PROC_FROM8086,flags.FLAGS_CJMP,argtype.ARG_RELIMM8,argtype.ARG_NONE,argtype.ARG_NONE,184),
  new asminstdata("jpo",0x7b,Proc.PROC_FROM8086,flags.FLAGS_CJMP,argtype.ARG_RELIMM8,argtype.ARG_NONE,argtype.ARG_NONE,185),
  new asminstdata("jl",0x7c,Proc.PROC_FROM8086,flags.FLAGS_CJMP,argtype.ARG_RELIMM8,argtype.ARG_NONE,argtype.ARG_NONE,186),
  new asminstdata("jge",0x7d,Proc.PROC_FROM8086,flags.FLAGS_CJMP,argtype.ARG_RELIMM8,argtype.ARG_NONE,argtype.ARG_NONE,187),
  new asminstdata("jle",0x7e,Proc.PROC_FROM8086,flags.FLAGS_CJMP,argtype.ARG_RELIMM8,argtype.ARG_NONE,argtype.ARG_NONE,188),
  new asminstdata("jg",0x7f,Proc.PROC_FROM8086,flags.FLAGS_CJMP,argtype.ARG_RELIMM8,argtype.ARG_NONE,argtype.ARG_NONE,189),
  new asminstdata(null,0x80,Proc.PROC_FROM8086,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,190),  //subtable 0x80
  new asminstdata(null,0x81,Proc.PROC_FROM8086,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,191),  //subtable 0x81
  new asminstdata(null,0x82,Proc.PROC_FROM8086,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,192),  //subtable 0x82
  new asminstdata(null,0x83,Proc.PROC_FROM8086,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,193),  //subtable 0x83
  new asminstdata("test",0x84,Proc.PROC_FROM8086,flags.FLAGS_MODRM|flags.FLAGS_8BIT,argtype.ARG_MODRM,argtype.ARG_REG,argtype.ARG_NONE,194),
  new asminstdata("test",0x85,Proc.PROC_FROM8086,flags.FLAGS_MODRM|flags.FLAGS_OMODE16,argtype.ARG_MODRM,argtype.ARG_REG,argtype.ARG_NONE,195),
  new asminstdata("test",0x85,Proc.PROC_FROM80386,flags.FLAGS_MODRM|flags.FLAGS_OMODE32,argtype.ARG_MODRM,argtype.ARG_REG,argtype.ARG_NONE,196),
  new asminstdata("xchg",0x86,Proc.PROC_FROM8086,flags.FLAGS_MODRM|flags.FLAGS_8BIT,argtype.ARG_MODRM,argtype.ARG_REG,argtype.ARG_NONE,197),
  new asminstdata("xchg",0x87,Proc.PROC_FROM8086,flags.FLAGS_MODRM|flags.FLAGS_OMODE16,argtype.ARG_MODRM,argtype.ARG_REG,argtype.ARG_NONE,198),
  new asminstdata("xchg",0x87,Proc.PROC_FROM80386,flags.FLAGS_MODRM|flags.FLAGS_OMODE32,argtype.ARG_MODRM,argtype.ARG_REG,argtype.ARG_NONE,199),
  new asminstdata("mov",0x88,Proc.PROC_FROM8086,flags.FLAGS_MODRM|flags.FLAGS_8BIT,argtype.ARG_MODRM,argtype.ARG_REG,argtype.ARG_NONE,200),
  new asminstdata("mov",0x89,Proc.PROC_FROM8086,flags.FLAGS_MODRM|flags.FLAGS_OMODE16,argtype.ARG_MODRM,argtype.ARG_REG,argtype.ARG_NONE,201),
  new asminstdata("mov",0x89,Proc.PROC_FROM80386,flags.FLAGS_MODRM|flags.FLAGS_OMODE32,argtype.ARG_MODRM,argtype.ARG_REG,argtype.ARG_NONE,202),
  new asminstdata("mov",0x8a,Proc.PROC_FROM8086,flags.FLAGS_MODRM|flags.FLAGS_8BIT,argtype.ARG_REG,argtype.ARG_MODRM,argtype.ARG_NONE,203),
  new asminstdata("mov",0x8b,Proc.PROC_FROM8086,flags.FLAGS_MODRM|flags.FLAGS_OMODE16,argtype.ARG_REG,argtype.ARG_MODRM,argtype.ARG_NONE,204),
  new asminstdata("mov",0x8b,Proc.PROC_FROM80386,flags.FLAGS_MODRM|flags.FLAGS_OMODE32,argtype.ARG_REG,argtype.ARG_MODRM,argtype.ARG_NONE,205),
  new asminstdata("mov",0x8c,Proc.PROC_FROM8086,flags.FLAGS_MODRM,argtype.ARG_MODRM,argtype.ARG_SREG,argtype.ARG_NONE,206),
  new asminstdata("lea",0x8d,Proc.PROC_FROM8086,flags.FLAGS_MODRM|flags.FLAGS_OMODE16,argtype.ARG_REG,argtype.ARG_MODRM,argtype.ARG_NONE,207),
  new asminstdata("lea",0x8d,Proc.PROC_FROM80386,flags.FLAGS_MODRM|flags.FLAGS_OMODE32,argtype.ARG_REG,argtype.ARG_MODRM,argtype.ARG_NONE,208),
  new asminstdata("mov",0x8e,Proc.PROC_FROM8086,flags.FLAGS_MODRM,argtype.ARG_SREG,argtype.ARG_MODRM,argtype.ARG_NONE,209),
  new asminstdata("pop",0x8f,Proc.PROC_FROM8086,flags.FLAGS_MODRM|flags.FLAGS_OMODE16,argtype.ARG_MODRM,argtype.ARG_NONE,argtype.ARG_NONE,210),
  new asminstdata("pop",0x8f,Proc.PROC_FROM80386,flags.FLAGS_MODRM|flags.FLAGS_OMODE32,argtype.ARG_MODRM,argtype.ARG_NONE,argtype.ARG_NONE,211),
  new asminstdata("nop",0x90,Proc.PROC_FROM8086,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,212),
  new asminstdata("xchg",0x91,Proc.PROC_FROM8086,flags.FLAGS_OMODE16,argtype.ARG_REG_AX,argtype.ARG_REG_CX,argtype.ARG_NONE,213),
  new asminstdata("xchg",0x91,Proc.PROC_FROM80386,flags.FLAGS_OMODE32,argtype.ARG_REG_AX,argtype.ARG_REG_CX,argtype.ARG_NONE,214),
  new asminstdata("xchg",0x92,Proc.PROC_FROM8086,flags.FLAGS_OMODE16,argtype.ARG_REG_AX,argtype.ARG_REG_DX,argtype.ARG_NONE,215),
  new asminstdata("xchg",0x92,Proc.PROC_FROM80386,flags.FLAGS_OMODE32,argtype.ARG_REG_AX,argtype.ARG_REG_DX,argtype.ARG_NONE,216),
  new asminstdata("xchg",0x93,Proc.PROC_FROM8086,flags.FLAGS_OMODE16,argtype.ARG_REG_AX,argtype.ARG_REG_BX,argtype.ARG_NONE,217),
  new asminstdata("xchg",0x93,Proc.PROC_FROM80386,flags.FLAGS_OMODE32,argtype.ARG_REG_AX,argtype.ARG_REG_BX,argtype.ARG_NONE,218),
  new asminstdata("xchg",0x94,Proc.PROC_FROM8086,flags.FLAGS_OMODE16,argtype.ARG_REG_AX,argtype.ARG_REG_SP,argtype.ARG_NONE,219),
  new asminstdata("xchg",0x94,Proc.PROC_FROM80386,flags.FLAGS_OMODE32,argtype.ARG_REG_AX,argtype.ARG_REG_SP,argtype.ARG_NONE,220),
  new asminstdata("xchg",0x95,Proc.PROC_FROM8086,flags.FLAGS_OMODE16,argtype.ARG_REG_AX,argtype.ARG_REG_BP,argtype.ARG_NONE,221),
  new asminstdata("xchg",0x95,Proc.PROC_FROM80386,flags.FLAGS_OMODE32,argtype.ARG_REG_AX,argtype.ARG_REG_BP,argtype.ARG_NONE,222),
  new asminstdata("xchg",0x96,Proc.PROC_FROM8086,flags.FLAGS_OMODE16,argtype.ARG_REG_AX,argtype.ARG_REG_SI,argtype.ARG_NONE,223),
  new asminstdata("xchg",0x96,Proc.PROC_FROM80386,flags.FLAGS_OMODE32,argtype.ARG_REG_AX,argtype.ARG_REG_SI,argtype.ARG_NONE,224),
  new asminstdata("xchg",0x97,Proc.PROC_FROM8086,flags.FLAGS_OMODE16,argtype.ARG_REG_AX,argtype.ARG_REG_DI,argtype.ARG_NONE,225),
  new asminstdata("xchg",0x97,Proc.PROC_FROM80386,flags.FLAGS_OMODE32,argtype.ARG_REG_AX,argtype.ARG_REG_DI,argtype.ARG_NONE,226),
  new asminstdata("cbw",0x98,Proc.PROC_FROM8086,flags.FLAGS_OMODE16,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,227),
  new asminstdata("cwde",0x98,Proc.PROC_FROM80386,flags.FLAGS_OMODE32,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,228),
  new asminstdata("cwd",0x99,Proc.PROC_FROM8086,flags.FLAGS_OMODE16,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,229),
  new asminstdata("cdq",0x99,Proc.PROC_FROM80386,flags.FLAGS_OMODE32,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,230),
  new asminstdata("callf",0x9a,Proc.PROC_FROM8086,flags.FLAGS_CALL|flags.FLAGS_OMODE16,argtype.ARG_FADDR,argtype.ARG_NONE,argtype.ARG_NONE,231),
  new asminstdata("callf",0x9a,Proc.PROC_FROM80386,flags.FLAGS_CALL|flags.FLAGS_OMODE32,argtype.ARG_FADDR,argtype.ARG_NONE,argtype.ARG_NONE,232),
  new asminstdata("wait",0x9b,Proc.PROC_FROM8086,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,233),
  new asminstdata("pushf",0x9c,Proc.PROC_FROM8086,flags.FLAGS_OMODE16,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,234),
  new asminstdata("pushfd",0x9c,Proc.PROC_FROM80386,flags.FLAGS_OMODE32,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,235),
  new asminstdata("popf",0x9d,Proc.PROC_FROM8086,flags.FLAGS_OMODE16,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,236),
  new asminstdata("popfd",0x9d,Proc.PROC_FROM80386,flags.FLAGS_OMODE32,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,237),
  new asminstdata("sahf",0x9e,Proc.PROC_FROM8086,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,238),
  new asminstdata("lahf",0x9f,Proc.PROC_FROM8086,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,239),
  new asminstdata("mov",0xa0,Proc.PROC_FROM8086,flags.FLAGS_8BIT,argtype.ARG_REG_AL,argtype.ARG_MEMLOC,argtype.ARG_NONE,240),
  new asminstdata("mov",0xa1,Proc.PROC_FROM8086,flags.FLAGS_OMODE16,argtype.ARG_REG_AX,argtype.ARG_MEMLOC,argtype.ARG_NONE,241),
  new asminstdata("mov",0xa1,Proc.PROC_FROM80386,flags.FLAGS_OMODE32,argtype.ARG_REG_AX,argtype.ARG_MEMLOC,argtype.ARG_NONE,242),
  new asminstdata("mov",0xa2,Proc.PROC_FROM8086,flags.FLAGS_8BIT,argtype.ARG_MEMLOC,argtype.ARG_REG_AL,argtype.ARG_NONE,243),
  new asminstdata("mov",0xa3,Proc.PROC_FROM8086,flags.FLAGS_OMODE16,argtype.ARG_MEMLOC,argtype.ARG_REG_AX,argtype.ARG_NONE,244),
  new asminstdata("mov",0xa3,Proc.PROC_FROM80386,flags.FLAGS_OMODE32,argtype.ARG_MEMLOC,argtype.ARG_REG_AX,argtype.ARG_NONE,245),
  new asminstdata("movsb",0xa4,Proc.PROC_FROM8086,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,246),
  new asminstdata("movsw",0xa5,Proc.PROC_FROM8086,flags.FLAGS_OMODE16,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,247),
  new asminstdata("movsd",0xa5,Proc.PROC_FROM80386,flags.FLAGS_OMODE32,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,248),
  new asminstdata("cmpsb",0xa6,Proc.PROC_FROM8086,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,249),
  new asminstdata("cmpsw",0xa7,Proc.PROC_FROM8086,flags.FLAGS_OMODE16,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,250),
  new asminstdata("cmpsd",0xa7,Proc.PROC_FROM80386,flags.FLAGS_OMODE32,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,251),
  new asminstdata("test",0xa8,Proc.PROC_FROM8086,flags.FLAGS_MODRM|flags.FLAGS_8BIT,argtype.ARG_REG_AL,argtype.ARG_IMM8,argtype.ARG_NONE,252),
  new asminstdata("test",0xa9,Proc.PROC_FROM8086,flags.FLAGS_MODRM|flags.FLAGS_OMODE16,argtype.ARG_REG_AX,argtype.ARG_IMM,argtype.ARG_NONE,253),
  new asminstdata("test",0xa9,Proc.PROC_FROM80386,flags.FLAGS_MODRM|flags.FLAGS_OMODE32,argtype.ARG_REG_AX,argtype.ARG_IMM,argtype.ARG_NONE,254),
  new asminstdata("stosb",0xaa,Proc.PROC_FROM8086,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,255),
  new asminstdata("stosw",0xab,Proc.PROC_FROM8086,flags.FLAGS_OMODE16,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,256),
  new asminstdata("stosd",0xab,Proc.PROC_FROM80386,flags.FLAGS_OMODE32,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,257),
  new asminstdata("lodsb",0xac,Proc.PROC_FROM8086,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,258),
  new asminstdata("lodsw",0xad,Proc.PROC_FROM8086,flags.FLAGS_OMODE16,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,259),
  new asminstdata("lodsd",0xad,Proc.PROC_FROM80386,flags.FLAGS_OMODE32,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,260),
  new asminstdata("scasb",0xae,Proc.PROC_FROM8086,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,261),
  new asminstdata("scasw",0xaf,Proc.PROC_FROM8086,flags.FLAGS_OMODE16,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,262),
  new asminstdata("scasd",0xaf,Proc.PROC_FROM80386,flags.FLAGS_OMODE32,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,263),
  new asminstdata("mov",0xb0,Proc.PROC_FROM8086,flags.FLAGS_8BIT,argtype.ARG_REG_AL,argtype.ARG_IMM8,argtype.ARG_NONE,264),
  new asminstdata("mov",0xb1,Proc.PROC_FROM8086,flags.FLAGS_8BIT,argtype.ARG_REG_CL,argtype.ARG_IMM8,argtype.ARG_NONE,265),
  new asminstdata("mov",0xb2,Proc.PROC_FROM8086,flags.FLAGS_8BIT,argtype.ARG_REG_DL,argtype.ARG_IMM8,argtype.ARG_NONE,266),
  new asminstdata("mov",0xb3,Proc.PROC_FROM8086,flags.FLAGS_8BIT,argtype.ARG_REG_BL,argtype.ARG_IMM8,argtype.ARG_NONE,267),
  new asminstdata("mov",0xb4,Proc.PROC_FROM8086,flags.FLAGS_8BIT,argtype.ARG_REG_AH,argtype.ARG_IMM8,argtype.ARG_NONE,268),
  new asminstdata("mov",0xb5,Proc.PROC_FROM8086,flags.FLAGS_8BIT,argtype.ARG_REG_CH,argtype.ARG_IMM8,argtype.ARG_NONE,269),
  new asminstdata("mov",0xb6,Proc.PROC_FROM8086,flags.FLAGS_8BIT,argtype.ARG_REG_DH,argtype.ARG_IMM8,argtype.ARG_NONE,270),
  new asminstdata("mov",0xb7,Proc.PROC_FROM8086,flags.FLAGS_8BIT,argtype.ARG_REG_BH,argtype.ARG_IMM8,argtype.ARG_NONE,271),
  new asminstdata("mov",0xb8,Proc.PROC_FROM8086,flags.FLAGS_OMODE16,argtype.ARG_REG_AX,argtype.ARG_IMM,argtype.ARG_NONE,272),
  new asminstdata("mov",0xb8,Proc.PROC_FROM80386,flags.FLAGS_OMODE32,argtype.ARG_REG_AX,argtype.ARG_IMM,argtype.ARG_NONE,273),
  new asminstdata("mov",0xb9,Proc.PROC_FROM8086,flags.FLAGS_OMODE16,argtype.ARG_REG_CX,argtype.ARG_IMM,argtype.ARG_NONE,274),
  new asminstdata("mov",0xb9,Proc.PROC_FROM80386,flags.FLAGS_OMODE32,argtype.ARG_REG_CX,argtype.ARG_IMM,argtype.ARG_NONE,275),
  new asminstdata("mov",0xba,Proc.PROC_FROM8086,flags.FLAGS_OMODE16,argtype.ARG_REG_DX,argtype.ARG_IMM,argtype.ARG_NONE,276),
  new asminstdata("mov",0xba,Proc.PROC_FROM80386,flags.FLAGS_OMODE32,argtype.ARG_REG_DX,argtype.ARG_IMM,argtype.ARG_NONE,277),
  new asminstdata("mov",0xbb,Proc.PROC_FROM8086,flags.FLAGS_OMODE16,argtype.ARG_REG_BX,argtype.ARG_IMM,argtype.ARG_NONE,278),
  new asminstdata("mov",0xbb,Proc.PROC_FROM80386,flags.FLAGS_OMODE32,argtype.ARG_REG_BX,argtype.ARG_IMM,argtype.ARG_NONE,279),
  new asminstdata("mov",0xbc,Proc.PROC_FROM8086,flags.FLAGS_OMODE16,argtype.ARG_REG_SP,argtype.ARG_IMM,argtype.ARG_NONE,280),
  new asminstdata("mov",0xbc,Proc.PROC_FROM80386,flags.FLAGS_OMODE32,argtype.ARG_REG_SP,argtype.ARG_IMM,argtype.ARG_NONE,281),
  new asminstdata("mov",0xbd,Proc.PROC_FROM8086,flags.FLAGS_OMODE16,argtype.ARG_REG_BP,argtype.ARG_IMM,argtype.ARG_NONE,282),
  new asminstdata("mov",0xbd,Proc.PROC_FROM80386,flags.FLAGS_OMODE32,argtype.ARG_REG_BP,argtype.ARG_IMM,argtype.ARG_NONE,283),
  new asminstdata("mov",0xbe,Proc.PROC_FROM8086,flags.FLAGS_OMODE16,argtype.ARG_REG_SI,argtype.ARG_IMM,argtype.ARG_NONE,284),
  new asminstdata("mov",0xbe,Proc.PROC_FROM80386,flags.FLAGS_OMODE32,argtype.ARG_REG_SI,argtype.ARG_IMM,argtype.ARG_NONE,285),
  new asminstdata("mov",0xbf,Proc.PROC_FROM8086,flags.FLAGS_OMODE16,argtype.ARG_REG_DI,argtype.ARG_IMM,argtype.ARG_NONE,286),
  new asminstdata("mov",0xbf,Proc.PROC_FROM80386,flags.FLAGS_OMODE32,argtype.ARG_REG_DI,argtype.ARG_IMM,argtype.ARG_NONE,287),
  new asminstdata(null,0xc0,Proc.PROC_FROM8086,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,288),  //subtable 0xc0
  new asminstdata(null,0xc1,Proc.PROC_FROM8086,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,289),  //subtable 0xc1
  new asminstdata("ret",0xc2,Proc.PROC_FROM8086,flags.FLAGS_16BIT|flags.FLAGS_RET,argtype.ARG_IMM,argtype.ARG_NONE,argtype.ARG_NONE,290),
  new asminstdata("ret",0xc3,Proc.PROC_FROM8086,flags.FLAGS_RET,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,291),
  new asminstdata("les",0xc4,Proc.PROC_FROM8086,flags.FLAGS_MODRM|flags.FLAGS_OMODE16,argtype.ARG_REG,argtype.ARG_MODRM,argtype.ARG_NONE,292),
  new asminstdata("les",0xc4,Proc.PROC_FROM80386,flags.FLAGS_MODRM|flags.FLAGS_OMODE32,argtype.ARG_REG,argtype.ARG_MODRM,argtype.ARG_NONE,293),
  new asminstdata("lds",0xc5,Proc.PROC_FROM8086,flags.FLAGS_MODRM|flags.FLAGS_OMODE16,argtype.ARG_REG,argtype.ARG_MODRM,argtype.ARG_NONE,294),
  new asminstdata("lds",0xc5,Proc.PROC_FROM80386,flags.FLAGS_MODRM|flags.FLAGS_OMODE32,argtype.ARG_REG,argtype.ARG_MODRM,argtype.ARG_NONE,295),
  new asminstdata("mov",0xc6,Proc.PROC_FROM8086,flags.FLAGS_MODRM|flags.FLAGS_8BIT,argtype.ARG_MODRM,argtype.ARG_IMM8,argtype.ARG_NONE,296),
  new asminstdata("mov",0xc7,Proc.PROC_FROM8086,flags.FLAGS_MODRM|flags.FLAGS_OMODE16,argtype.ARG_MODRM,argtype.ARG_IMM,argtype.ARG_NONE,297),
  new asminstdata("mov",0xc7,Proc.PROC_FROM80386,flags.FLAGS_MODRM|flags.FLAGS_OMODE32,argtype.ARG_MODRM,argtype.ARG_IMM,argtype.ARG_NONE,298),
  new asminstdata("enter",0xc8,Proc.PROC_FROM80286,flags.FLAGS_16BIT,argtype.ARG_IMM16_A,argtype.ARG_IMM8,argtype.ARG_NONE,299),
  new asminstdata("leave",0xc9,Proc.PROC_FROM80286,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,300),
  new asminstdata("retf",0xca,Proc.PROC_FROM8086,flags.FLAGS_16BIT|flags.FLAGS_RET,argtype.ARG_IMM,argtype.ARG_NONE,argtype.ARG_NONE,301),
  new asminstdata("retf",0xcb,Proc.PROC_FROM8086,flags.FLAGS_RET,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,302),
  new asminstdata("int 3",0xcc,Proc.PROC_FROM8086,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,303),
  new asminstdata("int",0xcd,Proc.PROC_FROM8086,flags.FLAGS_8BIT,argtype.ARG_IMM8,argtype.ARG_NONE,argtype.ARG_NONE,304),
  new asminstdata("into",0xce,Proc.PROC_FROM8086,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,305),
  new asminstdata("iret",0xcf,Proc.PROC_FROM8086,flags.FLAGS_RET,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,306),
  new asminstdata(null,0xd0,Proc.PROC_FROM8086,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,307),  //subtable 0xd0
  new asminstdata(null,0xd1,Proc.PROC_FROM8086,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,308),  //subtable 0xd1
  new asminstdata(null,0xd2,Proc.PROC_FROM8086,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,309),  //subtable 0xd2
  new asminstdata(null,0xd3,Proc.PROC_FROM8086,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,310),  //subtable 0xd3
  new asminstdata("aam",0xd4,Proc.PROC_FROM8086,flags.FLAGS_8BIT,argtype.ARG_IMM8,argtype.ARG_NONE,argtype.ARG_NONE,311),
  new asminstdata("aad",0xd5,Proc.PROC_FROM8086,flags.FLAGS_8BIT,argtype.ARG_IMM8,argtype.ARG_NONE,argtype.ARG_NONE,312),
  new asminstdata("setalc",0xd6,Proc.PROC_FROM80286,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,313), //UNDOCUMENTED
  new asminstdata("xlat",0xd7,Proc.PROC_FROM8086,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,314),
  new asminstdata(null,0xd8,Proc.PROC_FROM8086,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,315),  //subtable 0xd8
  new asminstdata(null,0xd9,Proc.PROC_FROM8086,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,316),  //subtable 0xd9
  new asminstdata(null,0xda,Proc.PROC_FROM8086,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,317),  //subtable 0xda
  new asminstdata(null,0xdb,Proc.PROC_FROM8086,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,318),  //subtable 0xdb
  new asminstdata(null,0xdc,Proc.PROC_FROM8086,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,319),  //subtable 0xdc
  new asminstdata(null,0xdd,Proc.PROC_FROM8086,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,320),  //subtable 0xdd
  new asminstdata(null,0xde,Proc.PROC_FROM8086,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,321),  //subtable 0xde
  new asminstdata(null,0xdf,Proc.PROC_FROM8086,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,322),  //subtable 0xdf
  new asminstdata("loopnz",0xe0,Proc.PROC_FROM8086,flags.FLAGS_CJMP,argtype.ARG_RELIMM8,argtype.ARG_NONE,argtype.ARG_NONE,323),
  new asminstdata("loopz",0xe1,Proc.PROC_FROM8086,flags.FLAGS_CJMP,argtype.ARG_RELIMM8,argtype.ARG_NONE,argtype.ARG_NONE,324),
  new asminstdata("loop",0xe2,Proc.PROC_FROM8086,flags.FLAGS_CJMP,argtype.ARG_RELIMM8,argtype.ARG_NONE,argtype.ARG_NONE,325),
  new asminstdata("jcxz",0xe3,Proc.PROC_FROM8086,flags.FLAGS_CJMP,argtype.ARG_RELIMM8,argtype.ARG_NONE,argtype.ARG_NONE,326),
  new asminstdata("in",0xe4,Proc.PROC_FROM8086,flags.FLAGS_8BIT,argtype.ARG_REG_AL,argtype.ARG_IMM8,argtype.ARG_NONE,327),
  new asminstdata("in",0xe5,Proc.PROC_FROM8086,flags.FLAGS_OMODE16,argtype.ARG_REG_AX,argtype.ARG_IMM8,argtype.ARG_NONE,328),
  new asminstdata("in",0xe5,Proc.PROC_FROM80386,flags.FLAGS_OMODE32,argtype.ARG_REG_AX,argtype.ARG_IMM8,argtype.ARG_NONE,329),
  new asminstdata("out",0xe6,Proc.PROC_FROM8086,flags.FLAGS_8BIT,argtype.ARG_IMM8,argtype.ARG_REG_AL,argtype.ARG_NONE,330),
  new asminstdata("out",0xe7,Proc.PROC_FROM8086,flags.FLAGS_OMODE16,argtype.ARG_IMM8,argtype.ARG_REG_AX,argtype.ARG_NONE,331),
  new asminstdata("out",0xe7,Proc.PROC_FROM80386,flags.FLAGS_OMODE32,argtype.ARG_IMM8,argtype.ARG_REG_AX,argtype.ARG_NONE,332),
  new asminstdata("call",0xe8,Proc.PROC_FROM8086,flags.FLAGS_CALL|flags.FLAGS_OMODE16,argtype.ARG_RELIMM,argtype.ARG_NONE,argtype.ARG_NONE,333),
  new asminstdata("call",0xe8,Proc.PROC_FROM80386,flags.FLAGS_CALL|flags.FLAGS_OMODE32,argtype.ARG_RELIMM,argtype.ARG_NONE,argtype.ARG_NONE,334),
  new asminstdata("jmp",0xe9,Proc.PROC_FROM8086,flags.FLAGS_JMP|flags.FLAGS_OMODE16,argtype.ARG_RELIMM,argtype.ARG_NONE,argtype.ARG_NONE,335),
  new asminstdata("jmp",0xe9,Proc.PROC_FROM80386,flags.FLAGS_JMP|flags.FLAGS_OMODE32,argtype.ARG_RELIMM,argtype.ARG_NONE,argtype.ARG_NONE,336),
  new asminstdata("jmp",0xea,Proc.PROC_FROM8086,flags.FLAGS_JMP|flags.FLAGS_OMODE16,argtype.ARG_FADDR,argtype.ARG_NONE,argtype.ARG_NONE,337),
  new asminstdata("jmp",0xea,Proc.PROC_FROM80386,flags.FLAGS_JMP|flags.FLAGS_OMODE32,argtype.ARG_FADDR,argtype.ARG_NONE,argtype.ARG_NONE,338),
  new asminstdata("jmp",0xeb,Proc.PROC_FROM8086,flags.FLAGS_JMP,argtype.ARG_RELIMM8,argtype.ARG_NONE,argtype.ARG_NONE,339),
  new asminstdata("in",0xec,Proc.PROC_FROM8086,0,argtype.ARG_REG_AL,argtype.ARG_16REG_DX,argtype.ARG_NONE,340),
  new asminstdata("in",0xed,Proc.PROC_FROM8086,flags.FLAGS_OMODE16,argtype.ARG_REG_AX,argtype.ARG_16REG_DX,argtype.ARG_NONE,341),
  new asminstdata("in",0xed,Proc.PROC_FROM80386,flags.FLAGS_OMODE32,argtype.ARG_REG_AX,argtype.ARG_16REG_DX,argtype.ARG_NONE,342),
  new asminstdata("out",0xee,Proc.PROC_FROM8086,0,argtype.ARG_16REG_DX,argtype.ARG_REG_AL,argtype.ARG_NONE,343),
  new asminstdata("out",0xef,Proc.PROC_FROM8086,flags.FLAGS_OMODE16,argtype.ARG_16REG_DX,argtype.ARG_REG_AX,argtype.ARG_NONE,344),
  new asminstdata("out",0xef,Proc.PROC_FROM80386,flags.FLAGS_OMODE32,argtype.ARG_16REG_DX,argtype.ARG_REG_AX,argtype.ARG_NONE,345),
  new asminstdata("lock:",0xf0,Proc.PROC_FROM8086,flags.FLAGS_PREFIX,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,346),
  new asminstdata("smi",0xf1,Proc.PROC_FROM80386,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,347),  //UNDOCUMENTED/AMD ?
  new asminstdata("repne:",0xf2,Proc.PROC_FROM8086,flags.FLAGS_PREFIX,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,348),
  new asminstdata("rep:",0xf3,Proc.PROC_FROM8086,flags.FLAGS_PREFIX,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,349),
  new asminstdata("hlt",0xf4,Proc.PROC_FROM8086,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,350),
  new asminstdata("cmc",0xf5,Proc.PROC_FROM8086,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,351),
  new asminstdata(null,0xf6,Proc.PROC_FROM8086,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,352),  //subtable 0xf6
  new asminstdata(null,0xf7,Proc.PROC_FROM8086,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,353),  //subtable 0xf7
  new asminstdata("clc",0xf8,Proc.PROC_FROM8086,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,354),
  new asminstdata("stc",0xf9,Proc.PROC_FROM8086,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,355),
  new asminstdata("cli",0xfa,Proc.PROC_FROM8086,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,356),
  new asminstdata("sti",0xfb,Proc.PROC_FROM8086,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,357),
  new asminstdata("cld",0xfc,Proc.PROC_FROM8086,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,358),
  new asminstdata("std",0xfd,Proc.PROC_FROM8086,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,359),
  new asminstdata(null,0xfe,Proc.PROC_FROM8086,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,360),  //subtable 0xfe
  new asminstdata(null,0xff,Proc.PROC_FROM8086,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,361),  //subtable 0xff
  new asminstdata(null,0x00,0,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,0)  //endCode marker - processor=0 & opcode=0
};
        //Subtables needed - 0x0f, 0x80, 0x81, 0x82, 0x83, 0xc0, 0xc1,           ***addressesDone!
        // 0xd0, 0xd1, 0xd2, 0xd3, 0xf6, 0xf7, 0xfe, 0xff                        ***addressesDone!
        // 0xd8, 0xd9, 0xda, 0xdb, 0xdc, 0xdd, 0xde, 0xdf - FPU instructions
        // 0x0f subtables : 0x00, 0x01, 0x18, 0x71, 0x72, 0x73, 0xae, 0xba, 0xc7 ***addressesDone!
        //nb some instructions change when they have adressOfCall segment overrider eg xlat.
        // - how will this go in ?
        //need to check undocumented instructions/amd insts- fileName/size/etc
        //- setalc, smi

        // subtable 0x0f
        public static asminstdata[] asm86sub0f = new asminstdata[]
{ new asminstdata(null,0x00,Proc.PROC_FROM8086,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,1000), //subtable 0x0f/0x00
  new asminstdata(null,0x01,Proc.PROC_FROM8086,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,1001), //subtable 0x0f/0x01
  new asminstdata("lar",0x02,Proc.PROC_FROM80286,flags.FLAGS_PMODE|flags.FLAGS_MODRM|flags.FLAGS_OMODE16,argtype.ARG_REG,argtype.ARG_MODRM,argtype.ARG_NONE,1002),
  new asminstdata("lar",0x02,Proc.PROC_FROM80386,flags.FLAGS_PMODE|flags.FLAGS_MODRM|flags.FLAGS_OMODE32,argtype.ARG_REG,argtype.ARG_MODRM,argtype.ARG_NONE,1003),
  new asminstdata("lsl",0x03,Proc.PROC_FROM80286,flags.FLAGS_PMODE|flags.FLAGS_MODRM|flags.FLAGS_OMODE16,argtype.ARG_REG,argtype.ARG_MODRM,argtype.ARG_NONE,1004),
  new asminstdata("lsl",0x03,Proc.PROC_FROM80386,flags.FLAGS_PMODE|flags.FLAGS_MODRM|flags.FLAGS_OMODE32,argtype.ARG_REG,argtype.ARG_MODRM,argtype.ARG_NONE,1005),
  new asminstdata("clts",0x06,Proc.PROC_FROM80286,flags.FLAGS_PMODE,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,1006),
  new asminstdata("invd",0x08,Proc.PROC_FROM80486,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,1007),
  new asminstdata("wbinvd",0x09,Proc.PROC_FROM80486,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,1008),
  new asminstdata("cflsh",0x0a,Proc.PROC_FROM80286,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,1009),
  new asminstdata("ud2",0x0b,Proc.PROC_FROM80286,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,1010),
  new asminstdata("movups",0x10,Proc.PROC_FROMPENTIUM2,flags.FLAGS_MODRM|flags.FLAGS_32BIT,argtype.ARG_XREG,argtype.ARG_XMMMODRM,argtype.ARG_NONE,1011),
  new asminstdata("movups",0x11,Proc.PROC_FROMPENTIUM2,flags.FLAGS_MODRM|flags.FLAGS_32BIT,argtype.ARG_XMMMODRM,argtype.ARG_XREG,argtype.ARG_NONE,1012),
  new asminstdata("movlps",0x12,Proc.PROC_FROMPENTIUM2,flags.FLAGS_MODRM|flags.FLAGS_32BIT,argtype.ARG_XREG,argtype.ARG_XMMMODRM,argtype.ARG_NONE,1013),
  new asminstdata("movlps",0x13,Proc.PROC_FROMPENTIUM2,flags.FLAGS_MODRM|flags.FLAGS_32BIT,argtype.ARG_XMMMODRM,argtype.ARG_XREG,argtype.ARG_NONE,1014),
  new asminstdata("unpcklps",0x14,Proc.PROC_FROMPENTIUM2,flags.FLAGS_MODRM|flags.FLAGS_32BIT,argtype.ARG_XREG,argtype.ARG_XMMMODRM,argtype.ARG_NONE,1015),
  new asminstdata("unpckhps",0x15,Proc.PROC_FROMPENTIUM2,flags.FLAGS_MODRM|flags.FLAGS_32BIT,argtype.ARG_XREG,argtype.ARG_XMMMODRM,argtype.ARG_NONE,1016),
  new asminstdata("movhps",0x16,Proc.PROC_FROMPENTIUM2,flags.FLAGS_MODRM|flags.FLAGS_32BIT,argtype.ARG_XREG,argtype.ARG_XMMMODRM,argtype.ARG_NONE,1017),
  new asminstdata("movhps",0x17,Proc.PROC_FROMPENTIUM2,flags.FLAGS_MODRM|flags.FLAGS_32BIT,argtype.ARG_XMMMODRM,argtype.ARG_XREG,argtype.ARG_NONE,1018),
  new asminstdata(null,0x18,Proc.PROC_FROM8086,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,1019), // subtable 0x0f/0x18
  new asminstdata("mov",0x20,Proc.PROC_FROM80386,flags.FLAGS_MODRM|flags.FLAGS_32BIT,argtype.ARG_MODREG,argtype.ARG_CREG,argtype.ARG_NONE,1020),
  new asminstdata("mov",0x21,Proc.PROC_FROM80386,flags.FLAGS_MODRM|flags.FLAGS_32BIT,argtype.ARG_MODREG,argtype.ARG_DREG,argtype.ARG_NONE,1021),
  new asminstdata("mov",0x22,Proc.PROC_FROM80386,flags.FLAGS_MODRM|flags.FLAGS_32BIT,argtype.ARG_CREG,argtype.ARG_MODREG,argtype.ARG_NONE,1022),
  new asminstdata("mov",0x23,Proc.PROC_FROM80386,flags.FLAGS_MODRM|flags.FLAGS_32BIT,argtype.ARG_DREG,argtype.ARG_MODREG,argtype.ARG_NONE,1023),
  new asminstdata("mov",0x24,Proc.PROC_80386|Proc.PROC_80486,flags.FLAGS_MODRM|flags.FLAGS_32BIT,argtype.ARG_MODREG,argtype.ARG_TREG_67,argtype.ARG_NONE,1024),
  new asminstdata("mov",0x26,Proc.PROC_80386|Proc.PROC_80486,flags.FLAGS_MODRM|flags.FLAGS_32BIT,argtype.ARG_MODREG,argtype.ARG_TREG,argtype.ARG_NONE,1025),
  new asminstdata("movaps",0x28,Proc.PROC_FROMPENTIUM2,flags.FLAGS_MODRM|flags.FLAGS_32BIT,argtype.ARG_XREG,argtype.ARG_XMMMODRM,argtype.ARG_NONE,1026),
  new asminstdata("movaps",0x29,Proc.PROC_FROMPENTIUM2,flags.FLAGS_MODRM|flags.FLAGS_32BIT,argtype.ARG_XMMMODRM,argtype.ARG_XREG,argtype.ARG_NONE,1027),
  new asminstdata("cvtpi2ps",0x2a,Proc.PROC_FROMPENTIUM2,flags.FLAGS_MODRM|flags.FLAGS_32BIT,argtype.ARG_XREG,argtype.ARG_MMXMODRM,argtype.ARG_NONE,1028),
  new asminstdata("movntps",0x2b,Proc.PROC_FROMPENTIUM2,flags.FLAGS_MODRM|flags.FLAGS_32BIT,argtype.ARG_XMMMODRM,argtype.ARG_XREG,argtype.ARG_NONE,1029),
  new asminstdata("cvttps2pi",0x2c,Proc.PROC_FROMPENTIUM2,flags.FLAGS_MODRM|flags.FLAGS_32BIT,argtype.ARG_MREG,argtype.ARG_XMMMODRM,argtype.ARG_NONE,1030),
  new asminstdata("cvtps2pi",0x2d,Proc.PROC_FROMPENTIUM2,flags.FLAGS_MODRM|flags.FLAGS_32BIT,argtype.ARG_MREG,argtype.ARG_XMMMODRM,argtype.ARG_NONE,1031),
  new asminstdata("ucomiss",0x2e,Proc.PROC_FROMPENTIUM2,flags.FLAGS_MODRM|flags.FLAGS_32BIT,argtype.ARG_XREG,argtype.ARG_XMMMODRM,argtype.ARG_NONE,1032),
  new asminstdata("comiss",0x2f,Proc.PROC_FROMPENTIUM2,flags.FLAGS_MODRM|flags.FLAGS_32BIT,argtype.ARG_XREG,argtype.ARG_XMMMODRM,argtype.ARG_NONE,1033),
  new asminstdata("wrmsr",0x30,Proc.PROC_FROMPENTIUM,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,1034),
  new asminstdata("rdtsc",0x31,Proc.PROC_FROMPENTIUM,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,1035),
  new asminstdata("rdmsr",0x32,Proc.PROC_FROMPENTIUM,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,1036),
  new asminstdata("rdpmc",0x33,Proc.PROC_FROMPENTPRO,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,1037),
  new asminstdata("sysenter",0x34,Proc.PROC_FROMPENTIUM2,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,1038),
  new asminstdata("sysexit",0x35,Proc.PROC_FROMPENTIUM2,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,1039),
  new asminstdata("cmovo",0x40,Proc.PROC_FROMPENTPRO,flags.FLAGS_MODRM,argtype.ARG_REG,argtype.ARG_MODRM,argtype.ARG_NONE,1040),
  new asminstdata("cmovno",0x41,Proc.PROC_FROMPENTPRO,flags.FLAGS_MODRM,argtype.ARG_REG,argtype.ARG_MODRM,argtype.ARG_NONE,1041),
  new asminstdata("cmovc",0x42,Proc.PROC_FROMPENTPRO,flags.FLAGS_MODRM,argtype.ARG_REG,argtype.ARG_MODRM,argtype.ARG_NONE,1042),
  new asminstdata("cmovnc",0x43,Proc.PROC_FROMPENTPRO,flags.FLAGS_MODRM,argtype.ARG_REG,argtype.ARG_MODRM,argtype.ARG_NONE,1043),
  new asminstdata("cmovz",0x44,Proc.PROC_FROMPENTPRO,flags.FLAGS_MODRM,argtype.ARG_REG,argtype.ARG_MODRM,argtype.ARG_NONE,1044),
  new asminstdata("cmovnz",0x45,Proc.PROC_FROMPENTPRO,flags.FLAGS_MODRM,argtype.ARG_REG,argtype.ARG_MODRM,argtype.ARG_NONE,1045),
  new asminstdata("cmovbe",0x46,Proc.PROC_FROMPENTPRO,flags.FLAGS_MODRM,argtype.ARG_REG,argtype.ARG_MODRM,argtype.ARG_NONE,1046),
  new asminstdata("cmova",0x47,Proc.PROC_FROMPENTPRO,flags.FLAGS_MODRM,argtype.ARG_REG,argtype.ARG_MODRM,argtype.ARG_NONE,1047),
  new asminstdata("cmovs",0x48,Proc.PROC_FROMPENTPRO,flags.FLAGS_MODRM,argtype.ARG_REG,argtype.ARG_MODRM,argtype.ARG_NONE,1048),
  new asminstdata("cmovns",0x49,Proc.PROC_FROMPENTPRO,flags.FLAGS_MODRM,argtype.ARG_REG,argtype.ARG_MODRM,argtype.ARG_NONE,1049),
  new asminstdata("cmovpe",0x4a,Proc.PROC_FROMPENTPRO,flags.FLAGS_MODRM,argtype.ARG_REG,argtype.ARG_MODRM,argtype.ARG_NONE,1050),
  new asminstdata("cmovpo",0x4b,Proc.PROC_FROMPENTPRO,flags.FLAGS_MODRM,argtype.ARG_REG,argtype.ARG_MODRM,argtype.ARG_NONE,1051),
  new asminstdata("cmovl",0x4c,Proc.PROC_FROMPENTPRO,flags.FLAGS_MODRM,argtype.ARG_REG,argtype.ARG_MODRM,argtype.ARG_NONE,1052),
  new asminstdata("cmovge",0x4d,Proc.PROC_FROMPENTPRO,flags.FLAGS_MODRM,argtype.ARG_REG,argtype.ARG_MODRM,argtype.ARG_NONE,1053),
  new asminstdata("cmovle",0x4e,Proc.PROC_FROMPENTPRO,flags.FLAGS_MODRM,argtype.ARG_REG,argtype.ARG_MODRM,argtype.ARG_NONE,1054),
  new asminstdata("cmovg",0x4f,Proc.PROC_FROMPENTPRO,flags.FLAGS_MODRM,argtype.ARG_REG,argtype.ARG_MODRM,argtype.ARG_NONE,1055),
  new asminstdata("movmskps",0x50,Proc.PROC_FROMPENTIUM2,flags.FLAGS_MODRM|flags.FLAGS_32BIT,argtype.ARG_REG,argtype.ARG_XMMMODRM,argtype.ARG_NONE,1056),
  new asminstdata("sqrtps",0x51,Proc.PROC_FROMPENTIUM2,flags.FLAGS_MODRM|flags.FLAGS_32BIT,argtype.ARG_XREG,argtype.ARG_XMMMODRM,argtype.ARG_NONE,1057),
  new asminstdata("rsqrtps",0x52,Proc.PROC_FROMPENTIUM2,flags.FLAGS_MODRM|flags.FLAGS_32BIT,argtype.ARG_XREG,argtype.ARG_XMMMODRM,argtype.ARG_NONE,1058),
  new asminstdata("rcpps",0x53,Proc.PROC_FROMPENTIUM2,flags.FLAGS_MODRM|flags.FLAGS_32BIT,argtype.ARG_XREG,argtype.ARG_XMMMODRM,argtype.ARG_NONE,1059),
  new asminstdata("andps",0x54,Proc.PROC_FROMPENTIUM2,flags.FLAGS_MODRM|flags.FLAGS_32BIT,argtype.ARG_XREG,argtype.ARG_XMMMODRM,argtype.ARG_NONE,1060),
  new asminstdata("andnps",0x55,Proc.PROC_FROMPENTIUM2,flags.FLAGS_MODRM|flags.FLAGS_32BIT,argtype.ARG_XREG,argtype.ARG_XMMMODRM,argtype.ARG_NONE,1061),
  new asminstdata("orps",0x56,Proc.PROC_FROMPENTIUM2,flags.FLAGS_MODRM|flags.FLAGS_32BIT,argtype.ARG_XREG,argtype.ARG_XMMMODRM,argtype.ARG_NONE,1062),
  new asminstdata("xorps",0x57,Proc.PROC_FROMPENTIUM2,flags.FLAGS_MODRM|flags.FLAGS_32BIT,argtype.ARG_XREG,argtype.ARG_XMMMODRM,argtype.ARG_NONE,1063),
  new asminstdata("addps",0x58,Proc.PROC_FROMPENTIUM2,flags.FLAGS_MODRM|flags.FLAGS_32BIT,argtype.ARG_XREG,argtype.ARG_XMMMODRM,argtype.ARG_NONE,1064),
  new asminstdata("mulps",0x59,Proc.PROC_FROMPENTIUM2,flags.FLAGS_MODRM|flags.FLAGS_32BIT,argtype.ARG_XREG,argtype.ARG_XMMMODRM,argtype.ARG_NONE,1065),
  new asminstdata("subps",0x5c,Proc.PROC_FROMPENTIUM2,flags.FLAGS_MODRM|flags.FLAGS_32BIT,argtype.ARG_XREG,argtype.ARG_XMMMODRM,argtype.ARG_NONE,1066),
  new asminstdata("minps",0x5d,Proc.PROC_FROMPENTIUM2,flags.FLAGS_MODRM|flags.FLAGS_32BIT,argtype.ARG_XREG,argtype.ARG_XMMMODRM,argtype.ARG_NONE,1067),
  new asminstdata("divps",0x5e,Proc.PROC_FROMPENTIUM2,flags.FLAGS_MODRM|flags.FLAGS_32BIT,argtype.ARG_XREG,argtype.ARG_XMMMODRM,argtype.ARG_NONE,1068),
  new asminstdata("maxps",0x5f,Proc.PROC_FROMPENTIUM2,flags.FLAGS_MODRM|flags.FLAGS_32BIT,argtype.ARG_XREG,argtype.ARG_XMMMODRM,argtype.ARG_NONE,1069),
  new asminstdata("punpcklbw",0x60,Proc.PROC_FROMPENTMMX,flags.FLAGS_MODRM|flags.FLAGS_32BIT,argtype.ARG_MREG,argtype.ARG_MMXMODRM,argtype.ARG_NONE,1070),
  new asminstdata("punpcklwd",0x61,Proc.PROC_FROMPENTMMX,flags.FLAGS_MODRM|flags.FLAGS_32BIT,argtype.ARG_MREG,argtype.ARG_MMXMODRM,argtype.ARG_NONE,1071),
  new asminstdata("punpckldq",0x62,Proc.PROC_FROMPENTMMX,flags.FLAGS_MODRM|flags.FLAGS_32BIT,argtype.ARG_MREG,argtype.ARG_MMXMODRM,argtype.ARG_NONE,1072),
  new asminstdata("packsswb",0x63,Proc.PROC_FROMPENTMMX,flags.FLAGS_MODRM|flags.FLAGS_32BIT,argtype.ARG_MREG,argtype.ARG_MMXMODRM,argtype.ARG_NONE,1073),
  new asminstdata("pcmpgtb",0x64,Proc.PROC_FROMPENTMMX,flags.FLAGS_MODRM|flags.FLAGS_32BIT,argtype.ARG_MREG,argtype.ARG_MMXMODRM,argtype.ARG_NONE,1074),
  new asminstdata("pcmpgtw",0x65,Proc.PROC_FROMPENTMMX,flags.FLAGS_MODRM|flags.FLAGS_32BIT,argtype.ARG_MREG,argtype.ARG_MMXMODRM,argtype.ARG_NONE,1075),
  new asminstdata("pcmpgtd",0x66,Proc.PROC_FROMPENTMMX,flags.FLAGS_MODRM|flags.FLAGS_32BIT,argtype.ARG_MREG,argtype.ARG_MMXMODRM,argtype.ARG_NONE,1076),
  new asminstdata("packuswb",0x67,Proc.PROC_FROMPENTMMX,flags.FLAGS_MODRM|flags.FLAGS_32BIT,argtype.ARG_MREG,argtype.ARG_MMXMODRM,argtype.ARG_NONE,1077),
  new asminstdata("punpckhbw",0x68,Proc.PROC_FROMPENTMMX,flags.FLAGS_MODRM|flags.FLAGS_32BIT,argtype.ARG_MREG,argtype.ARG_MMXMODRM,argtype.ARG_NONE,1078),
  new asminstdata("punpckhwd",0x69,Proc.PROC_FROMPENTMMX,flags.FLAGS_MODRM|flags.FLAGS_32BIT,argtype.ARG_MREG,argtype.ARG_MMXMODRM,argtype.ARG_NONE,1079),
  new asminstdata("punpckhdq",0x6a,Proc.PROC_FROMPENTMMX,flags.FLAGS_MODRM|flags.FLAGS_32BIT,argtype.ARG_MREG,argtype.ARG_MMXMODRM,argtype.ARG_NONE,1080),
  new asminstdata("packssdw",0x6b,Proc.PROC_FROMPENTMMX,flags.FLAGS_MODRM|flags.FLAGS_32BIT,argtype.ARG_MREG,argtype.ARG_MMXMODRM,argtype.ARG_NONE,1081),
  new asminstdata("movd",0x6e,Proc.PROC_FROMPENTMMX,flags.FLAGS_MODRM|flags.FLAGS_32BIT,argtype.ARG_MREG,argtype.ARG_MODRM,argtype.ARG_NONE,1082),
  new asminstdata("movq",0x6f,Proc.PROC_FROMPENTMMX,flags.FLAGS_MODRM|flags.FLAGS_32BIT,argtype.ARG_MREG,argtype.ARG_MMXMODRM,argtype.ARG_NONE,1083),
  new asminstdata("pshuf",0x70,Proc.PROC_FROMPENTIUM2,flags.FLAGS_MODRM|flags.FLAGS_32BIT,argtype.ARG_MREG,argtype.ARG_MMXMODRM,argtype.ARG_IMM8,1084),
  new asminstdata(null,0x71,Proc.PROC_FROMPENTMMX,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,1085), //subtable 0x0f/0x71
  new asminstdata(null,0x72,Proc.PROC_FROMPENTMMX,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,1086), //subtable 0x0f/0x72
  new asminstdata(null,0x73,Proc.PROC_FROMPENTMMX,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,1087), //subtable 0x0f/0x73
  new asminstdata("pcmpeqb",0x74,Proc.PROC_FROMPENTMMX,flags.FLAGS_MODRM|flags.FLAGS_32BIT,argtype.ARG_MREG,argtype.ARG_MMXMODRM,argtype.ARG_NONE,1088),
  new asminstdata("pcmpeqw",0x75,Proc.PROC_FROMPENTMMX,flags.FLAGS_MODRM|flags.FLAGS_32BIT,argtype.ARG_MREG,argtype.ARG_MMXMODRM,argtype.ARG_NONE,1089),
  new asminstdata("pcmpeqd",0x76,Proc.PROC_FROMPENTMMX,flags.FLAGS_MODRM|flags.FLAGS_32BIT,argtype.ARG_MREG,argtype.ARG_MMXMODRM,argtype.ARG_NONE,1090),
  new asminstdata("emms",0x77,Proc.PROC_FROMPENTMMX,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,1091),
  new asminstdata("movd",0x7e,Proc.PROC_FROMPENTMMX,flags.FLAGS_MODRM|flags.FLAGS_32BIT,argtype.ARG_MODRM,argtype.ARG_MREG,argtype.ARG_NONE,1092),
  new asminstdata("movq",0x7f,Proc.PROC_FROMPENTMMX,flags.FLAGS_MODRM|flags.FLAGS_32BIT,argtype.ARG_MMXMODRM,argtype.ARG_MREG,argtype.ARG_NONE,1093),
  new asminstdata("jo",0x80,Proc.PROC_FROM80386,flags.FLAGS_CJMP,argtype.ARG_RELIMM,argtype.ARG_NONE,argtype.ARG_NONE,1094),
  new asminstdata("jno",0x81,Proc.PROC_FROM80386,flags.FLAGS_CJMP,argtype.ARG_RELIMM,argtype.ARG_NONE,argtype.ARG_NONE,1095),
  new asminstdata("jc",0x82,Proc.PROC_FROM80386,flags.FLAGS_CJMP,argtype.ARG_RELIMM,argtype.ARG_NONE,argtype.ARG_NONE,1096),
  new asminstdata("jnc",0x83,Proc.PROC_FROM80386,flags.FLAGS_CJMP,argtype.ARG_RELIMM,argtype.ARG_NONE,argtype.ARG_NONE,1097),
  new asminstdata("jz",0x84,Proc.PROC_FROM80386,flags.FLAGS_CJMP,argtype.ARG_RELIMM,argtype.ARG_NONE,argtype.ARG_NONE,1098),
  new asminstdata("jnz",0x85,Proc.PROC_FROM80386,flags.FLAGS_CJMP,argtype.ARG_RELIMM,argtype.ARG_NONE,argtype.ARG_NONE,1099),
  new asminstdata("jbe",0x86,Proc.PROC_FROM80386,flags.FLAGS_CJMP,argtype.ARG_RELIMM,argtype.ARG_NONE,argtype.ARG_NONE,1100),
  new asminstdata("ja",0x87,Proc.PROC_FROM80386,flags.FLAGS_CJMP,argtype.ARG_RELIMM,argtype.ARG_NONE,argtype.ARG_NONE,1101),
  new asminstdata("js",0x88,Proc.PROC_FROM80386,flags.FLAGS_CJMP,argtype.ARG_RELIMM,argtype.ARG_NONE,argtype.ARG_NONE,1102),
  new asminstdata("jns",0x89,Proc.PROC_FROM80386,flags.FLAGS_CJMP,argtype.ARG_RELIMM,argtype.ARG_NONE,argtype.ARG_NONE,1103),
  new asminstdata("jpe",0x8a,Proc.PROC_FROM80386,flags.FLAGS_CJMP,argtype.ARG_RELIMM,argtype.ARG_NONE,argtype.ARG_NONE,1104),
  new asminstdata("jpo",0x8b,Proc.PROC_FROM80386,flags.FLAGS_CJMP,argtype.ARG_RELIMM,argtype.ARG_NONE,argtype.ARG_NONE,1105),
  new asminstdata("jl",0x8c,Proc.PROC_FROM80386,flags.FLAGS_CJMP,argtype.ARG_RELIMM,argtype.ARG_NONE,argtype.ARG_NONE,1106),
  new asminstdata("jge",0x8d,Proc.PROC_FROM80386,flags.FLAGS_CJMP,argtype.ARG_RELIMM,argtype.ARG_NONE,argtype.ARG_NONE,1107),
  new asminstdata("jle",0x8e,Proc.PROC_FROM80386,flags.FLAGS_CJMP,argtype.ARG_RELIMM,argtype.ARG_NONE,argtype.ARG_NONE,1108),
  new asminstdata("jg",0x8f,Proc.PROC_FROM80386,flags.FLAGS_CJMP,argtype.ARG_RELIMM,argtype.ARG_NONE,argtype.ARG_NONE,1109),
  new asminstdata("seto",0x90,Proc.PROC_FROM80386,flags.FLAGS_MODRM|flags.FLAGS_8BIT,argtype.ARG_MODRM,argtype.ARG_NONE,argtype.ARG_NONE,1110),
  new asminstdata("setno",0x91,Proc.PROC_FROM80386,flags.FLAGS_MODRM|flags.FLAGS_8BIT,argtype.ARG_MODRM,argtype.ARG_NONE,argtype.ARG_NONE,1111),
  new asminstdata("setc",0x92,Proc.PROC_FROM80386,flags.FLAGS_MODRM|flags.FLAGS_8BIT,argtype.ARG_MODRM,argtype.ARG_NONE,argtype.ARG_NONE,1112),
  new asminstdata("setnc",0x93,Proc.PROC_FROM80386,flags.FLAGS_MODRM|flags.FLAGS_8BIT,argtype.ARG_MODRM,argtype.ARG_NONE,argtype.ARG_NONE,1113),
  new asminstdata("setz",0x94,Proc.PROC_FROM80386,flags.FLAGS_MODRM|flags.FLAGS_8BIT,argtype.ARG_MODRM,argtype.ARG_NONE,argtype.ARG_NONE,1114),
  new asminstdata("setnz",0x95,Proc.PROC_FROM80386,flags.FLAGS_MODRM|flags.FLAGS_8BIT,argtype.ARG_MODRM,argtype.ARG_NONE,argtype.ARG_NONE,1115),
  new asminstdata("setbe",0x96,Proc.PROC_FROM80386,flags.FLAGS_MODRM|flags.FLAGS_8BIT,argtype.ARG_MODRM,argtype.ARG_NONE,argtype.ARG_NONE,1116),
  new asminstdata("seta",0x97,Proc.PROC_FROM80386,flags.FLAGS_MODRM|flags.FLAGS_8BIT,argtype.ARG_MODRM,argtype.ARG_NONE,argtype.ARG_NONE,1117),
  new asminstdata("sets",0x98,Proc.PROC_FROM80386,flags.FLAGS_MODRM|flags.FLAGS_8BIT,argtype.ARG_MODRM,argtype.ARG_NONE,argtype.ARG_NONE,1118),
  new asminstdata("setns",0x99,Proc.PROC_FROM80386,flags.FLAGS_MODRM|flags.FLAGS_8BIT,argtype.ARG_MODRM,argtype.ARG_NONE,argtype.ARG_NONE,1119),
  new asminstdata("setpe",0x9a,Proc.PROC_FROM80386,flags.FLAGS_MODRM|flags.FLAGS_8BIT,argtype.ARG_MODRM,argtype.ARG_NONE,argtype.ARG_NONE,1120),
  new asminstdata("setpo",0x9b,Proc.PROC_FROM80386,flags.FLAGS_MODRM|flags.FLAGS_8BIT,argtype.ARG_MODRM,argtype.ARG_NONE,argtype.ARG_NONE,1121),
  new asminstdata("setl",0x9c,Proc.PROC_FROM80386,flags.FLAGS_MODRM|flags.FLAGS_8BIT,argtype.ARG_MODRM,argtype.ARG_NONE,argtype.ARG_NONE,1122),
  new asminstdata("setge",0x9d,Proc.PROC_FROM80386,flags.FLAGS_MODRM|flags.FLAGS_8BIT,argtype.ARG_MODRM,argtype.ARG_NONE,argtype.ARG_NONE,1123),
  new asminstdata("setle",0x9e,Proc.PROC_FROM80386,flags.FLAGS_MODRM|flags.FLAGS_8BIT,argtype.ARG_MODRM,argtype.ARG_NONE,argtype.ARG_NONE,1124),
  new asminstdata("setg",0x9f,Proc.PROC_FROM80386,flags.FLAGS_MODRM|flags.FLAGS_8BIT,argtype.ARG_MODRM,argtype.ARG_NONE,argtype.ARG_NONE,1125),
  new asminstdata("push",0xa0,Proc.PROC_FROM80386,0,argtype.ARG_REG_FS,argtype.ARG_NONE,argtype.ARG_NONE,1126),
  new asminstdata("pop",0xa1,Proc.PROC_FROM80386,0,argtype.ARG_REG_FS,argtype.ARG_NONE,argtype.ARG_NONE,1127),
  new asminstdata("cpuid",0xa2,Proc.PROC_FROM80486,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,1128),
  new asminstdata("bt",0xa3,Proc.PROC_FROM80386,flags.FLAGS_MODRM,argtype.ARG_MODRM,argtype.ARG_REG,argtype.ARG_NONE,1129),
  new asminstdata("shld",0xa4,Proc.PROC_FROM80386,flags.FLAGS_MODRM,argtype.ARG_MODRM,argtype.ARG_REG,argtype.ARG_IMM8,1130),
  new asminstdata("shld",0xa5,Proc.PROC_FROM80386,flags.FLAGS_MODRM,argtype.ARG_MODRM,argtype.ARG_REG,argtype.ARG_REG_CL,1131),
  new asminstdata("push",0xa8,Proc.PROC_FROM80386,0,argtype.ARG_REG_GS,argtype.ARG_NONE,argtype.ARG_NONE,1132),
  new asminstdata("pop",0xa9,Proc.PROC_FROM80386,0,argtype.ARG_REG_GS,argtype.ARG_NONE,argtype.ARG_NONE,1133),
  new asminstdata("rsm",0xaa,Proc.PROC_FROM80386,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,1134),
  new asminstdata("bts",0xab,Proc.PROC_FROM80386,flags.FLAGS_MODRM,argtype.ARG_MODRM,argtype.ARG_REG,argtype.ARG_NONE,1135),
  new asminstdata("shrd",0xac,Proc.PROC_FROM80386,flags.FLAGS_MODRM,argtype.ARG_MODRM,argtype.ARG_REG,argtype.ARG_IMM8,1136),
  new asminstdata("shrd",0xad,Proc.PROC_FROM80386,flags.FLAGS_MODRM,argtype.ARG_MODRM,argtype.ARG_REG,argtype.ARG_REG_CL,1137),
  new asminstdata(null,0xae,Proc.PROC_FROMPENTIUM2,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,1138), //subtable 0x0f/0xae
  new asminstdata("imul",0xaf,Proc.PROC_FROM80386,flags.FLAGS_MODRM,argtype.ARG_REG,argtype.ARG_MODRM,argtype.ARG_NONE,1139),
  new asminstdata("cmpxchg",0xb0,Proc.PROC_FROM80486,flags.FLAGS_MODRM|flags.FLAGS_8BIT,argtype.ARG_MODRM,argtype.ARG_REG,argtype.ARG_NONE,1140),
  new asminstdata("cmpxchg",0xb1,Proc.PROC_FROM80486,flags.FLAGS_MODRM,argtype.ARG_MODRM,argtype.ARG_REG,argtype.ARG_NONE,1141),
  new asminstdata("lss",0xb2,Proc.PROC_FROM80386,flags.FLAGS_MODRM,argtype.ARG_REG,argtype.ARG_MODRM,argtype.ARG_NONE,1142),
  new asminstdata("btr",0xb3,Proc.PROC_FROM80386,flags.FLAGS_MODRM,argtype.ARG_MODRM,argtype.ARG_REG,argtype.ARG_NONE,1143),
  new asminstdata("lfs",0xb4,Proc.PROC_FROM80386,flags.FLAGS_MODRM,argtype.ARG_REG,argtype.ARG_MODRM,argtype.ARG_NONE,1144),
  new asminstdata("lgs",0xb5,Proc.PROC_FROM80386,flags.FLAGS_MODRM,argtype.ARG_REG,argtype.ARG_MODRM,argtype.ARG_NONE,1145),
  new asminstdata("movzx",0xb6,Proc.PROC_FROM80386,flags.FLAGS_MODRM,argtype.ARG_REG,argtype.ARG_MODRM8,argtype.ARG_NONE,1146),
  new asminstdata("movzx",0xb7,Proc.PROC_FROM80386,flags.FLAGS_MODRM,argtype.ARG_REG,argtype.ARG_MODRM16,argtype.ARG_NONE,1147),
  new asminstdata("ud1",0xb9,Proc.PROC_FROM8086,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,1148),
  new asminstdata(null,0xba,Proc.PROC_FROM8086,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,1149), //subtable 0x0f/0xba
  new asminstdata("btc",0xbb,Proc.PROC_FROM80386,flags.FLAGS_MODRM,argtype.ARG_MODRM,argtype.ARG_REG,argtype.ARG_NONE,1150),
  new asminstdata("bsf",0xbc,Proc.PROC_FROM80386,flags.FLAGS_MODRM,argtype.ARG_REG,argtype.ARG_MODRM,argtype.ARG_NONE,1151),
  new asminstdata("bsr",0xbd,Proc.PROC_FROM80386,flags.FLAGS_MODRM,argtype.ARG_REG,argtype.ARG_MODRM,argtype.ARG_NONE,1152),
  new asminstdata("movsx",0xbe,Proc.PROC_FROM80386,flags.FLAGS_MODRM,argtype.ARG_REG,argtype.ARG_MODRM8,argtype.ARG_NONE,1153),
  new asminstdata("movsx",0xbf,Proc.PROC_FROM80386,flags.FLAGS_MODRM,argtype.ARG_REG,argtype.ARG_MODRM16,argtype.ARG_NONE,1154),
  new asminstdata("xadd",0xc0,Proc.PROC_FROM80486,flags.FLAGS_MODRM|flags.FLAGS_8BIT,argtype.ARG_MODRM,argtype.ARG_REG,argtype.ARG_NONE,1155),
  new asminstdata("xadd",0xc1,Proc.PROC_FROM80486,flags.FLAGS_MODRM,argtype.ARG_MODRM,argtype.ARG_NONE,argtype.ARG_NONE,1156),
  new asminstdata(null,0xc2,Proc.PROC_FROMPENTIUM2,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,1157), //subtable 0x0f/0xc7
  new asminstdata("pinsrw",0xc4,Proc.PROC_FROMPENTIUM2,flags.FLAGS_MODRM|flags.FLAGS_32BIT,argtype.ARG_MREG,argtype.ARG_MODRM,argtype.ARG_IMM8,1158),
  new asminstdata("pextrw",0xc5,Proc.PROC_FROMPENTIUM2,flags.FLAGS_MODRM|flags.FLAGS_32BIT,argtype.ARG_REG,argtype.ARG_MMXMODRM,argtype.ARG_IMM8,1159),
  new asminstdata("shufps",0xc6,Proc.PROC_FROMPENTIUM2,flags.FLAGS_MODRM|flags.FLAGS_32BIT,argtype.ARG_XREG,argtype.ARG_XMMMODRM,argtype.ARG_IMM8,1160),
  new asminstdata(null,0xc7,Proc.PROC_FROMPENTMMX,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,1161), //subtable 0x0f/0xc7
  new asminstdata("bswap",0xc8,Proc.PROC_FROM80486,flags.FLAGS_32BIT,argtype.ARG_REG_AX,argtype.ARG_NONE,argtype.ARG_NONE,1162),
  new asminstdata("bswap",0xc9,Proc.PROC_FROM80486,flags.FLAGS_32BIT,argtype.ARG_REG_CX,argtype.ARG_NONE,argtype.ARG_NONE,1163),
  new asminstdata("bswap",0xca,Proc.PROC_FROM80486,flags.FLAGS_32BIT,argtype.ARG_REG_DX,argtype.ARG_NONE,argtype.ARG_NONE,1164),
  new asminstdata("bswap",0xcb,Proc.PROC_FROM80486,flags.FLAGS_32BIT,argtype.ARG_REG_BX,argtype.ARG_NONE,argtype.ARG_NONE,1165),
  new asminstdata("bswap",0xcc,Proc.PROC_FROM80486,flags.FLAGS_32BIT,argtype.ARG_REG_SP,argtype.ARG_NONE,argtype.ARG_NONE,1166),
  new asminstdata("bswap",0xcd,Proc.PROC_FROM80486,flags.FLAGS_32BIT,argtype.ARG_REG_BP,argtype.ARG_NONE,argtype.ARG_NONE,1167),
  new asminstdata("bswap",0xce,Proc.PROC_FROM80486,flags.FLAGS_32BIT,argtype.ARG_REG_SI,argtype.ARG_NONE,argtype.ARG_NONE,1168),
  new asminstdata("bswap",0xcf,Proc.PROC_FROM80486,flags.FLAGS_32BIT,argtype.ARG_REG_DI,argtype.ARG_NONE,argtype.ARG_NONE,1169),
  new asminstdata("psrlw",0xd1,Proc.PROC_FROMPENTMMX,flags.FLAGS_MODRM|flags.FLAGS_32BIT,argtype.ARG_MREG,argtype.ARG_MMXMODRM,argtype.ARG_NONE,1170),
  new asminstdata("psrld",0xd2,Proc.PROC_FROMPENTMMX,flags.FLAGS_MODRM|flags.FLAGS_32BIT,argtype.ARG_MREG,argtype.ARG_MMXMODRM,argtype.ARG_NONE,1171),
  new asminstdata("psrlq",0xd3,Proc.PROC_FROMPENTMMX,flags.FLAGS_MODRM|flags.FLAGS_32BIT,argtype.ARG_MREG,argtype.ARG_MMXMODRM,argtype.ARG_NONE,1172),
  new asminstdata("pmullw",0xd5,Proc.PROC_FROMPENTMMX,flags.FLAGS_MODRM|flags.FLAGS_32BIT,argtype.ARG_MREG,argtype.ARG_MMXMODRM,argtype.ARG_NONE,1173),
  new asminstdata("pmovmskb",0xd7,Proc.PROC_FROMPENTIUM2,flags.FLAGS_MODRM|flags.FLAGS_32BIT,argtype.ARG_REG,argtype.ARG_MMXMODRM,argtype.ARG_NONE,1174),
  new asminstdata("psubusb",0xd8,Proc.PROC_FROMPENTMMX,flags.FLAGS_MODRM|flags.FLAGS_32BIT,argtype.ARG_MREG,argtype.ARG_MMXMODRM,argtype.ARG_NONE,1175),
  new asminstdata("psubusw",0xd9,Proc.PROC_FROMPENTMMX,flags.FLAGS_MODRM|flags.FLAGS_32BIT,argtype.ARG_MREG,argtype.ARG_MMXMODRM,argtype.ARG_NONE,1176),
  new asminstdata("pminub",0xda,Proc.PROC_FROMPENTIUM2,flags.FLAGS_MODRM|flags.FLAGS_32BIT,argtype.ARG_MREG,argtype.ARG_MMXMODRM,argtype.ARG_NONE,1177),
  new asminstdata("pand",0xdb,Proc.PROC_FROMPENTMMX,flags.FLAGS_MODRM|flags.FLAGS_32BIT,argtype.ARG_MREG,argtype.ARG_MMXMODRM,argtype.ARG_NONE,1178),
  new asminstdata("paddusb",0xdc,Proc.PROC_FROMPENTMMX,flags.FLAGS_MODRM|flags.FLAGS_32BIT,argtype.ARG_MREG,argtype.ARG_MMXMODRM,argtype.ARG_NONE,1179),
  new asminstdata("paddusw",0xdd,Proc.PROC_FROMPENTMMX,flags.FLAGS_MODRM|flags.FLAGS_32BIT,argtype.ARG_MREG,argtype.ARG_MMXMODRM,argtype.ARG_NONE,1180),
  new asminstdata("pmaxub",0xde,Proc.PROC_FROMPENTIUM2,flags.FLAGS_MODRM|flags.FLAGS_32BIT,argtype.ARG_MREG,argtype.ARG_MMXMODRM,argtype.ARG_NONE,1181),
  new asminstdata("pandn",0xdf,Proc.PROC_FROMPENTMMX,flags.FLAGS_MODRM|flags.FLAGS_32BIT,argtype.ARG_MREG,argtype.ARG_MMXMODRM,argtype.ARG_NONE,1182),
  new asminstdata("pavgb",0xe0,Proc.PROC_FROMPENTIUM2,flags.FLAGS_MODRM|flags.FLAGS_32BIT,argtype.ARG_MREG,argtype.ARG_MMXMODRM,argtype.ARG_NONE,1183),
  new asminstdata("psraw",0xe1,Proc.PROC_FROMPENTMMX,flags.FLAGS_MODRM|flags.FLAGS_32BIT,argtype.ARG_MREG,argtype.ARG_MMXMODRM,argtype.ARG_NONE,1184),
  new asminstdata("psrad",0xe2,Proc.PROC_FROMPENTMMX,flags.FLAGS_MODRM|flags.FLAGS_32BIT,argtype.ARG_MREG,argtype.ARG_MMXMODRM,argtype.ARG_NONE,1185),
  new asminstdata("pavgw",0xe3,Proc.PROC_FROMPENTIUM2,flags.FLAGS_MODRM|flags.FLAGS_32BIT,argtype.ARG_MREG,argtype.ARG_MMXMODRM,argtype.ARG_NONE,1186),
  new asminstdata("pmulhuw",0xe4,Proc.PROC_FROMPENTIUM2,flags.FLAGS_MODRM|flags.FLAGS_32BIT,argtype.ARG_MREG,argtype.ARG_MMXMODRM,argtype.ARG_NONE,1187),
  new asminstdata("pmulhw",0xe5,Proc.PROC_FROMPENTMMX,flags.FLAGS_MODRM|flags.FLAGS_32BIT,argtype.ARG_MREG,argtype.ARG_MMXMODRM,argtype.ARG_NONE,1188),
  new asminstdata("movntq",0xe7,Proc.PROC_FROMPENTIUM2,flags.FLAGS_MODRM|flags.FLAGS_32BIT,argtype.ARG_MMXMODRM,argtype.ARG_MREG,argtype.ARG_NONE,1189),
  new asminstdata("psubsb",0xe8,Proc.PROC_FROMPENTMMX,flags.FLAGS_MODRM|flags.FLAGS_32BIT,argtype.ARG_MREG,argtype.ARG_MMXMODRM,argtype.ARG_NONE,1190),
  new asminstdata("psubsw",0xe9,Proc.PROC_FROMPENTMMX,flags.FLAGS_MODRM|flags.FLAGS_32BIT,argtype.ARG_MREG,argtype.ARG_MMXMODRM,argtype.ARG_NONE,1191),
  new asminstdata("pminsw",0xea,Proc.PROC_FROMPENTIUM2,flags.FLAGS_MODRM|flags.FLAGS_32BIT,argtype.ARG_MREG,argtype.ARG_MMXMODRM,argtype.ARG_NONE,1192),
  new asminstdata("por",0xeb,Proc.PROC_FROMPENTMMX,flags.FLAGS_MODRM|flags.FLAGS_32BIT,argtype.ARG_MREG,argtype.ARG_MMXMODRM,argtype.ARG_NONE,1193),
  new asminstdata("paddsb",0xec,Proc.PROC_FROMPENTMMX,flags.FLAGS_MODRM|flags.FLAGS_32BIT,argtype.ARG_MREG,argtype.ARG_MMXMODRM,argtype.ARG_NONE,1194),
  new asminstdata("paddsw",0xed,Proc.PROC_FROMPENTMMX,flags.FLAGS_MODRM|flags.FLAGS_32BIT,argtype.ARG_MREG,argtype.ARG_MMXMODRM,argtype.ARG_NONE,1195),
  new asminstdata("pmaxsw",0xee,Proc.PROC_FROMPENTIUM2,flags.FLAGS_MODRM|flags.FLAGS_32BIT,argtype.ARG_MREG,argtype.ARG_MMXMODRM,argtype.ARG_NONE,1196),
  new asminstdata("pxor",0xef,Proc.PROC_FROMPENTMMX,flags.FLAGS_MODRM|flags.FLAGS_32BIT,argtype.ARG_MREG,argtype.ARG_MMXMODRM,argtype.ARG_NONE,1197),
  new asminstdata("psllw",0xf1,Proc.PROC_FROMPENTMMX,flags.FLAGS_MODRM|flags.FLAGS_32BIT,argtype.ARG_MREG,argtype.ARG_MMXMODRM,argtype.ARG_NONE,1198),
  new asminstdata("pslld",0xf2,Proc.PROC_FROMPENTMMX,flags.FLAGS_MODRM|flags.FLAGS_32BIT,argtype.ARG_MREG,argtype.ARG_MMXMODRM,argtype.ARG_NONE,1199),
  new asminstdata("psllq",0xf3,Proc.PROC_FROMPENTMMX,flags.FLAGS_MODRM|flags.FLAGS_32BIT,argtype.ARG_MREG,argtype.ARG_MMXMODRM,argtype.ARG_NONE,1200),
  new asminstdata("pmaddwd",0xf5,Proc.PROC_FROMPENTMMX,flags.FLAGS_MODRM|flags.FLAGS_32BIT,argtype.ARG_MREG,argtype.ARG_MMXMODRM,argtype.ARG_NONE,1201),
  new asminstdata("psadbw",0xf6,Proc.PROC_FROMPENTIUM2,flags.FLAGS_MODRM|flags.FLAGS_32BIT,argtype.ARG_MREG,argtype.ARG_MMXMODRM,argtype.ARG_NONE,1202),
  new asminstdata("maskmovq",0xf7,Proc.PROC_FROMPENTIUM2,flags.FLAGS_MODRM|flags.FLAGS_32BIT,argtype.ARG_MREG,argtype.ARG_MMXMODRM,argtype.ARG_NONE,1203),
  new asminstdata("psubb",0xf8,Proc.PROC_FROMPENTMMX,flags.FLAGS_MODRM|flags.FLAGS_32BIT,argtype.ARG_MREG,argtype.ARG_MMXMODRM,argtype.ARG_NONE,1204),
  new asminstdata("psubw",0xf9,Proc.PROC_FROMPENTMMX,flags.FLAGS_MODRM|flags.FLAGS_32BIT,argtype.ARG_MREG,argtype.ARG_MMXMODRM,argtype.ARG_NONE,1205),
  new asminstdata("psubd",0xfa,Proc.PROC_FROMPENTMMX,flags.FLAGS_MODRM|flags.FLAGS_32BIT,argtype.ARG_MREG,argtype.ARG_MMXMODRM,argtype.ARG_NONE,1206),
  new asminstdata("paddb",0xfc,Proc.PROC_FROMPENTMMX,flags.FLAGS_MODRM|flags.FLAGS_32BIT,argtype.ARG_MREG,argtype.ARG_MMXMODRM,argtype.ARG_NONE,1207),
  new asminstdata("paddw",0xfd,Proc.PROC_FROMPENTMMX,flags.FLAGS_MODRM|flags.FLAGS_32BIT,argtype.ARG_MREG,argtype.ARG_MMXMODRM,argtype.ARG_NONE,1208),
  new asminstdata("paddd",0xfe,Proc.PROC_FROMPENTMMX,flags.FLAGS_MODRM|flags.FLAGS_32BIT,argtype.ARG_MREG,argtype.ARG_MMXMODRM,argtype.ARG_NONE,1209),
  new asminstdata(null,0x00,0,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,0) //endCode marker - processor=0 & opcode=0
};

        // subtable 0x80
        // - num is encoding of modrm bits 5,4,3 only
        public static asminstdata[] asm86sub80 = new asminstdata[]{ new asminstdata("add",0x0,Proc.PROC_FROM8086,flags.FLAGS_MODRM|flags.FLAGS_8BIT,argtype.ARG_MODRM,argtype.ARG_IMM8,argtype.ARG_NONE,2000),
  new asminstdata("or",0x1,Proc.PROC_FROM8086,flags.FLAGS_MODRM|flags.FLAGS_8BIT,argtype.ARG_MODRM,argtype.ARG_IMM8,argtype.ARG_NONE,2001),
  new asminstdata("adc",0x2,Proc.PROC_FROM8086,flags.FLAGS_MODRM|flags.FLAGS_8BIT,argtype.ARG_MODRM,argtype.ARG_IMM8,argtype.ARG_NONE,2002),
  new asminstdata("sbb",0x3,Proc.PROC_FROM8086,flags.FLAGS_MODRM|flags.FLAGS_8BIT,argtype.ARG_MODRM,argtype.ARG_IMM8,argtype.ARG_NONE,2003),
  new asminstdata("and",0x4,Proc.PROC_FROM8086,flags.FLAGS_MODRM|flags.FLAGS_8BIT,argtype.ARG_MODRM,argtype.ARG_IMM8,argtype.ARG_NONE,2004),
  new asminstdata("sub",0x5,Proc.PROC_FROM8086,flags.FLAGS_MODRM|flags.FLAGS_8BIT,argtype.ARG_MODRM,argtype.ARG_IMM8,argtype.ARG_NONE,2005),
  new asminstdata("xor",0x6,Proc.PROC_FROM8086,flags.FLAGS_MODRM|flags.FLAGS_8BIT,argtype.ARG_MODRM,argtype.ARG_IMM8,argtype.ARG_NONE,2006),
  new asminstdata("cmp",0x7,Proc.PROC_FROM8086,flags.FLAGS_MODRM|flags.FLAGS_8BIT,argtype.ARG_MODRM,argtype.ARG_IMM8,argtype.ARG_NONE,2007),
  new asminstdata(null,0x00,0,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,0)  //endCode marker - processor=0 & opcode=0
};

        // subtable 0x81
        // - num is encoding of modrm bits 5,4,3 only
        public static asminstdata[] asm86sub81 = new asminstdata[]
{ new asminstdata("add",0x0,Proc.PROC_FROM8086,flags.FLAGS_MODRM|flags.FLAGS_OMODE16,argtype.ARG_MODRM,argtype.ARG_IMM,argtype.ARG_NONE,3000),
  new asminstdata("add",0x0,Proc.PROC_FROM80386,flags.FLAGS_MODRM|flags.FLAGS_OMODE32,argtype.ARG_MODRM,argtype.ARG_IMM,argtype.ARG_NONE,3001),
  new asminstdata("or",0x1,Proc.PROC_FROM8086,flags.FLAGS_MODRM|flags.FLAGS_OMODE16,argtype.ARG_MODRM,argtype.ARG_IMM,argtype.ARG_NONE,3002),
  new asminstdata("or",0x1,Proc.PROC_FROM80386,flags.FLAGS_MODRM|flags.FLAGS_OMODE32,argtype.ARG_MODRM,argtype.ARG_IMM,argtype.ARG_NONE,3003),
  new asminstdata("adc",0x2,Proc.PROC_FROM8086,flags.FLAGS_MODRM|flags.FLAGS_OMODE16,argtype.ARG_MODRM,argtype.ARG_IMM,argtype.ARG_NONE,3004),
  new asminstdata("adc",0x2,Proc.PROC_FROM80386,flags.FLAGS_MODRM|flags.FLAGS_OMODE32,argtype.ARG_MODRM,argtype.ARG_IMM,argtype.ARG_NONE,3005),
  new asminstdata("sbb",0x3,Proc.PROC_FROM8086,flags.FLAGS_MODRM|flags.FLAGS_OMODE16,argtype.ARG_MODRM,argtype.ARG_IMM,argtype.ARG_NONE,3006),
  new asminstdata("sbb",0x3,Proc.PROC_FROM80386,flags.FLAGS_MODRM|flags.FLAGS_OMODE32,argtype.ARG_MODRM,argtype.ARG_IMM,argtype.ARG_NONE,3007),
  new asminstdata("and",0x4,Proc.PROC_FROM8086,flags.FLAGS_MODRM|flags.FLAGS_OMODE16,argtype.ARG_MODRM,argtype.ARG_IMM,argtype.ARG_NONE,3008),
  new asminstdata("and",0x4,Proc.PROC_FROM80386,flags.FLAGS_MODRM|flags.FLAGS_OMODE32,argtype.ARG_MODRM,argtype.ARG_IMM,argtype.ARG_NONE,3009),
  new asminstdata("sub",0x5,Proc.PROC_FROM8086,flags.FLAGS_MODRM|flags.FLAGS_OMODE16,argtype.ARG_MODRM,argtype.ARG_IMM,argtype.ARG_NONE,3010),
  new asminstdata("sub",0x5,Proc.PROC_FROM80386,flags.FLAGS_MODRM|flags.FLAGS_OMODE32,argtype.ARG_MODRM,argtype.ARG_IMM,argtype.ARG_NONE,3011),
  new asminstdata("xor",0x6,Proc.PROC_FROM8086,flags.FLAGS_MODRM|flags.FLAGS_OMODE16,argtype.ARG_MODRM,argtype.ARG_IMM,argtype.ARG_NONE,3012),
  new asminstdata("xor",0x6,Proc.PROC_FROM80386,flags.FLAGS_MODRM|flags.FLAGS_OMODE32,argtype.ARG_MODRM,argtype.ARG_IMM,argtype.ARG_NONE,3013),
  new asminstdata("cmp",0x7,Proc.PROC_FROM8086,flags.FLAGS_MODRM|flags.FLAGS_OMODE16,argtype.ARG_MODRM,argtype.ARG_IMM,argtype.ARG_NONE,3014),
  new asminstdata("cmp",0x7,Proc.PROC_FROM80386,flags.FLAGS_MODRM|flags.FLAGS_OMODE32,argtype.ARG_MODRM,argtype.ARG_IMM,argtype.ARG_NONE,3016),
  new asminstdata(null,0x00,0,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,0)  //endCode marker - processor=0 & opcode=0
};

        // subtable 0x82
        // - num is encoding of modrm bits 5,4,3 only
        public static asminstdata[] asm86sub82 = new asminstdata[]
{ new asminstdata("add",0x0,Proc.PROC_FROM8086,flags.FLAGS_MODRM|flags.FLAGS_8BIT,argtype.ARG_MODRM,argtype.ARG_SIMM8,argtype.ARG_NONE,4000),
  new asminstdata("or",0x1,Proc.PROC_FROM8086,flags.FLAGS_MODRM|flags.FLAGS_8BIT,argtype.ARG_MODRM,argtype.ARG_SIMM8,argtype.ARG_NONE,4001),
  new asminstdata("adc",0x2,Proc.PROC_FROM8086,flags.FLAGS_MODRM|flags.FLAGS_8BIT,argtype.ARG_MODRM,argtype.ARG_SIMM8,argtype.ARG_NONE,4002),
  new asminstdata("sbb",0x3,Proc.PROC_FROM8086,flags.FLAGS_MODRM|flags.FLAGS_8BIT,argtype.ARG_MODRM,argtype.ARG_SIMM8,argtype.ARG_NONE,4003),
  new asminstdata("and",0x4,Proc.PROC_FROM8086,flags.FLAGS_MODRM|flags.FLAGS_8BIT,argtype.ARG_MODRM,argtype.ARG_SIMM8,argtype.ARG_NONE,4004),
  new asminstdata("sub",0x5,Proc.PROC_FROM8086,flags.FLAGS_MODRM|flags.FLAGS_8BIT,argtype.ARG_MODRM,argtype.ARG_SIMM8,argtype.ARG_NONE,4005),
  new asminstdata("xor",0x6,Proc.PROC_FROM8086,flags.FLAGS_MODRM|flags.FLAGS_8BIT,argtype.ARG_MODRM,argtype.ARG_SIMM8,argtype.ARG_NONE,4006),
  new asminstdata("cmp",0x7,Proc.PROC_FROM8086,flags.FLAGS_MODRM|flags.FLAGS_8BIT,argtype.ARG_MODRM,argtype.ARG_SIMM8,argtype.ARG_NONE,4007),
  new asminstdata(null,0x00,0,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,0)  //endCode marker - processor=0 & opcode=0
};

        // subtable 0x83
        // - num is encoding of modrm bits 5,4,3 only
        public static asminstdata[] asm86sub83 = new asminstdata[]
{ new asminstdata("add",0x0,Proc.PROC_FROM8086,flags.FLAGS_MODRM|flags.FLAGS_OMODE16,argtype.ARG_MODRM,argtype.ARG_SIMM8,argtype.ARG_NONE,5000),
  new asminstdata("add",0x0,Proc.PROC_FROM80386,flags.FLAGS_MODRM|flags.FLAGS_OMODE32,argtype.ARG_MODRM,argtype.ARG_SIMM8,argtype.ARG_NONE,5001),
  new asminstdata("or",0x1,Proc.PROC_FROM8086,flags.FLAGS_MODRM|flags.FLAGS_OMODE16,argtype.ARG_MODRM,argtype.ARG_SIMM8,argtype.ARG_NONE,5002),
  new asminstdata("or",0x1,Proc.PROC_FROM80386,flags.FLAGS_MODRM|flags.FLAGS_OMODE32,argtype.ARG_MODRM,argtype.ARG_SIMM8,argtype.ARG_NONE,5003),
  new asminstdata("adc",0x2,Proc.PROC_FROM8086,flags.FLAGS_MODRM|flags.FLAGS_OMODE16,argtype.ARG_MODRM,argtype.ARG_SIMM8,argtype.ARG_NONE,5004),
  new asminstdata("adc",0x2,Proc.PROC_FROM80386,flags.FLAGS_MODRM|flags.FLAGS_OMODE32,argtype.ARG_MODRM,argtype.ARG_SIMM8,argtype.ARG_NONE,5005),
  new asminstdata("sbb",0x3,Proc.PROC_FROM8086,flags.FLAGS_MODRM|flags.FLAGS_OMODE16,argtype.ARG_MODRM,argtype.ARG_SIMM8,argtype.ARG_NONE,5006),
  new asminstdata("sbb",0x3,Proc.PROC_FROM80386,flags.FLAGS_MODRM|flags.FLAGS_OMODE32,argtype.ARG_MODRM,argtype.ARG_SIMM8,argtype.ARG_NONE,5007),
  new asminstdata("and",0x4,Proc.PROC_FROM8086,flags.FLAGS_MODRM|flags.FLAGS_OMODE16,argtype.ARG_MODRM,argtype.ARG_SIMM8,argtype.ARG_NONE,5008),
  new asminstdata("and",0x4,Proc.PROC_FROM80386,flags.FLAGS_MODRM|flags.FLAGS_OMODE32,argtype.ARG_MODRM,argtype.ARG_SIMM8,argtype.ARG_NONE,5009),
  new asminstdata("sub",0x5,Proc.PROC_FROM8086,flags.FLAGS_MODRM|flags.FLAGS_OMODE16,argtype.ARG_MODRM,argtype.ARG_SIMM8,argtype.ARG_NONE,5010),
  new asminstdata("sub",0x5,Proc.PROC_FROM80386,flags.FLAGS_MODRM|flags.FLAGS_OMODE32,argtype.ARG_MODRM,argtype.ARG_SIMM8,argtype.ARG_NONE,5011),
  new asminstdata("xor",0x6,Proc.PROC_FROM8086,flags.FLAGS_MODRM|flags.FLAGS_OMODE16,argtype.ARG_MODRM,argtype.ARG_SIMM8,argtype.ARG_NONE,5012),
  new asminstdata("xor",0x6,Proc.PROC_FROM80386,flags.FLAGS_MODRM|flags.FLAGS_OMODE32,argtype.ARG_MODRM,argtype.ARG_SIMM8,argtype.ARG_NONE,5013),
  new asminstdata("cmp",0x7,Proc.PROC_FROM8086,flags.FLAGS_MODRM|flags.FLAGS_OMODE16,argtype.ARG_MODRM,argtype.ARG_SIMM8,argtype.ARG_NONE,5014),
  new asminstdata("cmp",0x7,Proc.PROC_FROM80386,flags.FLAGS_MODRM|flags.FLAGS_OMODE32,argtype.ARG_MODRM,argtype.ARG_SIMM8,argtype.ARG_NONE,5015),
  new asminstdata(null,0x00,0,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,0)  //endCode marker - processor=0 & opcode=0
};

        // subtable 0xc0
        // - num is encoding of modrm bits 5,4,3 only
        public static asminstdata[] asm86subc0 =
        new asminstdata[] { new asminstdata("rol",0x0,Proc.PROC_FROM80286,flags.FLAGS_MODRM|flags.FLAGS_8BIT,argtype.ARG_MODRM,argtype.ARG_IMM8,argtype.ARG_NONE,6000),
  new asminstdata("ror",0x1,Proc.PROC_FROM80286,flags.FLAGS_MODRM|flags.FLAGS_8BIT,argtype.ARG_MODRM,argtype.ARG_IMM8,argtype.ARG_NONE,6001),
  new asminstdata("rcl",0x2,Proc.PROC_FROM80286,flags.FLAGS_MODRM|flags.FLAGS_8BIT,argtype.ARG_MODRM,argtype.ARG_IMM8,argtype.ARG_NONE,6002),
  new asminstdata("rcr",0x3,Proc.PROC_FROM80286,flags.FLAGS_MODRM|flags.FLAGS_8BIT,argtype.ARG_MODRM,argtype.ARG_IMM8,argtype.ARG_NONE,6003),
  new asminstdata("shl",0x4,Proc.PROC_FROM80286,flags.FLAGS_MODRM|flags.FLAGS_8BIT,argtype.ARG_MODRM,argtype.ARG_IMM8,argtype.ARG_NONE,6004),
  new asminstdata("shr",0x5,Proc.PROC_FROM80286,flags.FLAGS_MODRM|flags.FLAGS_8BIT,argtype.ARG_MODRM,argtype.ARG_IMM8,argtype.ARG_NONE,6005),
  new asminstdata("sal",0x6,Proc.PROC_FROM80286,flags.FLAGS_MODRM|flags.FLAGS_8BIT,argtype.ARG_MODRM,argtype.ARG_IMM8,argtype.ARG_NONE,6006),
  new asminstdata("sar",0x7,Proc.PROC_FROM80286,flags.FLAGS_MODRM|flags.FLAGS_8BIT,argtype.ARG_MODRM,argtype.ARG_IMM8,argtype.ARG_NONE,6007),
  new asminstdata(null,0x00,0,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,0)  //endCode marker - processor=0 & opcode=0
};

        // subtable 0xc1
        // - num is encoding of modrm bits 5,4,3 only
        public static asminstdata[] asm86subc1 = new asminstdata[]
{ new asminstdata("rol",0x0,Proc.PROC_FROM80286,flags.FLAGS_MODRM|flags.FLAGS_OMODE16,argtype.ARG_MODRM,argtype.ARG_IMM8,argtype.ARG_NONE,7000),
  new asminstdata("rol",0x0,Proc.PROC_FROM80386,flags.FLAGS_MODRM|flags.FLAGS_OMODE32,argtype.ARG_MODRM,argtype.ARG_IMM8,argtype.ARG_NONE,7001),
  new asminstdata("ror",0x1,Proc.PROC_FROM80286,flags.FLAGS_MODRM|flags.FLAGS_OMODE16,argtype.ARG_MODRM,argtype.ARG_IMM8,argtype.ARG_NONE,7002),
  new asminstdata("ror",0x1,Proc.PROC_FROM80386,flags.FLAGS_MODRM|flags.FLAGS_OMODE32,argtype.ARG_MODRM,argtype.ARG_IMM8,argtype.ARG_NONE,7003),
  new asminstdata("rcl",0x2,Proc.PROC_FROM80286,flags.FLAGS_MODRM|flags.FLAGS_OMODE16,argtype.ARG_MODRM,argtype.ARG_IMM8,argtype.ARG_NONE,7004),
  new asminstdata("rcl",0x2,Proc.PROC_FROM80386,flags.FLAGS_MODRM|flags.FLAGS_OMODE32,argtype.ARG_MODRM,argtype.ARG_IMM8,argtype.ARG_NONE,7005),
  new asminstdata("rcr",0x3,Proc.PROC_FROM80286,flags.FLAGS_MODRM|flags.FLAGS_OMODE16,argtype.ARG_MODRM,argtype.ARG_IMM8,argtype.ARG_NONE,7006),
  new asminstdata("rcr",0x3,Proc.PROC_FROM80386,flags.FLAGS_MODRM|flags.FLAGS_OMODE32,argtype.ARG_MODRM,argtype.ARG_IMM8,argtype.ARG_NONE,7007),
  new asminstdata("shl",0x4,Proc.PROC_FROM80286,flags.FLAGS_MODRM|flags.FLAGS_OMODE16,argtype.ARG_MODRM,argtype.ARG_IMM8,argtype.ARG_NONE,7008),
  new asminstdata("shl",0x4,Proc.PROC_FROM80386,flags.FLAGS_MODRM|flags.FLAGS_OMODE32,argtype.ARG_MODRM,argtype.ARG_IMM8,argtype.ARG_NONE,7009),
  new asminstdata("shr",0x5,Proc.PROC_FROM80286,flags.FLAGS_MODRM|flags.FLAGS_OMODE16,argtype.ARG_MODRM,argtype.ARG_IMM8,argtype.ARG_NONE,7010),
  new asminstdata("shr",0x5,Proc.PROC_FROM80386,flags.FLAGS_MODRM|flags.FLAGS_OMODE32,argtype.ARG_MODRM,argtype.ARG_IMM8,argtype.ARG_NONE,7011),
  new asminstdata("sal",0x6,Proc.PROC_FROM80286,flags.FLAGS_MODRM|flags.FLAGS_OMODE16,argtype.ARG_MODRM,argtype.ARG_IMM8,argtype.ARG_NONE,7012),
  new asminstdata("sal",0x6,Proc.PROC_FROM80386,flags.FLAGS_MODRM|flags.FLAGS_OMODE32,argtype.ARG_MODRM,argtype.ARG_IMM8,argtype.ARG_NONE,7013),
  new asminstdata("sar",0x7,Proc.PROC_FROM80286,flags.FLAGS_MODRM|flags.FLAGS_OMODE16,argtype.ARG_MODRM,argtype.ARG_IMM8,argtype.ARG_NONE,7014),
  new asminstdata("sar",0x7,Proc.PROC_FROM80386,flags.FLAGS_MODRM|flags.FLAGS_OMODE32,argtype.ARG_MODRM,argtype.ARG_IMM8,argtype.ARG_NONE,7015),
  new asminstdata(null,0x00,0,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,0)  //endCode marker - processor=0 & opcode=0
};

        // subtable 0xd0
        // - num is encoding of modrm bits 5,4,3 only
        public static asminstdata[] asm86subd0 = new asminstdata[]
{ new asminstdata("rol",0x0,Proc.PROC_FROM8086,flags.FLAGS_MODRM|flags.FLAGS_8BIT,argtype.ARG_MODRM,argtype.ARG_IMM_1,argtype.ARG_NONE,8000),
  new asminstdata("ror",0x1,Proc.PROC_FROM8086,flags.FLAGS_MODRM|flags.FLAGS_8BIT,argtype.ARG_MODRM,argtype.ARG_IMM_1,argtype.ARG_NONE,8001),
  new asminstdata("rcl",0x2,Proc.PROC_FROM8086,flags.FLAGS_MODRM|flags.FLAGS_8BIT,argtype.ARG_MODRM,argtype.ARG_IMM_1,argtype.ARG_NONE,8002),
  new asminstdata("rcr",0x3,Proc.PROC_FROM8086,flags.FLAGS_MODRM|flags.FLAGS_8BIT,argtype.ARG_MODRM,argtype.ARG_IMM_1,argtype.ARG_NONE,8003),
  new asminstdata("shl",0x4,Proc.PROC_FROM8086,flags.FLAGS_MODRM|flags.FLAGS_8BIT,argtype.ARG_MODRM,argtype.ARG_IMM_1,argtype.ARG_NONE,8004),
  new asminstdata("shr",0x5,Proc.PROC_FROM8086,flags.FLAGS_MODRM|flags.FLAGS_8BIT,argtype.ARG_MODRM,argtype.ARG_IMM_1,argtype.ARG_NONE,8005),
  new asminstdata("sal",0x6,Proc.PROC_FROM8086,flags.FLAGS_MODRM|flags.FLAGS_8BIT,argtype.ARG_MODRM,argtype.ARG_IMM_1,argtype.ARG_NONE,8006),
  new asminstdata("sar",0x7,Proc.PROC_FROM8086,flags.FLAGS_MODRM|flags.FLAGS_8BIT,argtype.ARG_MODRM,argtype.ARG_IMM_1,argtype.ARG_NONE,8007),
  new asminstdata(null,0x00,0,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,0)  //endCode marker - processor=0 & opcode=0
};

        // subtable 0xd1
        // - num is encoding of modrm bits 5,4,3 only
        static asminstdata[] asm86subd1 = new asminstdata[]
{ new asminstdata("rol",0x0,Proc.PROC_FROM8086,flags.FLAGS_MODRM|flags.FLAGS_OMODE16,argtype.ARG_MODRM,argtype.ARG_IMM_1,argtype.ARG_NONE,9000),
  new asminstdata("rol",0x0,Proc.PROC_FROM80386,flags.FLAGS_MODRM|flags.FLAGS_OMODE32,argtype.ARG_MODRM,argtype.ARG_IMM_1,argtype.ARG_NONE,9001),
  new asminstdata("ror",0x1,Proc.PROC_FROM8086,flags.FLAGS_MODRM|flags.FLAGS_OMODE16,argtype.ARG_MODRM,argtype.ARG_IMM_1,argtype.ARG_NONE,9002),
  new asminstdata("ror",0x1,Proc.PROC_FROM80386,flags.FLAGS_MODRM|flags.FLAGS_OMODE32,argtype.ARG_MODRM,argtype.ARG_IMM_1,argtype.ARG_NONE,9003),
  new asminstdata("rcl",0x2,Proc.PROC_FROM8086,flags.FLAGS_MODRM|flags.FLAGS_OMODE16,argtype.ARG_MODRM,argtype.ARG_IMM_1,argtype.ARG_NONE,9004),
  new asminstdata("rcl",0x2,Proc.PROC_FROM80386,flags.FLAGS_MODRM|flags.FLAGS_OMODE32,argtype.ARG_MODRM,argtype.ARG_IMM_1,argtype.ARG_NONE,9005),
  new asminstdata("rcr",0x3,Proc.PROC_FROM8086,flags.FLAGS_MODRM|flags.FLAGS_OMODE16,argtype.ARG_MODRM,argtype.ARG_IMM_1,argtype.ARG_NONE,9006),
  new asminstdata("rcr",0x3,Proc.PROC_FROM80386,flags.FLAGS_MODRM|flags.FLAGS_OMODE32,argtype.ARG_MODRM,argtype.ARG_IMM_1,argtype.ARG_NONE,9007),
  new asminstdata("shl",0x4,Proc.PROC_FROM8086,flags.FLAGS_MODRM|flags.FLAGS_OMODE16,argtype.ARG_MODRM,argtype.ARG_IMM_1,argtype.ARG_NONE,9008),
  new asminstdata("shl",0x4,Proc.PROC_FROM80386,flags.FLAGS_MODRM|flags.FLAGS_OMODE32,argtype.ARG_MODRM,argtype.ARG_IMM_1,argtype.ARG_NONE,9009),
  new asminstdata("shr",0x5,Proc.PROC_FROM8086,flags.FLAGS_MODRM|flags.FLAGS_OMODE16,argtype.ARG_MODRM,argtype.ARG_IMM_1,argtype.ARG_NONE,9010),
  new asminstdata("shr",0x5,Proc.PROC_FROM80386,flags.FLAGS_MODRM|flags.FLAGS_OMODE32,argtype.ARG_MODRM,argtype.ARG_IMM_1,argtype.ARG_NONE,9011),
  new asminstdata("sal",0x6,Proc.PROC_FROM8086,flags.FLAGS_MODRM|flags.FLAGS_OMODE16,argtype.ARG_MODRM,argtype.ARG_IMM_1,argtype.ARG_NONE,9012),
  new asminstdata("sal",0x6,Proc.PROC_FROM80386,flags.FLAGS_MODRM|flags.FLAGS_OMODE32,argtype.ARG_MODRM,argtype.ARG_IMM_1,argtype.ARG_NONE,9013),
  new asminstdata("sar",0x7,Proc.PROC_FROM8086,flags.FLAGS_MODRM|flags.FLAGS_OMODE16,argtype.ARG_MODRM,argtype.ARG_IMM_1,argtype.ARG_NONE,9014),
  new asminstdata("sar",0x7,Proc.PROC_FROM80386,flags.FLAGS_MODRM|flags.FLAGS_OMODE32,argtype.ARG_MODRM,argtype.ARG_IMM_1,argtype.ARG_NONE,9015),
  new asminstdata(null,0x00,0,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,0)  //endCode marker - processor=0 & opcode=0
};

        // subtable 0xd2
        // - num is encoding of modrm bits 5,4,3 only
        public static asminstdata[] asm86subd2 = new asminstdata[]
{ new asminstdata("rol",0x0,Proc.PROC_FROM8086,flags.FLAGS_MODRM|flags.FLAGS_8BIT,argtype.ARG_MODRM,argtype.ARG_REG_CL,argtype.ARG_NONE,10000),
  new asminstdata("ror",0x1,Proc.PROC_FROM8086,flags.FLAGS_MODRM|flags.FLAGS_8BIT,argtype.ARG_MODRM,argtype.ARG_REG_CL,argtype.ARG_NONE,10001),
  new asminstdata("rcl",0x2,Proc.PROC_FROM8086,flags.FLAGS_MODRM|flags.FLAGS_8BIT,argtype.ARG_MODRM,argtype.ARG_REG_CL,argtype.ARG_NONE,10002),
  new asminstdata("rcr",0x3,Proc.PROC_FROM8086,flags.FLAGS_MODRM|flags.FLAGS_8BIT,argtype.ARG_MODRM,argtype.ARG_REG_CL,argtype.ARG_NONE,10003),
  new asminstdata("shl",0x4,Proc.PROC_FROM8086,flags.FLAGS_MODRM|flags.FLAGS_8BIT,argtype.ARG_MODRM,argtype.ARG_REG_CL,argtype.ARG_NONE,10004),
  new asminstdata("shr",0x5,Proc.PROC_FROM8086,flags.FLAGS_MODRM|flags.FLAGS_8BIT,argtype.ARG_MODRM,argtype.ARG_REG_CL,argtype.ARG_NONE,10005),
  new asminstdata("sal",0x6,Proc.PROC_FROM8086,flags.FLAGS_MODRM|flags.FLAGS_8BIT,argtype.ARG_MODRM,argtype.ARG_REG_CL,argtype.ARG_NONE,10006),
  new asminstdata("sar",0x7,Proc.PROC_FROM8086,flags.FLAGS_MODRM|flags.FLAGS_8BIT,argtype.ARG_MODRM,argtype.ARG_REG_CL,argtype.ARG_NONE,10007),
  new asminstdata(null,0x00,0,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,0)  //endCode marker - processor=0 & opcode=0
};

        // subtable 0xd3
        // - num is encoding of modrm bits 5,4,3 only
        public static asminstdata[] asm86subd3 = new asminstdata[]
{ new asminstdata("rol",0x0,Proc.PROC_FROM8086,flags.FLAGS_MODRM|flags.FLAGS_OMODE16,argtype.ARG_MODRM,argtype.ARG_REG_CL,argtype.ARG_NONE,11000),
  new asminstdata("rol",0x0,Proc.PROC_FROM80386,flags.FLAGS_MODRM|flags.FLAGS_OMODE32,argtype.ARG_MODRM,argtype.ARG_REG_CL,argtype.ARG_NONE,11001),
  new asminstdata("ror",0x1,Proc.PROC_FROM8086,flags.FLAGS_MODRM|flags.FLAGS_OMODE16,argtype.ARG_MODRM,argtype.ARG_REG_CL,argtype.ARG_NONE,11002),
  new asminstdata("ror",0x1,Proc.PROC_FROM80386,flags.FLAGS_MODRM|flags.FLAGS_OMODE32,argtype.ARG_MODRM,argtype.ARG_REG_CL,argtype.ARG_NONE,11003),
  new asminstdata("rcl",0x2,Proc.PROC_FROM8086,flags.FLAGS_MODRM|flags.FLAGS_OMODE16,argtype.ARG_MODRM,argtype.ARG_REG_CL,argtype.ARG_NONE,11004),
  new asminstdata("rcl",0x2,Proc.PROC_FROM80386,flags.FLAGS_MODRM|flags.FLAGS_OMODE32,argtype.ARG_MODRM,argtype.ARG_REG_CL,argtype.ARG_NONE,11005),
  new asminstdata("rcr",0x3,Proc.PROC_FROM8086,flags.FLAGS_MODRM|flags.FLAGS_OMODE16,argtype.ARG_MODRM,argtype.ARG_REG_CL,argtype.ARG_NONE,11006),
  new asminstdata("rcr",0x3,Proc.PROC_FROM80386,flags.FLAGS_MODRM|flags.FLAGS_OMODE32,argtype.ARG_MODRM,argtype.ARG_REG_CL,argtype.ARG_NONE,11007),
  new asminstdata("shl",0x4,Proc.PROC_FROM8086,flags.FLAGS_MODRM|flags.FLAGS_OMODE16,argtype.ARG_MODRM,argtype.ARG_REG_CL,argtype.ARG_NONE,11008),
  new asminstdata("shl",0x4,Proc.PROC_FROM80386,flags.FLAGS_MODRM|flags.FLAGS_OMODE32,argtype.ARG_MODRM,argtype.ARG_REG_CL,argtype.ARG_NONE,11009),
  new asminstdata("shr",0x5,Proc.PROC_FROM8086,flags.FLAGS_MODRM|flags.FLAGS_OMODE16,argtype.ARG_MODRM,argtype.ARG_REG_CL,argtype.ARG_NONE,11010),
  new asminstdata("shr",0x5,Proc.PROC_FROM80386,flags.FLAGS_MODRM|flags.FLAGS_OMODE32,argtype.ARG_MODRM,argtype.ARG_REG_CL,argtype.ARG_NONE,11011),
  new asminstdata("sal",0x6,Proc.PROC_FROM8086,flags.FLAGS_MODRM|flags.FLAGS_OMODE16,argtype.ARG_MODRM,argtype.ARG_REG_CL,argtype.ARG_NONE,11012),
  new asminstdata("sal",0x6,Proc.PROC_FROM80386,flags.FLAGS_MODRM|flags.FLAGS_OMODE32,argtype.ARG_MODRM,argtype.ARG_REG_CL,argtype.ARG_NONE,11013),
  new asminstdata("sar",0x7,Proc.PROC_FROM8086,flags.FLAGS_MODRM|flags.FLAGS_OMODE16,argtype.ARG_MODRM,argtype.ARG_REG_CL,argtype.ARG_NONE,11014),
  new asminstdata("sar",0x7,Proc.PROC_FROM80386,flags.FLAGS_MODRM|flags.FLAGS_OMODE32,argtype.ARG_MODRM,argtype.ARG_REG_CL,argtype.ARG_NONE,11015),
  new asminstdata(null,0x00,0,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,0)  //endCode marker - processor=0 & opcode=0
};

        // subtable 0xf6
        // - num is encoding of modrm bits 5,4,3 only
        public static asminstdata[] asm86subf6 = new asminstdata[]
{ new asminstdata("test",0x0,Proc.PROC_FROM8086,flags.FLAGS_MODRM|flags.FLAGS_8BIT,argtype.ARG_MODRM,argtype.ARG_IMM8,argtype.ARG_NONE,12000),
  new asminstdata("test",0x1,Proc.PROC_FROM8086,flags.FLAGS_MODRM|flags.FLAGS_8BIT,argtype.ARG_MODRM,argtype.ARG_IMM8,argtype.ARG_NONE,12001),
  new asminstdata("not",0x2,Proc.PROC_FROM8086,flags.FLAGS_MODRM|flags.FLAGS_8BIT,argtype.ARG_MODRM,argtype.ARG_NONE,argtype.ARG_NONE,12002),
  new asminstdata("neg",0x3,Proc.PROC_FROM8086,flags.FLAGS_MODRM|flags.FLAGS_8BIT,argtype.ARG_MODRM,argtype.ARG_NONE,argtype.ARG_NONE,12003),
  new asminstdata("mul",0x4,Proc.PROC_FROM8086,flags.FLAGS_MODRM|flags.FLAGS_8BIT,argtype.ARG_MODRM,argtype.ARG_NONE,argtype.ARG_NONE,12004),
  new asminstdata("imul",0x5,Proc.PROC_FROM8086,flags.FLAGS_MODRM|flags.FLAGS_8BIT,argtype.ARG_MODRM,argtype.ARG_NONE,argtype.ARG_NONE,12005),
  new asminstdata("div",0x6,Proc.PROC_FROM8086,flags.FLAGS_MODRM|flags.FLAGS_8BIT,argtype.ARG_MODRM,argtype.ARG_NONE,argtype.ARG_NONE,12006),
  new asminstdata("idiv",0x7,Proc.PROC_FROM8086,flags.FLAGS_MODRM|flags.FLAGS_8BIT,argtype.ARG_MODRM,argtype.ARG_NONE,argtype.ARG_NONE,12007),
  new asminstdata(null,0x00,0,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,0)  //endCode marker - processor=0 & opcode=0
};

        // subtable 0xf7
        // - num is encoding of modrm bits 5,4,3 only
        public static asminstdata[] asm86subf7 = new asminstdata[]
{ new asminstdata("test",0x0,Proc.PROC_FROM8086,flags.FLAGS_MODRM|flags.FLAGS_OMODE16,argtype.ARG_MODRM,argtype.ARG_IMM,argtype.ARG_NONE,13000),
  new asminstdata("test",0x0,Proc.PROC_FROM80386,flags.FLAGS_MODRM|flags.FLAGS_OMODE32,argtype.ARG_MODRM,argtype.ARG_IMM,argtype.ARG_NONE,13001),
  new asminstdata("test",0x1,Proc.PROC_FROM8086,flags.FLAGS_MODRM|flags.FLAGS_OMODE16,argtype.ARG_MODRM,argtype.ARG_IMM,argtype.ARG_NONE,13002),
  new asminstdata("test",0x1,Proc.PROC_FROM80386,flags.FLAGS_MODRM|flags.FLAGS_OMODE32,argtype.ARG_MODRM,argtype.ARG_IMM,argtype.ARG_NONE,13003),
  new asminstdata("not",0x2,Proc.PROC_FROM8086,flags.FLAGS_MODRM|flags.FLAGS_OMODE16,argtype.ARG_MODRM,argtype.ARG_NONE,argtype.ARG_NONE,13004),
  new asminstdata("not",0x2,Proc.PROC_FROM80386,flags.FLAGS_MODRM|flags.FLAGS_OMODE32,argtype.ARG_MODRM,argtype.ARG_NONE,argtype.ARG_NONE,13005),
  new asminstdata("neg",0x3,Proc.PROC_FROM8086,flags.FLAGS_MODRM|flags.FLAGS_OMODE16,argtype.ARG_MODRM,argtype.ARG_NONE,argtype.ARG_NONE,13006),
  new asminstdata("neg",0x3,Proc.PROC_FROM80386,flags.FLAGS_MODRM|flags.FLAGS_OMODE32,argtype.ARG_MODRM,argtype.ARG_NONE,argtype.ARG_NONE,13007),
  new asminstdata("mul",0x4,Proc.PROC_FROM8086,flags.FLAGS_MODRM|flags.FLAGS_OMODE16,argtype.ARG_MODRM,argtype.ARG_NONE,argtype.ARG_NONE,13008),
  new asminstdata("mul",0x4,Proc.PROC_FROM80386,flags.FLAGS_MODRM|flags.FLAGS_OMODE32,argtype.ARG_MODRM,argtype.ARG_NONE,argtype.ARG_NONE,13009),
  new asminstdata("imul",0x5,Proc.PROC_FROM8086,flags.FLAGS_MODRM|flags.FLAGS_OMODE16,argtype.ARG_MODRM,argtype.ARG_NONE,argtype.ARG_NONE,13010),
  new asminstdata("imul",0x5,Proc.PROC_FROM80386,flags.FLAGS_MODRM|flags.FLAGS_OMODE32,argtype.ARG_MODRM,argtype.ARG_NONE,argtype.ARG_NONE,13011),
  new asminstdata("div",0x6,Proc.PROC_FROM8086,flags.FLAGS_MODRM|flags.FLAGS_OMODE16,argtype.ARG_MODRM,argtype.ARG_NONE,argtype.ARG_NONE,13012),
  new asminstdata("div",0x6,Proc.PROC_FROM80386,flags.FLAGS_MODRM|flags.FLAGS_OMODE32,argtype.ARG_MODRM,argtype.ARG_NONE,argtype.ARG_NONE,13013),
  new asminstdata("idiv",0x7,Proc.PROC_FROM8086,flags.FLAGS_MODRM|flags.FLAGS_OMODE16,argtype.ARG_MODRM,argtype.ARG_NONE,argtype.ARG_NONE,13014),
  new asminstdata("idiv",0x7,Proc.PROC_FROM80386,flags.FLAGS_MODRM|flags.FLAGS_OMODE32,argtype.ARG_MODRM,argtype.ARG_NONE,argtype.ARG_NONE,13015),
  new asminstdata(null,0x00,0,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,0)  //endCode marker - processor=0 & opcode=0
};

        // subtable 0xfe
        // - num is encoding of modrm bits 5,4,3 only
        public static asminstdata[] asm86subfe = new asminstdata[]
{ new asminstdata("inc",0x0,Proc.PROC_FROM8086,flags.FLAGS_MODRM|flags.FLAGS_8BIT,argtype.ARG_MODRM,argtype.ARG_NONE,argtype.ARG_NONE,14000),
  new asminstdata("dec",0x1,Proc.PROC_FROM8086,flags.FLAGS_MODRM|flags.FLAGS_8BIT,argtype.ARG_MODRM,argtype.ARG_NONE,argtype.ARG_NONE,14001),
  new asminstdata(null,0x00,0,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,0)  //endCode marker - processor=0 & opcode=0
};

        // subtable 0xff
        // - num is encoding of modrm bits 5,4,3 only
        static asminstdata[] asm86subff = new asminstdata[]
{ new asminstdata("inc",0x0,Proc.PROC_FROM8086,flags.FLAGS_MODRM|flags.FLAGS_OMODE16,argtype.ARG_MODRM,argtype.ARG_NONE,argtype.ARG_NONE,15000),
  new asminstdata("inc",0x0,Proc.PROC_FROM80386,flags.FLAGS_MODRM|flags.FLAGS_OMODE32,argtype.ARG_MODRM,argtype.ARG_NONE,argtype.ARG_NONE,15001),
  new asminstdata("dec",0x1,Proc.PROC_FROM8086,flags.FLAGS_MODRM|flags.FLAGS_OMODE16,argtype.ARG_MODRM,argtype.ARG_NONE,argtype.ARG_NONE,15002),
  new asminstdata("dec",0x1,Proc.PROC_FROM80386,flags.FLAGS_MODRM|flags.FLAGS_OMODE32,argtype.ARG_MODRM,argtype.ARG_NONE,argtype.ARG_NONE,15003),
  new asminstdata("call",0x2,Proc.PROC_FROM8086,flags.FLAGS_MODRM|flags.FLAGS_OMODE16|flags.FLAGS_CALL,argtype.ARG_MODRM,argtype.ARG_NONE,argtype.ARG_NONE,15004),
  new asminstdata("call",0x2,Proc.PROC_FROM80386,flags.FLAGS_MODRM|flags.FLAGS_OMODE32|flags.FLAGS_CALL,argtype.ARG_MODRM,argtype.ARG_NONE,argtype.ARG_NONE,15005),
  new asminstdata("call",0x3,Proc.PROC_FROM8086,flags.FLAGS_MODRM|flags.FLAGS_OMODE16|flags.FLAGS_CALL,argtype.ARG_MODRM_FPTR,argtype.ARG_NONE,argtype.ARG_NONE,15006),
  new asminstdata("call",0x3,Proc.PROC_FROM80386,flags.FLAGS_MODRM|flags.FLAGS_OMODE32|flags.FLAGS_CALL,argtype.ARG_MODRM_FPTR,argtype.ARG_NONE,argtype.ARG_NONE,15007),
  new asminstdata("jmp",0x4,Proc.PROC_FROM8086,flags.FLAGS_MODRM|flags.FLAGS_OMODE16|flags.FLAGS_JMP,argtype.ARG_MODRM,argtype.ARG_NONE,argtype.ARG_NONE,15008),
  new asminstdata("jmp",0x4,Proc.PROC_FROM80386,flags.FLAGS_MODRM|flags.FLAGS_OMODE32|flags.FLAGS_JMP,argtype.ARG_MODRM,argtype.ARG_NONE,argtype.ARG_NONE,15009),
  new asminstdata("jmp",0x5,Proc.PROC_FROM8086,flags.FLAGS_MODRM|flags.FLAGS_OMODE16|flags.FLAGS_JMP,argtype.ARG_MODRM_FPTR,argtype.ARG_NONE,argtype.ARG_NONE,15010),
  new asminstdata("jmp",0x5,Proc.PROC_FROM80386,flags.FLAGS_MODRM|flags.FLAGS_OMODE32|flags.FLAGS_JMP,argtype.ARG_MODRM_FPTR,argtype.ARG_NONE,argtype.ARG_NONE,15011),
  new asminstdata("push",0x6,Proc.PROC_FROM8086,flags.FLAGS_MODRM|flags.FLAGS_OMODE16,argtype.ARG_MODRM,argtype.ARG_NONE,argtype.ARG_NONE,15012),
  new asminstdata("push",0x6,Proc.PROC_FROM80386,flags.FLAGS_MODRM|flags.FLAGS_OMODE32,argtype.ARG_MODRM,argtype.ARG_NONE,argtype.ARG_NONE,15013),
  new asminstdata(null,0x00,0,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,0)  //endCode marker - processor=0 & opcode=0
};

        // subtable 0x0f/0x00
        // - num is encoding of modrm bits 5,4,3 only
        public static asminstdata[] asm86sub0f00 = new asminstdata[]
{ new asminstdata("sldt",0x0,Proc.PROC_FROM80286,flags.FLAGS_MODRM|flags.FLAGS_16BIT,argtype.ARG_MODRM,argtype.ARG_NONE,argtype.ARG_NONE,16000),
  new asminstdata("str",0x1,Proc.PROC_FROM80286,flags.FLAGS_MODRM|flags.FLAGS_16BIT,argtype.ARG_MODRM,argtype.ARG_NONE,argtype.ARG_NONE,16001),
  new asminstdata("lldt",0x2,Proc.PROC_FROM80286,flags.FLAGS_MODRM|flags.FLAGS_16BIT,argtype.ARG_MODRM,argtype.ARG_NONE,argtype.ARG_NONE,16002),
  new asminstdata("ltr",0x3,Proc.PROC_FROM80286,flags.FLAGS_MODRM|flags.FLAGS_16BIT,argtype.ARG_MODRM,argtype.ARG_NONE,argtype.ARG_NONE,16003),
  new asminstdata("verr",0x4,Proc.PROC_FROM80286,flags.FLAGS_MODRM|flags.FLAGS_16BIT,argtype.ARG_MODRM,argtype.ARG_NONE,argtype.ARG_NONE,16004),
  new asminstdata("verw",0x5,Proc.PROC_FROM80286,flags.FLAGS_MODRM|flags.FLAGS_16BIT,argtype.ARG_MODRM,argtype.ARG_NONE,argtype.ARG_NONE,16005),
  new asminstdata(null,0x00,0,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,0)  //endCode marker - processor=0 & opcode=0
};

        // subtable 0x0f/0x01
        // - num is encoding of modrm bits 5,4,3 only
        public static asminstdata[] asm86sub0f01 = new asminstdata[]
{ new asminstdata("sgdt",0x0,Proc.PROC_FROM80286,flags.FLAGS_MODRM,argtype.ARG_MODRM_S,argtype.ARG_NONE,argtype.ARG_NONE,17000),
  new asminstdata("sidt",0x1,Proc.PROC_FROM80286,flags.FLAGS_MODRM,argtype.ARG_MODRM_S,argtype.ARG_NONE,argtype.ARG_NONE,17001),
  new asminstdata("lgdt",0x2,Proc.PROC_FROM80286,flags.FLAGS_MODRM,argtype.ARG_MODRM_S,argtype.ARG_NONE,argtype.ARG_NONE,17002),
  new asminstdata("lidt",0x3,Proc.PROC_FROM80286,flags.FLAGS_MODRM,argtype.ARG_MODRM_S,argtype.ARG_NONE,argtype.ARG_NONE,17003),
  new asminstdata("smsw",0x4,Proc.PROC_FROM80286,flags.FLAGS_MODRM|flags.FLAGS_16BIT,argtype.ARG_MODRM,argtype.ARG_NONE,argtype.ARG_NONE,17004),
  new asminstdata("lmsw",0x6,Proc.PROC_FROM80286,flags.FLAGS_MODRM|flags.FLAGS_16BIT,argtype.ARG_MODRM,argtype.ARG_NONE,argtype.ARG_NONE,17005),
  new asminstdata("invlpg",0x7,Proc.PROC_FROM80486,flags.FLAGS_MODRM|flags.FLAGS_16BIT,argtype.ARG_MODRM,argtype.ARG_NONE,argtype.ARG_NONE,17006),
  new asminstdata(null,0x00,0,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,0)  //endCode marker - processor=0 & opcode=0
};

        // subtable 0x0f/0x18
        // - num is encoding of modrm bits 5,4,3 only
        public static asminstdata[] asm86sub0f18 = new asminstdata[]
{ new asminstdata("prefetchnta",0x0,Proc.PROC_FROMPENTIUM2,flags.FLAGS_MODRM,argtype.ARG_MODRM,argtype.ARG_NONE,argtype.ARG_NONE,18000),
  new asminstdata("prefetcht0",0x1,Proc.PROC_FROMPENTIUM2,flags.FLAGS_MODRM,argtype.ARG_MODRM,argtype.ARG_NONE,argtype.ARG_NONE,18001),
  new asminstdata("prefetcht1",0x2,Proc.PROC_FROMPENTIUM2,flags.FLAGS_MODRM,argtype.ARG_MODRM,argtype.ARG_NONE,argtype.ARG_NONE,18002),
  new asminstdata("prefetcht2",0x3,Proc.PROC_FROMPENTIUM2,flags.FLAGS_MODRM,argtype.ARG_MODRM,argtype.ARG_NONE,argtype.ARG_NONE,18003),
  new asminstdata(null,0x00,0,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,0)  //endCode marker - processor=0 & opcode=0
};

        // subtable 0x0f/0x71
        // - num is encoding of modrm bits 5,4,3 only
        public static asminstdata[] asm86sub0f71 = new asminstdata[]
{ new asminstdata("psrlw",0x2,Proc.PROC_FROMPENTMMX,flags.FLAGS_MODRM|flags.FLAGS_32BIT,argtype.ARG_MMXMODRM,argtype.ARG_IMM8,argtype.ARG_NONE,19000),
  new asminstdata("psraw",0x4,Proc.PROC_FROMPENTMMX,flags.FLAGS_MODRM|flags.FLAGS_32BIT,argtype.ARG_MMXMODRM,argtype.ARG_IMM8,argtype.ARG_NONE,19001),
  new asminstdata("psllw",0x6,Proc.PROC_FROMPENTMMX,flags.FLAGS_MODRM|flags.FLAGS_32BIT,argtype.ARG_MMXMODRM,argtype.ARG_IMM8,argtype.ARG_NONE,19002),
  new asminstdata(null,0x00,0,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,0)  //endCode marker - processor=0 & opcode=0
};

        // subtable 0x0f/0x72
        // - num is encoding of modrm bits 5,4,3 only
        public static asminstdata[] asm86sub0f72 = new asminstdata[]
{ new asminstdata("psrld",0x2,Proc.PROC_FROMPENTMMX,flags.FLAGS_MODRM|flags.FLAGS_32BIT,argtype.ARG_MMXMODRM,argtype.ARG_IMM8,argtype.ARG_NONE,20000),
  new asminstdata("psrad",0x4,Proc.PROC_FROMPENTMMX,flags.FLAGS_MODRM|flags.FLAGS_32BIT,argtype.ARG_MMXMODRM,argtype.ARG_IMM8,argtype.ARG_NONE,20001),
  new asminstdata("pslld",0x6,Proc.PROC_FROMPENTMMX,flags.FLAGS_MODRM|flags.FLAGS_32BIT,argtype.ARG_MMXMODRM,argtype.ARG_IMM8,argtype.ARG_NONE,20002),
  new asminstdata(null,0x00,0,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,0)  //endCode marker - processor=0 & opcode=0
};

        // subtable 0x0f/0x73
        // - num is encoding of modrm bits 5,4,3 only
        static asminstdata[] asm86sub0f73 = new asminstdata[]
{ new asminstdata("psrlq",0x2,Proc.PROC_FROMPENTMMX,flags.FLAGS_MODRM|flags.FLAGS_32BIT,argtype.ARG_MMXMODRM,argtype.ARG_IMM8,argtype.ARG_NONE,21000),
  new asminstdata("psllq",0x6,Proc.PROC_FROMPENTMMX,flags.FLAGS_MODRM|flags.FLAGS_32BIT,argtype.ARG_MMXMODRM,argtype.ARG_IMM8,argtype.ARG_NONE,21001),
  new asminstdata(null,0x00,0,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,0)  //endCode marker - processor=0 & opcode=0
};

        // subtable 0x0f/0xae
        // - num is encoding of modrm bits 5,4,3 only
        public static asminstdata[] asm86sub0fae = new asminstdata[]
{ new asminstdata("fxsave",0x0,Proc.PROC_FROMPENTMMX,flags.FLAGS_MODRM|flags.FLAGS_32BIT,argtype.ARG_MODRMM512,argtype.ARG_NONE,argtype.ARG_NONE,22000),
  new asminstdata("fxrstor",0x1,Proc.PROC_FROMPENTMMX,flags.FLAGS_MODRM|flags.FLAGS_32BIT,argtype.ARG_MODRMM512,argtype.ARG_NONE,argtype.ARG_NONE,22001),
  new asminstdata("ldmxcsr",0x2,Proc.PROC_FROMPENTIUM2,flags.FLAGS_MODRM|flags.FLAGS_32BIT,argtype.ARG_MODRM,argtype.ARG_NONE,argtype.ARG_NONE,22002),
  new asminstdata("stmxcsr",0x3,Proc.PROC_FROMPENTIUM2,flags.FLAGS_MODRM|flags.FLAGS_32BIT,argtype.ARG_MODRM,argtype.ARG_NONE,argtype.ARG_NONE,22003),
  new asminstdata("sfence",0x7,Proc.PROC_FROMPENTIUM2,flags.FLAGS_MODRM|flags.FLAGS_32BIT,argtype.ARG_NONEBYTE,argtype.ARG_NONE,argtype.ARG_NONE,22004),
  new asminstdata(null,0x00,0,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,0)  //endCode marker - processor=0 & opcode=0
};

        // subtable 0x0f/0xba
        // - num is encoding of modrm bits 5,4,3 only
        static asminstdata[] asm86sub0fba = new asminstdata[]
{ new asminstdata("bt",0x4,Proc.PROC_FROM80386,flags.FLAGS_MODRM,argtype.ARG_MODRM,argtype.ARG_IMM8,argtype.ARG_NONE,23000),
  new asminstdata("bts",0x5,Proc.PROC_FROM80386,flags.FLAGS_MODRM,argtype.ARG_MODRM,argtype.ARG_IMM8,argtype.ARG_NONE,23001),
  new asminstdata("btr",0x6,Proc.PROC_FROM80386,flags.FLAGS_MODRM,argtype.ARG_MODRM,argtype.ARG_IMM8,argtype.ARG_NONE,23002),
  new asminstdata("btc",0x7,Proc.PROC_FROM80386,flags.FLAGS_MODRM,argtype.ARG_MODRM,argtype.ARG_IMM8,argtype.ARG_NONE,23003),
  new asminstdata(null,0x00,0,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,0)  //endCode marker - processor=0 & opcode=0
};

        // subtable 0x0f/0xc2
        // -num is the follow up byte
        public static asminstdata[] asm86sub0fc2 = new asminstdata[]
{ new asminstdata("cmpeqps",0x0,Proc.PROC_FROMPENTIUM2,flags.FLAGS_MODRM|flags.FLAGS_32BIT,argtype.ARG_XREG,argtype.ARG_XMMMODRM,argtype.ARG_NONE,24000),
  new asminstdata("cmpltps",0x1,Proc.PROC_FROMPENTIUM2,flags.FLAGS_MODRM|flags.FLAGS_32BIT,argtype.ARG_XREG,argtype.ARG_XMMMODRM,argtype.ARG_NONE,24001),
  new asminstdata("cmpleps",0x2,Proc.PROC_FROMPENTIUM2,flags.FLAGS_MODRM|flags.FLAGS_32BIT,argtype.ARG_XREG,argtype.ARG_XMMMODRM,argtype.ARG_NONE,24002),
  new asminstdata("cmpunordps",0x3,Proc.PROC_FROMPENTIUM2,flags.FLAGS_MODRM|flags.FLAGS_32BIT,argtype.ARG_XREG,argtype.ARG_XMMMODRM,argtype.ARG_NONE,24003),
  new asminstdata("cmpneqps",0x4,Proc.PROC_FROMPENTIUM2,flags.FLAGS_MODRM|flags.FLAGS_32BIT,argtype.ARG_XREG,argtype.ARG_XMMMODRM,argtype.ARG_NONE,24004),
  new asminstdata("cmpnltps",0x5,Proc.PROC_FROMPENTIUM2,flags.FLAGS_MODRM|flags.FLAGS_32BIT,argtype.ARG_XREG,argtype.ARG_XMMMODRM,argtype.ARG_NONE,24005),
  new asminstdata("cmpnleps",0x6,Proc.PROC_FROMPENTIUM2,flags.FLAGS_MODRM|flags.FLAGS_32BIT,argtype.ARG_XREG,argtype.ARG_XMMMODRM,argtype.ARG_NONE,24006),
  new asminstdata("cmpordps",0x7,Proc.PROC_FROMPENTIUM2,flags.FLAGS_MODRM|flags.FLAGS_32BIT,argtype.ARG_XREG,argtype.ARG_XMMMODRM,argtype.ARG_NONE,24007),
  new asminstdata(null,0x00,0,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,0)  //endCode marker - processor=0 & opcode=0
};

        // subtable 0x0f/0xc7
        // - num is encoding of modrm bits 5,4,3 only
        public static asminstdata[] asm86sub0fc7 = new asminstdata[]
{ new asminstdata("cmpxch8b",0x1,Proc.PROC_FROMPENTIUM,flags.FLAGS_MODRM,argtype.ARG_MODRMQ,argtype.ARG_NONE,argtype.ARG_NONE,25000),
  new asminstdata(null,0x00,0,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,0)  //endCode marker - processor=0 & opcode=0
};

        // FPU Instructions

        // subtable 0xd8/ modrm = 0x00-0xbf
        // - num is encoding of modrm bits 5,4,3 only
        public static asminstdata[] asm86subd8a = new asminstdata[]
{ new asminstdata("fadd",0x0,Proc.PROC_FROM8086,flags.FLAGS_MODRM,argtype.ARG_MODRM_SREAL,argtype.ARG_NONE,argtype.ARG_NONE,26000),
  new asminstdata("fmul",0x1,Proc.PROC_FROM8086,flags.FLAGS_MODRM,argtype.ARG_MODRM_SREAL,argtype.ARG_NONE,argtype.ARG_NONE,26001),
  new asminstdata("fcom",0x2,Proc.PROC_FROM8086,flags.FLAGS_MODRM,argtype.ARG_MODRM_SREAL,argtype.ARG_NONE,argtype.ARG_NONE,26002),
  new asminstdata("fcomp",0x3,Proc.PROC_FROM8086,flags.FLAGS_MODRM,argtype.ARG_MODRM_SREAL,argtype.ARG_NONE,argtype.ARG_NONE,26003),
  new asminstdata("fsub",0x4,Proc.PROC_FROM8086,flags.FLAGS_MODRM,argtype.ARG_MODRM_SREAL,argtype.ARG_NONE,argtype.ARG_NONE,26004),
  new asminstdata("fsubr",0x5,Proc.PROC_FROM8086,flags.FLAGS_MODRM,argtype.ARG_MODRM_SREAL,argtype.ARG_NONE,argtype.ARG_NONE,26005),
  new asminstdata("fdiv",0x6,Proc.PROC_FROM8086,flags.FLAGS_MODRM,argtype.ARG_MODRM_SREAL,argtype.ARG_NONE,argtype.ARG_NONE,26006),
  new asminstdata("fdivr",0x7,Proc.PROC_FROM8086,flags.FLAGS_MODRM,argtype.ARG_MODRM_SREAL,argtype.ARG_NONE,argtype.ARG_NONE,26007),
  new asminstdata(null,0x00,0,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,0)  //endCode marker - processor=0 & opcode=0
};

        // subtable 0xd8/ modrm = 0xc0-0xff
        // - num is mod bits 7,6 only + bits 5,4,3 (ie modrm/8)
        public static asminstdata[] asm86subd8b = new asminstdata[]
{ new asminstdata("fadd",0x18,Proc.PROC_FROM8086,0,argtype.ARG_REG_ST0,argtype.ARG_FREG,argtype.ARG_NONE,27000),
  new asminstdata("fmul",0x19,Proc.PROC_FROM8086,0,argtype.ARG_REG_ST0,argtype.ARG_FREG,argtype.ARG_NONE,27001),
  new asminstdata("fcom",0x1a,Proc.PROC_FROM8086,0,argtype.ARG_REG_ST0,argtype.ARG_FREG,argtype.ARG_NONE,27002),
  new asminstdata("fcomp",0x1b,Proc.PROC_FROM8086,0,argtype.ARG_REG_ST0,argtype.ARG_FREG,argtype.ARG_NONE,27003),
  new asminstdata("fsub",0x1c,Proc.PROC_FROM8086,0,argtype.ARG_REG_ST0,argtype.ARG_FREG,argtype.ARG_NONE,27004),
  new asminstdata("fsubr",0x1d,Proc.PROC_FROM8086,0,argtype.ARG_REG_ST0,argtype.ARG_FREG,argtype.ARG_NONE,27005),
  new asminstdata("fdiv",0x1e,Proc.PROC_FROM8086,0,argtype.ARG_REG_ST0,argtype.ARG_FREG,argtype.ARG_NONE,27006),
  new asminstdata("fdivr",0x1f,Proc.PROC_FROM8086,0,argtype.ARG_REG_ST0,argtype.ARG_FREG,argtype.ARG_NONE,27007),
  new asminstdata(null,0x00,0,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,0)  //endCode marker - processor=0 & opcode=0
};

        // subtable 0xd9/modrm = 0x00-0xbf
        // - num is encoding of modrm bits 5,4,3 only
        public static asminstdata[] asm86subd9a = new asminstdata[]
{ new asminstdata("fld",0x0,Proc.PROC_FROM8086,flags.FLAGS_MODRM,argtype.ARG_MODRM_SREAL,argtype.ARG_NONE,argtype.ARG_NONE,28000),
  new asminstdata("fst",0x2,Proc.PROC_FROM8086,flags.FLAGS_MODRM,argtype.ARG_MODRM_SREAL,argtype.ARG_NONE,argtype.ARG_NONE,28001),
  new asminstdata("fstp",0x3,Proc.PROC_FROM8086,flags.FLAGS_MODRM,argtype.ARG_MODRM_SREAL,argtype.ARG_NONE,argtype.ARG_NONE,28002),
  new asminstdata("fldenv",0x4,Proc.PROC_FROM8086,flags.FLAGS_MODRM,argtype.ARG_MODRM_PTR,argtype.ARG_NONE,argtype.ARG_NONE,28003),
  new asminstdata("fldcw",0x5,Proc.PROC_FROM8086,flags.FLAGS_MODRM,argtype.ARG_MODRM_short,argtype.ARG_NONE,argtype.ARG_NONE,28004),
  new asminstdata("fstenv",0x6,Proc.PROC_FROM8086,flags.FLAGS_MODRM,argtype.ARG_MODRM_PTR,argtype.ARG_NONE,argtype.ARG_NONE,28005),
  new asminstdata("fstcw",0x7,Proc.PROC_FROM8086,flags.FLAGS_MODRM,argtype.ARG_MODRM_short,argtype.ARG_NONE,argtype.ARG_NONE,28006),
  new asminstdata(null,0x00,0,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,0)  //endCode marker - processor=0 & opcode=0
};

        // subtable 0xd9/ modrm = 0xc0-0xcf
        // - num is mod bits 7,6 only + bits 5,4,3 (ie modrm/8)
        public static asminstdata[] asm86subd9b = new asminstdata[]
{ new asminstdata("fld",0x18,Proc.PROC_FROM8086,0,argtype.ARG_FREG,argtype.ARG_NONE,argtype.ARG_NONE,29000),
  new asminstdata("fxch",0x19,Proc.PROC_FROM8086,0,argtype.ARG_FREG,argtype.ARG_NONE,argtype.ARG_NONE,29001),
  new asminstdata(null,0x00,0,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,0)  //endCode marker - processor=0 & opcode=0
};

        // subtable 0xd9/ modrm = 0xd0-0xff
        public static asminstdata[] asm86subd9c = new asminstdata[]
{ new asminstdata("fnop",0xd0,Proc.PROC_FROM8086,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,30000),
  new asminstdata("fchs",0xe0,Proc.PROC_FROM8086,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,30001),
  new asminstdata("fabs",0xe1,Proc.PROC_FROM8086,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,30002),
  new asminstdata("ftst",0xe4,Proc.PROC_FROM8086,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,30003),
  new asminstdata("fxam",0xe5,Proc.PROC_FROM8086,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,30004),
  new asminstdata("fld1",0xe8,Proc.PROC_FROM8086,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,30005),
  new asminstdata("fldl2t",0xe9,Proc.PROC_FROM8086,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,30006),
  new asminstdata("fldl2e",0xea,Proc.PROC_FROM8086,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,30007),
  new asminstdata("fldpi",0xeb,Proc.PROC_FROM8086,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,30008),
  new asminstdata("fldlg2",0xec,Proc.PROC_FROM8086,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,30009),
  new asminstdata("fldln2",0xed,Proc.PROC_FROM8086,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,30010),
  new asminstdata("fldz",0xee,Proc.PROC_FROM8086,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,30011),
  new asminstdata("f2xm1",0xf0,Proc.PROC_FROM8086,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,30012),
  new asminstdata("fyl2x",0xf1,Proc.PROC_FROM8086,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,30013),
  new asminstdata("fptan",0xf2,Proc.PROC_FROM8086,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,30014),
  new asminstdata("fpatan",0xf3,Proc.PROC_FROM8086,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,30015),
  new asminstdata("fxtract",0xf4,Proc.PROC_FROM8086,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,30016),
  new asminstdata("fprem1",0xf5,Proc.PROC_FROM80386,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,30017),
  new asminstdata("fdecstp",0xf6,Proc.PROC_FROM8086,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,30018),
  new asminstdata("fincstp",0xf7,Proc.PROC_FROM8086,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,30019),
  new asminstdata("fprem",0xf8,Proc.PROC_FROM8086,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,30020),
  new asminstdata("fyl2xp1",0xf9,Proc.PROC_FROM8086,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,30021),
  new asminstdata("fsqrt",0xfa,Proc.PROC_FROM8086,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,30022),
  new asminstdata("fsincos",0xfb,Proc.PROC_FROM80386,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,30023),
  new asminstdata("frndint",0xfc,Proc.PROC_FROM8086,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,30024),
  new asminstdata("fscale",0xfd,Proc.PROC_FROM8086,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,30025),
  new asminstdata("fsin",0xfe,Proc.PROC_FROM80386,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,30026),
  new asminstdata("fcos",0xff,Proc.PROC_FROM80386,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,30027),
  new asminstdata(null,0x00,0,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,0)  //endCode marker - processor=0 & opcode=0
};

        // subtable 0xda/ modrm = 0x00-0xbf
        // - num is encoding of modrm bits 5,4,3 only
        public static asminstdata[] asm86subdaa = new asminstdata[]
{ new asminstdata("fiadd",0x0,Proc.PROC_FROM8086,flags.FLAGS_MODRM,argtype.ARG_MODRM_SINT,argtype.ARG_NONE,argtype.ARG_NONE,31000),
  new asminstdata("fimul",0x1,Proc.PROC_FROM8086,flags.FLAGS_MODRM,argtype.ARG_MODRM_SINT,argtype.ARG_NONE,argtype.ARG_NONE,31001),
  new asminstdata("ficom",0x2,Proc.PROC_FROM8086,flags.FLAGS_MODRM,argtype.ARG_MODRM_SINT,argtype.ARG_NONE,argtype.ARG_NONE,31002),
  new asminstdata("ficomp",0x3,Proc.PROC_FROM8086,flags.FLAGS_MODRM,argtype.ARG_MODRM_SINT,argtype.ARG_NONE,argtype.ARG_NONE,31003),
  new asminstdata("fisub",0x4,Proc.PROC_FROM8086,flags.FLAGS_MODRM,argtype.ARG_MODRM_SINT,argtype.ARG_NONE,argtype.ARG_NONE,31004),
  new asminstdata("fisubr",0x5,Proc.PROC_FROM8086,flags.FLAGS_MODRM,argtype.ARG_MODRM_SINT,argtype.ARG_NONE,argtype.ARG_NONE,31005),
  new asminstdata("fidiv",0x6,Proc.PROC_FROM8086,flags.FLAGS_MODRM,argtype.ARG_MODRM_SINT,argtype.ARG_NONE,argtype.ARG_NONE,31006),
  new asminstdata("fidivr",0x7,Proc.PROC_FROM8086,flags.FLAGS_MODRM,argtype.ARG_MODRM_SINT,argtype.ARG_NONE,argtype.ARG_NONE,31007),
  new asminstdata(null,0x00,0,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,0)  //endCode marker - processor=0 & opcode=0
};

        // subtable 0xda/ modrm = 0xc0-0xdf
        // - num is mod bits 7,6 only + bits 5,4,3 (ie modrm/8)
        public static asminstdata[] asm86subdab = new asminstdata[]
{ new asminstdata("fmovb",0x18,Proc.PROC_FROMPENTPRO,0,argtype.ARG_REG_ST0,argtype.ARG_FREG,argtype.ARG_NONE,32000),
  new asminstdata("fmove",0x19,Proc.PROC_FROMPENTPRO,0,argtype.ARG_REG_ST0,argtype.ARG_FREG,argtype.ARG_NONE,32001),
  new asminstdata("fmovbe",0x1a,Proc.PROC_FROMPENTPRO,0,argtype.ARG_REG_ST0,argtype.ARG_FREG,argtype.ARG_NONE,32002),
  new asminstdata("fmovu",0x1b,Proc.PROC_FROMPENTPRO,0,argtype.ARG_REG_ST0,argtype.ARG_FREG,argtype.ARG_NONE,32003),
  new asminstdata(null,0x00,0,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,0)  //endCode marker - processor=0 & opcode=0
};

        // subtable 0xda/ modrm = 0xe0-0xff
        public static asminstdata[] asm86subdac = new asminstdata[]
{ new asminstdata("fucompp",0xe9,Proc.PROC_FROM80386,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,33000),
  new asminstdata(null,0x00,0,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,0)  //endCode marker - processor=0 & opcode=0
};

        // subtable 0xdb/modrm = 0x00-0xbf
        // - num is encoding of modrm bits 5,4,3 only
        public static asminstdata[] asm86subdba = new asminstdata[]
{ new asminstdata("fild",0x0,Proc.PROC_FROM8086,flags.FLAGS_MODRM,argtype.ARG_MODRM_SINT,argtype.ARG_NONE,argtype.ARG_NONE,34000),
  new asminstdata("fist",0x2,Proc.PROC_FROM8086,flags.FLAGS_MODRM,argtype.ARG_MODRM_SINT,argtype.ARG_NONE,argtype.ARG_NONE,34001),
  new asminstdata("fistp",0x3,Proc.PROC_FROM8086,flags.FLAGS_MODRM,argtype.ARG_MODRM_SINT,argtype.ARG_NONE,argtype.ARG_NONE,34002),
  new asminstdata("fld",0x5,Proc.PROC_FROM8086,flags.FLAGS_MODRM,argtype.ARG_MODRM_EREAL,argtype.ARG_NONE,argtype.ARG_NONE,34003),
  new asminstdata("fstp",0x7,Proc.PROC_FROM8086,flags.FLAGS_MODRM,argtype.ARG_MODRM_EREAL,argtype.ARG_NONE,argtype.ARG_NONE,34004),
  new asminstdata(null,0x00,0,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,0)  //endCode marker - processor=0 & opcode=0
};

        // subtable 0xdb/ modrm = 0xc0-0xff
        // - num is mod bits 7,6 only + bits 5,4,3 (ie modrm/8)
        public static asminstdata[] asm86subdbb = new asminstdata[]
{ new asminstdata("fcmovnb",0x18,Proc.PROC_FROMPENTPRO,0,argtype.ARG_REG_ST0,argtype.ARG_FREG,argtype.ARG_NONE,35000),
  new asminstdata("fcmovne",0x19,Proc.PROC_FROMPENTPRO,0,argtype.ARG_REG_ST0,argtype.ARG_FREG,argtype.ARG_NONE,35001),
  new asminstdata("fcmovnbe",0x1a,Proc.PROC_FROMPENTPRO,0,argtype.ARG_REG_ST0,argtype.ARG_FREG,argtype.ARG_NONE,35002),
  new asminstdata("fcmovnu",0x1b,Proc.PROC_FROMPENTPRO,0,argtype.ARG_REG_ST0,argtype.ARG_FREG,argtype.ARG_NONE,35003),
  new asminstdata("fucomi",0x1d,Proc.PROC_FROMPENTPRO,0,argtype.ARG_REG_ST0,argtype.ARG_FREG,argtype.ARG_NONE,35004),
  new asminstdata("fcomi",0x1e,Proc.PROC_FROMPENTPRO,0,argtype.ARG_REG_ST0,argtype.ARG_FREG,argtype.ARG_NONE,35005),
  new asminstdata(null,0x00,0,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,0)  //endCode marker - processor=0 & opcode=0
};

        // subtable 0xdb/ modrm = 0xe0-0xff
        public static asminstdata[] asm86subdbc = new asminstdata[]
{ new asminstdata("feni",0xe0,Proc.PROC_FROM8086,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,36000),
  new asminstdata("fdisi",0xe1,Proc.PROC_FROM8086,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,36001),
  new asminstdata("fclex",0xe2,Proc.PROC_FROM8086,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,36002),
  new asminstdata("finit",0xe3,Proc.PROC_FROM8086,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,36003),
  new asminstdata("fsetpm",0xe4,Proc.PROC_FROM80286,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,36004),
  new asminstdata("frstpm",0xe5,Proc.PROC_FROM80286,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,36005),
  new asminstdata("frint2",0xec,Proc.PROC_FROM80386,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,36006),
  new asminstdata(null,0x00,0,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,0)  //endCode marker - processor=0 & opcode=0
};

        // subtable 0xdc/ modrm = 0x00-0xbf
        // - num is encoding of modrm bits 5,4,3 only
        public static asminstdata[] asm86subdca = new asminstdata[]
{ new asminstdata("fadd",0x0,Proc.PROC_FROM8086,flags.FLAGS_MODRM,argtype.ARG_MODRM_DREAL,argtype.ARG_NONE,argtype.ARG_NONE,37000),
  new asminstdata("fmul",0x1,Proc.PROC_FROM8086,flags.FLAGS_MODRM,argtype.ARG_MODRM_DREAL,argtype.ARG_NONE,argtype.ARG_NONE,37001),
  new asminstdata("fcom",0x2,Proc.PROC_FROM8086,flags.FLAGS_MODRM,argtype.ARG_MODRM_DREAL,argtype.ARG_NONE,argtype.ARG_NONE,37002),
  new asminstdata("fcomp",0x3,Proc.PROC_FROM8086,flags.FLAGS_MODRM,argtype.ARG_MODRM_DREAL,argtype.ARG_NONE,argtype.ARG_NONE,37003),
  new asminstdata("fsub",0x4,Proc.PROC_FROM8086,flags.FLAGS_MODRM,argtype.ARG_MODRM_DREAL,argtype.ARG_NONE,argtype.ARG_NONE,37004),
  new asminstdata("fsubr",0x5,Proc.PROC_FROM8086,flags.FLAGS_MODRM,argtype.ARG_MODRM_DREAL,argtype.ARG_NONE,argtype.ARG_NONE,37005),
  new asminstdata("fdiv",0x6,Proc.PROC_FROM8086,flags.FLAGS_MODRM,argtype.ARG_MODRM_DREAL,argtype.ARG_NONE,argtype.ARG_NONE,37006),
  new asminstdata("fdivr",0x7,Proc.PROC_FROM8086,flags.FLAGS_MODRM,argtype.ARG_MODRM_DREAL,argtype.ARG_NONE,argtype.ARG_NONE,37007),
  new asminstdata(null,0x00,0,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,0)  //endCode marker - processor=0 & opcode=0
};

        // subtable 0xdc/ modrm = 0xc0-0xff
        // - num is mod bits 7,6 only + bits 5,4,3 (ie modrm/8)
        public static asminstdata[] asm86subdcb = new asminstdata[]
{ new asminstdata("fadd",0x18,Proc.PROC_FROM8086,0,argtype.ARG_FREG,argtype.ARG_REG_ST0,argtype.ARG_NONE,38000),
  new asminstdata("fmul",0x19,Proc.PROC_FROM8086,0,argtype.ARG_FREG,argtype.ARG_REG_ST0,argtype.ARG_NONE,38001),
  new asminstdata("fcom2",0x1a,Proc.PROC_FROM8086,0,argtype.ARG_FREG,argtype.ARG_REG_ST0,argtype.ARG_NONE,38002),
  new asminstdata("fcomp3",0x1b,Proc.PROC_FROM8086,0,argtype.ARG_FREG,argtype.ARG_REG_ST0,argtype.ARG_NONE,38003),
  new asminstdata("fsub",0x1c,Proc.PROC_FROM8086,0,argtype.ARG_FREG,argtype.ARG_REG_ST0,argtype.ARG_NONE,38004),
  new asminstdata("fsubr",0x1d,Proc.PROC_FROM8086,0,argtype.ARG_FREG,argtype.ARG_REG_ST0,argtype.ARG_NONE,38005),
  new asminstdata("fdiv",0x1e,Proc.PROC_FROM8086,0,argtype.ARG_FREG,argtype.ARG_REG_ST0,argtype.ARG_NONE,38006),
  new asminstdata("fdivr",0x1f,Proc.PROC_FROM8086,0,argtype.ARG_FREG,argtype.ARG_REG_ST0,argtype.ARG_NONE,38007),
  new asminstdata(null,0x00,0,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,0)  //endCode marker - processor=0 & opcode=0
};

        // subtable 0xdd/modrm = 0x00-0xbf
        // - num is encoding of modrm bits 5,4,3 only
        public static asminstdata[] asm86subdda = new asminstdata[]
{ new asminstdata("fld",0x0,Proc.PROC_FROM8086,flags.FLAGS_MODRM,argtype.ARG_MODRM_DREAL,argtype.ARG_NONE,argtype.ARG_NONE,39000),
  new asminstdata("fst",0x2,Proc.PROC_FROM8086,flags.FLAGS_MODRM,argtype.ARG_MODRM_DREAL,argtype.ARG_NONE,argtype.ARG_NONE,39001),
  new asminstdata("fstp",0x3,Proc.PROC_FROM8086,flags.FLAGS_MODRM,argtype.ARG_MODRM_DREAL,argtype.ARG_NONE,argtype.ARG_NONE,39002),
  new asminstdata("frstor",0x4,Proc.PROC_FROM8086,flags.FLAGS_MODRM,argtype.ARG_MODRM_PTR,argtype.ARG_NONE,argtype.ARG_NONE,39003),
  new asminstdata("fsave",0x6,Proc.PROC_FROM8086,flags.FLAGS_MODRM,argtype.ARG_MODRM_PTR,argtype.ARG_NONE,argtype.ARG_NONE,39004),
  new asminstdata("fstsw",0x7,Proc.PROC_FROM8086,flags.FLAGS_MODRM,argtype.ARG_MODRM_short,argtype.ARG_NONE,argtype.ARG_NONE,39005),
  new asminstdata(null,0x00,0,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,0)  //endCode marker - processor=0 & opcode=0
};

        // subtable 0xdd/ modrm = 0xc0-0xff
        // - num is mod bits 7,6 only + bits 5,4,3 (ie modrm/8)
        public static asminstdata[] asm86subddb = new asminstdata[]
{ new asminstdata("ffree",0x18,Proc.PROC_FROM8086,0,argtype.ARG_FREG,argtype.ARG_NONE,argtype.ARG_NONE,40000),
  new asminstdata("fxch4",0x19,Proc.PROC_FROM8086,0,argtype.ARG_FREG,argtype.ARG_NONE,argtype.ARG_NONE,40001),
  new asminstdata("fst",0x1a,Proc.PROC_FROM8086,0,argtype.ARG_FREG,argtype.ARG_NONE,argtype.ARG_NONE,40002),
  new asminstdata("fstp",0x1b,Proc.PROC_FROM8086,0,argtype.ARG_FREG,argtype.ARG_NONE,argtype.ARG_NONE,40003),
  new asminstdata("fucom",0x1c,Proc.PROC_FROM80386,0,argtype.ARG_REG_ST0,argtype.ARG_FREG,argtype.ARG_NONE,40004),
  new asminstdata("fucomp",0x1d,Proc.PROC_FROM80386,0,argtype.ARG_FREG,argtype.ARG_NONE,argtype.ARG_NONE,40005),
  new asminstdata(null,0x00,0,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,0)  //endCode marker - processor=0 & opcode=0
};

        // subtable 0xde/ modrm = 0x00-0xbf
        // - num is encoding of modrm bits 5,4,3 only
        public static asminstdata[] asm86subdea = new asminstdata[]
{ new asminstdata("fiadd",0x0,Proc.PROC_FROM8086,flags.FLAGS_MODRM,argtype.ARG_MODRM_WINT,argtype.ARG_NONE,argtype.ARG_NONE,41000),
  new asminstdata("fimul",0x1,Proc.PROC_FROM8086,flags.FLAGS_MODRM,argtype.ARG_MODRM_WINT,argtype.ARG_NONE,argtype.ARG_NONE,41001),
  new asminstdata("ficom",0x2,Proc.PROC_FROM8086,flags.FLAGS_MODRM,argtype.ARG_MODRM_WINT,argtype.ARG_NONE,argtype.ARG_NONE,41002),
  new asminstdata("ficomp",0x3,Proc.PROC_FROM8086,flags.FLAGS_MODRM,argtype.ARG_MODRM_WINT,argtype.ARG_NONE,argtype.ARG_NONE,41003),
  new asminstdata("fisub",0x4,Proc.PROC_FROM8086,flags.FLAGS_MODRM,argtype.ARG_MODRM_WINT,argtype.ARG_NONE,argtype.ARG_NONE,41004),
  new asminstdata("fisubr",0x5,Proc.PROC_FROM8086,flags.FLAGS_MODRM,argtype.ARG_MODRM_WINT,argtype.ARG_NONE,argtype.ARG_NONE,41005),
  new asminstdata("fidiv",0x6,Proc.PROC_FROM8086,flags.FLAGS_MODRM,argtype.ARG_MODRM_WINT,argtype.ARG_NONE,argtype.ARG_NONE,41006),
  new asminstdata("fidivr",0x7,Proc.PROC_FROM8086,flags.FLAGS_MODRM,argtype.ARG_MODRM_WINT,argtype.ARG_NONE,argtype.ARG_NONE,41007),
  new asminstdata(null,0x00,0,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,0)  //endCode marker - processor=0 & opcode=0
};

        // subtable 0xde/ modrm = 0xc0-0xdf
        // - num is mod bits 7,6 only + bits 5,4,3 (ie modrm/8)
        public static asminstdata[] asm86subdeb = new asminstdata[]
{ new asminstdata("faddp",0x18,Proc.PROC_FROM8086,0,argtype.ARG_FREG,argtype.ARG_REG_ST0,argtype.ARG_NONE,42000),
  new asminstdata("fmulp",0x19,Proc.PROC_FROM8086,0,argtype.ARG_FREG,argtype.ARG_REG_ST0,argtype.ARG_NONE,42001),
  new asminstdata("fcomp5",0x1a,Proc.PROC_FROM8086,0,argtype.ARG_FREG,argtype.ARG_REG_ST0,argtype.ARG_NONE,42002),
  new asminstdata("fsubrp",0x1c,Proc.PROC_FROM8086,0,argtype.ARG_FREG,argtype.ARG_REG_ST0,argtype.ARG_NONE,42003),
  new asminstdata("fsubp",0x1d,Proc.PROC_FROM8086,0,argtype.ARG_FREG,argtype.ARG_REG_ST0,argtype.ARG_NONE,42004),
  new asminstdata("fdivrp",0x1e,Proc.PROC_FROM8086,0,argtype.ARG_FREG,argtype.ARG_REG_ST0,argtype.ARG_NONE,42005),
  new asminstdata("fdivp",0x1f,Proc.PROC_FROM8086,0,argtype.ARG_FREG,argtype.ARG_REG_ST0,argtype.ARG_NONE,42006),
  new asminstdata(null,0x00,0,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,0)  //endCode marker - processor=0 & opcode=0
};

        // subtable 0xde/ modrm = 0xd8-0xdf
        public static asminstdata[] asm86subdec = new asminstdata[]
{ new asminstdata("fcompp",0xd9,Proc.PROC_FROM8086,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,43000),
  new asminstdata(null,0x00,0,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,0)  //endCode marker - processor=0 & opcode=0
};

        // subtable 0xdf/modrm = 0x00-0xbf
        // - num is encoding of modrm bits 5,4,3 only
        public static asminstdata[] asm86subdfa = new asminstdata[]
{ new asminstdata("fild",0x0,Proc.PROC_FROM8086,flags.FLAGS_MODRM,argtype.ARG_MODRM_WINT,argtype.ARG_NONE,argtype.ARG_NONE,44000),
  new asminstdata("fist",0x2,Proc.PROC_FROM8086,flags.FLAGS_MODRM,argtype.ARG_MODRM_WINT,argtype.ARG_NONE,argtype.ARG_NONE,44001),
  new asminstdata("fistp",0x3,Proc.PROC_FROM8086,flags.FLAGS_MODRM,argtype.ARG_MODRM_WINT,argtype.ARG_NONE,argtype.ARG_NONE,44002),
  new asminstdata("fbld",0x4,Proc.PROC_FROM8086,flags.FLAGS_MODRM,argtype.ARG_MODRM_BCD,argtype.ARG_NONE,argtype.ARG_NONE,44003),
  new asminstdata("fild",0x5,Proc.PROC_FROM8086,flags.FLAGS_MODRM,argtype.ARG_MODRM_LINT,argtype.ARG_NONE,argtype.ARG_NONE,44004),
  new asminstdata("fbstp",0x6,Proc.PROC_FROM8086,flags.FLAGS_MODRM,argtype.ARG_MODRM_BCD,argtype.ARG_NONE,argtype.ARG_NONE,44005),
  new asminstdata("fistp",0x7,Proc.PROC_FROM8086,flags.FLAGS_MODRM,argtype.ARG_MODRM_LINT,argtype.ARG_NONE,argtype.ARG_NONE,44006),
  new asminstdata(null,0x00,0,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,0)  //endCode marker - processor=0 & opcode=0
};

        // subtable 0xdf/ modrm = 0xc0-0xff
        // - num is mod bits 7,6 only + bits 5,4,3 (ie modrm/8)
        public static asminstdata[] asm86subdfb = new asminstdata[]
{ new asminstdata("ffreep",0x18,Proc.PROC_FROM8086,0,argtype.ARG_FREG,argtype.ARG_NONE,argtype.ARG_NONE,45000),
  new asminstdata("fxch7",0x19,Proc.PROC_FROM8086,0,argtype.ARG_FREG,argtype.ARG_NONE,argtype.ARG_NONE,45001),
  new asminstdata("fstp8",0x1a,Proc.PROC_FROM8086,0,argtype.ARG_FREG,argtype.ARG_NONE,argtype.ARG_NONE,45002),
  new asminstdata("fstp9",0x1b,Proc.PROC_FROM8086,0,argtype.ARG_FREG,argtype.ARG_NONE,argtype.ARG_NONE,45003),
  new asminstdata("fucomip",0x1d,Proc.PROC_FROMPENTPRO,0,argtype.ARG_REG_ST0,argtype.ARG_FREG,argtype.ARG_NONE,45004),
  new asminstdata("fcomip",0x1e,Proc.PROC_FROMPENTPRO,0,argtype.ARG_FREG,argtype.ARG_NONE,argtype.ARG_NONE,45005),
  new asminstdata(null,0x00,0,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,0)  //endCode marker - processor=0 & opcode=0
};

        // subtable 0xdf/ modrm = 0xe0-0xff
        public static asminstdata[] asm86subdfc = new asminstdata[]
{ new asminstdata("fstsw",0xe0,Proc.PROC_FROM8086,flags.FLAGS_16BIT,argtype.ARG_REG_AX,argtype.ARG_NONE,argtype.ARG_NONE,46000),
  new asminstdata("fstdw",0xe1,Proc.PROC_FROM80386,0,argtype.ARG_REG_AX,argtype.ARG_NONE,argtype.ARG_NONE,46001),
  new asminstdata("fstsg",0xe2,Proc.PROC_FROM80386,0,argtype.ARG_REG_AX,argtype.ARG_NONE,argtype.ARG_NONE,46002),
  new asminstdata("frinear",0xe2,Proc.PROC_FROM80386,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,46003),
  new asminstdata(null,0x00,0,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,0)  //endCode marker - processor=0 & opcode=0
};

        public asminstdata[] asmz80 = new asminstdata[]
{ new asminstdata("nop",0x00,Proc.PROC_Z80,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,100000),
  new asminstdata("ld",0x01,Proc.PROC_Z80,0,argtype.ARG_REG_BC,argtype.ARG_IMM16,argtype.ARG_NONE,100001),
  new asminstdata("ld",0x02,Proc.PROC_Z80,0,argtype.ARG_REG_BC_IND,argtype.ARG_REG_A,argtype.ARG_NONE,100002),
  new asminstdata("inc",0x03,Proc.PROC_Z80,0,argtype.ARG_REG_BC,argtype.ARG_NONE,argtype.ARG_NONE,100003),
  new asminstdata("inc",0x04,Proc.PROC_Z80,0,argtype.ARG_REG_B,argtype.ARG_NONE,argtype.ARG_NONE,100004),
  new asminstdata("dec",0x05,Proc.PROC_Z80,0,argtype.ARG_REG_B,argtype.ARG_NONE,argtype.ARG_NONE,100005),
  new asminstdata("ld",0x06,Proc.PROC_Z80,0,argtype.ARG_REG_B,argtype.ARG_IMM8,argtype.ARG_NONE,100006),
  new asminstdata("rlca",0x07,Proc.PROC_Z80,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,100007),
  new asminstdata("ex",0x08,Proc.PROC_Z80,0,argtype.ARG_REG_AF,argtype.ARG_REG_AF2,argtype.ARG_NONE,100008),
  new asminstdata("add",0x09,Proc.PROC_Z80,0,argtype.ARG_REG_HL,argtype.ARG_REG_BC,argtype.ARG_NONE,100009),
  new asminstdata("ld",0x0a,Proc.PROC_Z80,0,argtype.ARG_REG_A,argtype.ARG_REG_BC_IND,argtype.ARG_NONE,100010),
  new asminstdata("dec",0x0b,Proc.PROC_Z80,0,argtype.ARG_REG_BC,argtype.ARG_NONE,argtype.ARG_NONE,100011),
  new asminstdata("inc",0x0c,Proc.PROC_Z80,0,argtype.ARG_REG_C,argtype.ARG_NONE,argtype.ARG_NONE,100012),
  new asminstdata("dec",0x0d,Proc.PROC_Z80,0,argtype.ARG_REG_C,argtype.ARG_NONE,argtype.ARG_NONE,100013),
  new asminstdata("ld",0x0e,Proc.PROC_Z80,0,argtype.ARG_REG_C,argtype.ARG_IMM8,argtype.ARG_NONE,100014),
  new asminstdata("rrca",0x0f,Proc.PROC_Z80,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,100015),
  new asminstdata("djnz",0x10,Proc.PROC_Z80,flags.FLAGS_CJMP,argtype.ARG_RELIMM8,argtype.ARG_NONE,argtype.ARG_NONE,100016),
  new asminstdata("ld",0x11,Proc.PROC_Z80,0,argtype.ARG_REG_DE,argtype.ARG_IMM16,argtype.ARG_NONE,100017),
  new asminstdata("ld",0x12,Proc.PROC_Z80,0,argtype.ARG_REG_DE_IND,argtype.ARG_REG_A,argtype.ARG_NONE,100018),
  new asminstdata("inc",0x13,Proc.PROC_Z80,0,argtype.ARG_REG_DE,argtype.ARG_NONE,argtype.ARG_NONE,100019),
  new asminstdata("inc",0x14,Proc.PROC_Z80,0,argtype.ARG_REG_D,argtype.ARG_NONE,argtype.ARG_NONE,100020),
  new asminstdata("dec",0x15,Proc.PROC_Z80,0,argtype.ARG_REG_D,argtype.ARG_NONE,argtype.ARG_NONE,100021),
  new asminstdata("ld",0x16,Proc.PROC_Z80,0,argtype.ARG_REG_D,argtype.ARG_IMM8,argtype.ARG_NONE,100022),
  new asminstdata("rla",0x17,Proc.PROC_Z80,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,100023),
  new asminstdata("jr",0x18,Proc.PROC_Z80,flags.FLAGS_JMP,argtype.ARG_RELIMM8,argtype.ARG_NONE,argtype.ARG_NONE,100024),
  new asminstdata("add",0x19,Proc.PROC_Z80,0,argtype.ARG_REG_HL,argtype.ARG_REG_DE,argtype.ARG_NONE,100025),
  new asminstdata("ld",0x1a,Proc.PROC_Z80,0,argtype.ARG_REG_A,argtype.ARG_REG_DE_IND,argtype.ARG_NONE,100026),
  new asminstdata("dec",0x1b,Proc.PROC_Z80,0,argtype.ARG_REG_DE,argtype.ARG_NONE,argtype.ARG_NONE,100027),
  new asminstdata("inc",0x1c,Proc.PROC_Z80,0,argtype.ARG_REG_E,argtype.ARG_NONE,argtype.ARG_NONE,100028),
  new asminstdata("dec",0x1d,Proc.PROC_Z80,0,argtype.ARG_REG_E,argtype.ARG_NONE,argtype.ARG_NONE,100029),
  new asminstdata("ld",0x1e,Proc.PROC_Z80,0,argtype.ARG_REG_E,argtype.ARG_IMM8,argtype.ARG_NONE,100030),
  new asminstdata("rra",0x1f,Proc.PROC_Z80,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,100031),
  new asminstdata("jr nz",0x20,Proc.PROC_Z80,flags.FLAGS_CJMP,argtype.ARG_RELIMM8,argtype.ARG_NONE,argtype.ARG_NONE,100032),
  new asminstdata("ld",0x21,Proc.PROC_Z80,0,argtype.ARG_REG_HL,argtype.ARG_IMM16,argtype.ARG_NONE,100033),
  new asminstdata("ld",0x22,Proc.PROC_Z80,0,argtype.ARG_MEMLOC16,argtype.ARG_REG_HL,argtype.ARG_NONE,100034),
  new asminstdata("inc",0x23,Proc.PROC_Z80,0,argtype.ARG_REG_HL,argtype.ARG_NONE,argtype.ARG_NONE,100035),
  new asminstdata("inc",0x24,Proc.PROC_Z80,0,argtype.ARG_REG_H,argtype.ARG_NONE,argtype.ARG_NONE,100036),
  new asminstdata("dec",0x25,Proc.PROC_Z80,0,argtype.ARG_REG_H,argtype.ARG_NONE,argtype.ARG_NONE,100037),
  new asminstdata("ld",0x26,Proc.PROC_Z80,0,argtype.ARG_REG_H,argtype.ARG_IMM8,argtype.ARG_NONE,100038),
  new asminstdata("daa",0x27,Proc.PROC_Z80,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,100039),
  new asminstdata("jr z",0x28,Proc.PROC_Z80,flags.FLAGS_CJMP,argtype.ARG_RELIMM8,argtype.ARG_NONE,argtype.ARG_NONE,100040),
  new asminstdata("add",0x29,Proc.PROC_Z80,0,argtype.ARG_REG_HL,argtype.ARG_REG_HL,argtype.ARG_NONE,100041),
  new asminstdata("ld",0x2a,Proc.PROC_Z80,0,argtype.ARG_REG_HL,argtype.ARG_MEMLOC16,argtype.ARG_NONE,100042),
  new asminstdata("dec",0x2b,Proc.PROC_Z80,0,argtype.ARG_REG_HL,argtype.ARG_NONE,argtype.ARG_NONE,100043),
  new asminstdata("inc",0x2c,Proc.PROC_Z80,0,argtype.ARG_REG_L,argtype.ARG_NONE,argtype.ARG_NONE,100044),
  new asminstdata("dec",0x2d,Proc.PROC_Z80,0,argtype.ARG_REG_L,argtype.ARG_NONE,argtype.ARG_NONE,100045),
  new asminstdata("ld",0x2e,Proc.PROC_Z80,0,argtype.ARG_REG_L,argtype.ARG_IMM8,argtype.ARG_NONE,100046),
  new asminstdata("cpl",0x2f,Proc.PROC_Z80,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,100047),
  new asminstdata("jr nc",0x30,Proc.PROC_Z80,flags.FLAGS_CJMP,argtype.ARG_RELIMM8,argtype.ARG_NONE,argtype.ARG_NONE,100048),
  new asminstdata("ld",0x31,Proc.PROC_Z80,0,argtype.ARG_REG_SP,argtype.ARG_IMM16,argtype.ARG_NONE,100049),
  new asminstdata("ld",0x32,Proc.PROC_Z80,0,argtype.ARG_MEMLOC16,argtype.ARG_REG_A,argtype.ARG_NONE,100050),
  new asminstdata("inc",0x33,Proc.PROC_Z80,0,argtype.ARG_REG_SP,argtype.ARG_NONE,argtype.ARG_NONE,100051),
  new asminstdata("inc",0x34,Proc.PROC_Z80,0,argtype.ARG_REG_HL_IND,argtype.ARG_NONE,argtype.ARG_NONE,100052),
  new asminstdata("dec",0x35,Proc.PROC_Z80,0,argtype.ARG_REG_HL_IND,argtype.ARG_NONE,argtype.ARG_NONE,100053),
  new asminstdata("ld",0x36,Proc.PROC_Z80,0,argtype.ARG_REG_HL_IND,argtype.ARG_IMM8,argtype.ARG_NONE,100054),
  new asminstdata("scf",0x37,Proc.PROC_Z80,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,100055),
  new asminstdata("jr c",0x38,Proc.PROC_Z80,flags.FLAGS_CJMP,argtype.ARG_RELIMM8,argtype.ARG_NONE,argtype.ARG_NONE,100056),
  new asminstdata("add",0x39,Proc.PROC_Z80,0,argtype.ARG_REG_HL,argtype.ARG_REG_SP,argtype.ARG_NONE,100057),
  new asminstdata("ld",0x3a,Proc.PROC_Z80,0,argtype.ARG_REG_A,argtype.ARG_MEMLOC16,argtype.ARG_NONE,100058),
  new asminstdata("dec",0x3b,Proc.PROC_Z80,0,argtype.ARG_REG_SP,argtype.ARG_NONE,argtype.ARG_NONE,100059),
  new asminstdata("inc",0x3c,Proc.PROC_Z80,0,argtype.ARG_REG_A,argtype.ARG_NONE,argtype.ARG_NONE,100060),
  new asminstdata("dec",0x3d,Proc.PROC_Z80,0,argtype.ARG_REG_A,argtype.ARG_NONE,argtype.ARG_NONE,100061),
  new asminstdata("ld",0x3e,Proc.PROC_Z80,0,argtype.ARG_REG_A,argtype.ARG_IMM8,argtype.ARG_NONE,100062),
  new asminstdata("ccf",0x3f,Proc.PROC_Z80,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,100063),
  new asminstdata("ld",0x40,Proc.PROC_Z80,0,argtype.ARG_REG_B,argtype.ARG_REG_B,argtype.ARG_NONE,100064),
  new asminstdata("ld",0x41,Proc.PROC_Z80,0,argtype.ARG_REG_B,argtype.ARG_REG_C,argtype.ARG_NONE,100065),
  new asminstdata("ld",0x42,Proc.PROC_Z80,0,argtype.ARG_REG_B,argtype.ARG_REG_D,argtype.ARG_NONE,100066),
  new asminstdata("ld",0x43,Proc.PROC_Z80,0,argtype.ARG_REG_B,argtype.ARG_REG_E,argtype.ARG_NONE,100067),
  new asminstdata("ld",0x44,Proc.PROC_Z80,0,argtype.ARG_REG_B,argtype.ARG_REG_H,argtype.ARG_NONE,100068),
  new asminstdata("ld",0x45,Proc.PROC_Z80,0,argtype.ARG_REG_B,argtype.ARG_REG_L,argtype.ARG_NONE,100069),
  new asminstdata("ld",0x46,Proc.PROC_Z80,0,argtype.ARG_REG_B,argtype.ARG_REG_HL_IND,argtype.ARG_NONE,100070),
  new asminstdata("ld",0x47,Proc.PROC_Z80,0,argtype.ARG_REG_B,argtype.ARG_REG_A,argtype.ARG_NONE,100071),
  new asminstdata("ld",0x48,Proc.PROC_Z80,0,argtype.ARG_REG_C,argtype.ARG_REG_B,argtype.ARG_NONE,100072),
  new asminstdata("ld",0x49,Proc.PROC_Z80,0,argtype.ARG_REG_C,argtype.ARG_REG_C,argtype.ARG_NONE,100073),
  new asminstdata("ld",0x4a,Proc.PROC_Z80,0,argtype.ARG_REG_C,argtype.ARG_REG_D,argtype.ARG_NONE,100074),
  new asminstdata("ld",0x4b,Proc.PROC_Z80,0,argtype.ARG_REG_C,argtype.ARG_REG_E,argtype.ARG_NONE,100075),
  new asminstdata("ld",0x4c,Proc.PROC_Z80,0,argtype.ARG_REG_C,argtype.ARG_REG_H,argtype.ARG_NONE,100076),
  new asminstdata("ld",0x4d,Proc.PROC_Z80,0,argtype.ARG_REG_C,argtype.ARG_REG_L,argtype.ARG_NONE,100077),
  new asminstdata("ld",0x4e,Proc.PROC_Z80,0,argtype.ARG_REG_C,argtype.ARG_REG_HL_IND,argtype.ARG_NONE,100078),
  new asminstdata("ld",0x4f,Proc.PROC_Z80,0,argtype.ARG_REG_C,argtype.ARG_REG_A,argtype.ARG_NONE,100079),
  new asminstdata("ld",0x50,Proc.PROC_Z80,0,argtype.ARG_REG_D,argtype.ARG_REG_B,argtype.ARG_NONE,100080),
  new asminstdata("ld",0x51,Proc.PROC_Z80,0,argtype.ARG_REG_D,argtype.ARG_REG_C,argtype.ARG_NONE,100081),
  new asminstdata("ld",0x52,Proc.PROC_Z80,0,argtype.ARG_REG_D,argtype.ARG_REG_D,argtype.ARG_NONE,100082),
  new asminstdata("ld",0x53,Proc.PROC_Z80,0,argtype.ARG_REG_D,argtype.ARG_REG_E,argtype.ARG_NONE,100083),
  new asminstdata("ld",0x54,Proc.PROC_Z80,0,argtype.ARG_REG_D,argtype.ARG_REG_H,argtype.ARG_NONE,100084),
  new asminstdata("ld",0x55,Proc.PROC_Z80,0,argtype.ARG_REG_D,argtype.ARG_REG_L,argtype.ARG_NONE,100085),
  new asminstdata("ld",0x56,Proc.PROC_Z80,0,argtype.ARG_REG_D,argtype.ARG_REG_HL_IND,argtype.ARG_NONE,100086),
  new asminstdata("ld",0x57,Proc.PROC_Z80,0,argtype.ARG_REG_D,argtype.ARG_REG_A,argtype.ARG_NONE,100087),
  new asminstdata("ld",0x58,Proc.PROC_Z80,0,argtype.ARG_REG_E,argtype.ARG_REG_B,argtype.ARG_NONE,100088),
  new asminstdata("ld",0x59,Proc.PROC_Z80,0,argtype.ARG_REG_E,argtype.ARG_REG_C,argtype.ARG_NONE,100089),
  new asminstdata("ld",0x5a,Proc.PROC_Z80,0,argtype.ARG_REG_E,argtype.ARG_REG_D,argtype.ARG_NONE,100090),
  new asminstdata("ld",0x5b,Proc.PROC_Z80,0,argtype.ARG_REG_E,argtype.ARG_REG_E,argtype.ARG_NONE,100091),
  new asminstdata("ld",0x5c,Proc.PROC_Z80,0,argtype.ARG_REG_E,argtype.ARG_REG_H,argtype.ARG_NONE,100092),
  new asminstdata("ld",0x5d,Proc.PROC_Z80,0,argtype.ARG_REG_E,argtype.ARG_REG_L,argtype.ARG_NONE,100093),
  new asminstdata("ld",0x5e,Proc.PROC_Z80,0,argtype.ARG_REG_E,argtype.ARG_REG_HL_IND,argtype.ARG_NONE,100094),
  new asminstdata("ld",0x5f,Proc.PROC_Z80,0,argtype.ARG_REG_E,argtype.ARG_REG_A,argtype.ARG_NONE,100095),
  new asminstdata("ld",0x60,Proc.PROC_Z80,0,argtype.ARG_REG_H,argtype.ARG_REG_B,argtype.ARG_NONE,100096),
  new asminstdata("ld",0x61,Proc.PROC_Z80,0,argtype.ARG_REG_H,argtype.ARG_REG_C,argtype.ARG_NONE,100097),
  new asminstdata("ld",0x62,Proc.PROC_Z80,0,argtype.ARG_REG_H,argtype.ARG_REG_D,argtype.ARG_NONE,100098),
  new asminstdata("ld",0x63,Proc.PROC_Z80,0,argtype.ARG_REG_H,argtype.ARG_REG_E,argtype.ARG_NONE,100099),
  new asminstdata("ld",0x64,Proc.PROC_Z80,0,argtype.ARG_REG_H,argtype.ARG_REG_H,argtype.ARG_NONE,100100),
  new asminstdata("ld",0x65,Proc.PROC_Z80,0,argtype.ARG_REG_H,argtype.ARG_REG_L,argtype.ARG_NONE,100101),
  new asminstdata("ld",0x66,Proc.PROC_Z80,0,argtype.ARG_REG_H,argtype.ARG_REG_HL_IND,argtype.ARG_NONE,100102),
  new asminstdata("ld",0x67,Proc.PROC_Z80,0,argtype.ARG_REG_H,argtype.ARG_REG_A,argtype.ARG_NONE,100103),
  new asminstdata("ld",0x68,Proc.PROC_Z80,0,argtype.ARG_REG_L,argtype.ARG_REG_B,argtype.ARG_NONE,100104),
  new asminstdata("ld",0x69,Proc.PROC_Z80,0,argtype.ARG_REG_L,argtype.ARG_REG_C,argtype.ARG_NONE,100105),
  new asminstdata("ld",0x6a,Proc.PROC_Z80,0,argtype.ARG_REG_L,argtype.ARG_REG_D,argtype.ARG_NONE,100106),
  new asminstdata("ld",0x6b,Proc.PROC_Z80,0,argtype.ARG_REG_L,argtype.ARG_REG_E,argtype.ARG_NONE,100107),
  new asminstdata("ld",0x6c,Proc.PROC_Z80,0,argtype.ARG_REG_L,argtype.ARG_REG_H,argtype.ARG_NONE,100108),
  new asminstdata("ld",0x6d,Proc.PROC_Z80,0,argtype.ARG_REG_L,argtype.ARG_REG_L,argtype.ARG_NONE,100109),
  new asminstdata("ld",0x6e,Proc.PROC_Z80,0,argtype.ARG_REG_L,argtype.ARG_REG_HL_IND,argtype.ARG_NONE,100110),
  new asminstdata("ld",0x6f,Proc.PROC_Z80,0,argtype.ARG_REG_L,argtype.ARG_REG_A,argtype.ARG_NONE,100111),
  new asminstdata("ld",0x70,Proc.PROC_Z80,0,argtype.ARG_REG_HL_IND,argtype.ARG_REG_B,argtype.ARG_NONE,100112),
  new asminstdata("ld",0x71,Proc.PROC_Z80,0,argtype.ARG_REG_HL_IND,argtype.ARG_REG_C,argtype.ARG_NONE,100113),
  new asminstdata("ld",0x72,Proc.PROC_Z80,0,argtype.ARG_REG_HL_IND,argtype.ARG_REG_D,argtype.ARG_NONE,100114),
  new asminstdata("ld",0x73,Proc.PROC_Z80,0,argtype.ARG_REG_HL_IND,argtype.ARG_REG_E,argtype.ARG_NONE,100115),
  new asminstdata("ld",0x74,Proc.PROC_Z80,0,argtype.ARG_REG_HL_IND,argtype.ARG_REG_H,argtype.ARG_NONE,100116),
  new asminstdata("ld",0x75,Proc.PROC_Z80,0,argtype.ARG_REG_HL_IND,argtype.ARG_REG_L,argtype.ARG_NONE,100117),
  new asminstdata("halt",0x76,Proc.PROC_Z80,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,100118),
  new asminstdata("ld",0x77,Proc.PROC_Z80,0,argtype.ARG_REG_HL_IND,argtype.ARG_REG_A,argtype.ARG_NONE,100119),
  new asminstdata("ld",0x78,Proc.PROC_Z80,0,argtype.ARG_REG_A,argtype.ARG_REG_B,argtype.ARG_NONE,100120),
  new asminstdata("ld",0x79,Proc.PROC_Z80,0,argtype.ARG_REG_A,argtype.ARG_REG_C,argtype.ARG_NONE,100121),
  new asminstdata("ld",0x7a,Proc.PROC_Z80,0,argtype.ARG_REG_A,argtype.ARG_REG_D,argtype.ARG_NONE,100122),
  new asminstdata("ld",0x7b,Proc.PROC_Z80,0,argtype.ARG_REG_A,argtype.ARG_REG_E,argtype.ARG_NONE,100123),
  new asminstdata("ld",0x7c,Proc.PROC_Z80,0,argtype.ARG_REG_A,argtype.ARG_REG_H,argtype.ARG_NONE,100124),
  new asminstdata("ld",0x7d,Proc.PROC_Z80,0,argtype.ARG_REG_A,argtype.ARG_REG_L,argtype.ARG_NONE,100125),
  new asminstdata("ld",0x7e,Proc.PROC_Z80,0,argtype.ARG_REG_A,argtype.ARG_REG_HL_IND,argtype.ARG_NONE,100126),
  new asminstdata("ld",0x7f,Proc.PROC_Z80,0,argtype.ARG_REG_A,argtype.ARG_REG_A,argtype.ARG_NONE,100127),
  new asminstdata("add",0x80,Proc.PROC_Z80,0,argtype.ARG_REG_A,argtype.ARG_REG_B,argtype.ARG_NONE,100128),
  new asminstdata("add",0x81,Proc.PROC_Z80,0,argtype.ARG_REG_A,argtype.ARG_REG_C,argtype.ARG_NONE,100129),
  new asminstdata("add",0x82,Proc.PROC_Z80,0,argtype.ARG_REG_A,argtype.ARG_REG_D,argtype.ARG_NONE,100130),
  new asminstdata("add",0x83,Proc.PROC_Z80,0,argtype.ARG_REG_A,argtype.ARG_REG_E,argtype.ARG_NONE,100131),
  new asminstdata("add",0x84,Proc.PROC_Z80,0,argtype.ARG_REG_A,argtype.ARG_REG_H,argtype.ARG_NONE,100132),
  new asminstdata("add",0x85,Proc.PROC_Z80,0,argtype.ARG_REG_A,argtype.ARG_REG_L,argtype.ARG_NONE,100133),
  new asminstdata("add",0x86,Proc.PROC_Z80,0,argtype.ARG_REG_A,argtype.ARG_REG_HL_IND,argtype.ARG_NONE,100134),
  new asminstdata("add",0x87,Proc.PROC_Z80,0,argtype.ARG_REG_A,argtype.ARG_REG_A,argtype.ARG_NONE,100135),
  new asminstdata("adc",0x88,Proc.PROC_Z80,0,argtype.ARG_REG_A,argtype.ARG_REG_B,argtype.ARG_NONE,100136),
  new asminstdata("adc",0x89,Proc.PROC_Z80,0,argtype.ARG_REG_A,argtype.ARG_REG_C,argtype.ARG_NONE,100137),
  new asminstdata("adc",0x8a,Proc.PROC_Z80,0,argtype.ARG_REG_A,argtype.ARG_REG_D,argtype.ARG_NONE,100138),
  new asminstdata("adc",0x8b,Proc.PROC_Z80,0,argtype.ARG_REG_A,argtype.ARG_REG_E,argtype.ARG_NONE,100139),
  new asminstdata("adc",0x8c,Proc.PROC_Z80,0,argtype.ARG_REG_A,argtype.ARG_REG_H,argtype.ARG_NONE,100140),
  new asminstdata("adc",0x8d,Proc.PROC_Z80,0,argtype.ARG_REG_A,argtype.ARG_REG_L,argtype.ARG_NONE,100141),
  new asminstdata("adc",0x8e,Proc.PROC_Z80,0,argtype.ARG_REG_A,argtype.ARG_REG_HL_IND,argtype.ARG_NONE,100142),
  new asminstdata("adc",0x8f,Proc.PROC_Z80,0,argtype.ARG_REG_A,argtype.ARG_REG_A,argtype.ARG_NONE,100143),
  new asminstdata("sub",0x90,Proc.PROC_Z80,0,argtype.ARG_REG_A,argtype.ARG_REG_B,argtype.ARG_NONE,100144),
  new asminstdata("sub",0x91,Proc.PROC_Z80,0,argtype.ARG_REG_A,argtype.ARG_REG_C,argtype.ARG_NONE,100145),
  new asminstdata("sub",0x92,Proc.PROC_Z80,0,argtype.ARG_REG_A,argtype.ARG_REG_D,argtype.ARG_NONE,100146),
  new asminstdata("sub",0x93,Proc.PROC_Z80,0,argtype.ARG_REG_A,argtype.ARG_REG_E,argtype.ARG_NONE,100147),
  new asminstdata("sub",0x94,Proc.PROC_Z80,0,argtype.ARG_REG_A,argtype.ARG_REG_H,argtype.ARG_NONE,100148),
  new asminstdata("sub",0x95,Proc.PROC_Z80,0,argtype.ARG_REG_A,argtype.ARG_REG_L,argtype.ARG_NONE,100149),
  new asminstdata("sub",0x96,Proc.PROC_Z80,0,argtype.ARG_REG_A,argtype.ARG_REG_HL_IND,argtype.ARG_NONE,100150),
  new asminstdata("sub",0x97,Proc.PROC_Z80,0,argtype.ARG_REG_A,argtype.ARG_REG_A,argtype.ARG_NONE,100151),
  new asminstdata("sbc",0x98,Proc.PROC_Z80,0,argtype.ARG_REG_A,argtype.ARG_REG_B,argtype.ARG_NONE,100152),
  new asminstdata("sbc",0x99,Proc.PROC_Z80,0,argtype.ARG_REG_A,argtype.ARG_REG_C,argtype.ARG_NONE,100153),
  new asminstdata("sbc",0x9a,Proc.PROC_Z80,0,argtype.ARG_REG_A,argtype.ARG_REG_D,argtype.ARG_NONE,100154),
  new asminstdata("sbc",0x9b,Proc.PROC_Z80,0,argtype.ARG_REG_A,argtype.ARG_REG_E,argtype.ARG_NONE,100155),
  new asminstdata("sbc",0x9c,Proc.PROC_Z80,0,argtype.ARG_REG_A,argtype.ARG_REG_H,argtype.ARG_NONE,100156),
  new asminstdata("sbc",0x9d,Proc.PROC_Z80,0,argtype.ARG_REG_A,argtype.ARG_REG_L,argtype.ARG_NONE,100157),
  new asminstdata("sbc",0x9e,Proc.PROC_Z80,0,argtype.ARG_REG_A,argtype.ARG_REG_HL_IND,argtype.ARG_NONE,100158),
  new asminstdata("sbc",0x9f,Proc.PROC_Z80,0,argtype.ARG_REG_A,argtype.ARG_REG_A,argtype.ARG_NONE,100159),
  new asminstdata("and",0xa0,Proc.PROC_Z80,0,argtype.ARG_REG_A,argtype.ARG_REG_B,argtype.ARG_NONE,100160),
  new asminstdata("and",0xa1,Proc.PROC_Z80,0,argtype.ARG_REG_A,argtype.ARG_REG_C,argtype.ARG_NONE,100161),
  new asminstdata("and",0xa2,Proc.PROC_Z80,0,argtype.ARG_REG_A,argtype.ARG_REG_D,argtype.ARG_NONE,100162),
  new asminstdata("and",0xa3,Proc.PROC_Z80,0,argtype.ARG_REG_A,argtype.ARG_REG_E,argtype.ARG_NONE,100163),
  new asminstdata("and",0xa4,Proc.PROC_Z80,0,argtype.ARG_REG_A,argtype.ARG_REG_H,argtype.ARG_NONE,100164),
  new asminstdata("and",0xa5,Proc.PROC_Z80,0,argtype.ARG_REG_A,argtype.ARG_REG_L,argtype.ARG_NONE,100165),
  new asminstdata("and",0xa6,Proc.PROC_Z80,0,argtype.ARG_REG_A,argtype.ARG_REG_HL_IND,argtype.ARG_NONE,100166),
  new asminstdata("and",0xa7,Proc.PROC_Z80,0,argtype.ARG_REG_A,argtype.ARG_REG_A,argtype.ARG_NONE,100167),
  new asminstdata("xor",0xa8,Proc.PROC_Z80,0,argtype.ARG_REG_A,argtype.ARG_REG_B,argtype.ARG_NONE,100168),
  new asminstdata("xor",0xa9,Proc.PROC_Z80,0,argtype.ARG_REG_A,argtype.ARG_REG_C,argtype.ARG_NONE,100169),
  new asminstdata("xor",0xaa,Proc.PROC_Z80,0,argtype.ARG_REG_A,argtype.ARG_REG_D,argtype.ARG_NONE,100170),
  new asminstdata("xor",0xab,Proc.PROC_Z80,0,argtype.ARG_REG_A,argtype.ARG_REG_E,argtype.ARG_NONE,100171),
  new asminstdata("xor",0xac,Proc.PROC_Z80,0,argtype.ARG_REG_A,argtype.ARG_REG_H,argtype.ARG_NONE,100172),
  new asminstdata("xor",0xad,Proc.PROC_Z80,0,argtype.ARG_REG_A,argtype.ARG_REG_L,argtype.ARG_NONE,100173),
  new asminstdata("xor",0xae,Proc.PROC_Z80,0,argtype.ARG_REG_A,argtype.ARG_REG_HL_IND,argtype.ARG_NONE,100174),
  new asminstdata("xor",0xaf,Proc.PROC_Z80,0,argtype.ARG_REG_A,argtype.ARG_REG_A,argtype.ARG_NONE,100175),
  new asminstdata("or",0xb0,Proc.PROC_Z80,0,argtype.ARG_REG_A,argtype.ARG_REG_B,argtype.ARG_NONE,100176),
  new asminstdata("or",0xb1,Proc.PROC_Z80,0,argtype.ARG_REG_A,argtype.ARG_REG_C,argtype.ARG_NONE,100177),
  new asminstdata("or",0xb2,Proc.PROC_Z80,0,argtype.ARG_REG_A,argtype.ARG_REG_D,argtype.ARG_NONE,100178),
  new asminstdata("or",0xb3,Proc.PROC_Z80,0,argtype.ARG_REG_A,argtype.ARG_REG_E,argtype.ARG_NONE,100179),
  new asminstdata("or",0xb4,Proc.PROC_Z80,0,argtype.ARG_REG_A,argtype.ARG_REG_H,argtype.ARG_NONE,100180),
  new asminstdata("or",0xb5,Proc.PROC_Z80,0,argtype.ARG_REG_A,argtype.ARG_REG_L,argtype.ARG_NONE,100181),
  new asminstdata("or",0xb6,Proc.PROC_Z80,0,argtype.ARG_REG_A,argtype.ARG_REG_HL_IND,argtype.ARG_NONE,100182),
  new asminstdata("or",0xb7,Proc.PROC_Z80,0,argtype.ARG_REG_A,argtype.ARG_REG_A,argtype.ARG_NONE,100183),
  new asminstdata("cp",0xb8,Proc.PROC_Z80,0,argtype.ARG_REG_A,argtype.ARG_REG_B,argtype.ARG_NONE,100184),
  new asminstdata("cp",0xb9,Proc.PROC_Z80,0,argtype.ARG_REG_A,argtype.ARG_REG_C,argtype.ARG_NONE,100185),
  new asminstdata("cp",0xba,Proc.PROC_Z80,0,argtype.ARG_REG_A,argtype.ARG_REG_D,argtype.ARG_NONE,100186),
  new asminstdata("cp",0xbb,Proc.PROC_Z80,0,argtype.ARG_REG_A,argtype.ARG_REG_E,argtype.ARG_NONE,100187),
  new asminstdata("cp",0xbc,Proc.PROC_Z80,0,argtype.ARG_REG_A,argtype.ARG_REG_H,argtype.ARG_NONE,100188),
  new asminstdata("cp",0xbd,Proc.PROC_Z80,0,argtype.ARG_REG_A,argtype.ARG_REG_L,argtype.ARG_NONE,100189),
  new asminstdata("cp",0xbe,Proc.PROC_Z80,0,argtype.ARG_REG_A,argtype.ARG_REG_HL_IND,argtype.ARG_NONE,100190),
  new asminstdata("cp",0xbf,Proc.PROC_Z80,0,argtype.ARG_REG_A,argtype.ARG_REG_A,argtype.ARG_NONE,100191),
  new asminstdata("ret nz",0xc0,Proc.PROC_Z80,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,100192),
  new asminstdata("pop",0xc1,Proc.PROC_Z80,0,argtype.ARG_REG_BC,argtype.ARG_NONE,argtype.ARG_NONE,100193),
  new asminstdata("jp nz",0xc2,Proc.PROC_Z80,flags.FLAGS_CJMP,argtype.ARG_IMM16,argtype.ARG_NONE,argtype.ARG_NONE,100194),
  new asminstdata("jp",0xc3,Proc.PROC_Z80,flags.FLAGS_JMP,argtype.ARG_IMM16,argtype.ARG_NONE,argtype.ARG_NONE,100195),
  new asminstdata("call nz",0xc4,Proc.PROC_Z80,flags.FLAGS_CALL,argtype.ARG_IMM16,argtype.ARG_NONE,argtype.ARG_NONE,100196),
  new asminstdata("push",0xc5,Proc.PROC_Z80,0,argtype.ARG_REG_BC,argtype.ARG_NONE,argtype.ARG_NONE,100197),
  new asminstdata("add",0xc6,Proc.PROC_Z80,0,argtype.ARG_REG_A,argtype.ARG_IMM8,argtype.ARG_NONE,100198),
  new asminstdata("rst 00h",0xc7,Proc.PROC_Z80,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,100199),
  new asminstdata("ret z",0xc8,Proc.PROC_Z80,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,100200),
  new asminstdata("ret",0xc9,Proc.PROC_Z80,flags.FLAGS_RET,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,100201),
  new asminstdata("jp z",0xca,Proc.PROC_Z80,flags.FLAGS_CJMP,argtype.ARG_IMM16,argtype.ARG_NONE,argtype.ARG_NONE,100202),
  new asminstdata(null,0xcb,Proc.PROC_Z80,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,100203), // subtable 0xcb
  new asminstdata("call z",0xcc,Proc.PROC_Z80,flags.FLAGS_CALL,argtype.ARG_IMM16,argtype.ARG_NONE,argtype.ARG_NONE,100204),
  new asminstdata("call",0xcd,Proc.PROC_Z80,flags.FLAGS_CALL,argtype.ARG_IMM16,argtype.ARG_NONE,argtype.ARG_NONE,100205),
  new asminstdata("adc",0xce,Proc.PROC_Z80,0,argtype.ARG_REG_A,argtype.ARG_IMM8,argtype.ARG_NONE,100206),
  new asminstdata("rst 08h",0xcf,Proc.PROC_Z80,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,100207),
  new asminstdata("ret nc",0xd0,Proc.PROC_Z80,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,100208),
  new asminstdata("pop",0xd1,Proc.PROC_Z80,0,argtype.ARG_REG_DE,argtype.ARG_NONE,argtype.ARG_NONE,100209),
  new asminstdata("jp nc",0xd2,Proc.PROC_Z80,flags.FLAGS_CJMP,argtype.ARG_IMM16,argtype.ARG_NONE,argtype.ARG_NONE,100210),
  new asminstdata("out",0xd3,Proc.PROC_Z80,0,argtype.ARG_IMM8_IND,argtype.ARG_REG_A,argtype.ARG_NONE,100211),
  new asminstdata("call nc",0xd4,Proc.PROC_Z80,flags.FLAGS_CALL,argtype.ARG_IMM16,argtype.ARG_NONE,argtype.ARG_NONE,100212),
  new asminstdata("push",0xd5,Proc.PROC_Z80,0,argtype.ARG_REG_DE,argtype.ARG_NONE,argtype.ARG_NONE,100213),
  new asminstdata("sub",0xd6,Proc.PROC_Z80,0,argtype.ARG_REG_A,argtype.ARG_IMM8,argtype.ARG_NONE,100214),
  new asminstdata("rst 10h",0xd7,Proc.PROC_Z80,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,100215),
  new asminstdata("ret c",0xd8,Proc.PROC_Z80,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,100216),
  new asminstdata("exx",0xd9,Proc.PROC_Z80,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,100217),
  new asminstdata("jp c",0xda,Proc.PROC_Z80,flags.FLAGS_CJMP,argtype.ARG_IMM16,argtype.ARG_NONE,argtype.ARG_NONE,100218),
  new asminstdata("in",0xdb,Proc.PROC_Z80,0,argtype.ARG_REG_A,argtype.ARG_IMM8_IND,argtype.ARG_NONE,100219),
  new asminstdata("call c",0xdc,Proc.PROC_Z80,flags.FLAGS_CALL,argtype.ARG_IMM16,argtype.ARG_NONE,argtype.ARG_NONE,100220),
  new asminstdata(null,0xdd,Proc.PROC_Z80,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,100221), // subtable 0xdd
  new asminstdata("sbc",0xde,Proc.PROC_Z80,0,argtype.ARG_REG_A,argtype.ARG_IMM8,argtype.ARG_NONE,100222),
  new asminstdata("rst 18h",0xdf,Proc.PROC_Z80,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,100223),
  new asminstdata("ret po",0xe0,Proc.PROC_Z80,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,100224),
  new asminstdata("pop",0xe1,Proc.PROC_Z80,0,argtype.ARG_REG_HL,argtype.ARG_NONE,argtype.ARG_NONE,100225),
  new asminstdata("jp po",0xe2,Proc.PROC_Z80,flags.FLAGS_CJMP,argtype.ARG_IMM16,argtype.ARG_NONE,argtype.ARG_NONE,100226),
  new asminstdata("ex",0xe3,Proc.PROC_Z80,0,argtype.ARG_REG_SP_IND,argtype.ARG_REG_HL,argtype.ARG_NONE,100227),
  new asminstdata("call po",0xe4,Proc.PROC_Z80,flags.FLAGS_CALL,argtype.ARG_IMM16,argtype.ARG_NONE,argtype.ARG_NONE,100228),
  new asminstdata("push",0xe5,Proc.PROC_Z80,0,argtype.ARG_REG_HL,argtype.ARG_NONE,argtype.ARG_NONE,100229),
  new asminstdata("and",0xe6,Proc.PROC_Z80,0,argtype.ARG_REG_A,argtype.ARG_IMM8,argtype.ARG_NONE,100230),
  new asminstdata("rst 20h",0xe7,Proc.PROC_Z80,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,100231),
  new asminstdata("ret pe",0xe8,Proc.PROC_Z80,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,100232),
  new asminstdata("jp",0xe9,Proc.PROC_Z80,flags.FLAGS_IJMP,argtype.ARG_REG_HL_IND,argtype.ARG_NONE,argtype.ARG_NONE,100233),
  new asminstdata("jp pe",0xea,Proc.PROC_Z80,flags.FLAGS_CJMP,argtype.ARG_IMM16,argtype.ARG_NONE,argtype.ARG_NONE,100234),
  new asminstdata("ex",0xeb,Proc.PROC_Z80,0,argtype.ARG_REG_DE,argtype.ARG_REG_HL,argtype.ARG_NONE,100235),
  new asminstdata("call pe",0xec,Proc.PROC_Z80,flags.FLAGS_CALL,argtype.ARG_IMM16,argtype.ARG_NONE,argtype.ARG_NONE,100236),
  new asminstdata(null,0xed,Proc.PROC_Z80,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,100237), // subtable 0xed
  new asminstdata("xor",0xee,Proc.PROC_Z80,0,argtype.ARG_REG_A,argtype.ARG_IMM8,argtype.ARG_NONE,100238),
  new asminstdata("rst 28h",0xef,Proc.PROC_Z80,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,100239),
  new asminstdata("ret p",0xf0,Proc.PROC_Z80,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,100240),
  new asminstdata("pop",0xf1,Proc.PROC_Z80,0,argtype.ARG_REG_AF,argtype.ARG_NONE,argtype.ARG_NONE,100241),
  new asminstdata("jp p",0xf2,Proc.PROC_Z80,flags.FLAGS_CJMP,argtype.ARG_IMM16,argtype.ARG_NONE,argtype.ARG_NONE,100242),
  new asminstdata("di",0xf3,Proc.PROC_Z80,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,100243),
  new asminstdata("call p",0xf4,Proc.PROC_Z80,flags.FLAGS_CALL,argtype.ARG_IMM16,argtype.ARG_NONE,argtype.ARG_NONE,100244),
  new asminstdata("push",0xf5,Proc.PROC_Z80,0,argtype.ARG_REG_AF,argtype.ARG_NONE,argtype.ARG_NONE,100245),
  new asminstdata("or",0xf6,Proc.PROC_Z80,0,argtype.ARG_REG_A,argtype.ARG_IMM8,argtype.ARG_NONE,100246),
  new asminstdata("rst 30h",0xf7,Proc.PROC_Z80,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,100247),
  new asminstdata("ret m",0xf8,Proc.PROC_Z80,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,100248),
  new asminstdata("ld",0xf9,Proc.PROC_Z80,0,argtype.ARG_REG_SP,argtype.ARG_REG_HL,argtype.ARG_NONE,100249),
  new asminstdata("jp m",0xfa,Proc.PROC_Z80,flags.FLAGS_CJMP,argtype.ARG_IMM16,argtype.ARG_NONE,argtype.ARG_NONE,100250),
  new asminstdata("ei",0xfb,Proc.PROC_Z80,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,100251),
  new asminstdata("call m",0xfc,Proc.PROC_Z80,flags.FLAGS_CALL,argtype.ARG_IMM16,argtype.ARG_NONE,argtype.ARG_NONE,100252),
  new asminstdata(null,0xfd,Proc.PROC_Z80,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,100253), // subtable 0xfd
  new asminstdata("cp",0xfe,Proc.PROC_Z80,0,argtype.ARG_REG_A,argtype.ARG_IMM8,argtype.ARG_NONE,100254),
  new asminstdata("rst 38h",0xff,Proc.PROC_Z80,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,100255),
  new asminstdata(null,0x00,0,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,0)  //endCode marker - processor=0 & opcode=0
};

        /* z80 subtable 0xcb
        // - num = second byte/8
        // - reg = second byte&7
        asminstdata asmz80subcba[]=
        { {"rlc",0x0,Proc.PROC_Z80,0,argtype.ARG_REG,argtype.ARG_NONE,argtype.ARG_NONE,101000),
          {"rrc",0x1,Proc.PROC_Z80,0,argtype.ARG_REG,argtype.ARG_NONE,argtype.ARG_NONE,101001),
          {"rl",0x2,Proc.PROC_Z80,0,argtype.ARG_REG,argtype.ARG_NONE,argtype.ARG_NONE,101002),
          {"rr",0x3,Proc.PROC_Z80,0,argtype.ARG_REG,argtype.ARG_NONE,argtype.ARG_NONE,101003),
          {"sla",0x4,Proc.PROC_Z80,0,argtype.ARG_REG,argtype.ARG_NONE,argtype.ARG_NONE,101004),
          {"sra",0x5,Proc.PROC_Z80,0,argtype.ARG_REG,argtype.ARG_NONE,argtype.ARG_NONE,101005),
          {"srl",0x7,Proc.PROC_Z80,0,argtype.ARG_REG,argtype.ARG_NONE,argtype.ARG_NONE,101006),
          new asminstdata(null,0x00,0,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,0)  //endCode marker - processor=0 & opcode=0
        };

        // z80 subtable 0xcb part2
        // - num = second byte bits 7,6
        // - bit = second byte bits 5,4,3
        // - reg = second byte bits 2,1,0
        asminstdata asmz80subcbb[]=
        { {"bit",0x1,Proc.PROC_Z80,0,argtype.ARG_BIT,argtype.ARG_REG,argtype.ARG_NONE,102000),
          {"res",0x2,Proc.PROC_Z80,0,argtype.ARG_BIT,argtype.ARG_REG,argtype.ARG_NONE,102001),
          {"set",0x3,Proc.PROC_Z80,0,argtype.ARG_BIT,argtype.ARG_REG,argtype.ARG_NONE,102002),
          new asminstdata(null,0x00,0,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,0)  //endCode marker - processor=0 & opcode=0
        };

        // subtable 0xdd
        asminstdata asmz80subdd[]=
        { {"add",0x09,Proc.PROC_Z80,0,argtype.ARG_REG_IX,argtype.ARG_REG_BC,argtype.ARG_NONE,103000),
          {"add",0x19,Proc.PROC_Z80,0,argtype.ARG_REG_IX,argtype.ARG_REG_DE,argtype.ARG_NONE,103001),
          {"ld",0x21,Proc.PROC_Z80,0,argtype.ARG_REG_IX,argtype.ARG_IMM16,argtype.ARG_NONE,103002),
          {"ld",0x22,Proc.PROC_Z80,0,argtype.ARG_MEMLOC16,argtype.ARG_REG_IX,argtype.ARG_NONE,103003),
          {"inc",0x23,Proc.PROC_Z80,0,argtype.ARG_REG_IX,argtype.ARG_NONE,argtype.ARG_NONE,103004),
          {"add",0x29,Proc.PROC_Z80,0,argtype.ARG_REG_IX,argtype.ARG_REG_IX,argtype.ARG_NONE,103005),
          {"ld",0x2a,Proc.PROC_Z80,0,argtype.ARG_REG_IX,argtype.ARG_MEMLOC16,argtype.ARG_NONE,103006),
          {"dec",0x2b,Proc.PROC_Z80,0,argtype.ARG_REG_IX,argtype.ARG_NONE,argtype.ARG_NONE,103007),
          {"inc",0x34,Proc.PROC_Z80,Flags.FLAGS_INDEXREG,argtype.ARG_REG_IX_IND,argtype.ARG_NONE,argtype.ARG_NONE,103008),
          {"dec",0x35,Proc.PROC_Z80,Flags.FLAGS_INDEXREG,argtype.ARG_REG_IX_IND,argtype.ARG_NONE,argtype.ARG_NONE,103009),
          {"ld",0x36,Proc.PROC_Z80,Flags.FLAGS_INDEXREG,argtype.ARG_REG_IX_IND,argtype.ARG_IMM8,argtype.ARG_NONE,103010),
          {"add",0x39,Proc.PROC_Z80,0,argtype.ARG_REG_IX,argtype.ARG_REG_SP,argtype.ARG_NONE,103011),
          {"ld",0x46,Proc.PROC_Z80,Flags.FLAGS_INDEXREG,argtype.ARG_REG_B,argtype.ARG_REG_IX_IND,argtype.ARG_NONE,103012),
          {"ld",0x4e,Proc.PROC_Z80,Flags.FLAGS_INDEXREG,argtype.ARG_REG_C,argtype.ARG_REG_IX_IND,argtype.ARG_NONE,103013),
          {"ld",0x56,Proc.PROC_Z80,Flags.FLAGS_INDEXREG,argtype.ARG_REG_D,argtype.ARG_REG_IX_IND,argtype.ARG_NONE,103014),
          {"ld",0x5e,Proc.PROC_Z80,Flags.FLAGS_INDEXREG,argtype.ARG_REG_E,argtype.ARG_REG_IX_IND,argtype.ARG_NONE,103015),
          {"ld",0x66,Proc.PROC_Z80,Flags.FLAGS_INDEXREG,argtype.ARG_REG_H,argtype.ARG_REG_IX_IND,argtype.ARG_NONE,103016),
          {"ld",0x6e,Proc.PROC_Z80,Flags.FLAGS_INDEXREG,argtype.ARG_REG_L,argtype.ARG_REG_IX_IND,argtype.ARG_NONE,103017),
          {"ld",0x70,Proc.PROC_Z80,Flags.FLAGS_INDEXREG,argtype.ARG_REG_IX_IND,argtype.ARG_REG_B,argtype.ARG_NONE,103018),
          {"ld",0x71,Proc.PROC_Z80,Flags.FLAGS_INDEXREG,argtype.ARG_REG_IX_IND,argtype.ARG_REG_C,argtype.ARG_NONE,103019),
          {"ld",0x72,Proc.PROC_Z80,Flags.FLAGS_INDEXREG,argtype.ARG_REG_IX_IND,argtype.ARG_REG_D,argtype.ARG_NONE,103020),
          {"ld",0x73,Proc.PROC_Z80,Flags.FLAGS_INDEXREG,argtype.ARG_REG_IX_IND,argtype.ARG_REG_E,argtype.ARG_NONE,103021),
          {"ld",0x74,Proc.PROC_Z80,Flags.FLAGS_INDEXREG,argtype.ARG_REG_IX_IND,argtype.ARG_REG_H,argtype.ARG_NONE,103022),
          {"ld",0x75,Proc.PROC_Z80,Flags.FLAGS_INDEXREG,argtype.ARG_REG_IX_IND,argtype.ARG_REG_L,argtype.ARG_NONE,103023),
          {"ld",0x77,Proc.PROC_Z80,Flags.FLAGS_INDEXREG,argtype.ARG_REG_IX_IND,argtype.ARG_REG_A,argtype.ARG_NONE,103024),
          {"ld",0x7e,Proc.PROC_Z80,Flags.FLAGS_INDEXREG,argtype.ARG_REG_A,argtype.ARG_REG_IX_IND,argtype.ARG_NONE,103025),
          {"add",0x86,Proc.PROC_Z80,Flags.FLAGS_INDEXREG,argtype.ARG_REG_A,argtype.ARG_REG_IX_IND,argtype.ARG_NONE,103026),
          {"adc",0x8e,Proc.PROC_Z80,Flags.FLAGS_INDEXREG,argtype.ARG_REG_A,argtype.ARG_REG_IX_IND,argtype.ARG_NONE,103027),
          {"sub",0x96,Proc.PROC_Z80,Flags.FLAGS_INDEXREG,argtype.ARG_REG_A,argtype.ARG_REG_IX_IND,argtype.ARG_NONE,103028),
          {"sbc",0x9e,Proc.PROC_Z80,Flags.FLAGS_INDEXREG,argtype.ARG_REG_A,argtype.ARG_REG_IX_IND,argtype.ARG_NONE,103029),
          {"and",0xa6,Proc.PROC_Z80,Flags.FLAGS_INDEXREG,argtype.ARG_REG_A,argtype.ARG_REG_IX_IND,argtype.ARG_NONE,103030),
          {"xor",0xae,Proc.PROC_Z80,Flags.FLAGS_INDEXREG,argtype.ARG_REG_A,argtype.ARG_REG_IX_IND,argtype.ARG_NONE,103031),
          {"or",0xb6,Proc.PROC_Z80,Flags.FLAGS_INDEXREG,argtype.ARG_REG_A,argtype.ARG_REG_IX_IND,argtype.ARG_NONE,103032),
          {"cp",0xbe,Proc.PROC_Z80,Flags.FLAGS_INDEXREG,argtype.ARG_REG_A,argtype.ARG_REG_IX_IND,argtype.ARG_NONE,103033),
          new asminstdata(null,0xcb,Proc.PROC_Z80,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,103034), // subtable 0xdd/0xcb
          {"pop",0xe1,Proc.PROC_Z80,0,argtype.ARG_REG_IX,argtype.ARG_NONE,argtype.ARG_NONE,103035),
          {"ex",0xe3,Proc.PROC_Z80,0,argtype.ARG_REG_SP_IND,argtype.ARG_REG_IX,argtype.ARG_NONE,103036),
          {"push",0xe5,Proc.PROC_Z80,0,argtype.ARG_REG_IX,argtype.ARG_NONE,argtype.ARG_NONE,103037),
          {"jp",0xe9,Proc.PROC_Z80,Flags.FLAGS_IJMP,argtype.ARG_REG_IX_IND,argtype.ARG_NONE,argtype.ARG_NONE,103038),
          {"ld",0xf9,Proc.PROC_Z80,0,argtype.ARG_REG_SP,argtype.ARG_REG_IX,argtype.ARG_NONE,103039),
          new asminstdata(null,0x00,0,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,0)  //endCode marker - processor=0 & opcode=0
        };

        // z80 subtable 0xdd/0xcb
        // - num = fourth byte
        asminstdata asmz80subddcba[]=
        { {"rlc",0x06,Proc.PROC_Z80,Flags.FLAGS_INDEXREG,argtype.ARG_REG_IX_IND,argtype.ARG_NONE,argtype.ARG_NONE,104000),
          {"rrc",0x0e,Proc.PROC_Z80,Flags.FLAGS_INDEXREG,argtype.ARG_REG_IX_IND,argtype.ARG_NONE,argtype.ARG_NONE,104001),
          {"rl",0x16,Proc.PROC_Z80,Flags.FLAGS_INDEXREG,argtype.ARG_REG_IX_IND,argtype.ARG_NONE,argtype.ARG_NONE,104002),
          {"rr",0x1e,Proc.PROC_Z80,Flags.FLAGS_INDEXREG,argtype.ARG_REG_IX_IND,argtype.ARG_NONE,argtype.ARG_NONE,104003),
          {"sla",0x26,Proc.PROC_Z80,Flags.FLAGS_INDEXREG,argtype.ARG_REG_IX_IND,argtype.ARG_NONE,argtype.ARG_NONE,104004),
          {"sra",0x2e,Proc.PROC_Z80,Flags.FLAGS_INDEXREG,argtype.ARG_REG_IX_IND,argtype.ARG_NONE,argtype.ARG_NONE,104005),
          {"srl",0x3e,Proc.PROC_Z80,Flags.FLAGS_INDEXREG,argtype.ARG_REG_IX_IND,argtype.ARG_NONE,argtype.ARG_NONE,104006),
          new asminstdata(null,0x00,0,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,0)  //endCode marker - processor=0 & opcode=0
        };

        // z80 subtable 0xdd/0xcb part2
        // - num = second byte bits 7,6,2,1,0
        // - bit = second byte bits 5,4,3
        asminstdata asmz80subddcbb[]=
        { {"bit",0x46,Proc.PROC_Z80,Flags.FLAGS_INDEXREG,argtype.ARG_BIT,argtype.ARG_REG_IX_IND,argtype.ARG_NONE,105000),
          {"res",0x86,Proc.PROC_Z80,Flags.FLAGS_INDEXREG,argtype.ARG_BIT,argtype.ARG_REG_IX_IND,argtype.ARG_NONE,105001),
          {"set",0xc6,Proc.PROC_Z80,Flags.FLAGS_INDEXREG,argtype.ARG_BIT,argtype.ARG_REG_IX_IND,argtype.ARG_NONE,105002),
          new asminstdata(null,0x00,0,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,0)  //endCode marker - processor=0 & opcode=0
        };

        asminstdata asmz80subed[]=
        { {"in",0x40,Proc.PROC_Z80,0,argtype.ARG_REG_B,argtype.ARG_REG_C_IND,argtype.ARG_NONE,106000),
          {"out",0x41,Proc.PROC_Z80,0,argtype.ARG_REG_C_IND,argtype.ARG_REG_B,argtype.ARG_NONE,106001),
          {"sbc",0x42,Proc.PROC_Z80,0,argtype.ARG_REG_HL,argtype.ARG_REG_BC,argtype.ARG_NONE,106002),
          {"ld",0x43,Proc.PROC_Z80,0,argtype.ARG_MEMLOC16,argtype.ARG_REG_BC,argtype.ARG_NONE,106003),
          {"neg",0x44,Proc.PROC_Z80,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,106004),
          {"retn",0x45,Proc.PROC_Z80,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,106005),
          {"sectionHeader 0",0x46,Proc.PROC_Z80,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,106006),
          {"ld",0x47,Proc.PROC_Z80,0,argtype.ARG_REG_I,argtype.ARG_REG_A,argtype.ARG_NONE,106007),
          {"in",0x48,Proc.PROC_Z80,0,argtype.ARG_REG_C,argtype.ARG_REG_C_IND,argtype.ARG_NONE,106008),
          {"out",0x49,Proc.PROC_Z80,0,argtype.ARG_REG_C_IND,argtype.ARG_REG_C,argtype.ARG_NONE,106009),
          {"adc",0x4a,Proc.PROC_Z80,0,argtype.ARG_REG_HL,argtype.ARG_REG_BC,argtype.ARG_NONE,106010),
          {"ld",0x4b,Proc.PROC_Z80,0,argtype.ARG_REG_BC,argtype.ARG_MEMLOC16,argtype.ARG_NONE,106011),
          {"reti",0x4d,Proc.PROC_Z80,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,106012),
          {"ld",0x4f,Proc.PROC_Z80,0,argtype.ARG_REG_R,argtype.ARG_REG_A,argtype.ARG_NONE,106013),
          {"in",0x50,Proc.PROC_Z80,0,argtype.ARG_REG_D,argtype.ARG_REG_C_IND,argtype.ARG_NONE,106014),
          {"out",0x51,Proc.PROC_Z80,0,argtype.ARG_REG_C_IND,argtype.ARG_REG_D,argtype.ARG_NONE,106015),
          {"sbc",0x52,Proc.PROC_Z80,0,argtype.ARG_REG_HL,argtype.ARG_REG_DE,argtype.ARG_NONE,106016),
          {"ld",0x53,Proc.PROC_Z80,0,argtype.ARG_MEMLOC16,argtype.ARG_REG_DE,argtype.ARG_NONE,106017),
          {"sectionHeader 1",0x56,Proc.PROC_Z80,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,106018),
          {"ld",0x57,Proc.PROC_Z80,0,argtype.ARG_REG_A,argtype.ARG_REG_I,argtype.ARG_NONE,106019),
          {"in",0x58,Proc.PROC_Z80,0,argtype.ARG_REG_E,argtype.ARG_REG_C_IND,argtype.ARG_NONE,106020),
          {"out",0x59,Proc.PROC_Z80,0,argtype.ARG_REG_C_IND,argtype.ARG_REG_E,argtype.ARG_NONE,106021),
          {"adc",0x5a,Proc.PROC_Z80,0,argtype.ARG_REG_HL,argtype.ARG_REG_DE,argtype.ARG_NONE,106022),
          {"ld",0x5b,Proc.PROC_Z80,0,argtype.ARG_REG_DE,argtype.ARG_MEMLOC16,argtype.ARG_NONE,106023),
          {"sectionHeader 2",0x5e,Proc.PROC_Z80,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,106024),
          {"ld",0x5f,Proc.PROC_Z80,0,argtype.ARG_REG_A,argtype.ARG_REG_R,argtype.ARG_NONE,106025),
          {"in",0x60,Proc.PROC_Z80,0,argtype.ARG_REG_H,argtype.ARG_REG_C_IND,argtype.ARG_NONE,106026),
          {"out",0x61,Proc.PROC_Z80,0,argtype.ARG_REG_C_IND,argtype.ARG_REG_H,argtype.ARG_NONE,106027),
          {"sbc",0x62,Proc.PROC_Z80,0,argtype.ARG_REG_HL,argtype.ARG_REG_HL,argtype.ARG_NONE,106028),
          {"ld",0x63,Proc.PROC_Z80,0,argtype.ARG_MEMLOC16,argtype.ARG_REG_HL,argtype.ARG_NONE,106029),
          {"rrd",0x67,Proc.PROC_Z80,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,106030),
          {"in",0x68,Proc.PROC_Z80,0,argtype.ARG_REG_L,argtype.ARG_REG_C_IND,argtype.ARG_NONE,106031),
          {"out",0x69,Proc.PROC_Z80,0,argtype.ARG_REG_C_IND,argtype.ARG_REG_L,argtype.ARG_NONE,106032),
          {"adc",0x6a,Proc.PROC_Z80,0,argtype.ARG_REG_HL,argtype.ARG_REG_HL,argtype.ARG_NONE,106033),
          {"ld",0x6b,Proc.PROC_Z80,0,argtype.ARG_REG_HL,argtype.ARG_MEMLOC16,argtype.ARG_NONE,106034),
          {"rld",0x6f,Proc.PROC_Z80,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,106035),
          {"in",0x70,Proc.PROC_Z80,0,argtype.ARG_REG_HL_IND,argtype.ARG_REG_C_IND,argtype.ARG_NONE,106036),
          {"out",0x71,Proc.PROC_Z80,0,argtype.ARG_REG_C_IND,argtype.ARG_REG_HL_IND,argtype.ARG_NONE,106037),
          {"sbc",0x72,Proc.PROC_Z80,0,argtype.ARG_REG_HL,argtype.ARG_REG_SP,argtype.ARG_NONE,106038),
          {"ld",0x73,Proc.PROC_Z80,0,argtype.ARG_MEMLOC16,argtype.ARG_REG_SP,argtype.ARG_NONE,106039),
          {"in",0x78,Proc.PROC_Z80,0,argtype.ARG_REG_A,argtype.ARG_REG_C_IND,argtype.ARG_NONE,106040),
          {"out",0x79,Proc.PROC_Z80,0,argtype.ARG_REG_C_IND,argtype.ARG_REG_A,argtype.ARG_NONE,106041),
          {"adc",0x7a,Proc.PROC_Z80,0,argtype.ARG_REG_HL,argtype.ARG_REG_SP,argtype.ARG_NONE,106042),
          {"ld",0x7b,Proc.PROC_Z80,0,argtype.ARG_REG_SP,argtype.ARG_MEMLOC16,argtype.ARG_NONE,106043),
          {"ldi",0xa0,Proc.PROC_Z80,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,106044),
          {"cpi",0xa1,Proc.PROC_Z80,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,106045),
          {"ini",0xa2,Proc.PROC_Z80,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,106046),
          {"outi",0xa3,Proc.PROC_Z80,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,106047),
          {"ldd",0xa8,Proc.PROC_Z80,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,106048),
          {"cpd",0xa9,Proc.PROC_Z80,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,106049),
          {"ind",0xaa,Proc.PROC_Z80,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,106050),
          {"outd",0xab,Proc.PROC_Z80,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,106051),
          {"ldir",0xb0,Proc.PROC_Z80,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,106052),
          {"cpir",0xb1,Proc.PROC_Z80,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,106053),
          {"inir",0xb2,Proc.PROC_Z80,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,106054),
          {"outir",0xb3,Proc.PROC_Z80,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,106055),
          {"lddr",0xb8,Proc.PROC_Z80,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,106056),
          {"cpdr",0xb9,Proc.PROC_Z80,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,106057),
          {"indr",0xba,Proc.PROC_Z80,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,106058),
          {"otdr",0xbb,Proc.PROC_Z80,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,106059),
          new asminstdata(null,0x00,0,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,0)  //endCode marker - processor=0 & opcode=0
        };

        asminstdata asmz80subfd[]=
        { {"add",0x09,Proc.PROC_Z80,0,argtype.ARG_REG_IY,argtype.ARG_REG_BC,argtype.ARG_NONE,107000),
          {"add",0x19,Proc.PROC_Z80,0,argtype.ARG_REG_IY,argtype.ARG_REG_DE,argtype.ARG_NONE,107001),
          {"ld",0x21,Proc.PROC_Z80,0,argtype.ARG_REG_IY,argtype.ARG_IMM16,argtype.ARG_NONE,107002),
          {"ld",0x22,Proc.PROC_Z80,0,argtype.ARG_MEMLOC16,argtype.ARG_REG_IY,argtype.ARG_NONE,107003),
          {"inc",0x23,Proc.PROC_Z80,0,argtype.ARG_REG_IY,argtype.ARG_NONE,argtype.ARG_NONE,107004),
          {"add",0x29,Proc.PROC_Z80,0,argtype.ARG_REG_IY,argtype.ARG_REG_IX,argtype.ARG_NONE,107005),
          {"ld",0x2a,Proc.PROC_Z80,0,argtype.ARG_REG_IY,argtype.ARG_MEMLOC16,argtype.ARG_NONE,107006),
          {"dec",0x2b,Proc.PROC_Z80,0,argtype.ARG_REG_IY,argtype.ARG_NONE,argtype.ARG_NONE,107007),
          {"inc",0x34,Proc.PROC_Z80,Flags.FLAGS_INDEXREG,argtype.ARG_REG_IY_IND,argtype.ARG_NONE,argtype.ARG_NONE,107008),
          {"dec",0x35,Proc.PROC_Z80,Flags.FLAGS_INDEXREG,argtype.ARG_REG_IY_IND,argtype.ARG_NONE,argtype.ARG_NONE,107009),
          {"ld",0x36,Proc.PROC_Z80,Flags.FLAGS_INDEXREG,argtype.ARG_REG_IY_IND,argtype.ARG_IMM8,argtype.ARG_NONE,107010),
          {"add",0x39,Proc.PROC_Z80,0,argtype.ARG_REG_IY,argtype.ARG_REG_SP,argtype.ARG_NONE,107011),
          {"ld",0x46,Proc.PROC_Z80,Flags.FLAGS_INDEXREG,argtype.ARG_REG_B,argtype.ARG_REG_IY_IND,argtype.ARG_NONE,107012),
          {"ld",0x4e,Proc.PROC_Z80,Flags.FLAGS_INDEXREG,argtype.ARG_REG_C,argtype.ARG_REG_IY_IND,argtype.ARG_NONE,107013),
          {"ld",0x56,Proc.PROC_Z80,Flags.FLAGS_INDEXREG,argtype.ARG_REG_D,argtype.ARG_REG_IY_IND,argtype.ARG_NONE,107014),
          {"ld",0x5e,Proc.PROC_Z80,Flags.FLAGS_INDEXREG,argtype.ARG_REG_E,argtype.ARG_REG_IY_IND,argtype.ARG_NONE,107015),
          {"ld",0x66,Proc.PROC_Z80,Flags.FLAGS_INDEXREG,argtype.ARG_REG_H,argtype.ARG_REG_IY_IND,argtype.ARG_NONE,107016),
          {"ld",0x6e,Proc.PROC_Z80,Flags.FLAGS_INDEXREG,argtype.ARG_REG_L,argtype.ARG_REG_IY_IND,argtype.ARG_NONE,107017),
          {"ld",0x70,Proc.PROC_Z80,Flags.FLAGS_INDEXREG,argtype.ARG_REG_IY_IND,argtype.ARG_REG_B,argtype.ARG_NONE,107018),
          {"ld",0x71,Proc.PROC_Z80,Flags.FLAGS_INDEXREG,argtype.ARG_REG_IY_IND,argtype.ARG_REG_C,argtype.ARG_NONE,107019),
          {"ld",0x72,Proc.PROC_Z80,Flags.FLAGS_INDEXREG,argtype.ARG_REG_IY_IND,argtype.ARG_REG_D,argtype.ARG_NONE,107020),
          {"ld",0x73,Proc.PROC_Z80,Flags.FLAGS_INDEXREG,argtype.ARG_REG_IY_IND,argtype.ARG_REG_E,argtype.ARG_NONE,107021),
          {"ld",0x74,Proc.PROC_Z80,Flags.FLAGS_INDEXREG,argtype.ARG_REG_IY_IND,argtype.ARG_REG_H,argtype.ARG_NONE,107022),
          {"ld",0x75,Proc.PROC_Z80,Flags.FLAGS_INDEXREG,argtype.ARG_REG_IY_IND,argtype.ARG_REG_L,argtype.ARG_NONE,107023),
          {"ld",0x77,Proc.PROC_Z80,Flags.FLAGS_INDEXREG,argtype.ARG_REG_IY_IND,argtype.ARG_REG_A,argtype.ARG_NONE,107024),
          {"ld",0x7e,Proc.PROC_Z80,Flags.FLAGS_INDEXREG,argtype.ARG_REG_A,argtype.ARG_REG_IY_IND,argtype.ARG_NONE,107025),
          {"add",0x86,Proc.PROC_Z80,Flags.FLAGS_INDEXREG,argtype.ARG_REG_A,argtype.ARG_REG_IY_IND,argtype.ARG_NONE,107026),
          {"adc",0x8e,Proc.PROC_Z80,Flags.FLAGS_INDEXREG,argtype.ARG_REG_A,argtype.ARG_REG_IY_IND,argtype.ARG_NONE,107027),
          {"sub",0x96,Proc.PROC_Z80,Flags.FLAGS_INDEXREG,argtype.ARG_REG_A,argtype.ARG_REG_IY_IND,argtype.ARG_NONE,107028),
          {"sbc",0x9e,Proc.PROC_Z80,Flags.FLAGS_INDEXREG,argtype.ARG_REG_A,argtype.ARG_REG_IY_IND,argtype.ARG_NONE,107029),
          {"and",0xa6,Proc.PROC_Z80,Flags.FLAGS_INDEXREG,argtype.ARG_REG_A,argtype.ARG_REG_IY_IND,argtype.ARG_NONE,107030),
          {"xor",0xae,Proc.PROC_Z80,Flags.FLAGS_INDEXREG,argtype.ARG_REG_A,argtype.ARG_REG_IY_IND,argtype.ARG_NONE,107031),
          {"or",0xb6,Proc.PROC_Z80,Flags.FLAGS_INDEXREG,argtype.ARG_REG_A,argtype.ARG_REG_IY_IND,argtype.ARG_NONE,107032),
          {"cp",0xbe,Proc.PROC_Z80,Flags.FLAGS_INDEXREG,argtype.ARG_REG_A,argtype.ARG_REG_IY_IND,argtype.ARG_NONE,107033),
          new asminstdata(null,0xcb,Proc.PROC_Z80,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,107034), // subtable 0xfd/0xcb
          {"pop",0xe1,Proc.PROC_Z80,0,argtype.ARG_REG_IY,argtype.ARG_NONE,argtype.ARG_NONE,107035),
          {"ex",0xe3,Proc.PROC_Z80,0,argtype.ARG_REG_SP_IND,argtype.ARG_REG_IY,argtype.ARG_NONE,107036),
          {"push",0xe5,Proc.PROC_Z80,0,argtype.ARG_REG_IY,argtype.ARG_NONE,argtype.ARG_NONE,107037),
          {"jp",0xe9,Proc.PROC_Z80,Flags.FLAGS_IJMP,argtype.ARG_REG_IY_IND,argtype.ARG_NONE,argtype.ARG_NONE,107038),
          {"ld",0xf9,Proc.PROC_Z80,0,argtype.ARG_REG_SP,argtype.ARG_REG_IY,argtype.ARG_NONE,107039),
          new asminstdata(null,0x00,0,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,0)  //endCode marker - processor=0 & opcode=0
        };

        // z80 subtable 0xfd/0xcb
        // - num = fourth byte
        asminstdata asmz80subfdcba[]=
        { {"rlc",0x06,Proc.PROC_Z80,Flags.FLAGS_INDEXREG,argtype.ARG_REG_IY_IND,argtype.ARG_NONE,argtype.ARG_NONE,108000),
          {"rrc",0x0e,Proc.PROC_Z80,Flags.FLAGS_INDEXREG,argtype.ARG_REG_IY_IND,argtype.ARG_NONE,argtype.ARG_NONE,108001),
          {"rl",0x16,Proc.PROC_Z80,Flags.FLAGS_INDEXREG,argtype.ARG_REG_IY_IND,argtype.ARG_NONE,argtype.ARG_NONE,108002),
          {"rr",0x1e,Proc.PROC_Z80,Flags.FLAGS_INDEXREG,argtype.ARG_REG_IY_IND,argtype.ARG_NONE,argtype.ARG_NONE,108003),
          {"sla",0x26,Proc.PROC_Z80,Flags.FLAGS_INDEXREG,argtype.ARG_REG_IY_IND,argtype.ARG_NONE,argtype.ARG_NONE,108004),
          {"sra",0x2e,Proc.PROC_Z80,Flags.FLAGS_INDEXREG,argtype.ARG_REG_IY_IND,argtype.ARG_NONE,argtype.ARG_NONE,108005),
          {"srl",0x3e,Proc.PROC_Z80,Flags.FLAGS_INDEXREG,argtype.ARG_REG_IY_IND,argtype.ARG_NONE,argtype.ARG_NONE,108006),
          new asminstdata(null,0x00,0,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,0)  //endCode marker - processor=0 & opcode=0
        };

        // z80 subtable 0xfd/0xcb part2
        // - num = second byte bits 7,6,2,1,0
        // - bit = second byte bits 5,4,3
        asminstdata asmz80subfdcbb[]=
        { {"bit",0x46,Proc.PROC_Z80,Flags.FLAGS_INDEXREG,argtype.ARG_BIT,argtype.ARG_REG_IY_IND,argtype.ARG_NONE,109000),
          {"res",0x86,Proc.PROC_Z80,Flags.FLAGS_INDEXREG,argtype.ARG_BIT,argtype.ARG_REG_IY_IND,argtype.ARG_NONE,109001),
          {"set",0xc6,Proc.PROC_Z80,Flags.FLAGS_INDEXREG,argtype.ARG_BIT,argtype.ARG_REG_IY_IND,argtype.ARG_NONE,109002),
          new asminstdata(null,0x00,0,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,0)  //endCode marker - processor=0 & opcode=0
        };
        */
        public asminstdata[] asmint = new asminstdata[]
{ new asminstdata("dd",0x00,Proc.PROC_ALL,0,argtype.ARG_IMM32,argtype.ARG_NONE,argtype.ARG_NONE,200000),
  new asminstdata(null,0x00,0,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,0)  //endCode marker - processor=0 & opcode=0
};

        public asminstdata[] asmshort = new asminstdata[]
{new asminstdata("dw",0x00,Proc.PROC_ALL,0,argtype.ARG_IMM16,argtype.ARG_NONE,argtype.ARG_NONE,201000),
  new asminstdata(null,0x00,0,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,0)  //endCode marker - processor=0 & opcode=0
};

        public asminstdata[] asmstr = new asminstdata[]
{ new asminstdata("db",0x00,Proc.PROC_ALL,0,argtype.ARG_STRING,argtype.ARG_NONE,argtype.ARG_NONE,202000),
  new asminstdata("db",0x00,Proc.PROC_ALL,0,argtype.ARG_PSTRING,argtype.ARG_NONE,argtype.ARG_NONE,202001),
  new asminstdata("db",0x00,Proc.PROC_ALL,0,argtype.ARG_DOSSTRING,argtype.ARG_NONE,argtype.ARG_NONE,202002),
  new asminstdata("db",0x00,Proc.PROC_ALL,0,argtype.ARG_CUNICODESTRING,argtype.ARG_NONE,argtype.ARG_NONE,202003),
  new asminstdata("db",0x00,Proc.PROC_ALL,0,argtype.ARG_PUNICODESTRING,argtype.ARG_NONE,argtype.ARG_NONE,202004),
  new asminstdata(null,0x00,0,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,0)  //endCode marker - processor=0 & opcode=0
};

        public asminstdata[] asm_fp = new asminstdata[]
{ new asminstdata("int",0x00,Proc.PROC_ALL,0,argtype.ARG_IMM_SINGLE,argtype.ARG_NONE,argtype.ARG_NONE,203000),
  new asminstdata("qshort",0x00,Proc.PROC_ALL,0,argtype.ARG_IMM_DOUBLE,argtype.ARG_NONE,argtype.ARG_NONE,203001),
  new asminstdata("tbyte",0x00,Proc.PROC_ALL,0,argtype.ARG_IMM_LONGDOUBLE,argtype.ARG_NONE,argtype.ARG_NONE,203002),
  new asminstdata(null,0x00,0,0,argtype.ARG_NONE,argtype.ARG_NONE,argtype.ARG_NONE,0)  //endCode marker - processor=0 & opcode=0
};
        static byte TABLE_MAIN = 1;
        static byte TABLE_EXT = 2;
        static byte TABLE_EXT2 = 3;

        public static asmtable[] tables86 = new asmtable[]{ new asmtable(asm86,TABLE_MAIN,0,0,0,0xff,0,0xff,0),
  new asmtable(asm86sub0f,TABLE_EXT,0x0f,0,0,0xff,0,0xff,1),
  new asmtable(asm86sub80,TABLE_EXT,0x80,0,8,0x07,0,0xff,0),
  new asmtable(asm86sub81,TABLE_EXT,0x81,0,8,0x07,0,0xff,0),
  new asmtable(asm86sub82,TABLE_EXT,0x82,0,8,0x07,0,0xff,0),
  new asmtable(asm86sub83,TABLE_EXT,0x83,0,8,0x07,0,0xff,0),
  new asmtable(asm86subc0,TABLE_EXT,0xc0,0,8,0x07,0,0xff,0),
  new asmtable(asm86subc1,TABLE_EXT,0xc1,0,8,0x07,0,0xff,0),
  new asmtable(asm86subd0,TABLE_EXT,0xd0,0,8,0x07,0,0xff,0),
  new asmtable(asm86subd1,TABLE_EXT,0xd1,0,8,0x07,0,0xff,0),
  new asmtable(asm86subd2,TABLE_EXT,0xd2,0,8,0x07,0,0xff,0),
  new asmtable(asm86subd3,TABLE_EXT,0xd3,0,8,0x07,0,0xff,0),
  new asmtable(asm86subf6,TABLE_EXT,0xf6,0,8,0x07,0,0xff,0),
  new asmtable(asm86subf7,TABLE_EXT,0xf7,0,8,0x07,0,0xff,0),
  new asmtable(asm86subfe,TABLE_EXT,0xfe,0,8,0x07,0,0xff,0),
  new asmtable(asm86subff,TABLE_EXT,0xff,0,8,0x07,0,0xff,0),
  new asmtable(asm86sub0f00,TABLE_EXT2,0x0f,0x00,8,0x07,0,0xff,1),
  new asmtable(asm86sub0f01,TABLE_EXT2,0x0f,0x01,8,0x07,0,0xff,1),
  new asmtable(asm86sub0f18,TABLE_EXT2,0x0f,0x18,8,0x07,0,0xff,1),
  new asmtable(asm86sub0f71,TABLE_EXT2,0x0f,0x71,8,0x07,0,0xff,1),
  new asmtable(asm86sub0f72,TABLE_EXT2,0x0f,0x72,8,0x07,0,0xff,1),
  new asmtable(asm86sub0f73,TABLE_EXT2,0x0f,0x73,8,0x07,0,0xff,1),
  new asmtable(asm86sub0fae,TABLE_EXT2,0x0f,0xae,8,0x07,0,0xff,1),
  new asmtable(asm86sub0fba,TABLE_EXT2,0x0f,0xba,8,0x07,0,0xff,1),
  new asmtable(asm86sub0fc2,TABLE_EXT2,0x0f,0xc2,0,0x00,0,0xff,1),
  new asmtable(asm86sub0fc7,TABLE_EXT2,0x0f,0xc7,8,0x07,0,0xff,1),
  new asmtable(asm86subd8a,TABLE_EXT,0xd8,0,8,0x07,0,0xbf,0),
  new asmtable(asm86subd8b,TABLE_EXT,0xd8,0,8,0x1f,0xc0,0xff,0),
  new asmtable(asm86subd9a,TABLE_EXT,0xd9,0,8,0x07,0,0xbf,0),
  new asmtable(asm86subd9b,TABLE_EXT,0xd9,0,8,0x1f,0xc0,0xff,0),
  new asmtable(asm86subd9c,TABLE_EXT,0xd9,0,1,0xff,0xc0,0xff,1),
  new asmtable(asm86subdaa,TABLE_EXT,0xda,0,8,0x07,0,0xbf,0),
  new asmtable(asm86subdab,TABLE_EXT,0xda,0,8,0x1f,0xc0,0xff,0),
  new asmtable(asm86subdac,TABLE_EXT,0xda,0,1,0xff,0xc0,0xff,1),
  new asmtable(asm86subdba,TABLE_EXT,0xdb,0,8,0x07,0,0xbf,0),
  new asmtable(asm86subdbb,TABLE_EXT,0xdb,0,8,0x1f,0xc0,0xff,0),
  new asmtable(asm86subdbc,TABLE_EXT,0xdb,0,1,0xff,0xc0,0xff,1),
  new asmtable(asm86subdca,TABLE_EXT,0xdc,0,8,0x07,0,0xbf,0),
  new asmtable(asm86subdcb,TABLE_EXT,0xdc,0,8,0x1f,0xc0,0xff,0),
  new asmtable(asm86subdda,TABLE_EXT,0xdd,0,8,0x07,0,0xbf,0),
  new asmtable(asm86subddb,TABLE_EXT,0xdd,0,8,0x1f,0xc0,0xff,0),
  new asmtable(asm86subdea,TABLE_EXT,0xde,0,8,0x07,0,0xbf,0),
  new asmtable(asm86subdeb,TABLE_EXT,0xde,0,8,0x1f,0xc0,0xff,0),
  new asmtable(asm86subdec,TABLE_EXT,0xde,0,1,0xff,0xc0,0xff,1),
  new asmtable(asm86subdfa,TABLE_EXT,0xdf,0,8,0x07,0,0xbf,0),
  new asmtable(asm86subdfb,TABLE_EXT,0xdf,0,8,0x1f,0xc0,0xff,0),
  new asmtable(asm86subdfc,TABLE_EXT,0xdf,0,1,0xff,0xc0,0xff,1),
  new asmtable(null,0,0,0,0,0,0,0,0)
        };
        /*
        asmtable tablesz80[]=
        { {asmz80,TABLE_MAIN,0,0,0,0xff,0,0xff,0),
          {asmz80subcba,TABLE_EXT,0xcb,0,8,0x1f,0,0x39,0),
          {asmz80subcbb,TABLE_EXT,0xcb,0,64,0x03,0x40,0xff,0),
          {asmz80subdd,TABLE_EXT,0xdd,0,0,0xff,0,0xff,0),
          {asmz80subddcba,TABLE_EXT2,0xdd,0xcb,0,0xff,0,0xff,0),
          {asmz80subddcbb,TABLE_EXT2,0xdd,0xcb,0,0xc7,0,0xff,0),
          {asmz80subed,TABLE_EXT,0xed,0,0,0xff,0,0xff,0),
          {asmz80subfd,TABLE_EXT,0xfd,0,0,0xff,0,0xff,0),
          {asmz80subfdcba,TABLE_EXT2,0xfd,0xcb,0,0xff,0,0xff,0),
          {asmz80subfdcbb,TABLE_EXT2,0xfd,0xcb,0,0xc7,0,0xff,0),
          new asminstdata(null,0,0,0,0,0,0,0,0)
        };

        asminstdata *reconstruct[]=
        { asm86,asm86sub0f,asm86sub80,asm86sub81,asm86sub82,asm86sub83,asm86subc0,asm86subc1,asm86subd0,
          asm86subd1,asm86subd2,asm86subd3,asm86subf6,asm86subf7,asm86subfe,asm86subff,asm86sub0f00,
          asm86sub0f01,asm86sub0f18,asm86sub0f71,asm86sub0f72,asm86sub0f73,asm86sub0fae,asm86sub0fba,
          asm86sub0fc2,asm86sub0fc7,asm86subd8a,asm86subd8b,asm86subd9a,asm86subd9b,asm86subd9c,
          asm86subdaa,asm86subdab,asm86subdac,asm86subdba,asm86subdbb,asm86subdbc,asm86subdca,asm86subdcb,
          asm86subdda,asm86subddb,asm86subdea,asm86subdeb,asm86subdec,asm86subdfa,asm86subdfb,asm86subdfc,
          asmz80,asmz80subcba,asmz80subcbb,asmz80subdd,asmz80subddcba,asmz80subddcbb,asmz80subed,asmz80subfd,
          asmz80subfdcba,asmz80subfdcbb,asmint,asmshort,asmstr,asm_fp,null
        };
        */
    }
}
