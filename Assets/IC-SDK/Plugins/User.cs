using System;

namespace IC_SDK
{
    [Serializable]
    public class User
    {
        public string userName;
        public string principal;

        public User(string name, string principal) 
        {
            this.userName = name;
            this.principal = principal;
        }
        public User()
        {
            this.userName = "Empty";
            this.principal = "000000";
        }
    }
}