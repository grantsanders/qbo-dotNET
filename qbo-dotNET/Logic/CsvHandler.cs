using System;

namespace qbo_dotNET.Logic
{
    public class CsvHandler : ICsvHandler
    {

        public string? rawData { get; set; }
        public IApiHandler api { get; set; }

        public CsvHandler()
        {

        }

        public void formatData(string rawData)
        {

        }
    }
}

