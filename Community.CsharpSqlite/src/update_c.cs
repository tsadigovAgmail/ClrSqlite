using System;
using System.Diagnostics;
using u8=System.Byte;
using u32=System.UInt32;
namespace Community.CsharpSqlite {
	using sqlite3_value=Sqlite3.Mem;
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
    ** This file contains C code routines that are called by the parser
    ** to handle UPDATE statements.
    *************************************************************************
    **  Included in SQLite3 port to C#-SQLite;  2008 Noah B Hart
    **  C#-SQLite is an independent reimplementation of the SQLite software library
    **
    **  SQLITE_SOURCE_ID: 2011-06-23 19:49:22 4374b7e83ea0a3fbc3691f9c0c936272862f32f2
    **
    *************************************************************************
    *///#include "sqliteInt.h"
		#if !SQLITE_OMIT_VIRTUALTABLE
		///<summary>
		///Forward declaration
		///</summary>
		//static void updateVirtualTable(
		//Parse pParse,       /* The parsing context */
		//SrcList pSrc,       /* The virtual table to be modified */
		//Table pTab,         /* The virtual table */
		//ExprList pChanges,  /* The columns to change in the UPDATE statement */
		//Expr pRowidExpr,    /* Expression used to recompute the rowid */
		//int aXRef,          /* Mapping from columns of pTab to entries in pChanges */
		//Expr *pWhere,        /* WHERE clause of the UPDATE statement */
		//int onError          /* ON CONFLICT strategy */
		//);
		#endif
		///<summary>
		/// The most recently coded instruction was an OP_Column to retrieve the
		/// i-th column of table pTab. This routine sets the P4 parameter of the
		/// OP_Column to the default value, if any.
		///
		/// The default value of a column is specified by a DEFAULT clause in the
		/// column definition. This was either supplied by the user when the table
		/// was created, or added later to the table definition by an ALTER TABLE
		/// command. If the latter, then the row-records in the table btree on disk
		/// may not contain a value for the column and the default value, taken
		/// from the P4 parameter of the OP_Column instruction, is returned instead.
		/// If the former, then all row-records are guaranteed to include a value
		/// for the column and the P4 value is not required.
		///
		/// Column definitions created by an ALTER TABLE command may only have
		/// literal default values specified: a number, null or a string. (If a more
		/// complicated default expression value was provided, it is evaluated
		/// when the ALTER TABLE is executed and one of the literal values written
		/// into the sqlite_master table.)
		///
		/// Therefore, the P4 parameter is only required if the default value for
		/// the column is a literal number, string or null. The sqlite3ValueFromExpr()
		/// function is capable of transforming these types of expressions into
		/// sqlite3_value objects.
		///
		/// If parameter iReg is not negative, code an OP_RealAffinity instruction
		/// on register iReg. This is used when an equivalent integer value is
		/// stored in place of an 8-byte floating point value in order to save
		/// space.
		///</summary>
		static void sqlite3ColumnDefault(Vdbe v,Table pTab,int i,int iReg) {
			Debug.Assert(pTab!=null);
			if(null==pTab.pSelect) {
				sqlite3_value pValue=new sqlite3_value();
				SqliteEncoding enc=ENC(sqlite3VdbeDb(v));
				Column pCol=pTab.aCol[i];
				#if SQLITE_DEBUG
																																																																																        VdbeComment( v, "%s.%s", pTab.zName, pCol.zName );
#endif
				Debug.Assert(i<pTab.nCol);
				sqlite3ValueFromExpr(sqlite3VdbeDb(v),pCol.pDflt,enc,pCol.affinity,ref pValue);
				if(pValue!=null) {
					v.sqlite3VdbeChangeP4(-1,pValue,P4_MEM);
				}
				#if !SQLITE_OMIT_FLOATING_POINT
				if(iReg>=0&&pTab.aCol[i].affinity==SQLITE_AFF_REAL) {
					v.sqlite3VdbeAddOp1(OP_RealAffinity,iReg);
				}
				#endif
			}
		}
		/*
    ** Process an UPDATE statement.
    **
    **   UPDATE OR IGNORE table_wxyz SET a=b, c=d WHERE e<5 AND f NOT NULL;
    **          \_______/ \________/     \______/       \________________/
    *            onError   pTabList      pChanges             pWhere
    */static void sqlite3Update(Parse pParse,/* The parser context */SrcList pTabList,/* The table in which we should change things */ExprList pChanges,/* Things to be changed */Expr pWhere,/* The WHERE clause.  May be null */int onError/* How to handle constraint errors */) {
			int i,j;
			/* Loop counters */Table pTab;
			/* The table to be updated */int addr=0;
			/* VDBE instruction address of the start of the loop */WhereInfo pWInfo;
			/* Information about the WHERE clause */Vdbe v;
			/* The virtual database engine */Index pIdx;
			/* For looping over indices */int nIdx;
			/* Number of indices that need updating */int iCur;
			/* VDBE Cursor number of pTab */sqlite3 db;
			/* The database structure */int[] aRegIdx=null;
			/* One register assigned to each index to be updated */int[] aXRef=null;
			/* aXRef[i] is the index in pChanges.a[] of the
** an expression for the i-th column of the table.
** aXRef[i]==-1 if the i-th column is not changed. */bool chngRowid;
			/* True if the record number is being changed */Expr pRowidExpr=null;
			/* Expression defining the new record number */bool openAll=false;
			/* True if all indices need to be opened */AuthContext sContext;
			/* The authorization context */NameContext sNC;
			/* The name-context to resolve expressions in */int iDb;
			/* Database containing the table being updated */bool okOnePass;
			/* True for one-pass algorithm without the FIFO */bool hasFK;
			/* True if foreign key processing is required */
			#if !SQLITE_OMIT_TRIGGER
			bool isView;
			/* True when updating a view (INSTEAD OF trigger) */Trigger pTrigger;
			/* List of triggers on pTab, if required */int tmask=0;
			/* Mask of TRIGGER_BEFORE|TRIGGER_AFTER */
			#endif
			int newmask;
			/* Mask of NEW.* columns accessed by BEFORE triggers *//* Register Allocations */int regRowCount=0;
			/* A count of rows changed */int regOldRowid;
			/* The old rowid */int regNewRowid;
			/* The new rowid */int regNew;
			int regOld=0;
			int regRowSet=0;
			/* Rowset of rows to be updated */sContext=new AuthContext();
			//memset( &sContext, 0, sizeof( sContext ) );
			db=pParse.db;
			if(pParse.nErr!=0/*|| db.mallocFailed != 0 */) {
				goto update_cleanup;
			}
			Debug.Assert(pTabList.nSrc==1);
			/* Locate the table which we want to update.
      */pTab=sqlite3SrcListLookup(pParse,pTabList);
			if(pTab==null)
				goto update_cleanup;
			iDb=sqlite3SchemaToIndex(pParse.db,pTab.pSchema);
			/* Figure out if we have any triggers and if the table being
      ** updated is a view.
      */
			#if !SQLITE_OMIT_TRIGGER
			pTrigger=sqlite3TriggersExist(pParse,pTab,TK_UPDATE,pChanges,out tmask);
			isView=pTab.pSelect!=null;
			Debug.Assert(pTrigger!=null||tmask==0);
			#else
																																																												      const Trigger pTrigger = null;// define pTrigger 0
      const int tmask = 0;          // define tmask 0
#endif
			#if SQLITE_OMIT_TRIGGER || SQLITE_OMIT_VIEW
																																																												//     undef isView
      const bool isView = false;    // define isView 0
#endif
			if(sqlite3ViewGetColumnNames(pParse,pTab)!=0) {
				goto update_cleanup;
			}
			if(sqlite3IsReadOnly(pParse,pTab,tmask)) {
				goto update_cleanup;
			}
			aXRef=new int[pTab.nCol];
			// sqlite3DbMallocRaw(db, sizeof(int) * pTab.nCol);
			//if ( aXRef == null ) goto update_cleanup;
			for(i=0;i<pTab.nCol;i++)
				aXRef[i]=-1;
			/* Allocate a cursors for the main database table and for all indices.
      ** The index cursors might not be used, but if they are used they
      ** need to occur right after the database cursor.  So go ahead and
      ** allocate enough space, just in case.
      */pTabList.a[0].iCursor=iCur=pParse.nTab++;
			for(pIdx=pTab.pIndex;pIdx!=null;pIdx=pIdx.pNext) {
				pParse.nTab++;
			}
			/* Initialize the name-context */sNC=new NameContext();
			// memset(&sNC, 0, sNC).Length;
			sNC.pParse=pParse;
			sNC.pSrcList=pTabList;
			/* Resolve the column names in all the expressions of the
      ** of the UPDATE statement.  Also find the column index
      ** for each column to be updated in the pChanges array.  For each
      ** column to be updated, make sure we have authorization to change
      ** that column.
      */chngRowid=false;
			for(i=0;i<pChanges.nExpr;i++) {
				if(sqlite3ResolveExprNames(sNC,ref pChanges.a[i].pExpr)!=0) {
					goto update_cleanup;
				}
				for(j=0;j<pTab.nCol;j++) {
					if(pTab.aCol[j].zName.Equals(pChanges.a[i].zName,StringComparison.InvariantCultureIgnoreCase)) {
						if(j==pTab.iPKey) {
							chngRowid=true;
							pRowidExpr=pChanges.a[i].pExpr;
						}
						aXRef[j]=i;
						break;
					}
				}
				if(j>=pTab.nCol) {
					if(sqlite3IsRowid(pChanges.a[i].zName)) {
						chngRowid=true;
						pRowidExpr=pChanges.a[i].pExpr;
					}
					else {
						sqlite3ErrorMsg(pParse,"no such column: %s",pChanges.a[i].zName);
						pParse.checkSchema=1;
						goto update_cleanup;
					}
				}
				#if !SQLITE_OMIT_AUTHORIZATION
																																																																																{
int rc;
rc = sqlite3AuthCheck(pParse, SQLITE_UPDATE, pTab.zName,
pTab.aCol[j].zName, db.aDb[iDb].zName);
if( rc==SQLITE_DENY ){
goto update_cleanup;
}else if( rc==SQLITE_IGNORE ){
aXRef[j] = -1;
}
}
#endif
			}
			hasFK=pParse.sqlite3FkRequired(pTab,aXRef,chngRowid?1:0)!=0;
			/* Allocate memory for the array aRegIdx[].  There is one entry in the
      ** array for each index associated with table being updated.  Fill in
      ** the value with a register number for indices that are to be used
      ** and with zero for unused indices.
      */for(nIdx=0,pIdx=pTab.pIndex;pIdx!=null;pIdx=pIdx.pNext,nIdx++) {
			}
			if(nIdx>0) {
				aRegIdx=new int[nIdx];
				// sqlite3DbMallocRaw(db, Index*.Length * nIdx);
				if(aRegIdx==null)
					goto update_cleanup;
			}
			for(j=0,pIdx=pTab.pIndex;pIdx!=null;pIdx=pIdx.pNext,j++) {
				int reg;
				if(hasFK||chngRowid) {
					reg=++pParse.nMem;
				}
				else {
					reg=0;
					for(i=0;i<pIdx.nColumn;i++) {
						if(aXRef[pIdx.aiColumn[i]]>=0) {
							reg=++pParse.nMem;
							break;
						}
					}
				}
				aRegIdx[j]=reg;
			}
			/* Begin generating code. */v=sqlite3GetVdbe(pParse);
			if(v==null)
				goto update_cleanup;
			if(pParse.nested==0)
				sqlite3VdbeCountChanges(v);
			sqlite3BeginWriteOperation(pParse,1,iDb);
			#if !SQLITE_OMIT_VIRTUALTABLE
			/* Virtual tables must be handled separately */if(IsVirtual(pTab)) {
				updateVirtualTable(pParse,pTabList,pTab,pChanges,pRowidExpr,aXRef,pWhere,onError);
				pWhere=null;
				pTabList=null;
				goto update_cleanup;
			}
			#endif
			/* Allocate required registers. */regOldRowid=regNewRowid=++pParse.nMem;
			if(pTrigger!=null||hasFK) {
				regOld=pParse.nMem+1;
				pParse.nMem+=pTab.nCol;
			}
			if(chngRowid||pTrigger!=null||hasFK) {
				regNewRowid=++pParse.nMem;
			}
			regNew=pParse.nMem+1;
			pParse.nMem+=pTab.nCol;
			/* Start the view context. */if(isView) {
				sqlite3AuthContextPush(pParse,sContext,pTab.zName);
			}
			/* If we are trying to update a view, realize that view into
      ** a ephemeral table.
      */
			#if !(SQLITE_OMIT_VIEW) && !(SQLITE_OMIT_TRIGGER)
			if(isView) {
				sqlite3MaterializeView(pParse,pTab,pWhere,iCur);
			}
			#endif
			/* Resolve the column names in all the expressions in the
** WHERE clause.
*/if(sqlite3ResolveExprNames(sNC,ref pWhere)!=0) {
				goto update_cleanup;
			}
			/* Begin the database scan
      */v.sqlite3VdbeAddOp2(OP_Null,0,regOldRowid);
			ExprList NullOrderby=null;
			pWInfo=sqlite3WhereBegin(pParse,pTabList,pWhere,ref NullOrderby,WHERE_ONEPASS_DESIRED);
			if(pWInfo==null)
				goto update_cleanup;
			okOnePass=pWInfo.okOnePass!=0;
			/* Remember the rowid of every item to be updated.
      */v.sqlite3VdbeAddOp2(OP_Rowid,iCur,regOldRowid);
			if(!okOnePass) {
				regRowSet=++pParse.nMem;
				v.sqlite3VdbeAddOp2(OP_RowSetAdd,regRowSet,regOldRowid);
			}
			/* End the database scan loop.
      */sqlite3WhereEnd(pWInfo);
			/* Initialize the count of updated rows
      */if((db.flags&SQLITE_CountRows)!=0&&null==pParse.pTriggerTab) {
				regRowCount=++pParse.nMem;
				v.sqlite3VdbeAddOp2(OP_Integer,0,regRowCount);
			}
			if(!isView) {
				/*
        ** Open every index that needs updating.  Note that if any
        ** index could potentially invoke a REPLACE conflict resolution
        ** action, then we need to open all indices because we might need
        ** to be deleting some records.
        */if(!okOnePass)
					pParse.sqlite3OpenTable(iCur,iDb,pTab,OP_OpenWrite);
				if(onError==OE_Replace) {
					openAll=true;
				}
				else {
					openAll=false;
					for(pIdx=pTab.pIndex;pIdx!=null;pIdx=pIdx.pNext) {
						if(pIdx.onError==OE_Replace) {
							openAll=true;
							break;
						}
					}
				}
				for(i=0,pIdx=pTab.pIndex;pIdx!=null;pIdx=pIdx.pNext,i++) {
					if(openAll||aRegIdx[i]>0) {
						KeyInfo pKey=sqlite3IndexKeyinfo(pParse,pIdx);
						v.sqlite3VdbeAddOp4(OP_OpenWrite,iCur+i+1,pIdx.tnum,iDb,pKey,P4_KEYINFO_HANDOFF);
						Debug.Assert(pParse.nTab>iCur+i+1);
					}
				}
			}
			/* Top of the update loop */if(okOnePass) {
				int a1=v.sqlite3VdbeAddOp1(OP_NotNull,regOldRowid);
				addr=v.sqlite3VdbeAddOp0(OP_Goto);
				v.sqlite3VdbeJumpHere(a1);
			}
			else {
				addr=v.sqlite3VdbeAddOp3(OP_RowSetRead,regRowSet,0,regOldRowid);
			}
			/* Make cursor iCur point to the record that is being updated. If
      ** this record does not exist for some reason (deleted by a trigger,
      ** for example, then jump to the next iteration of the RowSet loop.  */v.sqlite3VdbeAddOp3(OP_NotExists,iCur,addr,regOldRowid);
			/* If the record number will change, set register regNewRowid to
      ** contain the new value. If the record number is not being modified,
      ** then regNewRowid is the same register as regOldRowid, which is
      ** already populated.  */Debug.Assert(chngRowid||pTrigger!=null||hasFK||regOldRowid==regNewRowid);
			if(chngRowid) {
				sqlite3ExprCode(pParse,pRowidExpr,regNewRowid);
				v.sqlite3VdbeAddOp1(OP_MustBeInt,regNewRowid);
			}
			/* If there are triggers on this table, populate an array of registers 
      ** with the required old.* column data.  */if(hasFK||pTrigger!=null) {
				u32 oldmask=(hasFK?pParse.sqlite3FkOldmask(pTab):0);
				oldmask|=sqlite3TriggerColmask(pParse,pTrigger,pChanges,0,TRIGGER_BEFORE|TRIGGER_AFTER,pTab,onError);
				for(i=0;i<pTab.nCol;i++) {
					if(aXRef[i]<0||oldmask==0xffffffff||(i<32&&0!=(oldmask&(1<<i)))) {
						sqlite3ExprCodeGetColumnOfTable(v,pTab,iCur,i,regOld+i);
					}
					else {
						v.sqlite3VdbeAddOp2(OP_Null,0,regOld+i);
					}
				}
				if(chngRowid==false) {
					v.sqlite3VdbeAddOp2(OP_Copy,regOldRowid,regNewRowid);
				}
			}
			/* Populate the array of registers beginning at regNew with the new
      ** row data. This array is used to check constaints, create the new
      ** table and index records, and as the values for any new.* references
      ** made by triggers.
      **
      ** If there are one or more BEFORE triggers, then do not populate the
      ** registers associated with columns that are (a) not modified by
      ** this UPDATE statement and (b) not accessed by new.* references. The
      ** values for registers not modified by the UPDATE must be reloaded from 
      ** the database after the BEFORE triggers are fired anyway (as the trigger 
      ** may have modified them). So not loading those that are not going to
      ** be used eliminates some redundant opcodes.
      */newmask=(int)sqlite3TriggerColmask(pParse,pTrigger,pChanges,1,TRIGGER_BEFORE,pTab,onError);
			for(i=0;i<pTab.nCol;i++) {
				if(i==pTab.iPKey) {
					v.sqlite3VdbeAddOp2(OP_Null,0,regNew+i);
				}
				else {
					j=aXRef[i];
					if(j>=0) {
						sqlite3ExprCode(pParse,pChanges.a[j].pExpr,regNew+i);
					}
					else
						if(0==(tmask&TRIGGER_BEFORE)||i>31||(newmask&(1<<i))!=0) {
							/* This branch loads the value of a column that will not be changed 
            ** into a register. This is done if there are no BEFORE triggers, or
            ** if there are one or more BEFORE triggers that use this value via
            ** a new.* reference in a trigger program.
            */testcase(i==31);
							testcase(i==32);
							v.sqlite3VdbeAddOp3(OP_Column,iCur,i,regNew+i);
							sqlite3ColumnDefault(v,pTab,i,regNew+i);
						}
				}
			}
			/* Fire any BEFORE UPDATE triggers. This happens before constraints are
      ** verified. One could argue that this is wrong.
      */if((tmask&TRIGGER_BEFORE)!=0) {
				v.sqlite3VdbeAddOp2(OP_Affinity,regNew,pTab.nCol);
				v.sqlite3TableAffinityStr(pTab);
				sqlite3CodeRowTrigger(pParse,pTrigger,TK_UPDATE,pChanges,TRIGGER_BEFORE,pTab,regOldRowid,onError,addr);
				/* The row-trigger may have deleted the row being updated. In this
        ** case, jump to the next row. No updates or AFTER triggers are 
        ** required. This behaviour - what happens when the row being updated
        ** is deleted or renamed by a BEFORE trigger - is left undefined in the
        ** documentation.
        */v.sqlite3VdbeAddOp3(OP_NotExists,iCur,addr,regOldRowid);
				/* If it did not delete it, the row-trigger may still have modified 
        ** some of the columns of the row being updated. Load the values for 
        ** all columns not modified by the update statement into their 
        ** registers in case this has happened.
        */for(i=0;i<pTab.nCol;i++) {
					if(aXRef[i]<0&&i!=pTab.iPKey) {
						v.sqlite3VdbeAddOp3(OP_Column,iCur,i,regNew+i);
						sqlite3ColumnDefault(v,pTab,i,regNew+i);
					}
				}
			}
			if(!isView) {
				int j1;
				/* Address of jump instruction *//* Do constraint checks. */int iDummy;
				pParse.sqlite3GenerateConstraintChecks(pTab,iCur,regNewRowid,aRegIdx,(chngRowid?regOldRowid:0),true,onError,addr,out iDummy);
				/* Do FK constraint checks. */if(hasFK) {
					pParse.sqlite3FkCheck(pTab,regOldRowid,0);
				}
				/* Delete the index entries associated with the current record.  */j1=v.sqlite3VdbeAddOp3(OP_NotExists,iCur,0,regOldRowid);
				sqlite3GenerateRowIndexDelete(pParse,pTab,iCur,aRegIdx);
				/* If changing the record number, delete the old record.  */if(hasFK||chngRowid) {
					v.sqlite3VdbeAddOp2(OP_Delete,iCur,0);
				}
				v.sqlite3VdbeJumpHere(j1);
				if(hasFK) {
					pParse.sqlite3FkCheck(pTab,0,regNewRowid);
				}
				/* Insert the new index entries and the new record. */pParse.sqlite3CompleteInsertion(pTab,iCur,regNewRowid,aRegIdx,true,false,false);
				/* Do any ON CASCADE, SET NULL or SET DEFAULT operations required to
        ** handle rows (possibly in other tables) that refer via a foreign key
        ** to the row just updated. */if(hasFK) {
					pParse.sqlite3FkActions(pTab,pChanges,regOldRowid);
				}
			}
			/* Increment the row counter 
      */if((db.flags&SQLITE_CountRows)!=0&&null==pParse.pTriggerTab) {
				v.sqlite3VdbeAddOp2(OP_AddImm,regRowCount,1);
			}
			sqlite3CodeRowTrigger(pParse,pTrigger,TK_UPDATE,pChanges,TRIGGER_AFTER,pTab,regOldRowid,onError,addr);
			/* Repeat the above with the next record to be updated, until
      ** all record selected by the WHERE clause have been updated.
      */v.sqlite3VdbeAddOp2(OP_Goto,0,addr);
			v.sqlite3VdbeJumpHere(addr);
			/* Close all tables */for(i=0,pIdx=pTab.pIndex;pIdx!=null;pIdx=pIdx.pNext,i++) {
				if(openAll||aRegIdx[i]>0) {
					v.sqlite3VdbeAddOp2(OP_Close,iCur+i+1,0);
				}
			}
			v.sqlite3VdbeAddOp2(OP_Close,iCur,0);
			/* Update the sqlite_sequence table by storing the content of the
      ** maximum rowid counter values recorded while inserting into
      ** autoincrement tables.
      */if(pParse.nested==0&&pParse.pTriggerTab==null) {
				pParse.sqlite3AutoincrementEnd();
			}
			/*
      ** Return the number of rows that were changed. If this routine is 
      ** generating code because of a call to sqlite3NestedParse(), do not
      ** invoke the callback function.
      */if((db.flags&SQLITE_CountRows)!=0&&null==pParse.pTriggerTab&&0==pParse.nested) {
				v.sqlite3VdbeAddOp2(OP_ResultRow,regRowCount,1);
				sqlite3VdbeSetNumCols(v,1);
				sqlite3VdbeSetColName(v,0,COLNAME_NAME,"rows updated",SQLITE_STATIC);
			}
			update_cleanup:
			#if !SQLITE_OMIT_AUTHORIZATION
																																																												sqlite3AuthContextPop(sContext);
#endif
			sqlite3DbFree(db,ref aRegIdx);
			sqlite3DbFree(db,ref aXRef);
			sqlite3SrcListDelete(db,ref pTabList);
			sqlite3ExprListDelete(db,ref pChanges);
			sqlite3ExprDelete(db,ref pWhere);
			return;
		}
		///<summary>
		///Make sure "isView" and other macros defined above are undefined. Otherwise
		/// thely may interfere with compilation of other functions in this file
		/// (or in another file, if this file becomes part of the amalgamation).
		///</summary>
		//#if isView
		// #undef isView
		//#endif
		//#if pTrigger
		// #undef pTrigger
		//#endif
		#if !SQLITE_OMIT_VIRTUALTABLE
		/*
** Generate code for an UPDATE of a virtual table.
**
** The strategy is that we create an ephemerial table that contains
** for each row to be changed:
**
**   (A)  The original rowid of that row.
**   (B)  The revised rowid for the row. (note1)
**   (C)  The content of every column in the row.
**
** Then we loop over this ephemeral table and for each row in
** the ephermeral table call VUpdate.
**
** When finished, drop the ephemeral table.
**
** (note1) Actually, if we know in advance that (A) is always the same
** as (B) we only store (A), then duplicate (A) when pulling
** it out of the ephemeral table before calling VUpdate.
*/static void updateVirtualTable(Parse pParse,/* The parsing context */SrcList pSrc,/* The virtual table to be modified */Table pTab,/* The virtual table */ExprList pChanges,/* The columns to change in the UPDATE statement */Expr pRowid,/* Expression used to recompute the rowid */int[] aXRef,/* Mapping from columns of pTab to entries in pChanges */Expr pWhere,/* WHERE clause of the UPDATE statement */int onError/* ON CONFLICT strategy */) {
			Vdbe v=pParse.pVdbe;
			/* Virtual machine under construction */ExprList pEList=null;
			/* The result set of the SELECT statement */Select pSelect=null;
			/* The SELECT statement */Expr pExpr;
			/* Temporary expression */int ephemTab;
			/* Table holding the result of the SELECT */int i;
			/* Loop counter */int addr;
			/* Address of top of loop */int iReg;
			/* First register in set passed to OP_VUpdate */sqlite3 db=pParse.db;
			/* Database connection */VTable pVTab=sqlite3GetVTable(db,pTab);
			SelectDest dest=new SelectDest();
			/* Construct the SELECT statement that will find the new values for
      ** all updated rows.
      */pEList=sqlite3ExprListAppend(pParse,0,sqlite3Expr(db,TK_ID,"_rowid_"));
			if(pRowid!=null) {
				pEList=sqlite3ExprListAppend(pParse,pEList,sqlite3ExprDup(db,pRowid,0));
			}
			Debug.Assert(pTab.iPKey<0);
			for(i=0;i<pTab.nCol;i++) {
				if(aXRef[i]>=0) {
					pExpr=sqlite3ExprDup(db,pChanges.a[aXRef[i]].pExpr,0);
				}
				else {
					pExpr=sqlite3Expr(db,TK_ID,pTab.aCol[i].zName);
				}
				pEList=sqlite3ExprListAppend(pParse,pEList,pExpr);
			}
			pSelect=sqlite3SelectNew(pParse,pEList,pSrc,pWhere,null,null,null,0,null,null);
			/* Create the ephemeral table into which the update results will
      ** be stored.
      */Debug.Assert(v!=null);
			ephemTab=pParse.nTab++;
			v.sqlite3VdbeAddOp2(OP_OpenEphemeral,ephemTab,pTab.nCol+1+((pRowid!=null)?1:0));
			v.sqlite3VdbeChangeP5(BTREE_UNORDERED);
			/* fill the ephemeral table
      */sqlite3SelectDestInit(dest,SelectResultType.Table,ephemTab);
			sqlite3Select(pParse,pSelect,ref dest);
			/* Generate code to scan the ephemeral table and call VUpdate. */iReg=++pParse.nMem;
			pParse.nMem+=pTab.nCol+1;
			addr=v.sqlite3VdbeAddOp2(OP_Rewind,ephemTab,0);
			v.sqlite3VdbeAddOp3(OP_Column,ephemTab,0,iReg);
			v.sqlite3VdbeAddOp3(OP_Column,ephemTab,(pRowid!=null?1:0),iReg+1);
			for(i=0;i<pTab.nCol;i++) {
				v.sqlite3VdbeAddOp3(OP_Column,ephemTab,i+1+((pRowid!=null)?1:0),iReg+2+i);
			}
			sqlite3VtabMakeWritable(pParse,pTab);
			v.sqlite3VdbeAddOp4(OP_VUpdate,0,pTab.nCol+2,iReg,pVTab,P4_VTAB);
			v.sqlite3VdbeChangeP5((byte)(onError==OE_Default?OE_Abort:onError));
			sqlite3MayAbort(pParse);
			v.sqlite3VdbeAddOp2(OP_Next,ephemTab,addr+1);
			v.sqlite3VdbeJumpHere(addr);
			v.sqlite3VdbeAddOp2(OP_Close,ephemTab,0);
			/* Cleanup */sqlite3SelectDelete(db,ref pSelect);
		}
	#endif
	}
}
