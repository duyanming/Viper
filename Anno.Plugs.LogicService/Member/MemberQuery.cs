using System.Collections.Generic;
using Anno.Domain.Dto;
using Anno.Repository;

namespace Anno.QueryServices.Member
{
    public class MemberQuery : IMemberQuery
    {
        private readonly SqlSugar.SqlSugarClient _db;
        public MemberQuery(IBaseQueryRepository baseRepository)
        {
            _db = baseRepository.Context;
        }
        public GetMemberModelOutputDto GetMemberModel(GetMemberModelInputDto args)
        {
            var rlt = _db
                .Queryable<Model.sys_member, Model.bif_company>((m, c) => new object[]
                    {SqlSugar.JoinType.Left, m.coid == c.ID})
                .Where((m, c) => m.ID == args.ID)
                .Select((m, c) => new GetMemberModelOutputDto { ID = m.ID,Name=m.name,CName=c.name,CAddress=c.address,CCode=c.code,CEmail=c.email,CRdt=c.rdt })
                .First();
            return rlt;
        }

        public List<GetMemberListModelOutputDto> GetMemberListModel()
        {
            var rlt = _db
                .Queryable<Model.sys_member, Model.bif_company>((m, c) => new object[]
                    {SqlSugar.JoinType.Left, m.coid == c.ID})
                .Select((m, c) => new GetMemberListModelOutputDto {ID = m.ID, name = m.name,account=m.account,coid=m.coid,rdt=m.rdt,position=m.position,CompanyName=c.name}).ToList();
            return rlt;
        }
    }
}
