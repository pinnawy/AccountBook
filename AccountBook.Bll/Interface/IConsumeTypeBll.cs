﻿using System.Collections.Generic;
using AccountBook.Model;

namespace AccountBook.Bll.Interface
{
    public interface IConsumeTypeBll
    {
        /// <summary>
        /// 获取消费类别列表
        /// </summary>
        /// <returns>消费类别List集合</returns>
        List<ConsumeType> GetConsumeTypes(int parentTypeId);

        /// <summary>
        /// 获取消费类别二级分类
        /// </summary>
        /// <returns>消费类别二级分类List集合</returns>
        List<ConsumeType> GetConsumeSubTypes();
    }
}
