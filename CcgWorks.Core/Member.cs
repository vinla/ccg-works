﻿using System;

namespace CcgWorks.Core
{
    public class Member
    {
        public Guid Id { get; set; }
        public string EmailAddress { get; set; }
        public string UserName { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is Member member)
                return member.Id == Id;

            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
