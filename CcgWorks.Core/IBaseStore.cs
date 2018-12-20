﻿using System;
using System.Threading.Tasks;

namespace CcgWorks.Core
{
    public interface IBaseStore<T> where T : BaseObject
    {
        Task Add(T item);
        Task UpdateOne(Guid id, Action<T> updateAction);
    }
}
