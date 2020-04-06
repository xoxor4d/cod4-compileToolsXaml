using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace cod4_compileToolsXaml
{
    class xshdLoader
    {
        public static IHighlightingDefinition LoadHighlightingDefinition(string resourceName)
        {
            var type = typeof(xshdLoader);
            var fullName = type.Namespace + "." + resourceName;

            using (var stream = type.Assembly.GetManifestResourceStream(fullName))
            {
                using (var reader = new XmlTextReader(stream))
                {
                    return HighlightingLoader.Load(reader, HighlightingManager.Instance);
                }
            }
        }
    }
}
