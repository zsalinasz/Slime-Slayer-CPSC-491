using UnityEngine;
using System;

public class OptionActionPair
{
    public string option;
    public Action action;

    public OptionActionPair(string _option, Action _action)
    {
        option = _option;
        action = _action;
    }
};
