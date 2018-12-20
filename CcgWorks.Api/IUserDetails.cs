using System;
using CcgWorks.Core;

namespace CcgWorks.Api
{
    public interface IUserContext
    {
        Member SignedInUser { get; }
    }
}