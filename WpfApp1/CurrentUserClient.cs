using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1
{
    public static class CurrentUserClient
    {
        public static int OwnerId { get; set; }
        public static int loggedInOwnerId { get; set; }
    }
}
