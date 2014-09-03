﻿using System;
using System.Diagnostics;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Text;
using Bitmask = System.UInt64;
using i16 = System.Int16;
using i64 = System.Int64;
using sqlite3_int64 = System.Int64;
using u8 = System.Byte;
using u16 = System.UInt16;
using u32 = System.UInt32;
using u64 = System.UInt64;
using unsigned = System.UInt64;
using Pgno = System.UInt32;

#if !SQLITE_MAX_VARIABLE_NUMBER
using ynVar = System.Int16;

#else
using ynVar = System.Int32; 
#endif
namespace Community.CsharpSqlite
{
    public partial class Sqlite3
    {
        ///
        ///<summary>
        ///Each database connection is an instance of the following structure.
        ///
        ///The sqlite.lastRowid records the last insert rowid generated by an
        ///insert statement.  Inserts on views do not affect its value.  Each
        ///trigger has its own context, so that lastRowid can be updated inside
        ///triggers as usual.  The previous value will be restored once the trigger
        ///exits.  Upon entering a before or instead of trigger, lastRowid is no
        ///</summary>
        ///<param name="longer (since after version 2.8.12) reset to ">1.</param>
        ///<param name=""></param>
        ///<param name="The sqlite.nChange does not count changes within triggers and keeps no">The sqlite.nChange does not count changes within triggers and keeps no</param>
        ///<param name="context.  It is reset at start of sqlite3_exec.">context.  It is reset at start of sqlite3_exec.</param>
        ///<param name="The sqlite.lsChange represents the number of changes made by the last">The sqlite.lsChange represents the number of changes made by the last</param>
        ///<param name="insert, update, or delete statement.  It remains constant throughout the">insert, update, or delete statement.  It remains constant throughout the</param>
        ///<param name="length of a statement and is then updated by OP_SetCounts.  It keeps a">length of a statement and is then updated by OP_SetCounts.  It keeps a</param>
        ///<param name="context stack just like lastRowid so that the count of changes">context stack just like lastRowid so that the count of changes</param>
        ///<param name="within a trigger is not seen outside the trigger.  Changes to views do not">within a trigger is not seen outside the trigger.  Changes to views do not</param>
        ///<param name="affect the value of lsChange.">affect the value of lsChange.</param>
        ///<param name="The sqlite.csChange keeps track of the number of current changes (since">The sqlite.csChange keeps track of the number of current changes (since</param>
        ///<param name="the last statement) and is used to update sqlite_lsChange.">the last statement) and is used to update sqlite_lsChange.</param>
        ///<param name=""></param>
        ///<param name="The member variables sqlite.errCode, sqlite.zErrMsg and sqlite.zErrMsg16">The member variables sqlite.errCode, sqlite.zErrMsg and sqlite.zErrMsg16</param>
        ///<param name="store the most recent error code and, if applicable, string. The">store the most recent error code and, if applicable, string. The</param>
        ///<param name="internal function sqlite3Error() is used to set these variables">internal function sqlite3Error() is used to set these variables</param>
        ///<param name="consistently.">consistently.</param>
        ///<param name=""></param>

        public class sqlite3
        {
            public sqlite3()
            {
            }

            public sqlite3_vfs pVfs;

            ///
            ///<summary>
            ///OS Interface 
            ///</summary>

            public int nDb;

            ///
            ///<summary>
            ///Number of backends currently in use 
            ///</summary>

            public Db[] aDb = new Db[SQLITE_MAX_ATTACHED];

            ///
            ///<summary>
            ///All backends 
            ///</summary>

            public int flags;

            ///
            ///<summary>
            ///Miscellaneous flags. See below 
            ///</summary>

            public int openFlags;

            ///
            ///<summary>
            ///Flags passed to sqlite3_vfs.xOpen() 
            ///</summary>

            public int errCode;

            ///
            ///<summary>
            ///Most recent error code (SQLITE_) 
            ///</summary>

            public int errMask;

            ///
            ///<summary>
            ///& result codes with this before returning 
            ///</summary>

            public u8 autoCommit;

            ///
            ///<summary>
            ///</summary>
            ///<param name="The auto">commit flag. </param>

            public u8 temp_store;

            ///
            ///<summary>
            ///1: file 2: memory 0: default 
            ///</summary>

            // Cannot happen under C#
            // public u8 mallocFailed;           /* True if we have seen a malloc failure */
            public u8 dfltLockMode;

            ///
            ///<summary>
            ///</summary>
            ///<param name="Default locking">mode for attached dbs </param>

            public int nextAutovac;

            ///
            ///<summary>
            ///Autovac setting after VACUUM if >=0 
            ///</summary>

            public u8 suppressErr;

            ///
            ///<summary>
            ///Do not issue error messages if true 
            ///</summary>

            public u8 vtabOnConflict;

            ///
            ///<summary>
            ///Value to return for s3_vtab_on_conflict() 
            ///</summary>

            public int nextPagesize;

            ///
            ///<summary>
            ///Pagesize after VACUUM if >0 
            ///</summary>

            public int nTable;

            ///
            ///<summary>
            ///Number of tables in the database 
            ///</summary>

            public CollSeq pDfltColl;

            ///
            ///<summary>
            ///The default collating sequence (BINARY) 
            ///</summary>

            public i64 lastRowid;

            ///
            ///<summary>
            ///ROWID of most recent insert (see above) 
            ///</summary>

            public u32 magic;

            ///
            ///<summary>
            ///Magic number for detect library misuse 
            ///</summary>

            public int nChange;

            ///
            ///<summary>
            ///Value returned by sqlite3_changes() 
            ///</summary>

            public int nTotalChange;

            ///
            ///<summary>
            ///Value returned by sqlite3_total_changes() 
            ///</summary>

            public sqlite3_mutex mutex;

            ///<summary>
            ///Connection mutex
            ///</summary>
            public int[] aLimit = new int[SQLITE_N_LIMIT];

            ///
            ///<summary>
            ///Limits 
            ///</summary>

            public class sqlite3InitInfo
            {
                ///
                ///<summary>
                ///Information used during initialization 
                ///</summary>

                public int iDb;

                ///
                ///<summary>
                ///When back is being initialized 
                ///</summary>

                public int newTnum;

                ///
                ///<summary>
                ///Rootpage of table being initialized 
                ///</summary>

                public u8 busy;

                ///
                ///<summary>
                ///TRUE if currently initializing 
                ///</summary>

                public u8 orphanTrigger;
                ///
                ///<summary>
                ///Last statement is orphaned TEMP trigger 
                ///</summary>

            };


            public sqlite3InitInfo init = new sqlite3InitInfo();

            public int nExtension;

            ///
            ///<summary>
            ///Number of loaded extensions 
            ///</summary>

            public object[] aExtension;

            ///
            ///<summary>
            ///Array of shared library handles 
            ///</summary>

            public Vdbe pVdbe;

            ///
            ///<summary>
            ///List of active virtual machines 
            ///</summary>

            public int activeVdbeCnt;

            ///
            ///<summary>
            ///Number of VDBEs currently executing 
            ///</summary>

            public int writeVdbeCnt;

            ///
            ///<summary>
            ///Number of active VDBEs that are writing 
            ///</summary>

            /// <summary>
            /// Number of nested calls to VdbeExec()
            /// </summary>
            public int callStackDepth;

            public dxTrace xTrace;

            //)(void*,const char);        /* Trace function */
            public object pTraceArg;

            ///
            ///<summary>
            ///Argument to the trace function 
            ///</summary>

            public dxProfile xProfile;

            //)(void*,const char*,u64);  /* Profiling function */
            public object pProfileArg;

            ///
            ///<summary>
            ///Argument to profile function 
            ///</summary>

            public object pCommitArg;

            ///
            ///<summary>
            ///Argument to xCommitCallback() 
            ///</summary>

            public dxCommitCallback xCommitCallback;

            //)(void);    /* Invoked at every commit. */
            public object pRollbackArg;

            ///
            ///<summary>
            ///Argument to xRollbackCallback() 
            ///</summary>

            public dxRollbackCallback xRollbackCallback;

            //)(void); /* Invoked at every commit. */
            public object pUpdateArg;

            public dxUpdateCallback xUpdateCallback;

            //)(void*,int, const char*,const char*,sqlite_int64);
#if !SQLITE_OMIT_WAL
																																																																																							//int (*xWalCallback)(void *, sqlite3 *, string , int);
//void *pWalArg;
#endif
            public dxCollNeeded xCollNeeded;

            //)(void*,sqlite3*,int eTextRep,const char);
            public dxCollNeeded xCollNeeded16;

            //)(void*,sqlite3*,int eTextRep,const void);
            public object pCollNeededArg;

            public Mem pErr;

            ///
            ///<summary>
            ///Most recent error message 
            ///</summary>

            public string zErrMsg;

            ///<summary>
            ///Most recent error message (UTF-8 encoded)
            ///</summary>
            public string zErrMsg16;

            ///
            ///<summary>
            ///</summary>
            ///<param name="Most recent error message (UTF">16 encoded) </param>

            public struct _u1
            {
                public bool isInterrupted;

                ///
                ///<summary>
                ///True if sqlite3_interrupt has been called 
                ///</summary>

                public double notUsed1;
                ///
                ///<summary>
                ///Spacer 
                ///</summary>

            }

            public _u1 u1;

            public Lookaside lookaside = new Lookaside();

            ///
            ///<summary>
            ///Lookaside malloc configuration 
            ///</summary>

#if !SQLITE_OMIT_AUTHORIZATION
																																																																																							public dxAuth xAuth;//)(void*,int,const char*,const char*,const char*,const char);
/* Access authorization function */
public object pAuthArg;               /* 1st argument to the access auth function */
#endif
#if !SQLITE_OMIT_PROGRESS_CALLBACK
            public dxProgress xProgress;

            //)(void );  /* The progress callback */
            public object pProgressArg;

            ///
            ///<summary>
            ///Argument to the progress callback 
            ///</summary>

            public int nProgressOps;

            ///
            ///<summary>
            ///Number of opcodes for progress callback 
            ///</summary>

#endif
#if !SQLITE_OMIT_VIRTUALTABLE
            public Hash aModule;

            ///
            ///<summary>
            ///populated by sqlite3_create_module() 
            ///</summary>

            public VtabCtx pVtabCtx;

            ///
            ///<summary>
            ///Context for active vtab connect/create 
            ///</summary>

            public VTable[] aVTrans;

            ///
            ///<summary>
            ///Virtual tables with open transactions 
            ///</summary>

            public int nVTrans;

            ///
            ///<summary>
            ///Allocated size of aVTrans 
            ///</summary>

            public VTable pDisconnect;

            ///
            ///<summary>
            ///Disconnect these in next sqlite3_prepare() 
            ///</summary>

#endif
            public FuncDefHash aFunc = new FuncDefHash();

            ///
            ///<summary>
            ///Hash table of connection functions 
            ///</summary>

            public Hash aCollSeq = new Hash();

            ///
            ///<summary>
            ///All collating sequences 
            ///</summary>

            public BusyHandler busyHandler = new BusyHandler();

            ///
            ///<summary>
            ///Busy callback 
            ///</summary>

            public int busyTimeout;

            ///
            ///<summary>
            ///Busy handler timeout, in msec 
            ///</summary>

            public Db[] aDbStatic = new Db[] {
				new Db (),
				new Db ()
			};

            ///
            ///<summary>
            ///Static space for the 2 default backends 
            ///</summary>

            public Savepoint pSavepoint;

            ///
            ///<summary>
            ///List of active savepoints 
            ///</summary>

            public int nSavepoint;

            ///
            ///<summary>
            ///</summary>
            ///<param name="Number of non">transaction savepoints </param>

            public int nStatement;

            ///
            ///<summary>
            ///</summary>
            ///<param name="Number of nested statement">transactions  </param>

            public u8 isTransactionSavepoint;

            ///
            ///<summary>
            ///True if the outermost savepoint is a TS 
            ///</summary>

            public i64 nDeferredCons;

            ///
            ///<summary>
            ///Net deferred constraints this transaction. 
            ///</summary>

            public int pnBytesFreed;

            ///
            ///<summary>
            ///If not NULL, increment this in DbFree() 
            ///</summary>

#if SQLITE_ENABLE_UNLOCK_NOTIFY
																																																																	/* The following variables are all protected by the STATIC_MASTER
** mutex, not by sqlite3.mutex. They are used by code in notify.c.
**
** When X.pUnlockConnection==Y, that means that X is waiting for Y to
** unlock so that it can proceed.
**
** When X.pBlockingConnection==Y, that means that something that X tried
** tried to do recently failed with an SQLITE_LOCKED error due to locks
** held by Y.
*/
sqlite3 *pBlockingConnection; /* Connection that caused SQLITE_LOCKED */
sqlite3 *pUnlockConnection;           /* Connection to watch for unlock */
void *pUnlockArg;                     /* Argument to xUnlockNotify */
void (*xUnlockNotify)(void **, int);  /* Unlock notify callback */
sqlite3 *pNextBlocked;        /* Next in list of all blocked connections */
#endif
            public void whereOrInfoDelete(WhereOrInfo p)
            {
                p.wc.whereClauseClear();
                this.sqlite3DbFree(ref p);
            }

            public void whereAndInfoDelete(WhereAndInfo p)
            {
                p.wc.whereClauseClear();
                this.sqlite3DbFree(ref p);
            }

            public string explainIndexRange(WhereLevel pLevel, Table pTab)
            {
                WherePlan pPlan = pLevel.plan;
                Index pIndex = pPlan.u.pIdx;
                uint nEq = pPlan.nEq;
                int i, j;
                Column[] aCol = pTab.aCol;
                int[] aiColumn = pIndex.aiColumn;
                StrAccum txt = new StrAccum(100);
                if (nEq == 0 && (pPlan.wsFlags & (WHERE_BTM_LIMIT | WHERE_TOP_LIMIT)) == 0)
                {
                    return null;
                }
                sqlite3StrAccumInit(txt, null, 0, SQLITE_MAX_LENGTH);
                txt.db = this;
                sqlite3StrAccumAppend(txt, " (", 2);
                for (i = 0; i < nEq; i++)
                {
                    txt.explainAppendTerm(i, aCol[aiColumn[i]].zName, "=");
                }
                j = i;
                if ((pPlan.wsFlags & WHERE_BTM_LIMIT) != 0)
                {
                    txt.explainAppendTerm(i++, aCol[aiColumn[j]].zName, ">");
                }
                if ((pPlan.wsFlags & WHERE_TOP_LIMIT) != 0)
                {
                    txt.explainAppendTerm(i, aCol[aiColumn[j]].zName, "<");
                }
                sqlite3StrAccumAppend(txt, ")", 1);
                return sqlite3StrAccumFinish(txt);
            }

            public void whereInfoFree(WhereInfo pWInfo)
            {
                if (ALWAYS(pWInfo != null))
                {
                    int i;
                    for (i = 0; i < pWInfo.nLevel; i++)
                    {
                        sqlite3_index_info pInfo = pWInfo.a[i] != null ? pWInfo.a[i].pIdxInfo : null;
                        if (pInfo != null)
                        {
                            ///
                            ///<summary>
                            ///Debug.Assert( pInfo.needToFreeIdxStr==0 || db.mallocFailed ); 
                            ///</summary>

                            if (pInfo.needToFreeIdxStr != 0)
                            {
                                //sqlite3_free( ref pInfo.idxStr );
                            }
                            this.sqlite3DbFree(ref pInfo);
                        }
                        if (pWInfo.a[i] != null && (pWInfo.a[i].plan.wsFlags & WHERE_TEMP_INDEX) != 0)
                        {
                            Index pIdx = pWInfo.a[i].plan.u.pIdx;
                            if (pIdx != null)
                            {
                                this.sqlite3DbFree(ref pIdx.zColAff);
                                this.sqlite3DbFree(ref pIdx);
                            }
                        }
                    }
                    pWInfo.pWC.whereClauseClear();
                    this.sqlite3DbFree(ref pWInfo);
                }
            }

            public void sqlite3DbFree(ref string pString)
            {
            }

            public void sqlite3DbFree<T>(ref T pT) where T : class
            {
            }

            public void sqlite3DbFree(ref Mem[] pPrior)
            {
                if (pPrior != null)
                    for (int i = 0; i < pPrior.Length; i++)
                        sqlite3MemFreeMem(ref pPrior[i]);
            }

            public void sqlite3DbFree(ref Mem pPrior)
            {
                if (pPrior != null)
                    sqlite3MemFreeMem(ref pPrior);
            }

            public void sqlite3DbFree(ref int[] pPrior)
            {
                if (pPrior != null)
                    sqlite3MemFreeInt(ref pPrior);
            }
        }
    }
}