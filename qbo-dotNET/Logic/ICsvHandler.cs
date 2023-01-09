using System;
namespace qbo_dotNET.Logic
{
	public interface ICsvHandler
	{
		
		public string? rawData { get; set; }
		//public async Task formatData() { }
        public void formatData() { }

        }
    }

