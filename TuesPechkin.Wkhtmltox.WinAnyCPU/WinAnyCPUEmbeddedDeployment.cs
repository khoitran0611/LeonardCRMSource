using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Text;
using TuesPechkin.Properties;
using SysPath = System.IO.Path;

namespace TuesPechkin
{
    [Serializable]
    public class WinAnyCPUEmbeddedDeployment : EmbeddedDeployment
    {
        public WinAnyCPUEmbeddedDeployment(IDeployment physical) : base(physical) { }

        public override string Path
        {
            get
            {
                return System.IO.Path.Combine(
                    base.Path,
                    GetType().Assembly.GetName().Version.ToString());
            }
        }

        protected override IEnumerable<KeyValuePair<string, Stream>> GetContents()
        {
            var resource = IntPtr.Size == 8
                ? Resources.wkhtmltox_64_dll
                : Resources.wkhtmltox_32_dll;

            return new[]
            { 
                new KeyValuePair<string, Stream>(
                    key: WkhtmltoxBindings.DLLNAME,
                    value: new GZipStream(
                        new MemoryStream(resource), 
                        CompressionMode.Decompress))
            };
        }
    }
}
