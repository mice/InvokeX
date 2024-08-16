using System.Text.RegularExpressions;
using UnityEngine.UIElements;

namespace XInspect
{
    public class IntXField : TextField
    {
        private Regex digital = new Regex("^[0-9]+$");
        private string _restrict = "";
        public IntXField(string label) : base(label)
        {
            this.maxLength = 11;
        }

        public string restrict
        {
            get
            {
                return _restrict;
            }
            set
            {
                _restrict = value;
                if(_restrict==null || _restrict.Length == 0)
                {
                    digital = new Regex(".+");
                }
                else
                {
                    digital = new Regex("^["+ restrict+"]+$");
                }
            }
        }

        public override string value
        {
            get
            {
                return base.value;
            }
            set
            {
                var old_value = base.rawValue;

                if (digital.IsMatch(value))
                {
                    base.value = value;
                    base.text = base.rawValue;
                }
                else
                {
                    if (!digital.IsMatch(old_value))
                    {
                        base.value = "";
                        base.text = "";
                    }
                    else
                    {
                        base.text = old_value;
                    }
                }
            }
        }
    }
}