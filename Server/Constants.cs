using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Server
{
    public static class Constants
    {
        public const string Issuer = Audiznce;
        public const string Audiznce = "http://localhost:5000/";
        public const string Secret = "not_too_short_secret_otherwise_it_might_error";

    }
}
