using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CMDEventSlots
{
    public static CMDEventSlots Instance_;

    public static CMDEventSlots Instance
    {
        get
        {
            if (Instance_ != null)
            {
                return Instance_;
            }
            Instance_ = new CMDEventSlots();
            return Instance_;
        }
    }
}
