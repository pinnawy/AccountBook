﻿using System;
using System.Collections.Generic;
using AccountBook.Dal.Interface;
using AccountBook.Model;

namespace AccountBook.Dal.Impl
{
    public class ConsumeTypeDal : IConsumeTypeDal
    {
        public List<ConsumeType> GetConsumeTypes(int parentTypeId)
        {
            string cmdText = @"SELECT *,(SELECT [TypeName] FROM [ConsumeType] WHERE [TypeId] = T.[ParentTypeId]) AS ParentTypeName 
                                FROM [ConsumeType] T WHERE 1=1";
            if(parentTypeId != 0)
            {
                cmdText = string.Format("{0} AND [ParentTypeId]={1}", cmdText, parentTypeId);
            }
            var reader = SqliteHelper.ExecuteReader(cmdText);
            return reader.ToConsumeTypeList();
        }

        public List<ConsumeType> GetConsumeSubTypes()
        {
            const string cmdText = @"SELECT *,(SELECT [TypeName] FROM [ConsumeType] WHERE [TypeId] = T.[ParentTypeId]) AS ParentTypeName 
                                    FROM [ConsumeType] T WHERE [ParentTypeId] <> 0";
            var reader = SqliteHelper.ExecuteReader(cmdText);
            return reader.ToConsumeTypeList();
        }
    }
}
