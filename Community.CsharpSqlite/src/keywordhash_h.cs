using System.Diagnostics;
using System;
using Community.CsharpSqlite.Ast;

namespace Community.CsharpSqlite
{
	public partial class Sqlite3
	{
		///
///<summary>
///This file contains automatically generated code ******
///
///The code in this file has been automatically generated by
///
///sqlite/tool/mkkeywordhash.c
///
///The code in this file implements a function that determines whether
///or not a given identifier is really an SQL keyword.  The same thing
///might be implemented more directly using a hand-written hash table.
///But by using this automatically generated code, the size of the code
///is substantially reduced.  This is important for embedded applications
///on platforms with limited memory.
///Included in SQLite3 port to C#-SQLite;  2008 Noah B Hart
///<param name="C#">SQLite is an independent reimplementation of the SQLite software library</param>
///<param name=""></param>
///<param name="SQLITE_SOURCE_ID: 2010">23 18:52:01 42537b60566f288167f1b5864a5435986838e3a3</param>

///Hash score: 175 

///zText[] encodes 811 bytes of keywords in 541 bytes 
///REINDEXEDESCAPEACHECKEYBEFOREIGNOREGEXPLAINSTEADDATABASELECT       

///ABLEFTHENDEFERRABLELSEXCEPTRANSACTIONATURALTERAISEXCLUSIVE         

///XISTSAVEPOINTERSECTRIGGEREFERENCESCONSTRAINTOFFSETEMPORARY         
///UNIQUERYATTACHAVINGROUPDATEBEGINNERELEASEBETWEENOTNULLIKE          
///CASCADELETECASECOLLATECREATECURRENT_DATEDETACHIMMEDIATEJOIN        
///SERTMATCHPLANALYZEPRAGMABORTVALUESVIRTUALIMITWHENWHERENAME         
///AFTEREPLACEANDEFAULTAUTOINCREMENTCASTCOLUMNCOMMITCONFLICTCROSS     
///CURRENT_TIMESTAMPRIMARYDEFERREDISTINCTDROPFAILFROMFULLGLOBYIF      
///ISNULLORDERESTRICTOUTERIGHTROLLBACKROWUNIONUSINGVACUUMVIEW         
///INITIALLY
		static string zText = new string (new char[540] {
			'R',
			'E',
			'I',
			'N',
			'D',
			'E',
			'X',
			'E',
			'D',
			'E',
			'S',
			'C',
			'A',
			'P',
			'E',
			'A',
			'C',
			'H',
			'E',
			'C',
			'K',
			'E',
			'Y',
			'B',
			'E',
			'F',
			'O',
			'R',
			'E',
			'I',
			'G',
			'N',
			'O',
			'R',
			'E',
			'G',
			'E',
			'X',
			'P',
			'L',
			'A',
			'I',
			'N',
			'S',
			'T',
			'E',
			'A',
			'D',
			'D',
			'A',
			'T',
			'A',
			'B',
			'A',
			'S',
			'E',
			'L',
			'E',
			'C',
			'T',
			'A',
			'B',
			'L',
			'E',
			'F',
			'T',
			'H',
			'E',
			'N',
			'D',
			'E',
			'F',
			'E',
			'R',
			'R',
			'A',
			'B',
			'L',
			'E',
			'L',
			'S',
			'E',
			'X',
			'C',
			'E',
			'P',
			'T',
			'R',
			'A',
			'N',
			'S',
			'A',
			'C',
			'T',
			'I',
			'O',
			'N',
			'A',
			'T',
			'U',
			'R',
			'A',
			'L',
			'T',
			'E',
			'R',
			'A',
			'I',
			'S',
			'E',
			'X',
			'C',
			'L',
			'U',
			'S',
			'I',
			'V',
			'E',
			'X',
			'I',
			'S',
			'T',
			'S',
			'A',
			'V',
			'E',
			'P',
			'O',
			'I',
			'N',
			'T',
			'E',
			'R',
			'S',
			'E',
			'C',
			'T',
			'R',
			'I',
			'G',
			'G',
			#if !SQLITE_OMIT_TRIGGER
			'E',
			#else
																																																									'\0',
#endif
			'R',
			#if !SQLITE_OMIT_FOREIGN_KEY
			'E',
			#else
																																																									'\0',
#endif
			'F',
			'E',
			'R',
			'E',
			'N',
			'C',
			'E',
			'S',
			'C',
			'O',
			'N',
			'S',
			'T',
			'R',
			'A',
			'I',
			'N',
			'T',
			'O',
			'F',
			'F',
			'S',
			'E',
			'T',
			'E',
			'M',
			'P',
			'O',
			'R',
			'A',
			'R',
			'Y',
			'U',
			'N',
			'I',
			'Q',
			'U',
			'E',
			'R',
			'Y',
			'A',
			'T',
			'T',
			'A',
			'C',
			'H',
			'A',
			'V',
			'I',
			'N',
			'G',
			'R',
			'O',
			'U',
			'P',
			'D',
			'A',
			'T',
			'E',
			'B',
			'E',
			'G',
			'I',
			'N',
			'N',
			'E',
			'R',
			'E',
			'L',
			'E',
			'A',
			'S',
			'E',
			'B',
			'E',
			'T',
			'W',
			'E',
			'E',
			'N',
			'O',
			'T',
			'N',
			'U',
			'L',
			'L',
			'I',
			'K',
			'E',
			'C',
			'A',
			'S',
			'C',
			'A',
			'D',
			'E',
			'L',
			'E',
			'T',
			'E',
			'C',
			'A',
			'S',
			'E',
			'C',
			'O',
			'L',
			'L',
			'A',
			'T',
			'E',
			'C',
			'R',
			'E',
			'A',
			'T',
			'E',
			'C',
			'U',
			'R',
			'R',
			'E',
			'N',
			'T',
			'_',
			'D',
			'A',
			'T',
			'E',
			'D',
			'E',
			'T',
			'A',
			'C',
			'H',
			'I',
			'M',
			'M',
			'E',
			'D',
			'I',
			'A',
			'T',
			'E',
			'J',
			'O',
			'I',
			'N',
			'S',
			'E',
			'R',
			'T',
			'M',
			'A',
			'T',
			'C',
			'H',
			'P',
			'L',
			'A',
			'N',
			'A',
			'L',
			'Y',
			'Z',
			'E',
			'P',
			'R',
			'A',
			'G',
			'M',
			'A',
			'B',
			'O',
			'R',
			'T',
			'V',
			'A',
			'L',
			'U',
			'E',
			'S',
			'V',
			'I',
			'R',
			'T',
			'U',
			'A',
			'L',
			'I',
			'M',
			'I',
			'T',
			'W',
			'H',
			'E',
			'N',
			'W',
			'H',
			'E',
			'R',
			'E',
			'N',
			'A',
			'M',
			'E',
			'A',
			'F',
			'T',
			'E',
			'R',
			'E',
			'P',
			'L',
			'A',
			'C',
			'E',
			'A',
			'N',
			'D',
			'E',
			'F',
			'A',
			'U',
			'L',
			'T',
			'A',
			'U',
			'T',
			'O',
			'I',
			'N',
			'C',
			'R',
			'E',
			'M',
			'E',
			'N',
			'T',
			'C',
			'A',
			'S',
			'T',
			'C',
			'O',
			'L',
			'U',
			'M',
			'N',
			'C',
			'O',
			'M',
			'M',
			'I',
			'T',
			'C',
			'O',
			'N',
			'F',
			'L',
			'I',
			'C',
			'T',
			'C',
			'R',
			'O',
			'S',
			'S',
			'C',
			'U',
			'R',
			'R',
			'E',
			'N',
			'T',
			'_',
			'T',
			'I',
			'M',
			'E',
			'S',
			'T',
			'A',
			'M',
			'P',
			'R',
			'I',
			'M',
			'A',
			'R',
			'Y',
			'D',
			'E',
			'F',
			'E',
			'R',
			'R',
			'E',
			'D',
			'I',
			'S',
			'T',
			'I',
			'N',
			'C',
			'T',
			'D',
			'R',
			'O',
			'P',
			'F',
			'A',
			'I',
			'L',
			'F',
			'R',
			'O',
			'M',
			'F',
			'U',
			'L',
			'L',
			'G',
			'L',
			'O',
			'B',
			'Y',
			'I',
			'F',
			'I',
			'S',
			'N',
			'U',
			'L',
			'L',
			'O',
			'R',
			'D',
			'E',
			'R',
			'E',
			'S',
			'T',
			'R',
			'I',
			'C',
			'T',
			'O',
			'U',
			'T',
			'E',
			'R',
			'I',
			'G',
			'H',
			'T',
			'R',
			'O',
			'L',
			'L',
			'B',
			'A',
			'C',
			'K',
			'R',
			'O',
			'W',
			'U',
			'N',
			'I',
			'O',
			'N',
			'U',
			'S',
			'I',
			'N',
			'G',
			'V',
			'A',
			'C',
			'U',
			'U',
			'M',
			'V',
			'I',
			'E',
			'W',
			'I',
			'N',
			'I',
			'T',
			'I',
			'A',
			'L',
			'L',
			'Y',
		});

		static byte[] aHash =  {
			//aHash[127]
			72,
			101,
			114,
			70,
			0,
			45,
			0,
			0,
			78,
			0,
			73,
			0,
			0,
			42,
			12,
			74,
			15,
			0,
			113,
			81,
			50,
			108,
			0,
			19,
			0,
			0,
			118,
			0,
			116,
			111,
			0,
			22,
			89,
			0,
			9,
			0,
			0,
			66,
			67,
			0,
			65,
			6,
			0,
			48,
			86,
			98,
			0,
			115,
			97,
			0,
			0,
			44,
			0,
			99,
			24,
			0,
			17,
			0,
			119,
			49,
			23,
			0,
			5,
			106,
			25,
			92,
			0,
			0,
			121,
			102,
			56,
			120,
			53,
			28,
			51,
			0,
			87,
			0,
			96,
			26,
			0,
			95,
			0,
			0,
			0,
			91,
			88,
			93,
			84,
			105,
			14,
			39,
			104,
			0,
			77,
			0,
			18,
			85,
			107,
			32,
			0,
			117,
			76,
			109,
			58,
			46,
			80,
			0,
			0,
			90,
			40,
			0,
			112,
			0,
			36,
			0,
			0,
			29,
			0,
			82,
			59,
			60,
			0,
			20,
			57,
			0,
			52,
		};

        
		static byte[] aNext =  {
			//aNext[121]
			0,
			0,
			0,
			0,
			4,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			2,
			0,
			0,
			0,
			0,
			0,
			0,
			13,
			0,
			0,
			0,
			0,
			0,
			7,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			33,
			0,
			21,
			0,
			0,
			0,
			43,
			3,
			47,
			0,
			0,
			0,
			0,
			30,
			0,
			54,
			0,
			38,
			0,
			0,
			0,
			1,
			62,
			0,
			0,
			63,
			0,
			41,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			61,
			0,
			0,
			0,
			0,
			31,
			55,
			16,
			34,
			10,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			11,
			68,
			75,
			0,
			8,
			0,
			100,
			94,
			0,
			103,
			0,
			83,
			0,
			71,
			0,
			0,
			110,
			27,
			37,
			69,
			79,
			0,
			35,
			64,
			0,
			0,
		};

		static byte[] aLen =  {
			//aLen[121]
			7,
			7,
			5,
			4,
			6,
			4,
			5,
			3,
			6,
			7,
			3,
			6,
			6,
			7,
			7,
			3,
			8,
			2,
			6,
			5,
			4,
			4,
			3,
			10,
			4,
			6,
			11,
			6,
			2,
			7,
			5,
			5,
			9,
			6,
			9,
			9,
			7,
			10,
			10,
			4,
			6,
			2,
			3,
			9,
			4,
			2,
			6,
			5,
			6,
			6,
			5,
			6,
			5,
			5,
			7,
			7,
			7,
			3,
			2,
			4,
			4,
			7,
			3,
			6,
			4,
			7,
			6,
			12,
			6,
			9,
			4,
			6,
			5,
			4,
			7,
			6,
			5,
			6,
			7,
			5,
			4,
			5,
			6,
			5,
			7,
			3,
			7,
			13,
			2,
			2,
			4,
			6,
			6,
			8,
			5,
			17,
			12,
			7,
			8,
			8,
			2,
			4,
			4,
			4,
			4,
			4,
			2,
			2,
			6,
			5,
			8,
			5,
			5,
			8,
			3,
			5,
			5,
			6,
			4,
			9,
			3,
		};

		static int[] aOffset =  {
			//aOffset[121]
			0,
			2,
			2,
			8,
			9,
			14,
			16,
			20,
			23,
			25,
			25,
			29,
			33,
			36,
			41,
			46,
			48,
			53,
			54,
			59,
			62,
			65,
			67,
			69,
			78,
			81,
			86,
			91,
			95,
			96,
			101,
			105,
			109,
			117,
			122,
			128,
			136,
			142,
			152,
			159,
			162,
			162,
			165,
			167,
			167,
			171,
			176,
			179,
			184,
			189,
			194,
			197,
			203,
			206,
			210,
			217,
			223,
			223,
			223,
			226,
			229,
			233,
			234,
			238,
			244,
			248,
			255,
			261,
			273,
			279,
			288,
			290,
			296,
			301,
			303,
			310,
			315,
			320,
			326,
			332,
			337,
			341,
			344,
			350,
			354,
			361,
			363,
			370,
			372,
			374,
			383,
			387,
			393,
			399,
			407,
			412,
			412,
			428,
			435,
			442,
			443,
			450,
			454,
			458,
			462,
			466,
			469,
			471,
			473,
			479,
			483,
			491,
			495,
			500,
			508,
			511,
			516,
			521,
			527,
			531,
			536,
		};

		static byte[] aCode =  {
			//aCode[121
			Sqlite3.TK_REINDEX,
			Sqlite3.TK_INDEXED,
			Sqlite3.TK_INDEX,
			Sqlite3.TK_DESC,
			Sqlite3.TK_ESCAPE,
			Sqlite3.TK_EACH,
			Sqlite3.TK_CHECK,
			Sqlite3.TK_KEY,
			Sqlite3.TK_BEFORE,
			Sqlite3.TK_FOREIGN,
			Sqlite3.TK_FOR,
			Sqlite3.TK_IGNORE,
			Sqlite3.TK_LIKE_KW,
			Sqlite3.TK_EXPLAIN,
			Sqlite3.TK_INSTEAD,
			Sqlite3.TK_ADD,
			Sqlite3.TK_DATABASE,
			Sqlite3.TK_AS,
			Sqlite3.TK_SELECT,
			Sqlite3.TK_TABLE,
			Sqlite3.TK_JOIN_KW,
			Sqlite3.TK_THEN,
			Sqlite3.TK_END,
			Sqlite3.TK_DEFERRABLE,
			Sqlite3.TK_ELSE,
			Sqlite3.TK_EXCEPT,
			Sqlite3.TK_TRANSACTION,
			Sqlite3.TK_ACTION,
			Sqlite3.TK_ON,
			Sqlite3.TK_JOIN_KW,
			Sqlite3.TK_ALTER,
			Sqlite3.TK_RAISE,
			Sqlite3.TK_EXCLUSIVE,
			Sqlite3.TK_EXISTS,
			Sqlite3.TK_SAVEPOINT,
			Sqlite3.TK_INTERSECT,
			Sqlite3.TK_TRIGGER,
			Sqlite3.TK_REFERENCES,
			Sqlite3.TK_CONSTRAINT,
			Sqlite3.TK_INTO,
			Sqlite3.TK_OFFSET,
			Sqlite3.TK_OF,
			Sqlite3.TK_SET,
			Sqlite3.TK_TEMP,
			Sqlite3.TK_TEMP,
			Sqlite3.TK_OR,
			Sqlite3.TK_UNIQUE,
			Sqlite3.TK_QUERY,
			Sqlite3.TK_ATTACH,
			Sqlite3.TK_HAVING,
			Sqlite3.TK_GROUP,
			Sqlite3.TK_UPDATE,
			Sqlite3.TK_BEGIN,
			Sqlite3.TK_JOIN_KW,
			Sqlite3.TK_RELEASE,
			Sqlite3.TK_BETWEEN,
			Sqlite3.TK_NOTNULL,
			Sqlite3.TK_NOT,
			Sqlite3.TK_NO,
			Sqlite3.TK_NULL,
			Sqlite3.TK_LIKE_KW,
			Sqlite3.TK_CASCADE,
			Sqlite3.TK_ASC,
			Sqlite3.TK_DELETE,
			Sqlite3.TK_CASE,
			Sqlite3.TK_COLLATE,
			Sqlite3.TK_CREATE,
			Sqlite3.TK_CTIME_KW,
			Sqlite3.TK_DETACH,
			Sqlite3.TK_IMMEDIATE,
			Sqlite3.TK_JOIN,
			Sqlite3.TK_INSERT,
			Sqlite3.TK_MATCH,
			Sqlite3.TK_PLAN,
			Sqlite3.TK_ANALYZE,
			Sqlite3.TK_PRAGMA,
			Sqlite3.TK_ABORT,
			Sqlite3.TK_VALUES,
			Sqlite3.TK_VIRTUAL,
			Sqlite3.TK_LIMIT,
			Sqlite3.TK_WHEN,
			Sqlite3.TK_WHERE,
			Sqlite3.TK_RENAME,
			Sqlite3.TK_AFTER,
			Sqlite3.TK_REPLACE,
			Sqlite3.TK_AND,
			Sqlite3.TK_DEFAULT,
			Sqlite3.TK_AUTOINCR,
			Sqlite3.TK_TO,
			Sqlite3.TK_IN,
			Sqlite3.TK_CAST,
			Sqlite3.TK_COLUMNKW,
			Sqlite3.TK_COMMIT,
			Sqlite3.TK_CONFLICT,
			Sqlite3.TK_JOIN_KW,
			Sqlite3.TK_CTIME_KW,
			Sqlite3.TK_CTIME_KW,
			Sqlite3.TK_PRIMARY,
			Sqlite3.TK_DEFERRED,
			Sqlite3.TK_DISTINCT,
			Sqlite3.TK_IS,
			Sqlite3.TK_DROP,
			Sqlite3.TK_FAIL,
			Sqlite3.TK_FROM,
			Sqlite3.TK_JOIN_KW,
			Sqlite3.TK_LIKE_KW,
			Sqlite3.TK_BY,
			Sqlite3.TK_IF,
			Sqlite3.TK_ISNULL,
			Sqlite3.TK_ORDER,
			Sqlite3.TK_RESTRICT,
			Sqlite3.TK_JOIN_KW,
			Sqlite3.TK_JOIN_KW,
			Sqlite3.TK_ROLLBACK,
			Sqlite3.TK_ROW,
			Sqlite3.TK_UNION,
			Sqlite3.TK_USING,
			Sqlite3.TK_VACUUM,
			Sqlite3.TK_VIEW,
			Sqlite3.TK_INITIALLY,
			Sqlite3.TK_ALL,
		};

		public static TokenType keywordCode (string z, int iOffset, int n)
		{
			return (TokenType)innerKeywordCode (z, iOffset, n);
		}

		static int innerKeywordCode (string z, int iOffset, int n)
		{
			int h, i;
			if (n < 2)
				return Sqlite3.TK_ID;
			h = ((_Custom.sqlite3UpperToLower [z [iOffset + 0]]) * 4 ^ //(charMap(z[iOffset+0]) * 4) ^
			(_Custom.sqlite3UpperToLower [z [iOffset + n - 1]] * 3) ^ //(charMap(z[iOffset+n - 1]) * 3) ^
			n) % 127;
			for (i = (aHash [h]) - 1; i >= 0; i = (aNext [i]) - 1) {
				if (aLen [i] == n && String.Compare (zText, aOffset [i], z, iOffset, n, StringComparison.InvariantCultureIgnoreCase) == 0) {
					sqliteinth.testcase (i == 0);
					///
///<summary>
///REINDEX 
///</summary>

					sqliteinth.testcase (i == 1);
					///
///<summary>
///INDEXED 
///</summary>

					sqliteinth.testcase (i == 2);
					///
///<summary>
///INDEX 
///</summary>

                    sqliteinth.testcase(i == 3);
					///
///<summary>
///DESC 
///</summary>

					sqliteinth.testcase (i == 4);
					///
///<summary>
///ESCAPE 
///</summary>

					sqliteinth.testcase (i == 5);
					///
///<summary>
///EACH 
///</summary>

					sqliteinth.testcase (i == 6);
					///
///<summary>
///CHECK 
///</summary>

					sqliteinth.testcase (i == 7);
					///
///<summary>
///KEY 
///</summary>

					sqliteinth.testcase (i == 8);
					///
///<summary>
///BEFORE 
///</summary>

					sqliteinth.testcase (i == 9);
					///
///<summary>
///FOREIGN 
///</summary>

					sqliteinth.testcase (i == 10);
					///
///<summary>
///FOR 
///</summary>

					sqliteinth.testcase (i == 11);
					///
///<summary>
///IGNORE 
///</summary>

					sqliteinth.testcase (i == 12);
					///
///<summary>
///REGEXP 
///</summary>

					sqliteinth.testcase (i == 13);
					///
///<summary>
///EXPLAIN 
///</summary>

					sqliteinth.testcase (i == 14);
					///
///<summary>
///INSTEAD 
///</summary>

					sqliteinth.testcase (i == 15);
					///
///<summary>
///ADD 
///</summary>

					sqliteinth.testcase (i == 16);
					///
///<summary>
///DATABASE 
///</summary>

					sqliteinth.testcase (i == 17);
					///
///<summary>
///AS 
///</summary>

					sqliteinth.testcase (i == 18);
					///
///<summary>
///SELECT 
///</summary>

					sqliteinth.testcase (i == 19);
					///
///<summary>
///TABLE 
///</summary>

					sqliteinth.testcase (i == 20);
					///
///<summary>
///LEFT 
///</summary>

					sqliteinth.testcase (i == 21);
					///
///<summary>
///THEN 
///</summary>

					sqliteinth.testcase (i == 22);
					///
///<summary>
///END 
///</summary>

					sqliteinth.testcase (i == 23);
					///
///<summary>
///DEFERRABLE 
///</summary>

					sqliteinth.testcase (i == 24);
					///
///<summary>
///ELSE 
///</summary>

					sqliteinth.testcase (i == 25);
					///
///<summary>
///EXCEPT 
///</summary>

					sqliteinth.testcase (i == 26);
					///
///<summary>
///TRANSACTION 
///</summary>

					sqliteinth.testcase (i == 27);
					///
///<summary>
///ACTION 
///</summary>

					sqliteinth.testcase (i == 28);
					///
///<summary>
///ON 
///</summary>

					sqliteinth.testcase (i == 29);
					///
///<summary>
///NATURAL 
///</summary>

					sqliteinth.testcase (i == 30);
					///
///<summary>
///ALTER 
///</summary>

					sqliteinth.testcase (i == 31);
					///
///<summary>
///RAISE 
///</summary>

					sqliteinth.testcase (i == 32);
					///
///<summary>
///EXCLUSIVE 
///</summary>

					sqliteinth.testcase (i == 33);
					///
///<summary>
///EXISTS 
///</summary>

					sqliteinth.testcase (i == 34);
					///
///<summary>
///SAVEPOINT 
///</summary>

					sqliteinth.testcase (i == 35);
					///
///<summary>
///INTERSECT 
///</summary>

					sqliteinth.testcase (i == 36);
					///
///<summary>
///TRIGGER 
///</summary>

					sqliteinth.testcase (i == 37);
					///
///<summary>
///REFERENCES 
///</summary>

					sqliteinth.testcase (i == 38);
					///
///<summary>
///CONSTRAINT 
///</summary>

					sqliteinth.testcase (i == 39);
					///
///<summary>
///INTO 
///</summary>

					sqliteinth.testcase (i == 40);
					///
///<summary>
///OFFSET 
///</summary>

					sqliteinth.testcase (i == 41);
					///
///<summary>
///OF 
///</summary>

					sqliteinth.testcase (i == 42);
					///
///<summary>
///SET 
///</summary>

					sqliteinth.testcase (i == 43);
					///
///<summary>
///TEMPORARY 
///</summary>

					sqliteinth.testcase (i == 44);
					///
///<summary>
///TEMP 
///</summary>

					sqliteinth.testcase (i == 45);
					///
///<summary>
///OR 
///</summary>

					sqliteinth.testcase (i == 46);
					///
///<summary>
///UNIQUE 
///</summary>

					sqliteinth.testcase (i == 47);
					///
///<summary>
///QUERY 
///</summary>

					sqliteinth.testcase (i == 48);
					///
///<summary>
///ATTACH 
///</summary>

					sqliteinth.testcase (i == 49);
					///
///<summary>
///HAVING 
///</summary>

					sqliteinth.testcase (i == 50);
					///
///<summary>
///GROUP 
///</summary>

					sqliteinth.testcase (i == 51);
					///
///<summary>
///UPDATE 
///</summary>

					sqliteinth.testcase (i == 52);
					///
///<summary>
///BEGIN 
///</summary>

					sqliteinth.testcase (i == 53);
					///
///<summary>
///INNER 
///</summary>

					sqliteinth.testcase (i == 54);
					///
///<summary>
///RELEASE 
///</summary>

					sqliteinth.testcase (i == 55);
					///
///<summary>
///BETWEEN 
///</summary>

					sqliteinth.testcase (i == 56);
					///
///<summary>
///NOTNULL 
///</summary>

					sqliteinth.testcase (i == 57);
					///
///<summary>
///NOT 
///</summary>

					sqliteinth.testcase (i == 58);
					///
///<summary>
///NO 
///</summary>

					sqliteinth.testcase (i == 59);
					///
///<summary>
///NULL 
///</summary>

					sqliteinth.testcase (i == 60);
					///
///<summary>
///LIKE 
///</summary>

					sqliteinth.testcase (i == 61);
					///
///<summary>
///CASCADE 
///</summary>

					sqliteinth.testcase (i == 62);
					///
///<summary>
///ASC 
///</summary>

					sqliteinth.testcase (i == 63);
					///
///<summary>
///DELETE 
///</summary>

					sqliteinth.testcase (i == 64);
					///
///<summary>
///CASE 
///</summary>

					sqliteinth.testcase (i == 65);
					///
///<summary>
///COLLATE 
///</summary>

					sqliteinth.testcase (i == 66);
					///
///<summary>
///CREATE 
///</summary>

					sqliteinth.testcase (i == 67);
					///
///<summary>
///CURRENT_DATE 
///</summary>

					sqliteinth.testcase (i == 68);
					///
///<summary>
///DETACH 
///</summary>

					sqliteinth.testcase (i == 69);
					///
///<summary>
///IMMEDIATE 
///</summary>

					sqliteinth.testcase (i == 70);
					///
///<summary>
///JOIN 
///</summary>

					sqliteinth.testcase (i == 71);
					///
///<summary>
///INSERT 
///</summary>

					sqliteinth.testcase (i == 72);
					///
///<summary>
///MATCH 
///</summary>

					sqliteinth.testcase (i == 73);
					///
///<summary>
///PLAN 
///</summary>

					sqliteinth.testcase (i == 74);
					///
///<summary>
///ANALYZE 
///</summary>

					sqliteinth.testcase (i == 75);
					///
///<summary>
///PRAGMA 
///</summary>

					sqliteinth.testcase (i == 76);
					///
///<summary>
///ABORT 
///</summary>

					sqliteinth.testcase (i == 77);
					///
///<summary>
///VALUES 
///</summary>

					sqliteinth.testcase (i == 78);
					///
///<summary>
///VIRTUAL 
///</summary>

					sqliteinth.testcase (i == 79);
					///
///<summary>
///LIMIT 
///</summary>

					sqliteinth.testcase (i == 80);
					///
///<summary>
///WHEN 
///</summary>

					sqliteinth.testcase (i == 81);
					///
///<summary>
///WHERE 
///</summary>

					sqliteinth.testcase (i == 82);
					///
///<summary>
///RENAME 
///</summary>

					sqliteinth.testcase (i == 83);
					///
///<summary>
///AFTER 
///</summary>

					sqliteinth.testcase (i == 84);
					///
///<summary>
///REPLACE 
///</summary>

					sqliteinth.testcase (i == 85);
					///
///<summary>
///AND 
///</summary>

					sqliteinth.testcase (i == 86);
					///
///<summary>
///DEFAULT 
///</summary>

					sqliteinth.testcase (i == 87);
					///
///<summary>
///AUTOINCREMENT 
///</summary>

					sqliteinth.testcase (i == 88);
					///
///<summary>
///TO 
///</summary>

					sqliteinth.testcase (i == 89);
					///
///<summary>
///IN 
///</summary>

					sqliteinth.testcase (i == 90);
					///
///<summary>
///CAST 
///</summary>

					sqliteinth.testcase (i == 91);
					///
///<summary>
///COLUMN 
///</summary>

					sqliteinth.testcase (i == 92);
					///
///<summary>
///COMMIT 
///</summary>

					sqliteinth.testcase (i == 93);
					///
///<summary>
///CONFLICT 
///</summary>

					sqliteinth.testcase (i == 94);
					///
///<summary>
///CROSS 
///</summary>

					sqliteinth.testcase (i == 95);
					///
///<summary>
///CURRENT_TIMESTAMP 
///</summary>

					sqliteinth.testcase (i == 96);
					///
///<summary>
///CURRENT_TIME 
///</summary>

					sqliteinth.testcase (i == 97);
					///
///<summary>
///PRIMARY 
///</summary>

					sqliteinth.testcase (i == 98);
					///
///<summary>
///DEFERRED 
///</summary>

					sqliteinth.testcase (i == 99);
					///
///<summary>
///DISTINCT 
///</summary>

					sqliteinth.testcase (i == 100);
					///
///<summary>
///IS 
///</summary>

					sqliteinth.testcase (i == 101);
					///
///<summary>
///DROP 
///</summary>

					sqliteinth.testcase (i == 102);
					///
///<summary>
///FAIL 
///</summary>

					sqliteinth.testcase (i == 103);
					///
///<summary>
///FROM 
///</summary>

					sqliteinth.testcase (i == 104);
					///
///<summary>
///FULL 
///</summary>

					sqliteinth.testcase (i == 105);
					///
///<summary>
///GLOB 
///</summary>

					sqliteinth.testcase (i == 106);
					///
///<summary>
///BY 
///</summary>

					sqliteinth.testcase (i == 107);
					///
///<summary>
///IF 
///</summary>

					sqliteinth.testcase (i == 108);
					///
///<summary>
///ISNULL 
///</summary>

					sqliteinth.testcase (i == 109);
					///
///<summary>
///ORDER 
///</summary>

					sqliteinth.testcase (i == 110);
					///
///<summary>
///RESTRICT 
///</summary>

					sqliteinth.testcase (i == 111);
					///
///<summary>
///OUTER 
///</summary>

					sqliteinth.testcase (i == 112);
					///
///<summary>
///RIGHT 
///</summary>

					sqliteinth.testcase (i == 113);
					///
///<summary>
///ROLLBACK 
///</summary>

					sqliteinth.testcase (i == 114);
					///
///<summary>
///ROW 
///</summary>

					sqliteinth.testcase (i == 115);
					///
///<summary>
///UNION 
///</summary>

					sqliteinth.testcase (i == 116);
					///
///<summary>
///USING 
///</summary>

					sqliteinth.testcase (i == 117);
					///
///<summary>
///VACUUM 
///</summary>

					sqliteinth.testcase (i == 118);
					///
///<summary>
///VIEW 
///</summary>

					sqliteinth.testcase (i == 119);
					///
///<summary>
///INITIALLY 
///</summary>

					sqliteinth.testcase (i == 120);
					///
///<summary>
///ALL 
///</summary>

					return aCode [i];
				}
			}
			return Sqlite3.TK_ID;
		}

		public static int sqlite3KeywordCode (string z, int n)
		{
			return innerKeywordCode (z, 0, n);
		}

		public const int SQLITE_N_KEYWORD = 121;
	//#define SQLITE_N_KEYWORD 121
	}
}
