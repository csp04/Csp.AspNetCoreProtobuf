using System;
using System.IO;

namespace Csp.AspNetCore.Mvc.Protobuf.Internal
{
    internal static class AspNetCoreFileTemp
    {
        private static string tempDirectory => Path.GetTempPath();

        public static string CreateTempFile() => Path.Combine(tempDirectory, Guid.NewGuid().ToString() + "_proto.tmp");
    }
}