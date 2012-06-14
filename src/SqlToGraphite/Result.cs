using System;

namespace SqlToGraphite
{
    public class Result : IResult
    {
        public Result(int value, string name, DateTime dateTime, string path)
        {
            this.Name = name.Replace(":", string.Empty);
            this.TimeStamp = dateTime;
            Path = path;
            Value = value;
        }

        public int Value { get; private set; }

        public DateTime TimeStamp { get; private set; }

        public string Name { get; private set; }

        public string Path { get; private set; }

        public string FullPath
        {
            get
            {
                var rtn = string.Empty;
                rtn = this.ReplaceHostname();                
                rtn = this.IfNameIsNotEmptryAddToEnd(rtn);
                rtn = rtn.Replace(" ", "_");
                return rtn;
            }
        }

        private string IfNameIsNotEmptryAddToEnd(string p)
        {
            string rtn;
            if (this.Name != string.Empty)
            {
                rtn = string.Format("{0}.{1}", p, this.Name);
            }
            else
            {
                rtn = string.Format("{0}", p);
            }

            return rtn;
        }

        private string ReplaceHostname()
        {
            var p = this.Path.Replace("%h", Environment.MachineName);
            return p;
        }
    }
}