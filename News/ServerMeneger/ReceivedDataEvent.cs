using System;

namespace News
{
    public class ReceivedDataEvent : EventArgs
	{
		public string Data { get; private set; }
		public bool Success { get; private set; }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="success"></param>
		/// <param name="data"></param>
		/// <param name="type"></param>
		public ReceivedDataEvent(bool success, string data = null, string type = null)
		{
			Success = success;
			Data = data;
		}
	}
}