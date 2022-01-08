﻿using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPF_Bestelbons.Models;

namespace WPF_Bestelbons.Events
{
    public class UserListChangedEvent
    {
        public BindableCollection<User> UserList { get; set; }
        public UserListChangedEvent(BindableCollection<User> message)
        {
            UserList = message;
        }

    }
}
