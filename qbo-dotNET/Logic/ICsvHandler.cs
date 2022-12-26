using System;
namespace qbo_dotNET.Logic
{
	public interface ICsvHandler
	{
		public string? fileContent { get; set; }

		public void formatData() { }


	}
}

