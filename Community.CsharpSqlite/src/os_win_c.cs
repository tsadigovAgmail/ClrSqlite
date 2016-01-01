#define SQLITE_OS_WIN
using Community.CsharpSqlite.Os;
using Community.CsharpSqlite.Utils;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using DWORD = System.UInt64;
using HANDLE = System.IntPtr;
using i64 = System.Int64;
using sqlite3_int64 = System.Int64;
using u32 = System.UInt32;
using u8 = System.Byte;

#if WINDOWS_PHONE || SQLITE_SILVERLIGHT
using System.IO.IsolatedStorage;
#endif
namespace Community.CsharpSqlite
{
	public partial class Sqlite3
	{
		///
///<summary>
///2004 May 22
///
///The author disclaims copyright to this source code.  In place of
///a legal notice, here is a blessing:
///
///May you do good and not evil.
///May you find forgiveness for yourself and forgive others.
///May you share freely, never taking more than you give.
///
///
///
///This file contains code that is specific to windows.
///
///</summary>
///<param name="Included in SQLite3 port to C#">SQLite;  2008 Noah B Hart</param>
///<param name="C#">SQLite is an independent reimplementation of the SQLite software library</param>
///<param name=""></param>
///<param name="SQLITE_SOURCE_ID: 2011">23 19:49:22 4374b7e83ea0a3fbc3691f9c0c936272862f32f2</param>
///<param name=""></param>
///<param name=""></param>
///<param name=""></param>

		//#include "sqliteInt.h"
		#if SQLITE_OS_WIN
		///
///<summary>
///A Note About Memory Allocation:
///
///This driver uses malloc()/free() directly rather than going through
///</summary>
///<param name="the SQLite">wrappers malloc_cs.sqlite3Malloc()/sqlite3DbFree(db,ref  ).  Those wrappers</param>
///<param name="are designed for use on embedded systems where memory is scarce and">are designed for use on embedded systems where memory is scarce and</param>
///<param name="malloc failures happen frequently.  Win32 does not typically run on">malloc failures happen frequently.  Win32 does not typically run on</param>
///<param name="embedded systems, and when it does the developers normally have bigger">embedded systems, and when it does the developers normally have bigger</param>
///<param name="problems to worry about than running out of memory.  So there is not">problems to worry about than running out of memory.  So there is not</param>
///<param name="a compelling need to use the wrappers.">a compelling need to use the wrappers.</param>
///<param name=""></param>
///<param name="But there is a good reason to not use the wrappers.  If we use the">But there is a good reason to not use the wrappers.  If we use the</param>
///<param name="wrappers then we will get simulated malloc() failures within this">wrappers then we will get simulated malloc() failures within this</param>
///<param name="driver.  And that causes all kinds of problems for our tests.  We">driver.  And that causes all kinds of problems for our tests.  We</param>
///<param name="could enhance SQLite to deal with simulated malloc failures within">could enhance SQLite to deal with simulated malloc failures within</param>
///<param name="the OS driver, but the code to deal with those failure would not">the OS driver, but the code to deal with those failure would not</param>
///<param name="be exercised on Linux (which does not need to malloc() in the driver)">be exercised on Linux (which does not need to malloc() in the driver)</param>
///<param name="and so we would have difficulty writing coverage tests for that">and so we would have difficulty writing coverage tests for that</param>
///<param name="code.  Better to leave the code out, we think.">code.  Better to leave the code out, we think.</param>
///<param name=""></param>
///<param name="The point of this discussion is as follows:  When creating a new">The point of this discussion is as follows:  When creating a new</param>
///<param name="OS layer for an embedded system, if you use this file as an example,">OS layer for an embedded system, if you use this file as an example,</param>
///<param name="avoid the use of malloc()/free().  Those routines work ok on windows">avoid the use of malloc()/free().  Those routines work ok on windows</param>
///<param name="desktops but not so well in embedded systems.">desktops but not so well in embedded systems.</param>

		//#include <winbase.h>
		#if __CYGWIN__
																																						// include <sys/cygwin.h>
#endif
		///
///<summary>
///Macros used to determine whether or not to use threads.
///</summary>

		#if THREADSAFE
																																						// define SQLITE_W32_THREADS 1
#endif
		///
///<summary>
///Include code that is common to all os_*.c files
///</summary>

		//#include "os_common.h"
		///
///<summary>
///Some microsoft compilers lack this definition.
///
///</summary>

		#if !INVALID_FILE_ATTRIBUTES
		//# define INVALID_FILE_ATTRIBUTES ((DWORD)-1)
		const int INVALID_FILE_ATTRIBUTES = -1;

		#endif
		///
///<summary>
///</summary>
///<param name="Determine if we are dealing with WindowsCE "> which has a much</param>
///<param name="reduced API.">reduced API.</param>

		#if SQLITE_OS_WINCE
																																						// define AreFileApisANSI() 1
// define GetDiskFreeSpaceW() 0
#endif
		///
///<summary>
///Forward references 
///</summary>

		//typedef struct winShm winShm;           /* A connection to shared-memory */
		//typedef struct winShmNode winShmNode;   /* A region of shared-memory */
		///
///<summary>
///WinCE lacks native support for file locking so we have to fake it
///with some code of our own.
///
///</summary>

		#if SQLITE_OS_WINCE
																																						typedef struct winceLock {
int nReaders;       /* Number of reader locks obtained */
BOOL bPending;      /* Indicates a pending lock has been obtained */
BOOL bReserved;     /* Indicates a reserved lock has been obtained */
BOOL bExclusive;    /* Indicates an exclusive lock has been obtained */
} winceLock;
#endif
		private static LockingStrategy lockingStrategy = HelperMethods.IsRunningMediumTrust () ? new MediumTrustLockingStrategy () : new LockingStrategy ();



		///
///<summary>
///Forward prototypes.
///
///</summary>

		//static int getSectorSize(
		//    sqlite3_vfs *pVfs,
		//    string zRelative     /* UTF-8 file name */
		//);
		///
///<summary>
///The following variable is (normally) set once and never changes
///thereafter.  It records whether the operating system is Win95
///or WinNT.
///
///0:   Operating system unknown.
///1:   Operating system is Win95.
///2:   Operating system is WinNT.
///
///In order to facilitate testing on a WinNT system, the test fixture
///can manually set this value to 1 to emulate Win98 behavior.
///
///</summary>

		#if SQLITE_TEST
																																						    int sqlite3_os_type = 0;
#else
		static int sqlite3_os_type = 0;

		#endif
		///<summary>
		/// Return true (non-zero) if we are running under WinNT, Win2K, WinXP,
		/// or WinCE.  Return false (zero) for Win95, Win98, or WinME.
		///
		/// Here is an interesting observation:  Win95, Win98, and WinME lack
		/// the LockFileEx() API.  But we can still statically link against that
		/// API as long as we don't call it when running Win95/98/ME.  A call to
		/// this routine is used to determine if the host is Win95/98/ME or
		/// WinNT/2K/XP so that we will know whether or not we can safely call
		/// the LockFileEx() API.
		///</summary>
		#if SQLITE_OS_WINCE
																																						// define isNT()  (1)
#else
		static bool isNT ()
		{
			//if (sqlite3_os_type == 0)
			//{
			//  OSVERSIONINFO sInfo;
			//  sInfo.dwOSVersionInfoSize = sInfo.Length;
			//  GetVersionEx(&sInfo);
			//  sqlite3_os_type = sInfo.dwPlatformId == VER_PLATFORM_WIN32_NT ? 2 : 1;
			//}
			//return sqlite3_os_type == 2;
			return Environment.OSVersion.Platform >= PlatformID.Win32NT;
		}

		#endif
		///
///<summary>
///</summary>
///<param name="Convert a UTF">16?).</param>
///<param name=""></param>
///<param name="Space to hold the returned string is obtained from malloc.">Space to hold the returned string is obtained from malloc.</param>

		//static WCHAR *utf8ToUnicode(string zFilename){
		//  int nChar;
		//  Wstring zWideFilename;
		//  nChar = MultiByteToWideChar(CP_UTF8, 0, zFilename, -1, NULL, 0);
		//  zWideFilename = malloc( nChar*sizeof(zWideFilename[0]) );
		//  if( zWideFilename==0 ){
		//    return 0;
		//  }
		//  nChar = MultiByteToWideChar(CP_UTF8, 0, zFilename, -1, zWideFilename, nChar);
		//  if( nChar==0 ){
		//    free(zWideFilename);
		//    zWideFileName = "";
		//  }
		//  return zWideFilename;
		//}
		///
///<summary>
///</summary>
///<param name="Convert microsoft unicode to UTF">8.  Space to hold the returned string is</param>
///<param name="obtained from malloc().">obtained from malloc().</param>
///<param name=""></param>

		//static char *unicodeToUtf8(const Wstring zWideFilename){
		//  int nByte;
		//  string zFilename;
		//  nByte = WideCharToMultiByte(CP_UTF8, 0, zWideFilename, -1, 0, 0, 0, 0);
		//  zFilename = malloc( nByte );
		//  if( zFilename==0 ){
		//    return 0;
		//  }
		//  nByte = WideCharToMultiByte(CP_UTF8, 0, zWideFilename, -1, zFilename, nByte,
		//                              0, 0);
		//  if( nByte == 0 ){
		//    free(zFilename);
		//    zFileName = "";
		//  }
		//  return zFilename;
		//}
		///
///<summary>
///Convert an ansi string to microsoft unicode, based on the
///current codepage settings for file apis.
///
///Space to hold the returned string is obtained
///from malloc.
///
///</summary>

		//static WCHAR *mbcsToUnicode(string zFilename){
		//  int nByte;
		//  Wstring zMbcsFilename;
		//  int codepage = AreFileApisANSI() ? CP_ACP : CP_OEMCP;
		//  nByte = MultiByteToWideChar(codepage, 0, zFilename, -1, NULL,0)*WCHAR.Length;
		//  zMbcsFilename = malloc( nByte*sizeof(zMbcsFilename[0]) );
		//  if( zMbcsFilename==0 ){
		//    return 0;
		//  }
		//  nByte = MultiByteToWideChar(codepage, 0, zFilename, -1, zMbcsFilename, nByte);
		//  if( nByte==0 ){
		//    free(zMbcsFilename);
		//    zMbcsFileName = "";
		//  }
		//  return zMbcsFilename;
		//}
		///
///<summary>
///Convert microsoft unicode to multibyte character string, based on the
///user's Ansi codepage.
///
///Space to hold the returned string is obtained from
///malloc().
///
///</summary>

		//static char *unicodeToMbcs(const Wstring zWideFilename){
		//  int nByte;
		//  string zFilename;
		//  int codepage = AreFileApisANSI() ? CP_ACP : CP_OEMCP;
		//  nByte = WideCharToMultiByte(codepage, 0, zWideFilename, -1, 0, 0, 0, 0);
		//  zFilename = malloc( nByte );
		//  if( zFilename==0 ){
		//    return 0;
		//  }
		//  nByte = WideCharToMultiByte(codepage, 0, zWideFilename, -1, zFilename, nByte,
		//                              0, 0);
		//  if( nByte == 0 ){
		//    free(zFilename);
		//    zFileName = "";
		//  }
		//  return zFilename;
		//}
		///
///<summary>
///</summary>
///<param name="Convert multibyte character string to UTF">8.  Space to hold the</param>
///<param name="returned string is obtained from malloc().">returned string is obtained from malloc().</param>
///<param name=""></param>

		//static char *sqlite3_win32_mbcs_to_utf8(string zFilename){
		//  string zFilenameUtf8;
		//  Wstring zTmpWide;
		//  zTmpWide = mbcsToUnicode(zFilename);
		//  if( zTmpWide==0 ){
		//    return 0;
		//  }
		//  zFilenameUtf8 = unicodeToUtf8(zTmpWide);
		//  free(zTmpWide);
		//  return zFilenameUtf8;
		//}
		///<summary>
		/// Convert UTF-8 to multibyte character string.  Space to hold the
		/// returned string is obtained from malloc().
		///
		///</summary>
		//char *sqlite3_win32_utf8_to_mbcs(string zFilename){
		//  string zFilenameMbcs;
		//  Wstring zTmpWide;
		//  zTmpWide = utf8ToUnicode(zFilename);
		//  if( zTmpWide==0 ){
		//    return 0;
		//  }
		//  zFilenameMbcs = unicodeToMbcs(zTmpWide);
		//  free(zTmpWide);
		//  return zFilenameMbcs;
		//}
		///
///<summary>
///The return value of getLastErrorMsg
///</summary>
///<param name="is zero if the error message fits in the buffer, or non">zero</param>
///<param name="otherwise (if the message was truncated).">otherwise (if the message was truncated).</param>

		static int getLastErrorMsg (int nBuf, ref string zBuf)
		{
			///
///<summary>
///FormatMessage returns 0 on failure.  Otherwise it
///returns the number of TCHARs written to the output
///buffer, excluding the terminating null char.
///
///</summary>

			//DWORD error = GetLastError();
			//DWORD dwLen = 0;
			//string zOut = "";
			//if( isNT() ){
			//Wstring zTempWide = NULL;
			//dwLen = FormatMessageW(FORMAT_MESSAGE_ALLOCATE_BUFFER | FORMAT_MESSAGE_FROM_SYSTEM | FORMAT_MESSAGE_IGNORE_INSERTS,
			//                       NULL,
			//                       error,
			//                       0,
			//                       (LPWSTR) &zTempWide,
			//                       0,
			//                       0);
			#if SQLITE_SILVERLIGHT
																																																									      zBuf = "Unknown error";
#else
			zBuf = Marshal.GetLastWin32Error ().ToString ();
			//new Win32Exception( Marshal.GetLastWin32Error() ).Message;
			#endif
			//if( dwLen > 0 ){
			//  /* allocate a buffer and convert to UTF8 */
			//  zOut = unicodeToUtf8(zTempWide);
			//  /* free the system buffer allocated by FormatMessage */
			//  LocalFree(zTempWide);
			//}
			///
///<summary>
///isNT() is 1 if SQLITE_OS_WINCE==1, so this else is never executed. 
///Since the ASCII version of these Windows API do not exist for WINCE,
///it's important to not reference them for WINCE builds.
///</summary>

			//#if !SQLITE_OS_WINCE //==0
			//  }else{
			//    string zTemp = null;
			//    dwLen = FormatMessageA(FORMAT_MESSAGE_ALLOCATE_BUFFER | FORMAT_MESSAGE_FROM_SYSTEM | FORMAT_MESSAGE_IGNORE_INSERTS,
			//                           null,
			//                           error,
			//                           0,
			//                           ref zTemp,
			//                           0,
			//                           0);
			//    if( dwLen > 0 ){
			//      /* allocate a buffer and convert to UTF8 */
			//      zOut = sqlite3_win32_mbcs_to_utf8(zTemp);
			//      /* free the system buffer allocated by FormatMessage */
			//      LocalFree(zTemp);
			//    }
			//#endif
			//  }
			//if( 0 == dwLen ){
			//  io.sqlite3_snprintf(nBuf, zBuf, "OsError 0x%x (%u)", error, error);
			//}else{
			//  /* copy a maximum of nBuf chars to output buffer */
			//  io.sqlite3_snprintf(nBuf, zBuf, "%s", zOut);
			//  /* free the UTF8 buffer */
			//  free(zOut);
			//}
			return 0;
		}

		///<summary>
		///
		/// This function - winLogErrorAtLine() - is only ever called via the macro
		/// winLogError().
		///
		/// This routine is invoked after an error occurs in an OS function.
		/// It logs a message using io.sqlite3_log() containing the current value of
		/// error code and, if possible, the human-readable equivalent from
		/// FormatMessage.
		///
		/// The first argument passed to the macro should be the error code that
		/// will be returned to SQLite (e.g. SQLITE_IOERR_DELETE, SQLITE_CANTOPEN).
		/// The two subsequent arguments should be the name of the OS function that
		/// failed and the the associated file-system path, if any.
		///</summary>
		//#define winLogError(a,b,c)     winLogErrorAtLine(a,b,c,__LINE__)
		static SqlResult  winLogError (SqlResult a, string b, string c)
		{
			StackTrace st = new StackTrace (new StackFrame (true));
			StackFrame sf = st.GetFrame (0);
			return winLogErrorAtLine (a, b, c, sf.GetFileLineNumber ());
		}

		static SqlResult winLogErrorAtLine (SqlResult errcode, ///
///<summary>
///SQLite error code 
///</summary>

		string zFunc, ///
///<summary>
///Name of OS function that failed 
///</summary>

		string zPath, ///
///<summary>
///File path associated with error 
///</summary>

		int iLine///
///<summary>
///Source line number where error occurred 
///</summary>

		)
		{
			string zMsg = null;
			///
///<summary>
///Human readable error text 
///</summary>

			int i;
			///
///<summary>
///Loop counter 
///</summary>

			DWORD iErrno;
			// = GetLastError();  /* Error code */
			#if SQLITE_SILVERLIGHT
																																																									      iErrno = (int)ERROR_NOT_SUPPORTED;
#else
			iErrno = (u32)Marshal.GetLastWin32Error ();
			#endif
			//zMsg[0] = 0;
			getLastErrorMsg (500, ref zMsg);
			Debug.Assert (errcode != SqlResult.SQLITE_OK);
			if (zPath == null)
				zPath = "";
			for (i = 0; i < zMsg.Length && zMsg [i] != '\r' && zMsg [i] != '\n'; i++) {
			}
			zMsg = zMsg.Substring (0, i);
			io.sqlite3_log (errcode, "os_win.c:%d: (%d) %s(%s) - %s", iLine, iErrno, zFunc, zPath, zMsg);
			return errcode;
		}

		#if SQLITE_OS_WINCE
																																						/*************************************************************************
** This section contains code for WinCE only.
*/
/*
** WindowsCE does not have a localtime() function.  So create a
** substitute.
*/
//include <time.h>
struct tm *__cdecl localtime(const time_t *t)
{
static struct tm y;
FILETIME uTm, lTm;
SYSTEMTIME pTm;
sqlite3_int64 t64;
t64 = *t;
t64 = (t64 + 11644473600)*10000000;
uTm.dwLowDateTime = (DWORD)(t64 & 0xFFFFFFFF);
uTm.dwHighDateTime= (DWORD)(t64 >> 32);
FileTimeToLocalFileTime(&uTm,&lTm);
FileTimeToSystemTime(&lTm,&pTm);
y.tm_year = pTm.wYear - 1900;
y.tm_mon = pTm.wMonth - 1;
y.tm_wday = pTm.wDayOfWeek;
y.tm_mday = pTm.wDay;
y.tm_hour = pTm.wHour;
y.tm_min = pTm.wMinute;
y.tm_sec = pTm.wSecond;
return &y;
}

/* This will never be called, but defined to make the code compile */
//define GetTempPathA(a,b)

//define LockFile(a,b,c,d,e)       winceLockFile(&a, b, c, d, e)
//define UnlockFile(a,b,c,d,e)     winceUnlockFile(&a, b, c, d, e)
//define LockFileEx(a,b,c,d,e,f)   winceLockFileEx(&a, b, c, d, e, f)

//define HANDLE_TO_WINFILE(a) (winFile)&((char)a)[-(int)offsetof(winFile,h)]

/*
** Acquire a lock on the handle h
*/
static void winceMutexAcquire(HANDLE h){
DWORD dwErr;
do {
dwErr = WaitForSingleObject(h, INFINITE);
} while (dwErr != WAIT_OBJECT_0 && dwErr != WAIT_ABANDONED);
}
/*
** Release a lock acquired by winceMutexAcquire()
*/
//define winceMutexRelease(h) ReleaseMutex(h)

/*
** Create the mutex and shared memory used for locking in the file
** descriptor pFile
*/
static BOOL winceCreateLock(string zFilename, sqlite3_file pFile){
Wstring zTok;
Wstring zName = utf8ToUnicode(zFilename);
BOOL bInit = TRUE;

/* Initialize the local lockdata */
ZeroMemory(pFile.local, pFile.local).Length;

/* Replace the backslashes from the filename and lowercase it
** to derive a mutex name. */
zTok = CharLowerW(zName);
for (;*zTok;zTok++){
if (*zTok == '\\') *zTok = '_';
}

/* Create/open the named mutex */
pFile.hMutex = CreateMutexW(NULL, FALSE, zName);
if (!pFile.hMutex){
pFile.lastErrno = (u32)GetLastError();
winLogError(SqlResult.SQLITE_ERROR, "winceCreateLock1", zFilename);
free(zName);
return FALSE;
}

/* Acquire the mutex before continuing */
winceMutexAcquire(pFile.hMutex);

/* Since the names of named mutexes, semaphores, file mappings etc are
** case-sensitive, take advantage of that by uppercasing the mutex name
** and using that as the shared filemapping name.
*/
CharUpperW(zName);
pFile.hShared = CreateFileMappingW(INVALID_HANDLE_VALUE, NULL,
PAGE_READWRITE, 0, winceLock.Length,
zName);

/* Set a flag that indicates we're the first to create the memory so it
** must be zero-initialized */
if (GetLastError() == ERROR_ALREADY_EXISTS){
bInit = FALSE;
}

free(zName);

/* If we succeeded in making the shared memory handle, map it. */
if (pFile.hShared){
pFile.shared = (winceLock)MapViewOfFile(pFile.hShared,
FILE_MAP_READ|FILE_MAP_WRITE, 0, 0, winceLock).Length;
/* If mapping failed, close the shared memory handle and erase it */
if (!pFile.shared){
pFile.lastErrno = (u32)GetLastError();
winLogError(SqlResult.SQLITE_ERROR, "winceCreateLock2", zFilename);
CloseHandle(pFile.hShared);
pFile.hShared = NULL;
}
}

/* If shared memory could not be created, then close the mutex and fail */
if (pFile.hShared == NULL){
winceMutexRelease(pFile.hMutex);
CloseHandle(pFile.hMutex);
pFile.hMutex = NULL;
return FALSE;
}

/* Initialize the shared memory if we're supposed to */
if (bInit) {
ZeroMemory(pFile.shared, winceLock).Length;
}

winceMutexRelease(pFile.hMutex);
return TRUE;
}

/*
** Destroy the part of sqlite3_file that deals with wince locks
*/
static void winceDestroyLock(sqlite3_file pFile){
if (pFile.hMutex){
/* Acquire the mutex */
winceMutexAcquire(pFile.hMutex);

/* The following blocks should probably Debug.Assert in debug mode, but they
are to cleanup in case any locks remained open */
if (pFile.local.nReaders){
pFile.shared.nReaders --;
}
if (pFile.local.bReserved){
pFile.shared.bReserved = FALSE;
}
if (pFile.local.bPending){
pFile.shared.bPending = FALSE;
}
if (pFile.local.bExclusive){
pFile.shared.bExclusive = FALSE;
}

/* De-reference and close our copy of the shared memory handle */
UnmapViewOfFile(pFile.shared);
CloseHandle(pFile.hShared);

/* Done with the mutex */
winceMutexRelease(pFile.hMutex);
CloseHandle(pFile.hMutex);
pFile.hMutex = NULL;
}
}

/*
** An implementation of the LockFile() API of windows for wince
*/
static BOOL winceLockFile(
HANDLE *phFile,
DWORD dwFileOffsetLow,
DWORD dwFileOffsetHigh,
DWORD nNumberOfBytesToLockLow,
DWORD nNumberOfBytesToLockHigh
){
winFile *pFile = HANDLE_TO_WINFILE(phFile);
BOOL bReturn = FALSE;

sqliteinth.UNUSED_PARAMETER(dwFileOffsetHigh);
sqliteinth.UNUSED_PARAMETER(nNumberOfBytesToLockHigh);

if (!pFile.hMutex) return TRUE;
winceMutexAcquire(pFile.hMutex);

/* Wanting an exclusive lock? */
if (dwFileOffsetLow == (DWORD)SHARED_FIRST
&& nNumberOfBytesToLockLow == (DWORD)SHARED_SIZE){
if (pFile.shared.nReaders == 0 && pFile.shared.bExclusive == 0){
pFile.shared.bExclusive = TRUE;
pFile.local.bExclusive = TRUE;
bReturn = TRUE;
}
}

/* Want a read-only lock? */
else if (dwFileOffsetLow == (DWORD)SHARED_FIRST &&
nNumberOfBytesToLockLow == 1){
if (pFile.shared.bExclusive == 0){
pFile.local.nReaders ++;
if (pFile.local.nReaders == 1){
pFile.shared.nReaders ++;
}
bReturn = TRUE;
}
}

/* Want a pending lock? */
else if (dwFileOffsetLow == (DWORD)PENDING_BYTE && nNumberOfBytesToLockLow == 1){
/* If no pending lock has been acquired, then acquire it */
if (pFile.shared.bPending == 0) {
pFile.shared.bPending = TRUE;
pFile.local.bPending = TRUE;
bReturn = TRUE;
}
}

/* Want a reserved lock? */
else if (dwFileOffsetLow == (DWORD)RESERVED_BYTE && nNumberOfBytesToLockLow == 1){
if (pFile.shared.bReserved == 0) {
pFile.shared.bReserved = TRUE;
pFile.local.bReserved = TRUE;
bReturn = TRUE;
}
}

winceMutexRelease(pFile.hMutex);
return bReturn;
}

/*
** An implementation of the UnlockFile API of windows for wince
*/
static BOOL winceUnlockFile(
HANDLE *phFile,
DWORD dwFileOffsetLow,
DWORD dwFileOffsetHigh,
DWORD nNumberOfBytesToUnlockLow,
DWORD nNumberOfBytesToUnlockHigh
){
winFile *pFile = HANDLE_TO_WINFILE(phFile);
BOOL bReturn = FALSE;

sqliteinth.UNUSED_PARAMETER(dwFileOffsetHigh);
sqliteinth.UNUSED_PARAMETER(nNumberOfBytesToUnlockHigh);

if (!pFile.hMutex) return TRUE;
winceMutexAcquire(pFile.hMutex);

/* Releasing a reader lock or an exclusive lock */
if (dwFileOffsetLow == (DWORD)SHARED_FIRST){
/* Did we have an exclusive lock? */
if (pFile.local.bExclusive){
Debug.Assert(nNumberOfBytesToUnlockLow == (DWORD)SHARED_SIZE);
pFile.local.bExclusive = FALSE;
pFile.shared.bExclusive = FALSE;
bReturn = TRUE;
}

/* Did we just have a reader lock? */
else if (pFile.local.nReaders){
Debug.Assert(nNumberOfBytesToUnlockLow == (DWORD)SHARED_SIZE || nNumberOfBytesToUnlockLow == 1);
pFile.local.nReaders --;
if (pFile.local.nReaders == 0)
{
pFile.shared.nReaders --;
}
bReturn = TRUE;
}
}

/* Releasing a pending lock */
else if (dwFileOffsetLow == (DWORD)PENDING_BYTE && nNumberOfBytesToUnlockLow == 1){
if (pFile.local.bPending){
pFile.local.bPending = FALSE;
pFile.shared.bPending = FALSE;
bReturn = TRUE;
}
}
/* Releasing a reserved lock */
else if (dwFileOffsetLow == (DWORD)RESERVED_BYTE && nNumberOfBytesToUnlockLow == 1){
if (pFile.local.bReserved) {
pFile.local.bReserved = FALSE;
pFile.shared.bReserved = FALSE;
bReturn = TRUE;
}
}

winceMutexRelease(pFile.hMutex);
return bReturn;
}

/*
** An implementation of the LockFileEx() API of windows for wince
*/
static BOOL winceLockFileEx(
HANDLE *phFile,
DWORD dwFlags,
DWORD dwReserved,
DWORD nNumberOfBytesToLockLow,
DWORD nNumberOfBytesToLockHigh,
LPOVERLAPPED lpOverlapped
){
sqliteinth.UNUSED_PARAMETER(dwReserved);
sqliteinth.UNUSED_PARAMETER(nNumberOfBytesToLockHigh);

/* If the caller wants a shared read lock, forward this call
** to winceLockFile */
if (lpOverlapped.Offset == (DWORD)SHARED_FIRST &&
dwFlags == 1 &&
nNumberOfBytesToLockLow == (DWORD)SHARED_SIZE){
return winceLockFile(phFile, SHARED_FIRST, 0, 1, 0);
}
return FALSE;
}
/*
** End of the special code for wince
*****************************************************************************/
#endif
		///
///<summary>
///
///The next group of routines implement the I/O methods specified
///by the sqlite3_io_methods object.
///
///</summary>

		///<summary>
		/// Some microsoft compilers lack this definition.
		///
		///</summary>
		#if !INVALID_SET_FILE_POINTER
		//# define INVALID_SET_FILE_POINTER ((DWORD)-1)
		const int INVALID_SET_FILE_POINTER = -1;

		#endif
		///
///<summary>
///Move the current position of the file handle passed as the first 
///argument to offset iOffset within the file. If successful, return 0. 
///</summary>
///<param name="Otherwise, set pFile">zero.</param>

		static int seekWinFile (sqlite3_file id, sqlite3_int64 iOffset)
		{
			//LONG upperBits;                 /* Most sig. 32 bits of new offset */
			//LONG lowerBits;                 /* Least sig. 32 bits of new offset */
			DWORD dwRet;
			///
///<summary>
///Value returned by SetFilePointer() 
///</summary>

			sqlite3_file pFile = id;
			//upperBits = (LONG)((iOffset>>32) & 0x7fffffff);
			//lowerBits = (LONG)(iOffset & 0xffffffff);
			///
///<summary>
///API oddity: If successful, SetFilePointer() returns a dword 
///</summary>
///<param name="containing the lower 32">offset. Or, if it fails,</param>
///<param name="it returns INVALID_SET_FILE_POINTER. However according to MSDN, ">it returns INVALID_SET_FILE_POINTER. However according to MSDN, </param>
///<param name="INVALID_SET_FILE_POINTER may also be a valid new offset. So to determine ">INVALID_SET_FILE_POINTER may also be a valid new offset. So to determine </param>
///<param name="whether an error has actually occured, it is also necessary to call ">whether an error has actually occured, it is also necessary to call </param>
///<param name="GetLastError().">GetLastError().</param>
///<param name=""></param>

			//dwRet = SetFilePointer(id, lowerBits, &upperBits, FILE_BEGIN);
			//if( (dwRet==INVALID_SET_FILE_POINTER && GetLastError()!=NO_ERROR) ){
			//  pFile->lastErrno = GetLastError();
			//  winLogError(SQLITE_IOERR_SEEK, "seekWinFile", pFile->zPath);
			try {
				id.fs.Seek (iOffset, SeekOrigin.Begin);
				// SetFilePointer(pFile.fs.Name, lowerBits, upperBits, FILE_BEGIN);
			}
			catch (Exception e) {
				#if SQLITE_SILVERLIGHT
																																																																												        pFile.lastErrno = 1;
#else
				pFile.lastErrno = (u32)Marshal.GetLastWin32Error ();
				#endif
                winLogError(SqlResult.SQLITE_IOERR_SEEK, "seekWinFile", pFile.zPath);
				return 1;
			}
			return 0;
		}

		///<summary>
		/// Close a file.
		///
		/// It is reported that an attempt to close a handle might sometimes
		/// fail.  This is a very unreasonable result, but windows is notorious
		/// for being unreasonable so I do not doubt that it might happen.  If
		/// the close fails, we pause for 100 milliseconds and try again.  As
		/// many as MX_CLOSE_ATTEMPT attempts to close the handle are made before
		/// giving up and returning an error.
		///
		///</summary>
		public static int MX_CLOSE_ATTEMPT = 3;

		static SqlResult winClose (sqlite3_file id)
		{
			bool rc;
			int cnt = 0;
			sqlite3_file pFile = (sqlite3_file)id;
			Debug.Assert (id != null);
			Debug.Assert (pFile.pShm == null);
			#if SQLITE_DEBUG
																																																									      OSTRACE( "CLOSE %d (%s)\n", pFile.fs.GetHashCode(), pFile.fs.Name );
#endif
			do {
				pFile.fs.Close ();
				rc = true;
				//  rc = CloseHandle(pFile.h);
				///
///<summary>
///SimulateIOError( rc=0; cnt=MX_CLOSE_ATTEMPT; ); 
///</summary>

				//  if (!rc && ++cnt < MX_CLOSE_ATTEMPT) Thread.Sleep(100); //, 1) );
			}
			while (!rc && ++cnt < MX_CLOSE_ATTEMPT);
			//, 1) );
			#if SQLITE_OS_WINCE
																																																									//define WINCE_DELETION_ATTEMPTS 3
winceDestroyLock(pFile);
if( pFile.zDeleteOnClose ){
int cnt = 0;
while(
DeleteFileW(pFile.zDeleteOnClose)==0
&& GetFileAttributesW(pFile.zDeleteOnClose)!=0xffffffff
&& cnt++ < WINCE_DELETION_ATTEMPTS
){
Sleep(100);  /* Wait a little before trying again */
}
free(pFile.zDeleteOnClose);
}
#endif
			#if SQLITE_TEST
																																																									      OSTRACE( "CLOSE %d %s\n", pFile.fs.GetHashCode(), rc ? "ok" : "failed" );
      OpenCounter( -1 );
#endif
            return rc ? SqlResult.SQLITE_OK : winLogError(SqlResult.SQLITE_IOERR_CLOSE, "winClose", pFile.zPath);
		}

		///<summary>
		/// Read data from a file into a buffer.  Return SqlResult.SQLITE_OK if all
		/// bytes were read successfully and SQLITE_IOERR if anything goes
		/// wrong.
		///
		///</summary>
		static SqlResult winRead (sqlite3_file id, ///
///<summary>
///File to read from 
///</summary>

		byte[] pBuf, ///
///<summary>
///Write content into this buffer 
///</summary>

		int amt, ///
///<summary>
///Number of bytes to read 
///</summary>

		sqlite3_int64 offset///
///<summary>
///Begin reading at this offset 
///</summary>

		)
		{
			long rc;
			sqlite3_file pFile = id;
			int nRead;
			///
///<summary>
///Number of bytes actually read from file 
///</summary>

			Debug.Assert (id != null);
			#if SQLITE_TEST
																																																									      if ( SimulateIOError() )
        return SQLITE_IOERR_READ;
#endif
			#if SQLITE_DEBUG
																																																									      OSTRACE( "READ %d lock=%d\n", pFile.fs.GetHashCode(), pFile.locktype );
#endif
			if (!id.fs.CanRead)
				return SqlResult.SQLITE_IOERR_READ;
			if (seekWinFile (pFile, offset) != 0) {
				return SqlResult.SQLITE_FULL;
			}
			try {
				nRead = id.fs.Read (pBuf, 0, amt);
				// i  if( null==ReadFile(pFile->h, pBuf, amt, &nRead, 0) ){
			}
			catch (Exception e) {
				#if SQLITE_SILVERLIGHT
																																																																												pFile.lastErrno = 1;
#else
				pFile.lastErrno = (u32)Marshal.GetLastWin32Error ();
				#endif
				return winLogError (SqlResult.SQLITE_IOERR_READ, "winRead", pFile.zPath);
			}
			if (nRead < amt) {
				///
///<summary>
///</summary>
///<param name="Unread parts of the buffer must be zero">filled </param>

				Array.Clear (pBuf, (int)nRead, (int)(amt - nRead));
				// memset(&((char)pBuf)[nRead], 0, amt-nRead);
				return SqlResult.SQLITE_IOERR_SHORT_READ;
			}
			return SqlResult.SQLITE_OK;
		}

		///<summary>
		/// Write data from a buffer into a file.  Return SqlResult.SQLITE_OK on success
		/// or some other error code on failure.
		///
		///</summary>
		static SqlResult winWrite (sqlite3_file id, ///
///<summary>
///File to write into 
///</summary>

		byte[] pBuf, ///
///<summary>
///The bytes to be written 
///</summary>

		int amt, ///
///<summary>
///Number of bytes to write 
///</summary>

		sqlite3_int64 offset///
///<summary>
///Offset into the file to begin writing at 
///</summary>

		)
		{
			int rc;
			///
///<summary>
///True if error has occured, else false 
///</summary>

			sqlite3_file pFile = id;
			///
///<summary>
///File handle 
///</summary>

			Debug.Assert (amt > 0);
			Debug.Assert (pFile != null);
			#if SQLITE_TEST
																																																									      if ( SimulateIOError() )
        return SQLITE_IOERR_WRITE;
      if ( SimulateDiskfullError() )
        return SQLITE_FULL;
#endif
			#if SQLITE_DEBUG
																																																									      OSTRACE( "WRITE %d lock=%d\n", id.fs.GetHashCode(), id.locktype );
#endif
			rc = seekWinFile (pFile, offset);
			//if( rc==0 ){
			//  u8 *aRem = (u8 )pBuf;        /* Data yet to be written */
			//  int nRem = amt;               /* Number of bytes yet to be written */
			//  DWORD nWrite;                 /* Bytes written by each WriteFile() call */
			//  while( nRem>0 && WriteFile(pFile->h, aRem, nRem, &nWrite, 0) && nWrite>0 ){
			//    aRem += nWrite;
			//    nRem -= nWrite;
			//  }
			long wrote = id.fs.Position;
			try {
				Debug.Assert (pBuf.Length >= amt);
				id.fs.Write (pBuf, 0, amt);
				rc = 1;
				// Success
				wrote = id.fs.Position - wrote;
			}
			catch (IOException e) {
                return SqlResult.SQLITE_READONLY;
			}
			if (rc == 0 || amt > (int)wrote) {
				#if SQLITE_SILVERLIGHT
																																																																												id.lastErrno  = 1;
#else
				id.lastErrno = (u32)Marshal.GetLastWin32Error ();
				#endif
				if ((id.lastErrno == _Custom.ERROR_HANDLE_DISK_FULL) || (id.lastErrno == _Custom.ERROR_DISK_FULL)) {
                    return SqlResult.SQLITE_FULL;
				}
				else {
                    return winLogError(SqlResult.SQLITE_IOERR_WRITE, "winWrite", pFile.zPath);
				}
			}
			return SqlResult.SQLITE_OK;
		}

		///<summary>
		/// Truncate an open file to a specified size
		///
		///</summary>
		static SqlResult winTruncate (sqlite3_file id, sqlite3_int64 nByte)
		{
			sqlite3_file pFile = id;
            ///
            ///<summary>
            ///File handle object 
            ///</summary>

            SqlResult rc = SqlResult.SQLITE_OK;
			///
///<summary>
///Return code for this function 
///</summary>

			Debug.Assert (pFile != null);
			#if SQLITE_DEBUG
																																																									      OSTRACE( "TRUNCATE %d %lld\n", id.fs.Name, nByte );
#endif
			#if SQLITE_TEST
																																																									      if ( SimulateIOError() )
        return SQLITE_IOERR_TRUNCATE;
      if ( SimulateIOError() )
        return SQLITE_IOERR_TRUNCATE;
#endif
			///
///<summary>
///</summary>
///<param name="If the user has configured a chunk">size for this file, truncate the</param>
///<param name="file so that it consists of an integer number of chunks (i.e. the">file so that it consists of an integer number of chunks (i.e. the</param>
///<param name="actual file size after the operation may be larger than the requested">actual file size after the operation may be larger than the requested</param>
///<param name="size).">size).</param>

			if (pFile.szChunk != 0) {
				nByte = ((nByte + pFile.szChunk - 1) / pFile.szChunk) * pFile.szChunk;
			}
			///
///<summary>
///</summary>
///<param name="SetEndOfFile() returns non">zero when successful, or zero when it fails. </param>

			//if ( seekWinFile( pFile, nByte ) )
			//{
			//  rc = winLogError(SQLITE_IOERR_TRUNCATE, "winTruncate1", pFile->zPath);
			//}
			//else if( 0==SetEndOfFile(pFile->h) ){
			//  pFile->lastErrno = GetLastError();
			//  rc = winLogError(SQLITE_IOERR_TRUNCATE, "winTruncate2", pFile->zPath);
			//}
			try {
				id.fs.SetLength (nByte);
				rc = SqlResult.SQLITE_OK;
			}
			catch (IOException e) {
				#if SQLITE_SILVERLIGHT
																																																																												id.lastErrno  = 1;
#else
				id.lastErrno = (u32)Marshal.GetLastWin32Error ();
				#endif
                rc = winLogError(SqlResult.SQLITE_IOERR_TRUNCATE, "winTruncate2", pFile.zPath);
			}
			OSTRACE ("TRUNCATE %d %lld %s\n", id.fs.GetHashCode (), nByte, rc == SqlResult.SQLITE_OK ? "ok" : "failed");
			return rc;
		}

		#if SQLITE_TEST
																																						    /*
** Count the number of fullsyncs and normal syncs.  This is used to test
** that syncs and fullsyncs are occuring at the right times.
*/
#if !TCLSH
																																						    static int sqlite3_sync_count = 0;
    static int sqlite3_fullsync_count = 0;
#else
																																						    static tcl.lang.Var.SQLITE3_GETSET sqlite3_sync_count = new tcl.lang.Var.SQLITE3_GETSET( "sqlite3_sync_count" );
    static tcl.lang.Var.SQLITE3_GETSET sqlite3_fullsync_count = new tcl.lang.Var.SQLITE3_GETSET( "sqlite_fullsync_count" );
#endif
																																						#endif
		///<summary>
		/// Make sure all writes to a particular file are committed to disk.
		///</summary>
		static SqlResult winSync (sqlite3_file id, int flags)
		{
			#if !(NDEBUG) || !(SQLITE_NO_SYNC) || (SQLITE_DEBUG)
			sqlite3_file pFile = (sqlite3_file)id;
			bool rc;
			#else
																																																									sqliteinth.UNUSED_PARAMETER(id);
#endif
			Debug.Assert (pFile != null);
			///
///<summary>
///Check that one of SQLITE_SYNC_NORMAL or FULL was passed 
///</summary>

			Debug.Assert ((flags & 0x0F) == SQLITE_SYNC_NORMAL || (flags & 0x0F) == SQLITE_SYNC_FULL);
			OSTRACE ("SYNC %d lock=%d\n", pFile.fs.GetHashCode (), pFile.locktype);
			///
///<summary>
///Unix cannot, but some systems may return SQLITE_FULL from here. This
///line is to test that doing so does not cause any problems.
///
///</summary>

			#if SQLITE_TEST
																																																									        if ( SimulateDiskfullError() )
        return SQLITE_FULL;
#endif
			#if !SQLITE_TEST
			sqliteinth.UNUSED_PARAMETER (flags);
			#else
																																																									      if ( (flags&0x0F)==SQLITE_SYNC_FULL )
      {
#if !TCLSH
																																																									        sqlite3_fullsync_count++;
      }
      sqlite3_sync_count++;
#else
																																																									        sqlite3_fullsync_count.iValue++;
      }
      sqlite3_sync_count.iValue++;
#endif
																																																									#endif
			///
///<summary>
///If we compiled with the SQLITE_NO_SYNC flag, then syncing is a
///</summary>
///<param name="no">op</param>

			#if SQLITE_NO_SYNC
																																																									return SqlResult.SQLITE_OK;
#else
			pFile.fs.Flush ();
			return SqlResult.SQLITE_OK;
			//rc = FlushFileBuffers(pFile->h);
			//SimulateIOError( rc=FALSE );
			//if( rc ){
			//  return SqlResult.SQLITE_OK;
			//}else{
			//  pFile->lastErrno = GetLastError();
			//  return winLogError(SQLITE_IOERR_FSYNC, "winSync", pFile->zPath);
			//}
			#endif
		}

		///<summary>
		/// Determine the current size of a file in bytes
		///
		///</summary>
		static SqlResult winFileSize (sqlite3_file id, ref long pSize)
		{
			//DWORD upperBits;
			//DWORD lowerBits;
			//  sqlite3_file pFile = (sqlite3_file)id;
			//  DWORD error;
			Debug.Assert (id != null);
			#if SQLITE_TEST
																																																									      if ( SimulateIOError() )
        return SQLITE_IOERR_FSTAT;
#endif
			//lowerBits = GetFileSize(pFile.fs.Name, upperBits);
			//if ( ( lowerBits == INVALID_FILE_SIZE )
			//   && ( ( error = GetLastError() ) != NO_ERROR ) )
			//{
			//  pFile.lastErrno = error;
			//  return winLogError(SQLITE_IOERR_FSTAT, "winFileSize", pFile->zPath);
			//}
			//pSize = (((sqlite3_int64)upperBits)<<32) + lowerBits;
			pSize = id.fs.CanRead ? id.fs.Length : 0;
			return SqlResult.SQLITE_OK;
		}

		///<summary>
		/// Acquire a reader lock.
		/// Different API routines are called depending on whether or not this
		/// is Win95 or WinNT.
		///
		///</summary>
		static int getReadLock (sqlite3_file pFile)
		{
			int res = 0;
			if (isNT ()) {
				res = lockingStrategy.SharedLockFile (pFile, SHARED_FIRST, SHARED_SIZE);
			}
			///
///<summary>
///isNT() is 1 if SQLITE_OS_WINCE==1, so this else is never executed.
///
///</summary>

			#if !SQLITE_OS_WINCE
			//else
			//{
			//  int lk;
			//  sqlite3_randomness(lk.Length, lk);
			//  pFile.sharedLockByte = (u16)((lk & 0x7fffffff)%(SHARED_SIZE - 1));
			//  res = pFile.fs.Lock( SHARED_FIRST + pFile.sharedLockByte, 0, 1, 0);
			#endif
			//}
			if (res == 0) {
				#if SQLITE_SILVERLIGHT
																																																																												pFile.lastErrno = 1;
#else
				pFile.lastErrno = (u32)Marshal.GetLastWin32Error ();
				#endif
			}
			///
///<summary>
///No need to log a failure to lock 
///</summary>

			return res;
		}

		///<summary>
		/// Undo a readlock
		///
		///</summary>
		static int unlockReadLock (sqlite3_file pFile)
		{
			int res = 1;
			if (isNT ()) {
				try {
					lockingStrategy.UnlockFile (pFile, SHARED_FIRST, SHARED_SIZE);
					//     res = UnlockFile(pFile.h, SHARED_FIRST, 0, SHARED_SIZE, 0);
				}
				catch (Exception e) {
					res = 0;
				}
			}
			///
///<summary>
///isNT() is 1 if SQLITE_OS_WINCE==1, so this else is never executed.
///
///</summary>

			#if !SQLITE_OS_WINCE
			else {
				Debugger.Break ();
				//    res = UnlockFile(pFile.h, SHARED_FIRST + pFilE.sharedLockByte, 0, 1, 0);
			}
			#endif
			if (res == 0) {
				#if SQLITE_SILVERLIGHT
																																																																												pFile.lastErrno = 1;
#else
				pFile.lastErrno = (u32)Marshal.GetLastWin32Error ();
				#endif
                winLogError(SqlResult.SQLITE_IOERR_UNLOCK, "unlockReadLock", pFile.zPath);
			}
			return res;
		}

		///<summary>
		/// Lock the file with the lock specified by parameter locktype - one
		/// of the following:
		///
		///     (1) SHARED_LOCK
		///     (2) RESERVED_LOCK
		///     (3) PENDING_LOCK
		///     (4) EXCLUSIVE_LOCK
		///
		/// Sometimes when requesting one lock state, additional lock states
		/// are inserted in between.  The locking might fail on one of the later
		/// transitions leaving the lock state different from what it started but
		/// still short of its goal.  The following chart shows the allowed
		/// transitions and the inserted intermediate states:
		///
		///    UNLOCKED . SHARED
		///    SHARED . RESERVED
		///    SHARED . (PENDING) . EXCLUSIVE
		///    RESERVED . (PENDING) . EXCLUSIVE
		///    PENDING . EXCLUSIVE
		///
		/// This routine will only increase a lock.  The winUnlock() routine
		/// erases all locks at once and returns us immediately to locking level 0.
		/// It is not possible to lower the locking level one step at a time.  You
		/// must go straight to locking level 0.
		///
		///</summary>
		static SqlResult winLock (sqlite3_file id, LockType locktype)
		{
			var rc = SqlResult.SQLITE_OK;
			///
///<summary>
///Return code from subroutines 
///</summary>

			int res = 1;
			///
///<summary>
///Result of a windows lock call 
///</summary>

            LockType newLocktype;
			///
///<summary>
///Set pFile.locktype to this value before exiting 
///</summary>

			bool gotPendingLock = false;
			///
///<summary>
///True if we acquired a PENDING lock this time 
///</summary>

			sqlite3_file pFile = (sqlite3_file)id;
			DWORD error = NO_ERROR;
			Debug.Assert (id != null);
			#if SQLITE_DEBUG
																																																									      OSTRACE( "LOCK %d %d was %d(%d)\n",
      pFile.fs.GetHashCode(), locktype, pFile.locktype, pFile.sharedLockByte );
#endif
			///
///<summary>
///If there is already a lock of this type or more restrictive on the
///OsFile, do nothing. Don't use the end_lock: exit path, as
///sqlite3OsEnterMutex() hasn't been called yet.
///</summary>

			if (pFile.locktype >= locktype) {
				return SqlResult.SQLITE_OK;
			}
			///
///<summary>
///Make sure the locking sequence is correct
///
///</summary>

            Debug.Assert(pFile.locktype != LockType.NO_LOCK || locktype == LockType.SHARED_LOCK);
            Debug.Assert(locktype != LockType.PENDING_LOCK);
            Debug.Assert(locktype != LockType.RESERVED_LOCK || pFile.locktype == LockType.SHARED_LOCK);
			///
///<summary>
///Lock the PENDING_LOCK byte if we need to acquire a PENDING lock or
///a SHARED lock.  If we are acquiring a SHARED lock, the acquisition of
///the PENDING_LOCK byte is temporary.
///
///</summary>

			newLocktype = pFile.locktype;
            if (pFile.locktype == LockType.NO_LOCK || ((locktype == LockType.EXCLUSIVE_LOCK) && (pFile.locktype == LockType.RESERVED_LOCK)))
            {
				int cnt = 3;
				res = 0;
				while (cnt-- > 0 && res == 0)//(res = LockFile(pFile.fs.SafeFileHandle.DangerousGetHandle().ToInt32(), PENDING_BYTE, 0, 1, 0)) == 0)
				 {
					try {
						lockingStrategy.LockFile (pFile, PENDING_BYTE, 1);
						res = 1;
					}
					catch (Exception e) {
						///
///<summary>
///Try 3 times to get the pending lock.  The pending lock might be
///held by another reader process who will release it momentarily.
///
///</summary>

						#if SQLITE_DEBUG
																																																																																																																		            OSTRACE( "could not get a PENDING lock. cnt=%d\n", cnt );
#endif
						Thread.Sleep (1);
					}
				}
				gotPendingLock = (res != 0);
				if (0 == res) {
					#if SQLITE_SILVERLIGHT
																																																																																															error = 1;
#else
					error = (u32)Marshal.GetLastWin32Error ();
					#endif
				}
			}
			///
///<summary>
///Acquire a shared lock
///
///</summary>

            if (locktype == LockType.SHARED_LOCK && res != 0)
            {
                Debug.Assert(pFile.locktype == LockType.NO_LOCK);
				res = getReadLock (pFile);
				if (res != 0) {
                    newLocktype = LockType.SHARED_LOCK;
				}
				else {
					#if SQLITE_SILVERLIGHT
																																																																																															error = 1;
#else
					error = (u32)Marshal.GetLastWin32Error ();
					#endif
				}
			}
			///
///<summary>
///Acquire a RESERVED lock
///
///</summary>

            if ((locktype == LockType.RESERVED_LOCK) && res != 0)
            {
                Debug.Assert(pFile.locktype == LockType.SHARED_LOCK);
				try {
					lockingStrategy.LockFile (pFile, RESERVED_BYTE, 1);
					//res = LockFile(pFile.fs.SafeFileHandle.DangerousGetHandle().ToInt32(), RESERVED_BYTE, 0, 1, 0);
                    newLocktype = LockType.RESERVED_LOCK;
					res = 1;
				}
				catch (Exception e) {
					res = 0;
					#if SQLITE_SILVERLIGHT
																																																																																															error = 1;
#else
					error = (u32)Marshal.GetLastWin32Error ();
					#endif
				}
				if (res != 0) {
                    newLocktype = LockType.RESERVED_LOCK;
				}
				else {
					#if SQLITE_SILVERLIGHT
																																																																																															error = 1;
#else
					error = (u32)Marshal.GetLastWin32Error ();
					#endif
				}
			}
			///
///<summary>
///Acquire a PENDING lock
///
///</summary>

            if (locktype == LockType.EXCLUSIVE_LOCK && res != 0)
            {
                newLocktype = LockType.PENDING_LOCK;
				gotPendingLock = false;
			}
			///
///<summary>
///Acquire an EXCLUSIVE lock
///
///</summary>

            if (locktype == LockType.EXCLUSIVE_LOCK && res != 0)
            {
                Debug.Assert(pFile.locktype >= LockType.SHARED_LOCK);
				res = unlockReadLock (pFile);
				#if SQLITE_DEBUG
																																																																												        OSTRACE( "unreadlock = %d\n", res );
#endif
				//res = LockFile(pFile.fs.SafeFileHandle.DangerousGetHandle().ToInt32(), SHARED_FIRST, 0, SHARED_SIZE, 0);
				try {
					lockingStrategy.LockFile (pFile, SHARED_FIRST, SHARED_SIZE);
                    newLocktype = LockType.EXCLUSIVE_LOCK;
					res = 1;
				}
				catch (Exception e) {
					res = 0;
				}
				if (res != 0) {
                    newLocktype = LockType.EXCLUSIVE_LOCK;
				}
				else {
					#if SQLITE_SILVERLIGHT
																																																																																															error = 1;
#else
					error = (u32)Marshal.GetLastWin32Error ();
					#endif
					#if SQLITE_DEBUG
																																																																																															          OSTRACE( "error-code = %d\n", error );
#endif
					getReadLock (pFile);
				}
			}
			///
///<summary>
///If we are holding a PENDING lock that ought to be released, then
///release it now.
///
///</summary>

            if (gotPendingLock && locktype == LockType.SHARED_LOCK)
            {
				lockingStrategy.UnlockFile (pFile, PENDING_BYTE, 1);
			}
			///
///<summary>
///Update the state of the lock has held in the file descriptor then
///return the appropriate result code.
///
///</summary>

			if (res != 0) {
				rc = SqlResult.SQLITE_OK;
			}
			else {
				#if SQLITE_DEBUG
																																																																												        OSTRACE( "LOCK FAILED %d trying for %d but got %d\n", pFile.fs.GetHashCode(),
        locktype, newLocktype );
#endif
				pFile.lastErrno = error;
				rc = SqlResult.SQLITE_BUSY;
			}
			pFile.locktype = newLocktype;
			return rc;
		}

		///<summary>
		/// This routine checks if there is a RESERVED lock held on the specified
		/// file by this or any other process. If such a lock is held, return
		/// non-zero, otherwise zero.
		///
		///</summary>
		static SqlResult winCheckReservedLock (sqlite3_file id, ref int pResOut)
		{
			int rc;
			sqlite3_file pFile = (sqlite3_file)id;
			if (SimulateIOError ())
                return SqlResult.SQLITE_IOERR_CHECKRESERVEDLOCK;
			Debug.Assert (id != null);
            if (pFile.locktype >= LockType.RESERVED_LOCK)
            {
				rc = 1;
				#if SQLITE_DEBUG
																																																																												        OSTRACE( "TEST WR-LOCK %d %d (local)\n", pFile.fs.Name, rc );
#endif
			}
			else {
				try {
					lockingStrategy.LockFile (pFile, RESERVED_BYTE, 1);
					lockingStrategy.UnlockFile (pFile, RESERVED_BYTE, 1);
					rc = 1;
				}
				catch (IOException e) {
					rc = 0;
				}
				rc = 1 - rc;
				// !rc
				#if SQLITE_DEBUG
																																																																												        OSTRACE( "TEST WR-LOCK %d %d (remote)\n", pFile.fs.GetHashCode(), rc );
#endif
			}
			pResOut = rc;
			return SqlResult.SQLITE_OK;
		}

		///<summary>
		/// Lower the locking level on file descriptor id to locktype.  locktype
		/// must be either NO_LOCK or SHARED_LOCK.
		///
		/// If the locking level of the file descriptor is already at or below
		/// the requested locking level, this routine is a no-op.
		///
		/// It is not possible for this routine to fail if the second argument
		/// is NO_LOCK.  If the second argument is SHARED_LOCK then this routine
		/// might return SQLITE_IOERR;
		///
		///</summary>
		static SqlResult winUnlock (sqlite3_file id, LockType locktype)
		{
            LockType type;
			sqlite3_file pFile = (sqlite3_file)id;
			var rc = SqlResult.SQLITE_OK;
			Debug.Assert (pFile != null);
            Debug.Assert(locktype <= LockType.SHARED_LOCK);
			#if SQLITE_DEBUG
																																																									      OSTRACE( "UNLOCK %d to %d was %d(%d)\n", pFile.fs.GetHashCode(), locktype,
      pFile.locktype, pFile.sharedLockByte );
#endif
			type = pFile.locktype;
            if (type >= LockType.EXCLUSIVE_LOCK)
            {
				lockingStrategy.UnlockFile (pFile, SHARED_FIRST, SHARED_SIZE);
				// UnlockFile(pFile.h, SHARED_FIRST, 0, SHARED_SIZE, 0);
                if (locktype == LockType.SHARED_LOCK && getReadLock(pFile) == 0)
                {
					///
///<summary>
///This should never happen.  We should always be able to
///reacquire the read lock 
///</summary>

                    rc = winLogError(SqlResult.SQLITE_IOERR_UNLOCK, "winUnlock", pFile.zPath);
				}
			}
            if (type >= LockType.RESERVED_LOCK)
            {
				try {
					lockingStrategy.UnlockFile (pFile, RESERVED_BYTE, 1);
					// UnlockFile(pFile.h, RESERVED_BYTE, 0, 1, 0);
				}
				catch (Exception e) {
				}
			}
            if (locktype == LockType.NO_LOCK && type >= LockType.SHARED_LOCK)
            {
				unlockReadLock (pFile);
			}
            if (type >= LockType.PENDING_LOCK)
            {
				try {
					lockingStrategy.UnlockFile (pFile, PENDING_BYTE, 1);
					//    UnlockFile(pFile.h, PENDING_BYTE, 0, 1, 0);
				}
				catch (Exception e) {
				}
			}
			pFile.locktype = locktype;
			return rc;
		}

		///<summary>
		/// Control and query of the open file handle.
		///
		///</summary>
        static SqlResult winFileControl(sqlite3_file id, int op, ref sqlite3_int64 pArg)
		{
			switch (op) {
			case SQLITE_FCNTL_LOCKSTATE: {
				pArg = (int)((sqlite3_file)id).locktype;
				return SqlResult.SQLITE_OK;
			}
			case SQLITE_LAST_ERRNO: {
				pArg = (int)((sqlite3_file)id).lastErrno;
				return SqlResult.SQLITE_OK;
			}
			case SQLITE_FCNTL_CHUNK_SIZE: {
				((sqlite3_file)id).szChunk = (int)pArg;
				return SqlResult.SQLITE_OK;
			}
			case SQLITE_FCNTL_SIZE_HINT: {
				sqlite3_int64 sz = (sqlite3_int64)pArg;
				SimulateIOErrorBenign (1);
				winTruncate (id, sz);
				SimulateIOErrorBenign (0);
				return SqlResult.SQLITE_OK;
			}
			case SQLITE_FCNTL_SYNC_OMITTED: {
				return SqlResult.SQLITE_OK;
			}
			}
            return SqlResult.SQLITE_NOTFOUND;
		}

		///<summary>
		/// Return the sector size in bytes of the underlying block device for
		/// the specified file. This is almost always 512 bytes, but may be
		/// larger for some devices.
		///
		/// SQLite code assumes this function cannot fail. It also assumes that
		/// if two files are created in the same file-system directory (i.e.
		/// a database and its journal file) that the sector size will be the
		/// same for both.
		///
		///</summary>
		static int winSectorSize (sqlite3_file id)
		{
			Debug.Assert (id != null);
			return (int)(id.sectorSize);
		}

		///
///<summary>
///Return a vector of device characteristics.
///
///</summary>

		static int winDeviceCharacteristics (sqlite3_file id)
		{
			sqliteinth.UNUSED_PARAMETER (id);
			return 0;
		}

		#if !SQLITE_OMIT_WAL
																																						

///<summary>
///
/// Windows will only let you create file view mappings
/// on allocation size granularity boundaries.
/// During sqlite3_os_init() we do a GetSystemInfo()
/// to get the granularity size.
///</summary>
SYSTEM_INFO winSysInfo;

///<summary>
/// Helper functions to obtain and relinquish the global mutex. The
/// global mutex is used to protect the winLockInfo objects used by
/// this file, all of which may be shared by multiple threads.
///
/// Function winShmMutexHeld() is used to Debug.Assert() that the global mutex
/// is held when required. This function is only used as part of Debug.Assert()
/// statements. e.g.
///
///   winShmEnterMutex()
///     Debug.Assert( winShmMutexHeld() );
///   winShmLeaveMutex()
///</summary>
static void winShmEnterMutex(void){
  sqlite3_mutex_enter(sqlite3MutexAlloc(SQLITE_MUTEX_STATIC_MASTER));
}
static void winShmLeaveMutex(void){
  sqlite3_mutex_leave(sqlite3MutexAlloc(SQLITE_MUTEX_STATIC_MASTER));
}
#if SQLITE_DEBUG
																																						static int winShmMutexHeld(void) {
  return Sqlite3.sqlite3_mutex_held(sqlite3MutexAlloc(SQLITE_MUTEX_STATIC_MASTER));
}
#endif
																																						
/*
** Object used to represent a single file opened and mmapped to provide
** shared memory.  When multiple threads all reference the same
** log-summary, each thread has its own winFile object, but they all
** point to a single instance of this object.  In other words, each
** log-summary is opened only once per process.
**
** winShmMutexHeld() must be true when creating or destroying
** this object or while reading or writing the following fields:
**
**      nRef
**      pNext 
**
** The following fields are read-only after the object is created:
** 
**      fid
**      zFilename
**
** Either winShmNode.mutex must be held or winShmNode.nRef==0 and
** winShmMutexHeld() is true when reading or writing any other field
** in this structure.
**
*/
struct winShmNode {
  sqlite3_mutex *mutex;      /* Mutex to access this object */
  string zFilename;           /* Name of the file */
  winFile hFile;             /* File handle from winOpen */

  int szRegion;              ///<summary>
///Size of shared-memory regions
///</summary>
  int nRegion;               /* Size of array apRegion */
  struct ShmRegion {
    HANDLE hMap;             /* File handle from CreateFileMapping */
    void *pMap;
  } *aRegion;
  DWORD lastErrno;           /* The Windows errno from the last I/O error */

  int nRef;                  /* Number of winShm objects pointing to this */
  winShm *pFirst;            /* All winShm objects pointing to this */
  winShmNode *pNext;         /* Next in list of all winShmNode objects */
#if SQLITE_DEBUG
																																						  u8 nextShmId;              /* Next available winShm.id value */
#endif
																																						};

///<summary>
/// A global array of all winShmNode objects.
///
/// The winShmMutexHeld() must be true while reading or writing this list.
///</summary>
static winShmNode *winShmNodeList = 0;

/*
** Structure used internally by this VFS to record the state of an
** open shared memory connection.
**
** The following fields are initialized when this object is created and
** are read-only thereafter:
**
**    winShm.pShmNode
**    winShm.id
**
** All other fields are read/write.  The winShm.pShmNode->mutex must be held
** while accessing any read/write fields.
*/
struct winShm {
  winShmNode *pShmNode;      /* The underlying winShmNode object */
  winShm *pNext;             /* Next winShm with the same winShmNode */
  u8 hasMutex;               /* True if holding the winShmNode mutex */
  u16 sharedMask;            /* Mask of shared locks held */
  u16 exclMask;              /* Mask of exclusive locks held */
#if SQLITE_DEBUG
																																						  u8 id;                     /* Id of this connection with its winShmNode */
#endif
																																						};

/*
** Constants used for locking
*/
//define WIN_SHM_BASE   ((22+SQLITE_SHM_NLOCK)*4)        /* first lock byte */
//define WIN_SHM_DMS    (WIN_SHM_BASE+SQLITE_SHM_NLOCK)  /* deadman switch */

///<summary>
/// Apply advisory locks for all n bytes beginning at ofst.
///</summary>
//define _SHM_UNLCK  1
//define _SHM_RDLCK  2
//define _SHM_WRLCK  3
static int winShmSystemLock(
  winShmNode *pFile,    /* Apply locks to this open shared-memory segment */
  int lockType,         /* _SHM_UNLCK, _SHM_RDLCK, or _SHM_WRLCK */
  int ofst,             /* Offset to first byte to be locked/unlocked */
  int nByte             /* Number of bytes to lock or unlock */
){
  OVERLAPPED ovlp;
  DWORD dwFlags;
  var rc = 0;           /* Result code form Lock/UnlockFileEx() */

  /* Access to the winShmNode object is serialized by the caller */
  Debug.Assert( Sqlite3.sqlite3_mutex_held(pFile->mutex) || pFile->nRef==0 );

  /* Initialize the locking parameters */
  dwFlags = LOCKFILE_FAIL_IMMEDIATELY;
  if( lockType == _SHM_WRLCK ) dwFlags |= LOCKFILE_EXCLUSIVE_LOCK;

  memset(&ovlp, 0, sizeof(OVERLAPPED));
  ovlp.Offset = ofst;

  /* Release/Acquire the system-level lock */
  if( lockType==_SHM_UNLCK ){
    rc = UnlockFileEx(pFile->hFile.h, 0, nByte, 0, &ovlp);
  }else{
    rc = LockFileEx(pFile->hFile.h, dwFlags, 0, nByte, 0, &ovlp);
  }
  
  if( rc!= 0 ){
    rc = SqlResult.SQLITE_OK;
  }else{
    pFile->lastErrno =  GetLastError();
    rc = SQLITE_BUSY;
  }

  OSTRACE(("SHM-LOCK %d %s %s 0x%08lx\n", 
           pFile->hFile.h,
           rc==SqlResult.SQLITE_OK ? "ok" : "failed",
           lockType==_SHM_UNLCK ? "UnlockFileEx" : "LockFileEx",
           pFile->lastErrno));

  return rc;
}

///<summary>
///Forward references to VFS methods
///</summary>
static int winOpen(sqlite3_vfs*,const char*,sqlite3_file*,int,int);
static int winDelete(sqlite3_vfs *,const char*,int);

///<summary>
/// Purge the winShmNodeList list of all entries with winShmNode.nRef==0.
///
/// This is not a VFS shared-memory method; it is a utility function called
/// by VFS shared-memory methods.
///</summary>
static void winShmPurge(sqlite3_vfs *pVfs, int deleteFlag){
  winShmNode **pp;
  winShmNode *p;
  BOOL bRc;
  Debug.Assert( winShmMutexHeld() );
  pp = winShmNodeList;
  while( (p = *pp)!=0 ){
    if( p->nRef==0 ){
      int i;
      if( p->mutex ) sqlite3_mutex_free(p->mutex);
      for(i=0; i<p->nRegion; i++){
        bRc = UnmapViewOfFile(p->aRegion[i].pMap);
        OSTRACE(("SHM-PURGE pid-%d unmap region=%d %s\n",
                 (int)GetCurrentProcessId(), i,
                 bRc ? "ok" : "failed"));
        bRc = CloseHandle(p->aRegion[i].hMap);
        OSTRACE(("SHM-PURGE pid-%d close region=%d %s\n",
                 (int)GetCurrentProcessId(), i,
                 bRc ? "ok" : "failed"));
      }
      if( p->hFile.h != INVALID_HANDLE_VALUE ){
        SimulateIOErrorBenign(1);
        winClose((sqlite3_file )&p->hFile);
        SimulateIOErrorBenign(0);
      }
      if( deleteFlag ){
        SimulateIOErrorBenign(1);
        winDelete(pVfs, p->zFilename, 0);
        SimulateIOErrorBenign(0);
      }
      *pp = p->pNext;
      malloc_cs.sqlite3_free(p->aRegion);
      malloc_cs.sqlite3_free(p);
    }else{
      pp = p->pNext;
    }
  }
}

/*
** Open the shared-memory area associated with database file pDbFd.
**
** When opening a new shared-memory file, if no other instances of that
** file are currently open, in this process or in other processes, then
** the file must be truncated to zero length or have its header cleared.
*/
static int winOpenSharedMemory(winFile *pDbFd){
  struct winShm *p;                  /* The connection to be opened */
  struct winShmNode *pShmNode = 0;   /* The underlying mmapped file */
  int rc;                            /* Result code */
  struct winShmNode *pNew;           /* Newly allocated winShmNode */
  int nName;                         /* Size of zName in bytes */

  Debug.Assert( pDbFd->pShm==null );    /* Not previously opened */

  /* Allocate space for the new sqlite3_shm object.  Also speculatively
  ** allocate space for a new winShmNode and filename.
  */
  p = sqlite3_malloc( sizeof(*p) );
  if( p==0 ) return SQLITE_NOMEM;
  memset(p, 0, sizeof(*p));
  nName = StringExtensions.sqlite3Strlen30(pDbFd->zPath);
  pNew = sqlite3_malloc( sizeof(*pShmNode) + nName + 15 );
  if( pNew==0 ){
    malloc_cs.sqlite3_free(p);
    return SQLITE_NOMEM;
  }
  memset(pNew, 0, sizeof(*pNew));
  pNew->zFilename = (char)&pNew[1];
  io.sqlite3_snprintf(nName+15, pNew->zFilename, "%s-shm", pDbFd->zPath);
  sqlite3FileSuffix3(pDbFd->zPath, pNew->zFilename); 

  /* Look to see if there is an existing winShmNode that can be used.
  ** If no matching winShmNode currently exists, create a new one.
  */
  winShmEnterMutex();
  for(pShmNode = winShmNodeList; pShmNode; pShmNode=pShmNode->pNext){
    /* TBD need to come up with better match here.  Perhaps
    ** use FILE_ID_BOTH_DIR_INFO Structure.
    */
    if( sqlite3StrICmp(pShmNode->zFilename, pNew->zFilename)==0 ) break;
  }
  if( pShmNode ){
    malloc_cs.sqlite3_free(pNew);
  }else{
    pShmNode = pNew;
    pNew = 0;
    ((winFile)(&pShmNode->hFile))->h = INVALID_HANDLE_VALUE;
    pShmNode->pNext = winShmNodeList;
    winShmNodeList = pShmNode;

    pShmNode->mutex = sqlite3_mutex_alloc(SQLITE_MUTEX_FAST);
    if( pShmNode->mutex==0 ){
      rc = SQLITE_NOMEM;
      goto shm_open_err;
    }

    rc = winOpen(pDbFd->pVfs,
                 pShmNode->zFilename,             /* Name of the file (UTF-8) */
                 (sqlite3_file)&pShmNode->hFile,  /* File handle here */
                 SQLITE_OPEN_WAL | SQLITE_OPEN_READWRITE | SQLITE_OPEN_CREATE, /* Mode flags */
                 0);
    if( SqlResult.SQLITE_OK!=rc ){
      rc =  sqliteinth.SQLITE_CANTOPEN_BKPT;
      goto shm_open_err;
    }

    /* Check to see if another process is holding the dead-man switch.
    ** If not, truncate the file to zero length. 
    */
    if( winShmSystemLock(pShmNode, _SHM_WRLCK, WIN_SHM_DMS, 1)==SqlResult.SQLITE_OK ){
      rc = winTruncate((sqlite3_file )&pShmNode->hFile, 0);
      if( rc!=SqlResult.SQLITE_OK ){
        rc = winLogError(SQLITE_IOERR_SHMOPEN, "winOpenShm", pDbFd->zPath);
      }
    }
    if( rc==SqlResult.SQLITE_OK ){
      winShmSystemLock(pShmNode, _SHM_UNLCK, WIN_SHM_DMS, 1);
      rc = winShmSystemLock(pShmNode, _SHM_RDLCK, WIN_SHM_DMS, 1);
    }
    if( rc ) goto shm_open_err;
  }

  /* Make the new connection a child of the winShmNode */
  p->pShmNode = pShmNode;
#if SQLITE_DEBUG
																																						  p->id = pShmNode->nextShmId++;
#endif
																																						  pShmNode->nRef++;
  pDbFd->pShm = p;
  winShmLeaveMutex();

  /* The reference count on pShmNode has already been incremented under
  ** the cover of the winShmEnterMutex() mutex and the pointer from the
  ** new (struct winShm) object to the pShmNode has been set. All that is
  ** left to do is to link the new object into the linked list starting
  ** at pShmNode->pFirst. This must be done while holding the pShmNode->mutex 
  ** mutex.
  */
  sqlite3_mutex_enter(pShmNode->mutex);
  p->pNext = pShmNode->pFirst;
  pShmNode->pFirst = p;
  sqlite3_mutex_leave(pShmNode->mutex);
  return SqlResult.SQLITE_OK;

  /* Jump here on any error */
shm_open_err:
  winShmSystemLock(pShmNode, _SHM_UNLCK, WIN_SHM_DMS, 1);
  winShmPurge(pDbFd->pVfs, 0);      /* This call frees pShmNode if required */
  malloc_cs.sqlite3_free(p);
  malloc_cs.sqlite3_free(pNew);
  winShmLeaveMutex();
  return rc;
}

/*
** Close a connection to shared-memory.  Delete the underlying 
** storage if deleteFlag is true.
*/
static int winShmUnmap(
  sqlite3_file *fd,          /* Database holding shared memory */
  int deleteFlag             /* Delete after closing if true */
){
  winFile *pDbFd;       /* Database holding shared-memory */
  winShm *p;            /* The connection to be closed */
  winShmNode *pShmNode; /* The underlying shared-memory file */
  winShm **pp;          /* For looping over sibling connections */

  pDbFd = (winFile)fd;
  p = pDbFd->pShm;
  if( p==0 ) return SqlResult.SQLITE_OK;
  pShmNode = p->pShmNode;

  /* Remove connection p from the set of connections associated
  ** with pShmNode */
  sqlite3_mutex_enter(pShmNode->mutex);
  for(pp=&pShmNode->pFirst; (*pp)!=p; pp = (*pp)->pNext){}
  *pp = p->pNext;

  /* Free the connection p */
  malloc_cs.sqlite3_free(p);
  pDbFd->pShm = 0;
  sqlite3_mutex_leave(pShmNode->mutex);

  /* If pShmNode->nRef has reached 0, then close the underlying
  ** shared-memory file, too */
  winShmEnterMutex();
  Debug.Assert( pShmNode->nRef>0 );
  pShmNode->nRef--;
  if( pShmNode->nRef==0 ){
    winShmPurge(pDbFd->pVfs, deleteFlag);
  }
  winShmLeaveMutex();

  return SqlResult.SQLITE_OK;
}

/*
** Change the lock state for a shared-memory segment.
*/
static int winShmLock(
  sqlite3_file *fd,          /* Database file holding the shared memory */
  int ofst,                  /* First lock to acquire or release */
  int n,                     /* Number of locks to acquire or release */
  int flags                  /* What to do with the lock */
){
  winFile *pDbFd = (winFile)fd;        /* Connection holding shared memory */
  winShm *p = pDbFd->pShm;              /* The shared memory being locked */
  winShm *pX;                           /* For looping over all siblings */
  winShmNode *pShmNode = p->pShmNode;
  var rc = SqlResult.SQLITE_OK;                   /* Result code */
  u16 mask;                             /* Mask of locks to take or release */

  Debug.Assert( ofst>=0 && ofst+n<=SQLITE_SHM_NLOCK );
  Debug.Assert( n>=1 );
  Debug.Assert( flags==(SQLITE_SHM_LOCK | SQLITE_SHM_SHARED)
       || flags==(SQLITE_SHM_LOCK | SQLITE_SHM_EXCLUSIVE)
       || flags==(SQLITE_SHM_UNLOCK | SQLITE_SHM_SHARED)
       || flags==(SQLITE_SHM_UNLOCK | SQLITE_SHM_EXCLUSIVE) );
  Debug.Assert( n==1 || (flags & SQLITE_SHM_EXCLUSIVE)!=0 );

  mask = (u16)((1U<<(ofst+n)) - (1U<<ofst));
  Debug.Assert( n>1 || mask==(1<<ofst) );
  sqlite3_mutex_enter(pShmNode->mutex);
  if( flags & SQLITE_SHM_UNLOCK ){
    u16 allMask = 0; /* Mask of locks held by siblings */

    /* See if any siblings hold this same lock */
    for(pX=pShmNode->pFirst; pX; pX=pX->pNext){
      if( pX==p ) continue;
      Debug.Assert( (pX->exclMask & (p->exclMask|p->sharedMask))==0 );
      allMask |= pX->sharedMask;
    }

    /* Unlock the system-level locks */
    if( (mask & allMask)==0 ){
      rc = winShmSystemLock(pShmNode, _SHM_UNLCK, ofst+WIN_SHM_BASE, n);
    }else{
      rc = SqlResult.SQLITE_OK;
    }

    /* Undo the local locks */
    if( rc==SqlResult.SQLITE_OK ){
      p->exclMask &= ~mask;
      p->sharedMask &= ~mask;
    } 
  }else if( flags & SQLITE_SHM_SHARED ){
    u16 allShared = 0;  /* Union of locks held by connections other than "p" */

    /* Find out which shared locks are already held by sibling connections.
    ** If any sibling already holds an exclusive lock, go ahead and return
    ** SQLITE_BUSY.
    */
    for(pX=pShmNode->pFirst; pX; pX=pX->pNext){
      if( (pX->exclMask & mask)!=0 ){
        rc = SQLITE_BUSY;
        break;
      }
      allShared |= pX->sharedMask;
    }

    /* Get shared locks at the system level, if necessary */
    if( rc==SqlResult.SQLITE_OK ){
      if( (allShared & mask)==0 ){
        rc = winShmSystemLock(pShmNode, _SHM_RDLCK, ofst+WIN_SHM_BASE, n);
      }else{
        rc = SqlResult.SQLITE_OK;
      }
    }

    /* Get the local shared locks */
    if( rc==SqlResult.SQLITE_OK ){
      p->sharedMask |= mask;
    }
  }else{
    /* Make sure no sibling connections hold locks that will block this
    ** lock.  If any do, return SQLITE_BUSY right away.
    */
    for(pX=pShmNode->pFirst; pX; pX=pX->pNext){
      if( (pX->exclMask & mask)!=0 || (pX->sharedMask & mask)!=0 ){
        rc = SQLITE_BUSY;
        break;
      }
    }
  
    /* Get the exclusive locks at the system level.  Then if successful
    ** also mark the local connection as being locked.
    */
    if( rc==SqlResult.SQLITE_OK ){
      rc = winShmSystemLock(pShmNode, _SHM_WRLCK, ofst+WIN_SHM_BASE, n);
      if( rc==SqlResult.SQLITE_OK ){
        Debug.Assert( (p->sharedMask & mask)==0 );
        p->exclMask |= mask;
      }
    }
  }
  sqlite3_mutex_leave(pShmNode->mutex);
  OSTRACE(("SHM-LOCK shmid-%d, pid-%d got %03x,%03x %s\n",
           p->id, (int)GetCurrentProcessId(), p->sharedMask, p->exclMask,
           rc ? "failed" : "ok"));
  return rc;
}

/*
** Implement a memory barrier or memory fence on shared memory.  
**
** All loads and stores begun before the barrier must complete before
** any load or store begun after the barrier.
*/
static void winShmBarrier(
  sqlite3_file *fd          /* Database holding the shared memory */
){
  sqliteinth.UNUSED_PARAMETER(fd);
  /* MemoryBarrier(); // does not work -- do not know why not */
  winShmEnterMutex();
  winShmLeaveMutex();
}

/*
** This function is called to obtain a pointer to region iRegion of the 
** shared-memory associated with the database file fd. Shared-memory regions 
** are numbered starting from zero. Each shared-memory region is szRegion 
** bytes in size.
**
** If an error occurs, an error code is returned and *pp is set to NULL.
**
** Otherwise, if the isWrite parameter is 0 and the requested shared-memory
** region has not been allocated (by any client, including one running in a
** separate process), then *pp is set to NULL and SqlResult.SQLITE_OK returned. If 
** isWrite is non-zero and the requested shared-memory region has not yet 
** been allocated, it is allocated by this function.
**
** If the shared-memory region has already been allocated or is allocated by
** this call as described above, then it is mapped into this processes 
** address space (if it is not already), *pp is set to point to the mapped 
** memory and SqlResult.SQLITE_OK returned.
*/
static int winShmMap(
  sqlite3_file *fd,               /* Handle open on database file */
  int iRegion,                    /* Region to retrieve */
  int szRegion,                   /* Size of regions */
  int isWrite,                    /* True to extend file if necessary */
  void volatile **pp              /* OUT: Mapped memory */
){
  winFile *pDbFd = (winFile)fd;
  winShm *p = pDbFd->pShm;
  winShmNode *pShmNode;
  var rc = SqlResult.SQLITE_OK;

  if( null==p ){
    rc = winOpenSharedMemory(pDbFd);
    if( rc!=SqlResult.SQLITE_OK ) return rc;
    p = pDbFd->pShm;
  }
  pShmNode = p->pShmNode;

  sqlite3_mutex_enter(pShmNode->mutex);
  Debug.Assert( szRegion==pShmNode->szRegion || pShmNode->nRegion==0 );

  if( pShmNode->nRegion<=iRegion ){
    struct ShmRegion *apNew;           /* New aRegion[] array */
    int nByte = (iRegion+1)*szRegion;  /* Minimum required file size */
    sqlite3_int64 sz;                  /* Current size of wal-index file */

    pShmNode->szRegion = szRegion;

    /* The requested region is not mapped into this processes address space.
    ** Check to see if it has been allocated (i.e. if the wal-index file is
    ** large enough to contain the requested region).
    */
    rc = winFileSize((sqlite3_file )&pShmNode->hFile, &sz);
    if( rc!=SqlResult.SQLITE_OK ){
      rc = winLogError(SQLITE_IOERR_SHMSIZE, "winShmMap1", pDbFd->zPath);
      goto shmpage_out;
    }

    if( sz<nByte ){
      /* The requested memory region does not exist. If isWrite is set to
      ** zero, exit early. *pp will be set to NULL and SqlResult.SQLITE_OK returned.
      **
      ** Alternatively, if isWrite is non-zero, use ftruncate() to allocate
      ** the requested memory region.
      */
      if( null==isWrite ) goto shmpage_out;
      rc = winTruncate((sqlite3_file )&pShmNode->hFile, nByte);
      if( rc!=SqlResult.SQLITE_OK ){
        rc = winLogError(SQLITE_IOERR_SHMSIZE, "winShmMap2", pDbFd->zPath);
        goto shmpage_out;
      }
    }

    /* Map the requested memory region into this processes address space. */
    apNew = (struct ShmRegion )sqlite3_realloc(
        pShmNode->aRegion, (iRegion+1)*sizeof(apNew[0])
    );
    if( null==apNew ){
      rc = SQLITE_IOERR_NOMEM;
      goto shmpage_out;
    }
    pShmNode->aRegion = apNew;

    while( pShmNode->nRegion<=iRegion ){
      HANDLE hMap;                /* file-mapping handle */
      void *pMap = 0;             /* Mapped memory region */
     
      hMap = CreateFileMapping(pShmNode->hFile.h, 
          NULL, PAGE_READWRITE, 0, nByte, NULL
      );
      OSTRACE(("SHM-MAP pid-%d create region=%d nbyte=%d %s\n",
               (int)GetCurrentProcessId(), pShmNode->nRegion, nByte,
               hMap ? "ok" : "failed"));
      if( hMap ){
        int iOffset = pShmNode->nRegion*szRegion;
        int iOffsetShift = iOffset % winSysInfo.dwAllocationGranularity;
        pMap = MapViewOfFile(hMap, FILE_MAP_WRITE | FILE_MAP_READ,
            0, iOffset - iOffsetShift, szRegion + iOffsetShift
        );
        OSTRACE(("SHM-MAP pid-%d map region=%d offset=%d size=%d %s\n",
                 (int)GetCurrentProcessId(), pShmNode->nRegion, iOffset, szRegion,
                 pMap ? "ok" : "failed"));
      }
      if( null==pMap ){
        pShmNode->lastErrno = GetLastError();
        rc = winLogError(SQLITE_IOERR_SHMMAP, "winShmMap3", pDbFd->zPath);
        if( hMap ) CloseHandle(hMap);
        goto shmpage_out;
      }

      pShmNode->aRegion[pShmNode->nRegion].pMap = pMap;
      pShmNode->aRegion[pShmNode->nRegion].hMap = hMap;
      pShmNode->nRegion++;
    }
  }

shmpage_out:
  if( pShmNode->nRegion>iRegion ){
    int iOffset = iRegion*szRegion;
    int iOffsetShift = iOffset % winSysInfo.dwAllocationGranularity;
    char *p = (char )pShmNode->aRegion[iRegion].pMap;
    *pp = (void )&p[iOffsetShift];
  }else{
    *pp = 0;
  }
  sqlite3_mutex_leave(pShmNode->mutex);
  return rc;
}

#else
		//# define winShmMap     0
		static int winShmMap (sqlite3_file fd, ///
///<summary>
///Handle open on database file 
///</summary>

		int iRegion, ///
///<summary>
///Region to retrieve 
///</summary>

		int szRegion, ///
///<summary>
///Size of regions 
///</summary>

		int isWrite, ///
///<summary>
///True to extend file if necessary 
///</summary>

		out object pp///
///<summary>
///OUT: Mapped memory 
///</summary>

		)
		{
			pp = null;
			return 0;
		}

		//# define winShmLock    0
		static int winShmLock (sqlite3_file fd, ///
///<summary>
///Database file holding the shared memory 
///</summary>

		int ofst, ///
///<summary>
///First lock to acquire or release 
///</summary>

		int n, ///
///<summary>
///Number of locks to acquire or release 
///</summary>

		int flags///
///<summary>
///What to do with the lock 
///</summary>

		)
		{
			return 0;
		}

		//# define winShmBarrier 0
		static void winShmBarrier (sqlite3_file fd///
///<summary>
///Database holding the shared memory 
///</summary>

		)
		{
		}

		//# define winShmUnmap   0
		static int winShmUnmap (sqlite3_file fd, ///
///<summary>
///Database holding shared memory 
///</summary>

		int deleteFlag///
///<summary>
///Delete after closing if true 
///</summary>

		)
		{
			return 0;
		}

		#endif
		///
///<summary>
///Here ends the implementation of all sqlite3_file methods.
///
///End sqlite3_file Methods *******************************
///
///</summary>

		///
///<summary>
///This vector defines all the methods that can operate on an
///sqlite3_file for win32.
///
///</summary>

		public static sqlite3_io_methods winIoMethod = new sqlite3_io_methods (2, ///
///<summary>
///iVersion 
///</summary>

		(dxClose)winClose, ///
///<summary>
///xClose 
///</summary>

		(dxRead)winRead, ///
///<summary>
///xRead 
///</summary>

		(dxWrite)winWrite, ///
///<summary>
///xWrite 
///</summary>

		(dxTruncate)winTruncate, ///
///<summary>
///xTruncate 
///</summary>

		(dxSync)winSync, ///
///<summary>
///xSync 
///</summary>

		(dxFileSize)winFileSize, ///
///<summary>
///xFileSize 
///</summary>

		(dxLock)winLock, ///
///<summary>
///xLock 
///</summary>

		(dxUnlock)winUnlock, ///
///<summary>
///xUnlock 
///</summary>

		(dxCheckReservedLock)winCheckReservedLock, ///
///<summary>
///xCheckReservedLock 
///</summary>

		(dxFileControl)winFileControl, ///
///<summary>
///xFileControl 
///</summary>

		(dxSectorSize)winSectorSize, ///
///<summary>
///xSectorSize 
///</summary>

		(dxDeviceCharacteristics)winDeviceCharacteristics, ///
///<summary>
///xDeviceCharacteristics 
///</summary>

		(dxShmMap)winShmMap, ///
///<summary>
///xShmMap 
///</summary>

		(dxShmLock)winShmLock, ///
///<summary>
///xShmLock 
///</summary>

		(dxShmBarrier)winShmBarrier, ///
///<summary>
///xShmBarrier 
///</summary>

		(dxShmUnmap)winShmUnmap///
///<summary>
///xShmUnmap 
///</summary>

		);

		///
///<summary>
///
///sqlite3_vfs methods ****************************
///
///This division contains the implementation of methods on the
///sqlite3_vfs object.
///
///</summary>

		///
///<summary>
///</summary>
///<param name="Convert a UTF">8 filename into whatever form the underlying</param>
///<param name="operating system wants filenames in.  Space to hold the result">operating system wants filenames in.  Space to hold the result</param>
///<param name="is obtained from malloc and must be freed by the calling">is obtained from malloc and must be freed by the calling</param>
///<param name="function.">function.</param>
///<param name=""></param>

		static string convertUtf8Filename (string zFilename)
		{
			return zFilename;
			// string zConverted = "";
			//if (isNT())
			//{
			//  zConverted = utf8ToUnicode(zFilename);
			///
///<summary>
///isNT() is 1 if SQLITE_OS_WINCE==1, so this else is never executed.
///
///</summary>

			#if !SQLITE_OS_WINCE
			//}
			//else
			//{
			//  zConverted = sqlite3_win32_utf8_to_mbcs(zFilename);
			#endif
			//}
			///
///<summary>
///caller will handle out of memory 
///</summary>

			//return zConverted;
		}

		///
///<summary>
///Create a temporary file name in zBuf.  zBuf must be big enough to
///hold at pVfs.mxPathname characters.
///
///</summary>

        static SqlResult getTempname(int nBuf, StringBuilder zBuf)
		{
			const string zChars = "abcdefghijklmnopqrstuvwxyz0123456789";
			//static char zChars[] =
			//  "abcdefghijklmnopqrstuvwxyz"
			//  "ABCDEFGHIJKLMNOPQRSTUVWXYZ"
			//  "0123456789";
			//size_t i, j;
			//char zTempPath[MAX_PATH+1];
			///
///<summary>
///</summary>
///<param name="It's odd to simulate an io">error here, but really this is just</param>
///<param name="using the io">error infrastructure to test that SQLite handles this</param>
///<param name="function failing. ">function failing. </param>
///<param name=""></param>

			#if SQLITE_TEST
																																																									      if ( SimulateIOError() )
        return SQLITE_IOERR;
#endif
			//if( sqlite3_temp_directory ){
			//  io.sqlite3_snprintf(MAX_PATH-30, zTempPath, "%s", sqlite3_temp_directory);
			//}else if( isNT() ){
			//  string zMulti;
			//  WCHAR zWidePath[MAX_PATH];
			//  GetTempPathW(MAX_PATH-30, zWidePath);
			//  zMulti = unicodeToUtf8(zWidePath);
			//  if( zMulti ){
			//    io.sqlite3_snprintf(MAX_PATH-30, zTempPath, "%s", zMulti);
			//    free(zMulti);
			//  }else{
			//    return SQLITE_NOMEM;
			//  }
			///
///<summary>
///isNT() is 1 if SQLITE_OS_WINCE==1, so this else is never executed.
///Since the ASCII version of these Windows API do not exist for WINCE,
///it's important to not reference them for WINCE builds.
///
///</summary>

			#if !SQLITE_OS_WINCE
			//}else{
			//  string zUtf8;
			//  char zMbcsPath[MAX_PATH];
			//  GetTempPathA(MAX_PATH-30, zMbcsPath);
			//  zUtf8 = sqlite3_win32_mbcs_to_utf8(zMbcsPath);
			//  if( zUtf8 ){
			//    io.sqlite3_snprintf(MAX_PATH-30, zTempPath, "%s", zUtf8);
			//    free(zUtf8);
			//  }else{
			//    return SQLITE_NOMEM;
			//  }
			#endif
			//}
			///
///<summary>
///Check that the output buffer is large enough for the temporary file 
///name. If it is not, return SqlResult.SQLITE_ERROR.
///
///</summary>

			//if( (StringExtensions.sqlite3Strlen30(zTempPath) + StringExtensions.sqlite3Strlen30(SQLITE_TEMP_FILE_PREFIX) + 17) >= nBuf ){
			//  return SqlResult.SQLITE_ERROR;
			//}
			StringBuilder zRandom = new StringBuilder (20);
			i64 iRandom = 0;
			for (int i = 0; i < 15; i++) {
				sqlite3_randomness (1, ref iRandom);
				zRandom.Append ((char)zChars [(int)(iRandom % (zChars.Length - 1))]);
			}
			//  zBuf[j] = 0;
			zBuf.Append (Path.GetTempPath () + SQLITE_TEMP_FILE_PREFIX + zRandom.ToString ());
			//for(i=StringExtensions.sqlite3Strlen30(zTempPath); i>0 && zTempPath[i-1]=='\\'; i--){}
			//zTempPath[i] = 0;
			//sqlite3_snprintf(nBuf-17, zBuf,
			//                 "%s\\"SQLITE_TEMP_FILE_PREFIX, zTempPath);
			//j = StringExtensions.sqlite3Strlen30(zBuf);
			//sqlite3_randomness(15, zBuf[j]);
			//for(i=0; i<15; i++, j++){
			//  zBuf[j] = (char)zChars[ ((unsigned char)zBuf[j])%(sizeof(zChars)-1) ];
			//}
			//zBuf[j] = 0;
			#if SQLITE_DEBUG
																																																									      OSTRACE( "TEMP FILENAME: %s\n", zBuf.ToString() );
#endif
			return SqlResult.SQLITE_OK;
		}

		///
///<summary>
///Open a file.
///
///</summary>

		static SqlResult winOpen (sqlite3_vfs pVfs, ///
///<summary>
///Not used 
///</summary>

		string zName, ///
///<summary>
///</summary>
///<param name="Name of the file (UTF">8) </param>

		sqlite3_file pFile, ///
///<summary>
///Write the SQLite file handle here 
///</summary>

		int flags, ///
///<summary>
///Open mode flags 
///</summary>

		out int pOutFlags///
///<summary>
///Status return flags 
///</summary>

		)
		{
			//HANDLE h;
			FileStream fs = null;
			FileAccess dwDesiredAccess;
			FileShare dwShareMode;
			FileMode dwCreationDisposition;
			#if !(SQLITE_SILVERLIGHT || WINDOWS_MOBILE)
			FileOptions dwFlagsAndAttributes;
			#endif
			#if SQLITE_OS_WINCE
																																																									int isTemp = 0;
#endif
			//winFile* pFile = (winFile)id;
			string zConverted;
///Filename in OS encoding 

			string zUtf8Name = zName;
///<param name="Filename in UTF">8 encoding </param>

			pOutFlags = 0;
///If argument zPath is a NULL pointer, this function is required to open
///a temporary file. Use this buffer to store the file name in.

			StringBuilder zTmpname = new StringBuilder (MAX_PATH + 1);
///Buffer used to create temp filename 

			var rc = SqlResult.SQLITE_OK;
///Function Return Code 

			int eType = (int)(flags & 0xFFFFFF00);
///Type of file to open 

			bool isExclusive = (flags & SQLITE_OPEN_EXCLUSIVE) != 0;
			bool isDelete = (flags & SQLITE_OPEN_DELETEONCLOSE) != 0;
			bool isCreate = (flags & SQLITE_OPEN_CREATE) != 0;
			bool isReadonly = (flags & SQLITE_OPEN_READONLY) != 0;
			bool isReadWrite = (flags & SQLITE_OPEN_READWRITE) != 0;
			bool isOpenJournal = (isCreate && (eType == SQLITE_OPEN_MASTER_JOURNAL || eType == SQLITE_OPEN_MAIN_JOURNAL || eType == SQLITE_OPEN_WAL));
///Check the following statements are true:
///(a) Exactly one of the READWRITE and READONLY flags must be set, and
///(b) if CREATE is set, then READWRITE must also be set, and
///(c) if EXCLUSIVE is set, then CREATE must also be set.
///(d) if DELETEONCLOSE is set, then CREATE must also be set.

			Debug.Assert ((isReadonly == false || isReadWrite == false) && (isReadWrite || isReadonly));
			Debug.Assert (isCreate == false || isReadWrite);
			Debug.Assert (isExclusive == false || isCreate);
			Debug.Assert (isDelete == false || isCreate);
			///
///<summary>
///The main DB, main journal, WAL file and master journal are never
///automatically deleted. Nor are they ever temporary files.  
///</summary>

			//Debug.Assert( ( !isDelete && !String.IsNullOrEmpty(zName) ) || eType != SQLITE_OPEN_MAIN_DB );
			Debug.Assert ((!isDelete && !String.IsNullOrEmpty (zName)) || eType != SQLITE_OPEN_MAIN_JOURNAL);
			Debug.Assert ((!isDelete && !String.IsNullOrEmpty (zName)) || eType != SQLITE_OPEN_MASTER_JOURNAL);
			Debug.Assert ((!isDelete && !String.IsNullOrEmpty (zName)) || eType != SQLITE_OPEN_WAL);
			///
///<summary>
///</summary>
///<param name="Assert that the upper layer has set one of the "file">type" flags. </param>

			Debug.Assert (eType == SQLITE_OPEN_MAIN_DB || eType == SQLITE_OPEN_TEMP_DB || eType == SQLITE_OPEN_MAIN_JOURNAL || eType == SQLITE_OPEN_TEMP_JOURNAL || eType == SQLITE_OPEN_SUBJOURNAL || eType == SQLITE_OPEN_MASTER_JOURNAL || eType == SQLITE_OPEN_TRANSIENT_DB || eType == SQLITE_OPEN_WAL);
			Debug.Assert (pFile != null);
			sqliteinth.UNUSED_PARAMETER (pVfs);
			pFile.fs = null;
			//.h = INVALID_HANDLE_VALUE;
			///
///<summary>
///If the second argument to this function is NULL, generate a
///temporary file name to use
///
///</summary>

			if (String.IsNullOrEmpty (zUtf8Name)) {
				Debug.Assert (isDelete && !isOpenJournal);
				rc = getTempname (MAX_PATH + 1, zTmpname);
				if (rc != SqlResult.SQLITE_OK) {
					return rc;
				}
				zUtf8Name = zTmpname.ToString ();
			}
			// /* Convert the filename to the system encoding. */
			zConverted = zUtf8Name;
			// convertUtf8Filename( zUtf8Name );
			if (zConverted.StartsWith ("/") && !zConverted.StartsWith ("//"))
				zConverted = zConverted.Substring (1);
			//if ( String.IsNullOrEmpty( zConverted ) )
			//{
			//  return SQLITE_NOMEM;
			//}
			if (isReadWrite) {
				dwDesiredAccess = FileAccess.Read | FileAccess.Write;
				// GENERIC_READ | GENERIC_WRITE;
			}
			else {
				dwDesiredAccess = FileAccess.Read;
				// GENERIC_READ;
			}
			///
///<summary>
///SQLITE_OPEN_EXCLUSIVE is used to make sure that a new file is
///created. SQLite doesn't use it to indicate "exclusive access"
///as it is usually understood.
///
///</summary>

			if (isExclusive) {
				///
///<summary>
///Creates a new file, only if it does not already exist. 
///</summary>

				///
///<summary>
///If the file exists, it fails. 
///</summary>

				dwCreationDisposition = FileMode.CreateNew;
				// CREATE_NEW;
			}
			else
				if (isCreate) {///Open existing file, or create if it doesn't exist 

					dwCreationDisposition = FileMode.OpenOrCreate;
					// OPEN_ALWAYS;
				}
				else {///Opens a file, only if it exists. 

					dwCreationDisposition = FileMode.Open;
					//OPEN_EXISTING;
				}
			dwShareMode = FileShare.Read | FileShare.Write;
			// FILE_SHARE_READ | FILE_SHARE_WRITE;
			if (isDelete) {
				#if SQLITE_OS_WINCE
																																																																												dwFlagsAndAttributes = FILE_ATTRIBUTE_HIDDEN;
isTemp = 1;
#else
				#if !(SQLITE_SILVERLIGHT || WINDOWS_MOBILE)
				dwFlagsAndAttributes = FileOptions.DeleteOnClose;
				// FILE_ATTRIBUTE_TEMPORARY
				//| FILE_ATTRIBUTE_HIDDEN
				//| FILE_FLAG_DELETE_ON_CLOSE;
				#endif
				#endif
			}
			else {
				#if !(SQLITE_SILVERLIGHT || WINDOWS_MOBILE)
				dwFlagsAndAttributes = FileOptions.None;
				// FILE_ATTRIBUTE_NORMAL;
				#endif
			}
///Reports from the internet are that performance is always
///better if FILE_FLAG_RANDOM_ACCESS is used.  Ticket #2699. 

			#if SQLITE_OS_WINCE
																																																									dwFlagsAndAttributes |= FileOptions.RandomAccess; // FILE_FLAG_RANDOM_ACCESS;
#endif
			if (isNT ()) {
				//h = CreateFileW((WCHAR)zConverted,
				//   dwDesiredAccess,
				//   dwShareMode,
				//   NULL,
				//   dwCreationDisposition,
				//   dwFlagsAndAttributes,
				//   NULL
				//);
				//
				// retry opening the file a few times; this is because of a racing condition between a delete and open call to the FS
				//
				int retries = 3;
				while ((fs == null) && (retries > 0))
					try {
						retries--;
						#if WINDOWS_PHONE || SQLITE_SILVERLIGHT
																																																																																																																		 fs = new IsolatedStorageFileStream(zConverted, dwCreationDisposition, dwDesiredAccess, dwShareMode, IsolatedStorageFile.GetUserStoreForApplication());
#elif !(SQLITE_SILVERLIGHT || WINDOWS_MOBILE)
						fs = new FileStream (zConverted, dwCreationDisposition, dwDesiredAccess, dwShareMode, 4096, dwFlagsAndAttributes);
						#else
																																																																																																																		            fs = new FileStream( zConverted, dwCreationDisposition, dwDesiredAccess, dwShareMode, 4096);
#endif
						#if SQLITE_DEBUG
																																																																																																																		            OSTRACE( "OPEN %d (%s)\n", fs.GetHashCode(), fs.Name );
#endif
					}
					catch (Exception e) {
						Thread.Sleep (100);
					}
				///
///<summary>
///isNT() is 1 if SQLITE_OS_WINCE==1, so this else is never executed.
///Since the ASCII version of these Windows API do not exist for WINCE,
///it's important to not reference them for WINCE builds.
///
///</summary>

				#if !SQLITE_OS_WINCE
			}
			else {
				Debugger.Break ();
				// Not NT
				//h = CreateFileA((char)zConverted,
				//   dwDesiredAccess,
				//   dwShareMode,
				//   NULL,
				//   dwCreationDisposition,
				//   dwFlagsAndAttributes,
				//   NULL
				//);
				#endif
			}
			OSTRACE ("OPEN %d %s 0x%lx %s\n", pFile.GetHashCode (), zName, dwDesiredAccess, fs == null ? "failed" : "ok");
			if (fs == null || 
			#if !(SQLITE_SILVERLIGHT || WINDOWS_MOBILE)
			fs.SafeFileHandle.IsInvalid
			#else
																																																									 !fs.CanRead
#endif
			)//(h == INVALID_HANDLE_VALUE)
			 {
				#if SQLITE_SILVERLIGHT
																																																																												pFile.lastErrno = 1;
#else
				//      pFile.lastErrno = GetLastError();
				pFile.lastErrno = (u32)Marshal.GetLastWin32Error ();
				#endif
                winLogError(SqlResult.SQLITE_CANTOPEN, "winOpen", zUtf8Name);
				//        free(zConverted);
				if (isReadWrite) {
					return winOpen (pVfs, zName, pFile, ((flags | SQLITE_OPEN_READONLY) & ~(SQLITE_OPEN_CREATE | SQLITE_OPEN_READWRITE)), out pOutFlags);
				}
				else {
                    return sqliteinth.SQLITE_CANTOPEN_BKPT();
				}
			}
			//if ( pOutFlags )
			//{
			if (isReadWrite) {
				pOutFlags = SQLITE_OPEN_READWRITE;
			}
			else {
				pOutFlags = SQLITE_OPEN_READONLY;
			}
			//}
			pFile.Clear ();
			// memset(pFile, 0, sizeof(*pFile));
			pFile.pMethods = winIoMethod;
			pFile.fs = fs;
			pFile.lastErrno = NO_ERROR;
			pFile.pVfs = pVfs;
			pFile.pShm = null;
			pFile.zPath = zName;
			pFile.sectorSize = (ulong)getSectorSize (pVfs, zUtf8Name);
			#if SQLITE_OS_WINCE
																																																									if( isReadWrite && eType==SQLITE_OPEN_MAIN_DB
&& !winceCreateLock(zName, pFile)
){
CloseHandle(h);
free(zConverted);
return  sqliteinth.SQLITE_CANTOPEN_BKPT;
}
if( isTemp ){
pFile.zDeleteOnClose = zConverted;
}else
#endif
			{
				// free(zConverted);
				#if SQLITE_TEST
																																																																											      OpenCounter( +1 );
#endif
			}
			return rc;
		}

		///
///<summary>
///Delete the named file.
///
///Note that windows does not allow a file to be deleted if some other
///process has it open.  Sometimes a virus scanner or indexing program
///will open a journal file shortly after it is created in order to do
///whatever it does.  While this other process is holding the
///file open, we will be unable to delete it.  To work around this
///problem, we delay 100 milliseconds and try to delete again.  Up
///to MX_DELETION_ATTEMPTs deletion attempts are run before giving
///up and returning an error.
///
///</summary>

		static int MX_DELETION_ATTEMPTS = 5;

		static SqlResult winDelete (sqlite3_vfs pVfs, ///
///<summary>
///Not used on win32 
///</summary>

		string zFilename, ///
///<summary>
///Name of file to delete 
///</summary>

		int syncDir///
///<summary>
///Not used on win32 
///</summary>

		)
		{
			int cnt = 0;
            SqlResult rc;
			int error;
			string zConverted;
			sqliteinth.UNUSED_PARAMETER (pVfs);
			sqliteinth.UNUSED_PARAMETER (syncDir);
			#if SQLITE_TEST
																																																									      if ( SimulateIOError() )
        return SQLITE_IOERR_DELETE;
#endif
			zConverted = convertUtf8Filename (zFilename);
			//if ( zConverted == null || zConverted == "" )
			//{
			//  return SQLITE_NOMEM;
			//}
			if (isNT ()) {
				do//  DeleteFileW(zConverted);
				//}while(   (   ((rc = GetFileAttributesW(zConverted)) != INVALID_FILE_ATTRIBUTES)
				//           || ((error = GetLastError()) == ERROR_ACCESS_DENIED))
				//       && (++cnt < MX_DELETION_ATTEMPTS)
				//       && (Sleep(100), 1) );
				 {
					#if WINDOWS_PHONE
																																																																																															           if ( !System.IO.IsolatedStorage.IsolatedStorageFile.GetUserStoreForApplication().FileExists( zFilename ) )
#elif SQLITE_SILVERLIGHT
																																																																																															            if (!IsolatedStorageFile.GetUserStoreForApplication().FileExists(zFilename))
#else
					if (!File.Exists (zFilename))
					#endif
					 {
						rc = SqlResult.SQLITE_IOERR;
						break;
					}
					try {
						#if WINDOWS_PHONE
																																																																																																																		              System.IO.IsolatedStorage.IsolatedStorageFile.GetUserStoreForApplication().DeleteFile(zFilename);
#elif SQLITE_SILVERLIGHT
																																																																																																																		              IsolatedStorageFile.GetUserStoreForApplication().DeleteFile(zFilename);
#else
						File.Delete (zConverted);
						#endif
						rc = SqlResult.SQLITE_OK;
					}
					catch (IOException e) {
						rc = SqlResult.SQLITE_IOERR;
						Thread.Sleep (100);
					}
				}
				while (rc != SqlResult.SQLITE_OK && ++cnt < MX_DELETION_ATTEMPTS);
				///
///<summary>
///isNT() is 1 if SQLITE_OS_WINCE==1, so this else is never executed.
///Since the ASCII version of these Windows API do not exist for WINCE,
///it's important to not reference them for WINCE builds.
///
///</summary>

				#if !SQLITE_OS_WINCE
			}
			else {
				do {
					//DeleteFileA( zConverted );
					//}while(   (   ((rc = GetFileAttributesA(zConverted)) != INVALID_FILE_ATTRIBUTES)
					//           || ((error = GetLastError()) == ERROR_ACCESS_DENIED))
					//       && (cnt++ < MX_DELETION_ATTEMPTS)
					//       && (Sleep(100), 1) );
					if (!File.Exists (zFilename)) {
						rc = SqlResult.SQLITE_IOERR;
						break;
					}
					try {
						File.Delete (zConverted);
						rc = SqlResult.SQLITE_OK;
					}
					catch (IOException e) {
						rc = SqlResult.SQLITE_IOERR;
						Thread.Sleep (100);
					}
				}
				while (rc != SqlResult.SQLITE_OK && cnt++ < MX_DELETION_ATTEMPTS);
				#endif
			}
			//free(zConverted);
			#if SQLITE_DEBUG
																																																									      OSTRACE( "DELETE \"%s\"\n", zFilename );
#endif
			if (rc == SqlResult.SQLITE_OK)
				return rc;
			#if SQLITE_SILVERLIGHT
																																																									      error = (int)ERROR_NOT_SUPPORTED;
#else
			error = Marshal.GetLastWin32Error ();
			#endif
            return ((rc == (SqlResult)Sqlite3.INVALID_FILE_ATTRIBUTES) && (error == _Custom.ERROR_FILE_NOT_FOUND)) ? SqlResult.SQLITE_OK : winLogError(SqlResult.SQLITE_IOERR_DELETE, "winDelete", zFilename);
		}

		///
///<summary>
///Check the existence and status of a file.
///
///</summary>

		static SqlResult winAccess (sqlite3_vfs pVfs, ///
///<summary>
///Not used on win32 
///</summary>

		string zFilename, ///
///<summary>
///Name of file to check 
///</summary>

		SQLITE_ACCESS flags, ///
///<summary>
///Type of test to make on this file 
///</summary>

		out int pResOut///
///<summary>
///OUT: Result 
///</summary>

		)
		{
			FileAttributes attr = 0;
			// DWORD attr;
			var rc = 0;
			//  void *zConverted;
			sqliteinth.UNUSED_PARAMETER (pVfs);
			#if SQLITE_TEST
																																																									      if ( SimulateIOError() )
      {
        pResOut = -1;
        return SQLITE_IOERR_ACCESS;
      }
#endif
			//zConverted = convertUtf8Filename(zFilename);
			//  if( zConverted==0 ){
			//    return SQLITE_NOMEM;
			//  }
			//if ( isNT() )
			//{
			//
			// Do a quick test to prevent the try/catch block
			if (flags == SQLITE_ACCESS.EXISTS) {
				#if WINDOWS_PHONE
																																																																												          pResOut = System.IO.IsolatedStorage.IsolatedStorageFile.GetUserStoreForApplication().FileExists(zFilename) ? 1 : 0;
#elif SQLITE_SILVERLIGHT
																																																																												          pResOut = IsolatedStorageFile.GetUserStoreForApplication().FileExists(zFilename) ? 1 : 0;
#else
				pResOut = File.Exists (zFilename) ? 1 : 0;
				#endif
				return SqlResult.SQLITE_OK;
			}
			//
			try {
				//WIN32_FILE_ATTRIBUTE_DATA sAttrData;
				//memset(&sAttrData, 0, sizeof(sAttrData));
				//if( GetFileAttributesExW((WCHAR)zConverted,
				//                         GetFileExInfoStandard, 
				//                         &sAttrData) ){
				//  /* For an SQLITE_ACCESS_EXISTS query, treat a zero-length file
				//  ** as if it does not exist.
				//  */
				//  if(    flags==SQLITE_ACCESS_EXISTS
				//      && sAttrData.nFileSizeHigh==0 
				//      && sAttrData.nFileSizeLow==0 ){
				//    attr = INVALID_FILE_ATTRIBUTES;
				//  }else{
				//    attr = sAttrData.dwFileAttributes;
				//  }
				//}else{
				//  if( GetLastError()!=ERROR_FILE_NOT_FOUND ){
				//    winLogError(SQLITE_IOERR_ACCESS, "winAccess", zFilename);
				//    free(zConverted);
				//    return SQLITE_IOERR_ACCESS;
				//  }else{
				//    attr = INVALID_FILE_ATTRIBUTES;
				//  }
				//}
				#if WINDOWS_PHONE || WINDOWS_MOBILE || SQLITE_SILVERLIGHT
																																																																												        if (new DirectoryInfo(zFilename).Exists)
#else
				attr = File.GetAttributes (zFilename);
				// GetFileAttributesW( (WCHAR)zConverted );
				if (attr == FileAttributes.Directory)
				#endif
				 {
					try {
						string name = Path.Combine (Path.GetTempPath (), Path.GetTempFileName ());
						FileStream fs = File.Create (name);
						fs.Close ();
						File.Delete (name);
						attr = FileAttributes.Normal;
					}
					catch (IOException e) {
						attr = FileAttributes.ReadOnly;
					}
				}
			}
			///
///<summary>
///isNT() is 1 if SQLITE_OS_WINCE==1, so this else is never executed.
///Since the ASCII version of these Windows API do not exist for WINCE,
///it's important to not reference them for WINCE builds.
///
///</summary>

			#if !SQLITE_OS_WINCE
			//}
			//else
			//{
			//  attr = GetFileAttributesA( (char)zConverted );
			#endif
			//}
			catch (IOException e) {
				winLogError (SqlResult.SQLITE_IOERR_ACCESS, "winAccess", zFilename);
			}
			//  free(zConverted);
			switch (flags) {
			case SQLITE_ACCESS.READ:
			case SQLITE_ACCESS.EXISTS:
				rc = attr != 0 ? 1 : 0;
				// != INVALID_FILE_ATTRIBUTES;
				break;
			case SQLITE_ACCESS.READWRITE:
				rc = attr == 0 ? 0 : (int)(attr & FileAttributes.ReadOnly) != 0 ? 0 : 1;
				//FILE_ATTRIBUTE_READONLY ) == 0;
				break;
			default:
				Debug.Assert ("" == "Invalid flags argument");
				rc = 0;
				break;
			}
			pResOut = rc;
			return SqlResult.SQLITE_OK;
		}

		///
///<summary>
///Turn a relative pathname into a full pathname.  Write the full
///pathname into zOut[].  zOut[] will be at least pVfs.mxPathname
///bytes in size.
///
///</summary>

		static SqlResult winFullPathname (sqlite3_vfs pVfs, ///
///<summary>
///Pointer to vfs object 
///</summary>

		string zRelative, ///
///<summary>
///Possibly relative input path 
///</summary>

		int nFull, ///
///<summary>
///Size of output buffer in bytes 
///</summary>

		StringBuilder zFull///
///<summary>
///Output buffer 
///</summary>

		)
		{
			#if __CYGWIN__
																																																									SimulateIOError( return SqlResult.SQLITE_ERROR );
sqliteinth.UNUSED_PARAMETER(nFull);
cygwin_conv_to_full_win32_path(zRelative, zFull);
return SqlResult.SQLITE_OK;
#endif
			#if SQLITE_OS_WINCE
																																																									SimulateIOError( return SqlResult.SQLITE_ERROR );
sqliteinth.UNUSED_PARAMETER(nFull);
/* WinCE has no concept of a relative pathname, or so I am told. */
sqlite3_snprintf(pVfs.mxPathname, zFull, "%s", zRelative);
return SqlResult.SQLITE_OK;
#endif
			#if !SQLITE_OS_WINCE && !__CYGWIN__
			int nByte;
			//string  zConverted;
			string zOut = null;
			///
///<summary>
///If this path name begins with "/X:", where "X" is any alphabetic
///character, discard the initial "/" from the pathname.
///
///</summary>

			if (zRelative [0] == '/' && Char.IsLetter (zRelative [1]) && zRelative [2] == ':') {
				zRelative = zRelative.Substring (1);
			}
			///
///<summary>
///</summary>
///<param name="It's odd to simulate an io">error here, but really this is just</param>
///<param name="using the io">error infrastructure to test that SQLite handles this</param>
///<param name="function failing. This function could fail if, for example, the">function failing. This function could fail if, for example, the</param>
///<param name="current working directory has been unlinked.">current working directory has been unlinked.</param>
///<param name=""></param>

			#if SQLITE_TEST
																																																									      if ( SimulateIOError() )
        return SqlResult.SQLITE_ERROR;
#endif
			sqliteinth.UNUSED_PARAMETER (nFull);
			//convertUtf8Filename(zRelative));
			if (isNT ()) {
				//string zTemp;
				//nByte = GetFullPathNameW( zConverted, 0, 0, 0) + 3;
				//zTemp = malloc( nByte*sizeof(zTemp[0]) );
				//if( zTemp==0 ){
				//  free(zConverted);
				//  return SQLITE_NOMEM;
				//}
				//zTemp = GetFullPathNameW(zConverted, nByte, zTemp, 0);
				// will happen on exit; was   free(zConverted);
				try {
					#if WINDOWS_PHONE || SQLITE_SILVERLIGHT
																																																																																															          zOut = zRelative;
#else
					zOut = Path.GetFullPath (zRelative);
					// was unicodeToUtf8(zTemp);
					#endif
				}
				catch (Exception e) {
					zOut = zRelative;
				}
				// will happen on exit; was   free(zTemp);
				///
///<summary>
///isNT() is 1 if SQLITE_OS_WINCE==1, so this else is never executed.
///Since the ASCII version of these Windows API do not exist for WINCE,
///it's important to not reference them for WINCE builds.
///
///</summary>

				#if !SQLITE_OS_WINCE
			}
			else {
				Debugger.Break ();
				// -- Not Running under NT
				//string zTemp;
				//nByte = GetFullPathNameA(zConverted, 0, 0, 0) + 3;
				//zTemp = malloc( nByte*sizeof(zTemp[0]) );
				//if( zTemp==0 ){
				//  free(zConverted);
				//  return SQLITE_NOMEM;
				//}
				//GetFullPathNameA( zConverted, nByte, zTemp, 0);
				// free(zConverted);
				//zOut = sqlite3_win32_mbcs_to_utf8(zTemp);
				// free(zTemp);
				#endif
			}
			if (zOut != null) {
				// io.sqlite3_snprintf(pVfs.mxPathname, zFull, "%s", zOut);
				if (zFull.Length > pVfs.mxPathname)
					zFull.Length = pVfs.mxPathname;
				zFull.Append (zOut);
				// will happen on exit; was   free(zOut);
				return SqlResult.SQLITE_OK;
			}
			else {
				return SqlResult.SQLITE_NOMEM;
			}
			#endif
		}

		///
///<summary>
///Get the sector size of the device used to store
///file.
///
///</summary>

		static int getSectorSize (sqlite3_vfs pVfs, string zRelative///
///<summary>
///</summary>
///<param name="UTF">8 file name </param>

		)
		{
			#if FALSE
																																																									int bytesPerSector = SQLITE_DEFAULT_SECTOR_SIZE;
/* GetDiskFreeSpace is not supported under WINCE */
#if SQLITE_OS_WINCE
																																																									sqliteinth.UNUSED_PARAMETER(pVfs);
sqliteinth.UNUSED_PARAMETER(zRelative);
#else
																																																									StringBuilder zFullpath = new StringBuilder( MAX_PATH + 1 );
int rc;
//bool dwRet = false;
//int dwDummy = 0;

/*
** We need to get the full path name of the file
** to get the drive letter to look up the sector
** size.
*/
SimulateIOErrorBenign(1);
rc = winFullPathname( pVfs, zRelative, MAX_PATH, zFullpath );
#if SQLITE_TEST
																																																									SimulateIOError( return SqlResult.SQLITE_ERROR )
#endif
																																																									if ( rc == SqlResult.SQLITE_OK )
{
StringBuilder zConverted = new StringBuilder( convertUtf8Filename( zFullpath.ToString() ) );
if ( zConverted.Length != 0 )
{
if ( isNT() )
{
/* trim path to just drive reference */
//for ( ; *p ; p++ )
//{
//  if ( *p == '\\' )
//  {
//    *p = '\0';
//    break;
//  }
//}
int i;
for ( i = 0 ; i < zConverted.Length && i < MAX_PATH ; i++ )
{
if ( zConverted[i] == '\\' )
{
i++;
break;
}
}
zConverted.Length = i;
//dwRet = GetDiskFreeSpace( zConverted,
//     ref dwDummy,
//     ref bytesPerSector,
//     ref dwDummy,
//     ref dwDummy );
//}else{
//  /* trim path to just drive reference */
//   char *p = (char )zConverted;
//  for ( ; *p ; p++ )
//  {
//    if ( *p == '\\' )
//    {
//      *p = '\0';
//      break;
//    }
//  }
//        dwRet = GetDiskFreeSpaceA((char)zConverted,
//                                  dwDummy,
//                                  ref bytesPerSector,
//                                  dwDummy,
//                                  dwDummy );
}
//free(zConverted);
}
//  if ( !dwRet )
//  {
//    bytesPerSector = SQLITE_DEFAULT_SECTOR_SIZE;
//  }
//}
//endif
bytesPerSector = GetbytesPerSector( zConverted );
}
#endif
																																																									return bytesPerSector == 0 ? SQLITE_DEFAULT_SECTOR_SIZE : bytesPerSector;
#endif
			return SQLITE_DEFAULT_SECTOR_SIZE;
		}

		#if !SQLITE_OMIT_LOAD_EXTENSION
		///
///<summary>
///Interfaces for opening a shared library, finding entry points
///within the shared library, and closing the shared library.
///</summary>

		///
///<summary>
///Interfaces for opening a shared library, finding entry points
///within the shared library, and closing the shared library.
///
///</summary>

		//static void winDlOpen(sqlite3_vfs pVfs, string zFilename){
		//  HANDLE h;
		//  void *zConverted = convertUtf8Filename(zFilename);
		//  sqliteinth.UNUSED_PARAMETER(pVfs);
		//  if( zConverted==0 ){
		//    return 0;
		//  }
		//  if( isNT() ){
		//    h = LoadLibraryW((WCHAR)zConverted);
		///
///<summary>
///isNT() is 1 if SQLITE_OS_WINCE==1, so this else is never executed.
///Since the ASCII version of these Windows API do not exist for WINCE,
///it's important to not reference them for WINCE builds.
///
///</summary>

		#if !SQLITE_OS_WINCE
		//  }else{
		//    h = LoadLibraryA((char)zConverted);
		#endif
		//  }
		//  free(zConverted);
		//  return (void)h;
		//}
		//static void winDlError(sqlite3_vfs pVfs, int nBuf, string zBufOut){
		//  sqliteinth.UNUSED_PARAMETER(pVfs);
		//  getLastErrorMsg(nBuf, zBufOut);
		//}
		//    static object winDlSym(sqlite3_vfs pVfs, HANDLE pHandle, String zSymbol){
		//  sqliteinth.UNUSED_PARAMETER(pVfs);
		//#if SQLITE_OS_WINCE
		//      /* The GetProcAddressA() routine is only available on wince. */
		//      return GetProcAddressA((HANDLE)pHandle, zSymbol);
		//#else
		//     /* All other windows platforms expect GetProcAddress() to take
		//      ** an Ansi string regardless of the _UNICODE setting */
		//      return GetProcAddress((HANDLE)pHandle, zSymbol);
		//#endif
		//   }
		//    static void winDlClose( sqlite3_vfs pVfs, HANDLE pHandle )
		//   {
		//  sqliteinth.UNUSED_PARAMETER(pVfs);
		//     FreeLibrary((HANDLE)pHandle);
		//   }
		//TODO -- Fix This
		static HANDLE winDlOpen (sqlite3_vfs vfs, string zFilename)
		{
			return new HANDLE ();
		}

		static int winDlError (sqlite3_vfs vfs, int nByte, string zErrMsg)
		{
			return 0;
		}

		static HANDLE winDlSym (sqlite3_vfs vfs, HANDLE data, string zSymbol)
		{
			return new HANDLE ();
		}

		static int winDlClose (sqlite3_vfs vfs, HANDLE data)
		{
			return 0;
		}

		#else
																																						static object winDlOpen(ref sqlite3_vfs vfs, string zFilename) { return null; }
static int winDlError(ref sqlite3_vfs vfs, int nByte, ref string zErrMsg) { return 0; }
static object winDlSym(ref sqlite3_vfs vfs, object data, string zSymbol) { return null; }
static int winDlClose(ref sqlite3_vfs vfs, object data) { return 0; }
#endif
		///
///<summary>
///Write up to nBuf bytes of randomness into zBuf.
///</summary>

		//[StructLayout( LayoutKind.Explicit, Size = 16, CharSet = CharSet.Ansi )]
		//public class _SYSTEMTIME
		//{
		//  [FieldOffset( 0 )]
		//  public u32 byte_0_3;
		//  [FieldOffset( 4 )]
		//  public u32 byte_4_7;
		//  [FieldOffset( 8 )]
		//  public u32 byte_8_11;
		//  [FieldOffset( 12 )]
		//  public u32 byte_12_15;
		//}
		//[DllImport( "Kernel32.dll" )]
		//private static extern bool QueryPerformanceCounter( out long lpPerformanceCount );
		static int winRandomness (sqlite3_vfs pVfs, int nBuf, byte[] zBuf)
		{
			int n = 0;
			sqliteinth.UNUSED_PARAMETER (pVfs);
			#if (SQLITE_TEST)
																																																									      n = nBuf;
      Array.Clear( zBuf, 0, n );// memset( zBuf, 0, nBuf );
#else
			byte[] sBuf = BitConverter.GetBytes (System.DateTime.Now.Ticks);
			zBuf [0] = sBuf [0];
			zBuf [1] = sBuf [1];
			zBuf [2] = sBuf [2];
			zBuf [3] = sBuf [3];
			;
			// memcpy(&zBuf[n], x, sizeof(x))
			n += 16;
			// sizeof(x);
			if (sizeof(DWORD) <= nBuf - n) {
				//DWORD pid = GetCurrentProcessId();
				u32 processId;
				#if !SQLITE_SILVERLIGHT
				processId = (u32)Process.GetCurrentProcess ().Id;
				#else
																																																																												processId = 28376023;
#endif
				Converter.put32bits (zBuf, n, processId);
				//(memcpy(&zBuf[n], pid, sizeof(pid));
				n += 4;
				// sizeof(pid);
			}
			if (sizeof(DWORD) <= nBuf - n) {
				//DWORD cnt = GetTickCount();
				System.DateTime dt = new System.DateTime ();
				Converter.put32bits (zBuf, n, (u32)dt.Ticks);
				// memcpy(&zBuf[n], cnt, sizeof(cnt));
				n += 4;
				// cnt.Length;
			}
			if (sizeof(long) <= nBuf - n) {
				long i;
				i = System.DateTime.UtcNow.Millisecond;
				// QueryPerformanceCounter(out i);
				Converter.put32bits (zBuf, n, (u32)(i & 0xFFFFFFFF));
				//memcpy(&zBuf[n], i, sizeof(i));
				Converter.put32bits (zBuf, n, (u32)(i >> 32));
				n += sizeof(long);
			}
			#endif
			return n;
		}

		///
///<summary>
///Sleep for a little while.  Return the amount of time slept.
///
///</summary>

		static int winSleep (sqlite3_vfs pVfs, int microsec)
		{
			Thread.Sleep (((microsec + 999) / 1000));
			sqliteinth.UNUSED_PARAMETER (pVfs);
			return ((microsec + 999) / 1000) * 1000;
		}

		///
///<summary>
///</summary>
///<param name="The following variable, if set to a non">zero value, is interpreted as</param>
///<param name="the number of seconds since 1970 and is used to set the result of">the number of seconds since 1970 and is used to set the result of</param>
///<param name="sqlite3OsCurrentTime() during testing.">sqlite3OsCurrentTime() during testing.</param>
///<param name=""></param>

		#if SQLITE_TEST
																																						#if !TCLSH
																																						    static int sqlite3_current_time = 0;//  /* Fake system time in seconds since 1970. */
#else
																																						    static tcl.lang.Var.SQLITE3_GETSET sqlite3_current_time = new tcl.lang.Var.SQLITE3_GETSET( "sqlite3_current_time" );
#endif
																																						#endif
		///
///<summary>
///Find the current time (in Universal Coordinated Time).  Write into *piNow
///the current time and date as a Julian Day number times 86_400_000.  In
///other words, write into *piNow the number of milliseconds since the Julian
///epoch of noon in Greenwich on November 24, 4714 B.C according to the
///proleptic Gregorian calendar.
///
///On success, return 0.  Return 1 if the time and date cannot be found.
///</summary>

		static int winCurrentTimeInt64 (sqlite3_vfs pVfs, ref sqlite3_int64 piNow)
		{
			///
///<summary>
///</summary>
///<param name="FILETIME structure is a 64">bit value representing the number of</param>
///<param name="100">nanosecond intervals since January 1, 1601 (= JD 2305813.5).</param>
///<param name=""></param>

			//var ft = new FILETIME();
			const sqlite3_int64 winFiletimeEpoch = 23058135 * (sqlite3_int64)8640000;
			#if SQLITE_TEST
																																																									      const sqlite3_int64 unixEpoch = 24405875 * (sqlite3_int64)8640000;
#endif
			///* 2^32 - to avoid use of LL and warnings in gcc */
			//const sqlite3_int64 max32BitValue =
			//(sqlite3_int64)2000000000 + (sqlite3_int64)2000000000 + (sqlite3_int64)294967296;
			//#if SQLITE_OS_WINCE
			//SYSTEMTIME time;
			//GetSystemTime(&time);
			///* if SystemTimeToFileTime() fails, it returns zero. */
			//if (!SystemTimeToFileTime(&time,&ft)){
			//return 1;
			//}
			//#else
			//      GetSystemTimeAsFileTime( ref ft );
			//      ft = System.DateTime.UtcNow.ToFileTime();
			//#endif
			//sqlite3_int64 ft = System.DateTime.UtcNow.ToFileTime();
			//piNow = winFiletimeEpoch + ft;
			//((((sqlite3_int64)ft.dwHighDateTime)*max32BitValue) + 
			//   (sqlite3_int64)ft.dwLowDateTime)/(sqlite3_int64)10000;
			piNow = winFiletimeEpoch + System.DateTime.UtcNow.ToFileTimeUtc () / (sqlite3_int64)10000;
			#if SQLITE_TEST
																																																									#if !TCLSH
																																																									      if ( ( sqlite3_current_time) != 0 )
      {
        piNow = 1000 * (sqlite3_int64)sqlite3_current_time + unixEpoch;
      }
#else
																																																									      if ( ( sqlite3_current_time.iValue ) != 0 )
      {
        piNow = 1000 * (sqlite3_int64)sqlite3_current_time.iValue + unixEpoch;
      }
#endif
																																																									#endif
			sqliteinth.UNUSED_PARAMETER (pVfs);
			return 0;
		}

		///
///<summary>
///Find the current time (in Universal Coordinated Time).  Write the
///current time and date as a Julian Day number into *prNow and
///return 0.  Return 1 if the time and date cannot be found.
///
///</summary>

		static int winCurrentTime (sqlite3_vfs pVfs, ref double prNow)
		{
			int rc;
			sqlite3_int64 i = 0;
			rc = winCurrentTimeInt64 (pVfs, ref i);
			if (0 == rc) {
				prNow = i / 86400000.0;
			}
			return rc;
		}

		///
///<summary>
///The idea is that this function works like a combination of
///GetLastError() and FormatMessage() on windows (or errno and
///strerror_r() on unix). After an error is returned by an OS
///function, SQLite calls this function with zBuf pointing to
///a buffer of nBuf bytes. The OS layer should populate the
///</summary>
///<param name="buffer with a nul">8 encoded error message</param>
///<param name="describing the last IO error to have occurred within the calling">describing the last IO error to have occurred within the calling</param>
///<param name="thread.">thread.</param>
///<param name=""></param>
///<param name="If the error message is too large for the supplied buffer,">If the error message is too large for the supplied buffer,</param>
///<param name="it should be truncated. The return value of xGetLastError">it should be truncated. The return value of xGetLastError</param>
///<param name="is zero if the error message fits in the buffer, or non">zero</param>
///<param name="otherwise (if the message was truncated). If non">zero is returned,</param>
///<param name="then it is not necessary to include the nul">terminator character</param>
///<param name="in the output buffer.">in the output buffer.</param>
///<param name=""></param>
///<param name="Not supplying an error message will have no adverse effect">Not supplying an error message will have no adverse effect</param>
///<param name="on SQLite. It is fine to have an implementation that never">on SQLite. It is fine to have an implementation that never</param>
///<param name="returns an error message:">returns an error message:</param>
///<param name=""></param>
///<param name="int xGetLastError(sqlite3_vfs pVfs, int nBuf, string zBuf){">int xGetLastError(sqlite3_vfs pVfs, int nBuf, string zBuf){</param>
///<param name="Debug.Assert(zBuf[0]=='\0');">Debug.Assert(zBuf[0]=='\0');</param>
///<param name="return 0;">return 0;</param>
///<param name="}">}</param>
///<param name=""></param>
///<param name="However if an error message is supplied, it will be incorporated">However if an error message is supplied, it will be incorporated</param>
///<param name="by sqlite into the error message available to the user using">by sqlite into the error message available to the user using</param>
///<param name="sqlite3_errmsg(), possibly making IO errors easier to debug.">sqlite3_errmsg(), possibly making IO errors easier to debug.</param>
///<param name=""></param>

		static int winGetLastError (sqlite3_vfs pVfs, int nBuf, ref string zBuf)
		{
			sqliteinth.UNUSED_PARAMETER (pVfs);
			return getLastErrorMsg (nBuf, ref zBuf);
		}

		static sqlite3_vfs s_winVfs = new sqlite3_vfs (3, 
///iVersion 

		-1, //sqlite3_file.Length,      /* szOsFile */
		MAX_PATH, 
///mxPathname 

		null, 
///pNext 
		"win32", 
///zName 

		0, 
///pAppData 

		(dxOpen)winOpen, 
///xOpen 
		(dxDelete)winDelete, ///
///<summary>
///xDelete 
///</summary>

		(dxAccess)winAccess, ///
///<summary>
///xAccess 
///</summary>

		(dxFullPathname)winFullPathname, ///
///<summary>
///xFullPathname 
///</summary>

		(dxDlOpen)winDlOpen, ///
///<summary>
///xDlOpen 
///</summary>

		(dxDlError)winDlError, ///
///<summary>
///xDlError 
///</summary>

		(dxDlSym)winDlSym, ///
///<summary>
///xDlSym 
///</summary>

		(dxDlClose)winDlClose, ///
///<summary>
///xDlClose 
///</summary>

		(dxRandomness)winRandomness, ///
///<summary>
///xRandomness 
///</summary>

		(dxSleep)winSleep, ///
///<summary>
///xSleep 
///</summary>

		(dxCurrentTime)winCurrentTime, ///
///<summary>
///xCurrentTime 
///</summary>

		(dxGetLastError)winGetLastError, ///
///<summary>
///xGetLastError 
///</summary>

		(dxCurrentTimeInt64)winCurrentTimeInt64, ///
///<summary>
///xCurrentTimeInt64 
///</summary>

		null, ///
///<summary>
///xSetSystemCall 
///</summary>

		null, ///
///<summary>
///xGetSystemCall 
///</summary>

		null///
///<summary>
///xNextSystemCall 
///</summary>

        );

        public static sqlite3_vfs winVfs
        {
            get { return Sqlite3.s_winVfs; }
            set { Sqlite3.s_winVfs = value; }
        }

		///
///<summary>
///Initialize and deinitialize the operating system interface.
///
///</summary>

		public static SqlResult sqlite3_os_init ()
		{
			#if !SQLITE_OMIT_WAL
																																																									/* get memory map allocation granularity */
memset(&winSysInfo, 0, sizeof(SYSTEM_INFO));
GetSystemInfo(&winSysInfo);
Debug.Assert(winSysInfo.dwAllocationGranularity > 0);
#endif
			os.sqlite3_vfs_register (winVfs, 1);
			return SqlResult.SQLITE_OK;
		}

		static SqlResult sqlite3_os_end ()
		{
			return SqlResult.SQLITE_OK;
		}

		#endif
		//
		//          Windows DLL definitions
		//
		const int NO_ERROR = 0;

		

		
	}



    /// <summary>
    /// Basic locking strategy for Console/Winform applications
    /// </summary>
    public class LockingStrategy
    {
#if !(SQLITE_SILVERLIGHT || WINDOWS_MOBILE)
        [DllImport("kernel32.dll")]
        static extern bool LockFileEx(IntPtr hFile, uint dwFlags, uint dwReserved, uint nNumberOfBytesToLockLow, uint nNumberOfBytesToLockHigh, [In] ref System.Threading.NativeOverlapped lpOverlapped);

        const int LOCKFILE_FAIL_IMMEDIATELY = 1;

#endif
        public virtual void LockFile(sqlite3_file pFile, long offset, long length)
        {
#if !(SQLITE_SILVERLIGHT || WINDOWS_MOBILE)
            pFile.fs.Lock(offset, length);
#endif
        }

        public virtual int SharedLockFile(sqlite3_file pFile, long offset, long length)
        {
#if !(SQLITE_SILVERLIGHT || WINDOWS_MOBILE)
            Debug.Assert(length == Sqlite3.SHARED_SIZE);
            Debug.Assert(offset == Sqlite3.SHARED_FIRST);
            NativeOverlapped ovlp = new NativeOverlapped();
            ovlp.OffsetLow = (int)offset;
            ovlp.OffsetHigh = 0;
            ovlp.EventHandle = IntPtr.Zero;
            return LockFileEx(pFile.fs.Handle, LOCKFILE_FAIL_IMMEDIATELY, 0, (uint)length, 0, ref ovlp) ? 1 : 0;
#else
																																																																												            return 1;
#endif
        }

        public virtual void UnlockFile(sqlite3_file pFile, long offset, long length)
        {
#if !(SQLITE_SILVERLIGHT || WINDOWS_MOBILE)
            pFile.fs.Unlock(offset, length);
#endif
        }
    }

    /// <summary>
    /// Locking strategy for Medium Trust. It uses the same trick used in the native code for WIN_CE
    /// which doesn't support LockFileEx as well.
    /// </summary>
    public class MediumTrustLockingStrategy : LockingStrategy
    {
        public override int SharedLockFile(sqlite3_file pFile, long offset, long length)
        {
#if !(SQLITE_SILVERLIGHT || WINDOWS_MOBILE)
            Debug.Assert(length == Sqlite3.SHARED_SIZE);
            Debug.Assert(offset == Sqlite3.SHARED_FIRST);
            try
            {
                pFile.fs.Lock(offset + pFile.sharedLockByte, 1);
            }
            catch (IOException)
            {
                return 0;
            }
#endif
            return 1;
        }
    }

	internal static class HelperMethods
	{
		public static bool IsRunningMediumTrust ()
		{
			// placeholder method
			// this is where it needs to check if it's running in an ASP.Net MediumTrust or lower environment
			// in order to pick the appropriate locking strategy
			#if SQLITE_SILVERLIGHT
																																																									return true;
#else
			return false;
			#endif
		}
	}
}
