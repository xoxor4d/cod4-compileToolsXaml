using System;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Text.RegularExpressions;
using System.Windows.Documents;
using System.Windows.Media;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;
using ICSharpCode.AvalonEdit.Highlighting;
using System.Xml;
using System.IO;

namespace cod4_compileToolsXaml
{
    /// <summary>
    /// Interaktionslogik für SyntaxConverterWindow.xaml
    /// </summary>

    public partial class SyntaxConverterWindow : Window
    {
        //public static IHighlightingDefinition LoadHighlightingDefinition(string resourceName)
        //{
        //    var type = typeof(ResourceLoader);
        //    var fullName = type.Namespace + "." + resourceName;
        //    using (var stream = type.Assembly.GetManifestResourceStream(fullName))
        //    using (var reader = new XmlTextReader(stream))
        //        return ICSharpCode.AvalonEdit.Highlighting.Xshd.HighlightingLoader.Load(reader, HighlightingManager.Instance);
        //}

        // as backup
        static List<string> _hlsl_function_tags = new List<string>();

        // GLSL->HLSL
        string BaseShader;

        public SyntaxConverterWindow()
        {
            InitializeComponent();

            //TimeOutTimeLabel.Content = "";

            // set session saved vars
            // if SyntaxConverterWidth != 0 then all other vars are set to
            if (Variables.SyntaxConverterWidth != 0)
            {
                this.Width = Variables.SyntaxConverterWidth;
                this.Height = Variables.SyntaxConverterHeight;

                //HLSLBOX.FontSize = Variables.SyntaxConverterFontSize;
                GLSLxBOX.FontSize = Variables.SyntaxConverterFontSize;
                HLSLxBOX.FontSize = Variables.SyntaxConverterFontSize;

                usePRECISION.IsChecked = Variables.SyntaxConverterOptimise;
            }

            // load hlsl syntax file for Avalon
            string xshd = System.AppDomain.CurrentDomain.BaseDirectory + "\\Resources\\" + "HLSLx.xshd";

            if (File.Exists(xshd))
            {
                using (XmlTextReader reader = new XmlTextReader(xshd))
                {
                    try
                    {
                        HLSLxBOX.SyntaxHighlighting = ICSharpCode.AvalonEdit.Highlighting.Xshd.HighlightingLoader.Load(reader, HighlightingManager.Instance);
                    }
                    catch
                    {
                        ClearConsole();
                        HLSLxBOX.Text = "Could not load HLSL-Syntax definition. Make sure \"" + xshd + "\" exists!";
                    }
                }
            }
            else
            {
                ClearConsole();
                HLSLxBOX.Text = "Could not load HLSL-Syntax definition. Make sure \"" + xshd + "\" exists!";
            }

            xshd = System.AppDomain.CurrentDomain.BaseDirectory + "\\Resources\\" + "GLSLx.xshd";

            if (File.Exists(xshd))
            {
                using (XmlTextReader reader = new XmlTextReader(xshd))
                {
                    try 
                    {
                        GLSLxBOX.SyntaxHighlighting = ICSharpCode.AvalonEdit.Highlighting.Xshd.HighlightingLoader.Load(reader, HighlightingManager.Instance);
                    }
                    catch 
                    {
                        ClearConsole();
                        GLSLxBOX.Text = "Could not load GLSL-Syntax definition. Make sure \"" + xshd + "\" exists!";
                    }
                    
                }
            }
            else
            {
                ClearConsole();
                GLSLxBOX.Text = "Could not load GLSL-Syntax definition. Make sure \"" + xshd + "\" exists!";
            }

            GLSLxBOX.TextArea.TextView.LinkTextForegroundBrush = new SolidColorBrush(Colors.DimGray);
            HLSLxBOX.TextArea.TextView.LinkTextForegroundBrush = new SolidColorBrush(Colors.DimGray);

            // as backup
            //string[] hlslFunctions = 
            //{
            //    "abs", "acos", "asin", "atan", "ceil", "clamp", "clip", "cos", "cosh", "cross", "ddx", "ddy", "degrees", "distance", "dot", "exp", "exp2",
            //    "faceforward", "floor", "fmod", "frac", "frexp", "fwidth", "isfinite", "isinf", "isnan", "ldexp", "length", "lerp", "lit", "log", "log10",
            //    "log2", "max", "min", "modf", "mul", "noise", "normalize", "pow", "radians", "reflect", "refract", "round", "rsqrt", "saturate", "sign",
            //    "sin", "sincos", "sinh", "smoothstep", "sqrt", "step", "tan", "tanh", "tex1D", "tex2D", "tex2Dlod", "tex2Dproj", "tex2Dgrad", "tex2Dbias",
            //    "tex3D", "texCUBE", "transpose", "trunc"
            //};

            //_hlsl_function_tags = new List<string>(hlslFunctions);
        }

        //private string[] stringGreenArray = {
        //    "//"
        //};

        //private string[] stringPinkArray = {
        //    "JUSTHEREASBACKUP"
        //};

        //private string[] stringRedArray = {
        //    "#define"
        //};

        //private void AppendColoredText(string text, string color)
        //{
        //    BrushConverter bc = new BrushConverter();
        //    TextRange textRange = new TextRange(HLSLBOX.Document.ContentEnd, HLSLBOX.Document.ContentEnd);
        //    textRange.Text = text + "\n";

        //    try
        //    {
        //        textRange.ApplyPropertyValue(TextElement.ForegroundProperty, bc.ConvertFromString(color));
        //    }
        //    catch (FormatException) { }
        //}

        private void AppendTextAvalon(string text)
        {
            HLSLxBOX.Text += text;
        }

        public void WriteHLSL(string s)
        {
            if (s == null)
            {
                return;
            }

            //HLSLBOX.Dispatcher.Invoke(new Action(() =>
            //{
            //    if (stringGreenArray.Any(s.Contains))
            //    {
            //        AppendColoredText(s, "DarkOliveGreen");
            //    }

            //    else if (stringPinkArray.Any(s.Contains))
            //    {
            //        AppendColoredText(s, "MediumPurple");
            //    }

            //    else
            //    {
            //        AppendColoredText(s, "WhiteSmoke");
            //    }

            //    HLSLBOX.ScrollToEnd();
            //}));

            HLSLxBOX.Dispatcher.Invoke(new Action(() =>
            {
                AppendTextAvalon(s);
            }));
        }

        public void ClearConsole()
        {
            if (HLSLxBOX != null) 
            {
                HLSLxBOX.Text = "";
            }
        }

        Thread delayedCalculationThread;
        int delay = 0;

        private void CalculateAfterStopTyping()
        {
            if (delay <= 800)
            {
                delay += 200;
            }

            if (delayedCalculationThread != null && delayedCalculationThread.IsAlive)
            {
                return;
            }

            delayedCalculationThread = new Thread(() =>
            {
                while (delay >= 150)
                {
                    delay = delay - 150;
                    try
                    {
//#if DEBUG
//                        Application.Current.Dispatcher.Invoke(new Action(() =>
//                        {
//                            TimeOutTimeLabel.Content = delay;
//                        }));
//#endif

                        Thread.Sleep(150);
                    }
                    catch (Exception) { }
                }

                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    ClearConsole();

                    //var textRange = new TextRange(GLSLBOX.Document.ContentStart, GLSLBOX.Document.ContentEnd);
                    string[] lines = GLSLxBOX.Text.Split('\n');

                    if (lines.Length != 0 && HLSLxBOX != null)
                    {
                        for (int i = 0; i < lines.Length; i++)
                        {
                            // fix first line not adding a linebreak - or needing 2 linebreaks for whatever reason
                            if (i == 0)
                            {
                                WriteHLSL(Convert(lines[i]) + "\n");
                            }
                            else
                            {
                                WriteHLSL(Convert(lines[i]));
                            }
                        }
                    }
                }));
            });

            delayedCalculationThread.Start();
        }

        public string Convert(string input)
        {
            BaseShader = input;

            // do nothing on comments
            if (Regex.IsMatch(BaseShader, @"^\/\/"))
            {
                return BaseShader;
            }


            // replace uniforms with defines
            if (Regex.IsMatch(BaseShader, @"(uniform[ ].*[ ]time;)"))
            {
                BaseReplace(@"(uniform[ ].*[ ]time;)", "//" + input + "#define time gameTime.w");
            }
            else if (Regex.IsMatch(BaseShader, @"(uniform[ ].*[ ]resolution;)"))
            {
                BaseReplace(@"(uniform[ ].*[ ]resolution;)", "//" + input + "#define resolution renderTargetSize");
            }
            else if (Regex.IsMatch(BaseShader, @"(uniform[ ].*[ ]mouse;)"))
            {
                BaseReplace(@"(uniform[ ].*[ ]mouse;)", "//" + input + "#define mouse.x 0.5\r#define mouse.y 0.5\r#define mouse " + ((bool)usePRECISION.IsChecked ? "half2" : "float2") + "(0.5, 0.5)");
            }
            // unsupported uniform
            else
            {
                BaseReplace(@"^.*\b(uniform)\b.*$", "//" + input);
            }

            // comment lines:
            BaseReplace(@"^.*\b(precision)\b.*$", "//" + input);
            
            BaseReplace(@"^.*\b(ifdef)\b.*$", "//" + input);
            BaseReplace(@"^.*\b(endif)\b.*$", "//" + input);

            BaseReplace(@"^.*\b(extension)\b.*$", "//" + input);
            BaseReplace(@"^.*\b(varying)\b.*$", "//" + input);
            
            // replace lines containing:
            if (Regex.IsMatch(BaseShader, @"^.*\b(void main)\b.*$") && Regex.IsMatch(BaseShader, "{"))
            {
                BaseReplace(@"^.*\b(void main)\b.*$", "PixelOutput ps_main( const PixelInput pixel )\r{");
            }

            // if we didnt replace void main, do it now
            BaseReplace(@"^.*\b(void main)\b.*$", "PixelOutput ps_main( const PixelInput pixel )\r");

            // if optimised performance
            if ((bool)usePRECISION.IsChecked)
            {
                // Extend short vectors
                //BaseReplace(@"\=\s*vec3\(([^;,]+)\)", "= vec3($1, $1, $1)", RegexOptions.Multiline | RegexOptions.Singleline);
                //BaseReplace(@"\=\s*vec4\(([^;,]+)\)", "= vec4($1, $1, $1, $1)", RegexOptions.Multiline | RegexOptions.Singleline);

                BaseReplace("vec|half|float", "half");

                BaseReplace(@"(tex2Dlod\()([^,]+\,)([^)]+\)?[)]+.+(?=\)))", "$1$2float4($3, 0)");
                BaseReplace(@"fixed4\(([^(,]+?)\)", "half4($1, $1, $1, $1)");
                BaseReplace(@"fixed3\(([^(,]+?)\)", "half3($1, $1, $1)");
                BaseReplace(@"fixed2\(([^(,]+?)\)", "half2($1, $1)");
                BaseReplace(@"tex2D\(([^,]+)\,\s*half2\(([^,].+)\)\,(.+)\)", "tex2Dlod($1, half4($2, half2($3, $3)))"); //when vec3 col = texture( iChannel0, vec2(uv.x,1.0-uv.y), lod ).xyz; -> https://www.shadertoy.com/view/4slGWn
                                                                                                                           //BaseReplace( @"#.+","");
                BaseReplace("resolution.xy", "half2(1.0, 1.0)");
                BaseReplace("mat2", "half2x2");
                BaseReplace("mat3", "half3x3");
                BaseReplace("mat4", "half4x4");
            }

            else
            {
                // Extend short vectors
                //BaseReplace(@"\=\s*vec3\(([^;,]+)\)", "= vec3($1, $1, $1)", RegexOptions.Multiline | RegexOptions.Singleline);
                //BaseReplace(@"\=\s*vec4\(([^;,]+)\)", "= vec4($1, $1, $1, $1)", RegexOptions.Multiline | RegexOptions.Singleline);

                BaseReplace("vec|half", "float");

                BaseReplace(@"(tex2Dlod\()([^,]+\,)([^)]+\)?[)]+.+(?=\)))", "$1$2float4($3, 0)");
                BaseReplace(@"float4\(([^(,]+?)\)", "float4($1, $1, $1, $1)");
                BaseReplace(@"float3\(([^(,]+?)\)", "float3($1, $1, $1)");
                BaseReplace(@"float2\(([^(,]+?)\)", "float2($1, $1)");
                BaseReplace(@"tex2D\(([^,]+)\,\s*float2\(([^,].+)\)\,(.+)\)", "tex2Dlod($1, float4($2, float2($3, $3)))");//when vec3 col = texture( iChannel0, vec2(uv.x,1.0-uv.y), lod ).xyz; -> https://www.shadertoy.com/view/4slGWn
                                                                                                                          //BaseReplace( @"#.+","");
                BaseReplace("resolution.xy", "float2(1.0, 1.0)");
                BaseReplace("mat2", "float2x2");
                BaseReplace("mat3", "float3x3");
                BaseReplace("mat4", "float4x4");
            }

            BaseReplace("dFdx", "ddx");
            BaseReplace("dFdy", "ddy");
            BaseReplace("fract", "frac");
            BaseReplace("refrac", "refract");
            BaseReplace("mod", "fmod");
            BaseReplace("mix", "lerp");
            BaseReplace(@"atan\(([^,]+?)\,([^,]+?)\)", "atan2($2, $1)");

            BaseReplace("texture", "tex2D");
            BaseReplace("tex2DLod", "tex2Dlod");
            BaseReplace(@"texelFetch", "tex2D");
            BaseReplace("iChannel0", "FIXME-iChannel0");
            BaseReplace("iChannel1", "FIXME-iChannel1");
            BaseReplace("iChannel2", "FIXME-iChannel2");
            BaseReplace("iChannel3", "FIXME-iChannel3");

            //BaseReplace("Time", "gameTime.w");
            //BaseReplace("time", "gameTime.w");

            BaseReplace(@"ifixed(\d)", "fixed$1");  //ifixed to fixed

            if (!Regex.IsMatch(BaseShader, @"(#define .*)"))
            {
                BaseReplace(@"iResolution.((x|y){1,2})?", "1");
                BaseReplace(@"iResolution(\.(x|y){1,2})?", "1");
                BaseReplace("iMouse", "1");
                BaseReplace("iGlobalTime", "gameTime.w");
                BaseReplace("iTime", "gameTime.w");
            }

            //BaseReplace(@"fragCoord.xy / iResolution.xy", "pixel.uv.xy");
            //BaseReplace(@"fragCoord(.xy)?", "pixel.uv.xy");
            
            BaseReplace("GL_ES", "FIXME-GL_ES");

            BaseReplace("gl_FragColor =", "PixelOutput.color =");
            BaseReplace("gl_FragColor", "PixelOutput.color");

            BaseReplace("gl_FragCoord =", "pixel.uv =");
            BaseReplace("gl_FragCoord", "pixel.uv"); //BaseReplace("gl_FragCoord", "((i.screenCoord.xy/i.screenCoord.w)*_ScreenParams.xy)");
            
            // replace non fixed stuff
            //BaseReplace("resolution", "FIXME-resolution");
            //BaseReplace("mouse", "FIXME-mouse");

            BaseReplace(@"(m)\*(p)", "mul($1,$2)"); // was commented ?
            //BaseReplace(@"(.+\s*)(\*\=)\s*([^ ;*+\/]+)", "$1 = mul($1, $3)"); // was commented

            // add whitespaces
            BaseReplace(@"(\)\,)(\w|\d)", "), $2"); // add whitespace on :: "),"
            BaseReplace(@"(\)\/)(\w|\d)", ") / $2"); // add whitespace on :: ")/"
            BaseReplace(@"(\)\*)(\w|\d)", ") * $2"); // add whitespace on :: ")*"
            BaseReplace(@"(\)\+)(\w|\d)", ") + $2"); // add whitespace on :: ")+"
            BaseReplace(@"(\)\-)(\w|\d)", ") - $2"); // add whitespace on :: ")-"

            BaseReplace(@"(\,\()", ", ("); // add whitespace on :: ",("
            BaseReplace(@"(\/\()", "/ ("); // add whitespace on :: "/("
            BaseReplace(@"(\*\()", "* ("); // add whitespace on :: "*("
            BaseReplace(@"(\+\()", "+ ("); // add whitespace on :: "+("
            BaseReplace(@"(\-\()", "- ("); // add whitespace on :: "-("

            BaseReplace(@"(\w|\d)(\*)(\w|\d)", "$1 * $3"); // add whitespace on :: "num/char*num/char"
            BaseReplace(@"(\w|\d)(\/)(\w|\d)", "$1 / $3"); // add whitespace on :: "num/char/num/char"
            BaseReplace(@"(\w|\d)(\+)(\w|\d)", "$1 + $3"); // add whitespace on :: "num/char+num/char"
            BaseReplace(@"(\w|\d)(\-)(\w|\d)", "$1 - $3"); // add whitespace on :: "num/char-num/char"

            BaseReplace(@"(\d)(\,)(\d)", "$1, $3"); // add whitespace on :: "0.0,0.0"
            BaseReplace(@"(\w|\d|[,.;])(\s\s)(\w|\d|[,.;])", "$1 $3"); // remove double whitespaces on :: num/char  num/char

            BaseReplace(@"(\w|\d)(=)(\w|\d)", "$1 $2 $3"); // add whitespace on :: num/char=num/char"
            BaseReplace(@"(\w|\d)(<)(\w|\d)", "$1 $2 $3"); // add whitespace on :: num/char<num/char"
            BaseReplace(@"(\w|\d)(>)(\w|\d)", "$1 $2 $3"); // add whitespace on :: num/char>num/char"

            // add numbers to shortend floats with only number + dot
            BaseReplace(@"(?<=\D)(\.\d+)(?=$|)", "0$1"); // add 0 on .0
            BaseReplace(@"(\d\.)(?!\d)", "$&0"); // add 0 on 1.

            // add whitespace after commas that are not at the end of the line
            BaseReplace(@",([^ \r\n])", ", $1");

            // add newlines to opening curly brackets if they are on the same line
            //BaseReplace(@"(\)[ \t]*\{\r)", "\r{\r");

            //// extend short vectors
            //if ((bool)usePRECISION.IsChecked)
            //{
            //    BaseReplace(@"(vec4\((\d\.\d|\s\d\.\d|\d\.\d\s|\s\d\.\d\s)\))", "fixed4($1, $1, $1, $1)");
            //    BaseReplace(@"(vec3\((\d\.\d|\s\d\.\d|\d\.\d\s|\s\d\.\d\s)\))", "fixed3($1, $1, $1)");
            //    BaseReplace(@"(vec2\((\d\.\d|\s\d\.\d|\d\.\d\s|\s\d\.\d\s)\))", "fixed2($1, $1)");
            //}

            //else
            //{

            //}

            return BaseShader;
        }

        void BaseReplace(string pattern, string replacement)
        {
            if (BaseShader != null && pattern != null && replacement != null)
            {
                BaseShader = Regex.Replace(BaseShader, pattern, replacement);
            }
        }

        void BaseReplace(string pattern, string replacement, RegexOptions options)
        {
            if (BaseShader != null && pattern != null && replacement != null)
            {
                BaseShader = Regex.Replace(BaseShader, pattern, replacement, options);
            }
        }

        /////////////////////////////////////////
        // UI-EVENTS ++++++++++++++++++++++++++++

        private void GLSLxBOX_TextChanged(object sender, EventArgs e)
        {
            CalculateAfterStopTyping();
        }

        private void Editor_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            var textToSync = (sender == GLSLxBOX) ? HLSLxBOX : GLSLxBOX;

            textToSync.ScrollToVerticalOffset(e.VerticalOffset);
            textToSync.ScrollToHorizontalOffset(e.HorizontalOffset);
        }

        private void ZoomRtb_Click(object sender, RoutedEventArgs e)
        {
            GLSLxBOX.FontSize = GLSLxBOX.FontSize + 1;
            HLSLxBOX.FontSize = GLSLxBOX.FontSize;
        }

        private void ZoomOutRtb_Click(object sender, RoutedEventArgs e)
        {
            GLSLxBOX.FontSize = GLSLxBOX.FontSize - 1;
            HLSLxBOX.FontSize = GLSLxBOX.FontSize;
        }

        private void UsePRECISION_Checked(object sender, RoutedEventArgs e)
        {
            CalculateAfterStopTyping();
        }

        private void UsePRECISION_Unchecked(object sender, RoutedEventArgs e)
        {
            CalculateAfterStopTyping();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Variables.SyntaxConverterWidth = this.Width;
            Variables.SyntaxConverterHeight = this.Height;

            Variables.SyntaxConverterFontSize = GLSLxBOX.FontSize;
            Variables.SyntaxConverterOptimise = (bool)usePRECISION.IsChecked;
        }

        private void RefreshSyntax_Click(object sender, RoutedEventArgs e)
        {
            // load hlsl syntax file for Avalon
            string xshd = System.AppDomain.CurrentDomain.BaseDirectory + "\\Resources\\" + "HLSLx.xshd";

            if (File.Exists(xshd))
            {
                using (XmlTextReader reader = new XmlTextReader(xshd))
                {
                    HLSLxBOX.SyntaxHighlighting = ICSharpCode.AvalonEdit.Highlighting.Xshd.HighlightingLoader.Load(reader, HighlightingManager.Instance);
                }
            }
            else
            {
                ClearConsole();
                HLSLxBOX.Text = "Could not load HLSL-Syntax definition. Make sure \"" + xshd + "\" exists!";
            }

            xshd = System.AppDomain.CurrentDomain.BaseDirectory + "\\Resources\\" + "GLSLx.xshd";

            if (File.Exists(xshd))
            {
                using (XmlTextReader reader = new XmlTextReader(xshd))
                {
                    GLSLxBOX.SyntaxHighlighting = ICSharpCode.AvalonEdit.Highlighting.Xshd.HighlightingLoader.Load(reader, HighlightingManager.Instance);
                }
            }
            else
            {
                ClearConsole();
                GLSLxBOX.Text = "Could not load GLSL-Syntax definition. Make sure \"" + xshd + "\" exists!";
            }
        }
    }
}
