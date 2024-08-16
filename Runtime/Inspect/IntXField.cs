using System.Text.RegularExpressions;
using UnityEngine.UIElements;


public class IntXField : TextField
{
    private static Regex digital = new Regex("^[0-9]+$");
    
    public IntXField(string label):base(label)
    {
        this.maxLength = 11;    
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
