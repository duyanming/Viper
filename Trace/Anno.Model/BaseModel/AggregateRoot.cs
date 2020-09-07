using System;
using System.Collections.Generic;
using System.Text;

namespace Anno.Model.BaseModel
{
    public class AggregateRoot : AggregateRoot<long>, IAggregateRoot
    {

    }

    public class AggregateRoot<TPrimaryKey> : Entity<TPrimaryKey>, IAggregateRoot<TPrimaryKey>
    {

    }
}
