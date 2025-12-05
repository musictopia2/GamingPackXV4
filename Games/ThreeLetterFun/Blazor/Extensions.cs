namespace ThreeLetterFun.Blazor;
public static class Extensions
{
    private static readonly BasicList<string> _vowelList = ["A", "E", "I", "O", "U"];
    extension(string letter)
    {
        public string ColorOfLetter
        {
            get
            {
                if (_vowelList.Count != 5)
                {
                    throw new Exception("Must have 5 vowels");
                }
                if (_vowelList.Exists(x => x == letter))
                {
                    return cc1.Red;
                }
                return cc1.Black;
            }       
        }
    }
    
}