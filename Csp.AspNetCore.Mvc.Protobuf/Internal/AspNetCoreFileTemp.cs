using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Csp.AspNetCore.Mvc.Protobuf.Internal
{
    internal static class AspNetCoreFileTemp
        {
            private static string tempDirectory => Path.GetTempPath();

            public static string CreateTempFile() => Path.Combine(tempDirectory, Guid.NewGuid().ToString() + "_proto.tmp");
        }
}
