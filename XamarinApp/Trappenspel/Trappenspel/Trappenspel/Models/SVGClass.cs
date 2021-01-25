using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Trappenspel.Models
{
    public class SVGClass
    {

        public Assembly SvgAssembly
        {
            get { return typeof(App).GetTypeInfo().Assembly; }
        }

        public string LogoSvg
        {
            get { return "Trappenspel.Images.Header.svg"; }
        }

    }
}
