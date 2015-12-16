using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FB_api.Helpers
{
    public static class StaticValue
    {
        private static Models.Profile profile;

        public static Models.Profile Profile
        {
            get;
            set;
        }
    }
}