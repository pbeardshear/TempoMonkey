using System;

/// <summary>
/// Summary description for Class1
/// </summary>
public class Tutorial
{
    /*
    delegate bool check();
    delegate void setup();
    check _condition;
    setup _setups;
     * */
    string _name, _instructions;
    Uri _source;

    public Tutorial(string name, string instructions, Uri source)
    {
        _name = name;
        _instructions = instructions;
        _source = source;
    }

    public Uri getSource(){
        return _source;
    }

/*    public Tutorial(check condition, string instructions, setup setups )
	{
        _condition = condition;
        _instructions = instructions;
        _setups = setups;
	}

    public bool check()
    {
        return _condition();
    }
 * */
}
