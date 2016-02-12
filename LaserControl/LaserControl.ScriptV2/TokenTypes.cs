using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaserControl.ScriptV2
{
    public enum TT
    {
        TT_BlockBegin, // {
        TT_BlockEnd,    // }
        TT_Comma,       // ,
        TT_Divide,      // /
        TT_Dollar,      // $
        TT_DoubleNumber,// 0.1223
        TT_EqualSign,   // =
        TT_EqualTest,   // ==
        TT_Ident,       // ABC
        TT_IdentCALLC,  // CALLC(
        TT_IdentCALLO,  // CALLO(
        TT_IdentCast,   // INT( / DOUBLE( /
        TT_IdentElse,   // ELSE
        TT_IdentFOR,    // FOR(
        TT_IdentID,     // ID
        TT_IdentInline, // ABC(
        TT_IdentIF,     // IF(
        TT_IdentISSET,  // ISSET(
        TT_IdentPAUSE,  // PAUSE
        TT_IdentRETURN, // RETURN or RETURN(
        TT_IdentTRUEFALSE, // true / false
        TT_IdentWHILE,  // WHILE(
        TT_IntNumber,   // 123
        TT_Minus,       // -
        TT_Multi,       // * 
        TT_NONE,        //         
        TT_ParenLeft,   // (
        TT_ParenRight,  // )
        TT_Percent,     // %
        TT_Plus,        // +
        TT_Power,       // ^
        TT_Semicolon,   // ;
        TT_SignGreater, // >
        TT_SignLess,    // <
        TT_SignLessOrEqual, //<=
        TT_ExclaMark,   // !
        TT_String,      // "dkyjlfdkgh öl"
        TT_UnEqualSign  // !=
    }
}
