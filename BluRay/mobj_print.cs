using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BluRay
{
    public class mobj_print
    {
        #region Data
        static string[] psr_info = new string[]{
    "/*     PSR0:  Interactive graphics stream number */",
    "/*     PSR1:  Primary audio stream number */",
    "/*     PSR2:  PG TextST stream number and PiP PG stream number */",
    "/*     PSR3:  Angle number */",
    "/*     PSR4:  Title number */",
    "/*     PSR5:  Chapter number */",
    "/*     PSR6:  PlayList ID */",
    "/*     PSR7:  PlayItem ID */",
    "/*     PSR8:  Presentation time */",
    "/*     PSR9:  Navigation timer */",
    "/*     PSR10: Selected button ID */",
    "/*     PSR11: Page ID */",
    "/*     PSR12: User style number */",
    "/* RO: PSR13: User age */",
    "/*     PSR14: Secondary audio stream number and secondary video stream number */",
    "/* RO: PSR15: player capability for audio */",
    "/* RO: PSR16: Language code for audio */",
    "/* RO: PSR17: Language code for PG and Text subtitles */",
    "/* RO: PSR18: Menu description language code */",
    "/* RO: PSR19: Country code */",
    "/* RO: PSR20: Region code */ /* 1 - A, 2 - B, 4 - C */",
    "/*     PSR21 */",
    "/*     PSR22 */",
    "/*     PSR23 */",
    "/*     PSR24 */",
    "/*     PSR25 */",
    "/*     PSR26 */",
    "/*     PSR27 */",
    "/*     PSR28 */",
    "/* RO: PSR29: player capability for video */",
    "/* RO: PSR30: player capability for text subtitle */",
    "/* RO: PSR31: Player profile and version */",
    "/*     PSR32 */",
    "/*     PSR33 */",
    "/*     PSR34 */",
    "/*     PSR35 */",
    "/*     PSR36: backup PSR4 */",
    "/*     PSR37: backup PSR5 */",
    "/*     PSR38: backup PSR6 */",
    "/*     PSR39: backup PSR7 */",
    "/*     PSR40: backup PSR8 */",
    "/*     PSR41: */",
    "/*     PSR42: backup PSR10 */",
    "/*     PSR43: backup PSR11 */",
    "/*     PSR44: backup PSR12 */",
    "/*     PSR45: */",
    "/*     PSR46: */",
    "/*     PSR47: */",
    "/* RO: PSR48: Characteristic text caps */",
    "/* RO: PSR49: Characteristic text caps */",
    "/* RO: PSR50: Characteristic text caps */",
    "/* RO: PSR51: Characteristic text caps */",
    "/* RO: PSR52: Characteristic text caps */",
    "/* RO: PSR53: Characteristic text caps */",
    "/* RO: PSR54: Characteristic text caps */",
    "/* RO: PSR55: Characteristic text caps */",
    "/* RO: PSR56: Characteristic text caps */",
    "/* RO: PSR57: Characteristic text caps */",
    "/* RO: PSR58: Characteristic text caps */",
    "/* RO: PSR59: Characteristic text caps */",
    "/* RO: PSR60: Characteristic text caps */",
    "/* RO: PSR61: Characteristic text caps */",
};

        static string[] insn_groups = new string[] {
    "BRANCH",
    "COMPARE",
    "SET",
};

        static string[] insn_group_branch = new string[]{
    "GOTO",
    "JUMP",
    "PLAY",
};

        static string[] insn_group_set = new string[]{
    "SET",
    "SETSYSTEM",
};

        static string[] insn_opt_set = new string[]{
    "",
    "move",
    "swap",
    "add",
    "sub",
    "mul",
    "div",
    "mod",
    "rnd",
    "and",
    "or",
    "xor",
    "bset",
    "bclr",
    "shl",
    "shr",
};

        static string[] insn_opt_setsys = new string[] {
    null,
    "SET_STREAM",
    "SET_NV_TIMER",
    "SET_BUTTON_PAGE",
    "ENABLE_BUTTON",
    "DISABLE_BUTTON",
    "SET_SEC_STREAM",
    "POPUP_OFF",
    "STILL_ON",
    "STILL_OFF",
};

        static string[] insn_opt_cmp = new string[]{
    null,
    "bc",
    "eq",
    "ne",
    "ge",
    "gt",
    "le",
    "lt",
};

        static string[] insn_opt_goto = new string[]{
    "nop",
    "goto",
    "break",
};

        static string[] insn_opt_jump = new string[]{
    "JUMP_OBJECT",
    "JUMP_TITLE",
    "CALL_OBJECT",
    "CALL_TITLE",
    "RESUME"
};

        static string[] insn_opt_play = new string[] {
    "PLAY_PL",
    "PLAY_PL_PI",
    "PLAY_PL_MK",
    "TERMINATE_PL",
    "LINK_PI",
    "LINK_MK"
};
        #endregion
        string _sprint_operand(string buf, bool imm, int op, ref int psr)
        {
            string start = "";

            if (!imm)
            {
                if ((op & 0x80000000) == 0x80000000)
                {
                    start += string.Format("PSR%-3u {0}", op & 0x7f);
                    psr = op & 0x7f;
                }
                else
                {
                    start += string.Format("r%-5u {0}", op & 0xfff);
                }
            }
            else
            {
                start = string.Format( "{0} {1}",buf, op);
            }

            return start;
        }
        string _sprint_operands(string buf, MOBJ_CMD cmd)
        {
            int psr1 = -1, psr2 = -1;
            if (cmd.insn.op_cnt > 0)
            {
                buf = _sprint_operand(buf, cmd.insn.imm_op1, cmd.Destination, ref psr1);
                if (cmd.insn.op_cnt > 1)
                {
                    buf = _sprint_operand(buf, cmd.insn.imm_op2, cmd.Source, ref psr2);
                }
            }
            if (psr1 >= 0)
                buf = string.Format("{0} {1}", buf, psr_info[psr1]);
            if ((psr2 >= 0) && (psr2 != psr1))
                buf = string.Format("{0} {1}", buf, psr_info[psr2]);
            return buf;
        }
        string _sprint_operands_hex(string buf, MOBJ_CMD cmd)
        {
            if (cmd.insn.op_cnt > 0)
            {
                buf = string.Format("{0} {1}", buf, cmd.Destination);
            }
            if (cmd.insn.op_cnt > 1)
            {
                buf = string.Format("{0} {1}", buf, cmd.Source);
            }
            return buf;
        }
        public string mobj_sprint_cmd(MOBJ_CMD cmd)
        {
            string buf="" ;
            string start = buf;
            HDMV_INSN insn = cmd.insn;
            switch ((hdmv_insn_grp)insn.grp)
            {
                case hdmv_insn_grp.INSN_GROUP_BRANCH:
                    #region groups Branch
                    switch ((hdmv_insn_grp_branch)insn.sub_grp)
                    {
                        case hdmv_insn_grp_branch.BRANCH_GOTO:
                            if (insn.branch_opt < insn_opt_goto.Length)
                            {
                                buf += insn_opt_goto[insn.branch_opt];
                                buf += _sprint_operands(buf, cmd);
                            }
                            else
                            {
                                //erreur
                                buf += string.Format( "[unknown BRANCH/GOTO option in opcode 0x{0:x}] ", (int)insn.Rawdata);
                            }
                            break;
                        case hdmv_insn_grp_branch.BRANCH_JUMP:
                            if (insn.branch_opt < insn_opt_jump.Length)
                            {
                                buf += insn_opt_jump[insn.branch_opt];
                                buf += _sprint_operands(buf, cmd);
                            }
                            else
                            {
                                //erreur
                                buf += string.Format("[unknown BRANCH/JUMP option in opcode 0x{0:x}] ", (int)insn.Rawdata);
                            }
                            break;
                        case hdmv_insn_grp_branch.BRANCH_PLAY:
                            if (insn.branch_opt < insn_opt_play.Length)
                            {
                                buf += insn_opt_play[insn.branch_opt];
                                buf += _sprint_operands(buf, cmd);
                            }
                            else
                            {
                                //erreur
                                buf += string.Format("[unknown BRANCH/PLAY option in opcode 0x{0:x}] ", (int)insn.Rawdata);
                            }
                            break;
                        default:
                            //erreur
                            buf += string.Format("[unknown BRANCH subgroup in opcode 0x{0:x}] ", (int)insn.Rawdata);
                            break;
                    }
                    break;
                    #endregion
                case hdmv_insn_grp.INSN_GROUP_CMP:
                    #region Compare group 
                    if (insn.cmp_opt < insn_opt_cmp.Length)
                    {
                        buf += insn_opt_cmp[insn.cmp_opt];
                        buf += _sprint_operands(buf, cmd);
                    }
                    else
                    {
                        //erreur
                        string.Format("[unknown COMPARE option in opcode 0x{0:x}] ", (int)insn.Rawdata);
                    }
                    break;
                    #endregion
                case hdmv_insn_grp.INSN_GROUP_SET:
                    #region Set group
                    switch ((hdmv_insn_grp_set)insn.sub_grp)
                    {
                        case hdmv_insn_grp_set.SET_SET:
                            if (insn.set_opt < insn_opt_set.Length)
                            {
                                buf += insn_opt_set[insn.set_opt];
                                buf += _sprint_operands(buf, cmd);
                            }
                            else
                            {
                                //erreur
                                string.Format("[unknown SET option in opcode 0x{0:x}] ", (int)insn.Rawdata);
                            }
                            break;
                        case hdmv_insn_grp_set.SET_SETSYSTEM:
                            if (insn.set_opt < insn_opt_setsys.Length)
                            {
                                buf += insn_opt_setsys[insn.set_opt];
                                buf += _sprint_operands_hex(buf, cmd);
                            }
                            else
                            {
                                //erreur
                                string.Format("[unknown SETSYSTEM option in opcode 0x{0:x}] ", (int)insn.Rawdata);
                            }
                            break;
                        default:
                            //erreur
                            string.Format("[unknown SET subgroup in opcode 0x{0:x}] ", (int)insn.Rawdata);
                            break;
                    }
                    break;
                    #endregion
                default:
                    //erreur
                    buf = string.Format("[unknown group in opcode 0x{0:x}] ", (int)insn.Rawdata);
                    break;
            }

            return buf;//buf - start;
        }
        public bool check_cmd(MOBJ_CMD cmd, out string Message)
        {
            string buf = "";
            HDMV_INSN insn = cmd.insn;
            switch ((hdmv_insn_grp)insn.grp)
            {
                case hdmv_insn_grp.INSN_GROUP_BRANCH:
                    #region groups Branch
                    switch ((hdmv_insn_grp_branch)insn.sub_grp)
                    {
                        case hdmv_insn_grp_branch.BRANCH_GOTO:
                            if (insn.branch_opt < insn_opt_goto.Length)
                            {
                                buf = insn_opt_goto[insn.branch_opt];
                                buf += _sprint_operands(buf, cmd);
                            }
                            else
                            {
                                 Message = string.Format("[unknown BRANCH/GOTO option in opcode 0x{0:x}] ", (int)insn.Rawdata);
                                return false;
                            }
                            break;
                        case hdmv_insn_grp_branch.BRANCH_JUMP:
                            if (insn.branch_opt < insn_opt_jump.Length)
                            {
                                buf = insn_opt_jump[insn.branch_opt];
                                buf += _sprint_operands(buf, cmd);
                            }
                            else
                            {
                                //erreur
                                Message = string.Format("[unknown BRANCH/JUMP option in opcode 0x{0:x}] ", (int)insn.Rawdata);
                                return false;
                            }
                            break;
                        case hdmv_insn_grp_branch.BRANCH_PLAY:
                            if (insn.branch_opt < insn_opt_play.Length)
                            {
                                buf = insn_opt_play[insn.branch_opt];
                                buf += _sprint_operands(buf, cmd);
                            }
                            else
                            {
                                //erreur
                                Message = string.Format("[unknown BRANCH/PLAY option in opcode 0x{0:x}] ", (int)insn.Rawdata);
                                return false;
                            }
                            break;
                        default:
                            //erreur
                            Message = string.Format("[unknown BRANCH subgroup in opcode 0x{0:x}] ", (int)insn.Rawdata);
                            return false;
                    }
                    break;
                    #endregion
                case hdmv_insn_grp.INSN_GROUP_CMP:
                    #region Compare group
                    if (insn.cmp_opt < insn_opt_cmp.Length)
                    {
                        buf = insn_opt_cmp[insn.cmp_opt];
                        buf += _sprint_operands(buf, cmd);
                    }
                    else
                    {
                         Message = string.Format("[unknown COMPARE option in opcode 0x{0:x}] ", (int)insn.Rawdata);
                        return false;
                    }
                    break;
                    #endregion
                case hdmv_insn_grp.INSN_GROUP_SET:
                    #region Set group
                    switch ((hdmv_insn_grp_set)insn.sub_grp)
                    {
                        case hdmv_insn_grp_set.SET_SET:
                            if (insn.set_opt < insn_opt_set.Length)
                            {
                                buf += insn_opt_set[insn.set_opt];
                                buf += _sprint_operands(buf, cmd);
                            }
                            else
                            {
                                Message = string.Format("[unknown SET option in opcode 0x{0:x}] ", (int)insn.Rawdata);
                                return false;
                            }
                            break;
                        case hdmv_insn_grp_set.SET_SETSYSTEM:
                            if (insn.set_opt < insn_opt_setsys.Length)
                            {
                                buf = insn_opt_setsys[insn.set_opt];
                                buf += _sprint_operands_hex(buf, cmd);
                            }
                            else
                            {
                                Message = string.Format("[unknown SETSYSTEM option in opcode 0x{0:x}] ", (int)insn.Rawdata);
                                return false;
                            }
                            break;
                        default:
                            Message = string.Format("[unknown SET subgroup in opcode 0x{0:x}] ", (int)insn.Rawdata);
                            return false;
                     }
                    break;
                    #endregion
                default:
                    //erreur
                    Message = string.Format("[unknown group in opcode 0x{0:x}] ", (int)insn.Rawdata);
                    return false;
            }
            Message = buf;
            return true;
        }
    }
    #region Instruction codes
    enum hdmv_insn_grp
    {
        INSN_GROUP_BRANCH = 0,
        INSN_GROUP_CMP = 1,
        INSN_GROUP_SET = 2,
    }
    /* BRANCH group    */
    /* BRANCH sub-groups */
    enum hdmv_insn_grp_branch
    {
        BRANCH_GOTO = 0x00,
        BRANCH_JUMP = 0x01,
        BRANCH_PLAY = 0x02,
    }
    /* GOTO sub-group */
    enum hdmv_insn_goto
    {
        INSN_NOP = 0x00,
        INSN_GOTO = 0x01,
        INSN_BREAK = 0x02,
    }
    /* JUMP sub-group */
    enum hdmv_insn_jump
    {
        INSN_JUMP_OBJECT = 0x00,
        INSN_JUMP_TITLE = 0x01,
        INSN_CALL_OBJECT = 0x02,
        INSN_CALL_TITLE = 0x03,
        INSN_RESUME = 0x04,
    }
    /* PLAY sub-group */
    enum hdmv_insn_play
    {
        INSN_PLAY_PL = 0x00,
        INSN_PLAY_PL_PI = 0x01,
        INSN_PLAY_PL_PM = 0x02,
        INSN_TERMINATE_PL = 0x03,
        INSN_LINK_PI = 0x04,
        INSN_LINK_MK = 0x05,
    }
    /* COMPARE group  */
    enum hdmv_insn_cmp
    {
        INSN_BC = 0x01,
        INSN_EQ = 0x02,
        INSN_NE = 0x03,
        INSN_GE = 0x04,
        INSN_GT = 0x05,
        INSN_LE = 0x06,
        INSN_LT = 0x07,
    }
    /* SET group   */
    /* SET sub-groups */
    enum hdmv_insn_grp_set
    {
        SET_SET = 0x00,
        SET_SETSYSTEM = 0x01,
    }
    /* SET sub-group */
    enum hdmv_insn_set
    {
        INSN_MOVE = 0x01,
        INSN_SWAP = 0x02,
        INSN_ADD = 0x03,
        INSN_SUB = 0x04,
        INSN_MUL = 0x05,
        INSN_DIV = 0x06,
        INSN_MOD = 0x07,
        INSN_RND = 0x08,
        INSN_AND = 0x09,
        INSN_OR = 0x0a,
        INSN_XOR = 0x0b,
        INSN_BITSET = 0x0c,
        INSN_BITCLR = 0x0d,
        INSN_SHL = 0x0e,
        INSN_SHR = 0x0f,
    }
    /* SETSYSTEM sub-group */
    enum hdmv_insn_setsystem
    {
        INSN_SET_STREAM = 0x01,
        INSN_SET_NV_TIMER = 0x02,
        INSN_SET_BUTTON_PAGE = 0x03,
        INSN_ENABLE_BUTTON = 0x04,
        INSN_DISABLE_BUTTON = 0x05,
        INSN_SET_SEC_STREAM = 0x06,
        INSN_POPUP_OFF = 0x07,
        INSN_STILL_ON = 0x08,
        INSN_STILL_OFF = 0x09,
    }
    #endregion

}
