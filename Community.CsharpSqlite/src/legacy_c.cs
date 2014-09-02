using System;
using System.Diagnostics;
using System.Text;
namespace Community.CsharpSqlite {
	using sqlite3_callback=Sqlite3.dxCallback;
	using sqlite3_stmt=Sqlite3.Vdbe;
	public partial class Sqlite3 {
		/*
    ** 2001 September 15
    **
    ** The author disclaims copyright to this source code.  In place of
    ** a legal notice, here is a blessing:
    **
    **    May you do good and not evil.
    **    May you find forgiveness for yourself and forgive others.
    **    May you share freely, never taking more than you give.
    **
    *************************************************************************
    ** Main file for the SQLite library.  The routines in this file
    ** implement the programmer interface to the library.  Routines in
    ** other files are for internal use by SQLite and should not be
    ** accessed by users of the library.
    *************************************************************************
    **  Included in SQLite3 port to C#-SQLite;  2008 Noah B Hart
    **  C#-SQLite is an independent reimplementation of the SQLite software library
    **
    **  SQLITE_SOURCE_ID: 2010-08-23 18:52:01 42537b60566f288167f1b5864a5435986838e3a3
    **
    *************************************************************************
    *///#include "sqliteInt.h"
		///<summary>
		/// Execute SQL code.  Return one of the SQLITE_ success/failure
		/// codes.  Also write an error message into memory obtained from
		/// malloc() and make pzErrMsg point to that message.
		///
		/// If the SQL is a query, then for each row in the query result
		/// the xCallback() function is called.  pArg becomes the first
		/// argument to xCallback().  If xCallback=NULL then no callback
		/// is invoked, even for queries.
		///
		///</summary>
		//C# Alias
		static public int exec(sqlite3 db,/* The database on which the SQL executes */string zSql,/* The SQL to be executed */int NoCallback,int NoArgs,int NoErrors) {
			string Errors="";
			return sqlite3_exec(db,zSql,null,null,ref Errors);
		}
		static public int exec(sqlite3 db,/* The database on which the SQL executes */string zSql,/* The SQL to be executed */sqlite3_callback xCallback,/* Invoke this callback routine */object pArg,/* First argument to xCallback() */int NoErrors) {
			string Errors="";
			return sqlite3_exec(db,zSql,xCallback,pArg,ref Errors);
		}
		static public int exec(sqlite3 db,/* The database on which the SQL executes */string zSql,/* The SQL to be executed */sqlite3_callback xCallback,/* Invoke this callback routine */object pArg,/* First argument to xCallback() */ref string pzErrMsg/* Write error messages here */) {
			return sqlite3_exec(db,zSql,xCallback,pArg,ref pzErrMsg);
		}
		//OVERLOADS 
		static public int sqlite3_exec(sqlite3 db,/* The database on which the SQL executes */string zSql,/* The SQL to be executed */int NoCallback,int NoArgs,int NoErrors) {
			string Errors="";
			return sqlite3_exec(db,zSql,null,null,ref Errors);
		}
		static public int sqlite3_exec(sqlite3 db,/* The database on which the SQL executes */string zSql,/* The SQL to be executed */sqlite3_callback xCallback,/* Invoke this callback routine */object pArg,/* First argument to xCallback() */int NoErrors) {
			string Errors="";
			return sqlite3_exec(db,zSql,xCallback,pArg,ref Errors);
		}
        static public int sqlite3_exec(sqlite3 db,/* The database on which the SQL executes */string zSql,/* The SQL to be executed */sqlite3_callback xCallback,/* Invoke this callback routine */object pArg,/* First argument to xCallback() */ref string pzErrMsg/* Write error messages here */)
        {
            SqlResult result = SqlResult.SQLITE_OK;
			/* Return code */string zLeftover="";
			/* Tail of unprocessed SQL */sqlite3_stmt pStmt=null;
			/* The current SQL statement */string[] azCols=null;
			/* Names of result columns */int nRetry=0;
			/* Number of retry attempts */int callbackIsInit;
			/* True if callback data is initialized */if(!sqlite3SafetyCheckOk(db))
				return SQLITE_MISUSE_BKPT();
			if(zSql==null)
				zSql="";
			sqlite3_mutex_enter(db.mutex);
			sqlite3Error(db,SQLITE_OK,0);
            while ((result == SqlResult.SQLITE_OK || (result == SqlResult.SQLITE_SCHEMA && (++nRetry) < 2)) && zSql != "")
            {
				int nCol;
				string[] azVals=null;
				pStmt=null;
                result = (SqlResult)sqlite3_prepare(db, zSql, -1, ref pStmt, ref zLeftover);
				Debug.Assert(result==SQLITE_OK||pStmt==null);
				if(result!=SQLITE_OK) {
					continue;
				}
				if(pStmt==null) {
					/* this happens for a comment or white-space */zSql=zLeftover;
					continue;
				}
				callbackIsInit=0;
				nCol=sqlite3_column_count(pStmt);
				while(true) {
					int i;
					result=sqlite3_step(pStmt);
                    /* Invoke the callback function if required */
                    if (xCallback != null && (SqlResult.SQLITE_ROW == result || (SqlResult.SQLITE_DONE == result && callbackIsInit == 0 && (db.flags & SQLITE_NullCallback) != 0)))
                    {
						if(0==callbackIsInit) {
							azCols=new string[nCol];
							//sqlite3DbMallocZero(db, 2*nCol*sizeof(const char*) + 1);
							//if ( azCols == null )
							//{
							//  goto exec_out;
							//}
							for(i=0;i<nCol;i++) {
								azCols[i]=sqlite3_column_name(pStmt,i);
								/* sqlite3VdbeSetColName() installs column names as UTF8
                ** strings so there is no way for sqlite3_column_name() to fail. */Debug.Assert(azCols[i]!=null);
							}
							callbackIsInit=1;
						}
                        if (result == SqlResult.SQLITE_ROW)
                        {
							azVals=new string[nCol];
							// azCols[nCol];
							for(i=0;i<nCol;i++) {
								azVals[i]=sqlite3_column_text(pStmt,i);
								if(azVals[i]==null&&sqlite3_column_type(pStmt,i)!=SQLITE_NULL) {
									//db.mallocFailed = 1;
									//goto exec_out;
								}
							}
						}
						if(xCallback(pArg,nCol,azVals,azCols)!=0) {
                            result = SqlResult.SQLITE_ABORT;
							sqlite3VdbeFinalize(ref pStmt);
							pStmt=null;
							sqlite3Error(db,SQLITE_ABORT,0);
							goto exec_out;
						}
					}
                    if (result != SqlResult.SQLITE_ROW)
                    {
                        result = (SqlResult)sqlite3VdbeFinalize(ref pStmt);
						pStmt=null;
                        if (result != SqlResult.SQLITE_SCHEMA)
                        {
							nRetry=0;
							if((zSql=zLeftover)!="") {
								int zindex=0;
								while(zindex<zSql.Length&&CharExtensions.sqlite3Isspace(zSql[zindex]))
									zindex++;
								if(zindex!=0)
									zSql=zindex<zSql.Length?zSql.Substring(zindex):"";
							}
						}
						break;
					}
				}
				db.sqlite3DbFree(ref azCols);
				azCols=null;
			}
			exec_out:
			if(pStmt!=null)
				sqlite3VdbeFinalize(ref pStmt);
			db.sqlite3DbFree(ref azCols);
            result = (SqlResult)sqlite3ApiExit(db, (int)result);
            if (result != SqlResult.SQLITE_OK && ALWAYS(result == (SqlResult)sqlite3_errcode(db)) && pzErrMsg != null)
            {
				//int nErrMsg = 1 + StringExtensions.sqlite3Strlen30(sqlite3_errmsg(db));
				//pzErrMsg = sqlite3Malloc(nErrMsg);
				//if (pzErrMsg)
				//{
				//   memcpy(pzErrMsg, sqlite3_errmsg(db), nErrMsg);
				//}else{
				//rc = SQLITE_NOMEM;
				//sqlite3Error(db, SQLITE_NOMEM, 0);
				//}
				pzErrMsg=sqlite3_errmsg(db);
			}
			else
				if(pzErrMsg!="") {
					pzErrMsg="";
				}
            Debug.Assert((result & (SqlResult)db.errMask) == result);
			sqlite3_mutex_leave(db.mutex);
			return (int)result;
		}
	}
}
