using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ExtensionAdapter {
    public class FunctionInfo {
        public string ButtonText { get; set; }
        public Func<string, string> Function { get; set; }
    }

    public interface IFunctionModule {
        FunctionInfo GetModuleFunctionInfo();
    }
}
