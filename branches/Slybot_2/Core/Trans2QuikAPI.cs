using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Core
{
	public static class Trans2QuikAPI
	{
		#region Êîíñòàíòû âîçâğàùàåìûõ çíà÷åíèé

		public enum enumQuikResults : int
		{
			TRANS2QUIK_SUCCESS = 0,
			TRANS2QUIK_FAILED = 1,
			TRANS2QUIK_QUIK_TERMINAL_NOT_FOUND = 2,
			TRANS2QUIK_DLL_VERSION_NOT_SUPPORTED = 3,
			TRANS2QUIK_ALREADY_CONNECTED_TO_QUIK = 4,
			TRANS2QUIK_WRONG_SYNTAX = 5,
			TRANS2QUIK_QUIK_NOT_CONNECTED = 6,
			TRANS2QUIK_DLL_NOT_CONNECTED = 7,
			TRANS2QUIK_QUIK_CONNECTED = 8,
			TRANS2QUIK_QUIK_DISCONNECTED = 9,
			TRANS2QUIK_DLL_CONNECTED = 10,
			TRANS2QUIK_DLL_DISCONNECTED = 11,
			TRANS2QUIK_MEMORY_ALLOCATION_ERROR = 12,
			TRANS2QUIK_WRONG_CONNECTION_HANDLE = 13,
			TRANS2QUIK_WRONG_INPUT_PARAMS = 14
		};

		#endregion

		#region C++ signatures

		/*
		long TRANS2QUIK_API __stdcall TRANS2QUIK_SEND_SYNC_TRANSACTION (
				LPSTR lpstTransactionString, 
				long* pnReplyCode, 
				PDWORD pdwTransId, 
				double* pdOrderNum, 
				LPSTR lpstrResultMessage, 
				DWORD dwResultMessageSize, 
				long* pnExtendedErrorCode, 
				LPSTR lpstErrorMessage, 
				DWORD dwErrorMessageSize);

		long TRANS2QUIK_API __stdcall TRANS2QUIK_SEND_ASYNC_TRANSACTION (
				LPSTR lpstTransactionString, 
				long* pnExtendedErrorCode, 
				LPSTR lpstErrorMessage, 
				DWORD dwErrorMessageSize);

		long TRANS2QUIK_API __stdcall TRANS2QUIK_CONNECT (
				LPSTR lpstConnectionParamsString, 
				long* pnExtendedErrorCode, 
				LPSTR lpstrErrorMessage, 
				DWORD dwErrorMessageSize);

		long TRANS2QUIK_API __stdcall TRANS2QUIK_DISCONNECT (
				long* pnExtendedErrorCode, 
				LPSTR lpstrErrorMessage, 
				DWORD dwErrorMessageSize);

		long TRANS2QUIK_API __stdcall TRANS2QUIK_SET_CONNECTION_STATUS_CALLBACK (
				TRANS2QUIK_CONNECTION_STATUS_CALLBACK pfConnectionStatusCallback, 
				long* pnExtendedErrorCode, 
				LPSTR lpstrErrorMessage, 
				DWORD dwErrorMessageSize);

		long TRANS2QUIK_API __stdcall TRANS2QUIK_SET_TRANSACTIONS_REPLY_CALLBACK (
				TRANS2QUIK_TRANSACTION_REPLY_CALLBACK pfTransactionReplyCallback, 
				long* pnExtendedErrorCode, 
				LPSTR lpstrErrorMessage, 
				DWORD dwErrorMessageSize);

		long TRANS2QUIK_API __stdcall TRANS2QUIK_IS_QUIK_CONNECTED (
				long* pnExtendedErrorCode, 
				LPSTR lpstrErrorMessage, 
				DWORD dwErrorMessageSize);

		long TRANS2QUIK_API __stdcall TRANS2QUIK_IS_DLL_CONNECTED (
				long* pnExtendedErrorCode, 
				LPSTR lpstrErrorMessage, 
				DWORD dwErrorMessageSize);
		*/

		#endregion

		#region Âñïîìîãàòåëüíûå ôóíêöèè

		public static string ResultToString(int res)
		{
			string result = "RESULT_UNKNOWN";

			if ((res >= 0) && (res <= 14))
				result = ((enumQuikResults)res).ToString();

			return result;
		}

		public static string ByteToString(byte[] strByte)
		{
			string result = string.Empty;

			for (int i = 0; i < strByte.Length; i++)
			{
				result += result + strByte[i].ToString();
			}

			return result;
		}

		#endregion;

		#region Connect

		[DllImport("TRANS2QUIK.DLL", EntryPoint = "_TRANS2QUIK_CONNECT@16",
			CallingConvention = CallingConvention.StdCall)]
		static extern int dllConnect(string connectionParamsString, ref int extendedErrorCode,
			string errorMessage, uint errorMessageSize);

		public static int Connect(string pathToQUIK)
		{
			int result = -1;

			uint errorMessageSize = 50;
			string errorMessage = string.Empty;
			int extendedErrorCode = 0;

			result = dllConnect(pathToQUIK, ref extendedErrorCode, errorMessage, errorMessageSize);

			return result & 255;
		}

		#endregion

		#region IsDLLConnected

		[DllImport("TRANS2QUIK.DLL", EntryPoint = "_TRANS2QUIK_IS_DLL_CONNECTED@12",
			CallingConvention = CallingConvention.StdCall)]
		static extern int dllIsDLLConnected(ref int extendedErrorCode, string errorMessage, uint errorMessageSize);

		public static bool IsDLLConnected()
		{
			bool result = false;

			uint errorMessageSize = 50;
			string errorMessage = string.Empty;
			int extendedErrorCode = 0;

			if ((dllIsDLLConnected(ref extendedErrorCode, errorMessage, errorMessageSize) & 255) ==
				(int)enumQuikResults.TRANS2QUIK_DLL_CONNECTED)
			{
				result = true;
			}

			return result;
		}

		#endregion

		#region Disconnect

		[DllImport("TRANS2QUIK.DLL", EntryPoint = "_TRANS2QUIK_DISCONNECT@12",
			CallingConvention = CallingConvention.StdCall)]
		static extern int dllDisconnect(ref int extendedErrorCode, string errorMessage, uint errorMessageSize);

		public static int Disconnect()
		{
			int result = -1;

			uint errorMessageSize = 50;
			string errorMessage = string.Empty;
			int extendedErrorCode = 0;

			result = dllDisconnect(ref extendedErrorCode, errorMessage, errorMessageSize);

			return result & 255;
		}

		#endregion

		#region IsQuikConnected

		[DllImport("TRANS2QUIK.DLL", EntryPoint = "_TRANS2QUIK_IS_QUIK_CONNECTED@12",
				CallingConvention = CallingConvention.StdCall)]
		static extern int dllIsQuikConnected(ref int extendedErrorCode, string errorMessage, uint errorMessageSize);

		public static bool IsQuikConnected()
		{
			bool result = false;

			uint errorMessageSize = 50;
			string errorMessage = string.Empty;
			int extendedErrorCode = 0;

			if ((dllIsQuikConnected(ref extendedErrorCode, errorMessage, errorMessageSize) & 255) ==
				(int)enumQuikResults.TRANS2QUIK_QUIK_CONNECTED)
			{
				result = true;
			}

			return result;
		}

		#endregion

		#region SendSyncTransaction

		[DllImport("TRANS2QUIK.DLL", EntryPoint = "_TRANS2QUIK_SEND_SYNC_TRANSACTION@36",
				CallingConvention = CallingConvention.StdCall)]
		static extern int dllSendSyncTransaction(string transactionString, ref int replyCode,
			ref uint transactionID, ref double orderNumber, string resultMessage, uint resultMessageSize,
			ref int extendedErrorCode, string errorMessage, uint errorMessageSize);

		public static int SendSyncTransaction(string strTransaction, double orderNumber)
		{
			int result = -1;

			uint errorMessageSize = 50;
			string errorMessage = string.Empty;
			int extendedErrorCode = 0;

			int replyCode = 0;
			uint transactionID = 0;
			uint resultMessageSize = 50;
			string resultMessage = string.Empty;

			result = dllSendSyncTransaction(strTransaction, ref replyCode, ref transactionID, ref orderNumber,
				resultMessage, resultMessageSize, ref extendedErrorCode, errorMessage, errorMessageSize);

			return result & 255;
		}

		#endregion

		#region SendAsyncTransaction

		[DllImport("TRANS2QUIK.DLL", EntryPoint = "_TRANS2QUIK_SEND_ASYNC_TRANSACTION@16",
				CallingConvention = CallingConvention.StdCall)]
		static extern int dllSendAsyncTransaction(string transactionString, ref int extendedErrorCode,
			string errorMessage, uint errorMessageSize);

		public static int SendAsyncTransaction(string strTransaction)
		{
			int result = -1;

			uint errorMessageSize = 50;
			string errorMessage = string.Empty;
			int extendedErrorCode = 0;

			result = dllSendAsyncTransaction(strTransaction, ref extendedErrorCode, errorMessage, errorMessageSize);

			return result & 255;
		}

		#endregion

		#region Äåëåãàòû

		#region ConnectionStatusCallback

		public delegate void ConnectionStatusCallback(int connectionEvent, int extendedErrorCode,
			string infoMessage);

		[DllImport("TRANS2QUIK.DLL", EntryPoint = "_TRANS2QUIK_SET_CONNECTION_STATUS_CALLBACK@16",
			CallingConvention = CallingConvention.StdCall)]
		public static extern int SetConnectionStatusCallback(ConnectionStatusCallback connectionStatusCallback,
			int extendedErrorCode, string errorMessage, int errorMessageSize);

		#endregion

		#region TransactionsReplyCallback

		//public delegate void TransactionsReplyCallback(out object transactionResult,
		//  out object transactionExtendedErrorCode, out object transactionReplyCode,
		//  out object transactionID, out object orderNumber, out object transactionReplyMessage);

		public delegate void TransactionsReplyCallback(int transactionResult,
			int transactionExtendedErrorCode, int transactionReplyCode,
			uint transactionID, double orderNumber, string transactionReplyMessage);

		[DllImport("TRANS2QUIK.DLL", EntryPoint = "_TRANS2QUIK_SET_TRANSACTIONS_REPLY_CALLBACK@16",
			CallingConvention = CallingConvention.StdCall)]
		static extern int SetTransactionsReplyCallback(TransactionsReplyCallback transactionsReplyCallback,
			int extendedErrorCode, string errorMessage, int errorMessageSize);

        public static int SetTransactionsReplyCallback(TransactionsReplyCallback transactionsReplyCallback)
        {
   			int result = -1;

			int errorMessageSize = 50;
			string errorMessage = string.Empty;
			int extendedErrorCode = 0;

            result = SetTransactionsReplyCallback(transactionsReplyCallback, extendedErrorCode, errorMessage, errorMessageSize);

            return result & 255;
        }

		#endregion

		#endregion
	}
}
