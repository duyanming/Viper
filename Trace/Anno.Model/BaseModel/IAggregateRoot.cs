using System;
using System.Collections.Generic;
using System.Text;

namespace Anno.Model.BaseModel
{
    public interface IAggregateRoot : IAggregateRoot<long>, IEntity
    {

    }

    public interface IAggregateRoot<TPrimaryKey> : IEntity<TPrimaryKey>
    {

    }
}
