using System;
using System.Diagnostics;
using System.Text;

namespace Community.CsharpSqlite
{
    public partial class Sqlite3
    {
        //#define Sqlite3.TK_SEMI                            1
        //#define Sqlite3.TK_EXPLAIN                         2
        //#define Sqlite3.TK_QUERY                           3
        //#define Sqlite3.TK_PLAN                            4
        //#define Sqlite3.TK_BEGIN                           5
        //#define Sqlite3.TK_TRANSACTION                     6
        //#define Sqlite3.TK_DEFERRED                        7
        //#define Sqlite3.TK_IMMEDIATE                       8
        //#define Sqlite3.TK_EXCLUSIVE                       9
        //#define Sqlite3.TK_COMMIT                         10
        //#define Sqlite3.TK_END                            11
        //#define Sqlite3.TK_ROLLBACK                       12
        //#define Sqlite3.TK_SAVEPOINT                      13
        //#define Sqlite3.TK_RELEASE                        14
        //#define Sqlite3.TK_TO                             15
        //#define Sqlite3.TK_TABLE                          16
        //#define Sqlite3.TK_CREATE                         17
        //#define Sqlite3.TK_IF                             18
        //#define Sqlite3.TK_NOT                            19
        //#define Sqlite3.TK_EXISTS                         20
        //#define Sqlite3.TK_TEMP                           21
        //#define Sqlite3.TK_LP                             22
        //#define Sqlite3.TK_RP                             23
        //#define Sqlite3.TK_AS                             24
        //#define Sqlite3.TK_COMMA                          25
        //#define Sqlite3.TK_ID                             26
        //#define Sqlite3.TK_INDEXED                        27
        //#define Sqlite3.TK_ABORT                          28
        //#define Sqlite3.TK_ACTION                         29
        //#define Sqlite3.TK_AFTER                          30
        //#define Sqlite3.TK_ANALYZE                        31
        //#define Sqlite3.TK_ASC                            32
        //#define Sqlite3.TK_ATTACH                         33
        //#define Sqlite3.TK_BEFORE                         34
        //#define Sqlite3.TK_BY                             35
        //#define Sqlite3.TK_CASCADE                        36
        //#define Sqlite3.TK_CAST                           37
        //#define Sqlite3.TK_COLUMNKW                       38
        //#define Sqlite3.TK_CONFLICT                       39
        //#define Sqlite3.TK_DATABASE                       40
        //#define Sqlite3.TK_DESC                           41
        //#define Sqlite3.TK_DETACH                         42
        //#define Sqlite3.TK_EACH                           43
        //#define Sqlite3.TK_FAIL                           44
        //#define Sqlite3.TK_FOR                            45
        //#define Sqlite3.TK_IGNORE                         46
        //#define Sqlite3.TK_INITIALLY                      47
        //#define Sqlite3.TK_INSTEAD                        48
        //#define Sqlite3.TK_LIKE_KW                        49
        //#define Sqlite3.TK_MATCH                          50
        //#define Sqlite3.TK_NO                             51
        //#define Sqlite3.TK_KEY                            52
        //#define Sqlite3.TK_OF                             53
        //#define Sqlite3.TK_OFFSET                         54
        //#define Sqlite3.TK_PRAGMA                         55
        //#define Sqlite3.TK_RAISE                          56
        //#define Sqlite3.TK_REPLACE                        57
        //#define Sqlite3.TK_RESTRICT                       58
        //#define Sqlite3.TK_ROW                            59
        //#define Sqlite3.TK_TRIGGER                        60
        //#define Sqlite3.TK_VACUUM                         61
        //#define Sqlite3.TK_VIEW                           62
        //#define Sqlite3.TK_VIRTUAL                        63
        //#define Sqlite3.TK_REINDEX                        64
        //#define Sqlite3.TK_RENAME                         65
        //#define Sqlite3.TK_CTIME_KW                       66
        //#define Sqlite3.TK_ANY                            67
        //#define Sqlite3.TK_OR                             68
        //#define Sqlite3.TK_AND                            69
        //#define Sqlite3.TK_IS                             70
        //#define Sqlite3.TK_BETWEEN                        71
        //#define Sqlite3.TK_IN                             72
        //#define Sqlite3.TK_ISNULL                         73
        //#define Sqlite3.TK_NOTNULL                        74
        //#define Sqlite3.TK_NE                             75
        //#define Sqlite3.TK_EQ                             76
        //#define Sqlite3.TK_GT                             77
        //#define Sqlite3.TK_LE                             78
        //#define Sqlite3.TK_LT                             79
        //#define Sqlite3.TK_GE                             80
        //#define Sqlite3.TK_ESCAPE                         81
        //#define Sqlite3.TK_BITAND                         82
        //#define Sqlite3.TK_BITOR                          83
        //#define Sqlite3.TK_LSHIFT                         84
        //#define Sqlite3.TK_RSHIFT                         85
        //#define Sqlite3.TK_PLUS                           86
        //#define Sqlite3.TK_MINUS                          87
        //#define Sqlite3.TK_STAR                           88
        //#define Sqlite3.TK_SLASH                          89
        //#define Sqlite3.TK_REM                            90
        //#define Sqlite3.TK_CONCAT                         91
        //#define Sqlite3.TK_COLLATE                        92
        //#define Sqlite3.TK_BITNOT                         93
        //#define Sqlite3.TK_STRING                         94
        //#define Sqlite3.TK_JOIN_KW                        95
        //#define Sqlite3.TK_CONSTRAINT                     96
        //#define Sqlite3.TK_DEFAULT                        97
        //#define Sqlite3.TK_NULL                           98
        //#define Sqlite3.TK_PRIMARY                        99
        //#define Sqlite3.TK_UNIQUE                         100
        //#define Sqlite3.TK_CHECK                          101
        //#define Sqlite3.TK_REFERENCES                     102
        //#define Sqlite3.TK_AUTOINCR                       103
        //#define Sqlite3.TK_ON                             104
        //#define Sqlite3.TK_INSERT                         105
        //#define Sqlite3.TK_DELETE                         106
        //#define Sqlite3.TK_UPDATE                         107
        //#define Sqlite3.TK_SET                            108
        //#define Sqlite3.TK_DEFERRABLE                     109
        //#define Sqlite3.TK_FOREIGN                        110
        //#define Sqlite3.TK_DROP                           111
        //#define Sqlite3.TK_UNION                          112
        //#define Sqlite3.TK_ALL                            113
        //#define Sqlite3.TK_EXCEPT                         114
        //#define Sqlite3.TK_INTERSECT                      115
        //#define Sqlite3.TK_SELECT                         116
        //#define Sqlite3.TK_DISTINCT                       117
        //#define Sqlite3.TK_DOT                            118
        //#define Sqlite3.TK_FROM                           119
        //#define Sqlite3.TK_JOIN                           120
        //#define Sqlite3.TK_USING                          121
        //#define Sqlite3.TK_ORDER                          122
        //#define Sqlite3.TK_GROUP                          123
        //#define Sqlite3.TK_HAVING                         124
        //#define Sqlite3.TK_LIMIT                          125
        //#define Sqlite3.TK_WHERE                          126
        //#define Sqlite3.TK_INTO                           127
        //#define Sqlite3.TK_VALUES                         128
        //#define Sqlite3.TK_INTEGER                        129
        //#define Sqlite3.TK_FLOAT                          130
        //#define Sqlite3.TK_BLOB                           131
        //#define Sqlite3.TK_REGISTER                       132
        //#define Sqlite3.TK_VARIABLE                       133
        //#define Sqlite3.TK_CASE                           134
        //#define Sqlite3.TK_WHEN                           135
        //#define Sqlite3.TK_THEN                           136
        //#define Sqlite3.TK_ELSE                           137
        //#define Sqlite3.TK_INDEX                          138
        //#define Sqlite3.TK_ALTER                          139
        //#define Sqlite3.TK_ADD                            140
        //#define Sqlite3.TK_TO_TEXT                        141
        //#define Sqlite3.TK_TO_BLOB                        142
        //#define Sqlite3.TK_TO_NUMERIC                     143
        //#define Sqlite3.TK_TO_INT                         144
        //#define Sqlite3.TK_TO_REAL                        145
        //#define Sqlite3.TK_ISNOT                          146
        //#define Sqlite3.TK_END_OF_FILE                    147
        //#define Sqlite3.TK_ILLEGAL                        148
        //#define Sqlite3.TK_SPACE                          149
        //#define Sqlite3.TK_UNCLOSED_STRING                150
        //#define Sqlite3.TK_FUNCTION                       151
        //#define Sqlite3.TK_COLUMN                         152
        //#define Sqlite3.TK_AGG_FUNCTION                   153
        //#define Sqlite3.TK_AGG_COLUMN                     154
        //#define Sqlite3.TK_CONST_FUNC                     155
        //#define Sqlite3.TK_UMINUS                         156
        //#define Sqlite3.TK_UPLUS                          157
        public const int TK_SEMI = 1;

        public const int TK_EXPLAIN = 2;

        public const int TK_QUERY = 3;

        public const int TK_PLAN = 4;

        public const int TK_BEGIN = 5;

        public const int TK_TRANSACTION = 6;

        public const int TK_DEFERRED = 7;

        public const int TK_IMMEDIATE = 8;

        public const int TK_EXCLUSIVE = 9;

        public const int TK_COMMIT = 10;

        public const int TK_END = 11;

        public const int TK_ROLLBACK = 12;

        public const int TK_SAVEPOINT = 13;

        public const int TK_RELEASE = 14;

        public const int TK_TO = 15;

        public const int TK_TABLE = 16;

        public const int TK_CREATE = 17;

        public const int TK_IF = 18;

        public const int TK_NOT = 19;

        public const int TK_EXISTS = 20;

        public const int TK_TEMP = 21;

        public const int TK_LP = 22;

        public const int TK_RP = 23;

        public const int TK_AS = 24;

        public const int TK_COMMA = 25;

        public const int TK_ID = 26;

        public const int TK_INDEXED = 27;

        public const int TK_ABORT = 28;

        public const int TK_ACTION = 29;

        public const int TK_AFTER = 30;

        public const int TK_ANALYZE = 31;

        public const int TK_ASC = 32;

        public const int TK_ATTACH = 33;

        public const int TK_BEFORE = 34;

        public const int TK_BY = 35;

        public const int TK_CASCADE = 36;

        public const int TK_CAST = 37;

        public const int TK_COLUMNKW = 38;

        public const int TK_CONFLICT = 39;

        public const int TK_DATABASE = 40;

        public const int TK_DESC = 41;

        public const int TK_DETACH = 42;

        public const int TK_EACH = 43;

        public const int TK_FAIL = 44;

        public const int TK_FOR = 45;

        public const int TK_IGNORE = 46;

        public const int TK_INITIALLY = 47;

        public const int TK_INSTEAD = 48;

        public const int TK_LIKE_KW = 49;

        public const int TK_MATCH = 50;

        public const int TK_NO = 51;

        public const int TK_KEY = 52;

        public const int TK_OF = 53;

        public const int TK_OFFSET = 54;

        public const int TK_PRAGMA = 55;

        public const int TK_RAISE = 56;

        public const int TK_REPLACE = 57;

        public const int TK_RESTRICT = 58;

        public const int TK_ROW = 59;

        public const int TK_TRIGGER = 60;

        public const int TK_VACUUM = 61;

        public const int TK_VIEW = 62;

        public const int TK_VIRTUAL = 63;

        public const int TK_REINDEX = 64;

        public const int TK_RENAME = 65;

        public const int TK_CTIME_KW = 66;

        public const int TK_ANY = 67;

        public const int TK_OR = 68;

        public const int TK_AND = 69;

        public const int TK_IS = 70;

        public const int TK_BETWEEN = 71;

        public const int TK_IN = 72;

        public const int TK_ISNULL = 73;

        public const int TK_NOTNULL = 74;

        public const int TK_NE = 75;

        public const int TK_EQ = 76;

        public const int TK_GT = 77;

        public const int TK_LE = 78;

        public const int TK_LT = 79;

        public const int TK_GE = 80;

        public const int TK_ESCAPE = 81;

        public const int TK_BITAND = 82;

        public const int TK_BITOR = 83;

        public const int TK_LSHIFT = 84;

        public const int TK_RSHIFT = 85;

        public const int TK_PLUS = 86;

        public const int TK_MINUS = 87;

        public const int TK_STAR = 88;

        public const int TK_SLASH = 89;

        public const int TK_REM = 90;

        public const int TK_CONCAT = 91;

        public const int TK_COLLATE = 92;

        public const int TK_BITNOT = 93;

        public const int TK_STRING = 94;

        public const int TK_JOIN_KW = 95;

        public const int TK_CONSTRAINT = 96;

        public const int TK_DEFAULT = 97;

        public const int TK_NULL = 98;

        public const int TK_PRIMARY = 99;

        public const int TK_UNIQUE = 100;

        public const int TK_CHECK = 101;

        public const int TK_REFERENCES = 102;

        public const int TK_AUTOINCR = 103;

        public const int TK_ON = 104;

        public const int TK_INSERT = 105;

        public const int TK_DELETE = 106;

        public const int TK_UPDATE = 107;

        public const int TK_SET = 108;

        public const int TK_DEFERRABLE = 109;

        public const int TK_FOREIGN = 110;

        public const int TK_DROP = 111;

        public const int TK_UNION = 112;

        public const int TK_ALL = 113;

        public const int TK_EXCEPT = 114;

        public const int TK_INTERSECT = 115;

        public const int TK_SELECT = 116;

        public const int TK_DISTINCT = 117;

        public const int TK_DOT = 118;

        public const int TK_FROM = 119;

        public const int TK_JOIN = 120;

        public const int TK_USING = 121;

        public const int TK_ORDER = 122;

        public const int TK_GROUP = 123;

        public const int TK_HAVING = 124;

        public const int TK_LIMIT = 125;

        public const int TK_WHERE = 126;

        public const int TK_INTO = 127;

        public const int TK_VALUES = 128;

        public const int TK_INTEGER = 129;

        public const int TK_FLOAT = 130;

        public const int TK_BLOB = 131;

        public const int TK_REGISTER = 132;

        public const int TK_VARIABLE = 133;

        public const int TK_CASE = 134;

        public const int TK_WHEN = 135;

        public const int TK_THEN = 136;

        public const int TK_ELSE = 137;

        public const int TK_INDEX = 138;

        public const int TK_ALTER = 139;

        public const int TK_ADD = 140;

        public const int TK_TO_TEXT = 141;

        public const int TK_TO_BLOB = 142;

        public const int TK_TO_NUMERIC = 143;

        public const int TK_TO_INT = 144;

        public const int TK_TO_REAL = 145;

        public const int TK_ISNOT = 146;

        public const int TK_END_OF_FILE = 147;

        public const int TK_ILLEGAL = 148;

        public const int TK_SPACE = 149;

        public const int TK_UNCLOSED_STRING = 150;

        public const int TK_FUNCTION = 151;

        public const int TK_COLUMN = 152;

        public const int TK_AGG_FUNCTION = 153;

        public const int TK_AGG_COLUMN = 154;

        public const int TK_CONST_FUNC = 155;

        public const int TK_UMINUS = 156;

        public const int TK_UPLUS = 157;


        public class Lexer
        {
            ///
            ///<summary>
            ///2001 September 15
            ///
            ///The author disclaims copyright to this source code.  In place of
            ///a legal notice, here is a blessing:
            ///
            ///May you do good and not evil.
            ///May you find forgiveness for yourself and forgive others.
            ///May you share freely, never taking more than you give.
            ///
            ///
            ///An tokenizer for SQL
            ///
            ///This file contains C code that splits an SQL input string up into
            ///</summary>
            ///<param name="individual tokens and sends those tokens one">one over to the</param>
            ///<param name="parser for analysis.">parser for analysis.</param>
            ///<param name=""></param>
            ///<param name="Included in SQLite3 port to C#">SQLite;  2008 Noah B Hart</param>
            ///<param name="C#">SQLite is an independent reimplementation of the SQLite software library</param>
            ///<param name=""></param>
            ///<param name="SQLITE_SOURCE_ID: 2011">23 19:49:22 4374b7e83ea0a3fbc3691f9c0c936272862f32f2</param>
            ///<param name=""></param>
            ///<param name=""></param>
            ///<param name=""></param>

            //#include "sqliteInt.h"
            //#include <stdlib.h>
            ///
            ///<summary>
            ///The charMap() macro maps alphabetic characters into their
            ///</summary>
            ///<param name="lower">case ASCII equivalent.  On ASCII machines, this is just</param>
            ///<param name="an upper">lower case map.  On EBCDIC machines we also need</param>
            ///<param name="to adjust the encoding.  Only alphabetic characters and underscores">to adjust the encoding.  Only alphabetic characters and underscores</param>
            ///<param name="need to be translated.">need to be translated.</param>
            ///<param name=""></param>

#if SQLITE_ASCII
            //# define charMap(X) _Custom.sqlite3UpperToLower[(unsigned char)X]
#endif
            //#if SQLITE_EBCDIC
            //# define charMap(X) ebcdicToAscii[(unsigned char)X]
            //const unsigned char ebcdicToAscii[] = {
            ///* 0   1   2   3   4   5   6   7   8   9   A   B   C   D   E   F */
            //   0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  /* 0x */
            //   0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  /* 1x */
            //   0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  /* 2x */
            //   0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  /* 3x */
            //   0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  /* 4x */
            //   0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  /* 5x */
            //   0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0, 95,  0,  0,  /* 6x */
            //   0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  /* 7x */
            //   0, 97, 98, 99,100,101,102,103,104,105,  0,  0,  0,  0,  0,  0,  /* 8x */
            //   0,106,107,108,109,110,111,112,113,114,  0,  0,  0,  0,  0,  0,  /* 9x */
            //   0,  0,115,116,117,118,119,120,121,122,  0,  0,  0,  0,  0,  0,  /* Ax */
            //   0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  /* Bx */
            //   0, 97, 98, 99,100,101,102,103,104,105,  0,  0,  0,  0,  0,  0,  /* Cx */
            //   0,106,107,108,109,110,111,112,113,114,  0,  0,  0,  0,  0,  0,  /* Dx */
            //   0,  0,115,116,117,118,119,120,121,122,  0,  0,  0,  0,  0,  0,  /* Ex */
            //   0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  /* Fx */
            //};
            //#endif
            ///
            ///<summary>
            ///The sqlite3KeywordCode function looks up an identifier to determine if
            ///it is a keyword.  If it is a keyword, the token code of that keyword is
            ///returned.  If the input is not a keyword, Sqlite3.TK_ID is returned.
            ///
            ///The implementation of this routine was generated by a program,
            ///mkkeywordhash.h, located in the tool subdirectory of the distribution.
            ///The output of the mkkeywordhash.c program is written into a file
            ///named keywordhash.h and then included into this source file by
            ///the #include below.
            ///</summary>

            //#include "keywordhash.h"
            ///<summary>
            /// If X is a character that can be used in an identifier then
            /// IdChar(X) will be true.  Otherwise it is false.
            ///
            /// For ASCII, any character with the high-order bit set is
            /// allowed in an identifier.  For 7-bit characters,
            /// sqlite3IsIdChar[X] must be 1.
            ///
            /// For EBCDIC, the rules are more complex but have the same
            /// end result.
            ///
            /// Ticket #1066.  the SQL standard does not allow '$' in the
            /// middle of identfiers.  But many SQL implementations do.
            /// SQLite will allow '$' in identifiers for compatibility.
            /// But the feature is undocumented.
            ///
            ///</summary>
#if SQLITE_ASCII
            //#define IdChar(C)  ((sqlite3CtypeMap[(unsigned char)C]&0x46)!=0)
#endif
            //#if SQLITE_EBCDIC
            //const char sqlite3IsEbcdicIdChar[] = {
            ///* x0 x1 x2 x3 x4 x5 x6 x7 x8 x9 xA xB xC xD xE xF */
            //    0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0,  /* 4x */
            //    0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 1, 0, 0, 0, 0,  /* 5x */
            //    0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 1, 0, 0,  /* 6x */
            //    0, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0,  /* 7x */
            //    0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 1, 1, 1, 0,  /* 8x */
            //    0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 1, 0, 1, 0,  /* 9x */
            //    1, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 1, 1, 1, 0,  /* Ax */
            //    0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,  /* Bx */
            //    0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 1, 1, 1, 1, 1,  /* Cx */
            //    0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 1, 1, 1, 1, 1,  /* Dx */
            //    0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 0, 1, 1, 1, 1, 1,  /* Ex */
            //    1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 1, 1, 1, 1, 0,  /* Fx */
            //};
            //#define IdChar(C)  (((c=C)>=0x42 && sqlite3IsEbcdicIdChar[c-0x40]))
            //#endif
            ///<summary>
            /// Return the length of the token that begins at z[iOffset + 0].
            /// Store the token type in *tokenType before returning.
            ///</summary>
            public static int sqlite3GetToken(string z, int iOffset, ref TokenType tokenType)
            {
                int i;
                byte c = 0;
                switch (z[iOffset + 0])
                {
                    case ' ':
                    case '\t':
                    case '\n':
                    case '\f':
                    case '\r':
                        {
                            sqliteinth.testcase(z[iOffset + 0] == ' ');
                            sqliteinth.testcase(z[iOffset + 0] == '\t');
                            sqliteinth.testcase(z[iOffset + 0] == '\n');
                            sqliteinth.testcase(z[iOffset + 0] == '\f');
                            sqliteinth.testcase(z[iOffset + 0] == '\r');
                            for (i = 1; z.Length > iOffset + i && CharExtensions.sqlite3Isspace(z[iOffset + i]); i++)
                            {
                            }
                            tokenType = TokenType.TK_SPACE;
                            return i;
                        }
                    case '-':
                        {
                            if (z.Length > iOffset + 1 && z[iOffset + 1] == '-')
                            {
                                ///
                                ///<summary>
                                ///</summary>
                                ///<param name="IMP: R"> syntax diagram for comments </param>

                                for (i = 2; z.Length > iOffset + i && (c = (byte)z[iOffset + i]) != 0 && c != '\n'; i++)
                                {
                                }
                                tokenType = TokenType.TK_SPACE;
                                ///
                                ///<summary>
                                ///</summary>
                                ///<param name="IMP: R">25134 </param>

                                return i;
                            }
                            tokenType = TokenType.TK_MINUS;
                            return 1;
                        }
                    case '(':
                        {
                            tokenType = TokenType.TK_LP;
                            return 1;
                        }
                    case ')':
                        {
                            tokenType = TokenType.TK_RP;
                            return 1;
                        }
                    case ';':
                        {
                            tokenType = TokenType.TK_SEMI;
                            return 1;
                        }
                    case '+':
                        {
                            tokenType = TokenType.TK_PLUS;
                            return 1;
                        }
                    case '*':
                        {
                            tokenType = TokenType.TK_STAR;
                            return 1;
                        }
                    case '/':
                        {
                            if (iOffset + 2 >= z.Length || z[iOffset + 1] != '*')
                            {
                                tokenType = TokenType.TK_SLASH;
                                return 1;
                            }
                            ///
                            ///<summary>
                            ///</summary>
                            ///<param name="IMP: R"> syntax diagram for comments </param>

                            for (i = 3, c = (byte)z[iOffset + 2]; iOffset + i < z.Length && (c != '*' || (z[iOffset + i] != '/') && (c != 0)); i++)
                            {
                                c = (byte)z[iOffset + i];
                            }
                            if (iOffset + i == z.Length)
                                c = 0;
                            if (c != 0)
                                i++;
                            tokenType = TokenType.TK_SPACE;
                            ///
                            ///<summary>
                            ///</summary>
                            ///<param name="IMP: R">25134 </param>

                            return i;
                        }
                    case '%':
                        {
                            tokenType = TokenType.TK_REM;
                            return 1;
                        }
                    case '=':
                        {
                            tokenType = TokenType.TK_EQ;
                            return 1 + (z[iOffset + 1] == '=' ? 1 : 0);
                        }
                    case '<':
                        {
                            if ((c = (byte)z[iOffset + 1]) == '=')
                            {
                                tokenType = TokenType.TK_LE;
                                return 2;
                            }
                            else
                                if (c == '>')
                                {
                                    tokenType = TokenType.TK_NE;
                                    return 2;
                                }
                                else
                                    if (c == '<')
                                    {
                                        tokenType = TokenType.TK_LSHIFT;
                                        return 2;
                                    }
                                    else
                                    {
                                        tokenType = TokenType.TK_LT;
                                        return 1;
                                    }
                        }
                    case '>':
                        {
                            if (z.Length > iOffset + 1 && (c = (byte)z[iOffset + 1]) == '=')
                            {
                                tokenType = TokenType.TK_GE;
                                return 2;
                            }
                            else
                                if (c == '>')
                                {
                                    tokenType = TokenType.TK_RSHIFT;
                                    return 2;
                                }
                                else
                                {
                                    tokenType = TokenType.TK_GT;
                                    return 1;
                                }
                        }
                    case '!':
                        {
                            if (z[iOffset + 1] != '=')
                            {
                                tokenType = TokenType.TK_ILLEGAL;
                                return 2;
                            }
                            else
                            {
                                tokenType = TokenType.TK_NE;
                                return 2;
                            }
                        }
                    case '|':
                        {
                            if (z[iOffset + 1] != '|')
                            {
                                tokenType = TokenType.TK_BITOR;
                                return 1;
                            }
                            else
                            {
                                tokenType = TokenType.TK_CONCAT;
                                return 2;
                            }
                        }
                    case ',':
                        {
                            tokenType = TokenType.TK_COMMA;
                            return 1;
                        }
                    case '&':
                        {
                            tokenType = TokenType.TK_BITAND;
                            return 1;
                        }
                    case '~':
                        {
                            tokenType = TokenType.TK_BITNOT;
                            return 1;
                        }
                    case '`':
                    case '\'':
                    case '"':
                        {
                            int delim = z[iOffset + 0];
                            sqliteinth.testcase(delim == '`');
                            sqliteinth.testcase(delim == '\'');
                            sqliteinth.testcase(delim == '"');
                            for (i = 1; (iOffset + i) < z.Length && (c = (byte)z[iOffset + i]) != 0; i++)
                            {
                                if (c == delim)
                                {
                                    if (z.Length > iOffset + i + 1 && z[iOffset + i + 1] == delim)
                                    {
                                        i++;
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                            }
                            if ((iOffset + i == z.Length && c != delim) || z[iOffset + i] != delim)
                            {
                                tokenType = TokenType.TK_ILLEGAL;
                                return i + 1;
                            }
                            if (c == '\'')
                            {
                                tokenType = TokenType.TK_STRING;
                                return i + 1;
                            }
                            else
                                if (c != 0)
                                {
                                    tokenType = TokenType.TK_ID;
                                    return i + 1;
                                }
                                else
                                {
                                    tokenType = TokenType.TK_ILLEGAL;
                                    return i;
                                }
                        }
                    case '.':
                        {
#if !SQLITE_OMIT_FLOATING_POINT
                            if (!CharExtensions.sqlite3Isdigit(z[iOffset + 1]))
#endif
                            {
                                tokenType = TokenType.TK_DOT;
                                return 1;
                            }
                            ///
                            ///<summary>
                            ///If the next character is a digit, this is a floating point
                            ///number that begins with ".".  Fall thru into the next case 
                            ///</summary>

                            goto case '0';
                        }
                    case '0':
                    case '1':
                    case '2':
                    case '3':
                    case '4':
                    case '5':
                    case '6':
                    case '7':
                    case '8':
                    case '9':
                        {
                            sqliteinth.testcase(z[iOffset] == '0');
                            sqliteinth.testcase(z[iOffset] == '1');
                            sqliteinth.testcase(z[iOffset] == '2');
                            sqliteinth.testcase(z[iOffset] == '3');
                            sqliteinth.testcase(z[iOffset] == '4');
                            sqliteinth.testcase(z[iOffset] == '5');
                            sqliteinth.testcase(z[iOffset] == '6');
                            sqliteinth.testcase(z[iOffset] == '7');
                            sqliteinth.testcase(z[iOffset] == '8');
                            sqliteinth.testcase(z[iOffset] == '9');
                            tokenType = TokenType.TK_INTEGER;
                            for (i = 0; z.Length > iOffset + i && CharExtensions.sqlite3Isdigit(z[iOffset + i]); i++)
                            {
                            }
#if !SQLITE_OMIT_FLOATING_POINT
                            if (z.Length > iOffset + i && z[iOffset + i] == '.')
                            {
                                i++;
                                while (z.Length > iOffset + i && CharExtensions.sqlite3Isdigit(z[iOffset + i]))
                                {
                                    i++;
                                }
                                tokenType = TokenType.TK_FLOAT;
                            }
                            if (z.Length > iOffset + i + 1 && (z[iOffset + i] == 'e' || z[iOffset + i] == 'E') && (CharExtensions.sqlite3Isdigit(z[iOffset + i + 1]) || z.Length > iOffset + i + 2 && ((z[iOffset + i + 1] == '+' || z[iOffset + i + 1] == '-') && CharExtensions.sqlite3Isdigit(z[iOffset + i + 2]))))
                            {
                                i += 2;
                                while (z.Length > iOffset + i && CharExtensions.sqlite3Isdigit(z[iOffset + i]))
                                {
                                    i++;
                                }
                                tokenType = TokenType.TK_FLOAT;
                            }
#endif
                            while (iOffset + i < z.Length && IdChar((byte)z[iOffset + i]))
                            {
                                tokenType = TokenType.TK_ILLEGAL;
                                i++;
                            }
                            return i;
                        }
                    case '[':
                        {
                            for (i = 1, c = (byte)z[iOffset + 0]; c != ']' && (iOffset + i) < z.Length && (c = (byte)z[iOffset + i]) != 0; i++)
                            {
                            }
                            tokenType = c == ']' ? TokenType.TK_ID : TokenType.TK_ILLEGAL;
                            return i;
                        }
                    case '?':
                        {
                            tokenType = TokenType.TK_VARIABLE;
                            for (i = 1; z.Length > iOffset + i && CharExtensions.sqlite3Isdigit(z[iOffset + i]); i++)
                            {
                            }
                            return i;
                        }
                    case '#':
                        {
                            for (i = 1; z.Length > iOffset + i && CharExtensions.sqlite3Isdigit(z[iOffset + i]); i++)
                            {
                            }
                            if (i > 1)
                            {
                                ///
                                ///<summary>
                                ///Parameters of the form #NNN (where NNN is a number) are used
                                ///internally by build.sqlite3NestedParse.  
                                ///</summary>

                                tokenType = TokenType.TK_REGISTER;
                                return i;
                            }
                            ///
                            ///<summary>
                            ///Fall through into the next case if the '#' is not followed by
                            ///a digit. Try to match #AAAA where AAAA is a parameter name. 
                            ///</summary>

                            goto case ':';
                        }
#if !SQLITE_OMIT_TCL_VARIABLE
                    case '$':
#endif
                    case '@':
                    ///
                    ///<summary>
                    ///For compatibility with MS SQL Server 
                    ///</summary>

                    case ':':
                        {
                            int n = 0;
                            sqliteinth.testcase(z[iOffset + 0] == '$');
                            sqliteinth.testcase(z[iOffset + 0] == '@');
                            sqliteinth.testcase(z[iOffset + 0] == ':');
                            tokenType = TokenType.TK_VARIABLE;
                            for (i = 1; z.Length > iOffset + i && (c = (byte)z[iOffset + i]) != 0; i++)
                            {
                                if (IdChar(c))
                                {
                                    n++;
#if !SQLITE_OMIT_TCL_VARIABLE
                                }
                                else
                                    if (c == '(' && n > 0)
                                    {
                                        do
                                        {
                                            i++;
                                        }
                                        while ((iOffset + i) < z.Length && (c = (byte)z[iOffset + i]) != 0 && !CharExtensions.sqlite3Isspace(c) && c != ')');
                                        if (c == ')')
                                        {
                                            i++;
                                        }
                                        else
                                        {
                                            tokenType = TokenType.TK_ILLEGAL;
                                        }
                                        break;
                                    }
                                    else
                                        if (c == ':' && z[iOffset + i + 1] == ':')
                                        {
                                            i++;
#endif
                                        }
                                        else
                                        {
                                            break;
                                        }
                            }
                            if (n == 0)
                                tokenType = TokenType.TK_ILLEGAL;
                            return i;
                        }
#if !SQLITE_OMIT_BLOB_LITERAL
                    case 'x':
                    case 'X':
                        {
                            sqliteinth.testcase(z[iOffset + 0] == 'x');
                            sqliteinth.testcase(z[iOffset + 0] == 'X');
                            if (z.Length > iOffset + 1 && z[iOffset + 1] == '\'')
                            {
                                tokenType = TokenType.TK_BLOB;
                                for (i = 2; z.Length > iOffset + i && CharExtensions.sqlite3Isxdigit(z[iOffset + i]); i++)
                                {
                                }
                                if (iOffset + i == z.Length || z[iOffset + i] != '\'' || i % 2 != 0)
                                {
                                    tokenType = TokenType.TK_ILLEGAL;
                                    while (z.Length > iOffset + i && z[iOffset + i] != '\'')
                                    {
                                        i++;
                                    }
                                }
                                if (z.Length > iOffset + i)
                                    i++;
                                return i;
                            }
                            goto default;
                            ///
                            ///<summary>
                            ///Otherwise fall through to the next case 
                            ///</summary>

                        }
#endif
                    default:
                        {
                            if (!IdChar((byte)z[iOffset]))
                            {
                                break;
                            }
                            for (i = 1; i < z.Length - iOffset && IdChar((byte)z[iOffset + i]); i++)
                            {
                            }
                            tokenType = keywordCode(z, iOffset, i);
                            return i;
                        }
                }
                tokenType = TokenType.TK_ILLEGAL;
                return 1;
            }

            public static Token GetToken(string z, int iOffset)
            {
                TokenType tokenType = 0;
                int length = sqlite3GetToken(z, iOffset, ref tokenType);
                var token = new Token()
                {
                    Start = iOffset,
                    TokenType = tokenType,
                    Text = z.Substring(iOffset, length),
                    zRestSql = z.Substring(iOffset),
                    Length = length
                };
                return token;
            }
            ///
            ///<summary>
            ///Run the parser on the given SQL string.  The parser structure is
            ///passed in.  An SQLITE_ status code is returned.  If an error occurs
            ///then an and attempt is made to write an error message into
            ///memory obtained from sqlite3_malloc() and to make pzErrMsg point to that
            ///error message.
            ///
            ///</summary>

        }
    }
}
