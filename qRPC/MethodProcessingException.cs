using Castle.Components.DictionaryAdapter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace qRPC
{
    [Serializable]
    public class MethodProcessingException : Exception
    {
        string _methodName = "";
        public string MethodName => _methodName;

        string[] _args = Array.Empty<string>();
        public string[] Args => _args;

        public MethodProcessingException()
        {
        }

        public MethodProcessingException(string message) : base(message)
        {
        }

        public MethodProcessingException(string methodName, params string[] args) : base($"Failed to process the method `{methodName}`")
        {
            Data.Add("methodName", methodName);
            Data.Add("arguments", string.Join('|', args));
        }


        public MethodProcessingException(Exception innerException, string methodName, params string[] args) 
            : base($"Failed to process the method `{methodName}`", innerException)
        {
            Data.Add("methodName", methodName);
            Data.Add("arguments", string.Join('|', args));
        }

        public MethodProcessingException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
