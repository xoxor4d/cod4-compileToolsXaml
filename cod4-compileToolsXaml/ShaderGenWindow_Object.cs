using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace cod4_compileToolsXaml
{
    class ShaderGenWindow_Object : IDataErrorInfo, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        // This method is called by the Set accessor of each property.
        // The CallerMemberName attribute that is applied to the optional propertyName
        // parameter causes the property name of the caller to be substituted as an argument.
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private string _shaderName;

        public string ShaderName
        {
            get
            {
                return _shaderName;
            }
            set
            {
                _shaderName = value;
                NotifyPropertyChanged("ShaderName");
            }
        }

        string IDataErrorInfo.Error
        {
            get
            {
                return null;
            }
        }

        static readonly string[] ValidatedProperties =
        {
            "ShaderName"
        };

        public bool isValid
        {
            get
            {
                foreach (string property in ValidatedProperties)
                {
                    var result = GetValidationError(property, true);

                    if (result != null)
                    {
                        return false;
                    }
                }

                return true;
            }
        }

        string IDataErrorInfo.this[string propertyName]
        {
            get
            {
                return GetValidationError(propertyName);
            }
        }

        string GetValidationError(string propertyName, bool isValidCheck = false)
        {
            string error = null;

            switch (propertyName)
            {
                case "ShaderName":
                    error = ValidateShaderName(isValidCheck);

                    break;

            }

            Variables.ShaderGenData_CurrentError = error;
            return error;
        }

        // Rules here
        private string ValidateShaderName(bool isValidCheck = false)
        {
            string ValidationCheckString = "";

            // Shadername will be null on ValidationCheck with Button because ShaderName is never really set
            // instead we put the current ShadernameTextbox Text into "ValidationCheckString" and check that
            if (ShaderName == null && isValidCheck)
            {
                ValidationCheckString = Variables.ShaderGenData_Name;
            }

            if (ShaderName == "Shader Name Here" || ( isValidCheck && ValidationCheckString == "Shader Name Here"))
            {
                return "Enter a Shader Name.";
            }

            // from here on down, dont do checks on "Shadername", but only on "ValidationCheckString"
            // check if "ValidationCheckString" is empty before we move on as we cant check against null
            if ((!isValidCheck && string.IsNullOrWhiteSpace(ShaderName)) || (isValidCheck && string.IsNullOrWhiteSpace(ValidationCheckString)))
            {
                return "Shader Name cannot be empty.";
            }

            // Only chars allowed: A-Z, a-z, 0-9, - & _
            string pattern = @"^[a-zA-Z0-9_-]*$";
            Regex regex = new Regex(pattern);

            if((!isValidCheck && Regex.IsMatch(ShaderName, @"^\d+")) || (isValidCheck && Regex.IsMatch(ValidationCheckString, @"^\d+")))
            {
                return "First char cannot be a numeric.";
            }

            if ((!isValidCheck && !regex.IsMatch(ShaderName)) || (isValidCheck && !regex.IsMatch(ValidationCheckString)))
            {
                return "Invalid expression : \"a-z, A-Z, 0-9, underscore \"";
            }

            if ((!isValidCheck && ShaderName.Length < 6) || (isValidCheck && ValidationCheckString.Length < 6))
            {
                return "Shader Name must be a minimum of 6 characters.";
            }

            // if shader injection is valid and the shader name != 11 chars long
            if (Variables.ShaderGenUI_InjectChk && Variables.ShaderGenUI_InjectChkEnabled)
            {
                if((!isValidCheck && ShaderName.Length != 11) || (isValidCheck && ValidationCheckString.Length != 11))
                {
                    return "Shader Injection needs a shader name of exactly 11 chars!";
                }
            }

            return null;
        }
    }
}