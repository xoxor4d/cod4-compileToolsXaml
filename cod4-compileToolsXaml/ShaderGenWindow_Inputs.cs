using System;
using System.Collections.Generic;
using System.ComponentModel;
namespace cod4_compileToolsXaml
{

    class ShaderGenWindow_Inputs : INotifyPropertyChanged
    {
        public ShaderGenWindow_Inputs()
        {
            Model = new ShaderGenWindow_Object()
            {
                ShaderName = "Shader Name Here"
            };
        }

        public ShaderGenWindow_Object Model
        {
            get;
            set;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;

            if(handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
