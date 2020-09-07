using System;
using System.Collections.Generic;
using System.Text;
using  Anno.Domain.Dto;

namespace Anno.QueryServices.Member
{
    public interface IMemberQuery
    {
        /// <summary>
        /// 根据ID获取会员对象
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        GetMemberModelOutputDto GetMemberModel(GetMemberModelInputDto args);
        /// <summary>
        /// 获取会员列表
        /// </summary>
        /// <returns></returns>
        List<GetMemberListModelOutputDto> GetMemberListModel();
    }
}
