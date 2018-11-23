using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Signals.Tests.Storage.Helpers
{
    public class TestFile
    {
        public string Dir { get; set; }
        public string Name { get; set; }
        public int Test { get; set; }
        public string Path => System.IO.Path.Combine(Dir, Name);

        public TestFile(string dir, string name, int test)
        {
            Dir = dir;
            Name = name.Replace("{#}", test.ToString());
        }
    }
}
